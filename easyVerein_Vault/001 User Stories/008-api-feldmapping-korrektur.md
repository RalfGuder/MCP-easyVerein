# User Story 008: API-Feldmapping und Query-Parameter korrigieren

> **GitHub Issue:** [#14 – US-0008 API-Feldmapping und Query-Parameter korrigieren](https://github.com/RalfGuder/MCP-easyVerein/issues/14)

## User Story

**Als** Vereinsadministrator,
**möchte ich** vollständige Mitgliedsdaten (Name, E-Mail, Adresse, Telefon) und korrekte Daten aller Endpunkte über den MCP-Server abrufen können,
**damit** ich Vereinsinformationen zuverlässig verwalten und auswerten kann.

## Hintergrund

Die easyVerein API trennt Daten in separate Ressourcen (`member`, `contact-details`, `invoice`, `event`), verwendet eigene Feldnamen (z.B. `familyName` statt `LastName`, `zip` statt `ZipCode`) und erfordert einen `query`-Parameter zur Feldauswahl. Ohne diesen Parameter liefert die API nur minimale Daten. Zudem ist die Standard-Pagination auf 5 Einträge limitiert.

## Akzeptanzkriterien

- [x] **Domain-Model refactored:** `Member` und `ContactDetails` sind getrennte Domain-Entities (entsprechend der API-Struktur)
- [x] **JSON-Mapping korrekt:** Alle Entities verwenden `[JsonPropertyName]`-Attribute, die den tatsächlichen API-Feldnamen entsprechen (z.B. `familyName`, `membershipNumber`, `zip`, `emailOrUserName`)
- [x] **Query-Parameter:** Alle API-Aufrufe (GET) verwenden den `query`-Parameter zur Feldauswahl mit korrekter Syntax `{field1,field2,nested{subfield}}`
- [x] **Pagination vollständig:** Alle Listen-Endpunkte rufen automatisch alle Seiten ab (über `next`-Link), sodass immer alle Datensätze zurückgegeben werden
- [x] **CRUD vollständig:** Create, Read, Update und Delete für alle Endpunkte funktionieren korrekt mit der tatsächlichen API-Struktur (z.B. Member-Erstellung zweistufig: erst ContactDetails, dann Member)
- [x] **Alle Endpunkte korrigiert:** Member, Invoice, Event und Contact-Details sind an die korrekte API-Struktur angepasst
- [x] **Tests:** 42 Tests bestanden (19 Domain + 10 Infrastructure + 13 Application), TDD-Ansatz
- [x] **MCP-Tools aktualisiert:** Die MCP-Server-Tools geben vollständige, korrekte Daten zurück

> **Abgeschlossen:** 2026-04-03 via PR [#15](https://github.com/RalfGuder/MCP-easyVerein/pull/15)

## Aufgaben

1. easyVerein API-Dokumentation für alle Endpunkte analysieren (Feldnamen, Datentypen, Beziehungen)
2. Domain-Entities refactoren: `Member` und `ContactDetails` trennen, `[JsonPropertyName]`-Attribute ergänzen
3. Weitere Entities (`Invoice`, `Event`) auf korrekte Feldnamen prüfen und anpassen
4. `EasyVereinApiClient` anpassen: `query`-Parameter an alle GET-Aufrufe anhängen
5. Pagination implementieren: automatisches Abrufen aller Seiten über `next`-Link
6. Create/Update-Logik für Member anpassen (zweistufig: ContactDetails -> Member)
7. MCP-Tools (`MemberTools`, `InvoiceTools`, `EventTools`, `ContactTools`) aktualisieren
8. Unit- und Integrationstests schreiben (TDD)
9. Manuelle Verifikation gegen die easyVerein API

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- API-Feldauswahl via `query`-Parameter: `?query={field1,field2,nested{sub1,sub2}}`
- Pagination: `?limit=100` (max), automatisch `next`-URL folgen
- Member -> ContactDetails ist eine Komposition (FK `contactDetails` auf Member)
- Member-Erstellung: POST `/contact-details` -> POST `/member` mit `contactDetails`-Referenz
- Feldnamen-Mapping (Auswahl):
  - `LastName` -> `familyName` (contact-details)
  - `FirstName` -> `firstName` (contact-details)
  - `Email` -> `emailOrUserName` (member) / `privateEmail` (contact-details)
  - `MemberNumber` -> `membershipNumber` (member)
  - `ZipCode` -> `zip` (contact-details)
  - `IsActive` -> abgeleitet aus `resignationDate`
- Priorität: **Hoch** – ohne korrekte Daten ist der MCP-Server nicht nutzbar
