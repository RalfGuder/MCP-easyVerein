# US-0067: Porto-Prompt – Kontierung nach aktueller Praxis

**Status:** Planned
**Erstellt:** 2026-11-27

## Abhängigkeiten
- US-0060 (MCP-Prompt `review_porto_buchungen`, umgesetzt mit PR #95) – dieser Prompt wird angepasst
- Bestehende Tools: `list_bookings`, `update_booking`, `get_invoice`, `list_invoice_items`, `update_invoice_item`, `list_billing_accounts`, `list_booking_projects`

## Ausgangslage
Der MCP-Prompt `review_porto_buchungen` setzt die Kostenstelle 2902 über das Projekt „Büromaterial“ an der **Buchung**. In der Praxis des Vereins liegt die Kostenstelle aber an der **Belegposition**, das Projekt an der Buchung ist „Kulturverein“. Außerdem überspringt der Prompt jede Buchung, bei der schon irgendein Wert gesetzt ist, korrigiert also nie halb oder falsch kontierte Porto-Buchungen, und er kennt keine abgeschlossenen Geschäftsjahre.

## Zielregel Porto
| Feld | Buchung | Belegposition |
|---|---|---|
| Konto | 68000 Porto | 68000 Porto |
| Sphäre | 2 (Vermögensverwaltung) | 2 |
| Kostenstelle | – | 2902 (Kulturverein – Büromaterial) |
| Projekt | Kulturverein | – |

## User Stories
- Als Kassenwart möchte ich Porto-Buchungen eines Zeitraums mit einem Prompt vollständig nach der Zielregel kontieren lassen (Buchung und Belegpositionen), damit ich das nicht für jede Buchung einzeln anstoßen muss.
- Als Kassenwart möchte ich, dass früher falsch oder unvollständig kontierte Porto-Buchungen korrigiert werden, damit alle Porto-Buchungen einheitlich kontiert sind.
- Als Kassenwart möchte ich, dass Buchungen in abgeschlossenen Geschäftsjahren nie verändert, sondern nur gemeldet werden, damit abgeschlossene Jahre unverändert bleiben.
- Als Kassenwart möchte ich eine Liste der Porto-Buchungen ohne Beleg, damit ich die Originalbelege nachreichen kann.

## Außerhalb des Umfangs
- Anlegen von Eigenbelegen (`create_receipt`) für Buchungen ohne Beleg
- Andere Buchungsregeln (Strom, Wasser, Entgeltabrechnung …) als Prompts
- Änderung von Belegkopf-Feldern (Datum, Nummer, Betrag) oder Verknüpfungen zwischen Buchung und Beleg
- Löschen oder Umbenennen des Projekts „Büromaterial“
- Änderungen an Tools; nur der Prompt-Text und seine Argumente ändern sich (Ausnahme: Argument-Binding der Prompts, siehe Product Decisions)

## Akzeptanzkriterien
Alle Kriterien beziehen sich auf den Text, den der Prompt `review_porto_buchungen` liefert (MCP `prompts/get`), bzw. auf seine Beschreibung in `prompts/list`.

- [ ] AK-1: Angenommen der Prompt wird ohne Argumente abgerufen, wenn der Text erzeugt wird, dann nennt er die Zielregel: Konto 68000 und Sphäre 2 an Buchung und Belegposition, Kostenstelle 2902 nur an der Belegposition, Projekt „Kulturverein“ nur an der Buchung.
- [ ] AK-2: Angenommen der Prompt wird abgerufen, dann fordert der Text an keiner Stelle auf, die Kostenstelle 2902 oder das Projekt „Büromaterial“ an der Buchung zu setzen.
- [ ] AK-3: Angenommen der Prompt wird abgerufen, dann nennt der Text die Porto-Stichwörter Deutsche Post, DHL, Hermes, UPS, GLS und DPD (Suche in Empfänger und Verwendungszweck, ohne Groß-/Kleinschreibung).
- [ ] AK-4: Angenommen der Prompt wird abgerufen, dann beschreibt der Text die Behandlung je Kandidat: ohne Konto oder mit Konto 68000 → fehlende oder abweichende Werte an Buchung und Positionen nach Zielregel setzen; mit einem anderen Konto → nicht ändern, in Liste „abweichend kontiert“; vollständig richtig → überspringen und mitzählen.
- [ ] AK-5: Angenommen der Prompt wird abgerufen, dann verlangt der Text für Buchungen ohne verknüpften Beleg: Konto, Sphäre und Projekt an der Buchung setzen, keinen Eigenbeleg anlegen und die Buchung in einer Liste „Beleg fehlt“ mit Datum, Betrag und Empfänger ausgeben.
- [ ] AK-6: Angenommen der Prompt wird ohne `gesperrtBis` abgerufen, dann nennt der Text als Sperrdatum den 31.12. des Vorjahres (bezogen auf das aktuelle Datum) im Format yyyy-MM-dd.
- [ ] AK-7: Angenommen der Prompt wird mit `gesperrtBis=2024-12-31` abgerufen, dann nennt der Text 2024-12-31 als Sperrdatum.
- [ ] AK-8: Angenommen der Prompt wird abgerufen, dann verbietet der Text Änderungen an Buchungen mit Datum bis einschließlich Sperrdatum und an deren Belegpositionen, auch bei `dryRun=false`, und verlangt eine Liste „gesperrt (Geschäftsjahr abgeschlossen)“ mit den Abweichungen von der Zielregel.
- [ ] AK-9: Angenommen `dryRun` ist nicht gesetzt oder `true`, dann fordert der Text nur Vorschläge (Tabelle mit Buchung, Empfänger, Betrag, Datum, geplanten Änderungen an Buchung und Positionen) und eine Rückfrage, und keine Änderungen.
- [ ] AK-10: Angenommen `dryRun=false`, dann fordert der Text auf, die Änderungen an Buchungen und Belegpositionen auszuführen und am Ende die geänderten Buchungen und Positionen mit ID aufzulisten.
- [ ] AK-11: Angenommen `dateVon` und/oder `dateBis` sind gesetzt, dann stehen diese Werte im Text als Zeitraum; ohne Angabe gilt wie bisher der aktuelle Kalendermonat.
- [ ] AK-12: Angenommen der Prompt wird abgerufen, dann verlangt der Text, die IDs für Konto 68000 und das Projekt „Kulturverein“ zu Beginn aufzulösen und bei nicht auffindbarer oder mehrdeutiger ID abzubrechen und nachzufragen.
- [ ] AK-13: Angenommen `prompts/list` wird abgerufen, dann beschreibt der Prompt `review_porto_buchungen` die neue Regel (Kostenstelle an der Belegposition, Projekt Kulturverein) und das Argument `gesperrtBis`.
- [ ] AK-14: Angenommen die bestehenden Grenzfälle, dann bleiben sie im Text erhalten: Nicht-Versand-Treffer (z. B. „DHL – Druckdienstleister“) und projektspezifischer Versand (z. B. „Dorffest-Einladungen“) führen zu einer Rückfrage.

## Edge Cases
- EC-1: Ein Porto-Beleg hat mehrere Positionen, von denen eine bereits ein anderes Konto als 68000 trägt → diese Position nicht ändern, Buchung in „abweichend kontiert“ melden.
- EC-2: Eine Buchung ist mit mehreren Belegen verknüpft → alle Positionen aller verknüpften Belege nach Zielregel behandeln.
- EC-3: Der Beleg ist finalisiert → Konto, Sphäre und Kostenstelle der Positionen sind trotzdem änderbar; Belegkopf wird nicht angefasst.
- EC-4: `gesperrtBis` liegt nach `dateBis` → alle Buchungen des Zeitraums sind gesperrt; der Prompt meldet nur.
- EC-5: `gesperrtBis` hat ein ungültiges Format → der Text weist darauf hin und nimmt den 31.12. des Vorjahres (oder der Prompt meldet einen Fehler; Entscheidung in `design.md`, aber nie stillschweigend ohne Sperre).
- EC-6: Konto 68000 ist im Kontenplan des Vereins nicht freigeschaltet → abbrechen und nachfragen (Buchungen mit nicht freigeschaltetem Konto lassen sich in easyVerein nicht bearbeiten).

## Technische Anforderungen
- Keine Aufrufe der echten easyVerein-API in Tests; der Prompt ist reiner Text und wird über xUnit-Tests auf seinen Inhalt geprüft.
- Bestehende Prompt-Argumente `dateVon`, `dateBis`, `dryRun` bleiben mit gleicher Bedeutung erhalten (non-breaking); `gesperrtBis` kommt optional hinzu.

## Offene Fragen
- Keine

## Product Decisions
- Kostenstelle nur an der Belegposition, Projekt „Kulturverein“ nur an der Buchung; Konto und Sphäre an beiden: einheitlich mit der Entgeltabrechnungs-Regel (2026-11-27)
- Buchungen ohne Beleg werden an der Buchung kontiert und gemeldet, aber ohne Eigenbeleg: Regel „Buchung ohne Beleg erst klären“ (2026-11-27)
- Bereits mit 68000 kontierte Buchungen werden nachgezogen, andere Konten nicht angefasst: alte Prompt-Ergebnisse korrigieren, bewusste Abweichungen respektieren (2026-11-27)
- Sperrdatum `gesperrtBis` mit Default 31.12. des Vorjahres: abgeschlossene Geschäftsjahre (aktuell bis 2025) dürfen nicht verändert werden; im Januar kann ein früheres Datum übergeben werden (2026-11-27)
- Nachtrag nach Umsetzung: Prompts erhalten eigene Serializer-Optionen (`PromptJson`) mit einem toleranten Bool-Konverter, weil MCP Prompt-Argumente als Zeichenketten überträgt (`"dryRun": "false"`) und der Aufruf sonst mit -32603 scheitert. Die Registrierung in `Program.cs` ändert sich dafür; die Tools bleiben unverändert. Begründung in `design.md` (2026-11-27)
- Nachtrag aus dem Review: `dateVon` und `dateBis` werden wie `gesperrtBis` auf yyyy-MM-dd geprüft; ein ungültiger Wert führt zu einem Hinweis im Text statt stiller Übernahme (2026-11-27)
