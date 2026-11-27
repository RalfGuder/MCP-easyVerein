using System.ComponentModel;
using System.Reflection;
using MCP.EasyVerein.Server.Prompts;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for the <see cref="PortoBuchungenPrompt"/> MCP prompt (US-0060, US-0067).</summary>
public class PortoBuchungenPromptTests
{
    /// <summary>Fixed "today" used to make the default lock date deterministic.</summary>
    private static readonly DateOnly Heute = new(2026, 11, 27);

    /// <summary>Builds the prompt text with the fixed <see cref="Heute"/> date.</summary>
    /// <param name="dateVon">Optional start date.</param>
    /// <param name="dateBis">Optional end date.</param>
    /// <param name="dryRun">Dry-run flag.</param>
    /// <param name="gesperrtBis">Optional lock date.</param>
    /// <returns>The generated prompt text.</returns>
    private static string Text(string? dateVon = null, string? dateBis = null, bool dryRun = true, string? gesperrtBis = null)
        => PortoBuchungenPrompt.BuildText(dateVon, dateBis, dryRun, gesperrtBis, Heute);

    /// <summary>The MCP prompt method itself still works without arguments.</summary>
    [Fact]
    public void ReviewPortoBuchungen_DefaultArgs_EnthaeltKernelemente()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen();

        Assert.Contains("68000", text);
        Assert.Contains("2902", text);
        Assert.Contains("Sphäre 2", text);
        Assert.Contains("list_bookings", text);
        Assert.Contains("list_billing_accounts", text);
        Assert.Contains("list_booking_projects", text);
        Assert.Contains("update_booking", text);
        Assert.Contains("list_invoice_items", text);
        Assert.Contains("update_invoice_item", text);
        Assert.Contains("dryRun=true", text);
    }

    /// <summary>AK-1: the target rule names account, sphere, cost centre and project at the right level.</summary>
    [Fact]
    public void Zielregel_WirdGenannt()
    {
        var text = Text();

        Assert.Contains("Konto 68000 (Porto) an Buchung und Belegposition", text);
        Assert.Contains("Sphäre 2 (Vermögensverwaltung) an Buchung und Belegposition", text);
        Assert.Contains("Kostenstelle 2902 nur an der Belegposition", text);
        Assert.Contains("Projekt \"Kulturverein\" nur an der Buchung", text);
    }

    /// <summary>AK-2: the old rule (cost centre / project "Büromaterial" on the booking) is gone.</summary>
    [Fact]
    public void AlteRegel_NichtMehrImText()
    {
        var text = Text(dryRun: false);

        Assert.DoesNotContain("Büromaterial\")", text);
        Assert.DoesNotContain("bookingProject = Kostenstelle 2902", text);
        Assert.DoesNotContain("search=[\"Büromaterial\"]", text);
        Assert.Contains("Setze an der Buchung niemals die Kostenstelle 2902 und niemals das Projekt \"Büromaterial\"", text);
    }

    /// <summary>AK-3: porto keywords, searched in receiver and description, case-insensitive.</summary>
    [Fact]
    public void PortoStichwoerter_WerdenGenannt()
    {
        var text = Text();

        foreach (var keyword in new[] { "Deutsche Post", "DHL", "Hermes", "UPS", "GLS", "DPD" })
            Assert.Contains($"- {keyword}", text);
        Assert.Contains("`receiver` oder `description`", text);
        Assert.Contains("ohne Groß-/Kleinschreibung", text);
    }

    /// <summary>AK-4 and EC-1: handling per candidate (correct, report deviating, skip).</summary>
    [Fact]
    public void BehandlungJeKandidat_WirdBeschrieben()
    {
        var text = Text();

        Assert.Contains("ohne Konto oder mit Konto 68000", text);
        Assert.Contains("abweichend kontiert", text);
        Assert.Contains("vollständig richtig", text);
        Assert.Contains("überspringen und mitzählen", text);
        Assert.Contains("Position mit einem anderen Konto als 68000", text);
    }

    /// <summary>AK-5: bookings without receipt are booked, no own receipt, listed in "Beleg fehlt".</summary>
    [Fact]
    public void OhneBeleg_ListeBelegFehlt_KeinEigenbeleg()
    {
        var text = Text();

        Assert.Contains("Beleg fehlt", text);
        Assert.Contains("keinen Eigenbeleg", text);
        Assert.Contains("create_receipt", text);
        Assert.Contains("Datum, Betrag und Empfänger", text);
    }

    /// <summary>EC-2/EC-3: multiple receipts and finalised receipts.</summary>
    [Fact]
    public void MehrereBelege_UndFinalisierteBelege_WerdenBehandelt()
    {
        var text = Text();

        Assert.Contains("alle Positionen aller verknüpften Belege", text);
        Assert.Contains("finalisiert", text);
        Assert.Contains("Belegkopf", text);
    }

    /// <summary>AK-6: without gesperrtBis the lock date is 31.12. of the previous year.</summary>
    [Theory]
    [InlineData(2026, 11, 27, "2025-12-31")]
    [InlineData(2026, 1, 5, "2025-12-31")]
    [InlineData(2027, 12, 31, "2026-12-31")]
    public void GesperrtBis_Default_IstEndeVorjahr(int y, int m, int d, string erwartet)
    {
        var text = PortoBuchungenPrompt.BuildText(null, null, true, null, new DateOnly(y, m, d));

        Assert.Contains($"Sperrdatum: {erwartet}", text);
        Assert.Contains($"gesperrtBis={erwartet}", text);
    }

    /// <summary>The MCP prompt method uses the real current date for the default lock date.</summary>
    [Fact]
    public void ReviewPortoBuchungen_OhneGesperrtBis_NutztAktuellesVorjahr()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen();

        Assert.Contains($"Sperrdatum: {DateTime.Today.Year - 1}-12-31", text);
    }

    /// <summary>AK-7: an explicit gesperrtBis is used as lock date.</summary>
    [Fact]
    public void GesperrtBis_Explizit_WirdUebernommen()
    {
        var text = PortoBuchungenPrompt.ReviewPortoBuchungen(gesperrtBis: "2024-12-31");

        Assert.Contains("Sperrdatum: 2024-12-31", text);
        Assert.DoesNotContain("ungültig", text);
    }

    /// <summary>EC-5: an invalid gesperrtBis is mentioned and replaced by the default; the lock stays active.</summary>
    [Theory]
    [InlineData("31.12.2024")]
    [InlineData("2024-13-01")]
    [InlineData("morgen")]
    public void GesperrtBis_Ungueltig_FaelltAufDefaultZurueck(string ungueltig)
    {
        var text = Text(gesperrtBis: ungueltig);

        Assert.Contains($"\"{ungueltig}\" ist ungültig", text);
        Assert.Contains("Sperrdatum: 2025-12-31", text);
    }

    /// <summary>AK-8: no changes up to the lock date, also not with dryRun=false; locked list required.</summary>
    [Fact]
    public void Sperre_GiltAuchBeiDryRunFalse()
    {
        var text = Text(dryRun: false, gesperrtBis: "2024-12-31");

        Assert.Contains("bis einschließlich 2024-12-31", text);
        Assert.Contains("auch bei dryRun=false", text);
        Assert.Contains("gesperrt (Geschäftsjahr abgeschlossen)", text);
        Assert.Contains("Abweichungen von der Zielregel", text);
    }

    /// <summary>AK-9: dryRun default/true only proposes and asks back.</summary>
    [Fact]
    public void DryRunTrue_NurVorschlaege()
    {
        foreach (var text in new[] { Text(), Text(dryRun: true) })
        {
            Assert.Contains("dryRun=true", text);
            Assert.Contains("NUR vorschlagen", text);
            Assert.Contains("Buchungs-ID, Empfänger, Betrag, Datum", text);
            Assert.Contains("geplante Änderungen an der Buchung", text);
            Assert.Contains("geplante Änderungen an den Belegpositionen", text);
            Assert.Contains("Frage den Nutzer", text);
            Assert.DoesNotContain("dryRun=false. Du sollst", text);
        }
    }

    /// <summary>AK-10: dryRun=false executes and lists changed bookings and items with ID.</summary>
    [Fact]
    public void DryRunFalse_FuehrtAus()
    {
        var text = Text(dryRun: false);

        Assert.Contains("dryRun=false", text);
        Assert.DoesNotContain("dryRun=true", text);
        Assert.Contains("tatsächlich ausführen", text);
        Assert.Contains("geänderten Buchungen und Belegpositionen jeweils mit ID", text);
    }

    /// <summary>AK-11: date range values appear; without them the current month applies.</summary>
    [Fact]
    public void Zeitraum_WieBisher()
    {
        Assert.Contains("Zeitraum: von 2026-04-01 bis 2026-04-30.", Text("2026-04-01", "2026-04-30"));
        Assert.Contains("Zeitraum: ab 2026-04-01 bis heute.", Text(dateVon: "2026-04-01"));
        Assert.Contains("bis 2026-04-30.", Text(dateBis: "2026-04-30"));
        Assert.Contains("Zeitraum: aktueller Kalendermonat", Text());
    }

    /// <summary>AK-12 and EC-6: IDs resolved at the start; abort on missing/ambiguous/not enabled.</summary>
    [Fact]
    public void IdsAufloesen_SonstAbbrechen()
    {
        var text = Text();

        Assert.Contains("list_billing_accounts", text);
        Assert.Contains("list_booking_projects(search=[\"Kulturverein\"])", text);
        Assert.Contains("nicht auffindbar oder mehrdeutig", text);
        Assert.Contains("brich ab und frage den Nutzer", text);
        Assert.Contains("nicht freigeschaltet", text);
    }

    /// <summary>AK-14: existing edge cases remain.</summary>
    [Fact]
    public void Grenzfaelle_BleibenErhalten()
    {
        var text = Text();

        Assert.Contains("DHL – Druckdienstleister", text);
        Assert.Contains("Dorffest-Einladungen", text);
    }

    /// <summary>AK-13: the prompts/list description mentions the new rule and gesperrtBis.</summary>
    [Fact]
    public void Beschreibung_NenntNeueRegelUndGesperrtBis()
    {
        var method = typeof(PortoBuchungenPrompt).GetMethod(nameof(PortoBuchungenPrompt.ReviewPortoBuchungen))!;
        var description = method.GetCustomAttribute<DescriptionAttribute>()!.Description;

        Assert.Contains("Kostenstelle 2902 an der Belegposition", description);
        Assert.Contains("Projekt Kulturverein", description);
        Assert.Contains("gesperrtBis", description);

        var param = method.GetParameters().Single(p => p.Name == "gesperrtBis");
        Assert.True(param.IsOptional);
        Assert.Null(param.DefaultValue);
        Assert.Contains("31.12.", param.GetCustomAttribute<DescriptionAttribute>()!.Description);
    }

    /// <summary>Technical requirement: existing arguments keep name, type and default.</summary>
    [Fact]
    public void BestehendeArgumente_Unveraendert()
    {
        var parameters = typeof(PortoBuchungenPrompt).GetMethod(nameof(PortoBuchungenPrompt.ReviewPortoBuchungen))!.GetParameters();

        Assert.Equal(new[] { "dateVon", "dateBis", "dryRun", "gesperrtBis" }, parameters.Select(p => p.Name));
        Assert.Equal(true, parameters[2].DefaultValue);
        Assert.Equal(typeof(string), parameters[3].ParameterType);
    }
}
