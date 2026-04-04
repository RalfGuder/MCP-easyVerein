# REQ-01: MCP-Server Kernfunktionalität

> **Thema:** MCP-Server und easyVerein API-Anbindung
> **Herkunft:** [US-0001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-0005](https://github.com/RalfGuder/MCP-easyVerein/issues/5), [US-0007](https://github.com/RalfGuder/MCP-easyVerein/issues/12), [US-0009](https://github.com/RalfGuder/MCP-easyVerein/issues/16), [US-0010](https://github.com/RalfGuder/MCP-easyVerein/issues/17), [US-0011](https://github.com/RalfGuder/MCP-easyVerein/issues/18), [US-0012](https://github.com/RalfGuder/MCP-easyVerein/issues/19), [US-0013](https://github.com/RalfGuder/MCP-easyVerein/issues/20), [US-0014](https://github.com/RalfGuder/MCP-easyVerein/issues/21), [US-0015](https://github.com/RalfGuder/MCP-easyVerein/issues/22), [US-0016](https://github.com/RalfGuder/MCP-easyVerein/issues/23), [US-0017](https://github.com/RalfGuder/MCP-easyVerein/issues/24), [US-0018](https://github.com/RalfGuder/MCP-easyVerein/issues/25), [US-0019](https://github.com/RalfGuder/MCP-easyVerein/issues/26), [US-0020](https://github.com/RalfGuder/MCP-easyVerein/issues/27), [US-0021](https://github.com/RalfGuder/MCP-easyVerein/issues/28), [US-0022](https://github.com/RalfGuder/MCP-easyVerein/issues/29), [US-0023](https://github.com/RalfGuder/MCP-easyVerein/issues/30), [US-0024](https://github.com/RalfGuder/MCP-easyVerein/issues/31), [US-0025](https://github.com/RalfGuder/MCP-easyVerein/issues/32), [US-0026](https://github.com/RalfGuder/MCP-easyVerein/issues/33), [US-0027](https://github.com/RalfGuder/MCP-easyVerein/issues/34), [US-0028](https://github.com/RalfGuder/MCP-easyVerein/issues/35), [US-0029](https://github.com/RalfGuder/MCP-easyVerein/issues/36), [US-0030](https://github.com/RalfGuder/MCP-easyVerein/issues/37), [US-0031](https://github.com/RalfGuder/MCP-easyVerein/issues/38), [US-0032](https://github.com/RalfGuder/MCP-easyVerein/issues/39), [US-0033](https://github.com/RalfGuder/MCP-easyVerein/issues/40), [US-0034](https://github.com/RalfGuder/MCP-easyVerein/issues/41)
> **Stand:** 2026-04-04

## Übersicht

Dieses Dokument beschreibt die Kernanforderungen an den easyVerein MCP-Server: den lokalen Serverbetrieb, die Authentifizierung, die CRUD-Operationen auf easyVerein-Ressourcen, die Konfiguration sowie die Unterstützung verschiedener API-Versionen.

## Funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| FR-001 | MCP-Server startet lokal und ist über stdio erreichbar | Must | US-0001 |
| FR-002 | Authentifizierung mit easyVerein API-Token | Must | US-0001 |
| FR-003 | Lese-Zugriff auf Mitgliederdaten als MCP-Tool | Must | US-0001 |
| FR-004 | CRUD-Operationen für Mitglieder | Must | US-0001 |
| FR-005 | CRUD-Operationen für Rechnungen | Must | US-0001 |
| FR-045 | CRUD-Operationen für Buchungen | Must | US-0001, US-0009 |
| FR-046 | CRUD-Operationen für Ankündigungen | Must | US-0010 |
| FR-047 | CRUD-Operationen für Bankkonten | Must | US-0011 |
| FR-048 | CRUD-Operationen für Abrechnungskonten | Must | US-0012 |
| FR-049 | CRUD-Operationen für Buchungsprojekte | Must | US-0013 |
| FR-050 | CRUD-Operationen für Kalender | Must | US-0014 |
| FR-051 | CRUD-Operationen für Vorstandsebenen | Must | US-0015 |
| FR-052 | CRUD-Operationen für Kontaktdaten-Gruppen | Must | US-0016 |
| FR-053 | CRUD-Operationen für Kontaktdaten-Änderungsprotokolle | Must | US-0017 |
| FR-054 | CRUD-Operationen für benutzerdefinierte Felder | Must | US-0018 |
| FR-055 | CRUD-Operationen für benutzerdefinierte Feldsammlungen | Must | US-0019 |
| FR-056 | CRUD-Operationen für benutzerdefinierte Filter | Must | US-0020 |
| FR-057 | CRUD-Operationen für benutzerdefinierte Steuersätze | Must | US-0021 |
| FR-058 | CRUD-Operationen für DOSB-Sportarten | Must | US-0022 |
| FR-059 | CRUD-Operationen für Feature-Requests | Must | US-0023 |
| FR-060 | CRUD-Operationen für Forum | Must | US-0024 |
| FR-061 | CRUD-Operationen für Token-Verwaltung | Must | US-0025 |
| FR-062 | CRUD-Operationen für Inventarobjekte | Must | US-0026 |
| FR-063 | CRUD-Operationen für Inventarobjekt-Gruppen | Must | US-0027 |
| FR-064 | CRUD-Operationen für Rechnungspositionen | Must | US-0028 |
| FR-065 | CRUD-Operationen für Ausleihen | Must | US-0029 |
| FR-066 | CRUD-Operationen für Veranstaltungsorte | Must | US-0030 |
| FR-067 | CRUD-Operationen für Mitgliedergruppen | Must | US-0031 |
| FR-068 | CRUD-Operationen für Normalisierung | Must | US-0032 |
| FR-069 | CRUD-Operationen für Benachrichtigungsprotokolle | Must | US-0033 |
| FR-070 | CRUD-Operationen für Organisationen | Must | US-0034 |
| FR-006 | CRUD-Operationen für Veranstaltungen | Must | US-0001 |
| FR-007 | CRUD-Operationen für Kontaktdaten | Must | US-0001 |
| FR-008 | Konfiguration über Umgebungsvariablen (API-Token, Basis-URL) | Must | US-0001 |
| FR-009 | Konfiguration über Konfigurationsdatei als Alternative | Should | US-0001 |
| FR-010 | Unterstützung aller verfügbaren easyVerein API-Versionen (v1, v2, weitere) | Must | US-0005 |
| FR-011 | Fest im Code hinterlegte Liste unterstützter API-Versionen | Must | US-0005 |
| FR-012 | Pro Serverinstanz eine Standard-API-Version | Must | US-0005 |
| FR-013 | API-Version konfigurierbar über Umgebungsvariable (`EASYVEREIN_API_VERSION`) | Must | US-0005 |
| FR-014 | API-Version pro MCP-Tool-Aufruf als Parameter überschreibbar | Should | US-0005 |
| FR-015 | Fehlermeldung mit Vorschlag der nächstmöglichen Version bei ungültiger API-Version | Should | US-0005 |
| FR-016 | SSE als zusätzlicher Transportkanal | Could | US-0001 |
| FR-041 | Konfiguration über Kommandozeilenparameter (`--api-url`, `--api-version`, `--api-key`) | Must | US-0007 |
| FR-042 | CLI-Parameter überschreiben Umgebungsvariablen (Priorität: CLI > Env-Var > Default) | Must | US-0007 |
| FR-043 | Warnung bei fehlendem Konfigurationswert, Fallback auf Standardwert | Must | US-0007 |
| FR-044 | `--help`-Parameter dokumentiert alle verfügbaren Startparameter | Must | US-0007 |

### FR-001: MCP-Server lokal über stdio erreichbar

**Priorität:** Must | **Herkunft:** US-0001

Der MCP-Server muss lokal gestartet werden können und über den stdio-Transportkanal erreichbar sein. Dies ist die primäre Kommunikationsschnittstelle gemäß MCP-Standard.

**Akzeptanzkriterien:**
- [x] Server startet ohne Fehler auf localhost (`Program.cs` mit `Host.CreateApplicationBuilder` + `RunAsync()`)
- [x] Kommunikation über stdio funktioniert gemäß MCP-Protokoll (`WithStdioServerTransport()` in Program.cs)
- [x] Server beendet sich sauber bei Abbruch (via .NET Host Lifecycle)

### FR-002: Authentifizierung mit easyVerein API-Token

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss sich mit einem easyVerein API-Token gegenüber der easyVerein REST-API authentifizieren.

**Akzeptanzkriterien:**
- [x] API-Token wird aus Konfiguration gelesen (`EasyVereinConfiguration.ApiKey`)
- [x] Token wird bei jedem API-Aufruf korrekt im Header mitgesendet (`Authorization: Bearer {token}` in `EasyVereinApiClient`-Konstruktor)
- [x] Ungültige Tokens werden erkannt und mit Fehlermeldung quittiert (`EnsureSuccessOrThrowAsync` behandelt HTTP 401/403)

### FR-003: Lese-Zugriff auf Mitgliederdaten

**Priorität:** Must | **Herkunft:** US-0001

Mindestens der Lese-Zugriff auf Mitgliederdaten muss als MCP-Tool bereitgestellt werden.

**Akzeptanzkriterien:**
- [x] MCP-Tool zum Abrufen von Mitgliederlisten verfügbar (`ListMembers`-Tool)
- [x] MCP-Tool zum Abrufen einzelner Mitgliederdaten verfügbar (`GetMember`-Tool)
- [x] Ergebnisse werden strukturiert zurückgegeben (JSON-serialisierte Rückgabe)

### FR-004 bis FR-007, FR-045 bis FR-062: CRUD-Operationen

**Priorität:** Must | **Herkunft:** US-0001, US-0009 bis US-0026

CRUD-Operationen (Create, Read, Update, Delete) für die zentralen easyVerein-Ressourcen:

| ID | Ressource | API-Endpoint | Akzeptanzkriterien |
|----|-----------|-------------|-------------------|
| FR-004 | Mitglieder | `/member` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-005 | Rechnungen | `/invoice` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-006 | Veranstaltungen | `/event` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-007 | Kontaktdaten | `/contact-details` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-045 | Buchungen | `/booking` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-046 | Ankündigungen | `/announcement` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-047 | Bankkonten | `/bank-account` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-048 | Abrechnungskonten | `/billing-account` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-049 | Buchungsprojekte | `/booking-project` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-050 | Kalender | `/calendar` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-051 | Vorstandsebenen | `/chairman-level` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-052 | Kontaktdaten-Gruppen | `/contact-details-group` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-053 | Kontaktdaten-Änderungsprotokolle | `/contact-details-log` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-054 | Benutzerdefinierte Felder | `/custom-field` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-055 | Benutzerdefinierte Feldsammlungen | `/custom-field-collection` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-056 | Benutzerdefinierte Filter | `/custom-filter` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-057 | Benutzerdefinierte Steuersätze | `/custom-tax-rate` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-058 | DOSB-Sportarten | `/dosb-sport` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-059 | Feature-Requests | `/feature-request` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-060 | Forum | `/forum` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-061 | Token-Verwaltung | `/get-token` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-062 | Inventarobjekte | `/inventory-object` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-063 | Inventarobjekt-Gruppen | `/inventory-object-group` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-064 | Rechnungspositionen | `/invoice-item` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-065 | Ausleihen | `/lending` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-066 | Veranstaltungsorte | `/location` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-067 | Mitgliedergruppen | `/member-group` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-068 | Normalisierung | `/normalize` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-069 | Benachrichtigungsprotokolle | `/notification-log` | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-070 | Organisationen | `/organization` | Anlegen, Abfragen, Bearbeiten, Löschen |

**Akzeptanzkriterien (je Ressource):**
- [x] Alle vier CRUD-Operationen als MCP-Tools verfügbar (Member und ContactDetails vollständig; Invoice fehlt `UpdateInvoice`, Event fehlt `UpdateEvent` als MCP-Tool — API-Client-Methoden vorhanden, aber nicht als Tool exponiert)
- [x] Eingabeparameter werden validiert (via `[Description]`-Attribute)
- [x] Fehler der easyVerein API werden sauber weitergeleitet (`EnsureSuccessOrThrowAsync` in ApiClient)
- [x] PATCH-Requests senden nur geänderte Felder (Dictionary-Ansatz für ContactDetails, nur geänderte Properties für Member)

### FR-045: CRUD-Operationen für Buchungen

**Priorität:** Must | **Herkunft:** US-0001, US-0009

CRUD-Operationen für Buchungen über den separaten easyVerein `/booking`-Endpoint. Buchungen sind von Rechnungen (`/invoice`) getrennte Ressourcen in der API.

**Akzeptanzkriterien:**
- [ ] `ListBookings` MCP-Tool mit Filterung nach ID, Datum, Mitglied
- [ ] `GetBooking` MCP-Tool zum Abrufen einzelner Buchungen
- [ ] `CreateBooking` MCP-Tool zum Anlegen neuer Buchungen
- [ ] `UpdateBooking` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteBooking` MCP-Tool zum Löschen von Buchungen
- [ ] `Booking`-Entity mit `BookingFields`-Konstanten und `BookingQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-050: CRUD-Operationen für Kalender

**Priorität:** Must | **Herkunft:** US-0014

CRUD-Operationen für Kalendereinträge über den easyVerein `/calendar`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListCalendars` MCP-Tool mit Filterung nach ID und Datum
- [ ] `GetCalendar` MCP-Tool zum Abrufen einzelner Kalendereinträge
- [ ] `CreateCalendar` MCP-Tool zum Anlegen neuer Kalendereinträge
- [ ] `UpdateCalendar` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteCalendar` MCP-Tool zum Löschen von Kalendereinträgen
- [ ] `Calendar`-Entity mit `CalendarFields`-Konstanten und `CalendarQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-051: CRUD-Operationen für Vorstandsebenen

**Priorität:** Must | **Herkunft:** US-0015

CRUD-Operationen für Vorstandsebenen über den easyVerein `/chairman-level`-Endpoint. Vorstandsebenen definieren die Berechtigungsstruktur des Vereins.

**Akzeptanzkriterien:**
- [ ] `ListChairmanLevels` MCP-Tool mit Filterung nach ID
- [ ] `GetChairmanLevel` MCP-Tool zum Abrufen einzelner Vorstandsebenen
- [ ] `CreateChairmanLevel` MCP-Tool zum Anlegen neuer Vorstandsebenen
- [ ] `UpdateChairmanLevel` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteChairmanLevel` MCP-Tool zum Löschen von Vorstandsebenen
- [ ] `ChairmanLevel`-Entity mit `ChairmanLevelFields`-Konstanten und `ChairmanLevelQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-052: CRUD-Operationen für Kontaktdaten-Gruppen

**Priorität:** Must | **Herkunft:** US-0016

CRUD-Operationen für Kontaktdaten-Gruppen über den easyVerein `/contact-details-group`-Endpoint. Gruppen dienen der thematischen Gruppierung von Kontakten.

**Akzeptanzkriterien:**
- [ ] `ListContactDetailsGroups` MCP-Tool mit Filterung nach ID
- [ ] `GetContactDetailsGroup` MCP-Tool zum Abrufen einzelner Gruppen
- [ ] `CreateContactDetailsGroup` MCP-Tool zum Anlegen neuer Gruppen
- [ ] `UpdateContactDetailsGroup` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteContactDetailsGroup` MCP-Tool zum Löschen von Gruppen
- [ ] `ContactDetailsGroup`-Entity mit `ContactDetailsGroupFields`-Konstanten und `ContactDetailsGroupQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-053: CRUD-Operationen für Kontaktdaten-Änderungsprotokolle

**Priorität:** Must | **Herkunft:** US-0017

CRUD-Operationen für Änderungsprotokolle von Kontaktdaten über den easyVerein `/contact-details-log`-Endpoint. Protokolle ermöglichen die Nachvollziehbarkeit von Datenänderungen.

**Akzeptanzkriterien:**
- [ ] `ListContactDetailsLogs` MCP-Tool mit Filterung nach ID, Datum und Kontakt-ID
- [ ] `GetContactDetailsLog` MCP-Tool zum Abrufen einzelner Protokolleinträge
- [ ] `CreateContactDetailsLog` MCP-Tool zum Anlegen neuer Protokolleinträge
- [ ] `UpdateContactDetailsLog` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteContactDetailsLog` MCP-Tool zum Löschen von Protokolleinträgen
- [ ] `ContactDetailsLog`-Entity mit `ContactDetailsLogFields`-Konstanten und `ContactDetailsLogQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-054: CRUD-Operationen für benutzerdefinierte Felder

**Priorität:** Must | **Herkunft:** US-0018

CRUD-Operationen für benutzerdefinierte Felder über den easyVerein `/custom-field`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListCustomFields` MCP-Tool mit Filterung nach ID
- [ ] `GetCustomField`, `CreateCustomField`, `UpdateCustomField`, `DeleteCustomField` MCP-Tools
- [ ] `CustomField`-Entity mit `CustomFieldFields`-Konstanten und `CustomFieldQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-055: CRUD-Operationen für benutzerdefinierte Feldsammlungen

**Priorität:** Must | **Herkunft:** US-0019

CRUD-Operationen für benutzerdefinierte Feldsammlungen über den easyVerein `/custom-field-collection`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListCustomFieldCollections` MCP-Tool mit Filterung nach ID
- [ ] `GetCustomFieldCollection`, `CreateCustomFieldCollection`, `UpdateCustomFieldCollection`, `DeleteCustomFieldCollection` MCP-Tools
- [ ] `CustomFieldCollection`-Entity mit `CustomFieldCollectionFields`-Konstanten und `CustomFieldCollectionQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-056: CRUD-Operationen für benutzerdefinierte Filter

**Priorität:** Must | **Herkunft:** US-0020

CRUD-Operationen für benutzerdefinierte Filter über den easyVerein `/custom-filter`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListCustomFilters` MCP-Tool mit Filterung nach ID
- [ ] `GetCustomFilter`, `CreateCustomFilter`, `UpdateCustomFilter`, `DeleteCustomFilter` MCP-Tools
- [ ] `CustomFilter`-Entity mit `CustomFilterFields`-Konstanten und `CustomFilterQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-057: CRUD-Operationen für benutzerdefinierte Steuersätze

**Priorität:** Must | **Herkunft:** US-0021

CRUD-Operationen für benutzerdefinierte Steuersätze über den easyVerein `/custom-tax-rate`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListCustomTaxRates` MCP-Tool mit Filterung nach ID
- [ ] `GetCustomTaxRate`, `CreateCustomTaxRate`, `UpdateCustomTaxRate`, `DeleteCustomTaxRate` MCP-Tools
- [ ] `CustomTaxRate`-Entity mit `CustomTaxRateFields`-Konstanten und `CustomTaxRateQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-058: CRUD-Operationen für DOSB-Sportarten

**Priorität:** Must | **Herkunft:** US-0022

CRUD-Operationen für DOSB-Sportarten über den easyVerein `/dosb-sport`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListDosbSports` MCP-Tool mit Filterung nach ID
- [ ] `GetDosbSport`, `CreateDosbSport`, `UpdateDosbSport`, `DeleteDosbSport` MCP-Tools
- [ ] `DosbSport`-Entity mit `DosbSportFields`-Konstanten und `DosbSportQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-059: CRUD-Operationen für Feature-Requests

**Priorität:** Must | **Herkunft:** US-0023

CRUD-Operationen für Feature-Requests über den easyVerein `/feature-request`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListFeatureRequests` MCP-Tool mit Filterung nach ID
- [ ] `GetFeatureRequest`, `CreateFeatureRequest`, `UpdateFeatureRequest`, `DeleteFeatureRequest` MCP-Tools
- [ ] `FeatureRequest`-Entity mit `FeatureRequestFields`-Konstanten und `FeatureRequestQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-060: CRUD-Operationen für Forum

**Priorität:** Must | **Herkunft:** US-0024

CRUD-Operationen für Forenbeiträge über den easyVerein `/forum`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListForums` MCP-Tool mit Filterung nach ID
- [ ] `GetForum`, `CreateForum`, `UpdateForum`, `DeleteForum` MCP-Tools
- [ ] `Forum`-Entity mit `ForumFields`-Konstanten und `ForumQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-061: CRUD-Operationen für Token-Verwaltung

**Priorität:** Must | **Herkunft:** US-0025

CRUD-Operationen für API-Tokens über den easyVerein `/get-token`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListGetTokens` MCP-Tool mit Filterung nach ID
- [ ] `GetGetToken`, `CreateGetToken`, `UpdateGetToken`, `DeleteGetToken` MCP-Tools
- [ ] `GetToken`-Entity mit `GetTokenFields`-Konstanten und `GetTokenQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-062: CRUD-Operationen für Inventarobjekte

**Priorität:** Must | **Herkunft:** US-0026

CRUD-Operationen für Inventarobjekte über den easyVerein `/inventory-object`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListInventoryObjects` MCP-Tool mit Filterung nach ID
- [ ] `GetInventoryObject`, `CreateInventoryObject`, `UpdateInventoryObject`, `DeleteInventoryObject` MCP-Tools
- [ ] `InventoryObject`-Entity mit `InventoryObjectFields`-Konstanten und `InventoryObjectQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-063: CRUD-Operationen für Inventarobjekt-Gruppen

**Priorität:** Must | **Herkunft:** US-0027

CRUD-Operationen für Inventarobjekt-Gruppen über den easyVerein `/inventory-object-group`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListInventoryObjectGroups` MCP-Tool mit Filterung nach ID
- [ ] `GetInventoryObjectGroup`, `CreateInventoryObjectGroup`, `UpdateInventoryObjectGroup`, `DeleteInventoryObjectGroup` MCP-Tools
- [ ] `InventoryObjectGroup`-Entity mit `InventoryObjectGroupFields`-Konstanten und `InventoryObjectGroupQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-064: CRUD-Operationen für Rechnungspositionen

**Priorität:** Must | **Herkunft:** US-0028

CRUD-Operationen für Rechnungspositionen über den easyVerein `/invoice-item`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListInvoiceItems` MCP-Tool mit Filterung nach ID
- [ ] `GetInvoiceItem`, `CreateInvoiceItem`, `UpdateInvoiceItem`, `DeleteInvoiceItem` MCP-Tools
- [ ] `InvoiceItem`-Entity mit `InvoiceItemFields`-Konstanten und `InvoiceItemQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-065: CRUD-Operationen für Ausleihen

**Priorität:** Must | **Herkunft:** US-0029

CRUD-Operationen für Ausleihen über den easyVerein `/lending`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListLendings` MCP-Tool mit Filterung nach ID
- [ ] `GetLending`, `CreateLending`, `UpdateLending`, `DeleteLending` MCP-Tools
- [ ] `Lending`-Entity mit `LendingFields`-Konstanten und `LendingQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-066: CRUD-Operationen für Veranstaltungsorte

**Priorität:** Must | **Herkunft:** US-0030

CRUD-Operationen für Veranstaltungsorte über den easyVerein `/location`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListLocations` MCP-Tool mit Filterung nach ID
- [ ] `GetLocation`, `CreateLocation`, `UpdateLocation`, `DeleteLocation` MCP-Tools
- [ ] `Location`-Entity mit `LocationFields`-Konstanten und `LocationQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-067: CRUD-Operationen für Mitgliedergruppen

**Priorität:** Must | **Herkunft:** US-0031

CRUD-Operationen für Mitgliedergruppen über den easyVerein `/member-group`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListMemberGroups` MCP-Tool mit Filterung nach ID
- [ ] `GetMemberGroup`, `CreateMemberGroup`, `UpdateMemberGroup`, `DeleteMemberGroup` MCP-Tools
- [ ] `MemberGroup`-Entity mit `MemberGroupFields`-Konstanten und `MemberGroupQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-068: CRUD-Operationen für Normalisierung

**Priorität:** Must | **Herkunft:** US-0032

CRUD-Operationen für die Normalisierungs-Funktion über den easyVerein `/normalize`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListNormalizes` MCP-Tool mit Filterung nach ID
- [ ] `GetNormalize`, `CreateNormalize`, `UpdateNormalize`, `DeleteNormalize` MCP-Tools
- [ ] `Normalize`-Entity mit `NormalizeFields`-Konstanten und `NormalizeQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-069: CRUD-Operationen für Benachrichtigungsprotokolle

**Priorität:** Must | **Herkunft:** US-0033

CRUD-Operationen für Benachrichtigungsprotokolle über den easyVerein `/notification-log`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListNotificationLogs` MCP-Tool mit Filterung nach ID
- [ ] `GetNotificationLog`, `CreateNotificationLog`, `UpdateNotificationLog`, `DeleteNotificationLog` MCP-Tools
- [ ] `NotificationLog`-Entity mit `NotificationLogFields`-Konstanten und `NotificationLogQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-070: CRUD-Operationen für Organisationen

**Priorität:** Must | **Herkunft:** US-0034

CRUD-Operationen für Organisationsdaten über den easyVerein `/organization`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListOrganizations` MCP-Tool mit Filterung nach ID
- [ ] `GetOrganization`, `CreateOrganization`, `UpdateOrganization`, `DeleteOrganization` MCP-Tools
- [ ] `Organization`-Entity mit `OrganizationFields`-Konstanten und `OrganizationQuery`-Klasse
- [ ] Error-Handling und Pagination

### FR-046: CRUD-Operationen für Ankündigungen

**Priorität:** Must | **Herkunft:** US-0010

CRUD-Operationen für Ankündigungen über den easyVerein `/announcement`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListAnnouncements` MCP-Tool mit Filterung nach ID, Datum, Mitglied
- [ ] `GetAnnouncement` MCP-Tool zum Abrufen einzelner Ankündigungen
- [ ] `CreateAnnouncement` MCP-Tool zum Anlegen neuer Ankündigungen
- [ ] `UpdateAnnouncement` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteAnnouncement` MCP-Tool zum Löschen von Ankündigungen
- [ ] `Announcement`-Entity mit `AnnouncementFields`-Konstanten und `AnnouncementQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-047: CRUD-Operationen für Bankkonten

**Priorität:** Must | **Herkunft:** US-0011

CRUD-Operationen für Bankkonten über den easyVerein `/bank-account`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListBankAccounts` MCP-Tool mit Filterung nach ID
- [ ] `GetBankAccount` MCP-Tool zum Abrufen einzelner Bankkonten
- [ ] `CreateBankAccount` MCP-Tool zum Anlegen neuer Bankkonten
- [ ] `UpdateBankAccount` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteBankAccount` MCP-Tool zum Löschen von Bankkonten
- [ ] `BankAccount`-Entity mit `BankAccountFields`-Konstanten und `BankAccountQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-048: CRUD-Operationen für Abrechnungskonten

**Priorität:** Must | **Herkunft:** US-0012

CRUD-Operationen für Abrechnungskonten über den easyVerein `/billing-account`-Endpoint.

**Akzeptanzkriterien:**
- [ ] `ListBillingAccounts` MCP-Tool mit Filterung nach ID
- [ ] `GetBillingAccount` MCP-Tool zum Abrufen einzelner Abrechnungskonten
- [ ] `CreateBillingAccount` MCP-Tool zum Anlegen neuer Abrechnungskonten
- [ ] `UpdateBillingAccount` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteBillingAccount` MCP-Tool zum Löschen von Abrechnungskonten
- [ ] `BillingAccount`-Entity mit `BillingAccountFields`-Konstanten und `BillingAccountQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-049: CRUD-Operationen für Buchungsprojekte

**Priorität:** Must | **Herkunft:** US-0013

CRUD-Operationen für Buchungsprojekte über den easyVerein `/booking-project`-Endpoint. Buchungsprojekte dienen der thematischen Gruppierung von Buchungen.

**Akzeptanzkriterien:**
- [ ] `ListBookingProjects` MCP-Tool mit Filterung nach ID
- [ ] `GetBookingProject` MCP-Tool zum Abrufen einzelner Buchungsprojekte
- [ ] `CreateBookingProject` MCP-Tool zum Anlegen neuer Buchungsprojekte
- [ ] `UpdateBookingProject` MCP-Tool – sendet nur geänderte Felder (PATCH-Dictionary)
- [ ] `DeleteBookingProject` MCP-Tool zum Löschen von Buchungsprojekten
- [ ] `BookingProject`-Entity mit `BookingProjectFields`-Konstanten und `BookingProjectQuery`-Klasse
- [ ] Error-Handling in allen Tool-Methoden
- [ ] Pagination für Listen-Endpunkt

### FR-008: Konfiguration über Umgebungsvariablen

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss über Umgebungsvariablen konfigurierbar sein (mindestens API-Token und Basis-URL). Umgebungsvariablen haben Vorrang vor Standardwerten, werden aber durch CLI-Parameter überschrieben (siehe FR-042).

**Akzeptanzkriterien:**
- [x] `EASYVEREIN_API_KEY` wird als Umgebungsvariable unterstützt
- [x] `EASYVEREIN_API_URL` wird als Umgebungsvariable unterstützt
- [x] `EASYVEREIN_API_VERSION` wird als Umgebungsvariable unterstützt
- [x] Fehlende Pflicht-Variablen führen zu Warnung und Nutzung des Standardwerts (`Resolve()`-Methode mit `LogWarning` und Default-Fallback)

### FR-009: Konfiguration über Konfigurationsdatei

**Priorität:** Should | **Herkunft:** US-0001

Als Alternative zu Umgebungsvariablen soll eine Konfigurationsdatei unterstützt werden.

**Akzeptanzkriterien:**
- [ ] Konfigurationsdatei (z.B. JSON oder YAML) wird unterstützt
- [ ] Umgebungsvariablen haben Vorrang vor Datei-Konfiguration
- [ ] Pfad zur Konfigurationsdatei ist konfigurierbar

### FR-010 bis FR-013: API-Versionierung

**Priorität:** Must | **Herkunft:** US-0005

Die easyVerein API-Versionierung muss vollständig unterstützt werden.

**Akzeptanzkriterien:**
- [x] Liste unterstützter Versionen ist im Code hinterlegt (`ApiVersion._supportedVersions`) (FR-011)
- [x] Standard-Version wird pro Serverinstanz festgelegt (v1.7) (FR-012)
- [x] Umgebungsvariable `EASYVEREIN_API_VERSION` steuert die Version (FR-013)
- [x] API-Aufrufe werden an die jeweilige Version angepasst (`GetVersionedBaseUrl()` in URL-Konstruktion) (FR-010)

### FR-014: API-Version pro Aufruf überschreibbar

**Priorität:** Should | **Herkunft:** US-0005

Die API-Version soll pro MCP-Tool-Aufruf als Parameter überschrieben werden können.

**Akzeptanzkriterien:**
- [ ] Optionaler Parameter `api_version` in jedem MCP-Tool
- [ ] Override hat Vorrang vor Serverinstanz-Konfiguration
- [ ] Ungültige Version wird mit Fehlermeldung abgewiesen

### FR-015: Fehlermeldung bei ungültiger API-Version

**Priorität:** Should | **Herkunft:** US-0005

Bei einer nicht unterstützten API-Version wird eine hilfreiche Fehlermeldung mit Vorschlag ausgegeben.

**Akzeptanzkriterien:**
- [x] Fehlermeldung nennt die angeforderte (ungültige) Version
- [x] Fehlermeldung schlägt die nächstmögliche unterstützte Version vor (`GetClosestVersion()`)
- [x] Liste aller unterstützten Versionen wird angezeigt

### FR-041: Konfiguration über Kommandozeilenparameter

**Priorität:** Must | **Herkunft:** US-0007

Der Server akzeptiert beim Start die Parameter `--api-url`, `--api-version` und `--api-key`.

**Akzeptanzkriterien:**
- [x] `--api-url` setzt die Basis-URL der easyVerein API (Switch-Mapping in Program.cs)
- [x] `--api-version` setzt die zu verwendende API-Version
- [x] `--api-key` setzt den API-Schlüssel für die Authentifizierung
- [ ] Unbekannte Parameter führen zu einer Fehlermeldung mit Hinweis auf `--help` (nicht spezifisch behandelt — `AddCommandLine` ignoriert unbekannte Parameter stillschweigend)

### FR-042: Prioritätsreihenfolge CLI > Env-Var > Default

**Priorität:** Must | **Herkunft:** US-0007

Konfigurationswerte werden in der Reihenfolge CLI-Parameter → Umgebungsvariablen → Standardwerte aufgelöst.

**Akzeptanzkriterien:**
- [x] CLI-Parameter überschreiben gleichnamige Umgebungsvariablen (IConfiguration Provider-Reihenfolge)
- [x] Umgebungsvariablen überschreiben Standardwerte
- [x] Prioritätsreihenfolge ist in `--help` und Dokumentation beschrieben (in --help-Ausgabe)

### FR-043: Warnung bei fehlendem Konfigurationswert

**Priorität:** Must | **Herkunft:** US-0007

Fehlt ein Konfigurationswert auf allen Ebenen (CLI, Env-Var), wird eine Warnung ausgegeben und ein Standardwert verwendet.

**Akzeptanzkriterien:**
- [x] Warnung enthält den Namen des fehlenden Parameters (`logger.LogWarning` mit Key-Name)
- [x] Warnung nennt den verwendeten Standardwert
- [x] Server startet trotz Warnung weiter (kein Abbruch, keine Exception)
- [x] Standardwerte sind im Code dokumentiert (in `PrintHelp()`)

### FR-044: `--help`-Parameter

**Priorität:** Must | **Herkunft:** US-0007

Der Server gibt bei `--help` eine strukturierte Übersicht aller unterstützten Startparameter aus.

**Akzeptanzkriterien:**
- [x] Alle Parameter (`--api-url`, `--api-version`, `--api-key`) sind aufgeführt
- [x] Beschreibung, Typ und Standardwert je Parameter sind angegeben
- [x] Zugehörige Umgebungsvariable je Parameter ist angegeben
- [x] Prioritätsreihenfolge (CLI > Env-Var > Default) ist beschrieben

### FR-016: SSE als Transportkanal

**Priorität:** Could | **Herkunft:** US-0001

SSE (Server-Sent Events) soll als zusätzlicher Transportkanal neben stdio unterstützt werden.

**Akzeptanzkriterien:**
- [ ] SSE-Transportkanal ist implementiert
- [ ] Transportkanal ist konfigurierbar (stdio oder SSE)

---

## Nicht-funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| NFR-001 | Fehlerbehandlung bei ungültigen API-Tokens | Must | US-0001 |
| NFR-002 | Fehlerbehandlung bei Netzwerkfehlern | Must | US-0001 |
| NFR-003 | Konformität mit dem MCP-Standard | Must | US-0001 |
| NFR-004 | Versionsabstraktion erweiterbar mit minimalem Aufwand | Should | US-0005 |
| NFR-005 | Dokumentation: Installations-/Konfigurationsanleitung | Must | US-0001 |
| NFR-006 | Dokumentation der Versionskonfiguration | Should | US-0005 |
| NFR-023 | Dokumentation aller CLI-Parameter und Konfigurationsquellen im README | Must | US-0007 |

### NFR-001: Fehlerbehandlung bei ungültigen API-Tokens

**Priorität:** Must | **Herkunft:** US-0001

Ungültige oder abgelaufene API-Tokens müssen erkannt und mit einer verständlichen Fehlermeldung quittiert werden.

**Akzeptanzkriterien:**
- [x] HTTP 401/403 von der easyVerein API wird abgefangen (`EnsureSuccessOrThrowAsync`)
- [x] Fehlermeldung gibt Hinweis auf Ursache ("Authentifizierung fehlgeschlagen... Bitte prüfen Sie Ihren API-Token")
- [x] Server stürzt nicht ab, sondern gibt den Fehler strukturiert zurück (Exception wird strukturiert zurückgegeben)

### NFR-002: Fehlerbehandlung bei Netzwerkfehlern

**Priorität:** Must | **Herkunft:** US-0001

Netzwerkfehler (Timeout, DNS-Fehler, etc.) müssen sauber behandelt werden.

**Akzeptanzkriterien:**
- [x] Timeout-Fehler werden erkannt und gemeldet (`TaskCanceledException` in `SendWithErrorHandling`)
- [x] DNS-Auflösungsfehler werden erkannt und gemeldet (`SocketException`-Handling)
- [x] Verbindungsabbrüche werden sauber behandelt (`SocketException`-Handling)

### NFR-003: Konformität mit dem MCP-Standard

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss dem MCP-Standard (Model Context Protocol) entsprechen.

**Akzeptanzkriterien:**
- [x] Implementierung folgt der MCP-Spezifikation unter modelcontextprotocol.io (nutzt ModelContextProtocol SDK)
- [x] Tools werden korrekt über `tools/list` und `tools/call` bereitgestellt (via `[McpServerTool]`-Attribute)
- [x] Protokoll-Handshake funktioniert mit MCP-kompatiblen Clients (getestet mit Claude Desktop und anderen MCP-Clients)

### NFR-004: Erweiterbare Versionsabstraktion

**Priorität:** Should | **Herkunft:** US-0005

Die Versionsabstraktion soll so gestaltet sein, dass neue API-Versionen mit minimalem Aufwand ergänzt werden können.

**Akzeptanzkriterien:**
- [x] Neue Version kann durch Hinzufügen einer Konfiguration/Klasse ergänzt werden (String zu `_supportedVersions`-Array hinzufügen)
- [x] Bestehende Versionen werden durch Erweiterung nicht beeinträchtigt

### NFR-005: Installations- und Konfigurationsdokumentation

**Priorität:** Must | **Herkunft:** US-0001

Eine vollständige Installations- und Konfigurationsanleitung muss im README bereitgestellt werden.

**Akzeptanzkriterien:**
- [ ] Installationsschritte sind dokumentiert
- [ ] Konfigurationsoptionen (Umgebungsvariablen, Datei) sind beschrieben
- [ ] Mindestens ein Beispiel für die Nutzung ist enthalten

### NFR-006: Dokumentation der Versionskonfiguration

**Priorität:** Should | **Herkunft:** US-0005

Die Konfiguration der API-Versionierung muss dokumentiert sein.

**Akzeptanzkriterien:**
- [ ] Unterstützte Versionen sind aufgelistet
- [ ] Konfiguration über Umgebungsvariable ist beschrieben
- [ ] Override-Mechanismus pro Aufruf ist dokumentiert

### NFR-023: Dokumentation aller Konfigurationsquellen

**Priorität:** Must | **Herkunft:** US-0007

Alle Konfigurationsquellen (CLI-Parameter, Umgebungsvariablen, Standardwerte) und ihre Prioritätsreihenfolge sind im README dokumentiert.

**Akzeptanzkriterien:**
- [ ] CLI-Parameter (`--api-url`, `--api-version`, `--api-key`) sind im README beschrieben
- [ ] Zugehörige Umgebungsvariablen je Parameter sind aufgeführt
- [ ] Prioritätsreihenfolge CLI > Env-Var > Default ist erklärt
- [ ] Mindestens ein Beispielaufruf mit CLI-Parametern ist enthalten

---

## Abhängigkeiten

| Requirement | hängt ab von | Grund |
|-------------|-------------|-------|
| FR-003 bis FR-007, FR-045 bis FR-070 | FR-002 | CRUD-Operationen benötigen Authentifizierung |
| FR-010 bis FR-015 | FR-001 | Versionierung setzt laufenden Server voraus |
| FR-014 | FR-012, FR-013 | Override setzt Standard-Version voraus |
| FR-042 | FR-008, FR-041 | Prioritätslogik setzt beide Konfigurationsquellen voraus |
| FR-043 | FR-041, FR-042 | Fallback-Logik setzt Prioritätsauflösung voraus |
| FR-044 | FR-041 | --help setzt definierte CLI-Parameter voraus |
| Alle FR | REQ-02 (NFR-008) | Clean Architecture definiert die Projektstruktur |

## Offene Fragen

- [x] ~~Welche easyVerein API-Versionen sind aktuell verfügbar und müssen initial unterstützt werden?~~ v1.4, v1.5, v1.6, v1.7 — implementiert in `ApiVersion.SupportedVersions`
- [ ] Gibt es Rate-Limiting seitens der easyVerein API, das berücksichtigt werden muss?
- [x] ~~Sollen Pagination-Mechanismen für Listenabfragen unterstützt werden?~~ Ja, implementiert: automatisches Abrufen aller Seiten über `next`-Link mit `?limit=100`
