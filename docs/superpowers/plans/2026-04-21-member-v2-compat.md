# Member-Entity v2.0-Kompatibel Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Teach the `Member` entity to deserialize both v1.7 and v2.0 easyVerein API responses without parse errors, by adding two custom `JsonConverter`s and one type change — without adding any of the 14 new v2.0-only fields.

**Architecture:** Two new `JsonConverter`s (`FlexibleDecimalConverter` for number-or-string decimals, `MemberContactDetailsConverter` for embedded-object-or-URL-string). `Member.ChairmanPermissionGroup` changes from `int?` to `string?` (zero consumers outside the entity, verified via grep), and a new derived `ChairmanPermissionGroupId` exposes the long id via the existing `UrlReference.ExtractId` helper. Real-world response fixtures (v1.7 and v2.0) drive the final roundtrip tests.

**Tech Stack:** .NET 8, C#, xUnit 2.4.2, System.Text.Json, `CopyToOutputDirectory` for test fixtures.

**Design-Spec:** `docs/superpowers/specs/2026-04-21-member-v2-compat-design.md`

---

## Precondition — Working Tree + Branch Check

- [ ] **Verify you are on `main`, the tree is clean, and SP 1 is merged:**

```bash
git fetch origin
git branch --show-current
git status --short
git log --oneline -3 main
```

Expected: branch is `main`; `git status` outputs nothing; `git log` shows `cd544bd docs(spec): add design for Sub-Projekt 2 — Member v2.0 compat` as the tip or very close.

If anything else is modified in the working tree, STOP and ask the maintainer before proceeding.

---

## File Structure

| File | Operation | Responsibility |
|---|---|---|
| `src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs` | Create | Read `decimal?` from `Number` or `String` tokens; write as `Number` or `Null`. |
| `src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs` | Create | Read `ContactDetails` from an embedded object (v1.7) or a URL-ref string (v2.0, populates `Id` only); write as object or null. |
| `src/MCP.EasyVerein.Domain/Entities/Member.cs` | Modify | Change `ChairmanPermissionGroup` from `int?` to `string?`; attach converter attributes to `ContactDetails` and `PaymentAmount`; add derived `ChairmanPermissionGroupId`. |
| `tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs` | Create | xUnit facts for Number, String-Number, Null, invalid token, and Write-roundtrip. |
| `tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs` | Create | xUnit facts for Null, Object, String-URL, malformed URL, wrong token. |
| `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs` | Modify | Two new fact methods reading the fixture files; assert per-version expectations. |
| `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json` | Create | Real v1.7 response trimmed to the relevant fields. |
| `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v2.0.json` | Create | Real v2.0 response trimmed to the relevant fields. |
| `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj` | Modify | Add a `<None Update>` entry for the two new fixtures. |
| `docs/001 User Stories/057-member-v2-compat.md` | Create | User-story markdown per project convention. |
| GitHub Issue `US-0057` | Create | External artefact, bidirectionally linked with the markdown. |

---

## Task 1: Feature-Branch anlegen

- [ ] **Step 1: Verify clean working tree**

```bash
git status --short
```

Expected: empty output.

- [ ] **Step 2: Create and switch to the feature branch**

```bash
git checkout -b feature/US-0057-member-v2-compat
```

Expected: `Switched to a new branch 'feature/US-0057-member-v2-compat'`.

---

## Task 2: GitHub-Issue US-0057 anlegen

- [ ] **Step 1: Create the issue**

```bash
gh issue create --title "US-0057 Member-Entity für v2.0-Response-Shape kompatibel machen" --body "$(cat <<'EOF'
## User Story

**Als** Betreiber eines easyVerein-MCP-Servers mit `EASYVEREIN_API_VERSION=v2.0`, **möchte ich**, dass `get_member` / `list_members` auf v2.0 genauso funktionieren wie auf v1.7, **damit** ich die neue API testen kann, ohne Deserialisierungsfehler zu bekommen.

## Akzeptanzkriterien

- [ ] `FlexibleDecimalConverter` liest Zahl, String-Zahl, Null; schreibt Zahl oder Null.
- [ ] `MemberContactDetailsConverter` liest Null, Object (→ voller `ContactDetails`), URL-String (→ nur `Id`); schreibt Object oder Null.
- [ ] v1.7-Fixture deserialisiert: `ContactDetails.FamilyName == "Rose"`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == null`, `ChairmanPermissionGroupId == null`.
- [ ] v2.0-Fixture deserialisiert: `ContactDetails.Id == 335684097`, `PaymentAmount == 0.00m`, `ChairmanPermissionGroup == "https://easyverein.com/api/v2.0/chairman-level/335682768"`, `ChairmanPermissionGroupId == 335682768`.
- [ ] Alle bestehenden Tests grün, Coverage ≥ 70 %.

## Kontext

Sub-Projekt 2 von 10 der v2.0-Migration. Strikt YAGNI: keine der 14 neuen v2.0-Felder werden aufgenommen. Typ-Wechsel `ChairmanPermissionGroup int?→string?` ist Breaking Change mit nachgewiesenem 0-Consumer-Blast-Radius.

## Links

- Markdown: [docs/001 User Stories/057-member-v2-compat.md](docs/001%20User%20Stories/057-member-v2-compat.md)
- Design-Spec: [docs/superpowers/specs/2026-04-21-member-v2-compat-design.md](docs/superpowers/specs/2026-04-21-member-v2-compat-design.md)
- Vorgänger-PR: #70 (SP 1)
EOF
)"
```

Expected: an issue URL like `https://github.com/RalfGuder/MCP-easyVerein/issues/NN`. **Note the issue number `NN`** — used in every subsequent commit message and in Task 7.

- [ ] **Step 2: Record the issue number**

```bash
gh issue list --search "US-0057 in:title" --json number --limit 1
```

Expected: JSON containing the issue number.

---

## Task 3: FlexibleDecimalConverter (Red-Green)

**Files:**
- Create: `src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs` with this exact content:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleDecimalConverterTests
{
    private sealed class Wrapper
    {
        [JsonConverter(typeof(FlexibleDecimalConverter))]
        public decimal? Value { get; set; }
    }

    [Fact]
    public void Read_NumberToken_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":0.00}");
        Assert.Equal(0.00m, w!.Value);
    }

    [Fact]
    public void Read_StringNumberToken_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"0.00\"}");
        Assert.Equal(0.00m, w!.Value);
    }

    [Fact]
    public void Read_StringNumberTokenWithFraction_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"12.34\"}");
        Assert.Equal(12.34m, w!.Value);
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_EmptyString_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"\"}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_BooleanToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Wrapper>("{\"Value\":true}"));
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_DecimalValue_WritesJsonNumber()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = 1.23m });
        Assert.Equal("{\"Value\":1.23}", json);
    }
}
```

- [ ] **Step 2: Run the new tests and confirm they fail to compile (Converter does not exist yet)**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~FlexibleDecimalConverterTests"
```

Expected: build error `CS0246: The type or namespace name 'FlexibleDecimalConverter' could not be found`.

- [ ] **Step 3: Create the converter**

Create `src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs` with this exact content:

```csharp
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for nullable <see cref="decimal"/> that accepts either a numeric token
/// (easyVerein API v1.7 monetary fields) or a string-encoded number (easyVerein API v2.0).
/// </summary>
public sealed class FlexibleDecimalConverter : JsonConverter<decimal?>
{
    /// <summary>Reads a nullable <see cref="decimal"/> accepting Number or String tokens.</summary>
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                return reader.GetDecimal();
            case JsonTokenType.String:
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return null;
                return decimal.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture);
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when parsing decimal.");
        }
    }

    /// <summary>Writes a nullable <see cref="decimal"/> as JSON number or null.</summary>
    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Value);
    }
}
```

- [ ] **Step 4: Run the tests and confirm they pass**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~FlexibleDecimalConverterTests"
```

Expected: `Passed: 8, Failed: 0`.

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs \
        tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs
git commit -m "$(cat <<'EOF'
feat(domain): add FlexibleDecimalConverter for number-or-string decimals

Supports easyVerein API v2.0 which returns monetary fields like
paymentAmount as JSON strings ("0.00") instead of numbers.

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

---

## Task 4: MemberContactDetailsConverter (Red-Green)

**Files:**
- Create: `src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs`
- Create: `tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs` with this exact content:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class MemberContactDetailsConverterTests
{
    private sealed class Wrapper
    {
        [JsonConverter(typeof(MemberContactDetailsConverter))]
        public ContactDetails? Value { get; set; }
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_UrlString_PopulatesOnlyId()
    {
        const string json = "{\"Value\":\"https://easyverein.com/api/v2.0/contact-details/335684097\"}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(335684097L, w.Value!.Id);
        Assert.Null(w.Value.FamilyName);
        Assert.Null(w.Value.FirstName);
    }

    [Fact]
    public void Read_EmbeddedObject_PopulatesFullEntity()
    {
        const string json = "{\"Value\":{\"id\":42,\"familyName\":\"Rose\",\"firstName\":\"Kathleen\"}}";
        var w = JsonSerializer.Deserialize<Wrapper>(json);
        Assert.NotNull(w!.Value);
        Assert.Equal(42L, w.Value!.Id);
        Assert.Equal("Rose", w.Value.FamilyName);
        Assert.Equal("Kathleen", w.Value.FirstName);
    }

    [Fact]
    public void Read_MalformedUrl_ThrowsJsonException()
    {
        const string json = "{\"Value\":\"https://example.com/no-id-here/\"}";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Wrapper>(json));
    }

    [Fact]
    public void Read_NumberToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Wrapper>("{\"Value\":123}"));
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_EntityValue_WritesEmbeddedObject()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = new ContactDetails { Id = 7 } });
        Assert.Contains("\"id\":7", json);
    }
}
```

- [ ] **Step 2: Run the new tests and confirm they fail to compile**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~MemberContactDetailsConverterTests"
```

Expected: build error `CS0246: The type or namespace name 'MemberContactDetailsConverter' could not be found`.

- [ ] **Step 3: Create the converter**

Create `src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs` with this exact content:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for <see cref="ContactDetails"/> on <see cref="Member"/> that accepts either
/// a full embedded object (easyVerein API v1.7) or a URL-reference string (easyVerein API v2.0).
/// For URL references, only <see cref="ContactDetails.Id"/> is populated; other properties
/// remain at their default values.
/// </summary>
public sealed class MemberContactDetailsConverter : JsonConverter<ContactDetails?>
{
    /// <summary>Reads a <see cref="ContactDetails"/> from an embedded object or a URL reference.</summary>
    public override ContactDetails? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize<ContactDetails>(ref reader, options);
            case JsonTokenType.String:
                var url = reader.GetString();
                var id = UrlReference.ExtractId(url);
                if (id is null)
                    throw new JsonException($"Cannot extract ContactDetails id from URL: '{url}'.");
                return new ContactDetails { Id = id.Value };
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} for contactDetails.");
        }
    }

    /// <summary>Writes a <see cref="ContactDetails"/> as an embedded object or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, ContactDetails? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else JsonSerializer.Serialize(writer, value, options);
    }
}
```

- [ ] **Step 4: Run the tests and confirm they pass**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~MemberContactDetailsConverterTests"
```

Expected: `Passed: 7, Failed: 0`.

- [ ] **Step 5: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs \
        tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs
git commit -m "$(cat <<'EOF'
feat(domain): add MemberContactDetailsConverter for dual-format reads

easyVerein API v1.7 returns contactDetails as an embedded object; v2.0
returns a URL reference. The converter accepts both shapes and yields
either a full ContactDetails entity or one with only Id populated.

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

---

## Task 5: Fixtures + Member-Entity + Entity-Tests (Red-Green)

**Files:**
- Create: `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json`
- Create: `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v2.0.json`
- Modify: `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj` (add fixture copy rules)
- Modify: `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs` (add two roundtrip tests)
- Modify: `src/MCP.EasyVerein.Domain/Entities/Member.cs` (type change, converters, new property)

- [ ] **Step 1: Create the v1.7 fixture**

Create `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json` with this exact content:

```json
{
  "_applicationDate": null,
  "_applicationWasAcceptedAt": null,
  "_chairmanPermissionGroup": null,
  "_editableByRelatedMembers": false,
  "_isApplication": false,
  "_isBlocked": false,
  "_isChairman": false,
  "_isMatrixSearchable": false,
  "_matrixCommunicationPermission": null,
  "_paymentStartDate": "2025-01-01T00:00:00+01:00",
  "_profilePicture": null,
  "_relatedMember": null,
  "blockedFromMatrix": false,
  "blockReason": null,
  "bulletinBoardNewPostNotification": false,
  "contactDetails": {
    "id": 335684097,
    "familyName": "Rose",
    "firstName": "Kathleen",
    "primaryEmail": "kathleen.rose@web.de",
    "dateOfBirth": "1959-09-16"
  },
  "declarationOfApplication": "https://easyverein.com/app/file/?category=cert&path=144_Kathleen_Rose/1762011939695247-251020_KathleenRose.pdf",
  "emailOrUserName": "kathleen.rose@web.de",
  "id": 4410903,
  "joinDate": "2025-10-20T12:00:00+02:00",
  "matrixBlockReason": null,
  "membershipNumber": "144",
  "paymentAmount": 0.00,
  "paymentIntervallMonths": -1,
  "relatedMembers": [],
  "requirePasswordChange": false,
  "resignationDate": null,
  "resignationNoticeDate": null,
  "sepaMandateFile": null,
  "showWarningsAndNotesToAdminsInProfile": false,
  "signatureText": null,
  "useBalanceForMembershipFee": true,
  "useMatrixGroupSettings": false
}
```

- [ ] **Step 2: Create the v2.0 fixture**

Create `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v2.0.json` with this exact content:

```json
{
  "joinDate": "2025-10-20T12:00:00+02:00",
  "resignationDate": null,
  "resignationNoticeDate": null,
  "declarationOfApplication": "https://easyverein.com/app/file/?category=cert&path=144_Kathleen_Rose/1762011939695247-251020_KathleenRose.pdf",
  "_paymentStartDate": "2025-01-01T00:00:00+01:00",
  "paymentAmount": "0.00",
  "paymentIntervallMonths": -1,
  "_relatedMember": null,
  "useBalanceForMembershipFee": true,
  "bulletinBoardNewPostNotification": false,
  "integrationDosbSport": [],
  "integrationDosbGender": null,
  "integrationLsbSport": [],
  "integrationLsbGender": null,
  "_isApplication": false,
  "membershipNumber": "144",
  "relatedMembers": [],
  "org": "https://easyverein.com/api/v2.0/organization/30189",
  "id": 4410903,
  "_deleteAfterDate": null,
  "_deletedBy": null,
  "declarationOfResignation": null,
  "declarationOfConsent": null,
  "sepaMandateFile": null,
  "memberGroups": ["https://easyverein.com/api/v2.0/member/4410903/groups/335695794"],
  "customFields": [],
  "_applicationDate": null,
  "_applicationWasAcceptedAt": null,
  "_isChairman": true,
  "_chairmanPermissionGroup": "https://easyverein.com/api/v2.0/chairman-level/335682768",
  "_profilePicture": "https://easyverein.com/app/image/17620983394159784.png",
  "contactDetails": "https://easyverein.com/api/v2.0/contact-details/335684097",
  "emailOrUserName": "kathleen.rose@web.de",
  "signatureText": "",
  "_editableByRelatedMembers": false,
  "requirePasswordChange": false,
  "_isBlocked": false,
  "blockReason": "",
  "wantsToCancelAt": null,
  "cancelReason": null,
  "showWarningsAndNotesToAdminsInProfile": true,
  "applicationForm": null,
  "_isMatrixSearchable": true,
  "matrixBlockReason": null,
  "blockedFromMatrix": false,
  "_matrixCommunicationPermission": 2,
  "useMatrixGroupSettings": true,
  "applicationKind": null
}
```

- [ ] **Step 3: Register the fixtures in the test project**

Modify `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj`. Find this existing block:

```xml
  <ItemGroup>
    <None Update="Fixtures\invoice-real-response.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

Add two sibling entries inside the same `<ItemGroup>`, resulting in:

```xml
  <ItemGroup>
    <None Update="Fixtures\invoice-real-response.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="Fixtures\member-v1.7.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="Fixtures\member-v2.0.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

- [ ] **Step 4: Add the failing entity tests**

Open `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs`. Add these two xUnit facts inside the existing `MemberEntityTests` class (before the closing `}`):

```csharp
    [Fact]
    public void Deserialize_V17Fixture_FullContactDetailsEmbedded()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "member-v1.7.json"));

        var member = JsonSerializer.Deserialize<Member>(json);

        Assert.NotNull(member);
        Assert.Equal(4410903L, member!.Id);
        Assert.Equal("144", member.MembershipNumber);
        Assert.NotNull(member.ContactDetails);
        Assert.Equal(335684097L, member.ContactDetails!.Id);
        Assert.Equal("Rose", member.ContactDetails.FamilyName);
        Assert.Equal("Kathleen", member.ContactDetails.FirstName);
        Assert.Equal(0.00m, member.PaymentAmount);
        Assert.Null(member.ChairmanPermissionGroup);
        Assert.Null(member.ChairmanPermissionGroupId);
    }

    [Fact]
    public void Deserialize_V20Fixture_ContactDetailsAsUrlRef()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "member-v2.0.json"));

        var member = JsonSerializer.Deserialize<Member>(json);

        Assert.NotNull(member);
        Assert.Equal(4410903L, member!.Id);
        Assert.Equal("144", member.MembershipNumber);
        Assert.NotNull(member.ContactDetails);
        Assert.Equal(335684097L, member.ContactDetails!.Id);
        // ContactDetails.FamilyName is `string` (non-nullable) with default string.Empty,
        // so URL-ref parses leave it as empty rather than null.
        Assert.Equal(string.Empty, member.ContactDetails.FamilyName);
        Assert.Equal(0.00m, member.PaymentAmount);
        Assert.Equal("https://easyverein.com/api/v2.0/chairman-level/335682768",
                     member.ChairmanPermissionGroup);
        Assert.Equal(335682768L, member.ChairmanPermissionGroupId);
    }
```

The file may already have a `using System.Text.Json;`. If it does not, add it at the top. Add `using System.IO;` only if not already present.

- [ ] **Step 5: Run the tests and confirm they fail to build**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~Deserialize_V17Fixture|FullyQualifiedName~Deserialize_V20Fixture"
```

Expected: **build error** because `member.ChairmanPermissionGroupId` does not exist yet on `Member` (`CS1061: 'Member' does not contain a definition for 'ChairmanPermissionGroupId'`). This is the red state.

- [ ] **Step 6: Modify `Member.cs`**

Open `src/MCP.EasyVerein.Domain/Entities/Member.cs`.

**6a. Add a new `using`** at the top, beside the existing `using MCP.EasyVerein.Domain.ValueObjects;`:

```csharp
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Helpers;
```

**6b. Change the `ChairmanPermissionGroup` property type from `int?` to `string?`.** Find this block:

```csharp
    /// <summary>
    /// Gets or sets the chairman permission group ID. Maps to API field ' <c>_chairmanPermissionGroup</c>'.
    /// </summary>
    [JsonPropertyName(MemberFields.ChairmanPermissionGroup)] 
    public int? ChairmanPermissionGroup { get; set; }
```

Replace with:

```csharp
    /// <summary>
    /// Gets or sets the chairman permission group URL reference (v2.0) or null (v1.7).
    /// Maps to API field ' <c>_chairmanPermissionGroup</c>'.
    /// </summary>
    [JsonPropertyName(MemberFields.ChairmanPermissionGroup)] 
    public string? ChairmanPermissionGroup { get; set; }

    /// <summary>
    /// Gets the numeric chairman-level id extracted from <see cref="ChairmanPermissionGroup"/>,
    /// or <c>null</c> if the field is null or not a resource URL.
    /// </summary>
    [JsonIgnore]
    public long? ChairmanPermissionGroupId => UrlReference.ExtractId(ChairmanPermissionGroup);
```

**6c. Attach the converter to `ContactDetails`.** Find this block:

```csharp
    /// <summary>
    /// Gets or sets the associated contact details. Maps to API field ' <c>contactDetails</c>'.
    /// </summary>
    [JsonPropertyName(MemberFields.ContactDetails)] 
    public ContactDetails? ContactDetails { get; set; }
```

Replace with:

```csharp
    /// <summary>
    /// Gets or sets the associated contact details. Maps to API field ' <c>contactDetails</c>'.
    /// In v1.7 this is an embedded object; in v2.0 it is a URL reference from which only
    /// the <see cref="Entities.ContactDetails.Id"/> is populated.
    /// </summary>
    [JsonPropertyName(MemberFields.ContactDetails)] 
    [JsonConverter(typeof(MemberContactDetailsConverter))]
    public ContactDetails? ContactDetails { get; set; }
```

**6d. Attach the converter to `PaymentAmount`.** Find this block:

```csharp
    /// <summary>
    /// Gets or sets the payment amount. Maps to API field ' <c>paymentAmount</c>'.
    /// </summary>
    [JsonPropertyName(MemberFields.PaymentAmount)] 
    public decimal? PaymentAmount { get; set; }
```

Replace with:

```csharp
    /// <summary>
    /// Gets or sets the payment amount. Maps to API field ' <c>paymentAmount</c>'.
    /// Accepts both number (v1.7) and string-number (v2.0) wire shapes.
    /// </summary>
    [JsonPropertyName(MemberFields.PaymentAmount)] 
    [JsonConverter(typeof(FlexibleDecimalConverter))]
    public decimal? PaymentAmount { get; set; }
```

- [ ] **Step 7: Run the new entity tests and confirm they pass**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj --filter "FullyQualifiedName~Deserialize_V17Fixture|FullyQualifiedName~Deserialize_V20Fixture"
```

Expected: `Passed: 2, Failed: 0`.

- [ ] **Step 8: Run the full suite**

```bash
dotnet test
```

Expected: every test green. If a pre-existing test stored a specific `int` value in `ChairmanPermissionGroup` it may break — grep showed no such test, but if one appears, adjust it to use `string?`/`null`.

- [ ] **Step 9: Commit**

```bash
git add src/MCP.EasyVerein.Domain/Entities/Member.cs \
        tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs \
        tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj \
        tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json \
        tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v2.0.json
git commit -m "$(cat <<'EOF'
feat(domain): make Member entity v1.7+v2.0 dual-format compatible

ChairmanPermissionGroup: int? -> string? (grep shows zero consumers
outside the entity). Adds derived ChairmanPermissionGroupId. Attaches
MemberContactDetailsConverter to ContactDetails and
FlexibleDecimalConverter to PaymentAmount. Covered by two fixture-
driven roundtrip tests using real API responses.

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

---

## Task 6: User-Story-Markdown anlegen

**Files:**
- Create: `docs/001 User Stories/057-member-v2-compat.md`

- [ ] **Step 1: Create the file**

Create `docs/001 User Stories/057-member-v2-compat.md` with this exact content (replace `NN` with the real issue number from Task 2):

```markdown
# US-0057 Member-Entity für v2.0-Response-Shape kompatibel machen

**Issue:** [#NN](https://github.com/RalfGuder/MCP-easyVerein/issues/NN)

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
```

- [ ] **Step 2: Stage and commit**

```bash
git add "docs/001 User Stories/057-member-v2-compat.md"
git commit -m "$(cat <<'EOF'
docs(user-story): add US-0057 for Member v2.0 compat

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

- [ ] **Step 3: Verify the link is real (not literal `NN`)**

```bash
grep -n "^\*\*Issue:\*\*" "docs/001 User Stories/057-member-v2-compat.md"
```

Expected: the `**Issue:**` line shows the real issue number. If it still shows literal `NN`, fix the file and create a **new** follow-up commit:

```bash
git add "docs/001 User Stories/057-member-v2-compat.md"
git commit -m "docs(user-story): fix issue link in US-0057"
```

---

## Task 7: Push + PR erstellen

**CRITICAL: Do NOT run `git reset --hard`, `git rebase`, `git push --force`, or `git commit --amend`.** Push the branch as-is. If `git push` is rejected, STOP and report BLOCKED — do not rewrite history.

- [ ] **Step 1: Push the feature branch**

```bash
git push -u origin feature/US-0057-member-v2-compat
```

Expected: the branch is created on origin with upstream tracking.

- [ ] **Step 2: Create the pull request**

```bash
gh pr create --title "US-0057 Member-Entity für v2.0-Response-Shape kompatibel machen" --body "$(cat <<'EOF'
## Summary

- `Member`-Entity wird v1.7+v2.0-dualformat-fähig.
- Neu: `FlexibleDecimalConverter` (Zahl/String-Zahl), `MemberContactDetailsConverter` (Object/URL).
- Breaking: `ChairmanPermissionGroup` wechselt `int?` → `string?` (grep zeigt 0 Consumer außerhalb der Entity).
- Neue Read-Only-Property `ChairmanPermissionGroupId` (long?) via `UrlReference.ExtractId`.
- Keine neuen v2.0-Felder (strikt YAGNI — 14 Felder absichtlich ausgelassen).
- Zwei Fixture-getriebene Roundtrip-Tests mit echten API-Responses.

Closes #NN.

## Test plan

- [ ] `dotnet test` lokal grün, Baseline +17 Tests (8 FlexibleDecimalConverter + 7 MemberContactDetailsConverter + 2 Member-Fixture), keine Regression.
- [ ] CI-Pipeline (Ubuntu/Windows/macOS) grün.
- [ ] Coverage ≥ 70 %.

## Referenzen

- Design-Spec: [docs/superpowers/specs/2026-04-21-member-v2-compat-design.md](docs/superpowers/specs/2026-04-21-member-v2-compat-design.md)
- Plan: [docs/superpowers/plans/2026-04-21-member-v2-compat.md](docs/superpowers/plans/2026-04-21-member-v2-compat.md)
- Vorgänger: PR #70 (SP 1)

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

Expected: a PR URL of the form `https://github.com/RalfGuder/MCP-easyVerein/pull/MM`.

- [ ] **Step 3: Sanity-check the PR**

```bash
gh pr view --json title,state,isDraft,files -q '{title, state, isDraft, fileCount: (.files | length), files: [.files[].path]}'
```

Expected: state `OPEN`, isDraft false. Files should be **exactly these eight**:

- `src/MCP.EasyVerein.Domain/Converters/FlexibleDecimalConverter.cs`
- `src/MCP.EasyVerein.Domain/Converters/MemberContactDetailsConverter.cs`
- `src/MCP.EasyVerein.Domain/Entities/Member.cs`
- `tests/MCP.EasyVerein.Domain.Tests/FlexibleDecimalConverterTests.cs`
- `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj`
- `tests/MCP.EasyVerein.Domain.Tests/MemberContactDetailsConverterTests.cs`
- `tests/MCP.EasyVerein.Domain.Tests/MemberEntityTests.cs`
- `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json`
- `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v2.0.json`
- `docs/001 User Stories/057-member-v2-compat.md`

Any other files means something leaked in — STOP and report.

---

## Done Criteria

All acceptance criteria from the design spec are satisfied:

- [ ] Both converters implemented and tested.
- [ ] `Member.ChairmanPermissionGroup` is `string?`; `ChairmanPermissionGroupId` returns the extracted long or null.
- [ ] Both fixtures round-trip successfully through `Member`.
- [ ] Full test suite green.
- [ ] GitHub issue `US-0057` created; linked both ways with the markdown.
- [ ] User-story markdown `docs/001 User Stories/057-member-v2-compat.md` exists.
- [ ] PR open against `main`.
