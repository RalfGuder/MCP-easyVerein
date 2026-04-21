# US-0057 Member-Entity für v2.0-Response-Shape kompatibel machen

**Issue:** [#71](https://github.com/RalfGuder/MCP-easyVerein/issues/71)

## User Story

**Als** Betreiber eines easyVerein-MCP-Servers mit `EASYVEREIN_API_VERSION=v2.0`, **möchte ich**, dass `get_member` und `list_members` auf v2.0 genauso funktionieren wie auf v1.7, **damit** ich die neue API testen kann, ohne Deserialisierungsfehler zu bekommen.

## Akzeptanzkriterien

- [x] `FlexibleDecimalConverter` liest Zahl, String-Zahl, Null; schreibt Zahl oder Null.
- [x] `MemberContactDetailsConverter` liest Null, Object (→ voller `ContactDetails`), URL-String (→ nur `Id`); schreibt Object oder Null.
- [x] v1.7-Fixture deserialisiert: `ContactDetails.FamilyName == "Rose"`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == null`, `ChairmanPermissionGroupId == null`.
- [x] v2.0-Fixture deserialisiert: `ContactDetails.Id == 335684097`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == "https://easyverein.com/api/v2.0/chairman-level/335682768"`, `ChairmanPermissionGroupId == 335682768`.
- [x] Keine Regression in bestehenden Tests.

## Aufgaben

- Feature-Branch `feature/US-0057-member-v2-compat` anlegen.
- `FlexibleDecimalConverter` + Tests.
- `MemberContactDetailsConverter` + Tests.
- `Member`-Entity anpassen (Typ-Wechsel + Attribute + derived Property) + Fixture-Tests.
- User-Story anlegen, PR gegen `main` erstellen.

## Technische Hinweise

- Zwei neue Converter unter `src/MCP.EasyVerein.Domain/Converters/` analog zum bestehenden `FlexibleDateTimeConverter`.
- Strikt YAGNI: keine der 14 neuen v2.0-Felder (DOSB/LSB, memberGroups, customFields, etc.) kommt in die Entity. Sie werden beim Deserialize stillschweigend ignoriert.
- `ChairmanPermissionGroup int?→string?` ist Breaking Change mit nachgewiesenem 0-Consumer-Blast-Radius.
- Siehe Design-Spec: [`docs/superpowers/specs/2026-04-21-member-v2-compat-design.md`](../superpowers/specs/2026-04-21-member-v2-compat-design.md)

## Kontext

Sub-Projekt 2 von 10 der v2.0-Migration. Folgende Sub-Projekte (SP 3–9) wenden das Muster analog auf die übrigen Entities an (ContactDetails, Invoice, Event, Booking, Calendar, Announcement, BankAccount). Der Default-Wechsel v1.7 → v2.0 ist SP 10 am Ende der Reihe.
