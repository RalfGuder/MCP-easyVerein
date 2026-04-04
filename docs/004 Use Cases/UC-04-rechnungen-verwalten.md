# UC-04: Rechnungen verwalten

> **Status:** Teilweise implementiert (Update-Tool fehlt)
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-008](https://github.com/RalfGuder/MCP-easyVerein/issues/14)
> **Requirements:** FR-005, NFR-001, NFR-002, NFR-003
> **Include:** UC-07 (Fehlerbehandlung)

## Kurzbeschreibung

Der KI-Assistent verwaltet Vereinsrechnungen über MCP-Tools: Rechnungen auflisten, einzelne abrufen, anlegen und löschen. Das Bearbeiten (Update) ist im API-Client implementiert, aber noch nicht als MCP-Tool exponiert.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Primärer Akteur – ruft MCP-Tools auf |
| **Vereinsadministrator** | Auftraggeber – gibt dem KI-Assistenten Anweisungen |
| **easyVerein API** | Externes System – liefert und speichert Rechnungsdaten |

## Vorbedingungen

- MCP-Server ist gestartet und konfiguriert (UC-01)
- Gültiger API-Key ist hinterlegt

## Hauptszenario: Rechnungen auflisten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | Vereinsadministrator | „Zeige mir alle Rechnungen" |
| 2 | KI-Assistent | Ruft `ListInvoices` auf |
| 3 | System | Sendet GET-Request an `/invoice` mit Feldauswahl-Query und `?limit=100` |
| 4 | easyVerein API | Gibt paginierte Ergebnisliste zurück |
| 5 | System | Ruft automatisch alle Seiten ab |
| 6 | System | Gibt die vollständige Rechnungsliste als JSON zurück |

## Alternativszenarien

### A1: Einzelne Rechnung abrufen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `GetInvoice(id=123)` auf |
| 3 | System | GET `/invoice/123` mit Feldauswahl |
| 4 | System | Gibt alle Rechnungsfelder zurück (Nummer, Betrag, Datum, Empfänger, Status etc.) |

### A2: Neue Rechnung anlegen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `CreateInvoice(invoiceNumber="RE-2026-001", totalPrice=119.00, description="Jahresbeitrag", kind="invoice")` auf |
| 3 | System | POST `/invoice` mit den angegebenen Feldern |
| 4 | System | Gibt die erstellte Rechnung mit zugewiesener ID zurück |

### A3: Rechnung bearbeiten (noch nicht als MCP-Tool verfügbar)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `UpdateInvoice(id=123, ...)` auf |
| 3 | System | **Noch nicht implementiert** – API-Client-Methode `UpdateInvoiceAsync` existiert, MCP-Tool fehlt |

### A4: Rechnung löschen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 2 | KI-Assistent | Ruft `DeleteInvoice(id=123)` auf |
| 3 | System | DELETE `/invoice/123` |
| 4 | System | Gibt Bestätigungsmeldung zurück |

## Fehlerfälle

→ Siehe [UC-07: Fehlerbehandlung](UC-07-fehlerbehandlung.md)

### F1: Rechnung nicht gefunden

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 3a | easyVerein API | Gibt HTTP 404 zurück |
| 3b | System | Gibt „Rechnung mit ID X nicht gefunden." zurück |

## Nachbedingungen

- **Auflisten/Abrufen:** Daten wurden gelesen, kein Zustand verändert
- **Anlegen:** Neue Rechnung existiert in easyVerein
- **Löschen:** Rechnung existiert nicht mehr

## Bekannte Lücke

> **UpdateInvoice MCP-Tool fehlt:** Die API-Client-Methode `UpdateInvoiceAsync` (PATCH) ist implementiert, aber es gibt kein entsprechendes MCP-Tool in `InvoiceTools.cs`. Nur List, Get, Create und Delete sind als MCP-Tools exponiert.

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| MCP-Tools | `src/MCP.EasyVerein.Server/Tools/InvoiceTools.cs` |
| API-Client | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` |
| Entity | `src/MCP.EasyVerein.Domain/Entities/Invoice.cs` |
| Felder | `src/MCP.EasyVerein.Domain/ValueObjects/InvoiceFields.cs` |
| Query | `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs` |
| Tests | `tests/MCP.EasyVerein.Domain.Tests/InvoiceEntityTests.cs`, `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` |
