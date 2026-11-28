# US-0070: API-Version pro Lese-Aufruf wählen

**Status:** Planned
**Erstellt:** 2026-11-28

## Abhängigkeiten
- US-0068: Das README beschreibt dann die API-Versionen. Diese Spec ergänzt den Abschnitt.
- Restpunkt aus US-0005 (#5), Issue #160. Die Infrastruktur ist vorhanden: `EasyVereinConfiguration.GetVersionedBaseUrl(versionOverride)`.

## Ausgangslage
- Jede Serverinstanz nutzt genau eine API-Version: die konfigurierte, Standard v2.0.
- Um Daten in einer anderen Version (z. B. v1.7) zu vergleichen, muss man heute die Konfiguration ändern und den Server neu starten.
- Nur `Member` liest die Formate von v1.7 und v2.0 nachweislich (US-0057). Bei anderen Endpoints kann das Lesen einer v1.x-Antwort scheitern.

## User Stories
- Als Entwickler des MCP-Servers möchte ich dieselben Daten gezielt in einer anderen API-Version abrufen, damit ich Unterschiede zwischen den Versionen prüfen kann, ohne den Server umzukonfigurieren.

## Außerhalb des Umfangs
- Versionswahl für schreibende Tools (create, update, delete, vote, create_receipt, update_accounting_plan) und für `get_token`
- Lesen der Formate aller Versionen für alle Entities (eigene Stories nach dem Muster von US-0057)
- Rohe API-Antwort als Ausgabe
- Versionswahl für MCP-Prompts
- Dauerhaftes Umschalten der Version zur Laufzeit

## Akzeptanzkriterien
- [ ] AK-1: Angenommen `tools/list` wird abgerufen, dann hat jedes `list_*`- und jedes `get_*`-Tool außer `get_token` einen optionalen Parameter `apiVersion` (Text, z. B. `"v1.7"`). Die Beschreibung nennt die unterstützten Versionen und sagt, dass ohne Angabe die konfigurierte Version gilt.
- [ ] AK-2: Angenommen `apiVersion` fehlt oder ist leer, dann ruft das Tool die API in der konfigurierten Version auf und antwortet wie bisher.
- [ ] AK-3: Angenommen `apiVersion` ist `"v1.7"` und die konfigurierte Version ist v2.0, dann geht der Aufruf an `…/api/v1.7/<ressource>…`. Die Konfiguration bleibt unverändert, der nächste Aufruf ohne `apiVersion` nutzt wieder v2.0.
- [ ] AK-4: Angenommen ein `list_*`-Tool mit `apiVersion` holt mehrere Seiten, dann nutzen alle Seitenabrufe dieselbe gewählte Version.
- [ ] AK-5: Angenommen `apiVersion` ist eine nicht unterstützte Version (z. B. `"v9.9"` oder `"abc"`), dann erfolgt kein API-Aufruf. Das Tool antwortet mit einer Fehlermeldung, die die unterstützten Versionen bzw. die nächstmögliche Version nennt. Der Server läuft weiter.
- [ ] AK-6: Angenommen die API antwortet in der gewählten Version, aber die Antwort lässt sich nicht lesen (fehlendes oder anders typisiertes Feld), dann antwortet das Tool mit einer verständlichen Fehlermeldung. Sie nennt die API-Version und die Ressource und sagt, dass die Antwort dieser Version nicht gelesen werden konnte. Kein Absturz, keine Rohdaten.
- [ ] AK-7: Angenommen die API lehnt den Aufruf in der gewählten Version ab (z. B. HTTP 400 wegen unbekannter Felder im Feld-Selektor oder 404, weil es die Ressource in dieser Version nicht gibt), dann meldet das Tool den Fehler wie andere API-Fehler und nennt die gewählte Version.
- [ ] AK-8: Angenommen ein schreibendes Tool (z. B. `update_booking`) wird abgerufen, dann hat es keinen Parameter `apiVersion`.
- [ ] AK-9: Angenommen das README wird gelesen, dann beschreibt der Abschnitt zu API-Versionen den Parameter `apiVersion`: nur lesende Tools, Zweck Diagnose, und der Hinweis, dass ältere Versionen nicht für jede Ressource gelesen werden können.

## Edge Cases
- EC-1: `apiVersion` gleich der konfigurierten Version → Verhalten identisch zu ohne Angabe.
- EC-2: Schreibweise `"V1.7"` oder `"1.7"` → gleiche Behandlung wie bei der Konfiguration (was dort gültig ist, ist auch hier gültig; sonst AK-5).
- EC-3: Mehrere Aufrufe mit unterschiedlichen Versionen kurz nacheinander oder gleichzeitig → jeder Aufruf nutzt nur seine eigene Version; keine gegenseitige Beeinflussung.
- EC-4: `get_*` mit ID, die es in der gewählten Version nicht gibt → Fehlermeldung wie heute bei unbekannter ID.

## Technische Anforderungen
- Neue Tool-Parameter mit Standardwert anlegen (Lehre aus PR #133: sonst Pflichtfeld im MCP-Schema).
- Kein Aufruf der echten easyVerein-API in Tests.

## Offene Fragen
- Keine.

## Product Decisions
- Nur lesende Tools: Zweck ist Diagnose/Vergleich; schreiben mit abweichender Version birgt das Risiko, Vereinsdaten mit falschem Format zu ändern (2026-11-28).
- Lesefehler als verständliche Meldung statt Rohdaten: Antwortformat bleibt einheitlich (`ToolJson`), Dual-Format je Entity ist eigene Arbeit (2026-11-28).
- Parametername `apiVersion`, optional, Default = konfigurierte Version (2026-11-28).
