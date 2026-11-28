# MCP-easyVerein

Ein lokaler MCP-Server (Model Context Protocol) zur Anbindung der [easyVerein API](https://easyverein.com/api/). Ermöglicht KI-Assistenten wie Claude den direkten Zugriff auf Vereinsdaten.

## Systemvoraussetzungen

Der MCP-Server kann auf zwei Wegen betrieben werden:

### Option 1: Docker (empfohlen)

| Komponente | Mindestversion |
|-----------|---------------|
| Docker | 20.10+ |

```bash
docker build -t mcp-easyverein .
docker run -i --rm -e EASYVEREIN_API_KEY=<token> mcp-easyverein
```

`-i` ist Pflicht: Der Server spricht MCP über Standardein- und -ausgabe (stdio). Ohne `-i` ist die Standardeingabe des Containers nicht verbunden, und kein Client kann mit dem Server sprechen. `--rm` entfernt den Container nach dem Ende wieder.

### Option 2: Nativ mit .NET 8 SDK

| Komponente | Mindestversion |
|-----------|---------------|
| .NET 8 SDK | 8.0+ (LTS) |

**Unterstützte Betriebssysteme:** Windows, Linux, macOS (x86_64 / ARM64)

```bash
dotnet build
dotnet run --project src/MCP.EasyVerein.Server -- --api-key <token>
```

Eine Vorlage für Claude Code liegt in [`.mcp.json.example`](.mcp.json.example): nach `.mcp.json` kopieren und den Platzhalter durch den eigenen Schlüssel ersetzen.

### Getestete Plattformen

| Betriebssystem | Architektur | Status |
|---------------|-------------|--------|
| Ubuntu 24.04 LTS | x86_64 | Unterstützt |
| Windows 10/11 | x86_64 | Unterstützt |
| macOS 13+ | x86_64 / ARM64 | Unterstützt |

## Konfiguration

Der Server braucht den API-Schlüssel von easyVerein. URL und API-Version haben Standardwerte.

| Einstellung | CLI-Parameter | Umgebungsvariable | Standardwert |
|-------------|---------------|-------------------|--------------|
| API-Schlüssel | `--api-key <wert>` | `EASYVEREIN_API_KEY` | keiner (muss gesetzt werden) |
| API-URL | `--api-url <wert>` | `EASYVEREIN_API_URL` | `https://easyverein.com/api` |
| API-Version | `--api-version <wert>` | `EASYVEREIN_API_VERSION` | `v2.0` |

**Priorität:** CLI-Parameter vor Umgebungsvariable vor Standardwert. Ein CLI-Parameter überschreibt also eine gesetzte Umgebungsvariable.

`--help` (oder `-h`) zeigt alle Parameter mit Umgebungsvariablen und Standardwerten an und beendet den Server:

```bash
dotnet run --project src/MCP.EasyVerein.Server -- --help
```

**Schlüssel ohne `Bearer `-Präfix eintragen**, also nur den Token selbst (`EASYVEREIN_API_KEY=<token>`). Der Server setzt `Bearer ` beim Aufruf der API selbst davor. Ein versehentlich mit eingetragenes Präfix (`Bearer <token>`, auch klein geschrieben) wird entfernt, damit ältere Konfigurationen weiter funktionieren.

### API-Versionen

Unterstützt werden die easyVerein-API-Versionen **v1.4, v1.5, v1.6, v1.7 und v2.0**. Standard ist **v2.0**.

Eine nicht unterstützte Version wird beim Start abgelehnt: Der Server schreibt eine Fehlermeldung mit der nächstmöglichen unterstützten Version und beendet sich mit Exit-Code 1, z. B.

```
Fehler: API-Version 'v1.8' wird nicht unterstützt. Nächste unterstützte Version: v1.7. Unterstützte Versionen: v1.4, v1.5, v1.6, v1.7, v2.0
```

## Einbindung in Claude Code

Claude Code startet MCP-Server über einen Eintrag in `.mcp.json` im Projektverzeichnis.

**Nativ** (Vorlage: [`.mcp.json.example`](.mcp.json.example)):

```json
{
  "mcpServers": {
    "easyverein": {
      "command": "dotnet",
      "args": ["run", "--project", "src/MCP.EasyVerein.Server"],
      "env": {
        "EASYVEREIN_API_KEY": "<your-easyverein-api-token-here>"
      }
    }
  }
}
```

**Docker** (Image vorher mit `docker build -t mcp-easyverein .` bauen):

```json
{
  "mcpServers": {
    "easyverein": {
      "command": "docker",
      "args": ["run", "-i", "--rm", "-e", "EASYVEREIN_API_KEY", "mcp-easyverein"],
      "env": {
        "EASYVEREIN_API_KEY": "<your-easyverein-api-token-here>"
      }
    }
  }
}
```

`-e EASYVEREIN_API_KEY` ohne Wert reicht den Schlüssel aus `env` in den Container durch, so steht er nicht in der Prozessliste. Weitere Einstellungen gehen genauso (`-e EASYVEREIN_API_VERSION`) oder als CLI-Parameter hinter dem Image-Namen (`"mcp-easyverein", "--api-version", "v1.7"`).

## Projektstruktur

Der Server folgt der Clean Architecture mit vier Schichten. Abhängigkeiten zeigen nur nach innen: Server → Infrastructure → Application → Domain. Die Domain hat keine externen Abhängigkeiten.

| Schicht | Projekt | Aufgabe |
|---------|---------|---------|
| Domain | `src/MCP.EasyVerein.Domain` | Entities, Fields-Value-Objects, `IEasyVereinApiClient`, `ApiVersion`; keine externen Abhängigkeiten |
| Application | `src/MCP.EasyVerein.Application` | Konfiguration aus CLI-Parametern, Umgebungsvariablen und Standardwerten |
| Infrastructure | `src/MCP.EasyVerein.Infrastructure` | `EasyVereinApiClient` (HTTP) und Query-Builder |
| Server | `src/MCP.EasyVerein.Server` | MCP-Tools und Prompts, DI-Setup, `Program.cs` (Stdio-Transport) |

Die Tests liegen je Schicht unter `tests/`.

**Neuen Endpoint anbinden** – immer in fünf Schritten:

1. Domain-Entity mit Fields-ValueObject anlegen
2. Interface `IEasyVereinApiClient` erweitern
3. `EasyVereinApiClient` implementieren, mit Query-Builder
4. Tool-Klasse mit MCP-Attributen im Server anlegen
5. Tool-Klasse in `Program.cs` registrieren (`.WithTools<T>()`)

Warum C# / .NET 8: siehe [ADR 0001](docs/adr/0001-csharp-dotnet8.md) (`docs/adr/0001-csharp-dotnet8.md`).

## Endpoints

Angebundene easyVerein-Endpoints mit ihren MCP-Tools. Die Parameter jedes Tools liefert der Server über `tools/list`. Updates laufen immer als partielles Update (PATCH).

| Endpoint | Tools |
|----------|-------|
| Member (Mitglieder) | `list_members`, `get_member`, `create_member`, `update_member`, `delete_member` |
| ContactDetails (Kontaktdaten) | `list_contact_details`, `get_contact_details`, `create_contact_details`, `update_contact_details`, `delete_contact_details` |
| ContactDetailsGroup (Kontaktgruppen) | `list_contact_details_groups`, `get_contact_details_group`, `create_contact_details_group`, `update_contact_details_group`, `delete_contact_details_group` |
| ContactDetailsLog (Änderungsprotokoll Kontakte) | `list_contact_details_logs`, `get_contact_details_log`, `create_contact_details_log`, `update_contact_details_log`, `delete_contact_details_log` |
| Invoice (Rechnungen) | `list_invoices`, `get_invoice`, `create_invoice`, `update_invoice`, `delete_invoice`, Sonderfall `create_receipt` (Eigenbeleg) |
| InvoiceItem (Rechnungspositionen) | `list_invoice_items`, `get_invoice_item`, `create_invoice_item`, `update_invoice_item`, `delete_invoice_item` |
| Booking (Buchungen) | `list_bookings`, `get_booking`, `create_booking`, `update_booking`, `delete_booking` |
| BookingProject (Buchungsprojekte) | `list_booking_projects`, `get_booking_project`, `create_booking_project`, `update_booking_project`, `delete_booking_project` |
| BankAccount (Bankkonten) | `list_bank_accounts`, `get_bank_account`, `create_bank_account`, `update_bank_account`, `delete_bank_account` |
| BillingAccount (Buchungskonten) | `list_billing_accounts`, `get_billing_account`, `create_billing_account`, `update_billing_account`, `delete_billing_account` |
| AccountingPlan (Kontenplan) | `list_accounting_plans`, `get_accounting_plan`, `update_accounting_plan` (Konten frei-/abschalten); kein create/delete |
| CustomTaxRate (Steuersätze) | `list_custom_tax_rates`, `get_custom_tax_rate`, `create_custom_tax_rate`, `update_custom_tax_rate`, `delete_custom_tax_rate` |
| Event (Termine) | `list_events`, `get_event`, `create_event`, `update_event`, `delete_event` |
| Calendar (Kalender) | `list_calendars`, `get_calendar`, `create_calendar`, `update_calendar`, `delete_calendar` |
| Announcement (Ankündigungen) | `list_announcements`, `get_announcement`, `create_announcement`, `update_announcement`, `delete_announcement` |
| Forum | `list_forums`, `get_forum`, `create_forum`, `update_forum`, `delete_forum` |
| ChairmanLevel (Vorstandsebenen) | `list_chairman_levels`, `get_chairman_level`, `create_chairman_level`, `update_chairman_level`, `delete_chairman_level` |
| CustomField (Individuelle Felder) | `list_custom_fields`, `get_custom_field`, `create_custom_field`, `update_custom_field`, `delete_custom_field` |
| CustomFieldCollection (Feldgruppen) | `list_custom_field_collections`, `get_custom_field_collection`, `create_custom_field_collection`, `update_custom_field_collection`, `delete_custom_field_collection` |
| CustomFilter (Gespeicherte Filter) | `list_custom_filters`, `get_custom_filter`, `create_custom_filter`, `update_custom_filter`, `delete_custom_filter` |
| InventoryObject (Inventar) | `list_inventory_objects`, `get_inventory_object`, `create_inventory_object`, `update_inventory_object`, `delete_inventory_object` |
| InventoryObjectGroup (Inventargruppen) | `list_inventory_object_groups`, `get_inventory_object_group`, `create_inventory_object_group`, `update_inventory_object_group`, `delete_inventory_object_group` |
| Lending (Ausleihen) | `list_lendings`, `get_lending`, `create_lending`, `update_lending`, `delete_lending` |
| DosbSport (DOSB-Sportarten) | `list_dosb_sports`, `get_dosb_sport`; nur lesend |
| FeatureRequest (Ideen-Board) | `list_feature_requests`, `get_feature_request`, `create_feature_request`, Sonderfall `vote_feature_request`; kein update/delete |
| GetToken (Login) | Sonderfall `get_token` (Token per Zugangsdaten, Ausgabe nur maskiert) |

## Antwortformat

Die Tools antworten mit kompaktem JSON ohne `null`-Felder. Verweise auf andere Datensätze (z. B. Bankkonto oder Rechnung einer Buchung) stehen nur als ID in der Antwort, auch wenn die API sie als URL liefert (z. B. `bookingProject`); die Details liefert das passende `get_*`-Tool.

Die `list_*`-Tools liefern seitenweise:

| Parameter | Bedeutung | Standard |
|-----------|-----------|----------|
| `limit` | Einträge je Seite (1–100) | 50 |
| `page` | Seitennummer ab 1 | 1 |

```json
{"items": [ ... ], "totalCount": 95, "hasMore": true, "page": 1, "limit": 50}
```

Solange `hasMore` `true` ist, gibt es weitere Seiten (`page` + 1). `totalCount` steht nur in der Antwort, wenn die Gesamtzahl bekannt ist; die easyVerein-API v2.0 meldet sie nicht, daher erscheint sie dort erst auf der letzten Seite.

## Branch-Strategie

Dieses Projekt verwendet **GitHub Flow** als Branch-Strategie.

### Regeln

1. **`main` ist immer stabil und deploybar**
2. Für jede User Story / jedes Issue wird ein eigener Branch erstellt
3. Änderungen kommen über **Pull Requests** zurück in `main`
4. CI/CD-Pipeline prüft Tests und Coverage vor dem Merge
5. Feature-Branches werden nach dem Merge gelöscht

### Branch-Namenskonvention

| Typ | Muster | Beispiel |
|-----|--------|----------|
| Feature | `feature/US-XXXX-kurzbeschreibung` | `feature/US-0001-easyverein-mcp-server` |
| Bugfix | `fix/kurzbeschreibung` | `fix/api-token-validation` |

### Workflow

```
main (immer stabil)
  ├── feature/US-0001-easyverein-mcp-server
  ├── feature/US-0002-rules-und-prompts
  └── fix/api-token-validation
```

1. Branch von `main` erstellen
2. Änderungen committen (TDD: Tests zuerst)
3. Pull Request erstellen
4. CI/CD prüft Tests + Coverage (≥ 70%)
5. Code Review + Merge in `main`
6. Feature-Branch löschen

## Lizenz

Dieses Projekt steht unter der [MIT-Lizenz](LICENSE).
