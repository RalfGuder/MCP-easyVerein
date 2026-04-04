# UC-05: Veranstaltungen verwalten

> **Status:** Teilweise implementiert (Update-Tool fehlt)
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-008](https://github.com/RalfGuder/MCP-easyVerein/issues/14)
> **Requirements:** FR-006, NFR-001, NFR-002, NFR-003
> **Include:** UC-07 (Fehlerbehandlung)

## Kurzbeschreibung

Der KI-Assistent verwaltet Vereinsveranstaltungen über MCP-Tools: Veranstaltungen auflisten, einzelne abrufen, anlegen und löschen. Das Bearbeiten (Update) ist im API-Client implementiert, aber noch nicht als MCP-Tool exponiert.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Primärer Akteur – ruft MCP-Tools auf |
| **Vereinsadministrator** | Auftraggeber – gibt dem KI-Assistenten Anweisungen |
| **easyVerein API** | Externes System – liefert und speichert Veranstaltungsdaten |

## Vorbedingungen

- MCP-Server ist gestartet und konfiguriert (UC-01)
- Gültiger API-Key ist hinterlegt

## Hauptszenario: Veranstaltungen auflisten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Zeige mir alle Veranstaltungen" |
| 2 | KI-Assistent | Ruft `ListEvents` auf |
| 3 | System | Sendet GET-Request an `/event` mit Feldauswahl-Query und `?limit=100` |
| 4 | easyVerein API | Gibt paginierte Ergebnisliste zurück |
| 5 | System | Ruft automatisch alle Seiten ab |
| 6 | System | Gibt die vollständige Veranstaltungsliste als JSON zurück |

## Alternativszenarien

### A1: Einzelne Veranstaltung abrufen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `GetEvent(id=7)` auf |
| 3 | System | GET `/event/7` mit Feldauswahl |
| 4 | System | Gibt alle Veranstaltungsfelder zurück (Name, Datum, Ort, Teilnehmerzahl etc.) |

### A2: Neue Veranstaltung anlegen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `CreateEvent(name="Jahreshauptversammlung", description="Jährliche Mitgliederversammlung", locationName="Vereinsheim")` auf |
| 3 | System | POST `/event` mit den angegebenen Feldern |
| 4 | System | Gibt die erstellte Veranstaltung mit zugewiesener ID zurück |

### A3: Veranstaltung bearbeiten (noch nicht als MCP-Tool verfügbar)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `UpdateEvent(id=7, ...)` auf |
| 3 | System | **Noch nicht implementiert** – API-Client-Methode `UpdateEventAsync` existiert, MCP-Tool fehlt |

### A4: Veranstaltung löschen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `DeleteEvent(id=7)` auf |
| 3 | System | DELETE `/event/7` |
| 4 | System | Gibt Bestätigungsmeldung zurück |

## Fehlerfälle

→ Siehe [UC-07: Fehlerbehandlung](UC-07-fehlerbehandlung.md)

### F1: Veranstaltung nicht gefunden

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | easyVerein API | Gibt HTTP 404 zurück |
| 3b | System | Gibt „Veranstaltung mit ID X nicht gefunden." zurück |

## Nachbedingungen

- **Auflisten/Abrufen:** Daten wurden gelesen, kein Zustand verändert
- **Anlegen:** Neue Veranstaltung existiert in easyVerein
- **Löschen:** Veranstaltung existiert nicht mehr

## Bekannte Lücke

> **UpdateEvent MCP-Tool fehlt:** Die API-Client-Methode `UpdateEventAsync` (PATCH) ist implementiert, aber es gibt kein entsprechendes MCP-Tool in `EventTools.cs`. Nur List, Get, Create und Delete sind als MCP-Tools exponiert.

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| MCP-Tools | `src/MCP.EasyVerein.Server/Tools/EventTools.cs` |
| API-Client | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` |
| Entity | `src/MCP.EasyVerein.Domain/Entities/Event.cs` |
| Felder | `src/MCP.EasyVerein.Domain/ValueObjects/EventFields.cs` |
| Tests | `tests/MCP.EasyVerein.Domain.Tests/EventEntityTests.cs`, `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` |
