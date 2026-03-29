namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Repräsentiert eine easyVerein API-Version (FR-010 bis FR-015).
/// </summary>
public sealed class ApiVersion
{
    private static readonly IReadOnlyList<string> _supportedVersions = new[] { "v1.4", "v1.5", "v1.6", "v1.7" };
    private const string DefaultVersion = "v1.7";

    public string Version { get; }

    private ApiVersion(string version)
    {
        Version = version;
    }

    public static IReadOnlyList<string> SupportedVersions => _supportedVersions;

    public static ApiVersion Default => new(DefaultVersion);

    public static ApiVersion Create(string version)
    {
        if (string.IsNullOrEmpty(version))
            throw new ArgumentException("API-Version darf nicht leer sein.", nameof(version));

        if (!IsSupported(version))
        {
            var closest = GetClosestVersion(version);
            var supported = string.Join(", ", _supportedVersions);
            var suggestion = closest != null ? $" Nächste unterstützte Version: {closest}." : "";
            throw new ArgumentException(
                $"API-Version '{version}' wird nicht unterstützt.{suggestion} Unterstützte Versionen: {supported}",
                nameof(version));
        }

        return new ApiVersion(version);
    }

    public static bool IsSupported(string version)
    {
        return _supportedVersions.Contains(version);
    }

    public static string? GetClosestVersion(string version)
    {
        if (_supportedVersions.Count == 0)
            return null;

        // Einfache Heuristik: alphabetisch nächste Version
        return _supportedVersions
            .OrderBy(v => Math.Abs(string.Compare(v, version, StringComparison.Ordinal)))
            .FirstOrDefault();
    }

    public override string ToString() => Version;
    public override bool Equals(object? obj) => obj is ApiVersion other && Version == other.Version;
    public override int GetHashCode() => Version.GetHashCode();
}
