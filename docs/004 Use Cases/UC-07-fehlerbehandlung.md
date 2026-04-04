# UC-07: Fehlerbehandlung

> **Status:** Implementiert
> **Herkunft:** [US-001](https://github.com/RalfGuder/MCP-easyVerein/issues/1)
> **Requirements:** NFR-001, NFR-002, NFR-003
> **Beziehung:** Wird von UC-02 bis UC-05 und UC-08 per `«include»` eingebunden

## Kurzbeschreibung

Der MCP-Server behandelt Fehler bei der Kommunikation mit der easyVerein API strukturiert und gibt verständliche Fehlermeldungen an den KI-Assistenten zurück, ohne abzustürzen.

## Akteure

| Akteur | Rolle |
|--------|-------|
| **KI-Assistent** | Empfänger der Fehlermeldung |
| **easyVerein API** | Quelle des Fehlers |

## Vorbedingungen

- MCP-Server ist gestartet
- KI-Assistent hat ein MCP-Tool aufgerufen
- Beim API-Aufruf tritt ein Fehler auf

## Fehlerszenarien

### F1: Ungültiger API-Token (HTTP 401/403)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | System | Sendet API-Request mit konfiguriertem Token |
| 2 | easyVerein API | Gibt HTTP 401 (Unauthorized) oder 403 (Forbidden) zurück |
| 3 | System | Fängt den Statuscode in `EnsureSuccessOrThrowAsync` ab |
| 4 | System | Wirft `UnauthorizedAccessException` mit Meldung: „Authentifizierung fehlgeschlagen (HTTP 401). Bitte prüfen Sie Ihren API-Token." |
| 5 | MCP-Tool | Fängt die Exception und gibt sie als `ERROR:`-Meldung zurück |
| 6 | KI-Assistent | Informiert den Benutzer über das Token-Problem |

### F2: Netzwerkfehler (keine Verbindung)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | System | Versucht API-Request zu senden |
| 2 | Netzwerk | Verbindung schlägt fehl (`SocketException`) |
| 3 | System | Fängt `SocketException` in `SendWithErrorHandling` ab |
| 4 | System | Wirft `InvalidOperationException` mit Meldung: „Netzwerkfehler: Verbindung zum easyVerein-Server konnte nicht hergestellt werden. Bitte prüfen Sie Ihre Internetverbindung." |
| 5 | MCP-Tool | Gibt Fehlermeldung an KI-Assistenten zurück |

### F3: Timeout

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | System | Sendet API-Request |
| 2 | easyVerein API | Antwortet nicht innerhalb des Timeouts |
| 3 | System | Fängt `TaskCanceledException` (ohne User-Cancellation) in `SendWithErrorHandling` ab |
| 4 | System | Wirft `InvalidOperationException` mit Meldung: „Zeitüberschreitung: Die easyVerein-API hat nicht rechtzeitig geantwortet." |

### F4: API-Fehler (HTTP 4xx/5xx)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | System | Sendet API-Request |
| 2 | easyVerein API | Gibt HTTP-Fehler zurück (z.B. 400, 500) |
| 3 | System | Liest den Response-Body aus |
| 4 | System | Wirft `HttpRequestException` mit Meldung: „HTTP 400 (Bad Request): {response body}" |
| 5 | MCP-Tool | Gibt die vollständige Fehlermeldung mit Statuscode und API-Antwort zurück |

### F5: Ressource nicht gefunden (HTTP 404)

| Schritt | Akteur | Aktion |
|---------|--------|--------|
| 1 | System | Sendet GET-Request für eine einzelne Ressource |
| 2 | easyVerein API | Gibt HTTP 404 zurück |
| 3 | System | Gibt `null` zurück (keine Exception) |
| 4 | MCP-Tool | Gibt benutzerfreundliche Meldung zurück: „[Ressource] mit ID X nicht gefunden." |

## Nachbedingungen

- **In allen Fällen:** Der Server läuft weiter und kann weitere Anfragen verarbeiten
- **Keine Abstürze:** Alle Fehler werden gefangen und strukturiert zurückgegeben

## Fehlerbehandlungs-Hierarchie

```
MCP-Tool-Aufruf
  └─ try/catch
      ├─ API-Client-Methode
      │   └─ SendWithErrorHandling()
      │       ├─ SocketException → InvalidOperationException (Netzwerkfehler)
      │       └─ TaskCanceledException → InvalidOperationException (Timeout)
      │   └─ EnsureSuccessOrThrowAsync()
      │       ├─ HTTP 401/403 → UnauthorizedAccessException
      │       ├─ HTTP 404 → return null
      │       └─ HTTP 4xx/5xx → HttpRequestException (mit Body)
      └─ catch (Exception ex)
          └─ return "ERROR: {Type}: {Message}"
```

## Implementierungsdetails

| Komponente | Datei |
|-----------|-------|
| Fehlerbehandlung API-Client | `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` – Methoden `SendWithErrorHandling`, `EnsureSuccessOrThrowAsync` |
| Error-Handling in Tools | `src/MCP.EasyVerein.Server/Tools/MemberTools.cs`, `ContactDetailsTools.cs`, `InvoiceTools.cs` (try/catch) |
| Tests | `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` – Tests für 401, 403, 400, 404, 500 |
