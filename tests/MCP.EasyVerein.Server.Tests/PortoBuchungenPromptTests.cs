using MCP.EasyVerein.Server.Prompts;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for the <see cref="PortoBuchungenPrompt"/> MCP prompt.</summary>
public class PortoBuchungenPromptTests
{
    /// <summary>
    /// Verifies the prompt text mentions all porto-related keywords and the
    /// three fixed classification values (68000 / 2902 / sphere 2) plus the
    /// tools the agent is expected to use.
    /// </summary>
    [Fact]
    public void ReviewPortoBuchungen_DefaultArgs_EnthaeltKernelemente()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen();

        Assert.Contains("Deutsche Post", text);
        Assert.Contains("DHL", text);
        Assert.Contains("Hermes", text);
        Assert.Contains("UPS", text);
        Assert.Contains("GLS", text);
        Assert.Contains("DPD", text);

        Assert.Contains("68000", text);
        Assert.Contains("2902", text);
        Assert.Contains("Sphäre 2", text);

        Assert.Contains("list_bookings", text);
        Assert.Contains("list_billing_accounts", text);
        Assert.Contains("list_booking_projects", text);
        Assert.Contains("update_booking", text);

        Assert.Contains("dryRun=true", text);
    }

    /// <summary>
    /// When explicit start/end dates are provided, they must appear verbatim
    /// in the prompt text so the agent uses them as the list_bookings filter.
    /// </summary>
    [Fact]
    public void ReviewPortoBuchungen_MitZeitraum_EnthaeltDatumswerte()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen(
            dateVon: "2026-04-01",
            dateBis: "2026-04-30");

        Assert.Contains("2026-04-01", text);
        Assert.Contains("2026-04-30", text);
    }

    /// <summary>
    /// With dryRun=false the prompt must explicitly instruct the agent to
    /// execute update_booking rather than just proposing changes.
    /// </summary>
    [Fact]
    public void ReviewPortoBuchungen_DryRunFalse_SchreibModusImText()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen(dryRun: false);

        Assert.Contains("dryRun=false", text);
        Assert.Contains("update_booking", text);
        // default-case marker must not leak when dryRun was explicitly disabled
        Assert.DoesNotContain("dryRun=true", text);
    }
}
