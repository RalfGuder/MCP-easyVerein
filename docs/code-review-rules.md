# Code-Review-Regeln – MCP-easyVerein

> **Quelle:** `main` @ `c1ca5a0`
> **Grundlage:** [code-review-observations.md](code-review-observations.md) (B-01 bis B-47), am 2026-09-29 erneut gegen den Code geprüft
> **Aufnahmekriterium:** Es werden nur Beobachtungen mit Konfidenz **hoch** oder **mittel** übernommen.

## Vorbemerkungen

**Maßstab.** Viele Gegenbeispiele stammen aus den sechs ältesten Endpoints (Member, ContactDetails, Invoice, Event, Booking, Calendar). Die Regeln orientieren sich an den neueren Endpoints, z. B. Lending, InventoryObject, InventoryObjectGroup und Forum. Ein Gegenbeispiel aus dem Legacy-Bestand widerlegt eine Regel nicht, es ist ein Kandidat für späteres Aufräumen.

**Schweregrade:**

| Stufe | Einsatz |
|---|---|
| **MUST** | Ein Verstoß bricht das MCP-Protokoll, riskiert Datenverlust oder Geheimnislecks, verletzt eine Schichtgrenze, oder `CLAUDE.md` formuliert die Regel ausdrücklich als Pflicht |
| **SHOULD** | Durchgängig gelebte Konvention; eine Abweichung braucht eine Begründung im PR |
| **CONSIDER** | Konfidenz mittel oder uneinheitlicher Bestand; Diskussionspunkt statt Befund |

**Pfadkürzel** wie im Beobachtungsdokument:
- `Dom/` = `src/MCP.EasyVerein.Domain/`
- `App/` = `src/MCP.EasyVerein.Application/`
- `Infra/` = `src/MCP.EasyVerein.Infrastructure/ApiClient/`
- `Client` = `Infra/EasyVereinApiClient.cs`
- `Tools/` = `src/MCP.EasyVerein.Server/Tools/`
- `T.Infra` = `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`
- `T.Server/` = `tests/MCP.EasyVerein.Server.Tests/`

**Werkzeughinweise.** Die unter „Automatisierbar" genannten Analyzer und Regel-IDs stammen **nicht** aus dem Repository, denn es gibt weder eine `.editorconfig` noch Analyzer (B-46). Sie sind jeweils gekennzeichnet mit *Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet*.

### Ergebnis der erneuten Prüfung

| Punkt | Ergebnis |
|---|---|
| Tools mit `CancellationToken ct` | 118/118 bestätigt |
| `catch`-Template in Tools | 113 `catch (Exception …)`, davon **109** wörtlich identisch. Im Beobachtungsdokument präzisiert. |
| Null-forgiving `!` in Infrastructure | 0 bestätigt |
| `.Result`/`.Wait()`/`GetAwaiter().GetResult()`/`async void` in `src` + `tests` | 0 bestätigt |
| `PutAsync`, `new HttpClient(` | 0 / 0 bestätigt |
| Aufrufe über `SendWithErrorHandling` | 95 bestätigt |
| `Times.Never` / `MockBehavior.Strict` in Tests | 26 bestätigt |
| `Console.Write*` in `src` | 1 Treffer: `Program.cs:91` (`--help`-Ausgabe **vor** Host-Start, unkritisch) |
| Zeilennummern der Client-Helfer | **korrigiert:** `HandleListResponseWithPagination` `Client:2099`, `HandleResponse` `:2130`, `SendWithErrorHandling` `:2144`. Im Beobachtungsdokument berichtigt. |

**Nicht übernommen**, weil die Konfidenz niedrig ist oder keine Regel ableitbar war: B-15 (monolithisches Interface), B-17 (keine Guards), B-21 (Converter-Fehlerverhalten), B-26 (kein Dispose), B-29 (Serializer-Optionen), B-35 (`DateTime`/`DateTimeOffset`), B-40 (Duplikation), B-46 (fehlende Durchsetzung), der Escaping-Teil von B-33, der Converter-Auswahl-Teil von B-28 und der Teil von B-22 zur Sprache von Kommentaren und Meldungen. Die ersten fünf sowie Escaping, fehlende Durchsetzung und Logging-Ausgabe stehen im [Anhang](#anhang-allgemeine-c-empfehlungen) als allgemeine Empfehlungen, die übrigen werden nicht weiter verfolgt.

---

# Architektur und Struktur

## CR-001: Projektreferenzen zeigen nur nach innen; Domain bleibt abhängigkeitsfrei

### Regel
- `MCP.EasyVerein.Domain` hat keine Projekt- und keine Paketreferenz.
- Application darf nur Domain referenzieren, Infrastructure nur Application und Domain, Server alle drei.
- Eine Referenz nach außen (z. B. Domain → Infrastructure) oder ein neues Paket in Domain ist ein Befund.

### Begründung
Alle 8 `.csproj` halten diese Richtung ein. `CLAUDE.md` beschreibt die 4 Schichten ausdrücklich („Domain … keine externen Abhängigkeiten"). Beobachtung B-01, Konfidenz hoch.

### Positives Beispiel
- `src/MCP.EasyVerein.Domain/MCP.EasyVerein.Domain.csproj`: keine `ProjectReference`/`PackageReference`
- `src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj`: nur Domain und `Microsoft.Extensions.*.Abstractions`

### Negatives Beispiel
- Grenzfall: `InternalsVisibleTo` für Infrastructure und Server in `MCP.EasyVerein.Domain.csproj`. Die Richtung bleibt korrekt, aber interne Domain-Typen werden nach außen geöffnet.

### Review-Frage
Ändert der PR eine `.csproj`? Zeigt jede neue Referenz nach innen, und bleibt Domain paketfrei?

### Schweregrad
MUST

### Automatisierbar
Teilweise. Der Diff von `.csproj`-Dateien ist leicht zu prüfen. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Architekturtests, z. B. NetArchTest.

---

## CR-002: Neue Endpoints vollständig nach dem Fünf-Schritte-Muster

### Regel
Ein neuer API-Endpoint umfasst **alle** folgenden Teile:
1. Entity + `*Fields`-Klasse (Domain)
2. Methoden in `IEasyVereinApiClient`
3. Implementierung in `EasyVereinApiClient` + `*Query`-Klasse
4. `*Tools`-Klasse
5. Registrierung per `.WithTools<T>()` in `Program.cs`

Eine `[McpServerToolType]`-Klasse ohne Registrierung ist ein Fehler.

### Begründung
Der Ablauf ist in `CLAUDE.md` („Muster für neue Endpoints") dokumentiert. Die 25 `.WithTools<>`-Aufrufe entsprechen exakt den 25 Tool-Klassen. Beobachtung B-03, Konfidenz hoch.

### Positives Beispiel
- Lending: `Dom/Entities/Lending.cs`, `Dom/ValueObjects/LendingFields.cs`, `Infra/LendingQuery.cs`, `Client:1443 ff.`, `Tools/LendingTools.cs`, `.WithTools<LendingTools>()` in `src/MCP.EasyVerein.Server/Program.cs:56-80`

### Negatives Beispiel
- `MemberGroup` ohne `*Fields`-Datei (`Dom/Entities/MemberGroup.cs:15`)
- Invoice ohne `ListInvoicesAsync` bzw. ohne genutzte `InvoiceQuery` (`Client:1814`, `Infra/InvoiceQuery.cs`)

### Review-Frage
Sind für den neuen Endpoint alle fünf Teile im PR enthalten, und ist die Tool-Klasse registriert?

### Schweregrad
MUST für die Registrierung. SHOULD für die Vollständigkeit der übrigen Teile.

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Ein Reflection-Test kann alle `[McpServerToolType]`-Typen mit den registrierten Tools vergleichen.

---

## CR-003: File-scoped Namespace passend zum Ordner, ein Typ pro Datei

### Regel
- Jede neue `.cs`-Datei hat einen file-scoped Namespace (`namespace X;`), der dem Ordnerpfad entspricht.
- Sie enthält genau einen Top-Level-Typ, und der Dateiname entspricht dem Typnamen.

### Begründung
59/69 Domain-Dateien, 21/24 Query-Klassen und 25/25 Tool-Klassen folgen dem. Die Abweichungen sind durchweg Legacy- oder Vorlagendateien. Beobachtung B-02, Konfidenz hoch.

### Positives Beispiel
- `Dom/Entities/Member.cs:7`, `Dom/Converters/FlexibleIdConverter.cs:5`, `Tools/LendingTools.cs`

### Negatives Beispiel
- Block-scoped: `Dom/Entities/AccountingPlan.cs`, `Location.cs`, `Organization.cs`, `Infra/InvoiceQuery.cs:3`, `MemberQuery.cs:3`, `ContactDetailsQuery.cs:3`
- Zwei Typen in einer Datei: `Dom/ValueObjects/FeatureRequestAuthor.cs:9,27`

### Review-Frage
Hat jede neue Datei einen file-scoped Namespace gemäß Ordner und genau einen Typ?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE0161 (`csharp_style_namespace_declarations = file_scoped`), IDE0130 (Namespace ↔ Ordner), SA1402/SA1649.

---

# Benennung

## CR-004: `Async`-Suffix und einheitliche CRUD-Verben im Client

### Regel
- Asynchrone Methoden in Domain und Infrastructure enden auf `Async`.
- Client-Methoden heißen `List{Ressourcen}Async` (Plural), `Get{Ressource}Async`, `Create{Ressource}Async`, `Update{Ressource}Async` bzw. `Delete{Ressource}Async`.

MCP-Tool-Methoden sind ausgenommen; für sie gilt CR-005.

### Begründung
117/117 Interface- und Client-Methoden tragen den Suffix. Die Verben sind durchgängig (List 23, Get 26, Create 23, Update 22, Delete 22). Beobachtung B-04, Konfidenz hoch.

### Positives Beispiel
- `Dom/Interfaces/IEasyVereinApiClient.cs:9` `ListAnnouncementsAsync`, `:14` `GetAnnouncementAsync`, `:58` `UpdateBankAccountAsync`

### Negatives Beispiel
- `IEasyVereinApiClient.cs:852`: `GetInvoicesAsync` statt `ListInvoicesAsync`
- Private Helfer ohne Suffix: `SendWithErrorHandling` (`Client:2144`), `HandleResponse` (`:2130`), `HandleListResponseWithPagination` (`:2099`)

### Review-Frage
Folgt jede neue asynchrone Client-Methode dem Schema `{Verb}{Ressource}Async`?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Der Suffix lässt sich prüfen, *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* VSTHRD200. Das Verbschema nicht.

---

## CR-005: MCP-Toolnamen explizit als `verb_ressource` in snake_case

### Regel
- Jede Tool-Methode setzt ihren Namen explizit: `[McpServerTool(Name = "verb_ressource")]`, in snake_case, Listen im Plural (`list_lendings`), sonst Singular.
- `[McpServerTool]` ohne `Name` ist ein Befund.
- Ein Toolname gehört zur öffentlichen Schnittstelle und wird nicht ohne Not umbenannt.

### Begründung
113 explizite `Name =`-Attribute, alle in snake_case. Nur eine Legacy-Klasse verlässt sich auf die SDK-Ableitung. Beobachtung B-05, Konfidenz hoch.

### Positives Beispiel
- `Tools/LendingTools.cs`: `list_lendings`, `get_lending`, `create_lending`, `update_lending`, `delete_lending`
- `Tools/GetTokenTools.cs:24`: `get_token`

### Negatives Beispiel
- `Tools/ContactDetailsTools.cs:24,44,61,81,95`: `[McpServerTool, Description(…)]` ohne `Name`

### Review-Frage
Hat jede neue oder geänderte Tool-Methode einen expliziten snake_case-Namen, und ändert sich der Name bestehender Tools?

### Schweregrad
MUST

### Automatisierbar
Ja. Ein Reflection-Test über alle `McpServerToolAttribute` (Name gesetzt, Regex `^[a-z]+(_[a-z]+)+$`) ist möglich. `T.Server/DosbSportToolsTests.cs:68-78` wertet das Attribut bereits per Reflection aus.

---

## CR-006: Private Felder `_camelCase`; bei Primärkonstruktor kein zusätzliches Feld

### Regel
- Private Instanzfelder heißen `_camelCase`.
- In Klassen mit Primärkonstruktor wird der Parameter direkt verwendet, nicht zusätzlich in ein Feld kopiert.

### Begründung
Alle privaten Felder folgen dem (`Client:18,21,24`, `Dom/ValueObjects/ApiVersion.cs:9`), und alle 25 Tool-Klassen nutzen Primärkonstruktor-Parameter direkt. Es gibt aber nur wenige Felder. Beobachtung B-06, Konfidenz mittel.

### Positives Beispiel
- `Client:18,21,24`: `_config`, `_httpClient`, `_jsonOptions`
- `Tools/LendingTools.cs:15`: `LendingTools(IEasyVereinApiClient client)` mit direkter Verwendung von `client`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Heißen neue private Felder `_camelCase`, und wird ein Primärkonstruktor-Parameter unnötig in ein Feld dupliziert?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `.editorconfig`-Namensregel + IDE1006.

---

## CR-007: Entity-Properties spiegeln den API-Feldnamen

### Regel
- Eine Entity-Property heißt wie das API-Feld in PascalCase.
- Abweichungen sind nur zulässig, um Tippfehler der API zu korrigieren oder Namenskonflikte aufzulösen. Sie werden über `[JsonPropertyName]` gemappt und im XML-Kommentar genannt.
- Boolesche Properties bekommen **keinen** zusätzlichen `Is`/`Has`-Präfix, wenn das API-Feld keinen hat.

### Begründung
Durchgängiges Muster. Die wenigen Umbenennungen sind jeweils begründet (`PaymentIntervalMonths` ↔ `paymentIntervallMonths`, `CustomTaxRateValue`). Beobachtung B-07, Konfidenz mittel.

### Positives Beispiel
- `Dom/Entities/Member.cs` + `Dom/ValueObjects/MemberFields.cs:66`: `PaymentIntervalMonths` → `"paymentIntervallMonths"`
- `Dom/Entities/Lending.cs`, `InventoryObject.cs`: 1:1-Namen

### Negatives Beispiel
- Uneinheitlich korrigiert: `BankAccount.Startsaldo` (`Dom/Entities/BankAccount.cs:50`) und `Invoice.PayedFromUser` (`Invoice.cs:63`) übernehmen API-Namen unverändert.

### Review-Frage
Weicht ein Property-Name vom API-Feld ab, und ist die Abweichung begründet und dokumentiert?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

## CR-008: Testmethoden `Methode_Szenario_Erwartung`

### Regel
- Testmethoden werden in PascalCase mit Unterstrichen als Trenner benannt.
- Standard ist `Methode_Szenario_Erwartung`. Das Szenario darf nur entfallen, wenn es trivial ist (`JsonPropertyNames_AreCorrect`).
- Der Methodenteil entspricht der getesteten Methode.

### Begründung
425/425 Testmethoden nutzen Unterstriche, 277 davon dreiteilig. Beobachtung B-08, Konfidenz hoch für die Trennung, mittel für die Dreiteilung.

### Positives Beispiel
- `T.Server/LendingToolsTests.cs:90`: `CreateLending_WithoutRequiredField_ReturnsError_WithoutCallingApi`
- `T.Server/GetTokenToolsTests.cs:89`: `GetToken_OnException_RedactsPassword`

### Negatives Beispiel
- `T.Infra:48`: `GetMembers_*` testet `ListMembersAsync`, der Methodenteil passt also nicht.

### Review-Frage
Erkennt man aus dem Testnamen, welche Methode in welchem Szenario was liefern soll?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Das Unterstrich-Muster ist per Regex prüfbar, die inhaltliche Passung nicht.

---

## CR-009: API-Feld- und Filternamen nur als `*Fields`-Konstante

### Regel
- API-Feldnamen und Filterschlüssel (`id__in`, `…__gte`, `ordering`, `search`) werden nie als String-Literal geschrieben, sondern als `const` in der `internal static class {Ressource}Fields`.
- Das gilt für `[JsonPropertyName]`, Query-Klassen und PATCH-Dictionaries.
- Neue Fields-Klassen sind `internal`.

### Begründung
28 Fields-Klassen werden in allen drei Verwendungsarten genutzt. 24/28 sind `internal`, die 4 `public` sind Legacy. Beobachtung B-09, Konfidenz hoch.

### Positives Beispiel
- `Dom/ValueObjects/BankAccountFields.cs:49` (`IdIn`), `Dom/ValueObjects/CalendarFields.cs:30` (`NameNot`)
- Verwendung: `Tools/InventoryObjectTools.cs:141-150` (PATCH-Schlüssel), `Infra/LendingQuery.cs:77` (`FieldQuery`)

### Negatives Beispiel
- Literale: `Infra/MemberQuery.cs:81,84,87`, `Infra/ContactDetailsQuery.cs:92-101`, `Dom/Entities/MemberGroup.cs:15`, `Dom/ValueObjects/InvoiceCharges.cs:9,12,15`
- `public`: `ContactDetailsFields`, `EventFields`, `InvoiceFields`, `MemberFields`

### Review-Frage
Taucht im Diff ein API-Feld- oder Filtername als String-Literal auf?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `JsonPropertyName("` mit Literal und `"…=` in Query-Klassen lassen sich per Textsuche finden.

---

# Klassen- und Typdesign

## CR-010: Entities sind nullable POCOs mit `IHasId`

### Regel
- Entities sind `public class X : IHasId` mit `{ get; set; }`-Properties ohne Geschäftslogik.
- Jede Property, die bei eingeschränkter Feldauswahl in der API-Antwort fehlen kann, ist nullable (`string?`, `int?`, `decimal?`, `DateTime?`/`DateTimeOffset?`).
- Erlaubt sind abgeleitete `[JsonIgnore]`-Properties (z. B. ID aus URL).

### Begründung
28/29 Entities implementieren `IHasId`. `string?` steht 175× gegen `string` 7×, alle `int`/`decimal` sind nullable. `IHasId` ist Voraussetzung für die generischen Referenz-Converter. Beobachtung B-10, Konfidenz hoch.

### Positives Beispiel
- `Dom/Entities/Lending.cs`, `Dom/Entities/InventoryObject.cs`
- `Dom/Entities/Invoice.cs:57,66,72`: `[JsonIgnore] long? XId => UrlReference.ExtractId(…)`

### Negatives Beispiel
- `Dom/Entities/ContactDetails.cs:13-213`: `{ get; init; }` (Legacy-Abweichung)
- `Dom/Entities/Member.cs:12`, `ContactDetails.cs:8`: wirkungsloses `[JsonSourceGenerationOptions]` an POCOs

### Review-Frage
Ist jede Property der neuen Entity nullable, sofern die API sie nicht garantiert, und implementiert die Entity `IHasId`?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Nicht-nullable Werttyp-Properties in `Entities/` und fehlendes `IHasId` lassen sich per Reflection-Test finden.

---

## CR-011: Tool-Klassen sind `public sealed`, instanzbasiert, mit Primärkonstruktor

### Regel
- Jede MCP-Tool-Klasse ist `[McpServerToolType] public sealed class {Ressource}Tools(IEasyVereinApiClient client)`.
- Keine statischen Tool-Klassen. Eine Klasse pro API-Ressource.
- Zusätzliche Abhängigkeiten, z. B. `EasyVereinConfiguration`, sind nur mit fachlichem Grund zulässig.

### Begründung
25/25 Tool-Klassen folgen genau diesem Muster, 22 davon mit dem Client als einziger Abhängigkeit. Beobachtung B-11, Konfidenz hoch.

### Positives Beispiel
- `Tools/InventoryObjectTools.cs:15`, `Tools/LendingTools.cs:15`
- Begründete Zusatzabhängigkeit: `Tools/InvoiceTools.cs:15` (`config` für Ressourcen-URLs)

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Ist die neue Tool-Klasse `public sealed`, nicht statisch, und bekommt sie ihre Abhängigkeiten per Primärkonstruktor?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Ein Reflection-Test über alle `[McpServerToolType]`-Typen (`IsSealed`, nicht `IsAbstract`) ist möglich.

---

## CR-012: Standardmäßig `internal`; `sealed` für Hilfs- und Infrastrukturtypen

### Regel
- Neue Typen sind `internal`, außer sie werden als API über Projektgrenzen hinweg benötigt (Entities, Interfaces, Converter, Client, Tools).
- Query-Builder, Fields-Klassen und DTOs für Antwort-Wrapper sind `internal`.
- Properties eines `internal`-Typs sind nicht `public`.
- Converter und Value Objects ohne Vererbungsabsicht sind `sealed`.

### Begründung
24/24 Query-Klassen, 24/28 Fields-Klassen und `ApiListResponse<T>` sind `internal`. 6/6 Converter, `ApiVersion` und 25/25 Tool-Klassen sind `sealed`. Beobachtung B-12, Konfidenz mittel, weil es für Entities und Client keine erkennbare `sealed`-Linie gibt.

### Positives Beispiel
- `Infra/LendingQuery.cs` (`internal class`), `Client:2171` (`internal class ApiListResponse<T>`)
- `Dom/Converters/FlexibleIdConverter.cs:14` (`public sealed`)

### Negatives Beispiel
- `public` Properties in `internal` Query-Klassen: `Infra/BookingQuery.cs:14-26`, `Infra/MemberQuery.cs:13` vs. `:18`
- 4 `public` Fields-Klassen (siehe CR-009)

### Review-Frage
Muss der neue Typ bzw. Member wirklich `public` sein? Ist eine Klasse ohne Vererbungsabsicht `sealed`?

### Schweregrad
SHOULD für `internal`. CONSIDER für `sealed`.

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* CA1852 (sealed für nicht abgeleitete interne Typen).

---

## CR-013: Abhängigkeiten über Interfaces; HTTP nur über den Typed Client

### Regel
- Dienste werden über ihr Interface injiziert. Tools hängen von `IEasyVereinApiClient` ab, nie von `EasyVereinApiClient`.
- HTTP-Zugriffe laufen ausschließlich über den per `AddHttpClient<IEasyVereinApiClient, EasyVereinApiClient>()` registrierten Typed Client.
- `new HttpClient()` ist verboten.
- Registrierungen gehören nur in `Program.cs`.

### Begründung
`Program.cs:43` registriert den Typed Client, 25/25 Tools injizieren das Interface, `new HttpClient(` kommt 0× vor (erneut geprüft). Die Server-Tests nutzen die Austauschbarkeit durchgängig (`new Mock<IEasyVereinApiClient>()`). Beobachtung B-13, Konfidenz hoch.

### Positives Beispiel
- `src/MCP.EasyVerein.Server/Program.cs:43`, `:56-80`
- `Tools/LendingTools.cs:15`

### Negatives Beispiel
Im Repository nicht vorhanden. Grenzfall: `Client:43-44` verändert `DefaultRequestHeaders` des injizierten Clients.

### Review-Frage
Wird irgendwo ein `HttpClient` selbst erzeugt oder eine konkrete Klasse statt des Interfaces injiziert?

### Schweregrad
MUST

### Automatisierbar
Teilweise. `new HttpClient(` ist per Textsuche auffindbar, die Interface-Injection per Konstruktor-Reflection prüfbar.

---

## CR-014: Client-Methoden folgen der Signaturform ihrer Operationsart

### Regel
Neue Methoden in `IEasyVereinApiClient` haben diese Form (`ct` immer als letzter Parameter):

| Methode | Rückgabe | Parameter |
|---|---|---|
| `List…Async` | `Task<IReadOnlyList<X>>` | optionale Filter mit `= null` |
| `Get…Async` | `Task<X?>` | `long id` |
| `Create…Async` | `Task<X>` | Entity |
| `Update…Async` | `Task<X>` | `long id, object patchData` |
| `Delete…Async` | `Task` | `long id` |

Abweichungen, etwa eine Rohstring-Rückgabe, brauchen eine Begründung im XML-Kommentar.

### Begründung
117/117 Methoden mit `CancellationToken ct = default`, alle 22 Updates mit `object patchData`, alle List-Methoden mit `IReadOnlyList`. Beobachtung B-14, Konfidenz hoch.

### Positives Beispiel
- `Dom/Interfaces/IEasyVereinApiClient.cs:9-58` (Announcement, BankAccount)
- Begründete Abweichung: `:527` `VoteFeatureRequestAsync` (API verlangt GET, Rückgabe als String dokumentiert)

### Negatives Beispiel
- `:935` `UpdateEventAsync` → `Task<string>`, die XML-Doku sagt Event (`Client:1983-1998`)
- `:906` `ListMembersAsync` ohne Default-Werte, `:870` `ListBookingsAsync` mit `= default`/`= null` gemischt

### Review-Frage
Entspricht die neue Interface-Methode der Signaturform ihrer Operationsart, und ist eine Abweichung dokumentiert?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Rückgabetypen pro Verb-Präfix lassen sich per Reflection-Test prüfen.

---

# Nullable

## CR-015: Nullable ist aktiv; `!` nur direkt nach einer Prüfung

### Regel
- Jedes Projekt setzt `<Nullable>enable</Nullable>`.
- Nullbarkeit wird im Typ ausgedrückt (`Task<X?>`, `string?`).
- Der Null-forgiving-Operator `!` ist nur direkt nach einer Prüfung zulässig, die der Compiler nicht erkennt (z. B. `if (HasValue(s)) patch[…] = s!;`).
- Nullable-Warnungen werden nicht per `#pragma` oder `!` unterdrückt.

### Begründung
Alle 8 `.csproj` aktivieren Nullable. In Infrastructure gibt es 0× `!` (erneut geprüft), in Tools steht es nur nach `HasValue`. Beobachtung B-16, Konfidenz hoch.

### Positives Beispiel
- `Tools/InventoryObjectTools.cs:141-150`: `if (HasValue(s)) patch[…] = s!;`
- `Dom/Interfaces/IEasyVereinApiClient.cs:14`: `Task<Announcement?>`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Steht ein `!` im Diff, und ist unmittelbar davor sichergestellt, dass der Wert nicht `null` ist?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `<TreatWarningsAsErrors>` für `nullable`-Warnungen. `!` ist per Textsuche auffindbar.

---

# Fehlerbehandlung

## CR-016: Tool-Methoden lassen keine Exception an die MCP-Grenze

### Regel
- Jede Tool-Methode umschließt den API-Aufruf mit `try { … } catch (Exception ex)`.
- Im `catch` gibt sie den einheitlichen String zurück:

  ```
  $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}"
  ```

- Tools werfen nicht.

### Begründung
113/118 Tool-Methoden fangen alle Exceptions, 109 mit wörtlich identischem Template (erneut geprüft). Die 5 Ausnahmen sind Legacy (ContactDetails, Invoice). Beobachtung B-18, Konfidenz hoch.

### Positives Beispiel
- `Tools/InventoryObjectTools.cs:44-47`, `Tools/LendingTools.cs:62-65`

### Negatives Beispiel
- Ohne `try/catch`: `Tools/ContactDetailsTools.cs:45,62,82`, `Tools/InvoiceTools.cs:43,282`
- Abweichender Variablenname: `Tools/MemberTools.cs:93,147,169` (`e`)

### Review-Frage
Kann aus dieser Tool-Methode irgendeine Exception entkommen?

### Schweregrad
MUST

### Automatisierbar
Teilweise. Tool-Methoden ohne `try` lassen sich per Roslyn- oder Textanalyse finden.

---

## CR-017: HTTP-Aufrufe nur über die zentralen Client-Helfer; nur gefilterte `catch`-Blöcke

### Regel
- Jeder HTTP-Aufruf im Client läuft über `SendWithErrorHandling(…)`.
- Der Erfolg wird mit `EnsureSuccessOrThrowAsync` geprüft, nicht mit `EnsureSuccessStatusCode`.
- Gefangen werden im Client nur konkrete Exception-Typen mit `when`-Filter. Beim Neuwerfen bleibt die `InnerException` erhalten.
- Ein vom Aufrufer ausgelöster Abbruch (`ct.IsCancellationRequested`) wird nicht in einen Fehler umgedeutet.

### Begründung
95/95 HTTP-Aufrufe laufen über `SendWithErrorHandling` (erneut geprüft). Die beiden `catch`-Blöcke im Client sind gefiltert und behalten die Ursache. Beobachtung B-19, Konfidenz hoch.

### Positives Beispiel
- `Client:2144-2165`: `SendWithErrorHandling` mit
  - `catch (HttpRequestException ex) when (ex.InnerException is SocketException)` (`:2151`)
  - `catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)` (`:2158`)
- `Client:2074`: `EnsureSuccessOrThrowAsync` (401/403 → `UnauthorizedAccessException`)

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Ruft neuer Client-Code `_httpClient` direkt auf, ohne `SendWithErrorHandling`, oder fängt er Exceptions ungefiltert?

### Schweregrad
MUST

### Automatisierbar
Teilweise. `_httpClient.` außerhalb eines `SendWithErrorHandling(`-Lambdas und `catch` ohne `when` im Client sind per Textsuche auffindbar.

---

## CR-018: „Nicht gefunden" ist bei Get kein Fehler

### Regel
- `Get…Async` gibt bei HTTP 404 `null` zurück (`Task<X?>`).
- Das Tool übersetzt `null` in `"{Ressource} with ID {id} not found."`.
- Update, Delete und List werfen bei 404 weiterhin.

### Begründung
24/24 Get-Methoden im Client prüfen `HttpStatusCode.NotFound` und liefern `null` (erneut geprüft, z. B. `Client:94,163,247`). Die Tools reagieren einheitlich darauf. Beobachtung B-20, Konfidenz hoch.

### Positives Beispiel
- `Client:94`: `if (response.StatusCode == HttpStatusCode.NotFound) return null;`
- `Tools/LendingTools.cs`: `get_lending` mit Not-found-Meldung

### Negatives Beispiel
Im Repository nicht vorhanden. Nur Formatabweichungen: `Client:1472`, `:1739`, `:1830`.

### Review-Frage
Behandelt die neue Get-Methode 404 als `null`, und gibt das Tool dafür eine verständliche Meldung aus?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Das Verhalten lässt sich per Test mit Fake-Handler prüfen (Muster in `T.Infra`).

---

## CR-019: Bekannte Pflichtfelder und Formate vor dem API-Aufruf prüfen

### Regel
- Pflichtfelder, Längen, Wertebereiche und Formate, die die API mit HTTP 400 ablehnen würde, prüft das Tool **vorher**.
- Bei ungültiger Eingabe gibt es `"ERROR: …"` mit einem konkreten Hinweis zurück und ruft die API **nicht** auf.
- Ein Test belegt, dass der API-Aufruf unterbleibt (`Times.Never` oder `MockBehavior.Strict`).
- Ungültige Eingaben werden nie still verworfen.

### Begründung
Alle neueren Tools mit bekannten Pflichtfeldern arbeiten so (9 von 25 Tool-Klassen). Die Tests prüfen das 26× mit `Times.Never`/`Strict` (erneut geprüft). Beobachtung B-24, Konfidenz mittel, weil ältere Tools gegenteilig arbeiten.

### Positives Beispiel
- `Tools/LendingTools.cs:105-114` (4 Pflichtfelder, Kommentar zum live verifizierten HTTP 400), `:195-212` (`Validate`)
- `Tools/InventoryObjectTools.cs:94`: `"ERROR: Invalid purchase date '…'. Use YYYY-MM-DD or ISO 8601."`
- `T.Server/LendingToolsTests.cs:90-102` (`Times.Never`)

### Negatives Beispiel
- Still verworfen: `Tools/MemberTools.cs:133-141` (`DateTime.TryParse`/`int.TryParse`; bei Fehlschlag wird das Feld weggelassen)
- `Tools/AnnouncementTools.cs:97-98`, `BankAccountTools.cs:93`, `EventTools.cs:144-146`

### Review-Frage
Was passiert bei einer ungültigen Eingabe: klare `ERROR:`-Meldung ohne API-Aufruf, stiller Verlust oder ein API-Fehler 400?

### Schweregrad
SHOULD

### Automatisierbar
Nein. Ob die Validierung vollständig ist, zeigt nur der Test pro Tool.

---

# Logging und Sicherheit

## CR-020: Nichts schreibt auf stdout, außer dem MCP-Transport

### Regel
- Nach dem Host-Start schreibt kein Code auf stdout, also kein `Console.Write*`, keine Console-Logger und kein `Console.Out`.
- stdout gehört exklusiv dem MCP-Stdio-Protokoll.
- Ausnahme: Ausgaben vor dem Host-Start, z. B. `--help`.

### Begründung
`Program.cs:22` entfernt alle Logging-Provider mit genau dieser Begründung. In `src` gibt es genau einen `Console.WriteLine`, und der steht in `PrintHelp()` vor dem Host-Start (`Program.cs:91`, erneut geprüft). Beobachtung B-23, Konfidenz mittel. Die Regel ergibt sich zwingend aus dem Stdio-Transport.

### Positives Beispiel
- `src/MCP.EasyVerein.Server/Program.cs:22`: `builder.Logging.ClearProviders();`
- `src/MCP.EasyVerein.Server/Program.cs:13`, `:91`: `--help` wird vor dem Start behandelt

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Kann der geänderte Code zur Laufzeit etwas auf stdout ausgeben?

### Schweregrad
MUST

### Automatisierbar
Ja, per Textsuche nach `Console.Write`/`Console.Out`/`AddConsole` in `src`.

> Der Bestand zeigt eine Nebenwirkung: `App/Configuration/EasyVereinConfiguration.cs:115` loggt eine Warnung, die ohne Provider verloren geht (siehe Anhang).

---

## CR-021: Zugangsdaten und Tokens nie im Klartext ausgeben

### Regel
- API-Key, Passwörter und Tokens erscheinen weder in Tool-Rückgaben noch in Fehlermeldungen oder Logs.
- Tools, die solche Daten verarbeiten, maskieren sie und entfernen Eingabe-Geheimnisse aus Exception-Meldungen.
- Die Beschreibung des Tools beginnt mit `SENSITIVE:`.
- Maskierung und Schwärzung sind durch Tests abgesichert.

### Begründung
`get_token` setzt alle vier Maßnahmen um und testet sie. Der API-Key wird nirgends ausgegeben. Beobachtung B-23, Konfidenz mittel: Es gibt nur ein Tool mit sensiblen Daten, dort aber eine eindeutige Absicht.

### Positives Beispiel
- `Tools/GetTokenTools.cs:24-27` (`SENSITIVE:`), `:45` (Maskierung), `:56` (`message.Replace(password, "***")`), `:61-64` (`Mask`)
- `T.Server/GetTokenToolsTests.cs:15-47`, `:89-102`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Kann ein Geheimnis, auch über `ex.Message` oder einen API-Response-Body, in der Rückgabe des Tools landen, und gibt es einen Test dagegen?

### Schweregrad
MUST

### Automatisierbar
Nein

---

# async/await

## CR-022: Durchgängig asynchron; `CancellationToken ct` als letzter Parameter, immer weitergereicht

### Regel
- Jede I/O-Methode ist `async Task`/`Task<T>`.
- Sie nimmt `CancellationToken ct` als **letzten** Parameter: in Interface und Client `= default`, in Tool-Methoden als Pflichtparameter.
- Sie reicht `ct` an jeden awaitbaren Aufruf weiter, der einen Token annimmt.
- `.Result`, `.Wait()`, `GetAwaiter().GetResult()` und `async void` sind verboten.

### Begründung
Die Zählung:
- 117/117 Interface-Methoden und 118/118 Tool-Methoden haben `ct`.
- 95/95 HTTP-Aufrufe erhalten `ct`.
- Sync-over-Async und `async void` kommen in `src` und `tests` 0× vor (erneut geprüft).

Beobachtung B-25, Konfidenz hoch.

### Positives Beispiel
- `Dom/Interfaces/IEasyVereinApiClient.cs`: 117× `CancellationToken ct = default`
- `Client:2099-2121`: `_httpClient.GetAsync(url, ct)`, `ReadAsStringAsync(ct)`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Nimmt jede neue I/O-Methode `ct` entgegen und gibt ihn an **alle** inneren Aufrufe weiter?

### Schweregrad
MUST

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* CA2016 (ct weitergeben), VSTHRD002/VSTHRD100/VSTHRD103. Sync-over-Async ist per Textsuche auffindbar.

> Nicht Teil der Regel: `ConfigureAwait(false)`. Es wird im Bestand 0× verwendet, das ist konsistent, aber nicht als Absicht belegt.

---

# JSON, HTTP und Collections

## CR-023: Updates senden nur gesetzte Felder als PATCH-Dictionary

### Regel
Update-Tools bauen ein `Dictionary<string, object>` mit `*Fields`-Konstanten als Schlüssel:
- String-Parameter werden nur mit `HasValue` (nicht `null`, nicht leer, nicht der Literalstring `"null"`) übernommen.
- Werttypen werden nur mit `.HasValue` übernommen.

Der Client sendet das per `PatchAsync`. PUT und ein Update mit ganzer Entity sind verboten. Ein Test belegt, dass leere Werte und `"null"` nicht mitgesendet werden.

### Begründung
- 21/22 Update-Tools nutzen das Dictionary.
- `PutAsync` kommt 0× vor (erneut geprüft).
- `CLAUDE.md`: „Unsere MCP-Tools nutzen ausschließlich PATCH".
- Die Tests prüfen Typ, Schlüsselanzahl und das Ignorieren von `"null"`.

Ein mitgesendetes `null` bzw. `"null"` würde Daten in easyVerein überschreiben. Beobachtung B-27, Konfidenz hoch.

### Positives Beispiel
- `Tools/InventoryObjectTools.cs:141-150`, `Tools/LendingTools.cs:156-165`
- `T.Server/ForumToolsTests.cs:73-91`, `T.Server/CustomFieldCollectionToolsTests.cs:16-26`

### Negatives Beispiel
- `Tools/ContactDetailsTools.cs:96,101`: ganze `ContactDetails`-Entity als Update-Parameter
- `Tools/BookingTools.cs:128-135`: nur `!= null`, `""`/`"null"` würden also gesendet
- `Tools/AnnouncementTools.cs:102`: `string.IsNullOrEmpty` statt `HasValue`

### Review-Frage
Kann ein nicht angegebener, leerer oder `"null"`-Parameter im PATCH-Body landen und ein bestehendes Feld überschreiben?

### Schweregrad
MUST

### Automatisierbar
Teilweise. `PutAsync` ist per Textsuche auffindbar. Das Weglassen leerer Werte zeigt nur der Test.

---

## CR-024: JSON-Mapping nur mit System.Text.Json, `[JsonPropertyName(Fields.X)]` und Property-Convertern

### Regel
- Es wird ausschließlich System.Text.Json verwendet.
- Jede serialisierte Entity-Property trägt `[JsonPropertyName({Ressource}Fields.Konstante)]`.
- API-Sonderformate (ID als URL, Zahl als String, Datum ohne Zeit) werden per `[JsonConverter(typeof(…))]` an der **einzelnen Property** behandelt, mit den vorhandenen, wiederverwendbaren Convertern:
  - `FlexibleIdConverter`
  - `FlexibleDecimalConverter`
  - `FlexibleDateTimeConverter`
  - `DateOnlyJsonConverter`
  - `FlexibleReferenceConverter<T>`
- Ein neuer entity-spezifischer Converter ist ein Befund, wenn ein generischer ausreicht.

### Begründung
28/28 Entities nutzen `[JsonPropertyName]`, und Newtonsoft kommt 0× vor. Die Referenz-Converter sind generisch über `where T : class, IHasId, new()`. Beobachtung B-28, Konfidenz hoch für diesen Teil. Wann welcher Converter nötig ist, ist **nicht** Teil der Regel (Konfidenz niedrig).

### Positives Beispiel
- `Dom/Entities/Lending.cs:24,30`: `long?` + `FlexibleIdConverter`
- `Dom/Converters/FlexibleReferenceConverter.cs`: generisch
- `Dom/Converters/DateOnlyJsonConverter.cs`: Datumsfelder im API-Format `yyyy-MM-dd`

### Negatives Beispiel
- `Dom/Entities/MemberGroup.cs:15`: `[JsonPropertyName("id")]` als Literal

### Review-Frage
Wird für das neue Feld ein vorhandener Converter wiederverwendet, und stammt der JSON-Name aus der Fields-Klasse?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `using Newtonsoft` und `JsonPropertyName("` mit Literal sind per Textsuche auffindbar.

---

## CR-025: Request-Bodies mit fester Länge, nie chunked

### Regel
- Request-Bodies werden vollständig serialisiert und mit fester `Content-Length` gesendet (`BuildJsonContent`).
- `PostAsJsonAsync`, `PatchAsJsonAsync` oder gestreamtes `JsonContent` ohne feste Länge sind verboten.

### Begründung
Die API antwortet auf Chunked Encoding mit HTTP 411. Das ist in `Client:48-55` dokumentiert und durch einen eigenen Regressionsabschnitt in den Tests abgesichert. 24/24 Creates nutzen `BuildJsonContent`. Beobachtung B-30, Konfidenz hoch.

### Positives Beispiel
- `Client:48-55`: `BuildJsonContent` mit Begründung
- `T.Infra:3276 ff.` (Abschnitt „HTTP transport/POST"), `T.Infra:4207-4219` (`CapturingFakeHttpHandler` zeichnet das Chunked-Flag auf)

### Negatives Beispiel
Im Repository nicht vorhanden. Die 22 Updates bauen `StringContent` inline (`Client:123`, `:207`, `:1945`). Das ist gleichwertig, aber dupliziert.

### Review-Frage
Wird der Body eines neuen Requests mit fester Länge gesendet, und prüft ein Test das?

### Schweregrad
MUST

### Automatisierbar
Teilweise. `PostAsJsonAsync`/`PatchAsJsonAsync`/`JsonContent.Create` sind per Textsuche auffindbar.

---

## CR-026: Listen als `IReadOnlyList<T>`; Paginierung bleibt im Client

### Regel
- Öffentliche Collection-Rückgaben sind `IReadOnlyList<T>`.
- `List…Async` folgt dem `next`-Link der API und gibt alle Seiten gesammelt zurück, und zwar über `HandleListResponseWithPagination`.
- Aufrufer, also Tools, sehen keine Seiten.

### Begründung
Alle List-Methoden des Interfaces liefern `Task<IReadOnlyList<…>>` (erneut geprüft). Die Paginierung ist zentral implementiert und mit `MultiPageFakeHttpHandler` getestet. Beobachtung B-31, Konfidenz hoch.

### Positives Beispiel
- `Client:2099-2121`: `HandleListResponseWithPagination` mit `allResults.AsReadOnly()`
- `T.Infra:4171`: `MultiPageFakeHttpHandler`

### Negatives Beispiel
- Intern: `Client:2171` `ApiListResponse<T>.Results` als veränderliche `List<T>`
- `Client:2049`: `&limit=100` hart codiert

### Review-Frage
Gibt die neue List-Methode ein `IReadOnlyList<T>` zurück, und nutzt sie den zentralen Paginierungs-Helfer?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Rückgabetypen der `List…Async`-Methoden sind per Reflection prüfbar.

---

## CR-027: Eine Query-Klasse pro Ressource mit `FieldQuery` und `ToString()`

### Regel
- Jede Ressource hat eine `internal class {Ressource}Query` mit einer `const string FieldQuery = "query={…}"` aus Fields-Konstanten und Filter-Properties.
- Sie überschreibt `ToString()` und baut den Query-String aus einer `List<string>` von Teilen mit `string.Join("&", parts)`.
- URLs werden nicht an anderer Stelle per String-Konkatenation zusammengesetzt.

### Begründung
`FieldQuery` und der `ToString`-Override stehen jeweils in 24/24 Query-Klassen, `string.Join("&", parts)` ebenfalls in 24/24. Beobachtungen B-32 (mittel) und B-33 (Struktur hoch), zusammen Konfidenz hoch.

### Positives Beispiel
- `Infra/LendingQuery.cs:77` (`FieldQuery`), `:98` (`ToString` mit `AddText`/`AddNumber`)
- `Infra/AnnouncementQuery.cs:34` (minimale Variante)

### Negatives Beispiel
- `Infra/InvoiceQuery.cs:13-23`: Die Klasse wird nie instanziiert, ihre Filter-Properties sind toter Code.
- `Infra/ContactDetailsQuery.cs:101`: Der Filter `Name` wird unter dem Schlüssel `familyName=` gesendet (Fehler).

### Review-Frage
Gibt es für die neue Ressource eine Query-Klasse, und wird jeder Filter unter dem richtigen Fields-Schlüssel ausgegeben?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Das Mapping Filter → Schlüssel prüft nur ein Test (Muster: `LastRequestUri` in `T.Infra`).

> Das **Escaping** der Werte ist bewusst nicht Teil dieser Regel. Im Bestand ist es zu uneinheitlich (B-33), siehe Anhang.

---

## CR-028: API-Werte kulturunabhängig parsen und formatieren

### Regel
- Zahlen und Datumswerte, die an die API gehen oder von ihr kommen, werden mit `CultureInfo.InvariantCulture` geparst und formatiert.
- Feste Formate werden mit `TryParseExact` und explizitem Format geparst.
- Kulturabhängiges `DateTime.TryParse(s, out …)` ohne Kultur ist ein Befund.

### Begründung
Alle kulturabhängigen Aufrufe in den Convertern nutzen `InvariantCulture` (7 Treffer). Neuere Tools verwenden `TryParseExact` bzw. `InvariantCulture`. `DateTime.Now` kommt in `src` nicht vor. Beobachtung B-34, Konfidenz mittel.

### Positives Beispiel
- `Dom/Converters/FlexibleDecimalConverter.cs`, `DateOnlyJsonConverter.cs`
- `Tools/InventoryObjectTools.cs:192`: `DateTimeOffset.TryParse(…, CultureInfo.InvariantCulture, …)`
- `Tools/LendingTools.cs:218`: `DateTime.TryParseExact("yyyy-MM-dd")`

### Negatives Beispiel
- `Tools/MemberTools.cs:133-141`: `DateTime.TryParse`/`int.TryParse` ohne Kultur
- `Tools/AnnouncementTools.cs`, `Tools/BankAccountTools.cs`

### Review-Frage
Hängt das Parsen oder Formatieren eines API-Werts von der Systemkultur des Servers ab?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* CA1304, CA1305.

---

# MCP-Tools

## CR-029: Einheitliches Tool-Skelett

### Regel
- Tool-Methoden haben die Signatur `public async Task<string> Xxx(<Parameter>, CancellationToken ct)`.
- Pflichtparameter sind nicht-nullable (`long id`). Optionale Parameter sind nullable **ohne** Default-Wert und korrekt typisiert (`bool?`, `decimal?`, nicht `string?` für Zahlen oder Booleans).
- Der Ablauf ist: Validierung (CR-019) → `try` → Client-Aufruf → Serialisierung → `catch` (CR-016).

### Begründung
118/118 Tool-Methoden liefern `Task<string>` und nehmen `ct`. Die neueren Tools typisieren optionale Parameter korrekt. Beobachtung B-36, Konfidenz hoch.

### Positives Beispiel
- `Tools/LendingTools.cs:24-65`, `Tools/InventoryObjectTools.cs:23-47`

### Negatives Beispiel
- `Tools/EventTools.cs:131-133`, `Tools/MemberTools.cs:124-127`: Booleans und Zahlen als `string?`
- `Tools/EventTools.cs:148`: gibt Roh-JSON aus `UpdateEventAsync` zurück

### Review-Frage
Folgt die neue Tool-Methode dem Skelett, und sind ihre Parameter so typisiert, wie der MCP-Client sie senden soll?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Rückgabetyp, `ct` und Parametertypen sind per Reflection-Test prüfbar.

---

# Dokumentation

## CR-030: Englische `/// <summary>` an jedem Member, auch `private`

### Regel
- Jeder Typ und jeder Member im Produktivcode hat einen englischen XML-Dokumentationskommentar.
- Das gilt auch für private Felder, Konstanten, Konstruktoren und Hilfsmethoden.

### Begründung
`CLAUDE.md` („Code-Konventionen"): „Alle Members (inkl. private) müssen englische XML-Dokumentationskommentare haben". Umgesetzt ist das in Infrastructure (398/398) und Tools (162/162) vollständig, in Domain fehlen 9 Member. Beobachtungen B-37 und B-22 (Sprache der XML-Doku), Konfidenz hoch.

### Positives Beispiel
- `Client:17-24`: private Felder mit `<summary>`
- `Dom/ValueObjects/ApiVersion.cs:8,11,17`: private Konstante, Feld, Konstruktor

### Negatives Beispiel
- Ohne Doku: `Dom/Entities/AccountingPlan.cs:7,9`, `Location.cs:7,9,10`, `Member.cs:185-186`, `Dom/ValueObjects/MemberFields.cs:77`
- Deutsche Summaries in Testhilfen: `T.Infra:4143,4168,4198`

### Review-Frage
Hat jeder neue Member, auch private, einen englischen `<summary>`?

### Schweregrad
MUST

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `<GenerateDocumentationFile>` + CS1591 decken nur öffentliche Member ab. Für private Member ist eine StyleCop-Regel (SA1600 mit `documentPrivateElements`) oder ein eigenes Skript nötig.

> Für **Testmethoden** ist die Regel im Bestand nicht umgesetzt: Nur Server.Tests dokumentiert sie. Ob `CLAUDE.md` Tests einschließt, sollte ausdrücklich entschieden werden.

---

## CR-031: Englische `[Description]` an jedem Tool und jedem Tool-Parameter

### Regel
- Jede Tool-Methode und jeder ihrer Parameter (außer `ct`) trägt eine englische `[Description]`.
- Die Beschreibung ist das Einzige, was der LLM-Client über das Tool erfährt. Sie nennt deshalb Pflichtfelder, erlaubte Formate (z. B. `YYYY-MM-DD`) und Wertebereiche.

### Begründung
538 von 568 Tool-Parametern sind beschrieben. Die 30 fehlenden liegen alle in den Legacy-Klassen ContactDetails und Invoice. Beobachtung B-38, Konfidenz hoch.

### Positives Beispiel
- `Tools/LendingTools.cs:24-48`: 22 Filterparameter, jeder mit Beschreibung
- `Tools/GetTokenTools.cs:24-32`

### Negatives Beispiel
- `Tools/ContactDetailsTools.cs`: 11 Parameter ohne Beschreibung
- `Tools/InvoiceTools.cs:43,74-91,282`: 19 Parameter ohne Beschreibung

### Review-Frage
Könnte ein LLM das Tool allein anhand der Beschreibungen korrekt aufrufen?

### Schweregrad
MUST

### Automatisierbar
Ja. Ein Reflection-Test über alle Tool-Parameter (außer `CancellationToken`) auf `DescriptionAttribute` ist möglich.

---

## CR-032: Löschende Tools benennen Berechtigung und Löschverhalten

### Regel
- Die `[Description]` jedes `delete_*`-Tools endet mit „Only authorized users are able to perform this action!".
- Sie sagt, ob der Datensatz in den Papierkorb wandert oder endgültig gelöscht wird.
- Die Rückgabemeldung nach dem Löschen sagt dasselbe wie die Beschreibung.

### Begründung
21/21 Delete-Tools haben den Berechtigungshinweis. Die neueren Tools (InventoryObject, InventoryObjectGroup, Lending) nennen den Papierkorb in Beschreibung **und** Rückgabe. Beobachtung B-38, Teil Löschverhalten, Konfidenz mittel.

### Positives Beispiel
- `Tools/LendingTools.cs:177` (Beschreibung „wastebasket") und `:185` (Rückgabe „moved to the wastebasket")
- `Tools/InventoryObjectTools.cs:162`, `:170`

### Negatives Beispiel
- `Tools/CustomTaxRateTools.cs:118` sagt Papierkorb, `:126` meldet „has been deleted".
- 16 Delete-Tools ohne Angabe zum Löschverhalten

### Review-Frage
Weiß der Nutzer vor dem Aufruf, ob der Datensatz wiederherstellbar ist, und bestätigt die Rückgabe das?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Der Berechtigungssatz ist per Reflection prüfbar, die Übereinstimmung von Beschreibung und Rückgabe nicht.

---

## CR-033: Nicht offensichtliche Entscheidungen mit FR-ID oder beobachtetem API-Verhalten begründen

### Regel
Weicht Code vom Erwartbaren ab oder bildet er ein API-Verhalten nach (Pflichtfelder, Formate, HTTP 411, schreibbare `read_only`-Felder), dann nennt ein Kommentar den Grund:
- die Anforderungs-ID (`FR-xxx`/`NFR-xxx`), oder
- das live beobachtete API-Verhalten, möglichst mit Datum.

### Begründung
Häufig im Code und in Tests, z. B. `Client:42` „(FR-002)", `Tools/LendingTools.cs:105-106`, `T.Server/LendingToolsTests.cs:81-84` „verified live on 2026-09-22". Beobachtung B-39, Konfidenz mittel.

### Positives Beispiel
- `Client:48-51`: Begründung für `BuildJsonContent` (HTTP 411)
- `Client:2144-2165`: `NFR-002` an der Netzwerk- und Timeout-Behandlung
- `Dom/ValueObjects/ApiVersion.cs:4`: „(FR-010 to FR-015)"

### Negatives Beispiel
- Unbegründete Parser-Logik in älteren Tools, z. B. `Tools/MemberTools.cs:133-141`

### Review-Frage
Würde ein anderer Entwickler verstehen, **warum** dieser Code so ist, ohne die API erneut auszuprobieren?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

# Tests

## CR-034: Neuer Code wird in der gespiegelten Testklasse `<Subjekt>Tests` getestet

### Regel
- Jede neue Produktivklasse bekommt Tests in `tests/<Projekt>.Tests/…/<Subjekt>Tests.cs`.
- Das gilt insbesondere für neue Tool-Klassen, die eine eigene `<Ressource>ToolsTests`-Klasse erhalten.

### Begründung
Die Struktur ist durchgängig: 4 gespiegelte Testprojekte, `<Subjekt>Tests`, eine Datei pro Klasse. `CLAUDE.md` schreibt TDD „vor der Implementierung" vor. Beobachtung B-41, Konfidenz hoch.

### Positives Beispiel
- `T.Server/LendingToolsTests.cs`, `T.Server/GetTokenToolsTests.cs`
- `tests/MCP.EasyVerein.Domain.Tests/…/LendingEntityTests.cs`

### Negatives Beispiel
- 14 von 25 Tool-Klassen ohne eigene Testklasse, darunter Member, Event, Booking und Calendar
- `T.Infra`: eine Datei mit 4 250 Zeilen für den gesamten Client

### Review-Frage
Gibt es für jede neue bzw. geänderte Klasse eine passende `<Subjekt>Tests`-Klasse mit neuen Tests?

### Schweregrad
MUST (laut `CLAUDE.md` TDD-Pflicht)

### Automatisierbar
Teilweise. Die Existenz der Testklasse ist per Skript prüfbar. Eine Coverage-Schwelle (siehe Anhang) würde fehlende Tests aufdecken.

---

## CR-035: Kein echtes HTTP in Unit-Tests; Interface-Mock für Tools, Fake-Handler für den Client

### Regel
- Tool-Tests mocken `IEasyVereinApiClient` mit Moq und prüfen den übergebenen Payload per `.Callback`.
- „Kein API-Aufruf" wird mit `Times.Never` oder `MockBehavior.Strict` belegt.
- Client-Tests verwenden die vorhandenen Fake-Handler (`FakeHttpHandler`, `MultiPageFakeHttpHandler`, `CapturingFakeHttpHandler`) und prüfen Methode, Pfad und Body.
- Echte Netzwerkaufrufe in Unit-Tests sind verboten.

### Begründung
Durchgängig umgesetzt: 26× `Times.Never`/`Strict`, 189× `LastRequestUri`. Echtes HTTP kommt in Tests nicht vor. Beobachtung B-42, Konfidenz hoch.

### Positives Beispiel
- `T.Server/LendingToolsTests.cs:58-60` (Callback), `:102` (`Times.Never`)
- `T.Server/GetTokenToolsTests.cs:77` (`MockBehavior.Strict`)
- `T.Infra:4146`, `:4171`, `:4201` (Fake-Handler)

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Verlässt der neue Test den Prozess, und prüft er, **was** an die API bzw. den Client übergeben wird, nicht nur, dass etwas zurückkommt?

### Schweregrad
MUST für das Verbot echter HTTP-Aufrufe. SHOULD für das Mock-Muster.

### Automatisierbar
Teilweise. `new HttpClient()` ohne Fake-Handler in `tests/` ist per Textsuche auffindbar.

---

## CR-036: Tests sind unabhängig; Setup über statische Factory-Helfer

### Regel
- Tests teilen keinen Zustand: keine Test-Konstruktoren mit Setup, keine `IClassFixture`, keine statischen veränderlichen Felder.
- Jeder Test erzeugt seine Mocks selbst, gemeinsames Setup läuft über statische Helfer (`CreateClient`, `CreateConfig`).
- Mehrere Asserts pro Test sind zulässig, solange sie **ein** Verhalten prüfen.

### Begründung
`IClassFixture`, `IAsyncLifetime` und Test-Konstruktoren kommen 0× vor. Statische Helfer gibt es in Server und Infrastructure. Beobachtung B-43, Konfidenz hoch.

### Positives Beispiel
- `T.Infra:11`: `CreateClient`
- `T.Server/InvoiceToolsTests.cs:14`: `CreateConfig`
- `T.Server/LendingToolsTests.cs:69-78`: mehrere Asserts zu einem Payload

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Hängt das Ergebnis dieses Tests von der Ausführungsreihenfolge oder von einem anderen Test ab?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Konstruktoren und `IClassFixture` in Testklassen sind per Textsuche auffindbar.

---

## CR-037: Jeder Bugfix bekommt einen Regressionstest, der den Fehler benennt

### Regel
- Ein Fix enthält einen Test, der ohne den Fix fehlschlägt.
- Der Name des Tests bzw. der Abschnittstitel beschreibt das Fehlverhalten, z. B. „Chunked", „null-omission", „RedactsPassword".

### Begründung
Eigene Regressionsabschnitte in `T.Infra` (`:3276` HTTP transport/POST, `:3341` null-omission) und benannte Einzeltests. `CLAUDE.md` schreibt Red-Green-Refactor vor. Beobachtung B-45, Konfidenz mittel.

### Positives Beispiel
- `T.Infra:3276 ff.`, `T.Infra:3341 ff.`
- `T.Server/GetTokenToolsTests.cs:89`: `GetToken_OnException_RedactsPassword`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Welcher Test wäre ohne diesen Fix rot gewesen?

### Schweregrad
SHOULD

### Automatisierbar
Nein

---

## CR-038: Reale API-Antworten als versionierte Fixture ablegen

### Regel
Hat eine echte API-Antwort einen Deserialisierungsfehler verursacht, wird sie (anonymisiert) als `Fixtures/{ressource}-{version|real-response}.json` in `Domain.Tests` abgelegt, mit `CopyToOutputDirectory`, und in einem Test deserialisiert.

### Begründung
Drei Fixtures, darunter eine pro API-Version für Member, jeweils mit Test. Beobachtung B-44, Konfidenz mittel.

### Positives Beispiel
- `tests/MCP.EasyVerein.Domain.Tests/Fixtures/member-v1.7.json`, `member-v2.0.json`, `invoice-real-response.json`
- `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj:30-37`

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Stammt der Anlass für diese Änderung aus einer echten API-Antwort, und liegt diese als Fixture vor?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

# Konfiguration

## CR-039: Neue Einstellungen über `IConfiguration` mit Env-Variable und CLI-Switch

### Regel
Eine neue Einstellung wird in drei Schritten ergänzt:
1. als Umgebungsvariable `EASYVEREIN_*`,
2. als CLI-Switch in den Switch-Mappings (`Program.cs`),
3. mit Default in `EasyVereinConfiguration`, aufgelöst über `FromConfiguration`.

Die Priorität ist CLI > Umgebung > Default. `Environment.GetEnvironmentVariable` im Anwendungscode ist ein Befund.

### Begründung
Die Priorität ist in `CLAUDE.md` („Konfiguration (Priorität)") dokumentiert und über die Provider-Reihenfolge umgesetzt. Beobachtung B-47, Konfidenz mittel: Es gibt nur eine Konfigurationsklasse.

### Positives Beispiel
- `src/MCP.EasyVerein.Server/Program.cs:26-32` (`AddCommandLine` mit Switch-Mappings)
- `App/Configuration/EasyVereinConfiguration.cs:81` (`FromConfiguration`), `:108` (`Resolve`)

### Negatives Beispiel
- `App/Configuration/EasyVereinConfiguration.cs:51`: `FromEnvironment()`, als „Legacy" markiert, liest direkt und verhält sich bei einem fehlenden Key anders

### Review-Frage
Ist die neue Einstellung per CLI **und** Umgebungsvariable setzbar, mit dokumentiertem Default?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `Environment.GetEnvironmentVariable` ist per Textsuche auffindbar.

---

# Anhang: Allgemeine C#-Empfehlungen

**Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet.**

Die folgenden Punkte betreffen Befunde, für die der Bestand **keine** positive Konvention zeigt (Konfidenz niedrig oder durchgängig anders umgesetzt). Sie sind keine CR-Regeln. Sie stehen hier, weil die Beobachtungen konkrete Risiken aufgezeigt haben.

| Thema | Empfehlung | Bezug im Repository |
|---|---|---|
| IDisposable | `HttpResponseMessage` und `StringContent` mit `using`/`using var` freigeben | 0× `using` in Infrastructure (B-26) |
| URL-Escaping | Jeden Nutzerwert in Query-Strings mit `Uri.EscapeDataString` codieren | uneinheitlich; `Ordering` 0/24 escaped; `EventQuery`, `CalendarQuery`, `BookingQuery` (B-33) |
| Serializer-Optionen | `JsonSerializerOptions` als `static readonly` wiederverwenden | 93× `new JsonSerializerOptions` in Tools (B-29) |
| Argumentprüfung | Öffentliche Methoden mit `ArgumentNullException.ThrowIfNull` absichern | 0 Guards in Client und Domain (B-17) |
| Logging | Einen stderr-Logger registrieren (`LogToStandardErrorThreshold`), damit Warnungen nicht verloren gehen | `EasyVereinConfiguration.cs:115` + `Program.cs:22` (B-23) |
| Duplikation | `HasValue`, das `catch`-Template und die Serialisierung in einen gemeinsamen Helfer auslagern | 21× / 109× / 93× dupliziert (B-40) |
| Durchsetzung | Coverage-Schwelle in der CI erzwingen, Pre-Commit-Hook einrichten, `.editorconfig` mit Analyzern | in `CLAUDE.md` gefordert, technisch nicht umgesetzt (B-46) |
| Zeittypen | Für Zeitstempel einheitlich `DateTimeOffset?` verwenden | `DateTime?`/`DateTimeOffset?` gemischt, teils in einer Klasse (B-35) |
| Converter-Fehler | Converter werfen bei ungültiger Eingabe einheitlich `JsonException` | `FormatException` bzw. `null` in einzelnen Convertern (B-21) |

---

# Übersicht

| Regel | Titel | Schweregrad | Automatisierbar | Quelle |
|---|---|---|---|---|
| CR-001 | Referenzen nach innen, Domain abhängigkeitsfrei | MUST | Teilweise | B-01 |
| CR-002 | Fünf-Schritte-Muster, Tool registriert | MUST / SHOULD | Teilweise | B-03 |
| CR-003 | File-scoped Namespace, ein Typ pro Datei | SHOULD | Ja | B-02 |
| CR-004 | `Async`-Suffix, CRUD-Verben | SHOULD | Teilweise | B-04 |
| CR-005 | Toolnamen explizit `verb_ressource` | MUST | Ja | B-05 |
| CR-006 | `_camelCase`, Primärkonstruktor | SHOULD | Ja | B-06 |
| CR-007 | Properties spiegeln API-Namen | CONSIDER | Nein | B-07 |
| CR-008 | Testnamen `Methode_Szenario_Erwartung` | SHOULD | Teilweise | B-08 |
| CR-009 | Fields-Konstanten statt Literale | SHOULD | Teilweise | B-09 |
| CR-010 | Entities: nullable POCO + `IHasId` | SHOULD | Teilweise | B-10 |
| CR-011 | Tools `public sealed`, Primärkonstruktor | SHOULD | Teilweise | B-11 |
| CR-012 | `internal` standardmäßig, `sealed` für Hilfstypen | SHOULD / CONSIDER | Teilweise | B-12 |
| CR-013 | Interface-Injection, Typed HttpClient | MUST | Teilweise | B-13 |
| CR-014 | Signaturform Client-Interface | SHOULD | Teilweise | B-14 |
| CR-015 | Nullable aktiv, `!` nur nach Prüfung | SHOULD | Teilweise | B-16 |
| CR-016 | Keine Exception an der MCP-Grenze | MUST | Teilweise | B-18 |
| CR-017 | Zentrale HTTP-Helfer, gefilterte `catch` | MUST | Teilweise | B-19 |
| CR-018 | 404 bei Get → `null` → not found | SHOULD | Teilweise | B-20 |
| CR-019 | Vorab-Validierung ohne API-Aufruf | SHOULD | Nein | B-24 |
| CR-020 | Kein stdout außer MCP | MUST | Ja | B-23 |
| CR-021 | Keine Geheimnisse im Klartext | MUST | Nein | B-23 |
| CR-022 | async + `ct` durchgereicht | MUST | Teilweise | B-25 |
| CR-023 | PATCH nur mit gesetzten Feldern | MUST | Teilweise | B-27 |
| CR-024 | STJ + `[JsonPropertyName(Fields)]` + Converter | SHOULD | Teilweise | B-28 |
| CR-025 | Request-Body mit fester Länge | MUST | Teilweise | B-30 |
| CR-026 | `IReadOnlyList<T>`, zentrale Paginierung | SHOULD | Teilweise | B-31 |
| CR-027 | Query-Klasse pro Ressource | SHOULD | Teilweise | B-32, B-33 |
| CR-028 | Kulturunabhängiges Parsen | SHOULD | Ja | B-34 |
| CR-029 | Einheitliches Tool-Skelett | SHOULD | Teilweise | B-36 |
| CR-030 | Englische XML-Doku an allen Membern | MUST | Teilweise | B-37, B-22 |
| CR-031 | `[Description]` an Tool und Parametern | MUST | Ja | B-38 |
| CR-032 | Delete: Berechtigung + Löschverhalten | SHOULD | Teilweise | B-38 |
| CR-033 | Begründung per FR-ID / Live-Verhalten | CONSIDER | Nein | B-39 |
| CR-034 | Gespiegelte `<Subjekt>Tests` | MUST | Teilweise | B-41 |
| CR-035 | Kein echtes HTTP, Mock-Muster | MUST / SHOULD | Teilweise | B-42 |
| CR-036 | Unabhängige Tests, statische Helfer | SHOULD | Teilweise | B-43 |
| CR-037 | Regressionstest pro Bugfix | SHOULD | Nein | B-45 |
| CR-038 | Reale Antworten als Fixture | CONSIDER | Nein | B-44 |
| CR-039 | Konfiguration über `IConfiguration` | SHOULD | Teilweise | B-47 |
