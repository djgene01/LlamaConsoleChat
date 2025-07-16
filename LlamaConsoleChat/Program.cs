using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LlamaConsoleChat
{
    public class Program
    {
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public static async Task Main(string[] args)
        {
            var http = new HttpClient();
            var history = new List<Message>
            {
                new() { Role = "system", Content = "You are a helpful assistant." }
            };

            Console.WriteLine("🤖  Chat with LLaMA 3.2 via Ollama  (type 'exit' to quit)\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("You: ");
                Console.ResetColor();

                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                history.Add(new Message { Role = "user", Content = input });

                var request = new ChatRequest
                {
                    Model = "llama3.2:latest",
                    Messages = history,
                    Stream = true       // tell Ollama we want a streaming response
                };

                // Build a manual HttpRequestMessage so we can ask for streaming
                var url = "http://localhost:11434/api/chat";
                var content = JsonContent.Create(request, options: JsonOpts);
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

                using var response = await http.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead);   // don't buffer the body

                response.EnsureSuccessStatusCode();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("AI: ");

                // Read the chunked JSON objects line-by-line
                var stream = await response.Content.ReadAsStreamAsync();
                var reader = new StreamReader(stream);
                var sb = new StringBuilder();              // build the full assistant reply
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // /api/chat returns raw JSON objects per line.
                    // (If you switch to the OpenAI-compatible /v1 endpoint you’d strip the leading "data: ")
                    var chunk = JsonSerializer.Deserialize<ChatStreamChunk>(line, JsonOpts);
                    if (chunk?.Message?.Content is { } delta)
                    {
                        Console.Write(delta);     // realtime token(s)
                        sb.Append(delta);
                    }

                    if (chunk?.Done == true)
                        break;
                }

                Console.WriteLine("\n");
                Console.ResetColor();

                // Store assistant reply back into history so the model has context next turn
                history.Add(new Message { Role = "assistant", Content = sb.ToString() });
            }
        }
    }

    // ──────────────────── models ────────────────────
    public class Message
    {
        [JsonPropertyName("role")] public string Role { get; set; }
        [JsonPropertyName("content")] public string Content { get; set; }
    }

    public class ChatRequest
    {
        [JsonPropertyName("model")] public string Model { get; set; }
        [JsonPropertyName("messages")] public List<Message> Messages { get; set; } = new();
        [JsonPropertyName("stream")] public bool Stream { get; set; } = true;
    }

    // each JSON line in a streaming response looks like: { "message": { "role": "assistant", "content": "he" } , "done": false , ... }
    public class ChatStreamChunk
    {
        [JsonPropertyName("message")] public Message Message { get; set; }
        [JsonPropertyName("done")] public bool Done { get; set; }
    }
}
