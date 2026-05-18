# US-0062 — ApiQueries Static-Singleton → per-call Query-Instanzen — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:subagent-driven-development` (recommended) or `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Eliminiere die static-shared mutable `ApiQueries`-Klasse vollständig. Jeder `ListXxxAsync` baut seine Query-Instanz lokal; jeder `GetXxxAsync` nutzt ausschließlich `XxxQuery.FieldQuery`. Damit keine Filter-Leaks zwischen Aufrufen und keine Race-Conditions bei parallelen MCP-Tool-Calls.

**Architecture:** Pro Query-Klasse (`AnnouncementQuery`, `BankAccountQuery`, `BillingAccountQuery`, `BookingQuery`, `BookingProjectQuery`, `CalendarQuery`, `ChairmanLevelQuery`, `ContactDetailsQuery`, `EventQuery`, `InvoiceItemQuery`, `MemberQuery`) wird die `FieldQuery`-const auf `internal` hochgezogen (Booking bereits erledigt). Die korrespondierende `ListXxxAsync`-Methode in `EasyVereinApiClient` instanziiert `new XxxQuery { ... }` lokal; `GetXxxAsync` ruft `XxxQuery.FieldQuery` direkt auf. Die `Invoice`-Spezialbehandlung (eigene `InvoiceQuery`-Klasse existiert, wird aber nicht genutzt) wird in Task 11 konsolidiert. Am Ende wird `ApiQueries.cs` gelöscht.

**Tech Stack:** .NET 8 / C# / xUnit. Test-Pattern: `CapturingFakeHttpHandler` (einzelner Request) und `MultiPageFakeHttpHandler` (Sequenz, mit `LastRequestUri` seit PR #101).

---

## Vorbedingungen

- Branch: `feature/US-0062-per-call-query-instanzen` (bereits erstellt, mit User-Story-Doc als HEAD-Commit `af47be3`)
- Baseline: `main @ 74b8fef`, 173 Tests grün
- Testlauf für Baseline-Verifikation:
  ```
  dotnet test --nologo --verbosity minimal
  ```
  Erwartet: `erfolgreich: 173`

---

## Task 1: Refactor-Pattern an `Booking` etablieren (ListBookingsAsync + Concurrency-Test)

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (Methode `ListBookingsAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` (neuer Regressions-Test + Concurrency-Test)

Hintergrund: PR #101 hat `GetBookingAsync` und `BookingQuery.FieldQuery` schon umgestellt. `ListBookingsAsync` mutiert weiter `ApiQueries.BookingQuery`. Diese Task vollendet die Migration für Booking und etabliert die beiden neuen Test-Patterns (List→Get-Sequenz pro Endpoint hat #101 bereits; jetzt fehlt nur noch der Concurrency-Smoke).

- [ ] **Step 1: Concurrency-Regressions-Test schreiben (Red)**

  In `EasyVereinApiClientTests.cs`, direkt unterhalb des bestehenden `GetBooking_AfterListWithFilters_DoesNotLeakFiltersIntoUrl`-Tests, neuen Test einfügen:

  ```csharp
  [Fact]
  public async Task ListBookings_ConcurrentCallsWithDifferentFilters_DoNotLeakBetweenEachOther()
  {
      var emptyPage = JsonSerializer.Serialize(new
      {
          results = Array.Empty<object>(),
          next = (string?)null
      });

      // Zwei Clients mit jeweils einem eigenen capturing handler — wir wollen sicherstellen,
      // dass die Query-Builder-Instanzen nicht über Threads hinweg geteilt werden.
      var handlerA = new CapturingFakeHttpHandler(HttpStatusCode.OK, emptyPage);
      var handlerB = new CapturingFakeHttpHandler(HttpStatusCode.OK, emptyPage);
      var clientA = CreateClient(handlerA);
      var clientB = CreateClient(handlerB);

      var taskA = clientA.ListBookingsAsync(idIn: "111,222");
      var taskB = clientB.ListBookingsAsync(idIn: "999");
      await Task.WhenAll(taskA, taskB);

      Assert.NotNull(handlerA.LastRequestUri);
      Assert.NotNull(handlerB.LastRequestUri);
      Assert.Contains("id__in=111%2C222", handlerA.LastRequestUri!.Query);
      Assert.DoesNotContain("id__in=999", handlerA.LastRequestUri!.Query);
      Assert.Contains("id__in=999", handlerB.LastRequestUri!.Query);
      Assert.DoesNotContain("id__in=111", handlerB.LastRequestUri!.Query);
  }
  ```

- [ ] **Step 2: Test laufen lassen → muss FAIL liefern**

  ```
  dotnet test --nologo --verbosity minimal --filter "FullyQualifiedName~ListBookings_ConcurrentCallsWithDifferentFilters"
  ```
  Erwartet: Fehler, weil `ApiQueries.BookingQuery` (static singleton) zwischen den beiden Aufrufen wechseln kann und `IdIn` überschrieben wird. Welcher Assert genau fehlschlägt ist race-abhängig, aber es gibt einen Fehler.

  Falls der Test zufällig grün ist (sehr schnelles Sequenz-Race ohne Überlappung): Step erneut laufen lassen. Falls weiterhin grün, mehr Iterationen pro Task hinzufügen:
  ```csharp
  for (int i = 0; i < 50; i++) { /* Inhalt oben */ }
  ```

- [ ] **Step 3: `ListBookingsAsync` auf lokale Query-Instanz umstellen (Green)**

  In `EasyVereinApiClient.cs`, `ListBookingsAsync` ersetzen:

  Vorher (etwa Zeile 812–824):
  ```csharp
  public async Task<IReadOnlyList<Booking>> ListBookingsAsync(string? idIn = null, string? date = default, string? dateGt = default, string? dateLt = default, string? ordering = default, string[]? search = default,
      CancellationToken ct = default)
  {
      ApiQueries.BookingQuery.IdIn = idIn;
      ApiQueries.BookingQuery.Search = search;
      ApiQueries.BookingQuery.Date = date;
      ApiQueries.BookingQuery.DateGt = dateGt;
      ApiQueries.BookingQuery.DateLt = dateLt;
      ApiQueries.BookingQuery.Ordering = ordering;

      return await HandleListResponseWithPagination<Booking>(
          BuildListUrl("booking", ApiQueries.Booking), ct);
  }
  ```

  Nachher:
  ```csharp
  public async Task<IReadOnlyList<Booking>> ListBookingsAsync(string? idIn = null, string? date = default, string? dateGt = default, string? dateLt = default, string? ordering = default, string[]? search = default,
      CancellationToken ct = default)
  {
      var query = new BookingQuery
      {
          IdIn = idIn,
          Search = search,
          Date = date,
          DateGt = dateGt,
          DateLt = dateLt,
          Ordering = ordering
      };
      return await HandleListResponseWithPagination<Booking>(
          BuildListUrl("booking", query.ToString()), ct);
  }
  ```

- [ ] **Step 4: Tests laufen lassen → alle grün**

  ```
  dotnet test --nologo --verbosity minimal
  ```
  Erwartet: `erfolgreich: 174` (Baseline 173 + 1 neuer Concurrency-Test). Falls weiterhin Fehler im `GetBooking_AfterListWithFilters`-Test: prüfen ob `BookingQuery.FieldQuery` weiterhin verwendet wird.

- [ ] **Step 5: Commit**

  ```
  git add src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
  git commit -m "refactor(booking): ListBookingsAsync nutzt per-call BookingQuery-Instanz (US-0062)

  Eliminiert die letzte Stelle, an der ApiQueries.BookingQuery static-state
  mutiert wird. Concurrency-Regressions-Test ergaenzt.

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 2: Member-Endpoint (FieldQuery hochziehen, ListMembersAsync + GetMemberAsync refactorn)

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/MemberQuery.cs` (FieldQuery: private → internal)
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListMembersAsync`, `GetMemberAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs` (List→Get-Sequenz-Test)

Filter-Properties laut Client-Quelle: `Id`, `MembershipNumber`, `Search`.

- [ ] **Step 1: Regressions-Test schreiben (Red)**

  Im selben Test-File, in der `Members`-Sektion, neuen Test ergänzen:

  ```csharp
  [Fact]
  public async Task GetMember_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, emailOrUserName = "x@y.de" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListMembersAsync(id: 12345, membershipNumber: "M-42", search: new[] { "Mueller" });
      await client.GetMemberAsync(999);

      Assert.NotNull(handler.LastRequestUri);
      var path = handler.LastRequestUri!.AbsolutePath;
      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/member/999", path);
      Assert.DoesNotContain("id=", query);
      Assert.DoesNotContain("membershipNumber=", query);
      Assert.DoesNotContain("search=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test laufen lassen → muss FAIL liefern**

  ```
  dotnet test --nologo --verbosity minimal --filter "FullyQualifiedName~GetMember_AfterListWithFilters"
  ```
  Erwartet: Fehler in einem der `DoesNotContain`-Asserts.

- [ ] **Step 3: `MemberQuery.FieldQuery` von private auf internal hochziehen**

  In `MemberQuery.cs`, Zeile 28:
  ```csharp
  private const string FieldQuery =
  ```
  ändern zu:
  ```csharp
  /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
  internal const string FieldQuery =
  ```

- [ ] **Step 4: `ListMembersAsync` auf lokale Instanz umstellen**

  In `EasyVereinApiClient.cs`, ca. Zeile 887:

  Vorher:
  ```csharp
  public async Task<IReadOnlyList<Member>> ListMembersAsync(long? id = null, string? membershipNumber = null, string[]? search = null, CancellationToken ct = default)
  {
      ApiQueries.MemberQuery.Id = id;
      ApiQueries.MemberQuery.MembershipNumber = membershipNumber;
      ApiQueries.MemberQuery.Search = search;

      return await HandleListResponseWithPagination<Member>(
          BuildListUrl("member", ApiQueries.Member), ct);
  }
  ```

  Nachher:
  ```csharp
  public async Task<IReadOnlyList<Member>> ListMembersAsync(long? id = null, string? membershipNumber = null, string[]? search = null, CancellationToken ct = default)
  {
      var query = new MemberQuery
      {
          Id = id,
          MembershipNumber = membershipNumber,
          Search = search
      };
      return await HandleListResponseWithPagination<Member>(
          BuildListUrl("member", query.ToString()), ct);
  }
  ```

- [ ] **Step 5: `GetMemberAsync` auf `MemberQuery.FieldQuery` umstellen**

  In `EasyVereinApiClient.cs`, ca. Zeile 794:

  Vorher:
  ```csharp
  public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
  {
      var response = await SendWithErrorHandling(
          () => _httpClient.GetAsync(BuildGetUrl($"member/{id}", ApiQueries.Member), ct), ct);
      if (response.StatusCode == HttpStatusCode.NotFound)
          return null;
      return await HandleResponse<Member>(response, ct);
  }
  ```

  Nachher:
  ```csharp
  public async Task<Member?> GetMemberAsync(long id, CancellationToken ct = default)
  {
      var response = await SendWithErrorHandling(
          () => _httpClient.GetAsync(BuildGetUrl($"member/{id}", MemberQuery.FieldQuery), ct), ct);
      if (response.StatusCode == HttpStatusCode.NotFound)
          return null;
      return await HandleResponse<Member>(response, ct);
  }
  ```

- [ ] **Step 6: Tests laufen lassen → alle grün**

  ```
  dotnet test --nologo --verbosity minimal
  ```
  Erwartet: `erfolgreich: 175` (Baseline nach Task 1: 174 + 1).

- [ ] **Step 7: Commit**

  ```
  git add -- src/MCP.EasyVerein.Infrastructure/ApiClient/MemberQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
  git commit -m "refactor(member): per-call MemberQuery-Instanz, kein ApiQueries-State (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 3: ContactDetails-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ContactDetailsQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListContactDetailsAsync`, `GetContactDetailsAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Id`, `FirstName`, `FamilyName`, `Name`.

- [ ] **Step 1: Regressions-Test schreiben (Red)**

  ```csharp
  [Fact]
  public async Task GetContactDetails_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, firstName = "Anna" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListContactDetailsAsync(id: 12345, firstName: "Bob", familyName: "Smith", name: "BSmith");
      await client.GetContactDetailsAsync(999);

      Assert.NotNull(handler.LastRequestUri);
      var path = handler.LastRequestUri!.AbsolutePath;
      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/contact-details/999", path);
      Assert.DoesNotContain("firstName=", query);
      Assert.DoesNotContain("familyName=", query);
      Assert.DoesNotContain("name=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**
  ```
  dotnet test --nologo --verbosity minimal --filter "FullyQualifiedName~GetContactDetails_AfterListWithFilters"
  ```

- [ ] **Step 3: `ContactDetailsQuery.FieldQuery` → internal**

  In `ContactDetailsQuery.cs`, Zeile 33: `private const string FieldQuery =` → `internal const string FieldQuery =`.

- [ ] **Step 4: `ListContactDetailsAsync` refactorn**

  In `EasyVereinApiClient.cs`, ca. Zeile 868:

  Vorher:
  ```csharp
  public async Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
      string? familyName = null, string? name = null, CancellationToken ct = default)
  {
      ApiQueries.ContactDetailsQuery.Id = id;
      ApiQueries.ContactDetailsQuery.FirstName = firstName;
      ApiQueries.ContactDetailsQuery.FamilyName = familyName;
      ApiQueries.ContactDetailsQuery.Name = name;
      return await HandleListResponseWithPagination<ContactDetails>(
          BuildListUrl("contact-details", ApiQueries.ContactDetails), ct);
  }
  ```

  Nachher:
  ```csharp
  public async Task<IReadOnlyList<ContactDetails>> ListContactDetailsAsync(long? id = null, string? firstName = null,
      string? familyName = null, string? name = null, CancellationToken ct = default)
  {
      var query = new ContactDetailsQuery
      {
          Id = id,
          FirstName = firstName,
          FamilyName = familyName,
          Name = name
      };
      return await HandleListResponseWithPagination<ContactDetails>(
          BuildListUrl("contact-details", query.ToString()), ct);
  }
  ```

- [ ] **Step 5: `GetContactDetailsAsync` refactorn (ca. Zeile 704)**

  Aufruf `BuildGetUrl($"contact-details/{id}", ApiQueries.ContactDetails)` → `BuildGetUrl($"contact-details/{id}", ContactDetailsQuery.FieldQuery)`.

- [ ] **Step 6: Tests → grün, 176**
  ```
  dotnet test --nologo --verbosity minimal
  ```

- [ ] **Step 7: Commit**
  ```
  git add -- src/MCP.EasyVerein.Infrastructure/ApiClient/ContactDetailsQuery.cs src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs
  git commit -m "refactor(contact-details): per-call ContactDetailsQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 4: Event-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EventQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListEventsAsync`, `GetEventAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `StartGte`, `StartLte`, `EndGte`, `EndLte`, `Calendar`, `Canceled`, `IsPublic`, `IdIn`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetEvent_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListEventsAsync(
          name: "Sommerfest",
          startGte: "2026-01-01",
          startLte: "2026-12-31",
          endGte: "2026-01-01",
          endLte: "2026-12-31",
          calendar: "5",
          canceled: "false",
          isPublic: "true",
          idIn: "1,2",
          ordering: "name",
          search: new[] { "fest" });
      await client.GetEventAsync(999);

      Assert.NotNull(handler.LastRequestUri);
      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/event/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("start__gte=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.DoesNotContain("ordering=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL** (`--filter "FullyQualifiedName~GetEvent_AfterListWithFilters"`)

- [ ] **Step 3: `EventQuery.FieldQuery` → internal** (Zeile 44)

- [ ] **Step 4: `ListEventsAsync` refactorn (ca. Zeile 741)**

  Vorher:
  ```csharp
  ApiQueries.EventQuery.Name = name;
  ApiQueries.EventQuery.StartGte = startGte;
  // … usw …
  return await HandleListResponseWithPagination<Event>(BuildListUrl("event", ApiQueries.Event), ct);
  ```

  Nachher:
  ```csharp
  var query = new EventQuery
  {
      Name = name,
      StartGte = startGte,
      StartLte = startLte,
      EndGte = endGte,
      EndLte = endLte,
      Calendar = calendar,
      Canceled = canceled,
      IsPublic = isPublic,
      IdIn = idIn,
      Ordering = ordering,
      Search = search
  };
  return await HandleListResponseWithPagination<Event>(BuildListUrl("event", query.ToString()), ct);
  ```

- [ ] **Step 5: `GetEventAsync` refactorn (ca. Zeile 719)**

  `ApiQueries.Event` → `EventQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 177**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(event): per-call EventQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 5: Calendar-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/CalendarQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListCalendarsAsync`, `GetCalendarAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `Color`, `Short`, `NameNot`, `ColorNot`, `ShortNot`, `IdIn`, `AllowedGroups`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetCalendar_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "Cal" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListCalendarsAsync(
          name: "Vereinskalender",
          color: "#f00",
          short_: "VK",
          idIn: "1,2",
          ordering: "name",
          search: new[] { "kal" });
      await client.GetCalendarAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/calendar/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("color=", query);
      Assert.DoesNotContain("short=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL** (`--filter "FullyQualifiedName~GetCalendar_AfterListWithFilters"`)

- [ ] **Step 3: `CalendarQuery.FieldQuery` → internal** (Zeile 41)

- [ ] **Step 4: `ListCalendarsAsync` refactorn (ca. Zeile 839)**

  Lokale Instanz aufbauen analog zu Task 4. Alle 10 Filter-Properties übertragen.

- [ ] **Step 5: `GetCalendarAsync` refactorn (ca. Zeile 690)**

  `ApiQueries.Calendar` → `CalendarQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 178**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(calendar): per-call CalendarQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 6: Announcement-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/AnnouncementQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListAnnouncementsAsync`, `GetAnnouncementAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetAnnouncement_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, title = "X" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListAnnouncementsAsync(ordering: "title", search: new[] { "wartung" });
      await client.GetAnnouncementAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/announcement/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("ordering=", query);
      Assert.DoesNotContain("search=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `AnnouncementQuery.FieldQuery` → internal** (Zeile 17)

- [ ] **Step 4: `ListAnnouncementsAsync` refactorn (ca. Zeile 103)**

  ```csharp
  var query = new AnnouncementQuery
  {
      Ordering = ordering,
      Search = search
  };
  return await HandleListResponseWithPagination<Announcement>(
      BuildListUrl("announcement", query.ToString()), ct);
  ```

- [ ] **Step 5: `GetAnnouncementAsync` refactorn (ca. Zeile 90)**

  `ApiQueries.Announcement` → `AnnouncementQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 179**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(announcement): per-call AnnouncementQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 7: BankAccount-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/BankAccountQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListBankAccountsAsync`, `GetBankAccountAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `Iban`, `Bic`, `AccountHolder`, `BankName`, `IdIn`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetBankAccount_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListBankAccountsAsync(
          name: "Sparkasse",
          iban: "DE00",
          bic: "BIC",
          accountHolder: "Verein",
          bankName: "Sparkasse",
          idIn: "1,2",
          ordering: "name",
          search: new[] { "spk" });
      await client.GetBankAccountAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/bank-account/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("iban=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `BankAccountQuery.FieldQuery` → internal** (Zeile 35)

- [ ] **Step 4: `ListBankAccountsAsync` refactorn (ca. Zeile 176)**

  Alle 8 Filter-Properties in `new BankAccountQuery { ... }`.

- [ ] **Step 5: `GetBankAccountAsync` refactorn (ca. Zeile 157)**

  `ApiQueries.BankAccount` → `BankAccountQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 180**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(bank-account): per-call BankAccountQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 8: BillingAccount-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/BillingAccountQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListBillingAccountsAsync`, `GetBillingAccountAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `IdIn`, `Skr`, `SkrIn`, `NumberGte`, `NumberLte`, `Deleted`, `AccountingPlanIsNull`, `ShowOwnBillingAccounts`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetBillingAccount_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListBillingAccountsAsync(
          name: "Spendenkonto",
          idIn: "1,2",
          skr: "42",
          deleted: "false",
          ordering: "number");
      await client.GetBillingAccountAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/billing-account/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.DoesNotContain("skr=", query);
      Assert.DoesNotContain("deleted=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `BillingAccountQuery.FieldQuery` → internal** (Zeile 49)

- [ ] **Step 4: `ListBillingAccountsAsync` refactorn (ca. Zeile 260)**

  Alle 11 Filter-Properties in `new BillingAccountQuery { ... }`.

- [ ] **Step 5: `GetBillingAccountAsync` refactorn (ca. Zeile 238)**

  `ApiQueries.BillingAccount` → `BillingAccountQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 181**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(billing-account): per-call BillingAccountQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 9: BookingProject-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/BookingProjectQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListBookingProjectsAsync`, `GetBookingProjectAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `Short`, `Completed`, `IdIn`, `BudgetGt`, `BudgetLt`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetBookingProject_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "X" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListBookingProjectsAsync(
          name: "Dorffest",
          @short: "DF",
          completed: "false",
          idIn: "1,2",
          ordering: "name");
      await client.GetBookingProjectAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/booking-project/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("short=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `BookingProjectQuery.FieldQuery` → internal** (Zeile 35)

- [ ] **Step 4: `ListBookingProjectsAsync` refactorn (ca. Zeile 347)**

  Alle 8 Filter-Properties in `new BookingProjectQuery { ... }`.

- [ ] **Step 5: `GetBookingProjectAsync` refactorn (ca. Zeile 328)**

  `ApiQueries.BookingProject` → `BookingProjectQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 182**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(booking-project): per-call BookingProjectQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 10: ChairmanLevel-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/ChairmanLevelQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListChairmanLevelsAsync`, `GetChairmanLevelAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `Name`, `Short`, `IdIn`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetChairmanLevel_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, name = "Vorstand" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListChairmanLevelsAsync(
          name: "Vorstand",
          @short: "VS",
          idIn: "1,2",
          ordering: "name");
      await client.GetChairmanLevelAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/chairman-level/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("name=", query);
      Assert.DoesNotContain("short=", query);
      Assert.DoesNotContain("id__in=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `ChairmanLevelQuery.FieldQuery` → internal** (Zeile 26)

- [ ] **Step 4: `ListChairmanLevelsAsync` refactorn (ca. Zeile 498)**

  Alle 5 Filter-Properties in `new ChairmanLevelQuery { ... }`.

- [ ] **Step 5: `GetChairmanLevelAsync` refactorn (ca. Zeile 482)**

  `ApiQueries.ChairmanLevel` → `ChairmanLevelQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 183**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(chairman-level): per-call ChairmanLevelQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 11: InvoiceItem-Endpoint

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceItemQuery.cs`
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`ListInvoiceItemsAsync`, `GetInvoiceItemAsync`)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Filter-Properties: `IdIn`, `RelatedInvoice`, `Ordering`, `Search`.

- [ ] **Step 1: Regressions-Test (Red)**

  ```csharp
  [Fact]
  public async Task GetInvoiceItem_AfterListWithFilters_DoesNotLeakFiltersIntoUrl()
  {
      var listJson = JsonSerializer.Serialize(new { results = Array.Empty<object>(), next = (string?)null });
      var getJson = JsonSerializer.Serialize(new { id = 999, title = "Pos" });
      var handler = new MultiPageFakeHttpHandler(new[]
      {
          (HttpStatusCode.OK, listJson),
          (HttpStatusCode.OK, getJson)
      });
      var client = CreateClient(handler);

      await client.ListInvoiceItemsAsync(
          idIn: "1,2",
          relatedInvoice: "42",
          ordering: "id",
          search: new[] { "Pos" });
      await client.GetInvoiceItemAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/invoice-item/999", handler.LastRequestUri!.AbsolutePath);
      Assert.DoesNotContain("id__in=", query);
      Assert.DoesNotContain("relatedInvoice=", query);
      Assert.DoesNotContain("ordering=", query);
      Assert.DoesNotContain("search=", query);
      Assert.Contains("query=", query);
  }
  ```

- [ ] **Step 2: Test → FAIL**

- [ ] **Step 3: `InvoiceItemQuery.FieldQuery` → internal** (Zeile 23)

- [ ] **Step 4: `ListInvoiceItemsAsync` refactorn (ca. Zeile 425)**

  Alle 4 Filter-Properties in `new InvoiceItemQuery { ... }`.

- [ ] **Step 5: `GetInvoiceItemAsync` refactorn (ca. Zeile 410)**

  `ApiQueries.InvoiceItem` → `InvoiceItemQuery.FieldQuery`.

- [ ] **Step 6: Tests → grün, 184**

- [ ] **Step 7: Commit**

  ```
  git commit -m "refactor(invoice-item): per-call InvoiceItemQuery-Instanz (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 12: Invoice-Endpoint konsolidieren

**Files:**
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs` (FieldQuery → internal; ToString-Filter sind unbenutzt; Filter-Properties bleiben für künftige Erweiterungen)
- Modify: `src/MCP.EasyVerein.Infrastructure/ApiClient/EasyVereinApiClient.cs` (`GetInvoicesAsync`, `GetInvoiceAsync` — beide nutzen aktuell `ApiQueries.Invoice` const string)
- Modify: `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`

Sonderfall: Es gibt zwei verschiedene FieldQuery-Definitionen — `ApiQueries.Invoice` (hardcoded const string mit ~30 Feldern) und `InvoiceQuery.FieldQuery` (private const string mit ~50 Feldern). Die Diskrepanz ist eigene Schuld; in dieser Task wird `InvoiceQuery.FieldQuery` als Single Source of Truth etabliert und `ApiQueries.Invoice` entfällt mit Task 13.

Vor der Code-Änderung: prüfe, ob die beiden Field-Sets identisch sind. Falls nicht, behalte die längere Variante (`InvoiceQuery.FieldQuery`) und passe nur die Felder an, die in `ApiQueries.Invoice` zusätzlich auftauchen.

- [ ] **Step 1: Feld-Sets vergleichen**

  ```
  grep -E '^[[:space:]]+"' src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs | head -60
  grep -E 'InvoiceFields' src/MCP.EasyVerein.Infrastructure/ApiClient/InvoiceQuery.cs | wc -l
  ```
  Notiere die Liste. Vergleiche mit dem const string `ApiQueries.Invoice` (ca. Zeile 111–122 in `ApiQueries.cs`). Felder, die in `ApiQueries.Invoice` enthalten sind aber in `InvoiceQuery.FieldQuery` fehlen, müssen ergänzt werden.

- [ ] **Step 2: `InvoiceQuery.FieldQuery` → internal (Zeile 28)**

  ```csharp
  internal const string FieldQuery =
  ```

- [ ] **Step 3: Falls Felder fehlen — `InvoiceQuery.FieldQuery` erweitern**

  Felder ergänzen in der `FieldQuery`-const, jedes mit Trennkomma. Beispiel falls `useAddressBalance` fehlt:
  ```csharp
  InvoiceFields.UseAddressBalance + "," +
  ```
  (Stelle in `InvoiceFields.cs` sicher, dass die Konstante existiert; falls nicht, anlegen mit dem API-Feldnamen.)

- [ ] **Step 4: Regressions-Test schreiben (Red)**

  ```csharp
  [Fact]
  public async Task GetInvoice_UsesInvoiceQueryFieldQuery_NotApiQueriesConst()
  {
      var json = JsonSerializer.Serialize(new { id = 999, invNumber = "INV-001" });
      var handler = new CapturingFakeHttpHandler(HttpStatusCode.OK, json);
      var client = CreateClient(handler);

      await client.GetInvoiceAsync(999);

      var query = handler.LastRequestUri!.Query;
      Assert.EndsWith("/invoice/999", handler.LastRequestUri!.AbsolutePath);
      Assert.Contains("query=", query);
      Assert.DoesNotContain("id=", query);
  }
  ```

- [ ] **Step 5: Test → FAIL falls aktuell `ApiQueries.Invoice` ein anderer String wäre, sonst skip Refactor; Step 6 jedenfalls**

- [ ] **Step 6: `GetInvoiceAsync` und `GetInvoicesAsync` refactorn**

  In `EasyVereinApiClient.cs`, ca. Zeile 769:

  ```csharp
  public async Task<Invoice?> GetInvoiceAsync(long id, CancellationToken ct = default)
  {
      var response = await SendWithErrorHandling(
          () => _httpClient.GetAsync(BuildGetUrl($"invoice/{id}", InvoiceQuery.FieldQuery), ct), ct);
      if (response.StatusCode == HttpStatusCode.NotFound) return null;
      return await HandleResponse<Invoice>(response, ct);
  }

  public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken ct = default)
  {
      return await HandleListResponseWithPagination<Invoice>(
          BuildListUrl("invoice", InvoiceQuery.FieldQuery), ct);
  }
  ```

- [ ] **Step 7: Tests → grün, 185**

- [ ] **Step 8: Commit**

  ```
  git commit -m "refactor(invoice): InvoiceQuery.FieldQuery als Single Source of Truth (US-0062)

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 13: `ApiQueries.cs` entfernen + Compile-Verifikation

**Files:**
- Delete: `src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs`

- [ ] **Step 1: Datei löschen**

  ```
  git rm src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs
  ```

- [ ] **Step 2: Build + Tests laufen lassen**

  ```
  dotnet build --nologo --verbosity minimal
  dotnet test --nologo --verbosity minimal
  ```
  Erwartet: Build erfolgreich (alle `ApiQueries.X`-References sind in Tasks 1–12 entfernt worden). Tests grün, mindestens `erfolgreich: 185`.

  Falls Build fehlschlägt: Compile-Fehler zeigen genau, welche `ApiQueries.X`-Reference übersehen wurde. In Task 1–12 das jeweilige Get/List nachziehen, dann Step 2 wiederholen.

- [ ] **Step 3: Commit**

  ```
  git add -A
  git commit -m "refactor: ApiQueries-Static-Klasse entfernt (US-0062)

  Letzte Stufe der Migration auf per-call Query-Instanzen. Die Klasse hat
  fuer jede Entitaet einen shared mutable Singleton gehalten und war
  Ursache fuer state-leaks (Bug #2 / PR #101) sowie potentielle
  Race-Conditions bei parallelen MCP-Tool-Calls.

  Verlinkt mit GitHub Issue #102"
  ```

---

## Task 14: Akzeptanzkriterien-Check + Tests gesamt

- [ ] **Step 1: Test-Suite vollständig laufen lassen mit Coverage-Report**

  ```
  dotnet test --nologo --verbosity minimal --collect:"XPlat Code Coverage"
  ```
  Erwartet: alle Tests grün; Coverage in `tests/MCP.EasyVerein.Infrastructure.Tests/TestResults/<guid>/coverage.cobertura.xml`. Coverage-Schwelle gemäß CLAUDE.md: ≥ 70 %.

- [ ] **Step 2: Manueller Akzeptanzkriterien-Walkthrough**

  Issue #102 öffnen und jedes Kriterium gegen die Codebase verifizieren:
  - `git ls-files src/MCP.EasyVerein.Infrastructure/ApiClient/ApiQueries.cs` → leer (Datei weg)
  - Per Query-Klasse `grep -nE "new \w+Query \{|internal const string FieldQuery"` durchlaufen lassen
  - `IEasyVereinApiClient`-Signaturen unverändert: `git diff main -- src/MCP.EasyVerein.Domain/Interfaces/IEasyVereinApiClient.cs` → leer

- [ ] **Step 3: Push**

  ```
  git push -u origin feature/US-0062-per-call-query-instanzen
  ```

- [ ] **Step 4: Pull Request öffnen**

  ```
  gh pr create --title "feat(US-0062): ApiQueries Static-Singleton durch per-call Query-Instanzen ersetzen" --body "$(cat <<'EOF'
  ## Summary

  Eliminiert die `Infrastructure/ApiClient/ApiQueries`-Klasse vollständig. Jeder `ListXxxAsync` instanziiert seine Query-Klasse lokal, jeder `GetXxxAsync` nutzt direkt `XxxQuery.FieldQuery`. Filter aus früheren Aufrufen können nicht mehr in nachfolgende Requests lecken; parallele MCP-Tool-Calls überschreiben sich nicht mehr gegenseitig.

  Vorbild war PR #101 (`BookingQuery.FieldQuery` + `GetBookingAsync`), das die Methode für nur einen einzigen Endpoint umgestellt hat.

  ## Test plan

  - [x] Regressions-Test pro Endpoint (List→Get-URL ohne Filter-Leak)
  - [x] Concurrency-Smoke-Test für Booking-Listing (zwei parallele Aufrufe mit unterschiedlichen Filtern)
  - [x] Bestehende 173 Tests bleiben grün
  - [x] Coverage ≥ 70 %
  - [ ] Live-Smoke gegen echte easyVerein-API: list_xxx mit Filtern, dann get_xxx(andere-id) → muss korrekte Resource liefern, nicht 404

  Closes #102.
  EOF
  )"
  ```

---

## Self-Review-Checklist (für den ausführenden Engineer)

Nach Abschluss aller Tasks, vor PR-Merge:

1. **Spec coverage:** Jedes Issue-Akzeptanzkriterium hat einen entsprechenden Test in `EasyVereinApiClientTests.cs`. Lücken? → Test ergänzen.
2. **Type consistency:** Filter-Property-Namen in den `new XxxQuery { ... }`-Blöcken stimmen exakt mit den Property-Definitionen der jeweiligen Query-Klasse überein. Falls die Tests builden, ist das automatisch garantiert.
3. **No leftovers:** `grep -rn "ApiQueries" src/` → leer. `grep -rn "ApiQueries" tests/` → leer (außer evtl. historische Kommentare).
4. **Konvention CLAUDE.md:** Alle Members (auch private) haben englische XML-Doc-Kommentare. Neue Tests sind im bestehenden flachen Folder (kein neuer Subfolder).
5. **Branch-Discipline (Memory):** PR enthält ausschließlich US-0062-Refactor. Keine orthogonalen CI/Docs/Skill-Files dabei. Vor jedem `git add`/`git commit` prüfen mit `git status --short`.
6. **Daily Note 2026-05-18:** PR-Eintrag in `docs/006 Daily Notes/2026-05-18.md` nach Merge ergänzen.
