# Design: Sub-Projekt 2 — Member-Entity v2.0-Kompatibel (Dual-Format)

**Datum:** 2026-04-21
**Sub-Projekt:** 2 von 10 der v2.0-Migration (SP 1 in PR #70 bereits gemergt)
**User Story:** US-0057 (anzulegen)
**Vorgänger-Spec:** `docs/superpowers/specs/2026-04-21-api-v2-support-design.md`

---

## Kontext

SP 1 hat `v2.0` zu `ApiVersion.SupportedVersions` hinzugefügt; Default bleibt `v1.7`. Wer heute `EASYVEREIN_API_VERSION=v2.0` setzt, bekommt beim `GET /member/{pk}` einen Deserialisierungsfehler, weil drei Felder die Shape geändert haben:

| Feld | v1.7 | v2.0 |
|---|---|---|
| `contactDetails` | Eingebettetes Objekt | URL-String |
| `_chairmanPermissionGroup` | null (Typ `int?` im Code) | URL-String |
| `paymentAmount` | Zahl `0.00` | String `"0.00"` |

Dieses SP macht die `Member`-Entity dual-format-fähig, damit beide Versionen parsen.

## Ziel

`Member` deserialisiert fehlerfrei sowohl echte v1.7- als auch echte v2.0-Responses. Die neuen v2.0-Felder (DOSB/LSB, memberGroups, customFields, declarationOfResignation/Consent, wantsToCancelAt, cancelReason, applicationForm/Kind, org, `_deleteAfterDate`/`_deletedBy`) werden **nicht** in die Entity aufgenommen (strikt YAGNI) — sie werden beim Deserialize stillschweigend ignoriert. Folgen-SPs können sie nachrüsten, wenn ein konkretes MCP-Tool sie benötigt.

## Nicht-Ziele

- Keine neuen v2.0-only Felder in `Member`.
- Keine Änderungen an `ContactDetails`, `MemberGroup`, `ChairmanLevel`, `Organization`.
- Kein Nachladen von referenzierten Ressourcen (URL → voller ContactDetails-Fetch).
- Kein Wechsel des Defaults (bleibt SP 10 vorbehalten).

## Architektur

Dual-Format durch zwei neue `JsonConverter` + eine Typ-Änderung. Das im Projekt etablierte `UrlReference.ExtractId`-Pattern wird weiterverwendet.

```
┌──────────────────────────────────────────────────────────────────────┐
│  Member.cs                                                           │
│                                                                      │
│  [JsonConverter(MemberContactDetailsConverter)]   ContactDetails     │
│  [JsonConverter(FlexibleDecimalConverter)]        PaymentAmount      │
│                                                   ChairmanPermission │
│                                                   Group   (string?)  │
│                                                   ChairmanPermission │
│                                                   GroupId (long? — via UrlReference) │
└──────────────────────────────────────────────────────────────────────┘
         │
         ├─► FlexibleDecimalConverter       (neu; Vorlage: FlexibleDateTimeConverter)
         ├─► MemberContactDetailsConverter  (neu; String → Id-only; Object → voll)
         └─► UrlReference.ExtractId         (bestehend)
```

### FlexibleDecimalConverter (neu)

- Read: `Number` → `reader.GetDecimal()`; `String` → `decimal.Parse(s, CultureInfo.InvariantCulture)`; `Null` → `null`.
- Write: Ist Wert `null` → `WriteNullValue()`; sonst `WriteNumberValue(value.Value)`.

### MemberContactDetailsConverter (neu)

- Read:
  - `Null` → `null`
  - `StartObject` → Standard-Deserialisierung zu `ContactDetails` (`JsonSerializer.Deserialize<ContactDetails>(ref reader, options)`).
  - `String` → `UrlReference.ExtractId(url)` aufrufen; bei `null` (malformed URL) `JsonException` mit dem fehlerhaften Wert werfen; sonst `new ContactDetails { Id = id.Value }` (alle anderen Properties bleiben Default/null; `Id` ist `long`, non-nullable, init-only).
  - Andere Tokens (Number, Array, etc.) → `JsonException`.
- Write:
  - `null` → `WriteNullValue()`.
  - Sonst → Standard-Serialisierung als Objekt (`JsonSerializer.Serialize(writer, value, options)`).

## Dateien

| # | Datei | Operation |
|---|---|---|
| 1 | `src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs` | **Neu** |
| 2 | `src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs` | **Neu** |
| 3 | `src/MCP.EasyVerein.Domain/Entities/Member.cs` | **Modify**: Typ `ChairmanPermissionGroup` `int?`→`string?`; Converter-Attribute auf `ContactDetails` und `PaymentAmount`; neue Read-Only-Property `ChairmanPermissionGroupId` (long?). |
| 4 | `tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs` | **Neu** |
| 5 | `tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs` | **Neu** |
| 6 | `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs` | **Modify**: zwei neue Tests, die die realen Sample-Responses (v1.7 + v2.0) gegen dieselbe `Member`-Klasse parsen. |
| 7 | `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member/member-v1.7.json` | **Neu** (Fixture aus realem Smoke-Test) |
| 8 | `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member/member-v2.0.json` | **Neu** (Fixture aus realem Smoke-Test) |
| 9 | `docs/001 User Stories/057-member-v2-compat.md` | **Neu** |

## Breaking-Change-Analyse

Ausgangspunkt: `Member.ChairmanPermissionGroup` wechselt Typ von `int?` zu `string?`.

`grep -r ChairmanPermissionGroup` findet Treffer nur in:

- `src/MCP.EasyVerein.Domain/Entities/Member.cs` (wird in diesem SP geändert)
- `src/MCP.EasyVerein.Domain/ValueObjects/MemberFields.cs` (Field-Konstante, bleibt)
- `docs/005 SuperPowers/plans/2026-04-03-api-feldmapping.md` (Doku, nicht Code)
- `docs/005 SuperPowers/specs/2026-04-03-api-feldmapping-design.md` (Doku, nicht Code)

**Keine Tests, keine Server-Tools, kein HTTP-Client** greifen auf das Feld zu. Blast-Radius = null; Typwechsel ist gefahrlos.

## TDD-Ablauf

1. **Red**: Tests für `FlexibleDecimalConverter` (Number, String, Null, Write-Roundtrip) → schlagen fehl (Converter fehlt).
2. **Green**: Converter implementieren → grün.
3. **Red**: Tests für `MemberContactDetailsConverter` (Null, Object, String-URL, falscher Token) → schlagen fehl.
4. **Green**: Converter implementieren → grün.
5. **Red**: Fixtures v1.7 + v2.0 anlegen, Tests `Deserialize_V17Fixture_FullContactDetailsEmbedded` und `Deserialize_V20Fixture_ContactDetailsIdOnly_PaymentAmountAsString` schreiben → schlagen fehl.
6. **Green**: `Member.cs` anpassen (Typ ändern, Attribute setzen, neue Property) → grün.
7. **Verify**: volle Test-Suite grün, keine Regression.
8. **Commits** einzeln pro logischem Schritt (Converter 1, Converter 2, Entity + Fixtures).

## Akzeptanzkriterien

- [ ] `FlexibleDecimalConverter` liest Zahl, String-Zahl und Null; schreibt als Zahl oder Null.
- [ ] `MemberContactDetailsConverter` liest Null, Object (→ voller `ContactDetails`), URL-String (→ nur `Id`); schreibt Object oder Null.
- [ ] v1.7-Fixture deserialisiert: `ContactDetails.FamilyName == "Rose"`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == null`, `ChairmanPermissionGroupId == null`.
- [ ] v2.0-Fixture deserialisiert: `ContactDetails.Id == 335684097`, `ContactDetails.FamilyName == null`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == "https://easyverein.com/api/v2.0/chairman-level/335682768"`, `ChairmanPermissionGroupId == 335682768`.
- [ ] Alle bestehenden Tests in Domain/Application/Infrastructure bleiben grün.
- [ ] Coverage ≥ 70 %.
- [ ] GitHub-Issue **US-0057** angelegt, verlinkt mit User-Story-Markdown.
- [ ] User-Story-Markdown `docs/001 User Stories/057-member-v2-compat.md` existiert und verlinkt auf Issue.
- [ ] CLAUDE.md-Projektstatus ggf. angepasst (neuer User-Story-Count).

## Branch / PR

- **Branch:** `feature/US-0057-member-v2-compat`
- **Issue-Titel:** `US-0057 Member-Entity für v2.0-Response-Shape kompatibel machen`
- **PR:** gegen `main`.

## Risiken

- **FlexibleDecimalConverter-Kultur**: `decimal.Parse` MUSS `CultureInfo.InvariantCulture` verwenden, sonst wird `"0.00"` auf deutschen Systemen als `0` (Zehntausender-Gruppierung) oder Exception. Abgedeckt durch expliziten Test mit String-Wert auf Worker-Ebene.
- **ContactDetails-Default-Werte**: `new ContactDetails { Id = … }` füllt alle anderen Properties mit Defaults (null/0/empty). Konsumenten, die v2.0-Responses lesen, sehen leere Felder anstelle echter Werte. Das ist bewusst — wer mehr will, kann `get_contact_details(id)` aufrufen.
- **Kein Nachladen**: Ausdrücklich out-of-scope. Falls später nötig, ist das ein eigenes SP.

## Folge-Sub-Projekte

SP 3–9 (ContactDetails, Invoice, Event, Booking, Calendar, Announcement, BankAccount) werden analog vorgehen. SP 10 wechselt den Default.
