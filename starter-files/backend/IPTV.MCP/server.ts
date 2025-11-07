import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";

// Initialize MCP Server
const server = new Server(
  {
    name: "iptv-mcp-server",
    version: "0.1.0",
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Test tool - will be replaced later
server.registerTool(
  {
    name: "hello",
    description: "A test tool to verify MCP server is working",
    inputSchema: {
      type: "object",
      properties: {},
    },
  },
  async () => {
    return {
      content: [
        {
          type: "text",
          text: "Hello from IPTV MCP Server! 🎬",
        },
      ],
    };
  }
);

async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("IPTV MCP Server running on stdio");
}

main().catch((error) => {
  console.error("Fatal error:", error);
  process.exit(1);
});
