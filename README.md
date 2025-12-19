
This project is an end-to-end demonstration of an AI-driven, microservice-based tool execution system built using .NET Minimal APIs, a lightweight MCP (Model Context Protocol–style) JSON-RPC gateway, and an n8n workflow powered by an AI agent. At its core, the system exposes business capabilities (such as arithmetic operations) as HTTP endpoints inside a .NET service called MathService, where each endpoint is treated as a “tool” that can be dynamically discovered and invoked by an AI workflow without hardcoding. The MathService uses a custom `/__tools` endpoint that automatically inspects all registered Minimal API routes and generates a structured tool definition containing the HTTP method, route, human-readable description, and parameter schema. Tool descriptions and parameters are intentionally defined inline using `.WithMetadata()` on each endpoint, keeping the system simple and avoiding extra files, attributes, or reflection-heavy approaches. These tool definitions are consumed by an MCP Gateway, which acts as a JSON-RPC server implementing `tools/list` and `tools/call`. The gateway aggregates tools from one or more microservices and routes tool execution requests to the correct service using Dapr service invocation, returning normalized JSON-RPC responses. On top of this infrastructure sits an n8n workflow that begins with a chat trigger, fetches the available tools dynamically from the MCP Gateway, and passes them into an AI Agent node. The AI agent is strictly constrained by system instructions to only select tools that actually exist, map user input to the correct arguments, and return a valid native JSON object that conforms exactly to the JSON-RPC `tools/call` schema. A JavaScript Code node then safely validates, parses, and executes the AI-generated tool call, handling malformed outputs, missing tools, or runtime errors gracefully by returning a consistent fallback message instead of breaking the workflow. Finally, the result is normalized and sent back to the user via the chat response node. The entire architecture is intentionally modular, allowing new tools to be added simply by defining new Minimal API endpoints with metadata, without touching the gateway or the n8n workflow logic. This makes the system highly extensible, AI-friendly, and production-ready for scenarios where large language models need to reason over available capabilities and safely execute real backend actions. The project serves as a practical reference for building AI-integrated microservices, automated tool discovery, and robust agent execution pipelines using modern .NET, JSON-RPC, and low-code orchestration.


how to run:

pre req:
dotnet 9
wsl 
dockers-n8n 


first run MathService program.cs:
command:
dapr run --app-id math-service --app-port 5092 --dapr-http-port 3500 dotnet run

second run McpGateway program.cs
command:
dotnet run

n8n run command:
docker run -it --rm   -p 5678:5678   -v ~/.n8n:/home/node/.n8n   n8nio/n8n


http://host.docker.internal:5258/




