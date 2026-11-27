# QA-Bericht: US-0067 Porto-Prompt – Kontierung nach aktueller Praxis

**Datum:** 2026-11-27 · **Ergebnis:** bestanden

## Akzeptanzkriterien
| AK | Ergebnis | Nachweis |
|---|---|---|
| AK-1 Zielregel genannt | ✅ | `Zielregel_WirdGenannt` |
| AK-2 keine 2902/„Büromaterial“ an der Buchung | ✅ | `AlteRegel_NichtMehrImText` |
| AK-3 Stichwörter Deutsche Post, DHL, Hermes, UPS, GLS, DPD | ✅ | `PortoStichwoerter_WerdenGenannt` |
| AK-4 Behandlung je Kandidat | ✅ | `BehandlungJeKandidat_WirdBeschrieben` |
| AK-5 ohne Beleg, Liste „Beleg fehlt“ | ✅ | `OhneBeleg_ListeBelegFehlt_KeinEigenbeleg` |
| AK-6 Default 31.12. Vorjahr | ✅ | `GesperrtBis_Default_IstEndeVorjahr` (3 Daten), `ReviewPortoBuchungen_OhneGesperrtBis_NutztAktuellesVorjahr` |
| AK-7 `gesperrtBis=2024-12-31` | ✅ | `GesperrtBis_Explizit_WirdUebernommen`, JSON-RPC `prompts/get` |
| AK-8 Sperre auch bei dryRun=false, Liste „gesperrt …“ | ✅ | `Sperre_GiltAuchBeiDryRunFalse` |
| AK-9 dryRun=true nur Vorschläge | ✅ | `DryRunTrue_NurVorschlaege` |
| AK-10 dryRun=false ausführen, IDs auflisten | ✅ | `DryRunFalse_FuehrtAus`, JSON-RPC mit `"dryRun":"false"` |
| AK-11 Zeitraum wie bisher | ✅ | `Zeitraum_WieBisher` |
| AK-12 IDs auflösen, sonst abbrechen | ✅ | `IdsAufloesen_SonstAbbrechen` |
| AK-13 Beschreibung in `prompts/list` | ✅ | `Beschreibung_NenntNeueRegelUndGesperrtBis`, JSON-RPC `prompts/list` |
| AK-14 Grenzfälle bleiben | ✅ | `Grenzfaelle_BleibenErhalten` |

Edge Cases EC-1 bis EC-6 sind im Text abgedeckt (`BehandlungJeKandidat…`, `MehrereBelege_UndFinalisierteBelege…`, `GesperrtBis_Ungueltig_FaelltAufDefaultZurueck`, `IdsAufloesen_SonstAbbrechen`).

## Manuelle Prüfung per JSON-RPC (Platzhalter-Key, keine API-Aufrufe)
- `prompts/list`: vier optionale Argumente `dateVon`, `dateBis`, `dryRun`, `gesperrtBis`.
- `prompts/get` mit `{"dryRun":"false","gesperrtBis":"2024-12-31"}`: Text enthält „Sperrdatum: 2024-12-31“ und „Modus: dryRun=false“.
- Gegenprobe ohne `PromptJson.SerializerOptions`: dieselbe Anfrage endet mit `{"code":-32603,"message":"An error occurred."}`. Der tolerante Bool-Converter ist also nötig.

## Tests und Coverage
- 593 Tests grün (Domain 163, Application 13, Infrastructure 236, Server 181).
- Zeilenabdeckung: `PortoBuchungenPrompt.cs` 100 %, `PromptJson.cs` 83–100 %.

## Sicherheit
- Keine neuen Netzwerkzugriffe, keine Geheimnisse. Ein ungültiges `gesperrtBis` wird nur als Text in den Prompt zurückgegeben (kein Ausführen, keine Ausgabe in HTML).
- Der Bool-Converter akzeptiert nur `true`/`false`; andere Werte werfen `JsonException`.
