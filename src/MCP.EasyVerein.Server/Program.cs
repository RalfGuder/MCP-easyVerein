using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Infrastructure.ApiClient;
using MCP.EasyVerein.Server.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

// Intercept --help before host startup (FR-044)
if (args.Contains("--help") || args.Contains("-h"))
{
    PrintHelp();
    return;
}

var builder = Host.CreateApplicationBuilder(args);

// Remove console logging – stdout is reserved exclusively for MCP JSON-RPC
builder.Logging.ClearProviders();

// Switch mappings: CLI parameters → IConfiguration keys (FR-041, FR-042)
// Priority: CLI (registered last) > Env vars (from CreateApplicationBuilder) > Defaults
var switchMappings = new Dictionary<string, string>
{
    ["--api-key"]     = EasyVereinConfiguration.EnvironmentVariableApiKey,
    ["--api-url"]     = EasyVereinConfiguration.EnvironmentVariableApiUrl,
    ["--api-version"] = EasyVereinConfiguration.EnvironmentVariableApiVersion
};
builder.Configuration.AddCommandLine(args, switchMappings);

// Resolve EasyVereinConfiguration via DI (FR-041–FR-043)
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<EasyVereinConfiguration>>();
    return EasyVereinConfiguration.FromConfiguration(configuration, logger);
});

// HttpClient + API client (FR-002)
builder.Services.AddHttpClient<IEasyVereinApiClient, EasyVereinApiClient>();

// MCP server setup (FR-001, NFR-003)
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
    .WithTools<ContactDetailsTools>();

var app = builder.Build();
await app.RunAsync();
var xx = app.Services.GetService<MemberTools>();

/// <summary>
/// Prints the CLI usage help text with all available parameters, environment variables, and defaults.
/// </summary>
static void PrintHelp()
{
    Console.WriteLine("""
        easyVerein MCP-Server

        Verwendung: MCP.EasyVerein.Server [Optionen]

        Optionen:
          --api-key <value>      API-Schlüssel        (Env: EASYVEREIN_API_KEY)
          --api-url <value>      API-Basis-URL        (Env: EASYVEREIN_API_URL,     Standard: https://easyverein.com/api)
          --api-version <value>  API-Version          (Env: EASYVEREIN_API_VERSION, Standard: v1.7)
          --help, -h             Diese Hilfe anzeigen

        Priorität: CLI-Parameter > Umgebungsvariablen > Standardwerte
        """);
}
