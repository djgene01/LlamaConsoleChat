LlamaConsoleChat 🐑💬
A minimal .NET console application that streams responses from a local Ollama-hosted LLaMA 3.2 model in real time.
Type your message, watch tokens arrive live, and keep the entire chat history in memory for context.

✨ Features
Feature	Description
Real-time streaming	Uses Ollama’s stream=true mode and prints tokens as soon as they arrive.
Chat history	Maintains the full conversation so the model has context every turn.
Zero external dependencies	Only needs System.Net.Http and System.Text.Json (built-in to .NET).
Works everywhere .NET runs	Windows, Linux, macOS—including WSL and Docker containers.

🛠️ Prerequisites
Tool	Notes
.NET SDK 8.0 +	https://dotnet.microsoft.com/download
Ollama 0.1.34 +	https://ollama.com (or `curl -fsSL https://ollama.com/install.sh
LLaMA 3.2 model	ollama pull llama3.2:latest

🚀 Getting Started
bash
Copy
Edit
# 1. Clone the repo
git clone https://github.com/djgene01/LlamaConsoleChat.git
cd LlamaConsoleChat

# 2. Restore & build
dotnet build

# 3. Start the model (in another terminal)
ollama run llama3.2

# 4. Run the chat app
dotnet run
Sample Session
text
Copy
Edit
🤖  Chat with LLaMA 3.2 via Ollama  (type 'exit' to quit)

You: Hi! What's the weather like on Mars?
AI: While Mars does not have weather in the Earthly sense, it experiences large
dust storms, thin-atmosphere temperature swings, and seasonal CO₂ ice at the
poles...

You: exit
Tokens appear as soon as they’re generated—no long waits for the full reply.

⚙️ Configuration
Setting	Where	Default	Description
Ollama URL	url variable in Program.cs	http://localhost:11434	Change if Ollama runs on a different host/port.
Model name	Model property in ChatRequest	llama3.2:latest	Swap in any model you’ve pulled (e.g., llama3 or phi3).
System prompt	First element in history	“You are a helpful assistant.”	Adjust tone, persona, or domain behavior.

For environment-based or file-based configuration, wrap the settings in appsettings.json or use environment variables and IConfiguration.
