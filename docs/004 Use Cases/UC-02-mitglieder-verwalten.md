# UC-02: Mitglieder verwalten

> **Status:** Implementiert
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-008](https://github.com/RalfGuder/MCP-easyVerein/issues/14)
> **Requirements:** FR-003, FR-004, NFR-001, NFR-002, NFR-003
> **Include:** UC-03 (Kontaktdaten beim Anlegen), UC-07 (Fehlerbehandlung)

## Kurzbeschreibung

Der KI-Assistent verwaltet Vereinsmitglieder über MCP-Tools: Mitglieder auflisten, einzelne abrufen, anlegen, bearbeiten und löschen.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Primärer Akteur – ruft MCP-Tools auf |
| **Vereinsadministrator** | Auftraggeber – gibt dem KI-Assistenten Anweisungen |
| **easyVerein API** | Externes System – liefert und speichert Mitgliederdaten |

## Vorbedingungen

- MCP-Server ist gestartet und konfiguriert (UC-01)
- Gültiger API-Key ist hinterlegt
- KI-Assistent ist über stdio verbunden

## Hauptszenario: Mitglieder auflisten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Zeige mir alle Mitglieder" |
| 2 | KI-Assistent | Ruft `list_members` auf (optional mit Filtern: `id`, `membershipNumber`, `search`) |
| 3 | System | Sendet GET-Request an `/member` mit Query-Parametern und `?limit=100` |
| 4 | easyVerein API | Gibt paginierte Ergebnisliste zurück |
| 5 | System | Ruft automatisch alle weiteren Seiten über den `next`-Link ab |
| 6 | System | Gibt die vollständige Mitgliederliste als JSON an den KI-Assistenten zurück |
| 7 | KI-Assistent | Präsentiert die Ergebnisse dem Vereinsadministrator |

## Alternativszenarien

### A1: Einzelnes Mitglied abrufen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Zeige mir Mitglied 42" |
| 2 | KI-Assistent | Ruft `get_member(id=42)` auf |
| 3 | System | Sendet GET-Request an `/member/42` mit Feldauswahl-Query |
| 4 | easyVerein API | Gibt Mitgliedsdaten zurück |
| 5 | System | Gibt JSON mit allen Mitgliedsfeldern zurück |

### A2: Neues Mitglied anlegen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Lege ein neues Mitglied an: Max Mustermann, max@test.de" |
| 2 | KI-Assistent | Ruft `create_member(emailOrUserName="max@test.de", firstName="Max", familyName="Mustermann")` auf |
| 3 | System | Erstellt zuerst `ContactDetails` über POST `/contact-details` (**UC-03 include**) |
| 4 | easyVerein API | Gibt erstellte ContactDetails mit ID zurück |
| 5 | System | Erstellt `Member` über POST `/member` mit Referenz auf ContactDetails-ID |
| 6 | easyVerein API | Gibt erstelltes Mitglied zurück |
| 7 | System | Gibt das neue Mitglied als JSON zurück |

### A3: Mitglied bearbeiten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Ändere die Mitgliedsnummer von Mitglied 42 auf M-100" |
| 2 | KI-Assistent | Ruft `update_member(id=42, membershipNumber="M-100")` auf |
| 3 | System | Sendet PATCH-Request an `/member/42` mit nur den geänderten Feldern |
| 4 | easyVerein API | Gibt das aktualisierte Mitglied zurück |
| 5 | System | Gibt das aktualisierte Mitglied als JSON zurück |

### A4: Mitglied löschen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Lösche Mitglied 42" |
| 2 | KI-Assistent | Ruft `delete_member(id=42)` auf |
| 3 | System | Sendet DELETE-Request an `/member/42` |
| 4 | easyVerein API | Bestätigt die Löschung |
| 5 | System | Gibt Bestätigungsmeldung zurück |

## Fehlerfälle

→ Siehe [UC-07: Fehlerbehandlung](UC-07-fehlerbehandlung.md) für HTTP 401/403, Netzwerkfehler und Timeouts.

### F1: Mitglied nicht gefunden

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | easyVerein API | Gibt HTTP 404 zurück |
| 3b | System | Gibt `null` / „Member with ID X not found." zurück |

## Nachbedingungen

- **Auflisten/Abrufen:** Daten wurden gelesen, kein Zustand verändert
- **Anlegen:** Neues Mitglied und zugehörige Kontaktdaten existieren in easyVerein
- **Bearbeiten:** Nur die angegebenen Felder wurden aktualisiert
- **Löschen:** Mitglied existiert nicht mehr in easyVerein

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| MCP-Tools | `src/MCP.EasyVerein.Server/Tools/MemberTools.cs` |
| API-Client | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` |
| Entity | `src/MCP.EasyVerein.Domain/Entities/Member.cs` |
| Felder | `src/MCP.EasyVerein.Domain/ValueObjects/MemberFields.cs` |
| Query | `src/MCP.EasyVerein.Infrastructure/ApiClient/MemberQuery.cs` |
| Tests | `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs`, `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` |
