using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Prompts;

/// <summary>
/// MCP prompts that guide the LLM agent through recurring bookkeeping
/// workflows for the Kulturverein Milower Land e.V.
/// </summary>
[McpServerPromptType]
public sealed class PortoBuchungenPrompt
{
    /// <summary>
    /// Returns a natural-language instruction the LLM agent uses to classify
    /// porto-type bookings (Deutsche Post, DHL, Hermes, UPS, GLS, DPD) with
    /// the fixed triple billingAccount 68000 (Porto), sphere 2
    /// (Vermögensverwaltung) and bookingProject 2902 (Büromaterial).
    /// </summary>
    /// <param name="dateVon">Start date (ISO yyyy-MM-dd). Optional; agent defaults to the current calendar month when null.</param>
    /// <param name="dateBis">End date (ISO yyyy-MM-dd). Optional; agent defaults to today when null.</param>
    /// <param name="dryRun">If <c>true</c> (default) the agent only proposes updates; if <c>false</c> it actually calls <c>update_booking</c>.</param>
    /// <returns>The prompt text sent back to the MCP client as a single user message.</returns>
    [McpServerPrompt(Name = "review_porto_buchungen"),
     Description("Findet unklassifizierte Porto-Buchungen (Deutsche Post, DHL, Hermes, UPS, GLS, DPD) in einem Zeitraum und setzt billingAccount 68000 / Sphäre 2 / Kostenstelle 2902. Mit dryRun (Default true) werden nur Vorschläge erstellt.")]
    public static string ReviewPortoBuchungen(
        [Description("Startdatum im Format yyyy-MM-dd. Optional; ohne Angabe verwendet der Agent den Beginn des aktuellen Kalendermonats.")] string? dateVon = null,
        [Description("Enddatum im Format yyyy-MM-dd. Optional; ohne Angabe verwendet der Agent das heutige Datum.")] string? dateBis = null,
        [Description("Bei true (Default) erstellt der Agent nur Vorschläge. Bei false ruft er update_booking direkt auf.")] bool dryRun = true)
    {
        var zeitraumBlock = (dateVon, dateBis) switch
        {
            (null, null) => "Zeitraum: aktueller Kalendermonat (heute rückwärts bis zum 1. des Monats).",
            (not null, null) => $"Zeitraum: ab {dateVon} bis heute.",
            (null, not null) => $"Zeitraum: seit Beginn des aktuellen Monats bis {dateBis}.",
            (not null, not null) => $"Zeitraum: von {dateVon} bis {dateBis}."
        };

        var modusBlock = dryRun
            ? """
              Modus: dryRun=true (Default). Du sollst Aktualisierungen NUR vorschlagen, nicht ausführen.
              Für jede Kandidatin würdest du update_booking mit den drei Zielwerten aufrufen – tu das aber noch nicht.
              Zeige am Ende eine Tabelle mit Buchungs-ID, Empfänger, Betrag, Datum und den drei Zielwerten.
              Frage den Nutzer danach, ob er die Aktualisierungen ausführen lassen möchte.
              """
            : """
              Modus: dryRun=false. Du sollst die Aktualisierungen tatsächlich ausführen.
              Rufe update_booking für jede Kandidatin mit den aufgelösten billingAccountId, bookingProjectId und sphere=2 auf.
              Liste am Ende alle erfolgreich aktualisierten Buchungen mit ihrer ID.
              """;

        return $$"""
        Du bist ein Buchhaltungs-Assistent für den Kulturverein Milower Land e.V. und sollst Porto-Buchungen nach einer festen Regel klassifizieren.

        ## Feste Regel (nicht verhandelbar)

        Für jede als "Porto" erkannte Buchung gilt:
        - billingAccount = Konto 68000 (Porto)
        - sphere = 2 (Vermögensverwaltung); Sphäre 2 ist zwingend
        - bookingProject = Kostenstelle 2902 (Büromaterial)

        ## Porto-Kandidaten

        Eine Buchung ist ein Porto-Kandidat, wenn `receiver` oder `description` eines der folgenden Keywords enthält (case-insensitive):
        - Deutsche Post
        - DHL
        - Hermes
        - UPS
        - GLS
        - DPD

        ## Ausschluss bereits klassifizierter Buchungen

        Überspringe Buchungen, bei denen bereits mindestens eines der folgenden gesetzt ist:
        - `billingAccount` (nicht null und id != null)
        - `bookingProject` (nicht null)
        - `sphere` ist nicht 9 (also schon klassifiziert, 9 = Default/unklassifiziert)

        ## Ablauf

        {{zeitraumBlock}}

        1. Rufe `list_bookings` mit `dateGt` = Startdatum − 1 Tag und `dateLt` = Enddatum + 1 Tag auf.
        2. Filtere das Ergebnis nach Porto-Kandidaten (siehe oben). Zeige am Ende die Anzahl.
        3. Schließe bereits klassifizierte Buchungen aus und nenne die Zahl der verbleibenden Kandidaten.
        4. Löse einmal zu Beginn die beiden IDs auf und merke sie dir:
           - billingAccountId: `list_billing_accounts(search=["Porto"])` → Eintrag mit number=68000 nehmen.
           - bookingProjectId: `list_booking_projects(search=["Büromaterial"])` → Eintrag mit projectCostCentre="2902" nehmen.
        5. Wenn eine ID nicht auffindbar ist, breche mit einer Erklärung ab und frage den Nutzer.

        {{modusBlock}}

        ## Argumente

        - dateVon={{dateVon ?? "(nicht gesetzt)"}}
        - dateBis={{dateBis ?? "(nicht gesetzt)"}}
        - dryRun={{(dryRun ? "true" : "false")}}

        ## Grenzfälle (beim LLM nachfragen)

        - "DHL – Druckdienstleister" oder ähnliche Nicht-Versand-Treffer: frage beim Nutzer zurück, ob die Porto-Regel gelten soll.
        - Projektspezifischer Versand (z. B. `description` enthält "Dorffest-Einladungen"): frage nach einer alternativen Kostenstelle aus Konten.md.
        - Mehrere Treffer für Konto 68000 / Kostenstelle 2902: liste sie dem Nutzer, damit er auswählt.

        Beginne jetzt mit Schritt 1.
        """;
    }
}
