# US-0059 billing-account: accountingPlan-Feld aus Query und Entity entfernen

**Issue:** [#92](https://github.com/RalfGuder/MCP-easyVerein/issues/92)

## User Story

**Als** Betreiber des easyVerein-MCP-Servers,
**möchte ich**, dass `list_billing_accounts` und `get_billing_account` wieder funktionieren und keinen HTTP 400 zurückgeben,
**damit** ich Buchungskonten auflisten und per ID abrufen kann und somit auch Automatisierungen wie die Porto-Buchungsregel (US-0058-Nachnutzung) IDs zuverlässig auflösen können.

## Fehlerbild

```
HttpRequestException: HTTP 400 (Bad Request): ["'accountingPlan' field is not found"]
```

Tritt bei jedem Aufruf von `list_billing_accounts` und `get_billing_account` auf.

## Root Cause

`src/MCP.EasyVerein.Infrastructure/ApiClient/BillingAccountQuery.cs:44-56` fragt in der `query=`-Feld-Selection das Feld `accountingPlan` an. Laut v2.0-Spec (`docs/api/easyverein-v2.0.yaml` Z. 6566ff.) und v1.7-Spec existiert `accountingPlan` als Response-Feld **nicht** — nur der Filter `accountingPlan__isnull` ist erlaubt. In v2.0 validiert die API Feldnamen strikt und wirft HTTP 400.

## Akzeptanzkriterien

- [ ] `BillingAccountFields.AccountingPlan` wird aus der `FieldQuery`-Konstante in `BillingAccountQuery.cs` entfernt.
- [ ] `BillingAccount.AccountingPlan` (Entity-Property) und `BillingAccountFields.AccountingPlan` (Konstante) werden entfernt, da sie in keiner API-Version (v1.4–v2.0) Teil der Response sind.
- [ ] Der Filter-Parameter `accountingPlan__isnull` (`BillingAccountFields.AccountingPlanIsNull`) bleibt erhalten — er ist laut v2.0-Spec gültig.
- [ ] `list_billing_accounts` liefert ohne Fehler eine Liste zurück.
- [ ] `get_billing_account` liefert ohne Fehler einen einzelnen Eintrag zurück.
- [ ] Infrastructure-Tests decken das geänderte Query-Verhalten ab (erwarteter Query-String enthält `accountingPlan` nicht mehr im Feld-Selektor, enthält aber `accountingPlan__isnull` als Filter).
- [ ] Domain-Tests für `BillingAccount`-Entity werden an die entfernte Property angepasst.
- [ ] Test-Suite bleibt grün, Coverage ≥ 70 %.

## Aufgaben

- Feature-Branch `feature/US-0059-billing-account-accountingplan-fix` (bereits angelegt).
- TDD: Failing-Test in `Infrastructure.Tests` für `BillingAccountQuery.ToString()` → `accountingPlan` nicht mehr im Feld-Selektor.
- Entity-Property & Konstante entfernen, Tests anpassen.
- MCP-Tool-Aufrufe live gegen API verifizieren (Smoketest nach Deployment).
- PR gegen `main`, CI grün, Review.

## Technische Hinweise

- Keine Breaking-Change-Bedenken: Property war ohnehin bisher durch die API immer auf null gesetzt oder gar nicht existent, Consumer haben keinen sinnvollen Wert gelesen.
- v2.0-Spec zeigt: POST/PATCH/PUT-Request-Body nimmt nur `name`, `excludeInEur`, `number`, `defaultSphere` entgegen. Unsere Create/Update-Tools dürften daher von der Entfernung ebenfalls nicht negativ betroffen sein.
- Verwandt: Porto-Buchungsregel (Memory), die via `list_billing_accounts` das Konto 68000 auflösen will — aktuell scheitert diese Auflösung.
