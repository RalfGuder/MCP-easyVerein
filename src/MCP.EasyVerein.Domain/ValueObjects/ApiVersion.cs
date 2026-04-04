namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Represents an easyVerein API version (FR-010 to FR-015).
/// </summary>
public sealed class ApiVersion
{
    private static readonly IReadOnlyList<string> _supportedVersions = new[] { "v1.4", "v1.5", "v1.6", "v1.7" };
    private const string DefaultVersion = "v1.7";

    /// <summary>Gets the version string.</summary>
    public string Version { get; }

    private ApiVersion(string version)
    {
        Version = version;
    }

    /// <summary>Gets the list of all supported API versions.</summary>
    public static IReadOnlyList<string> SupportedVersions => _supportedVersions;

    /// <summary>Gets the default API version.</summary>
    public static ApiVersion Default => new(DefaultVersion);

    /// <summary>Creates a new <see cref="ApiVersion"/> after validating that the version is supported.</summary>
    /// <param name="version">The version string to validate and wrap.</param>
    /// <returns>A validated <see cref="ApiVersion"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the version is null, empty, or unsupported.</exception>
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

    /// <summary>Checks whether the given version string is supported.</summary>
    /// <param name="version">The version string to check.</param>
    /// <returns><c>true</c> if the version is supported; otherwise <c>false</c>.</returns>
    public static bool IsSupported(string version)
    {
        return _supportedVersions.Contains(version);
    }

    /// <summary>Returns the closest supported version to the given version string.</summary>
    /// <param name="version">The version string to find the closest match for.</param>
    /// <returns>The closest supported version, or <c>null</c> if no versions are available.</returns>
    public static string? GetClosestVersion(string version)
    {
        if (_supportedVersions.Count == 0)
            return null;

        // Einfache Heuristik: alphabetisch nächste Version
        return _supportedVersions
            .OrderBy(v => Math.Abs(string.Compare(v, version, StringComparison.Ordinal)))
            .FirstOrDefault();
    }

    /// <inheritdoc />
    public override string ToString() => Version;
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ApiVersion other && Version == other.Version;
    /// <inheritdoc />
    public override int GetHashCode() => Version.GetHashCode();
}
