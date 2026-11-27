# Design: US-0067 Porto-Prompt – Kontierung nach aktueller Praxis

## Ansatz
Der MCP-Prompt `review_porto_buchungen` (`src/MCP.EasyVerein.Server/Prompts/PortoBuchungenPrompt.cs`) bleibt reiner Text. Es ändern sich nur der Text, die Beschreibung und die Argumente; Tools bleiben unverändert.

## Bausteine
- **Zielregel als Tabelle** am Anfang des Textes (Konto 68000 und Sphäre 2 an Buchung und Position, Kostenstelle 2902 nur an der Position per `update_invoice_item`, Projekt „Kulturverein“ nur an der Buchung per `update_booking`). Ausdrückliches Verbot, Kostenstelle 2902 oder Projekt „Büromaterial“ an der Buchung zu setzen.
- **Behandlung je Kandidat:** ohne Konto/68000 → nachziehen; anderes Konto (Buchung oder Position) → nicht ändern, Liste „abweichend kontiert“; vollständig richtig → überspringen und zählen; ohne Beleg → nur Buchung kontieren, kein Eigenbeleg, Liste „Beleg fehlt“. Mehrere Belege und finalisierte Belege sind abgedeckt (EC-1 bis EC-3).
- **Sperrdatum `gesperrtBis`** (neues optionales Argument): Default 31.12. des Vorjahres. Gültige Werte (`yyyy-MM-dd`) werden übernommen. **Entscheidung EC-5:** Ein ungültiger Wert erzeugt keinen Fehler, sondern einen Hinweis im Text und den Rückfall auf den 31.12. des Vorjahres – damit gilt nie stillschweigend keine Sperre. EC-4 (Sperrdatum nach Zeitraumende) steht als Satz im Text.
- **Testbarkeit:** `BuildText(..., DateOnly heute)` nimmt das heutige Datum als Parameter, damit der Default in Tests deterministisch ist. Die MCP-Methode ruft es mit `DateTime.Today` auf.
- **Argument-Bindung:** MCP überträgt Prompt-Argumente als Strings (`"dryRun": "false"`). Der SDK-Standard kann das nicht in `bool` umwandeln und antwortet mit Fehler -32603. Deshalb registriert `Program.cs` den Prompt mit `PromptJson.SerializerOptions` (MCP-Defaults plus toleranter Bool-Converter für `true`/`false` als JSON-Bool oder String). Sonst wäre AK-10 über `prompts/get` nicht erreichbar.

## Tests
xUnit-Tests in `PortoBuchungenPromptTests` (je AK mindestens ein Test, Default-Sperrdatum über mehrere Daten, ungültige Werte) und `PromptJsonTests` (Bool-Converter). Keine API-Aufrufe.
