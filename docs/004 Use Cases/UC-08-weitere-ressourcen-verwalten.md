# UC-08: Weitere Ressourcen verwalten

> **Status:** Geplant (nicht implementiert)
> **Herkunft:** [US-009](https://github.com/RalfGuder/MCP-easyVerein/issues/16) bis [US-055](https://github.com/RalfGuder/MCP-easyVerein/issues/60)
> **Requirements:** FR-045 bis FR-091
> **Include:** UC-07 (Fehlerbehandlung)

## Kurzbeschreibung

Der KI-Assistent verwaltet alle weiteren easyVerein-Ressourcen über MCP-Tools. Jede Ressource folgt dem gleichen CRUD-Muster wie UC-02 bis UC-05 (Auflisten, Abrufen, Anlegen, Bearbeiten, Löschen).

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Primärer Akteur – ruft MCP-Tools auf |
| **Vereinsadministrator** | Auftraggeber |
| **easyVerein API** | Externes System |

## Vorbedingungen

- MCP-Server ist gestartet und konfiguriert (UC-01)
- Gültiger API-Key ist hinterlegt
- Die jeweilige Ressource ist als MCP-Tool implementiert und registriert

## Generisches Hauptszenario (pro Ressource)

Jede Ressource folgt dem identischen Ablauf. `{Ressource}` ist ein Platzhalter (z.B. Booking, Calendar, Protocol).

### Auflisten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | KI-Assistent | Ruft `List{Ressource}s` auf (optional mit Filtern) |
| 2 | System | GET `/{endpoint}` mit Query-Parametern und Pagination (`?limit=100`) |
| 3 | System | Ruft automatisch alle Seiten über `next`-Link ab |
| 4 | System | Gibt vollständige Liste als JSON zurück |

### Einzelne Ressource abrufen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | KI-Assistent | Ruft `Get{Ressource}(id=N)` auf |
| 2 | System | GET `/{endpoint}/N` mit Feldauswahl-Query |
| 3 | System | Gibt die Ressource als JSON zurück (oder „nicht gefunden") |

### Anlegen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | KI-Assistent | Ruft `Create{Ressource}(...)` mit den erforderlichen Feldern auf |
| 2 | System | POST `/{endpoint}` mit den angegebenen Feldern |
| 3 | System | Gibt die erstellte Ressource mit zugewiesener ID zurück |

### Bearbeiten

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | KI-Assistent | Ruft `Update{Ressource}(id=N, ...)` mit den zu ändernden Feldern auf |
| 2 | System | Baut Dictionary mit nur den geänderten Feldern auf (PATCH-Semantik) |
| 3 | System | PATCH `/{endpoint}/N` mit Partial-Update |
| 4 | System | Gibt die aktualisierte Ressource zurück |

### Löschen

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | KI-Assistent | Ruft `Delete{Ressource}(id=N)` auf |
| 2 | System | DELETE `/{endpoint}/N` |
| 3 | System | Gibt Bestätigungsmeldung zurück |

## Geplante Ressourcen

### Finanzen und Buchhaltung

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-009 | Buchungen | `/booking` | FR-045 |
| US-028 | Rechnungspositionen | `/invoice-item` | FR-064 |
| US-011 | Bankkonten | `/bank-account` | FR-047 |
| US-012 | Abrechnungskonten | `/billing-account` | FR-048 |
| US-013 | Buchungsprojekte | `/booking-project` | FR-049 |
| US-035 | Kontenpläne | `/accounting-plan` | FR-071 |
| US-044 | Preisgruppen | `/price-group` | FR-080 |
| US-021 | Benutzerdefinierte Steuersätze | `/custom-tax-rate` | FR-057 |

### Veranstaltungen und Kalender

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-014 | Kalender | `/calendar` | FR-050 |
| US-030 | Veranstaltungsorte | `/location` | FR-066 |

### Mitglieder und Kontakte

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-031 | Mitgliedergruppen | `/member-group` | FR-067 |
| US-016 | Kontaktdaten-Gruppen | `/contact-details-group` | FR-052 |
| US-017 | Kontaktdaten-Änderungsprotokolle | `/contact-details-log` | FR-053 |
| US-015 | Vorstandsebenen | `/chairman-level` | FR-051 |

### Forum und Kommunikation

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-024 | Forum | `/forum` | FR-060 |
| US-043 | Forenbeiträge | `/post` | FR-079 |
| US-052 | Forenthemen | `/topic` | FR-088 |
| US-010 | Ankündigungen | `/announcement` | FR-046 |
| US-033 | Benachrichtigungsprotokolle | `/notification-log` | FR-069 |

### Sitzungen und Protokolle

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-045 | Sitzungsprotokolle | `/protocol` | FR-081 |
| US-046 | Protokollelemente | `/protocol-element` | FR-082 |
| US-047 | Protokollelement-Kommentare | `/protocol-element-comment` | FR-083 |
| US-048 | Protokoll-Uploads | `/protocol-upload` | FR-084 |

### Aufgaben

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-049 | Aufgaben | `/task` | FR-085 |
| US-050 | Aufgaben-Kommentare | `/task-comment` | FR-086 |
| US-051 | Aufgabengruppen | `/task-group` | FR-087 |

### Abstimmungen

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-053 | Abstimmungen | `/voting` | FR-089 |
| US-054 | Abstimmungsfragen | `/voting-question` | FR-090 |

### Inventar und Ausleihe

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-026 | Inventarobjekte | `/inventory-object` | FR-062 |
| US-027 | Inventarobjekt-Gruppen | `/inventory-object-group` | FR-063 |
| US-029 | Ausleihen | `/lending` | FR-065 |

### Benutzerdefinierte Felder und Filter

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-018 | Benutzerdefinierte Felder | `/custom-field` | FR-054 |
| US-019 | Benutzerdefinierte Feldsammlungen | `/custom-field-collection` | FR-055 |
| US-020 | Benutzerdefinierte Filter | `/custom-filter` | FR-056 |

### Pässe und Ausweise (Passcreator)

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-039 | Pässe | `/pass` | FR-075 |
| US-040 | Pass-Felder | `/pass-field` | FR-076 |
| US-041 | Pass-Vorlagen | `/pass-template` | FR-077 |
| US-042 | Passcreator-Integrationen | `/passcreator-integration` | FR-078 |

### Organisation und System

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-034 | Organisationen | `/organization` | FR-070 |
| US-037 | Organisationseinstellungen | `/organization-settings` | FR-073 |
| US-022 | DOSB-Sportarten | `/dosb-sport` | FR-058 |
| US-023 | Feature-Requests | `/feature-request` | FR-059 |
| US-055 | Webseiten | `/website` | FR-091 |

### Token-Verwaltung

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-025 | Get-Token | `/get-token` | FR-061 |
| US-036 | Benachrichtigungs-Tokens | `/notification-token` | FR-072 |
| US-038 | Organisations-Tokens | `/organization-token` | FR-074 |

### Sonstiges

| US | Ressource | Endpoint | FR |
|----|-----------|----------|-----|
| US-032 | Normalisierung | `/normalize` | FR-068 |

## Implementierungsmuster

Jede Ressource wird nach dem gleichen Muster implementiert (siehe US-008 als Referenz):

1. `{Entity}Fields.cs` – API-Feldnamen als Konstanten
2. `{Entity}.cs` – Domain-Entity mit `[JsonPropertyName]`-Attributen
3. `{Entity}Query.cs` – Query-Klasse für Filterung
4. `IEasyVereinApiClient` – Interface um CRUD-Methoden erweitern
5. `EasyVereinApiClient` – CRUD-Methoden implementieren
6. `{Entity}Tools.cs` – MCP-Tools mit Error-Handling
7. `Program.cs` – `.WithTools<{Entity}Tools>()` registrieren
8. Unit-Tests nach TDD

## Fehlerfälle

→ Identisch mit [UC-07: Fehlerbehandlung](UC-07-fehlerbehandlung.md) – alle CRUD-Operationen nutzen die gleiche Fehlerbehandlung.
