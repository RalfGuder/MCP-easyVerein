using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Infrastructure.ApiClient;
using MCP.EasyVerein.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// Konfiguration (FR-008, FR-013)
var config = EasyVereinConfiguration.FromEnvironment();
builder.Services.AddSingleton(config);

// HttpClient + API-Client (FR-002)
builder.Services.AddHttpClient<IEasyVereinApiClient, EasyVereinApiClient>();

// MCP-Server (FR-001, NFR-003)
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "easyVerein MCP-Server",
            Version = "1.0.0"
        };
    })
    .WithStdioServerTransport()
    .WithTools<MemberTools>()
    .WithTools<InvoiceTools>()
    .WithTools<EventTools>()
    .WithTools<ContactTools>();

var app = builder.Build();
await app.RunAsync();
