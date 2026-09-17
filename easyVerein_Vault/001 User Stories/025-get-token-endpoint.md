# User Story 025: Get-Token-Endpoint implementieren

> **GitHub Issue:** [#32 – US-0025 Get-Token-Endpoint implementieren](https://github.com/RalfGuder/MCP-easyVerein/issues/32)

## User Story

**Als** Vereinsadministrator,
**möchte ich** mich über den MCP-Server mit Benutzername und Passwort bei easyVerein anmelden und prüfen können, ob ein API-Token ausgestellt wird,
**damit** ich Zugangsdaten (inkl. Zwei-Faktor-Anmeldung) direkt aus dem MCP-Server heraus verifizieren kann.

> **Umfang angepasst (2026-09-17):** Ursprünglich war CRUD geplant. `get-token` ist jedoch ein reiner **Login-Endpoint** (`POST`); GET → 405. Die Verwaltung von Tokens (Liste, Anlegen, Löschen) läuft über `organization-token` und ist Gegenstand von US-0038 (#52). Auf Entscheidung des Product Owners: Login-Tool, Token wird **nur maskiert** zurückgegeben.

## Akzeptanzkriterien

- [x] **Entity `GetTokenResult`:** Antwort mit `token` und `needs2FA`, `[JsonPropertyName]` über `GetTokenFields`
- [x] **ValueObject `GetTokenFields.cs`:** Feldnamen `username`, `password`, `2FA`, `token`, `needs2FA`
- [x] **API-Client:** `GetTokenAsync(username, password, twoFactorCode)` → `POST get-token`; `2FA` nur, wenn angegeben
- [x] **MCP-Tool:** `GetTokenTools.cs` mit `get_token` – inkl. Error-Handling
- [x] **Sicherheit:** Token nur maskiert (erste/letzte 4 Zeichen + Länge; kurze Tokens vollständig maskiert); Passwort wird in allen Fehlermeldungen geschwärzt; Tool-Beschreibung trägt einen **SENSITIVE**-Hinweis
- [x] **Pflichtfelder:** `username` und `password` werden vor dem API-Aufruf geprüft
- [x] **2FA:** `needs2FA` → Hinweis, das Tool mit `twoFactorCode` erneut aufzurufen
- [x] **Tests:** Unit-Tests nach TDD (Red-Green-Refactor), +16 (2 Domain + 5 Infrastructure + 9 Server)

## Aufgaben

1. easyVerein API-Dokumentation für den `get-token`-Endpoint analysieren
2. `GetTokenFields.cs`, `GetTokenResult.cs` erstellen
3. ~~`GetTokenQuery.cs` / `ApiQueries.cs`~~ (entfällt: kein Lese-Endpoint)
4. `IEasyVereinApiClient` und `EasyVereinApiClient` um `GetTokenAsync` erweitern
5. `GetTokenTools.cs` als MCP-Tool-Klasse erstellen
6. `Program.cs` um Registrierung erweitern
7. Unit-Tests schreiben
8. Manuelle Verifikation gegen die easyVerein API (nur mit ungültigen Zugangsdaten, siehe unten)

## Technische Hinweise

- easyVerein API-Doku: https://easyverein.com/api/documentation/
- Endpoint: `POST /get-token` (Allow: POST, OPTIONS); funktioniert mit und ohne `Authorization`-Header
  - Body: `username` (Pflicht, Format `$orgShort_$emailOrUsername`, z. B. `abc_some@example.de`), `password` (Pflicht), `2FA` (optional)
  - Antwort 200: `token` oder `needs2FA: true`
  - 400 bei falschen Zugangsdaten: `{"non_field_errors": ["Die angegebenen Zugangsdaten stimmen nicht."]}`; bei fehlenden Feldern je Feld „Dieses Feld ist zwingend erforderlich.“
- Verwandte Endpoints (nicht Teil dieser Story): `organization-token` (US-0038), `refresh-token` (GET, neuer Token mit dem aktuellen Key – würde den konfigurierten Key ggf. ablösen), `member/me/get-ev-community-token`
- **Sicherheit:** Das Passwort läuft als Tool-Parameter durch den MCP-Kontext (Chatverlauf/Transkripte). Das Tool nur aufrufen, wenn der Nutzer Zugangsdaten ausdrücklich dafür angibt.
- Live-Verifikation (2026-09-17) mit der gebauten Tool-Klasse, **nur mit ungültigen Fantasie-Zugangsdaten**: 400 wird als `ERROR` gemeldet (auch mit 2FA-Code), leeres Passwort wird vorab abgewiesen. Ein **erfolgreicher Login und die 2FA-Abfrage sind nicht live geprüft** (keine Test-Zugangsdaten); beides ist per Unit-Test abgesichert.
- Priorität: **Mittel**
