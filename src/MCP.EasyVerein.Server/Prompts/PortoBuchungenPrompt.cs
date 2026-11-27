using System.ComponentModel;
using System.Globalization;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Prompts;

/// <summary>
/// MCP prompts that guide the LLM agent through recurring bookkeeping
/// workflows for the Kulturverein Milower Land e.V.
/// </summary>
[McpServerPromptType]
public sealed class PortoBuchungenPrompt
{
    /// <summary>ISO date format used for all date arguments and the lock date.</summary>
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Returns a natural-language instruction the LLM agent uses to book
    /// porto-type bookings (Deutsche Post, DHL, Hermes, UPS, GLS, DPD) according
    /// to the current practice (US-0067): account 68000 and sphere 2 on booking
    /// and invoice items, cost centre 2902 only on the invoice items, project
    /// "Kulturverein" only on the booking. Bookings up to <paramref name="gesperrtBis"/>
    /// are never changed.
    /// </summary>
    /// <param name="dateVon">Start date (ISO yyyy-MM-dd). Optional; agent defaults to the current calendar month when null.</param>
    /// <param name="dateBis">End date (ISO yyyy-MM-dd). Optional; agent defaults to today when null.</param>
    /// <param name="dryRun">If <c>true</c> (default) the agent only proposes updates; if <c>false</c> it executes them.</param>
    /// <param name="gesperrtBis">Lock date (ISO yyyy-MM-dd). Optional; defaults to 31 December of the previous year.</param>
    /// <returns>The prompt text sent back to the MCP client as a single user message.</returns>
    [McpServerPrompt(Name = "review_porto_buchungen"),
     Description("Kontiert Porto-Buchungen (Deutsche Post, DHL, Hermes, UPS, GLS, DPD) eines Zeitraums nach aktueller Praxis: Konto 68000 und Sphäre 2 an Buchung und Belegposition, Kostenstelle 2902 an der Belegposition, Projekt Kulturverein an der Buchung. Korrigiert ohne Konto oder mit 68000 kontierte Buchungen, meldet abweichend kontierte und Buchungen ohne Beleg. Buchungen bis gesperrtBis (Default 31.12. des Vorjahres) werden nie geändert, nur gemeldet. Mit dryRun (Default true) werden nur Vorschläge erstellt.")]
    public static string ReviewPortoBuchungen(
        [Description("Startdatum im Format yyyy-MM-dd. Optional; ohne Angabe verwendet der Agent den Beginn des aktuellen Kalendermonats.")] string? dateVon = null,
        [Description("Enddatum im Format yyyy-MM-dd. Optional; ohne Angabe verwendet der Agent das heutige Datum.")] string? dateBis = null,
        [Description("Bei true (Default) erstellt der Agent nur Vorschläge. Bei false führt er die Änderungen an Buchungen und Belegpositionen aus.")] bool dryRun = true,
        [Description("Sperrdatum im Format yyyy-MM-dd (abgeschlossenes Geschäftsjahr). Buchungen bis einschließlich zu diesem Datum werden nie geändert. Optional; Default ist der 31.12. des Vorjahres.")] string? gesperrtBis = null)
        => BuildText(dateVon, dateBis, dryRun, gesperrtBis, DateOnly.FromDateTime(DateTime.Today));

    /// <summary>
    /// Builds the prompt text for <see cref="ReviewPortoBuchungen"/> with an explicit
    /// "today" so that the default lock date is deterministic in tests.
    /// </summary>
    /// <param name="dateVon">Optional start date (yyyy-MM-dd).</param>
    /// <param name="dateBis">Optional end date (yyyy-MM-dd).</param>
    /// <param name="dryRun">If <c>true</c> only proposals are requested.</param>
    /// <param name="gesperrtBis">Optional lock date (yyyy-MM-dd); invalid values fall back to the default.</param>
    /// <param name="heute">The current date used to compute the default lock date.</param>
    /// <returns>The prompt text.</returns>
    internal static string BuildText(string? dateVon, string? dateBis, bool dryRun, string? gesperrtBis, DateOnly heute)
    {
        var (von, vonHinweis) = ParseOptionalDate(dateVon, nameof(dateVon), "der Beginn des aktuellen Kalendermonats");
        var (bis, bisHinweis) = ParseOptionalDate(dateBis, nameof(dateBis), "das heutige Datum");
        var (sperrdatum, sperrHinweis) = ResolveLockDate(gesperrtBis, heute);

        var zeitraumBlock = vonHinweis + bisHinweis + BuildPeriodBlock(von, bis);
        var gesamtSperre = IsWholePeriodLocked(bis, sperrdatum, heute)
            ? $"Der gesamte Zeitraum liegt bis einschließlich zum Sperrdatum {Format(sperrdatum)}: Alle Buchungen sind gesperrt. Ändere nichts, melde nur.\n"
            : "";
        var modusBlock = BuildModeBlock(dryRun);
        var sperrdatumText = Format(sperrdatum);

        return $$"""
        Du bist ein Buchhaltungs-Assistent für den Kulturverein Milower Land e.V. und sollst Porto-Buchungen nach einer festen Regel kontieren.

        ## Zielregel Porto (nicht verhandelbar)

        | Feld | Buchung | Belegposition |
        |---|---|---|
        | Konto | 68000 Porto | 68000 Porto |
        | Sphäre | 2 (Vermögensverwaltung) | 2 |
        | Kostenstelle | – | 2902 (Kulturverein – Büromaterial) |
        | Projekt | Kulturverein | – |

        Kurz:
        - Konto 68000 (Porto) an Buchung und Belegposition.
        - Sphäre 2 (Vermögensverwaltung) an Buchung und Belegposition.
        - Kostenstelle 2902 nur an der Belegposition (über update_invoice_item).
        - Projekt "Kulturverein" nur an der Buchung (über update_booking).
        - Setze an der Buchung niemals die Kostenstelle 2902 und niemals das Projekt "Büromaterial"; an den Belegpositionen wird kein Projekt gesetzt.

        Feldnamen:
        - Gelesen (Felder in den Antworten): `billingAccount`, `bookingProject`, `sphere` (Buchung); `billingAccount`, `sphere`, `costCentre` (Belegposition).
        - Geschrieben mit update_booking: `billingAccountId`, `bookingProjectId`, `sphere`.
        - Geschrieben mit update_invoice_item: `billingAccountId`, `sphere`, `costCentre`.

        ## Sperrdatum (abgeschlossene Geschäftsjahre)

        Sperrdatum: {{sperrdatumText}}
        {{sperrHinweis}}{{gesamtSperre}}Buchungen mit Datum bis einschließlich {{sperrdatumText}} und deren Belegpositionen darfst du nie ändern – auch bei dryRun=false nicht.
        Nimm sie stattdessen in die Liste "gesperrt (Geschäftsjahr abgeschlossen)" auf, jeweils mit Buchungs-ID, Datum, Betrag, Empfänger und den Abweichungen von der Zielregel (an Buchung und Positionen).
        Liegt das Sperrdatum nach dem Ende des Zeitraums, sind alle Buchungen gesperrt; dann meldest du nur.

        ## Porto-Kandidaten

        Eine Buchung ist ein Porto-Kandidat, wenn `receiver` oder `description` eines der folgenden Stichwörter enthält (ohne Groß-/Kleinschreibung):
        - Deutsche Post
        - DHL
        - Hermes
        - UPS
        - GLS
        - DPD

        ## Behandlung je Kandidat

        Prüfe für jeden nicht gesperrten Kandidaten Buchung und alle Positionen aller verknüpften Belege (`relatedInvoice` der Buchung; Positionen über `list_invoice_items(relatedInvoice=<Beleg-ID>)`).
        `billingAccount` und `bookingProject` stehen in den Antworten als ID; vergleiche sie mit den in Schritt 1 aufgelösten IDs (Konto 68000 = aufgelöste Konto-ID).
        - Buchung ohne Konto oder mit Konto 68000: setze fehlende oder abweichende Werte an Buchung und Positionen nach der Zielregel (Konto, Sphäre und Projekt an der Buchung; Konto, Sphäre und Kostenstelle an den Positionen).
        - Buchung mit einem anderen Konto als 68000: nichts ändern, Buchung in die Liste "abweichend kontiert" aufnehmen.
        - Position mit einem anderen Konto als 68000: diese Position nicht ändern, Buchung in die Liste "abweichend kontiert" aufnehmen; die übrigen Positionen und die Buchung werden trotzdem nach Zielregel behandelt.
        - Buchung und Positionen sind bereits vollständig richtig nach Zielregel: überspringen und mitzählen.
        - Buchung ohne verknüpften Beleg (`relatedInvoice` leer): nur Konto, Sphäre und Projekt an der Buchung setzen. Lege keinen Eigenbeleg an (create_receipt nicht aufrufen) und nimm die Buchung in die Liste "Beleg fehlt" mit Datum, Betrag und Empfänger auf.
        - Ist ein Beleg finalisiert, sind Konto, Sphäre und Kostenstelle der Positionen trotzdem änderbar. Den Belegkopf (Datum, Nummer, Betrag) und die Verknüpfung zwischen Buchung und Beleg änderst du nie.

        ## Ablauf

        {{zeitraumBlock}}

        1. Löse einmal zu Beginn die beiden IDs auf und merke sie dir:
           - Konto-ID: `list_billing_accounts(search=["Porto"])` → Eintrag mit number=68000 nehmen.
           - Projekt-ID: `list_booking_projects(search=["Kulturverein"])` → Eintrag mit dem Namen "Kulturverein" nehmen.
           Ist eine ID nicht auffindbar oder mehrdeutig, brich ab und frage den Nutzer (bei mehreren Treffern liste sie zur Auswahl auf).
           Ist Konto 68000 im Kontenplan des Vereins nicht freigeschaltet, brich ebenfalls ab und frage den Nutzer (Buchungen mit nicht freigeschaltetem Konto lassen sich nicht bearbeiten).
        2. Rufe `list_bookings` mit `dateGt` = Startdatum − 1 Tag, `dateLt` = Enddatum + 1 Tag und `limit` = 100 auf.
           Solange `hasMore` true ist, rufe die nächste Seite (`page` + 1) ab; die Buchungen stehen in `items`.
        3. Filtere nach Porto-Kandidaten und nenne die Anzahl.
        4. Trenne gesperrte Buchungen (Datum bis einschließlich {{sperrdatumText}}) ab.
        5. Lade für alle Kandidaten – auch die gesperrten – die Belegpositionen (`list_invoice_items`, bei Bedarf `get_invoice`).
           Bei gesperrten Kandidaten nur lesend: ermittle die Abweichungen von der Zielregel an Buchung und Positionen für die Liste "gesperrt (Geschäftsjahr abgeschlossen)" und ändere nichts.
           Bestimme für die übrigen Kandidaten die Behandlung (siehe oben).
        6. Nenne am Ende die Zahlen: Kandidaten, gesperrt, zu ändern, abweichend kontiert, bereits richtig (übersprungen), Beleg fehlt.

        {{modusBlock}}

        ## Listen am Ende

        - "abweichend kontiert": Buchungs-ID, Datum, Betrag, Empfänger, vorhandenes Konto (an Buchung bzw. Position).
        - "Beleg fehlt": Buchungs-ID, Datum, Betrag und Empfänger.
        - "gesperrt (Geschäftsjahr abgeschlossen)": Buchungs-ID, Datum, Betrag, Empfänger und Abweichungen von der Zielregel.

        ## Argumente

        - dateVon={{dateVon ?? "(nicht gesetzt)"}}
        - dateBis={{dateBis ?? "(nicht gesetzt)"}}
        - dryRun={{(dryRun ? "true" : "false")}}
        - gesperrtBis={{sperrdatumText}}

        ## Grenzfälle (beim Nutzer nachfragen)

        - "DHL – Druckdienstleister" oder ähnliche Nicht-Versand-Treffer: frage beim Nutzer zurück, ob die Porto-Regel gelten soll.
        - Projektspezifischer Versand (z. B. `description` enthält "Dorffest-Einladungen"): frage nach, ob die Zielregel gelten soll oder eine andere Kostenstelle bzw. ein anderes Projekt aus Konten.md.

        Beginne jetzt mit Schritt 1.
        """;
    }

    /// <summary>Builds the period line from the validated start and end dates.</summary>
    /// <param name="von">Validated start date or <c>null</c>.</param>
    /// <param name="bis">Validated end date or <c>null</c>.</param>
    /// <returns>The period line for the prompt text.</returns>
    private static string BuildPeriodBlock(DateOnly? von, DateOnly? bis) => (von, bis) switch
    {
        (null, null) => "Zeitraum: aktueller Kalendermonat (heute rückwärts bis zum 1. des Monats).",
        ({ } v, null) => $"Zeitraum: ab {Format(v)} bis heute.",
        (null, { } b) => $"Zeitraum: seit Beginn des aktuellen Monats bis {Format(b)}.",
        ({ } v, { } b) => $"Zeitraum: von {Format(v)} bis {Format(b)}."
    };

    /// <summary>Builds the mode block for proposal-only (dry run) or execution mode.</summary>
    /// <param name="dryRun">If <c>true</c> only proposals are requested.</param>
    /// <returns>The mode block for the prompt text.</returns>
    private static string BuildModeBlock(bool dryRun) => dryRun
        ? """
          Modus: dryRun=true (Default). Du sollst Änderungen NUR vorschlagen, nicht ausführen.
          Rufe weder update_booking noch update_invoice_item auf.
          Zeige eine Tabelle mit Buchungs-ID, Empfänger, Betrag, Datum, geplante Änderungen an der Buchung und geplante Änderungen an den Belegpositionen (Positions-ID, Feld alt → neu).
          Zeige danach die Listen (siehe unten).
          Frage den Nutzer danach, ob er die Änderungen ausführen lassen möchte.
          """
        : """
          Modus: dryRun=false. Du sollst die Änderungen tatsächlich ausführen.
          Rufe für jede zu korrigierende Buchung update_booking und für jede zu korrigierende Belegposition update_invoice_item mit den aufgelösten IDs auf (nur die fehlenden oder abweichenden Felder).
          Liste am Ende alle geänderten Buchungen und Belegpositionen jeweils mit ID und den geänderten Feldern auf, danach die Listen (siehe unten).
          """;

    /// <summary>
    /// Returns <c>true</c> if the effective end of the period (end date or today) is on or before the lock date.
    /// </summary>
    /// <param name="bis">Validated end date or <c>null</c> (then today is the end).</param>
    /// <param name="sperrdatum">Effective lock date.</param>
    /// <param name="heute">The current date.</param>
    /// <returns>Whether all bookings of the period are locked.</returns>
    private static bool IsWholePeriodLocked(DateOnly? bis, DateOnly sperrdatum, DateOnly heute)
        => (bis ?? heute) <= sperrdatum;

    /// <summary>
    /// Validates an optional date argument. Missing values yield <c>null</c> without notice;
    /// invalid values yield <c>null</c> plus a notice line naming the fallback.
    /// </summary>
    /// <param name="value">The raw argument value.</param>
    /// <param name="name">The argument name used in the notice.</param>
    /// <param name="fallback">Human-readable description of the fallback used instead.</param>
    /// <returns>The parsed date (or <c>null</c>) and an optional notice line.</returns>
    private static (DateOnly? Datum, string Hinweis) ParseOptionalDate(string? value, string name, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (null, "");

        if (TryParseIso(value, out var parsed))
            return (parsed, "");

        return (null,
            $"Hinweis: Der übergebene Wert {name}=\"{value}\" ist ungültig (erwartet yyyy-MM-dd). Stattdessen gilt {fallback}. Weise den Nutzer zu Beginn darauf hin.\n");
    }

    /// <summary>
    /// Determines the effective lock date. A valid <paramref name="gesperrtBis"/> is used as-is;
    /// a missing or invalid value falls back to 31 December of the previous year.
    /// </summary>
    /// <param name="gesperrtBis">The lock date argument (may be null or invalid).</param>
    /// <param name="heute">The current date.</param>
    /// <returns>The effective lock date and an optional notice line for invalid input.</returns>
    private static (DateOnly Sperrdatum, string Hinweis) ResolveLockDate(string? gesperrtBis, DateOnly heute)
    {
        var standard = EndOfPreviousYear(heute);

        if (string.IsNullOrWhiteSpace(gesperrtBis))
            return (standard, "");

        if (TryParseIso(gesperrtBis, out var parsed))
            return (parsed, "");

        return (standard,
            $"Hinweis: Der übergebene Wert gesperrtBis=\"{gesperrtBis}\" ist ungültig (erwartet yyyy-MM-dd). Stattdessen gilt der 31.12. des Vorjahres ({Format(standard)}). Weise den Nutzer zu Beginn darauf hin.\n");
    }

    /// <summary>Returns 31 December of the year before <paramref name="heute"/>.</summary>
    /// <param name="heute">The current date.</param>
    /// <returns>The last day of the previous year.</returns>
    private static DateOnly EndOfPreviousYear(DateOnly heute) => new(heute.Year - 1, 12, 31);

    /// <summary>Parses a trimmed value strictly in <see cref="DateFormat"/>.</summary>
    /// <param name="value">The raw value.</param>
    /// <param name="date">The parsed date on success.</param>
    /// <returns><c>true</c> if the value is a valid ISO date.</returns>
    private static bool TryParseIso(string value, out DateOnly date)
        => DateOnly.TryParseExact(value.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

    /// <summary>Formats a date in <see cref="DateFormat"/>.</summary>
    /// <param name="date">The date to format.</param>
    /// <returns>The ISO date string.</returns>
    private static string Format(DateOnly date) => date.ToString(DateFormat, CultureInfo.InvariantCulture);
}
