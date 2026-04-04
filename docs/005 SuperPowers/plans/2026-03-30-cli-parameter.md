# CLI-Konfigurationsparameter (US-0007) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Den easyVerein MCP-Server so erweitern, dass `--api-key`, `--api-url` und `--api-version` als Kommandozeilenparameter akzeptiert werden (Priorität CLI > Env-Var > Default), mit `--help`-Ausgabe und Warnungen bei fehlenden Werten.

**Architecture:** `Microsoft.Extensions.Configuration.CommandLine` wird via Switch-Mappings in `Program.cs` integriert. `EasyVereinConfiguration` erhält eine neue `FromConfiguration(IConfiguration, ILogger)` Factory-Methode. `Program.cs` löst `EasyVereinConfiguration` über den DI-Container auf.

**Tech Stack:** .NET 8, `Microsoft.Extensions.Configuration.Abstractions` 8.0.0, `Microsoft.Extensions.Logging.Abstractions` 8.0.0, `Microsoft.Extensions.Configuration` 8.0.0 (nur Tests), xUnit, Moq

---

## Dateiübersicht

| Datei | Aktion | Inhalt |
|-------|--------|--------|
| `src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs` | Modify | Umbenennung + `FromConfiguration()` |
| `src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj` | Modify | Neue Packages |
| `src/MCP.EasyVerein.Server/Program.cs` | Modify | `--help`, Switch-Mappings, DI-Registrierung |
| `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs` | Modify | Bestehende Tests + neue TDD-Tests |
| `tests/MCP.EasyVerein.Application.Tests/MCP.EasyVerein.Application.Tests.csproj` | Modify | Neue Test-Packages |
| `tests/MCP.EasyVerein.Application.Tests/GlobalUsings.cs` | Modify | Neue Using-Direktiven |

---

## Task 1: Feature-Branch erstellen

**Files:** —

- [ ] **Schritt 1: Feature-Branch anlegen**

```bash
git checkout -b feature/US-0007-cli-parameter
```

- [ ] **Schritt 2: Branch verifizieren**

```bash
git branch --show-current
```

Expected output: `feature/US-0007-cli-parameter`

---

## Task 2: Packages hinzufügen

**Files:**
- Modify: `src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj`
- Modify: `tests/MCP.EasyVerein.Application.Tests/MCP.EasyVerein.Application.Tests.csproj`
- Modify: `tests/MCP.EasyVerein.Application.Tests/GlobalUsings.cs`

- [ ] **Schritt 1: Application.csproj anpassen**

Inhalt von `src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj` nach der Änderung:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MCP.EasyVerein.Domain\MCP.EasyVerein.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

- [ ] **Schritt 2: Application.Tests.csproj anpassen**

Inhalt von `tests/MCP.EasyVerein.Application.Tests/MCP.EasyVerein.Application.Tests.csproj` nach der Änderung:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.6.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MCP.EasyVerein.Application\MCP.EasyVerein.Application.csproj" />
    <ProjectReference Include="..\..\src\MCP.EasyVerein.Domain\MCP.EasyVerein.Domain.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Schritt 3: GlobalUsings.cs im Test-Projekt erweitern**

Inhalt von `tests/MCP.EasyVerein.Application.Tests/GlobalUsings.cs` nach der Änderung:

```csharp
global using Xunit;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Moq;
```

- [ ] **Schritt 4: Packages wiederherstellen**

```bash
dotnet restore
```

Expected: keine Fehler, alle Packages werden heruntergeladen.

- [ ] **Schritt 5: Build prüfen**

```bash
dotnet build
```

Expected: `Build succeeded.`

---

## Task 3: EasyVereinConfiguration umbenennen – Tests zuerst anpassen (Red)

**Files:**
- Modify: `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs`

- [ ] **Schritt 1: Bestehende Tests auf neue Namen umstellen**

Ersetze den gesamten Inhalt von `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs` mit:

```csharp
using MCP.EasyVerein.Application.Configuration;

namespace MCP.EasyVerein.Application.Tests;

public class EasyVereinConfigurationTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var config = new EasyVereinConfiguration();

        Assert.Equal("https://easyverein.com/api", config.ApiUrl);
        Assert.Equal("v1.7", config.ApiVersion);
        Assert.Equal(string.Empty, config.ApiKey);
    }

    [Fact]
    public void GetVersionedBaseUrl_ReturnsCorrectUrl()
    {
        var config = new EasyVereinConfiguration
        {
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };

        Assert.Equal("https://easyverein.com/api/v1.7", config.GetVersionedBaseUrl());
    }

    [Fact]
    public void GetVersionedBaseUrl_WithOverride_UsesOverride()
    {
        var config = new EasyVereinConfiguration { ApiVersion = "v1.7" };
        Assert.Equal("https://easyverein.com/api/v1.6", config.GetVersionedBaseUrl("v1.6"));
    }

    [Fact]
    public void GetVersionedBaseUrl_WithInvalidOverride_Throws()
    {
        var config = new EasyVereinConfiguration();
        Assert.Throws<ArgumentException>(() => config.GetVersionedBaseUrl("v99"));
    }

    [Fact]
    public void FromEnvironment_WithoutApiKey_ThrowsInvalidOperation()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
        Assert.Throws<InvalidOperationException>(() => EasyVereinConfiguration.FromEnvironment());
    }

    [Fact]
    public void FromEnvironment_WithApiKey_ReturnsConfig()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v1.7");

        try
        {
            var config = EasyVereinConfiguration.FromEnvironment();
            Assert.Equal("test-token", config.ApiKey);
            Assert.Equal("v1.7", config.ApiVersion);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    [Fact]
    public void FromEnvironment_WithInvalidVersion_Throws()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v99");

        try
        {
            Assert.Throws<ArgumentException>(() => EasyVereinConfiguration.FromEnvironment());
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }
}
```

- [ ] **Schritt 2: Build schlägt fehl – erwartete Fehler bestätigen**

```bash
dotnet build tests/MCP.EasyVerein.Application.Tests
```

Expected: Compile errors wegen `ApiUrl`, `ApiKey`, `EnvironmentVariableApiKey` (noch nicht vorhanden).

---

## Task 4: EasyVereinConfiguration umbenennen – Implementierung (Green)

**Files:**
- Modify: `src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs`

- [ ] **Schritt 1: EasyVereinConfiguration.cs vollständig ersetzen**

```csharp
using MCP.EasyVerein.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MCP.EasyVerein.Application.Configuration;

/// <summary>
/// Konfiguration für den easyVerein MCP-Server (FR-008, FR-013, FR-041–FR-044).
/// </summary>
public class EasyVereinConfiguration
{
    public const string EnvironmentVariableApiKey     = "EASYVEREIN_API_KEY";
    public const string EnvironmentVariableApiUrl     = "EASYVEREIN_API_URL";
    public const string EnvironmentVariableApiVersion = "EASYVEREIN_API_VERSION";

    public const string DefaultApiUrl = "https://easyverein.com/api";

    public string ApiKey     { get; set; } = string.Empty;
    public string ApiUrl     { get; set; } = DefaultApiUrl;
    public string ApiVersion { get; set; } = Domain.ValueObjects.ApiVersion.Default.Version;

    /// <summary>
    /// Erstellt Konfiguration aus Umgebungsvariablen (FR-008). Legacy-Methode.
    /// </summary>
    public static EasyVereinConfiguration FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable(EnvironmentVariableApiKey);
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException(
                $"Umgebungsvariable '{EnvironmentVariableApiKey}' ist nicht gesetzt. " +
                "Bitte setzen Sie Ihren easyVerein API-Schlüssel.");

        var config = new EasyVereinConfiguration
        {
            ApiKey     = apiKey,
            ApiUrl     = Environment.GetEnvironmentVariable(EnvironmentVariableApiUrl) ?? DefaultApiUrl,
            ApiVersion = Environment.GetEnvironmentVariable(EnvironmentVariableApiVersion)
                         ?? Domain.ValueObjects.ApiVersion.Default.Version
        };

        // API-Version validieren (FR-015)
        Domain.ValueObjects.ApiVersion.Create(config.ApiVersion);

        return config;
    }

    /// <summary>
    /// Erstellt Konfiguration aus IConfiguration (FR-041–FR-043).
    /// CLI-Parameter überschreiben Env-Vars (Priorität via IConfiguration-Provider-Reihenfolge).
    /// Fehlende Werte lösen eine Warnung aus; es wird der Standardwert verwendet.
    /// </summary>
    public static EasyVereinConfiguration FromConfiguration(IConfiguration configuration, ILogger logger)
    {
        var apiVersion = Resolve(
            configuration, EnvironmentVariableApiVersion,
            Domain.ValueObjects.ApiVersion.Default.Version,
            "api-version", logger);

        // Ungültige API-Version führt zu einer Exception (gewollt, FR-015)
        Domain.ValueObjects.ApiVersion.Create(apiVersion);

        return new EasyVereinConfiguration
        {
            ApiKey     = Resolve(configuration, EnvironmentVariableApiKey, string.Empty, "api-key", logger),
            ApiUrl     = Resolve(configuration, EnvironmentVariableApiUrl, DefaultApiUrl, "api-url", logger),
            ApiVersion = apiVersion
        };
    }

    private static string Resolve(
        IConfiguration configuration, string key, string defaultValue,
        string paramName, ILogger logger)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            logger.LogWarning(
                "Konfigurationswert '{Key}' nicht gesetzt (weder --{Param} noch Umgebungsvariable). " +
                "Standardwert wird verwendet: '{Default}'",
                key, paramName, defaultValue);
            return defaultValue;
        }
        return value;
    }

    /// <summary>
    /// Gibt die vollständige Basis-URL inkl. Version zurück.
    /// </summary>
    public string GetVersionedBaseUrl(string? versionOverride = null)
    {
        var version = versionOverride ?? ApiVersion;
        if (versionOverride != null)
            Domain.ValueObjects.ApiVersion.Create(versionOverride);

        return $"{ApiUrl.TrimEnd('/')}/{version}";
    }
}
```

- [ ] **Schritt 2: Build und bestehende Tests ausführen**

```bash
dotnet test tests/MCP.EasyVerein.Application.Tests
```

Expected:
```
Bestanden!   : Fehler: 0, erfolgreich: 7, übersprungen: 0
```

---

## Task 5: `FromConfiguration`-Tests schreiben (Red)

**Files:**
- Modify: `tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs`

- [ ] **Schritt 1: Neue Tests am Ende der Klasse `EasyVereinConfigurationTests` ergänzen**

Ersetze den gesamten Inhalt der Datei mit der vollständigen Version inkl. neuer Tests:

```csharp
using MCP.EasyVerein.Application.Configuration;

namespace MCP.EasyVerein.Application.Tests;

public class EasyVereinConfigurationTests
{
    // --- Bestehende Tests (unverändert) ---

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var config = new EasyVereinConfiguration();

        Assert.Equal("https://easyverein.com/api", config.ApiUrl);
        Assert.Equal("v1.7", config.ApiVersion);
        Assert.Equal(string.Empty, config.ApiKey);
    }

    [Fact]
    public void GetVersionedBaseUrl_ReturnsCorrectUrl()
    {
        var config = new EasyVereinConfiguration
        {
            ApiUrl = "https://easyverein.com/api",
            ApiVersion = "v1.7"
        };

        Assert.Equal("https://easyverein.com/api/v1.7", config.GetVersionedBaseUrl());
    }

    [Fact]
    public void GetVersionedBaseUrl_WithOverride_UsesOverride()
    {
        var config = new EasyVereinConfiguration { ApiVersion = "v1.7" };
        Assert.Equal("https://easyverein.com/api/v1.6", config.GetVersionedBaseUrl("v1.6"));
    }

    [Fact]
    public void GetVersionedBaseUrl_WithInvalidOverride_Throws()
    {
        var config = new EasyVereinConfiguration();
        Assert.Throws<ArgumentException>(() => config.GetVersionedBaseUrl("v99"));
    }

    [Fact]
    public void FromEnvironment_WithoutApiKey_ThrowsInvalidOperation()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
        Assert.Throws<InvalidOperationException>(() => EasyVereinConfiguration.FromEnvironment());
    }

    [Fact]
    public void FromEnvironment_WithApiKey_ReturnsConfig()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v1.7");

        try
        {
            var config = EasyVereinConfiguration.FromEnvironment();
            Assert.Equal("test-token", config.ApiKey);
            Assert.Equal("v1.7", config.ApiVersion);
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    [Fact]
    public void FromEnvironment_WithInvalidVersion_Throws()
    {
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, "test-token");
        Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, "v99");

        try
        {
            Assert.Throws<ArgumentException>(() => EasyVereinConfiguration.FromEnvironment());
        }
        finally
        {
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiKey, null);
            Environment.SetEnvironmentVariable(EasyVereinConfiguration.EnvironmentVariableApiVersion, null);
        }
    }

    // --- Neue Tests: FromConfiguration (TDD) ---

    [Fact]
    public void FromConfiguration_WithAllValues_ReturnsCorrectConfig()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey]     = "my-api-key",
                [EasyVereinConfiguration.EnvironmentVariableApiUrl]     = "https://custom.api",
                [EasyVereinConfiguration.EnvironmentVariableApiVersion] = "v1.6"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("my-api-key", config.ApiKey);
        Assert.Equal("https://custom.api", config.ApiUrl);
        Assert.Equal("v1.6", config.ApiVersion);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiKey_UsesEmptyDefaultAndLogsWarning()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var loggerMock = new Mock<ILogger>();
        loggerMock.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);

        var config = EasyVereinConfiguration.FromConfiguration(configuration, loggerMock.Object);

        Assert.Equal(string.Empty, config.ApiKey);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("EASYVEREIN_API_KEY")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiUrl_UsesDefaultUrl()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "test-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal(EasyVereinConfiguration.DefaultApiUrl, config.ApiUrl);
    }

    [Fact]
    public void FromConfiguration_WithMissingApiVersion_UsesDefaultVersion()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "test-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("v1.7", config.ApiVersion);
    }

    [Fact]
    public void FromConfiguration_WithInvalidApiVersion_Throws()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey]     = "test-key",
                [EasyVereinConfiguration.EnvironmentVariableApiVersion] = "v99"
            })
            .Build();

        Assert.Throws<ArgumentException>(() =>
            EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance));
    }

    [Fact]
    public void FromConfiguration_LaterProviderWins_CliOverridesEnvVar()
    {
        // Simuliert: Env-Var als erste Quelle, CLI als zweite (höhere Priorität)
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "env-key"
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [EasyVereinConfiguration.EnvironmentVariableApiKey] = "cli-key"
            })
            .Build();

        var config = EasyVereinConfiguration.FromConfiguration(configuration, NullLogger.Instance);

        Assert.Equal("cli-key", config.ApiKey);
    }
}
```

- [ ] **Schritt 2: Tests ausführen – neue Tests müssen grün sein (FromConfiguration ist bereits implementiert)**

```bash
dotnet test tests/MCP.EasyVerein.Application.Tests --verbosity normal
```

Expected:
```
Bestanden!   : Fehler: 0, erfolgreich: 13, übersprungen: 0
```

- [ ] **Schritt 3: Commit**

```bash
git add src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj
git add src/MCP.EasyVerein.Application/Configuration/EasyVereinConfiguration.cs
git add tests/MCP.EasyVerein.Application.Tests/MCP.EasyVerein.Application.Tests.csproj
git add tests/MCP.EasyVerein.Application.Tests/GlobalUsings.cs
git add tests/MCP.EasyVerein.Application.Tests/EasyVereinConfigurationTests.cs
git commit -m "feat: EasyVereinConfiguration – CLI-Parameter via FromConfiguration (US-0007)

- ApiToken → ApiKey, BaseUrl → ApiUrl (Breaking: Env-Var-Namen angepasst)
- FromConfiguration(IConfiguration, ILogger) hinzugefügt
- Priorität CLI > Env-Var > Default via IConfiguration-Provider-Reihenfolge
- Warnungen bei fehlenden Werten via ILogger
- Verlinkt mit GitHub Issue #12"
```

---

## Task 6: Program.cs auf CLI-Parameter umstellen

**Files:**
- Modify: `src/MCP.EasyVerein.Server/Program.cs`

- [ ] **Schritt 1: Program.cs vollständig ersetzen**

```csharp
using MCP.EasyVerein.Application.Configuration;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Infrastructure.ApiClient;
using MCP.EasyVerein.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

// --help vor Host-Start abfangen (FR-044)
if (args.Contains("--help") || args.Contains("-h"))
{
    PrintHelp();
    return;
}

var builder = Host.CreateApplicationBuilder(args);

// Switch-Mappings: CLI-Parameter → IConfiguration-Keys (FR-041, FR-042)
// Priorität: CLI (zuletzt) > Env-Var (von CreateApplicationBuilder) > Defaults
var switchMappings = new Dictionary<string, string>
{
    ["--api-key"]     = EasyVereinConfiguration.EnvironmentVariableApiKey,
    ["--api-url"]     = EasyVereinConfiguration.EnvironmentVariableApiUrl,
    ["--api-version"] = EasyVereinConfiguration.EnvironmentVariableApiVersion
};
builder.Configuration.AddCommandLine(args, switchMappings);

// EasyVereinConfiguration via DI auflösen (FR-041–FR-043)
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<EasyVereinConfiguration>>();
    return EasyVereinConfiguration.FromConfiguration(configuration, logger);
});

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
```

- [ ] **Schritt 2: Prüfen ob `IConfiguration` im Server-Projekt verfügbar ist**

```bash
dotnet build src/MCP.EasyVerein.Server
```

Expected: `Build succeeded.` — `IConfiguration` ist via `Microsoft.Extensions.Hosting` transitiv verfügbar.

Falls Fehler: `Microsoft.Extensions.Configuration.Abstractions` explizit in `MCP.EasyVerein.Server.csproj` ergänzen:
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
```

- [ ] **Schritt 3: Alle Tests ausführen**

```bash
dotnet test
```

Expected:
```
Bestanden!   : Fehler: 0, erfolgreich: 13, übersprungen: 0, gesamt: 13
```
(Domain: 10, Application: 13, Infrastructure: 8 — Server.Tests hat noch keine Tests)

- [ ] **Schritt 4: Commit**

```bash
git add src/MCP.EasyVerein.Server/Program.cs
git commit -m "feat: Program.cs – CLI-Parameter via Switch-Mappings und --help (US-0007)

- --help Ausgabe mit allen Parametern, Env-Var-Namen und Defaults
- Switch-Mappings: --api-key/--api-url/--api-version → EASYVEREIN_*
- EasyVereinConfiguration über DI mit ILogger aufgelöst
- Verlinkt mit GitHub Issue #12"
```

---

## Task 7: EasyVereinApiClient auf neue Property-Namen anpassen

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs`

- [ ] **Schritt 1: EasyVereinApiClient.cs lesen und auf ApiKey/ApiUrl prüfen**

Prüfe in `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` ob `ApiToken` oder `BaseUrl` referenziert werden. Falls ja: ersetzen.

```bash
grep -n "ApiToken\|BaseUrl" src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
```

- [ ] **Schritt 2: Alle Vorkommen von `ApiToken` → `ApiKey` und `BaseUrl` → `ApiUrl` ersetzen**

Suche nach allen verbleibenden Referenzen im gesamten Projekt:

```bash
grep -rn "ApiToken\|\.BaseUrl\b\|EnvironmentVariableToken\|EnvironmentVariableBaseUrl" src/ tests/
```

Jedes gefundene Vorkommen ersetzen:
- `\.ApiToken` → `.ApiKey`
- `\.BaseUrl` → `.ApiUrl`
- `EnvironmentVariableToken` → `EnvironmentVariableApiKey`
- `EnvironmentVariableBaseUrl` → `EnvironmentVariableApiUrl`

- [ ] **Schritt 3: Alle Tests ausführen**

```bash
dotnet test
```

Expected: alle Tests grün, keine Fehler.

- [ ] **Schritt 4: Commit (nur wenn Änderungen nötig waren)**

```bash
git add src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
git commit -m "refactor: ApiToken → ApiKey, BaseUrl → ApiUrl in Infrastructure (US-0007)"
```

---

## Task 8: Push, PR und Merge

**Files:** —

- [ ] **Schritt 1: Branch pushen**

```bash
git push -u origin feature/US-0007-cli-parameter
```

- [ ] **Schritt 2: Pull Request erstellen**

```bash
gh pr create \
  --title "feat: CLI-Konfigurationsparameter --api-key/--api-url/--api-version (US-0007)" \
  --body "$(cat <<'EOF'
## Summary

- Neue CLI-Parameter `--api-key`, `--api-url`, `--api-version` beim Serverstart
- Priorität: CLI > Env-Var > Default via `IConfiguration`-Provider-Reihenfolge
- Warnungen bei fehlenden Werten via `ILogger`, kein Abbruch
- `--help`-Ausgabe mit allen Parametern, Env-Vars und Standardwerten
- Breaking: Env-Var `EASYVEREIN_API_TOKEN` → `EASYVEREIN_API_KEY`, `EASYVEREIN_BASE_URL` → `EASYVEREIN_API_URL`

## Test Plan

- [ ] `dotnet test` – alle 13 Tests grün
- [ ] `MCP.EasyVerein.Server --help` gibt Hilfetext aus
- [ ] `MCP.EasyVerein.Server --api-key mykey` startet Server mit gesetztem Key
- [ ] Start ohne Parameter zeigt Warnungen, Server läuft weiter

Verlinkt mit GitHub Issue #12
EOF
)"
```

- [ ] **Schritt 3: PR mergen**

```bash
gh pr merge --merge --delete-branch
```

- [ ] **Schritt 4: main lokal aktualisieren**

```bash
git checkout main && git pull
```
