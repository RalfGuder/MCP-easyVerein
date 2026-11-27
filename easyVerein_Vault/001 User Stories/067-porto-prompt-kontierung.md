# US-0067 Porto-Prompt – Kontierung nach aktueller Praxis

**Issue:** folgt mit `/to-factory US-0067`
**Spec (Vertrag für die Software Factory):** [features/US-0067-porto-prompt-kontierung/spec.md](https://github.com/RalfGuder/MCP-easyVerein/blob/main/features/US-0067-porto-prompt-kontierung/spec.md)
**Vorgänger:** [[060-porto-prompt]] (US-0060, PR #95)

## User Story

**Als** Kassenwart,
**möchte ich**, dass der MCP-Prompt `review_porto_buchungen` Porto-Buchungen nach der aktuellen Kontierungspraxis bearbeitet (Konto 68000 und Sphäre 2 an Buchung und Belegposition, Kostenstelle 2902 nur an der Belegposition, Projekt „Kulturverein“ nur an der Buchung), früher falsch kontierte Porto-Buchungen korrigiert und abgeschlossene Geschäftsjahre nicht verändert,
**damit** alle Porto-Buchungen einheitlich und richtig kontiert sind, ohne dass ich jede einzeln anstoße.

## Akzeptanzkriterien

Die verbindlichen Akzeptanzkriterien (AK-1 bis AK-14) und Edge Cases stehen in der Spec. Kurzfassung:

- [ ] Prompt-Text nennt die Zielregel und setzt Kostenstelle/„Büromaterial“ nicht mehr an der Buchung.
- [ ] Kandidaten ohne Konto oder mit 68000 werden nach Zielregel ergänzt/korrigiert; andere Konten werden nur gemeldet.
- [ ] Buchungen ohne Beleg: an der Buchung kontieren, kein Eigenbeleg, Liste „Beleg fehlt“.
- [ ] Neues Argument `gesperrtBis` (Default 31.12. des Vorjahres); bis dahin nur melden, nie ändern.
- [ ] `dateVon`, `dateBis`, `dryRun` bleiben unverändert nutzbar.

## Aufgaben

- Umsetzung durch die Software Factory (Prüfung, Bau, Abnahme, PR).
- Nach Merge: Vault-Story abhaken, Issue-Link eintragen.

## Technische Hinweise

- Betroffen: `src/MCP.EasyVerein.Server/Prompts/PortoBuchungenPrompt.cs` und `PortoBuchungenPromptTests`.
- Regel-Quelle: Memory „Porto-Buchungsregel“ (Stand 2026-11-27), analog Entgeltabrechnung.
