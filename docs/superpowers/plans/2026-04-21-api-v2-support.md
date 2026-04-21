# API-Version v2.0 Support — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add `v2.0` as an accepted value for `EASYVEREIN_API_VERSION`, keeping the default at `v1.7`, so operators can opt into v2.0 without a forced migration.

**Architecture:** Pure additive change to the `ApiVersion` value object — append `"v2.0"` to `_supportedVersions`. All consumers (`EasyVereinConfiguration`, HTTP client, MCP tools, tests) route through that value object and need no further change. TDD: two red tests → one-line green → refactor not required.

**Tech Stack:** .NET 8, C#, xUnit 2.4.2, Moq 4.20.72, Git, GitHub CLI (`gh`).

**Design-Spec:** `docs/superpowers/specs/2026-04-21-api-v2-support-design.md`

---

## Precondition — Working Tree Check

There is an uncommitted modification in `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (Bearer-prefix for the auth header). This is **unrelated parallel work by the maintainer** and is explicitly out of scope for US-0056. The plan takes care not to stage or commit that file.

- [ ] **Verify the uncommitted change is still just the Bearer-prefix edit:**

```bash
git diff --stat
git diff src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
```

Expected: exactly one file listed with `1 insertion(+), 1 deletion(-)`; the diff shows `Bearer {config.ApiKey}` replacing `{config.ApiKey}`. If anything else is modified, **stop and ask the maintainer** before proceeding.

---

## File Structure

| File | Operation | Responsibility |
|---|---|---|
| `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs` | Modify | Append `"v2.0"` to `_supportedVersions`. |
| `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` | Modify | Add two new xUnit facts. |
| `CLAUDE.md` | Modify | Update the "API-Versionen" line. |
| `docs/001 User Stories/056-api-version-v2.md` | Create | User-story markdown per project convention. |
| GitHub Issue `US-0056` | Create | External artefact, linked bidirectionally with the markdown. |

---

## Task 1: Feature-Branch anlegen

**Files:** none (git state only)

- [ ] **Step 1: Ensure the working tree has no conflicting staged changes**

```bash
git status --short
```

Expected output (the only dirty entry is the maintainer's in-progress Bearer-prefix change):

```
 M src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs
```

- [ ] **Step 2: Create and switch to the feature branch**

```bash
git checkout -b feature/US-0056-api-version-v2
```

Expected: `Switched to a new branch 'feature/US-0056-api-version-v2'`. The uncommitted `EasyVereinApiClient.cs` modification travels with us — that is fine; we simply never stage it.

---

## Task 2: GitHub-Issue US-0056 anlegen

**Files:** none (remote artefact only)

- [ ] **Step 1: Create the issue via `gh`**

```bash
gh issue create \
  --title "US-0056 API-Version v2.0 als unterstützte Version hinzufügen" \
  --body "$(cat <<'EOF'
## User Story

**Als** Betreiber eines easyVerein-MCP-Servers, **möchte ich** die API-Version v2.0 explizit konfigurieren können (`EASYVEREIN_API_VERSION=v2.0` oder `--api-version v2.0`), **damit** ich die neue API testen kann, ohne den Default-Stand meiner Produktion zu verändern.

## Akzeptanzkriterien

- [ ] `ApiVersion.SupportedVersions` enthält `"v2.0"`.
- [ ] `ApiVersion.Create("v2.0")` erzeugt eine gültige Instanz.
- [ ] `ApiVersion.Default.Version` liefert weiterhin `"v1.7"`.
- [ ] `EasyVereinConfiguration.FromConfiguration` akzeptiert `v2.0`.
- [ ] Zwei neue Tests in `ApiVersionTests` erfasst.
- [ ] Alle bestehenden Tests bleiben grün.
- [ ] `CLAUDE.md` dokumentiert v2.0 als unterstützte Version.

## Kontext

Sub-Projekt 1 von 10 der v2.0-Migration. Entity-Anpassungen folgen in SP 2–9, der Default-Wechsel in SP 10.

## Links

- Markdown: [docs/001 User Stories/056-api-version-v2.md](docs/001%20User%20Stories/056-api-version-v2.md)
- Design-Spec: [docs/superpowers/specs/2026-04-21-api-v2-support-design.md](docs/superpowers/specs/2026-04-21-api-v2-support-design.md)
EOF
)"
```

Expected: A URL of the form `https://github.com/RalfGuder/MCP-easyVerein/issues/NN` — **note the issue number `NN`; it is needed in Task 6.**

- [ ] **Step 2: Record the issue number for later use**

```bash
gh issue list --search "US-0056 in:title" --json number,title --limit 1
```

Expected JSON with the issue number. Save the number — we'll substitute it into the User-Story markdown.

---

## Task 3: Red-Green-Zyklus für v2.0-Support

**Files:**
- Modify: `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` (append two facts)
- Modify: `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs` (extend array literal)

- [ ] **Step 1: Add two failing tests at the bottom of `ApiVersionTests.cs`**

Open `tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs` and insert these two facts immediately before the final closing brace of the class:

```csharp
    [Fact]
    public void SupportedVersions_Contains_V20()
    {
        Assert.Contains("v2.0", ApiVersion.SupportedVersions);
    }

    [Fact]
    public void Create_WithV20_Succeeds()
    {
        var version = ApiVersion.Create("v2.0");
        Assert.Equal("v2.0", version.Version);
    }
```

- [ ] **Step 2: Run the two new tests and confirm they fail**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj \
  --filter "FullyQualifiedName~SupportedVersions_Contains_V20|FullyQualifiedName~Create_WithV20_Succeeds"
```

Expected: `Failed: 2`. The first test fails on `Assert.Contains`, the second throws `ArgumentException` inside `ApiVersion.Create`.

- [ ] **Step 3: Apply the one-line implementation change**

In `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs`, modify line 9:

```csharp
    private static readonly IReadOnlyList<string> _supportedVersions = new[] { "v1.4", "v1.5", "v1.6", "v1.7" };
```

so it becomes:

```csharp
    private static readonly IReadOnlyList<string> _supportedVersions = new[] { "v1.4", "v1.5", "v1.6", "v1.7", "v2.0" };
```

- [ ] **Step 4: Run the two new tests and confirm they pass**

```bash
dotnet test tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj \
  --filter "FullyQualifiedName~SupportedVersions_Contains_V20|FullyQualifiedName~Create_WithV20_Succeeds"
```

Expected: `Passed: 2, Failed: 0`.

- [ ] **Step 5: Run the full test suite to confirm nothing regressed**

```bash
dotnet test
```

Expected: `Passed: 72, Failed: 0` (previous baseline was 70; we added 2). If the existing baseline is different, the delta should still be exactly `+2 passed, 0 failed`.

- [ ] **Step 6: Stage and commit (code + tests only; do NOT stage `EasyVereinApiClient.cs`)**

```bash
git add src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs \
        tests/MCP.EasyVerein.Domain.Tests/ApiVersionTests.cs
git status --short
```

Expected: the staged files are the two above; the maintainer's `EasyVereinApiClient.cs` is still shown as unstaged (`M` without a preceding space-prefixed `M`).

```bash
git commit -m "$(cat <<'EOF'
feat(domain): add v2.0 to ApiVersion.SupportedVersions

Default remains v1.7. Two new xUnit facts cover the support list
and the Create factory. Part of Sub-Projekt 1 of the v2.0 migration.

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number recorded in Task 2.

---

## Task 4: CLAUDE.md aktualisieren

**Files:** `CLAUDE.md` (modify)

- [ ] **Step 1: Update the "API-Versionen" line**

Find this exact line in `CLAUDE.md`:

```
Unterstützt: v1.4, v1.5, v1.6, v1.7 (Default: v1.7)
```

Replace with:

```
Unterstützt: v1.4, v1.5, v1.6, v1.7, v2.0 (Default: v1.7)
```

- [ ] **Step 2: Stage and commit**

```bash
git add CLAUDE.md
git commit -m "$(cat <<'EOF'
docs: list v2.0 as supported API version

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number.

---

## Task 5: User-Story-Markdown anlegen

**Files:** `docs/001 User Stories/056-api-version-v2.md` (create)

- [ ] **Step 1: Create the file with the full user-story content**

Create `docs/001 User Stories/056-api-version-v2.md` with this exact content (replace `NN` with the real issue number from Task 2):

```markdown
# US-0056 API-Version v2.0 als unterstützte Version hinzufügen

**Issue:** [#NN](https://github.com/RalfGuder/MCP-easyVerein/issues/NN)

## User Story

**Als** Betreiber eines easyVerein-MCP-Servers, **möchte ich** die API-Version v2.0 explizit konfigurieren können (`EASYVEREIN_API_VERSION=v2.0` oder `--api-version v2.0`), **damit** ich die neue API testen kann, ohne den Default-Stand meiner Produktion zu verändern.

## Akzeptanzkriterien

- [x] `ApiVersion.SupportedVersions` enthält `"v2.0"`.
- [x] `ApiVersion.Create("v2.0")` erzeugt eine gültige Instanz.
- [x] `ApiVersion.Default.Version` liefert weiterhin `"v1.7"`.
- [x] `EasyVereinConfiguration.FromConfiguration` akzeptiert `v2.0`.
- [x] Zwei neue Tests in `ApiVersionTests` erfasst.
- [x] Alle bestehenden Tests bleiben grün.
- [x] `CLAUDE.md` dokumentiert v2.0 als unterstützte Version.

## Aufgaben

- Feature-Branch `feature/US-0056-api-version-v2` anlegen
- TDD: zwei Red-Tests → Green-Implementation → Refactor (entfällt)
- `CLAUDE.md` aktualisieren
- PR gegen `main` erstellen

## Technische Hinweise

- Einzige Code-Änderung: `_supportedVersions`-Array in `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs`.
- `ApiVersion.Default.Version` bleibt bewusst `"v1.7"`; der Wechsel des Defaults passiert in einem späteren Sub-Projekt (SP 10) nach vollständiger Entity-Migration.
- Siehe Design-Spec: [`docs/superpowers/specs/2026-04-21-api-v2-support-design.md`](../superpowers/specs/2026-04-21-api-v2-support-design.md)

## Kontext

Teil der v2.0-Migration, Sub-Projekt 1 von 10. Folgende Sub-Projekte (SP 2–9) migrieren die Entities (Member, ContactDetails, Invoice, Event, Booking, Calendar, Announcement, BankAccount). Der Default-Wechsel v1.7 → v2.0 ist Sub-Projekt 10 am Ende der Reihe.
```

- [ ] **Step 2: Stage and commit**

```bash
git add "docs/001 User Stories/056-api-version-v2.md"
git commit -m "$(cat <<'EOF'
docs(user-story): add US-0056 for API v2.0 support

Verlinkt mit GitHub Issue #NN
EOF
)"
```

Replace `#NN` with the issue number.

---

## Task 6: Issue und Markdown bidirektional verlinken

The issue body already points at the markdown path (Task 2). Verify the markdown points back at the correct issue number, then update the issue if the path needs adjustment.

**Files:** none local; updates the GitHub issue body.

- [ ] **Step 1: Double-check the markdown references the correct issue**

```bash
grep -n "^\*\*Issue:\*\*" "docs/001 User Stories/056-api-version-v2.md"
```

Expected: the `**Issue:**` line shows the real issue number (not the literal `NN`). If it still shows `NN`, fix the file, then create a **new** fix commit (do not `git commit --amend`):

```bash
git add "docs/001 User Stories/056-api-version-v2.md"
git commit -m "docs(user-story): fix issue link in US-0056"
```

- [ ] **Step 2: No update required on the issue body**

The issue body from Task 2 already includes a link to `docs/001 User Stories/056-api-version-v2.md`. The file now exists at that path, so the link will resolve once the branch is pushed. No `gh issue edit` is needed.

---

## Task 7: Push + PR erstellen

- [ ] **Step 1: Push the feature branch**

```bash
git push -u origin feature/US-0056-api-version-v2
```

Expected: branch is created on the remote with upstream tracking.

- [ ] **Step 2: Create the pull request**

```bash
gh pr create --title "US-0056 API-Version v2.0 als unterstützte Version hinzufügen" --body "$(cat <<'EOF'
## Summary

- Fügt `v2.0` zu `ApiVersion.SupportedVersions` hinzu (additive Änderung).
- Default bleibt `v1.7`; der Wechsel des Defaults ist Sub-Projekt 10.
- Zwei neue xUnit-Tests in `ApiVersionTests`; alle bestehenden Tests grün.
- `CLAUDE.md` aktualisiert; User Story `docs/001 User Stories/056-api-version-v2.md` angelegt.

Closes #NN.

## Test plan

- [ ] `dotnet test` lokal grün, `+2` gegenüber Baseline (Baseline: 70 Tests).
- [ ] CI-Pipeline (Ubuntu/Windows/macOS) grün.
- [ ] Coverage-Report bleibt ≥ 70 %.

## Referenzen

- Design-Spec: [docs/superpowers/specs/2026-04-21-api-v2-support-design.md](docs/superpowers/specs/2026-04-21-api-v2-support-design.md)
- Plan: [docs/superpowers/plans/2026-04-21-api-v2-support.md](docs/superpowers/plans/2026-04-21-api-v2-support.md)

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

Replace `#NN` with the issue number from Task 2.

Expected: a PR URL of the form `https://github.com/RalfGuder/MCP-easyVerein/pull/MM`.

- [ ] **Step 3: Final sanity check**

```bash
gh pr view --json title,state,isDraft,files
dotnet test
```

Expected: PR state `OPEN`, `isDraft` false, files list contains exactly the four expected files (`ApiVersion.cs`, `ApiVersionTests.cs`, `CLAUDE.md`, `docs/001 User Stories/056-api-version-v2.md`). Tests still pass.

---

## Done Criteria

All items from the design spec's Akzeptanzkriterien are satisfied:

- [ ] `ApiVersion.SupportedVersions` contains `"v2.0"`.
- [ ] `ApiVersion.Create("v2.0")` returns without throwing.
- [ ] `ApiVersion.Default.Version` still returns `"v1.7"`.
- [ ] `EasyVereinConfiguration.FromConfiguration` accepts `v2.0` (covered transitively by `ApiVersion.Create`).
- [ ] All existing tests in Domain/Application/Infrastructure pass.
- [ ] Two new tests in `ApiVersionTests` pass.
- [ ] `CLAUDE.md` lists v2.0 as supported.
- [ ] GitHub issue `US-0056` created; linked both ways with the markdown.
- [ ] User-story markdown `docs/001 User Stories/056-api-version-v2.md` exists.

Uncommitted Bearer-prefix change in `EasyVereinApiClient.cs` remains untouched and un-merged (maintainer's parallel work).
