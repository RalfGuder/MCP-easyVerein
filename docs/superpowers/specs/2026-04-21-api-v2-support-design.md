# Design: Sub-Projekt 1 — API-Version v2.0 als unterstützte Version hinzufügen

**Datum:** 2026-04-21
**Sub-Projekt:** 1 von (voraussichtlich) 10 der v2.0-Migration
**User Story:** US-0056 (anzulegen)
**Status:** Design approved; Implementierungsplan ausstehend

---

## Kontext

Die easyVerein-API bietet seit kurzem `v2.0` neben den bisher unterstützten Versionen `v1.4`–`v1.7` an. Unser MCP-Server beschränkt `EASYVEREIN_API_VERSION` aktuell auf die v1.x-Reihe und würde `v2.0` mit `ArgumentException` ablehnen.

Ein Smoke-Test gegen `GET /api/v2.0/member/{pk}` hat Breaking Changes im Response-Schema aufgedeckt (u.a. `contactDetails` als URL-Reference statt eingebettetes Objekt, neue Pflichtfelder für DOSB/LSB, `paymentAmount` als String). Die Migration der Entities ist deshalb in Sub-Projekte 2–9 zerlegt; der Default-Wechsel ist ein separates Sub-Projekt 10 am Ende der Reihe.

Dieses Sub-Projekt 1 ist bewusst minimal gehalten: **nur** `v2.0` als unterstützt markieren, Default **unverändert** bei `v1.7` belassen.

## Ziel

Anwender können `EASYVEREIN_API_VERSION=v2.0` bzw. `--api-version v2.0` setzen, ohne dass das Value Object ablehnt. Produktions-Setups ohne explizite Versionsangabe bleiben unangetastet.

## Nicht-Ziele

- Keine Entity-Migration (Member, ContactDetails, Invoice, Event, Booking, Calendar, Announcement, BankAccount).
- Keine Änderungen am HTTP-Client oder Query-Builder.
- Kein Wechsel des Default-Version-Strings (kommt in Sub-Projekt 10).
- Kein `Bearer`-Prefix für den Auth-Header (wird parallel durch den Maintainer bearbeitet).

## Architektur

Das Value Object `ApiVersion` ist die einzige Stelle, an der die Liste der akzeptierten Versionen definiert ist. Alle anderen Komponenten (`EasyVereinConfiguration`, HTTP-Client, MCP-Tools, Tests) lesen `SupportedVersions`/`Default` über dieses Value Object. Eine Erweiterung der Liste genügt.

```
EasyVereinConfiguration.FromEnvironment / FromConfiguration
                │
                ▼  validiert via ApiVersion.Create(…)
   ┌────────────────────────────┐
   │   ApiVersion (Value Obj.)  │ ← einziger Änderungspunkt
   │   SupportedVersions: [ … ] │
   │   DefaultVersion: "v1.7"   │
   └────────────────────────────┘
```

## Änderungen

| # | Datei | Änderung |
|---|-------|----------|
| 1 | `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs` | `_supportedVersions`-Literal um `"v2.0"` erweitern. `DefaultVersion` bleibt `"v1.7"`. |
| 2 | `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` | Zwei neue Tests: `SupportedVersions_Contains_V20`, `Create_WithV20_Succeeds`. |
| 3 | `CLAUDE.md` | "Unterstützt: v1.4, v1.5, v1.6, v1.7 (Default: v1.7)" → "Unterstützt: v1.4, v1.5, v1.6, v1.7, v2.0 (Default: v1.7)". |
| 4 | `docs/001 User Stories/056-api-version-v2.md` | Neue User-Story-Markdown im Projektformat. |

Existierende Tests bleiben unverändert: `Default_Is_V17`, `Create_WithSupportedVersion_Succeeds`, `IsSupported_ReturnsTrueForValid`, `GetVersionedBaseUrl_*` etc. assertieren entweder den Default (bleibt v1.7) oder konkret `v1.7`/`v1.6` — das ist korrekt.

## TDD-Ablauf

1. **Red:** `SupportedVersions_Contains_V20` + `Create_WithV20_Succeeds` schreiben. Beide schlagen mit `ArgumentException` / Assert-Fehler fehl.
2. **Green:** `"v2.0"` ans Ende von `_supportedVersions` anhängen. Beide Tests werden grün. Alle bestehenden Tests bleiben grün.
3. **Refactor:** Nicht nötig.

## Test-Coverage

Ziel: ≥ 70 % (Projektvorgabe). Da dieser Change rein additiv in einem bereits getesteten Value Object wirkt, ändert sich die Coverage nicht relevant.

## Akzeptanzkriterien (für US-0056)

- [ ] `ApiVersion.SupportedVersions` enthält `"v2.0"`.
- [ ] `ApiVersion.Create("v2.0")` liefert eine Instanz zurück, ohne zu werfen.
- [ ] `ApiVersion.Default.Version` liefert weiterhin `"v1.7"`.
- [ ] `EasyVereinConfiguration.FromConfiguration` akzeptiert `v2.0` als `EASYVEREIN_API_VERSION`.
- [ ] Alle bestehenden Tests in `Domain.Tests`, `Application.Tests`, `Infrastructure.Tests` bleiben grün.
- [ ] Zwei neue Tests in `ApiVersionTests` erfasst, beide grün.
- [ ] `CLAUDE.md` dokumentiert v2.0 als unterstützte Version.
- [ ] GitHub-Issue `US-0056` angelegt, verlinkt auf die User-Story-Markdown.
- [ ] User-Story-Datei `docs/001 User Stories/056-api-version-v2.md` existiert und verlinkt auf das GitHub-Issue.

## Branch / PR

- **Branch:** `feature/US-0056-api-version-v2`
- **Issue-Titel:** `US-0056 API-Version v2.0 als unterstützte Version hinzufügen`
- **PR:** gegen `main`, Reviewer = Maintainer.

## Risiken

- **Gering.** Additive Änderung. Einzige Möglichkeit einer Regression wäre ein irgendwo hartkodierter Check auf genau die v1.x-Liste — grep nach `v1.7`/`v1.6` im Code zeigt: nur Tests und CLAUDE.md. Keine Branch-Logik hängt an der Versionsliste.
- **Kein Deployment-Risiko**, da der Default nicht bewegt wird. Existierende Clients laufen unverändert.

## Folge-Sub-Projekte (nur zur Einordnung, nicht Teil dieses Specs)

| SP | Thema | Grob-Scope |
|----|-------|------------|
| 2 | Member-Entity v2.0-kompatibel | `contactDetails` als URL-Ref, neue DOSB/LSB-Felder, String-Amounts |
| 3 | ContactDetails-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 4 | Invoice-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 5 | Event-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 6 | Booking-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 7 | Calendar-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 8 | Announcement-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 9 | BankAccount-Entity v2.0 | Schema-Abgleich + Smoke-Test |
| 10 | Default-Wechsel v1.7 → v2.0 | Einzeiler in `ApiVersion.DefaultVersion` + Test-Anpassung + Doku |
