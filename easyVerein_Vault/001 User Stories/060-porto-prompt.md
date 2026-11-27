# US-0060 MCP-Prompt `review_porto_buchungen`

**Issue:** [#94](https://github.com/RalfGuder/MCP-easyVerein/issues/94) (geschlossen 2026-11-27, umgesetzt mit PR #95)

## User Story

**Als** Kassenwart,
**möchte ich** einen MCP-Prompt, der einen LLM-Agenten anleitet, in einem vorgegebenen Zeitraum alle noch unklassifizierten Porto-Buchungen (Deutsche Post, DHL, Hermes, UPS, GLS, DPD) zu finden und mit der festen Regel billingAccount=68000 / sphere=2 / bookingProject=2902 zu aktualisieren,
**damit** ich die wiederkehrende manuelle Klassifikation nicht für jede Buchung einzeln anstoßen muss.

## Akzeptanzkriterien

- [x] Neue Klasse `PortoBuchungenPrompt` mit `[McpServerPromptType]` und einer Methode `ReviewPortoBuchungen` mit `[McpServerPrompt(Name = "review_porto_buchungen")]`.
- [x] Argumente: `dateVon` (string?, ISO-Date), `dateBis` (string?, ISO-Date), `dryRun` (bool, Default `true`).
- [x] Der Prompt-Text enthält erkennbar alle Porto-Keywords (`Deutsche Post`, `DHL`, `Hermes`, `UPS`, `GLS`, `DPD`).
- [x] Der Prompt-Text enthält Konto 68000, Kostenstelle 2902, Sphäre 2.
- [x] Der Prompt-Text verweist auf die zu verwendenden Tools `list_bookings`, `list_billing_accounts`, `list_booking_projects`, `update_booking`.
- [x] Die Zeitraum-Argumente werden im Prompt-Text eingebettet, wenn übergeben.
- [x] `dryRun=false` erzeugt einen Text, der explizit zum Ausführen von `update_booking` auffordert; `dryRun=true` (Default) erzeugt nur Vorschläge + Rückfrage.
- [x] Registrierung in `Program.cs`: `.WithPrompts<PortoBuchungenPrompt>()`.
- [x] 3 neue xUnit-Tests in `PortoBuchungenPromptTests.cs` decken die drei Haupt-Fälle ab.
- [x] Gesamtzahl Tests steigt auf 152, Suite bleibt grün.

## Technische Hinweise

- ModelContextProtocol v1.2.0 unterstützt Prompts (verifiziert in `D:\packages\modelcontextprotocol(.core)\1.2.0`).
- Methoden-Rückgabetyp `string` → SDK konvertiert zu `PromptMessage` mit Text-Content (siehe `ModelContextProtocol.Core.xml` Z. 12252ff.).
- Die Regel stammt aus Memory `feedback_porto_buchungsregel.md` (Session 2026-04-22).
- Nachfrage-Verhalten bei Mehrdeutigkeit obliegt dem LLM-Agenten.

## Out of Scope

- Rechnungs-Anpassung (`relatedInvoice`) — separate Story.
- Weitere Buchungsregeln (Strom, Wasser, Mitgliedsbeiträge) — folgen später über das gleiche Muster.
