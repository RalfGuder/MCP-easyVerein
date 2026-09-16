# UC-03: Kontaktdaten verwalten

> **Status:** Implementiert
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-008](https://github.com/RalfGuder/MCP-easyVerein/issues/14)
> **Requirements:** FR-007, NFR-001, NFR-002, NFR-003
> **Include:** UC-07 (Fehlerbehandlung)

## Kurzbeschreibung

Der KI-Assistent verwaltet Kontaktdaten von Vereinsmitgliedern über MCP-Tools: Kontaktdaten auflisten, einzelne abrufen, anlegen, bearbeiten und löschen. Kontaktdaten sind von Mitgliedern getrennte Entities mit eigenem API-Endpoint.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Primärer Akteur – ruft MCP-Tools auf |
| **Vereinsadministrator** | Auftraggeber – gibt dem KI-Assistenten Anweisungen |
| **easyVerein API** | Externes System – liefert und speichert Kontaktdaten |

## Vorbedingungen

- MCP-Server ist gestartet und konfiguriert (UC-01)
- Gültiger API-Key ist hinterlegt

## Hauptszenario: Kontaktdaten auflisten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Suche Kontaktdaten für Schmidt" |
| 2 | KI-Assistent | Ruft `ListContactDetails(familyName="Schmidt")` auf |
| 3 | System | Sendet GET-Request an `/contact-details` mit Filter-Query |
| 4 | easyVerein API | Gibt paginierte Ergebnisliste zurück |
| 5 | System | Ruft automatisch alle Seiten ab und gibt vollständige Liste zurück |
| 6 | KI-Assistent | Präsentiert die gefundenen Kontaktdaten |

## Alternativszenarien

### A1: Einzelne Kontaktdaten abrufen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `GetContactDetails(id=99)` auf |
| 3 | System | GET `/contact-details/99` mit Feldauswahl-Query |
| 4 | System | Gibt alle Kontaktfelder als JSON zurück (Name, Adresse, Telefon, E-Mail, Bank, Firma etc.) |

### A2: Kontaktdaten anlegen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `CreateContactDetails(firstName="Anna", familyName="Müller", privateEmail="anna@test.de")` auf |
| 3 | System | POST `/contact-details` mit den angegebenen Feldern |
| 4 | System | Gibt die erstellten Kontaktdaten mit zugewiesener ID zurück |

### A3: Kontaktdaten bearbeiten (PATCH)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `UpdateContactDetails(id=99, firstName="Anna-Maria")` auf |
| 3 | System | Baut ein Dictionary mit nur den geänderten Feldern auf |
| 4 | System | PATCH `/contact-details/99` mit dem Partial-Update |
| 5 | System | Gibt die aktualisierten Kontaktdaten zurück |

### A4: Kontaktdaten löschen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `DeleteContactDetails(id=99)` auf |
| 3 | System | DELETE `/contact-details/99` |
| 4 | System | Gibt Bestätigungsmeldung zurück |

## Fehlerfälle

→ Siehe [UC-07: Fehlerbehandlung](UC-07-fehlerbehandlung.md)

### F1: Kontaktdaten nicht gefunden

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | easyVerein API | Gibt HTTP 404 zurück |
| 3b | System | Gibt „Contact details with ID X not found." zurück |

## Nachbedingungen

- **Auflisten/Abrufen:** Daten wurden gelesen, kein Zustand verändert
- **Anlegen:** Neue Kontaktdaten existieren in easyVerein
- **Bearbeiten:** Nur die angegebenen Felder wurden aktualisiert (PATCH-Semantik)
- **Löschen:** Kontaktdaten existieren nicht mehr

## Besonderheiten

- Kontaktdaten enthalten über 40 Felder (persönliche Daten, Adresse, Firma, Bank, Kommunikation)
- Bei `UpdateContactDetails` werden nur geänderte Felder als Dictionary gesendet (echtes PATCH)
- Kontaktdaten werden auch implizit durch UC-02 (Mitglied anlegen) erstellt

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| MCP-Tools | `src/MCP.EasyVerein.Server/Tools/ContactDetailsTools.cs` |
| API-Client | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` |
| Entity | `src/MCP.EasyVerein.Domain/Entities/ContactDetails.cs` |
| Felder | `src/MCP.EasyVerein.Domain/ValueObjects/ContactDetailsFields.cs` |
| Query | `src/MCP.EasyVerein.Infrastructure/ApiClient/ContactDetailsQuery.cs` |
| Tests | `tests/MCP.EasyVerein.Domain.Tests/ContactDetailsEntityTests.cs` |
