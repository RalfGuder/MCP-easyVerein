using System.Reflection;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>
/// Checks that README, Dockerfile, .mcp.json.example and the ADR match the code (US-0068).
/// </summary>
public class DocumentationTests
{
    /// <summary>The repository root directory (contains MCP.EasyVerein.sln).</summary>
    private static readonly string RepoRoot = FindRepoRoot();

    /// <summary>Walks up from the test binary directory until the solution file is found.</summary>
    /// <returns>The absolute path of the repository root.</returns>
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "MCP.EasyVerein.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Repository root not found.");
    }

    /// <summary>Reads a file relative to the repository root.</summary>
    /// <param name="relativePath">Path relative to the repository root.</param>
    /// <returns>The file content.</returns>
    private static string Read(string relativePath) => File.ReadAllText(Path.Combine(RepoRoot, relativePath));

    /// <summary>Converts a PascalCase method name to the snake_case tool name used by the MCP SDK.</summary>
    /// <param name="name">The method name.</param>
    /// <returns>The snake_case name.</returns>
    private static string ToSnakeCase(string name) =>
        Regex.Replace(name, "(?<!^)([A-Z])", "_$1").ToLowerInvariant();

    /// <summary>Returns the tool class names registered via <c>.WithTools&lt;T&gt;()</c> in Program.cs.</summary>
    /// <returns>The registered tool class names.</returns>
    private static List<string> RegisteredToolClasses() =>
        Regex.Matches(Read("src/MCP.EasyVerein.Server/Program.cs"), @"\.WithTools<(\w+)>\(\)")
            .Select(m => m.Groups[1].Value).ToList();

    /// <summary>Every tool of every class registered in Program.cs appears in the README endpoint table (AK-8).</summary>
    [Fact]
    public void Readme_EndpointTable_ListsAllRegisteredTools()
    {
        var readme = Read("README.md");
        var serverAssembly = typeof(MCP.EasyVerein.Server.Tools.ToolJson).Assembly;
        var classes = RegisteredToolClasses();
        Assert.NotEmpty(classes);

        foreach (var className in classes)
        {
            var type = serverAssembly.GetTypes().Single(t => t.Name == className);
            var tools = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Select(m => (Method: m, Attr: m.GetCustomAttribute<McpServerToolAttribute>()))
                .Where(x => x.Attr != null)
                .Select(x => x.Attr!.Name ?? ToSnakeCase(x.Method.Name))
                .ToList();
            Assert.NotEmpty(tools);
            foreach (var tool in tools)
                Assert.True(readme.Contains($"`{tool}`"), $"README endpoint table misses tool '{tool}' ({className}).");
        }
    }

    /// <summary>The README documents configuration, API versions, start examples and project structure (AK-1 to AK-7).</summary>
    [Fact]
    public void Readme_ContainsConfigurationStartAndStructureSections()
    {
        var readme = Read("README.md");
        foreach (var expected in new[]
        {
            "## Konfiguration", "--api-key", "--api-url", "--api-version", "--help",
            "EASYVEREIN_API_KEY", "EASYVEREIN_API_URL", "EASYVEREIN_API_VERSION",
            "https://easyverein.com/api", "Bearer ",
            "v1.4", "v1.5", "v1.6", "v1.7", "v2.0",
            "docker run -i --rm -e EASYVEREIN_API_KEY", "\"command\": \"docker\"",
            "dotnet run --project src/MCP.EasyVerein.Server", ".mcp.json.example",
            "## Projektstruktur", "Domain", "Application", "Infrastructure", "Server",
            "docs/adr/0001-csharp-dotnet8.md",
            "## Antwortformat", "## Branch-Strategie", "## Lizenz"
        })
            Assert.True(readme.Contains(expected), $"README misses '{expected}'.");

        Assert.DoesNotContain("EASYVEREIN_API_TOKEN", readme);
        Assert.DoesNotContain("EASYVEREIN_BASE_URL", readme);
    }

    /// <summary>The Dockerfile sets the variables the code reads (AK-9).</summary>
    [Fact]
    public void Dockerfile_UsesVariablesReadByTheCode()
    {
        var dockerfile = Read("Dockerfile");
        Assert.Contains("EASYVEREIN_API_KEY", dockerfile);
        Assert.Contains("EASYVEREIN_API_URL", dockerfile);
        Assert.Contains("EASYVEREIN_API_VERSION", dockerfile);
        Assert.DoesNotContain("EASYVEREIN_API_TOKEN", dockerfile);
        Assert.DoesNotContain("EASYVEREIN_BASE_URL", dockerfile);
    }

    /// <summary>The example configuration contains the key without a Bearer prefix (AK-10).</summary>
    [Fact]
    public void McpJsonExample_KeyHasNoBearerPrefix()
    {
        var example = Read(".mcp.json.example");
        Assert.Contains("\"EASYVEREIN_API_KEY\": \"<your-easyverein-api-token-here>\"", example);
        Assert.DoesNotContain("Bearer", example);
    }

    /// <summary>The ADR documents the decision with all required sections (AK-11).</summary>
    [Fact]
    public void Adr_CSharpDotnet8_HasRequiredSections()
    {
        var adr = Read("docs/adr/0001-csharp-dotnet8.md");
        foreach (var expected in new[]
        {
            "## Status", "## Kontext", "## Entscheidung", "## Betrachtete Alternativen", "## Konsequenzen",
            ".NET 8", "LTS", "Windows", "Linux", "macOS", "Clean Architecture", "ModelContextProtocol", "Docker"
        })
            Assert.True(adr.Contains(expected), $"ADR misses '{expected}'.");
    }
}
