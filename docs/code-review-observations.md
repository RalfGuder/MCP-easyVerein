# Code-Review-Beobachtungen – MCP-easyVerein

> **Status:** Beobachtungssammlung, **keine** endgültigen Review-Regeln.
> **Quelle:** `main` @ `c1ca5a0` (nach Merge PR #128, US-0029 Lending)
> **Erstellt:** 2026-09-29

## Evidenzbasis und Methodik

| Projekt | .cs-Dateien | Zeilen |
|---|---|---|
| `src/MCP.EasyVerein.Domain` | 69 | ~5 150 |
| `src/MCP.EasyVerein.Application` | 1 | ~140 |
| `src/MCP.EasyVerein.Infrastructure` | 25 | ~4 190 |
| `src/MCP.EasyVerein.Server` | 27 | ~4 140 |
| `tests/*` (4 Projekte) | 53 | ~9 000 |

Zusätzlich ausgewertet: `.csproj`, `MCP.EasyVerein.sln`, `Directory.Build.props`, `.github/workflows/*.yml`, `CLAUDE.md`.

**Vorgehen:**
- Die Muster wurden per Volltextsuche gezählt, z. B. „113 von 118 Tool-Methoden".
- Gegenbeispiele wurden gezielt gesucht.
- Stichproben wurden von Hand verifiziert: `ContactDetailsQuery.cs:101`, `UpdateEventAsync`, `using`-Zählung, `catch`-Zählung, Block-Namespaces, `IHasId`, Git-Hooks.
- Mehrere Fundstellen im selben Muster-Skelett zählen als unabhängig, wenn sie zu verschiedenen Endpoints (User Stories) gehören.

**Abkürzungen für Pfade:**
- `Dom/` = `src/MCP.EasyVerein.Domain/`
- `App/` = `src/MCP.EasyVerein.Application/`
- `Infra/` = `src/MCP.EasyVerein.Infrastructure/ApiClient/`
- `Client` = `Infra/EasyVereinApiClient.cs`
- `Tools/` = `src/MCP.EasyVerein.Server/Tools/`
- `Program.cs` = `src/MCP.EasyVerein.Server/Program.cs`
- `T.Infra` = `tests/MCP.EasyVerein.Infrastructure.Tests/EasyVereinApiClientTests.cs`
- `T.Server/` = `tests/MCP.EasyVerein.Server.Tests/`
- `T.Dom/` = `tests/MCP.EasyVerein.Domain.Tests/`

**Kategorien:**
- **K1 – bewusste Konvention:** explizit dokumentiert (`CLAUDE.md`, Kommentar, Test) oder durchgängig und erkennbar gewollt
- **K2 – häufiges Muster:** in vielen unabhängigen Stellen, aber nirgends festgeschrieben
- **K3 – Zufall / inkonsistent:** vereinzelt oder widersprüchlich

**Häufigkeit:** *sehr häufig* ≥ 20 Stellen bzw. ≥ 80 % · *häufig* 5–19 · *gelegentlich* 2–4 · *Einzelfall* 1

**Konfidenz:**
- *hoch*: mehrere unabhängige Fundstellen, kaum Gegenbeispiele
- *mittel*: belegt, aber mit relevanten Gegenbeispielen
- *niedrig*: Einzelfall oder widersprüchlich

### Querschnittsbefund: zwei Code-Generationen

Viele Inkonsistenzen konzentrieren sich auf die **sechs zuerst implementierten Endpoints**: Member, ContactDetails, Invoice, Event, Booking und Calendar (US-0001, US-0009, US-0014). Die später ergänzten Endpoints (US-0010 ff.) folgen einem einheitlicheren, strengeren Muster. Wo unten Gegenbeispiele genannt sind, stammen sie überwiegend aus dieser „Legacy-Gruppe". Für die Regelbildung heißt das: **Maßstab sind die neueren Endpoints**, z. B. Lending, InventoryObject, InventoryObjectGroup und Forum.

---

# B-01 · Architektur · Vier Schichten mit Abhängigkeiten nur nach innen

## Beobachtung
Die Solution folgt Clean Architecture in vier Projekten. Die Projektreferenzen zeigen ausschließlich nach innen:
- Domain hat keine Projekt- und keine Paketreferenz.
- Application referenziert Domain.
- Infrastructure referenziert Application und Domain.
- Server referenziert alle drei.

Jedes `src`-Projekt hat ein gespiegeltes Testprojekt.

## Beispiele
- `Dom/MCP.EasyVerein.Domain.csproj`: keine `ProjectReference`, keine `PackageReference`
- `App/MCP.EasyVerein.Application.csproj`: referenziert nur Domain und `Microsoft.Extensions.*.Abstractions`
- `src/MCP.EasyVerein.Infrastructure/*.csproj`: referenziert Application und Domain
- `MCP.EasyVerein.sln`: Solution-Ordner `src` und `tests` mit je 4 Projekten
- `CLAUDE.md` Abschnitt „Clean Architecture (4 Schichten)"

## Häufigkeit
sehr häufig (8 von 8 Projekten konsistent)

## Vermutete Regel
Domain bleibt ohne externe Abhängigkeiten. Referenzen zeigen nur von außen nach innen (Server → Infrastructure → Application → Domain). Jedes neue `src`-Projekt bekommt ein Testprojekt `<Projekt>.Tests`.

## Konfidenz
hoch

## Gegenbeispiele
- `Dom/MCP.EasyVerein.Domain.csproj`: `InternalsVisibleTo` für **Infrastructure und Server**. Äußere Schichten greifen damit auf interne Domain-Typen zu (die `*Fields`-Klassen, B-09). Die Kapselung wird also über Schichtgrenzen hinweg geöffnet.
- Application enthält nur eine Klasse (`App/Configuration/EasyVereinConfiguration.cs`). Eine Anwendungslogik- oder Use-Case-Schicht gibt es nicht; die Server-Tools rufen den Infrastructure-Client direkt über das Domain-Interface auf.
- Es gibt keine automatisierte Absicherung der Schichtgrenzen (keine Architekturtests).

---

# B-02 · Struktur · Ordner = Namespace, file-scoped Namespaces, ein Typ pro Datei

## Beobachtung
Der Namespace entspricht dem Ordnerpfad (`MCP.EasyVerein.Domain.Entities`, `…ValueObjects`, `…Converters`, `MCP.EasyVerein.Server.Tools`). Namespaces sind file-scoped, jede Datei enthält genau einen Typ mit gleichem Namen.

## Beispiele
- `Dom/Entities/Member.cs:7`, `Dom/Converters/FlexibleIdConverter.cs:5`, `Dom/Interfaces/IHasId.cs:1`
- `Infra/LendingQuery.cs`, `Infra/AnnouncementQuery.cs`
- `Tools/LendingTools.cs`, `Tools/InventoryObjectTools.cs`
- Zählung: Domain 59/69 file-scoped, Query-Klassen 21/24, Tools 25/25

## Häufigkeit
sehr häufig

## Vermutete Regel
File-scoped Namespace passend zum Ordner. Ein öffentlicher bzw. interner Top-Level-Typ pro Datei, Dateiname = Typname.

## Konfidenz
hoch

## Gegenbeispiele
- Block-scoped Namespace in 10 Domain-Dateien: `Entities/AccountingPlan.cs`, `Location.cs`, `Organization.cs` und `ValueObjects/{AccountingPlan,ContactDetails,Event,Invoice,Location,Member,Organization}Fields.cs`
- Block-scoped in 3 Query-Klassen: `Infra/InvoiceQuery.cs:3`, `MemberQuery.cs:3`, `ContactDetailsQuery.cs:3`
- Zwei Typen in einer Datei: `Dom/ValueObjects/FeatureRequestAuthor.cs:9,27`; `Client:2171` (`ApiListResponse<T>` in der Client-Datei)
- Testhilfen als Top-Level-Klassen in der Testdatei: `T.Infra:4139 ff.`
- `AccountingPlanFields.cs:1-5`, `LocationFields.cs:1-5`: ungenutzte `using`-Direktiven und BOM, vermutlich Reste einer Vorlage

---

# B-03 · Architektur · Festes Fünf-Schritte-Muster pro Endpoint

## Beobachtung
Jeder API-Endpoint wird nach demselben Ablauf angelegt:
1. Entity + `*Fields`
2. Methoden in `IEasyVereinApiClient`
3. Client-Implementierung + `*Query`
4. `*Tools`-Klasse
5. `.WithTools<T>()` in `Program.cs`

Der Ablauf ist in `CLAUDE.md` („Muster für neue Endpoints") dokumentiert und im Code vollständig umgesetzt.

## Beispiele
- Lending: `Dom/Entities/Lending.cs`, `Dom/ValueObjects/LendingFields.cs`, `Infra/LendingQuery.cs`, `Client:1443 ff.`, `Tools/LendingTools.cs`, `Program.cs` (`WithTools<LendingTools>`)
- InventoryObject und Forum: dasselbe Set
- `Program.cs:56-80`: 25 `.WithTools<>`-Aufrufe entsprechen genau den 25 `[McpServerToolType]`-Klassen, keine fehlt und keine ist zu viel

## Häufigkeit
sehr häufig (25 Endpoints)

## Vermutete Regel
Ein neuer Endpoint besteht aus Entity, Fields, Interface-Methoden, Client-Methoden, Query-Klasse, Tool-Klasse und Registrierung. Ein PR, der nur einen Teil davon enthält, ist unvollständig.

## Konfidenz
hoch

## Gegenbeispiele
- `MemberGroup` hat keine `*Fields`-Datei und nutzt als einzige Entity ein Literal (`Dom/Entities/MemberGroup.cs:15`).
- Invoice hat keine `ListInvoicesAsync`-Methode. Stattdessen gibt es `GetInvoicesAsync(ct)` ohne Filter (`Dom/Interfaces/IEasyVereinApiClient.cs:852`, `Client:1814`), und `InvoiceQuery` wird nirgends instanziiert. Dazu passt, dass `list_invoices` keine Filter hat (`Tools/InvoiceTools.cs:23`).
- Die Legacy-Sechs sind in Interface und Client nach Verb statt nach Ressource gruppiert (`IEasyVereinApiClient.cs:731-951`, `Client:1558-2031`).

---

# B-04 · Namenskonventionen · `Async`-Suffix und einheitliche CRUD-Verben

## Beobachtung
Alle asynchronen öffentlichen Methoden tragen den Suffix `Async`. Die Client-Methoden folgen dem Verbschema `List…Async` (Plural), `Get…Async`, `Create…Async`, `Update…Async` und `Delete…Async`.

## Beispiele
- `Dom/Interfaces/IEasyVereinApiClient.cs:9` `ListAnnouncementsAsync`, `:14` `GetAnnouncementAsync`, `:58` `UpdateBankAccountAsync`
- Zählung Interface: 117/117 mit `Async`. Verteilung: List 23, Get 26, Create 23, Update 22, Delete 22, Vote 1
- Client: 117/117 öffentliche Methoden `async Task…` mit Suffix

## Häufigkeit
sehr häufig

## Vermutete Regel
Asynchrone Methoden enden auf `Async`. Datenzugriffe heißen `List{Ressourcen}Async`, `Get{Ressource}Async`, `Create…`, `Update…` bzw. `Delete…`.

## Konfidenz
hoch

## Gegenbeispiele
- `GetInvoicesAsync` statt `ListInvoicesAsync` (`IEasyVereinApiClient.cs:852`)
- Private asynchrone Hilfsmethoden **ohne** Suffix: `SendWithErrorHandling` (`Client:2144`), `HandleResponse` (`:2130`), `HandleListResponseWithPagination` (`:2099`). Dagegen `EnsureSuccessOrThrowAsync` (`:2074`) mit Suffix.
- Tool-Methoden in `Tools/` haben keinen `Async`-Suffix (`GetLending`, `CreateLending`). Das ist durchgängig so, deshalb wohl bewusst: Der Methodenname ist dort nicht die MCP-Schnittstelle.

---

# B-05 · Namenskonventionen · MCP-Toolnamen `verb_ressource` in snake_case, explizit gesetzt

## Beobachtung
Jede Tool-Methode setzt den MCP-Namen explizit per `[McpServerTool(Name = "…")]` in snake_case nach dem Schema `verb_ressource`.

## Beispiele
- `Tools/LendingTools.cs`: `list_lendings`, `get_lending`, `create_lending`, `update_lending`, `delete_lending`
- `Tools/GetTokenTools.cs:24`: `get_token`
- `Tools/FeatureRequestTools.cs`: `vote_feature_request`
- Zählung: 113 `Name =`-Attribute, alle in snake_case (Regex-Gegenprobe ohne Abweichung)

## Häufigkeit
sehr häufig

## Vermutete Regel
Jedes Tool erhält einen expliziten Namen `verb_ressource` in snake_case (Liste im Plural, sonst Singular). Es wird nicht auf den vom SDK abgeleiteten Standardnamen vertraut.

## Konfidenz
hoch

## Gegenbeispiele
- `Tools/ContactDetailsTools.cs:24,44,61,81,95`: `[McpServerTool, Description(…)]` **ohne** `Name`. Das sind 5 Tools, die sich auf die SDK-Ableitung verlassen.
- `Tools/MemberTools.cs:47,74,115,159`: `Name="…"` ohne Leerzeichen (nur Formatierung)

---

# B-06 · Namenskonventionen · Private Felder `_camelCase`, Primärkonstruktor-Parameter camelCase

## Beobachtung
Private Instanzfelder heißen `_camelCase`. Tool-Klassen verwenden stattdessen Primärkonstruktoren, deren Parameter (`client`, `config`) direkt genutzt werden.

## Beispiele
- `Client:18,21,24`: `_config`, `_httpClient`, `_jsonOptions`
- `Dom/ValueObjects/ApiVersion.cs:9`: `_supportedVersions`
- `Tools/LendingTools.cs:15`: `public sealed class LendingTools(IEasyVereinApiClient client)`, 25/25 Tool-Klassen

## Häufigkeit
häufig

## Vermutete Regel
Private Felder heißen `_camelCase`. In Klassen mit Primärkonstruktor werden die Parameter direkt verwendet, ohne zusätzliches Feld.

## Konfidenz
mittel. Es gibt nur wenige private Felder im Code, aber kein Gegenbeispiel.

## Gegenbeispiele
- Keine abweichenden Feldnamen gefunden.
- Private Konstanten werden in PascalCase geschrieben (`DefaultVersion`, `DateFormat` in `Dom/Converters/DateOnlyJsonConverter.cs:15`). Das ist konsistent, aber eine andere Kategorie.

---

# B-07 · Namenskonventionen · Entity-Properties spiegeln die API-Feldnamen

## Beobachtung
Property-Namen sind die PascalCase-Form des API-Feldnamens. Nur vereinzelt wird davon abgewichen, meist um Tippfehler der API zu korrigieren oder Namenskonflikte aufzulösen. Boolesche Properties bekommen deshalb meist **keinen** `Is`/`Has`-Präfix.

## Beispiele
- `Dom/Entities/Member.cs`: `PaymentIntervalMonths` ↔ API `paymentIntervallMonths` (`Dom/ValueObjects/MemberFields.cs:66`), Tippfehler korrigiert
- `Dom/Entities/Invoice.cs`: `InvoiceNumber` ↔ API `invNumber`
- `Dom/Entities/CustomTaxRate.cs`: `CustomTaxRateValue`, weil ein Member nicht wie seine Klasse heißen darf
- Boolesche Properties: 25 von 55 ohne Präfix (`Blocked`, `Deleted`, `Gross`, `Shared`)

## Häufigkeit
sehr häufig

## Vermutete Regel
Properties übernehmen den API-Namen in PascalCase. Abweichungen sind nur bei Tippfehlern der API oder Namenskonflikten erlaubt und werden über `[JsonPropertyName]` gemappt.

## Konfidenz
mittel. Das Muster ist klar, eine bewusste Festlegung dazu gibt es aber nicht.

## Gegenbeispiele
- Deutsche bzw. fehlerhafte API-Namen wurden **beibehalten**: `BankAccount.Startsaldo`, `ImportSaldo` (`Dom/Entities/BankAccount.cs:50,54`), `Invoice.PayedFromUser` (`Invoice.cs:63`). Korrigiert wird also nicht durchgängig.

---

# B-08 · Namenskonventionen · Testmethoden `Methode_Szenario_Erwartung`

## Beobachtung
Alle Testmethoden sind in PascalCase mit Unterstrichen als Trenner benannt. Das dominierende Schema ist dreiteilig (`Methode_Szenario_Erwartung`), daneben gibt es zweiteilige Namen (`Methode_Erwartung`).

## Beispiele
- `T.Server/LendingToolsTests.cs:90`: `CreateLending_WithoutRequiredField_ReturnsError_WithoutCallingApi`
- `T.Server/GetTokenToolsTests.cs:89`: `GetToken_OnException_RedactsPassword`
- `T.Infra:1072`: `DeleteBillingAccount_SendsDeleteToExpectedPath`
- Zählung: 425 Testmethoden, 425 mit Unterstrich. 277 dreiteilig, 148 zweiteilig.

## Häufigkeit
sehr häufig

## Vermutete Regel
Testmethoden heißen `Methode_Szenario_Erwartung`. Das Szenario darf entfallen, wenn es trivial ist.

## Konfidenz
hoch für die Unterstrich-Trennung. **mittel** für die Dreiteilung.

## Gegenbeispiele
- Zweiteilige Namen wie `JsonPropertyNames_AreCorrect` in ca. 25 Domain-Testdateien, `T.Server/DosbSportToolsTests.cs:68`
- `T.Infra:48`: `GetMembers_*` testet `ListMembersAsync`, der Name weicht also von der getesteten Methode ab.
- Testklassen-Suffix in Domain uneinheitlich: `*EntityTests` vs. `InvoiceItemTests`, `InvoiceChargesTests`, `InvoiceRealResponseTests`

---

# B-09 · Klassendesign · `*Fields`-Klassen als zentrale Quelle für API-Schlüssel

## Beobachtung
Jede Ressource hat eine `internal static class XFields` in `Dom/ValueObjects/` mit `internal const string`-Konstanten. Sie enthält sowohl Antwortfelder als auch Filter-Schlüssel (`IdIn = "id__in"`, `…__gte`, `…__isnull`, `Ordering`, `Search`). Die Konstanten werden an drei Stellen verwendet:
- `[JsonPropertyName]` in den Entities
- Query-Builder (`FieldQuery`, Filter)
- PATCH-Dictionaries der Tools (B-27)

## Beispiele
- `Dom/ValueObjects/BankAccountFields.cs:49` `IdIn`
- `Dom/ValueObjects/BillingAccountFields.cs:49` `AccountingPlanIsNull`
- `Dom/ValueObjects/CalendarFields.cs:30` `NameNot`
- Verwendung: `Tools/InventoryObjectTools.cs:141-150` (PATCH-Schlüssel), `Infra/LendingQuery.cs:77` (`FieldQuery`)
- Zählung: 28 Fields-Klassen, 24 davon `internal`

## Häufigkeit
sehr häufig

## Vermutete Regel
API-Feld- und Filternamen stehen nie als String-Literal im Code, sondern als Konstante in der `*Fields`-Klasse der Ressource.

## Konfidenz
hoch

## Gegenbeispiele
- 4 Klassen sind `public` statt `internal` (`ContactDetailsFields`, `EventFields`, `InvoiceFields`, `MemberFields`), genau die Legacy-Dateien mit Block-Namespace.
- Literale statt Konstanten:
  - `Infra/MemberQuery.cs:81,84,87`, `Infra/ContactDetailsQuery.cs:92-101`
  - `Dom/Entities/MemberGroup.cs:15` (`"id"`), `Dom/ValueObjects/InvoiceCharges.cs:9,12,15`
- Fehlende Filterkonstanten `ordering`/`search` bei AccountingPlan, ContactDetails, GetToken, Invoice, Location, Member und Organization
- `AccountingPlanFields`, `OrganizationFields` und `LocationFields` sind Stubs mit 1–2 Konstanten.
- Die Klassen liegen in `ValueObjects/`, sind aber keine Value Objects im DDD-Sinn. Derselbe Ordner enthält außerdem DTOs (`FeatureRequestAuthor`, `InvoiceCharges`).

---

# B-10 · Klassendesign · Entities als veränderliche, durchgehend nullable POCOs mit `IHasId`

## Beobachtung
Entities sind `public class X : IHasId` mit `{ get; set; }`-Properties. Fast alle Werttypen sind nullable (`int?`, `decimal?`, `DateTime?`), denn die API liefert je nach Feldauswahl nur Teilmengen. Entities enthalten keine Logik außer abgeleiteten `[JsonIgnore]`-Properties.

## Beispiele
- `Dom/Entities/Member.cs`, `Lending.cs`, `InventoryObject.cs`
- Zählung: 28/29 Entities implementieren `IHasId`. `{ get; set; }` 344×. `string?` 175× vs. `string` 7×. `int` 0× nicht-nullable (alle 39 nullable). `decimal` alle 16 nullable.
- `Dom/Entities/Invoice.cs:57,66,72`: abgeleitete `[JsonIgnore] long? XId => UrlReference.ExtractId(…)`

## Häufigkeit
sehr häufig

## Vermutete Regel
Entities sind reine Datenträger. Jede Property, die bei eingeschränkter Feldauswahl fehlen kann, ist nullable. Entities implementieren `IHasId`.

## Konfidenz
hoch

## Gegenbeispiele
- `Dom/Entities/ContactDetails.cs:13-213`: 49× `{ get; init; }`, nur `Id` ist `set`.
- `Dom/Entities/GetTokenResult.cs:10`: kein `IHasId`, als Antwort-DTO aber fachlich begründet
- Nicht-nullable mit Default `string.Empty`: `Member.cs:72`, `Event.cs:22`
- `Dom/Entities/Member.cs:12`, `ContactDetails.cs:8`: `[JsonSourceGenerationOptions(…)]` an POCOs. Das Attribut wirkt nur an einem `JsonSerializerContext` und bleibt hier wirkungslos.
- Attribut und Property auf derselben Zeile in `Invoice.cs`, `Booking.cs`, `Calendar.cs`, sonst auf getrennten Zeilen

---

# B-11 · Klassendesign · Tool-Klassen: `public sealed`, Primärkonstruktor, eine Klasse pro Ressource

## Beobachtung
Jede MCP-Tool-Klasse ist `[McpServerToolType] public sealed class {Ressource}Tools(IEasyVereinApiClient client)`. Es sind Instanzklassen (nicht `static`), die Abhängigkeiten per Primärkonstruktor erhalten.

## Beispiele
- `Tools/InventoryObjectTools.cs:15`, `Tools/LendingTools.cs:15`, `Tools/ForumTools.cs`
- Zählung: 25/25 `sealed`, 25/25 Instanzklassen, 22/25 injizieren nur den Client

## Häufigkeit
sehr häufig

## Vermutete Regel
Tool-Klassen sind `public sealed`, nicht statisch, und erhalten ihre Abhängigkeiten per Primärkonstruktor. Pro API-Ressource gibt es genau eine Klasse `{Ressource}Tools`.

## Konfidenz
hoch

## Gegenbeispiele
- `Tools/BookingTools.cs:15`, `InvoiceTools.cs:15`, `InvoiceItemTools.cs:15` injizieren zusätzlich `EasyVereinConfiguration`, um Ressourcen-URLs zu bauen. Das ist fachlich begründet, kein Verstoß.

---

# B-12 · Sichtbarkeiten · Implementierungsdetails `internal`, `sealed` für Infrastruktur-Typen

## Beobachtung
Nur was über Projektgrenzen hinweg als API gebraucht wird, ist `public`: Entities, Interfaces, Converter, Client und Tool-Klassen. Query-Builder, Fields-Klassen und das Paginierungs-DTO sind `internal`. `sealed` wird für Converter, Value Objects und Tool-Klassen verwendet, nicht aber für Entities und den Client.

## Beispiele
- `Infra/*Query.cs`: 24/24 `internal class`
- `Client:2171`: `internal class ApiListResponse<T>`
- `Dom/ValueObjects/*Fields.cs`: 24/28 `internal static`
- `Dom/Converters/*.cs`: 6/6 `public sealed` (z. B. `FlexibleIdConverter.cs:14`)
- `Dom/ValueObjects/ApiVersion.cs`: `public sealed`

## Häufigkeit
sehr häufig

## Vermutete Regel
Typen sind standardmäßig `internal`, `public` nur bei projektübergreifender Nutzung. Hilfs- und Infrastrukturklassen ohne Vererbungsabsicht sind `sealed`.

## Konfidenz
mittel. Für `internal` ist das Muster klar, für `sealed` fehlt eine erkennbare Linie (Entities und Client sind nicht sealed, ohne dass Vererbung genutzt wird).

## Gegenbeispiele
- 4 `public` Fields-Klassen (B-09)
- `public` Properties in `internal` Query-Klassen: `Infra/BookingQuery.cs:14-26`, `ContactDetailsQuery.cs:13-28`, `InvoiceQuery.cs:18,23`, `MemberQuery.cs:18,23`. `MemberQuery.cs:13` (internal) und `:18` (public) mischen es sogar in einer Datei.
- `InternalsVisibleTo` öffnet Domain-Interna für Infrastructure und Server (B-01).

---

# B-13 · Dependency Injection · Typed HttpClient, zentrale Registrierung, Abhängigkeit nur über Interface

## Beobachtung
Die Registrierung erfolgt ausschließlich in `Program.cs`:
- Der HttpClient wird als Typed Client `AddHttpClient<IEasyVereinApiClient, EasyVereinApiClient>()` registriert.
- Die Konfiguration ist ein Singleton, das über eine Factory mit Logger aufgelöst wird.
- Die Tools werden per `.WithTools<T>()` eingebunden.

Die Tools hängen nur vom Interface `IEasyVereinApiClient` ab, nie von der konkreten Klasse. `new HttpClient` kommt nicht vor.

## Beispiele
- `Program.cs:35-40` (Konfigurations-Singleton), `:43` (`AddHttpClient`), `:56-80` (`WithTools`)
- `Tools/*.cs`: 25/25 injizieren `IEasyVereinApiClient`
- `T.Server/*`: `new Mock<IEasyVereinApiClient>()`. Die Austauschbarkeit wird in allen Server-Tests genutzt.

## Häufigkeit
sehr häufig (für Interface-Injection). Einzelfall für die Composition Root, die es naturgemäß nur einmal gibt.

## Vermutete Regel
Dienste werden über ihr Interface injiziert. Die HTTP-Kommunikation läuft über den per `IHttpClientFactory` verwalteten Typed Client, nie über `new HttpClient()`.

## Konfidenz
hoch

## Gegenbeispiele
- `Client:43-44`: Der Konstruktor verändert `DefaultRequestHeaders` des injizierten `HttpClient` (`Clear()` + `Authorization`). Das funktioniert beim Typed Client, wäre bei geteilten Instanzen aber riskant.
- `Program.cs:52`: Die Server-Version ist hart als `"1.0.0"` codiert, obwohl `Directory.Build.props` eine `VersionPrefix` definiert.

---

# B-14 · Interfaces · Einheitliche Signaturform im Client-Interface

## Beobachtung
`IEasyVereinApiClient` hat pro Ressource dieselbe Signaturform:

| Methode | Rückgabe | Parameter |
|---|---|---|
| `List…Async` | `Task<IReadOnlyList<X>>` | optionale Filter `= null` |
| `Get…Async` | `Task<X?>` (null bei 404) | `long id` |
| `Create…Async` | `Task<X>` | Entity |
| `Update…Async` | `Task<X>` | `long id, object patchData` |
| `Delete…Async` | `Task` | `long id` |

`CancellationToken ct = default` steht immer als letzter Parameter.

## Beispiele
- `Dom/Interfaces/IEasyVereinApiClient.cs:9-58` (Announcement, BankAccount)
- Zählung: 117/117 mit `CancellationToken ct = default`. Alle 22 `Update…Async` nehmen `object patchData`.

## Häufigkeit
sehr häufig

## Vermutete Regel
Neue Client-Methoden folgen der Signaturform ihrer Operationsart. Abweichungen (Rohstring-Rückgabe, lose Parameter statt Entity) brauchen eine Begründung im XML-Kommentar.

## Konfidenz
hoch

## Gegenbeispiele
- `:935` `UpdateEventAsync` → `Task<string>` (Roh-JSON). Laut XML-Doku soll ein Event zurückkommen (`Client:1983-1998`).
- `:527` `VoteFeatureRequestAsync` → `Task<string>` per GET. Das ist dokumentiert als API-Vorgabe.
- `:766` `CreateMemberAsync(string emailOrUserName, ContactDetails, ct)`: lose Parameter
- `:906` `ListMembersAsync(…)`: Filter ohne Default-Werte
- `:870` `ListBookingsAsync`: mischt `= null` und `= default`
- `:885` Parametername `short_`, dagegen `@short` in `Client:1409`
- `:753` `CreateEventAsync(Event ev)`: abgekürzter Parametername

---

# B-15 · SOLID · Ein breites Client-Interface und eine monolithische Implementierung

## Beobachtung
Es gibt genau ein Interface (`IEasyVereinApiClient`, 117 Methoden, 952 Zeilen) und genau eine Implementierung (`EasyVereinApiClient`, 2 192 Zeilen, keine `partial`-Aufteilung). Jede Tool-Klasse hängt vom gesamten Interface ab, benutzt aber nur 2–6 Methoden. Die Querschnittslogik (Fehlerbehandlung, Paginierung, URL-Bau) ist zentral und nicht dupliziert.

## Beispiele
- `Dom/Interfaces/IEasyVereinApiClient.cs` (952 Zeilen)
- `Client` (2 192 Zeilen), Abschnitts-Banner pro Ressource (`Client:61-1556`)
- Zentrale Helfer: `SendWithErrorHandling` (`Client:2144`), `HandleListResponseWithPagination` (`:2099`), `BuildUrl` (`:2064`)

## Häufigkeit
Einzelfall (strukturell), aber zentral

## Vermutete Regel
Keine Regel ableitbar. Das Design widerspricht dem Interface Segregation Principle, ist aber offensichtlich bewusst einfach gehalten und im Test gut mockbar. **Offene Designfrage** für die Regelbildung: Soll bei weiterem Wachstum (53 Ressourcen laut API) aufgeteilt werden?

## Konfidenz
niedrig (als Regel), hoch (als Befund)

## Gegenbeispiele
Keine. Es gibt keine Aufteilung nach Ressourcen.

---

# B-16 · Nullable · `<Nullable>enable</Nullable>` überall, kein `!`, Null-Vergleich mit `==`/`!=`

## Beobachtung
Alle acht Projekte aktivieren Nullable Reference Types. Der Null-forgiving-Operator `!` kommt in Infrastructure nicht vor und in Tools nur nach einer vorherigen `HasValue`-Prüfung. Null wird mit `== null`/`!= null` geprüft, nicht mit `is null`.

## Beispiele
- Alle 8 `.csproj`: `<Nullable>enable</Nullable>`
- `Infra/LendingQuery.cs:126`, `Infra/InvoiceQuery.cs:92`, `Client:2114`: `!= null`
- `Tools/InventoryObjectTools.cs:141-150`: `if (HasValue(s)) patch[…] = s!;`, also `!` nach expliziter Prüfung
- Zählung Infrastructure: `!` 0×, `is null`/`is not null`/`??=` 0×

## Häufigkeit
sehr häufig

## Vermutete Regel
Nullable-Warnungen werden nicht mit `!` unterdrückt. Das ist nur direkt nach einer Prüfung zulässig, die der Compiler nicht erkennt (`HasValue`). Nullbarkeit wird im Typ ausgedrückt (`Task<X?>`, `string?`).

## Konfidenz
hoch für Nullable und `!`. **niedrig** für den Stil `==` vs. `is`, weil es dazu nur Gewohnheit, keine erkennbare Absicht gibt.

## Gegenbeispiele
- `Infra/MemberQuery.cs:86`: `Search!= null` (Formatierung)

---

# B-17 · Nullable · Keine Argumentprüfung in Infrastructure und Domain

## Beobachtung
Öffentliche Methoden prüfen ihre Argumente nicht: `ArgumentNullException.ThrowIfNull` und `ArgumentException.ThrowIf…` kommen nirgends vor. Validiert wird stattdessen an der Systemgrenze, in den Tools (B-24).

## Beispiele
- `Client`: 0 Guards. `patchData.GetType()` würde bei `null` eine `NullReferenceException` werfen (z. B. `Client:123`).
- `Dom/Helpers/UrlReference.cs`: null-tolerant, liefert `null` statt zu werfen
- Einzige validierende Domain-Klasse: `Dom/ValueObjects/ApiVersion.cs:39,46` (`ArgumentException` mit `nameof`)

## Häufigkeit
sehr häufig (als Abwesenheit)

## Vermutete Regel
Keine eindeutige Regel. Möglich: Validierung nur an der Systemgrenze (MCP-Tool), innere Schichten vertrauen ihren Aufrufern.

## Konfidenz
niedrig. Es ist nicht erkennbar, ob das bewusst so ist.

## Gegenbeispiele
- `ApiVersion.Create` validiert (s. o.).

---

# B-18 · Exceptions · Tools werfen nie, sondern liefern `"ERROR: …"`-Strings

## Beobachtung
Jede Tool-Methode kapselt ihren Rumpf in `try { … } catch (Exception ex)` und gibt einen einheitlichen Fehlerstring zurück:

```
ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}
```

Exceptions erreichen den MCP-Client nicht. `McpException` wird nicht verwendet.

## Beispiele
- `Tools/InventoryObjectTools.cs:44-47`, `Tools/LendingTools.cs:62-65`, `Tools/DosbSportTools.cs:33`
- Zählung: 113 von 118 Tool-Methoden mit `catch (Exception …)`, davon 109 mit wörtlich identischem Template (Abweichung: Variablenname `e` in `MemberTools`)

## Häufigkeit
sehr häufig

## Vermutete Regel
Jede Tool-Methode fängt alle Exceptions und gibt eine lesbare `ERROR:`-Meldung mit Typ, Nachricht und innerer Nachricht zurück. Ungefangene Exceptions an der MCP-Grenze sind ein Befund.

## Konfidenz
hoch

## Gegenbeispiele
- Ohne `try/catch`: `Tools/ContactDetailsTools.cs:45,62,82` (Get, Create, Delete), `Tools/InvoiceTools.cs:43,282` (GetInvoice, DeleteInvoice)
- `Tools/MemberTools.cs:93,147,169`: Variable `e` statt `ex`
- `Tools/GetTokenTools.cs:34-37`: Vorprüfung außerhalb von `try`. Das ist unkritisch, weil sie nicht wirft.

---

# B-19 · Exceptions · Zentrale HTTP-Fehlerbehandlung im Client

## Beobachtung
Alle HTTP-Aufrufe laufen über `SendWithErrorHandling`. Erfolg wird mit dem eigenen `EnsureSuccessOrThrowAsync` geprüft:
- 401 und 403 → `UnauthorizedAccessException`
- sonst → `HttpRequestException` mit dem Response-Body in der Nachricht

Netzwerk- und Timeout-Fehler werden mit **Exception-Filtern** (`when`) gezielt gefangen und als `InvalidOperationException` mit erhaltener `InnerException` neu geworfen. `EnsureSuccessStatusCode`, leere und breite `catch`-Blöcke sowie `throw ex` kommen im Client nicht vor.

## Beispiele
- `Client:2074-2088`: `EnsureSuccessOrThrowAsync`
- `Client:2144`: `SendWithErrorHandling`, umschließt 95/95 HTTP-Aufrufe
- `Client:2151`: `catch (HttpRequestException ex) when (ex.InnerException is SocketException)`
- `Client:2158`: `catch (TaskCanceledException) when (!ct.IsCancellationRequested)`, unterscheidet also Timeout von Abbruch

## Häufigkeit
sehr häufig (95/95 Aufrufe)

## Vermutete Regel
HTTP-Aufrufe gehen immer über den zentralen Fehlerbehandlungs-Helfer. Gefangen werden nur konkrete Exception-Typen mit Filter. Beim Neuwerfen bleibt die `InnerException` erhalten. Ein Abbruch durch den Aufrufer (`ct`) wird nicht in einen Fehler umgedeutet.

## Konfidenz
hoch

## Gegenbeispiele
- Keine im Client.
- Hinweis: Der Response-Body in der Fehlermeldung (`Client:2086-2088`) wird über B-18 ungefiltert an den MCP-Client weitergereicht.

---

# B-20 · Exceptions · Get bei 404 → `null` → „not found"-Meldung

## Beobachtung
Einzelabfragen behandeln HTTP 404 nicht als Fehler, sondern geben `null` zurück. Das Tool übersetzt `null` in `"{Ressource} with ID {id} not found."`. Update, Delete und List werfen bei 404.

## Beispiele
- `Client:94`, `:163`, `:247`: `if (response.StatusCode == HttpStatusCode.NotFound) return null;`
- Zählung: 24/24 Get-Methoden
- Tools: `x != null ? JsonSerializer.Serialize(…) : $"… with ID {id} not found."` (z. B. `Tools/ContactDetailsTools.cs:48-50`)

## Häufigkeit
sehr häufig

## Vermutete Regel
„Nicht gefunden" ist bei Get kein Fehler: Das Interface liefert `Task<X?>`, und das Tool gibt eine sprechende Meldung zurück.

## Konfidenz
hoch

## Gegenbeispiele
- `Client:1472`: `System.Net.HttpStatusCode` voll qualifiziert, `Client:1739`, `:1830`: zweizeilige Prüfung (nur Formatierung)

---

# B-21 · Exceptions · Converter-Fehlerverhalten uneinheitlich

## Beobachtung
Die JSON-Converter reagieren unterschiedlich auf ungültige Eingaben: Manche werfen `JsonException`, andere liefern `null`, die Datums-Converter werfen `FormatException`.

## Beispiele
- `Dom/Converters/FlexibleDecimalConverter.cs:27`, `FlexibleReferenceConverter.cs:30,33`, `FlexibleReferenceListConverter.cs:23,38,45` → `JsonException`
- `Dom/Converters/FlexibleIdConverter.cs:31` → `null` (tolerant)
- `Dom/Converters/FlexibleDateTimeConverter.cs:23`, `DateOnlyJsonConverter.cs:26` → `DateTime.Parse` wirft `FormatException`

## Häufigkeit
gelegentlich

## Vermutete Regel
Keine ableitbar.

## Konfidenz
niedrig

## Gegenbeispiele
Siehe Beispiele. Das Verhalten ist in sich widersprüchlich.

---

# B-22 · Sprache · Code-Doku Englisch, Laufzeitmeldungen gemischt

## Beobachtung
Die Sprache ist je Kategorie unterschiedlich:

| Kategorie | Sprache | Fundstellen |
|---|---|---|
| XML-Dokumentation, `[Description]`-Texte | Englisch | durchgängig (B-37) |
| Inline-Kommentare | überwiegend Deutsch | `Client:42` „Authentifizierung (FR-002)", `App/Configuration/EasyVereinConfiguration.cs:67,88` |
| Exception- und Log-Meldungen in Domain, Application, Infrastructure | Deutsch | `ApiVersion.cs:39,47`, `EasyVereinConfiguration.cs:56,116`, `Client:2135` „Leere API-Antwort." |
| Tool-Rückgaben | Englisch | „… not found.", „has been deleted" |
| CI-Schrittnamen | Deutsch | `.github/workflows/build.yml` |

## Häufigkeit
häufig

## Vermutete Regel
XML-Doku und Tool-Beschreibungen sind Englisch (siehe `CLAUDE.md` „Code-Konventionen"). Für Kommentare und Meldungen gibt es keine feste Regel. `CLAUDE.md` erlaubt „Deutsch oder Englisch nach Kontext".

## Konfidenz
hoch für Englisch in XML-Doku und `[Description]`. **niedrig** für alle anderen Kategorien.

## Gegenbeispiele
- Deutsche Begriffe in englischen Beschreibungen: `Tools/BookingTools.cs:125`, `InvoiceItemTools.cs:68-69`, `InvoiceTools.cs:203`
- Deutscher Laufzeittitel „Rechnung zur Buchung …" (`Tools/InvoiceTools.cs:224`)
- Deutsche XML-Summaries in Testhilfen (`T.Infra:4143,4168,4198`)

---

# B-23 · Logging · Kein Logging auf stdout; Geheimnisse werden nie ausgegeben

## Beobachtung
`Program.cs` entfernt alle Logging-Provider mit dem Kommentar, dass stdout für das MCP-Protokoll reserviert ist. Infrastructure und Tools loggen nicht. API-Key, Passwort und Token erscheinen weder in Logs noch in Tool-Ausgaben. `get_token` maskiert das Token, entfernt das Passwort aus Fehlermeldungen und kennzeichnet die Beschreibung mit `SENSITIVE:`. Beides ist durch Tests abgesichert.

## Beispiele
- `Program.cs:22`: `builder.Logging.ClearProviders();`
- `Tools/GetTokenTools.cs:24-27` (`SENSITIVE:`), `:45` (Maskierung), `:56` (`message.Replace(password, "***")`), `:61-64` (`Mask`)
- `T.Server/GetTokenToolsTests.cs:15-47,89-102`: Tests für Maskierung und Schwärzung
- Infrastructure: 0 `ILogger`-Verwendungen

## Häufigkeit
Einzelfall (Composition Root) bzw. gelegentlich (Maskierung)

## Vermutete Regel
Nichts darf auf stdout schreiben, außer dem MCP-Transport. Zugangsdaten und Tokens werden nie im Klartext zurückgegeben oder geloggt, und neue Tools mit sensiblen Daten brauchen Tests dafür.

## Konfidenz
mittel. Die Absicht ist eindeutig dokumentiert, es gibt aber nur ein Tool mit sensiblen Daten.

## Gegenbeispiele
- `App/Configuration/EasyVereinConfiguration.cs:115`: `LogWarning` bei fehlender Konfiguration. Weil in `Program.cs:22` alle Provider entfernt werden, **geht diese Warnung verloren**. Ein stderr-Provider fehlt.
- `Client:2086-2088`: Response-Bodies stehen in Fehlermeldungen (B-19). Ob darin sensible Daten stehen können, ist nicht geprüft.

---

# B-24 · Validierung · Pflichtfelder und Formate vor dem API-Aufruf prüfen (neuere Tools)

## Beobachtung
Neuere Tools prüfen Pflichtfelder, Längen, Wertebereiche und Formate **vor** dem API-Aufruf und geben dann `"ERROR: …"` zurück. Kommentare verweisen auf das live verifizierte API-Verhalten (HTTP 400). Tests belegen, dass die API in diesem Fall nicht aufgerufen wird.

## Beispiele
- `Tools/LendingTools.cs:105-114`: 4 Pflichtfelder, Kommentar zum live verifizierten HTTP 400. `:195-212`: gemeinsamer `Validate`-Helfer. `:218`: `DateTime.TryParseExact("yyyy-MM-dd")`. `:224`: `TimeSpan.TryParseExact(["hh\\:mm","hh\\:mm\\:ss"])`
- `Tools/InventoryObjectTools.cs:85-86` (`pieces` Pflicht), `:180-189` (`ValidateText`, max. 500)
- `Tools/CustomTaxRateTools.cs:78,101` (Bereichsprüfung), `Tools/ForumTools.cs:82-84,119`, `Tools/InventoryObjectGroupTools.cs:79-83,147-151`
- Tests: `T.Server/LendingToolsTests.cs:90-102` (`Times.Never`)
- Zählung: 9 von 25 Tool-Klassen

## Häufigkeit
häufig

## Vermutete Regel
Pflichtfelder und Formate, die die API mit 400 ablehnen würde, werden im Tool vorab geprüft. Bei ungültiger Eingabe gibt das Tool eine `ERROR:`-Meldung zurück, **ohne** die API aufzurufen, und ein Test belegt das.

## Konfidenz
mittel. In neuerem Code ist das Muster klar, älterer Code verhält sich gegenteilig.

## Gegenbeispiele
- Ungültige Eingaben werden **still verworfen**, statt einen Fehler zu melden:
  - `Tools/MemberTools.cs:133-141`: `DateTime.TryParse`/`int.TryParse` ohne Kultur, bei Fehlschlag wird das Feld einfach weggelassen
  - `Tools/AnnouncementTools.cs:97-98,152-153`, `BankAccountTools.cs:93,139`, `EventTools.cs:144-146`, `CalendarTools.cs:102`
- `Tools/EventTools.cs:142-143`: Start und Ende ungeprüft durchgereicht
- Datumsparsing uneinheitlich: `DateTimeOffset.TryParse(…, InvariantCulture)` (`InventoryObjectTools.cs:192`), `TryParseExact` (`LendingTools.cs:218`) und kulturabhängiges `DateTime.TryParse` (Member, Announcement, BankAccount)
- Typisierung: Ältere Tools nehmen `bool`/Zahlen als `string?` (`EventTools.cs:131-133`, `MemberTools.cs:124-127`), neuere typisieren korrekt (`bool?`, `decimal?`).

---

# B-25 · async/await · Durchgängig asynchron, `CancellationToken ct` immer durchgereicht

## Beobachtung
Die gesamte I/O-Kette ist asynchron. `CancellationToken ct` ist in Interface und Client der letzte Parameter (`= default`), in Tools ein Pflichtparameter. Der Token wird an jeden `HttpClient`- und `ReadAs…`-Aufruf weitergegeben. Synchrones Warten und `async void` gibt es nicht.

## Beispiele
- Interface: 117/117 mit `CancellationToken ct = default`
- Tools: 118/118 mit `CancellationToken ct`
- Client: 95/95 HTTP-Aufrufe erhalten `ct` (auch mehrzeilig, `Client:1392-1393`)
- Suche nach `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, `async void` in `src` und `tests`: 0 Treffer
- Tests: 284 `async Task`-Tests, 0 synchrone Wartevorgänge

## Häufigkeit
sehr häufig

## Vermutete Regel
Jede I/O-Methode ist `async Task`, nimmt `CancellationToken ct` als letzten Parameter und reicht ihn weiter. Sync-over-Async ist ein Befund.

## Konfidenz
hoch

## Gegenbeispiele
- `ConfigureAwait(false)`: 0 von ca. 220 `await`. Das ist konsistent, als Anwendungscode vermutlich bewusst so.
- Abbruchverhalten wird nie getestet (0 `CancellationTokenSource`/`OperationCanceledException` in Tests).
- Private Helfer ohne `Async`-Suffix (B-04)

---

# B-26 · IDisposable · `HttpResponseMessage` und `StringContent` werden nicht freigegeben

## Beobachtung
In Infrastructure gibt es kein `using`. Keine der 95 `HttpResponseMessage`-Instanzen wird freigegeben, ebenso wenig die 46 `StringContent`-Instanzen (24 aus `BuildJsonContent`, 22 inline in Updates). Test-Fakes implementieren ebenfalls kein Dispose.

## Beispiele
- `Client`: 0× `using`
- `Client:55` `BuildJsonContent`, `Client:123`, `:207`, `:1945`: inline `StringContent`

## Häufigkeit
sehr häufig (als Abwesenheit)

## Vermutete Regel
Keine Konvention. Die Auslassung wirkt **konsistent, aber unbeabsichtigt**. Für die Regelbildung kommt nur eine allgemeine C#-Empfehlung infrage, keine aus dem Repository abgeleitete Regel.

## Konfidenz
hoch (als Befund), keine (als Regel)

## Gegenbeispiele
Keine. Es gibt keine einzige freigegebene Ressource.

---

# B-27 · JSON · PATCH-Payload als `Dictionary<string, object>` mit Fields-Schlüsseln, nur gesetzte Werte

## Beobachtung
Update-Tools bauen ein `Dictionary<string, object>` und tragen nur Parameter ein, die tatsächlich gesetzt sind. Als Schlüssel dienen `*Fields`-Konstanten. Der Helfer `HasValue` behandelt `null`, `""` **und den Literalstring `"null"`**, den LLM-Clients oft senden, als „nicht gesetzt". Tests prüfen Typ, Schlüsselanzahl und das Ignorieren von `"null"`.

## Beispiele
- `Tools/InventoryObjectTools.cs:141-150`, `Tools/LendingTools.cs:156-165`, `Tools/CustomTaxRateTools.cs:104-106`
- `Tools/CustomFilterTools.cs:103-116`: lädt bei Bedarf zusätzlich den aktuellen Stand
- Tests: `T.Server/ForumToolsTests.cs:73-91`, `T.Server/CustomFieldCollectionToolsTests.cs:16-26`
- Zählung: 21/22 Update-Tools mit Dictionary. `HasValue` in 21/25 Klassen.

## Häufigkeit
sehr häufig

## Vermutete Regel
Updates senden nur geänderte Felder als PATCH-Dictionary mit Fields-Konstanten als Schlüssel. Leere Werte und `"null"` werden nie mitgesendet. Ein Test belegt das.

## Konfidenz
hoch

## Gegenbeispiele
- `Tools/ContactDetailsTools.cs:96,101`: nimmt eine komplette `ContactDetails`-Entity statt einzelner Parameter
- `Tools/BookingTools.cs:128-135`: prüft nur `!= null`, `""` und `"null"` würden also gesendet
- `Tools/AnnouncementTools.cs:102`: `string.IsNullOrEmpty` statt `HasValue`
- `HasValue` ist 21-mal dupliziert, statt einmal zentral definiert zu sein (B-40).

---

# B-28 · JSON · System.Text.Json mit `[JsonPropertyName(Fields.X)]` und Property-Convertern

## Beobachtung
Es wird ausschließlich System.Text.Json verwendet. Jede serialisierte Property trägt `[JsonPropertyName(XFields.Konstante)]`. Abweichende API-Formate (IDs als URL, Zahlen als String, Datum ohne Zeit) werden über `[JsonConverter(typeof(…))]` **an der einzelnen Property** behandelt, nicht global.

## Beispiele
- Alle 28 Entities mit `[JsonPropertyName]` und Konstanten (Ausnahme B-09)
- Converter-Verwendungen:

  | Converter | Anzahl |
  |---|---|
  | `FlexibleIdConverter` | 10 |
  | `FlexibleDateTimeConverter` | 9 |
  | `FlexibleDecimalConverter` | 7 |
  | `FlexibleReferenceConverter<T>` | 4 |
  | `DateOnlyJsonConverter` | 2 |
  | `FlexibleReferenceListConverter<T>` | 1 |

- `Dom/Converters/FlexibleReferenceConverter.cs`: generisch mit `where T : class, IHasId, new()`, also kein Converter pro Entity
- Newtonsoft.Json: 0 Treffer

## Häufigkeit
sehr häufig

## Vermutete Regel
JSON-Mapping erfolgt ausschließlich mit System.Text.Json über `[JsonPropertyName]` mit Fields-Konstanten. Sonderformate der API werden per Property-Converter behandelt, wiederverwendbar und generisch über `IHasId`.

## Konfidenz
hoch für `[JsonPropertyName]` und System.Text.Json. **niedrig** für die Frage, wann welcher Converter nötig ist (siehe Gegenbeispiele).

## Gegenbeispiele
- `FlexibleDecimalConverter` nur an 7 von 16 `decimal?`-Properties. Es fehlt z. B. an `Booking.cs:16,90`, `Invoice.cs:21,24,27,140`, `BankAccount.cs:50,54`.
- Nur 11 von 46 Datums-Properties haben einen Converter, keine der 20 `DateTimeOffset?`-Properties.
- Drei Strategien für Fremdschlüssel:
  1. `string?` + `[JsonIgnore] long? XId => UrlReference.ExtractId(…)` (`Invoice.cs`, `Member.cs:57`)
  2. `long?` + `FlexibleIdConverter` (`Lending.cs:24,30`, `InventoryObject.cs:24,55`)
  3. eingebettetes Objekt + `FlexibleReferenceConverter<T>` (`Booking.cs:23,31,84`)
- `Booking.cs:42`: `BookingProject` als reiner `string?` ohne ID-Helfer

---

# B-29 · JSON · Serializer-Optionen werden nicht geteilt

## Beobachtung
Der Client hält **eine** `JsonSerializerOptions`-Instanz pro Client-Instanz. Die Tools erzeugen dagegen bei jedem Aufruf `new JsonSerializerOptions { WriteIndented = true }`. Eine gemeinsame, statische Options-Instanz gibt es nicht.

## Beispiele
- `Client:35-40`: `_jsonOptions` im Konstruktor (CaseInsensitive, `AllowReadingFromString`, `WhenWritingNull`)
- `Tools/InventoryObjectTools.cs:42,60,111,153`: 4× `new JsonSerializerOptions` in einer Datei
- Zählung Tools: 93× `new JsonSerializerOptions`

## Häufigkeit
sehr häufig

## Vermutete Regel
Keine Konvention. Das Muster ist häufig, aber offensichtlich durch Kopieren des Tool-Skeletts entstanden (B-40), nicht durch eine Designentscheidung.

## Konfidenz
niedrig (als Regel)

## Gegenbeispiele
- Keine gemeinsame Instanz vorhanden.

---

# B-30 · JSON/HTTP · Request-Body mit expliziter Länge (kein Chunked Transfer)

## Beobachtung
Create-Requests serialisieren den Body über `BuildJsonContent` in einen `StringContent` mit fester Länge, statt `PostAsJsonAsync` zu verwenden. Der XML-Kommentar begründet das mit HTTP 411 bei Chunked Encoding. Ein eigener Regressions-Testabschnitt prüft `Content-Length` und dass kein Chunked Encoding verwendet wird.

## Beispiele
- `Client:48-55`: `BuildJsonContent` mit Begründung
- `T.Infra:3276 ff.`: Abschnitt „HTTP transport/POST"
- `T.Infra:4207-4219`: `CapturingFakeHttpHandler` zeichnet `Content-Length` und Chunked-Flag auf

## Häufigkeit
sehr häufig (24/24 Creates)

## Vermutete Regel
Request-Bodies werden nie gestreamt bzw. chunked gesendet. `PostAsJsonAsync`/`JsonContent.Create` ohne feste Länge sind ein Befund.

## Konfidenz
hoch

## Gegenbeispiele
- Die 22 Updates nutzen `BuildJsonContent` **nicht**, sondern bauen den `StringContent` inline (`Client:123`, `:207`, `:1945`). Das Ergebnis ist gleichwertig (feste Länge), der Code aber dupliziert.

---

# B-31 · Collections · Listen als `IReadOnlyList<T>`, Paginierung zentral im Client

## Beobachtung
Listenmethoden geben `IReadOnlyList<T>` zurück. Der Client folgt dem `next`-Link der API und sammelt alle Seiten ein, bevor er zurückgibt. Aufrufer sehen keine Seiten.

## Beispiele
- `Client:2105-2120`: Schleife über `page.Next`, Rückgabe per `List<T>.AsReadOnly()`
- Zählung: 23/23 `List…Async` → `Task<IReadOnlyList<T>>`
- `Dom/ValueObjects/ApiVersion.cs:27`: `SupportedVersions` als `IReadOnlyList<string>`
- `T.Infra:4171`: `MultiPageFakeHttpHandler` testet die Paginierung

## Häufigkeit
sehr häufig

## Vermutete Regel
Öffentliche Collection-Rückgaben sind `IReadOnlyList<T>`. Paginierung ist ein Detail des Clients und wird nicht nach außen gegeben.

## Konfidenz
hoch

## Gegenbeispiele
- `Client:2171`: `ApiListResponse<T>.Results` als öffentliche, veränderliche `List<T>` (intern)
- Entities verwenden `List<T>?` (7×) bzw. ein Array (`Calendar.cs:35`), nicht `IReadOnlyList`.
- `Client:2049`: `&limit=100` hart codiert

---

# B-32 · Collections/LINQ · Kaum LINQ; String-Aufbau per Liste + `string.Join`

## Beobachtung
LINQ wird fast nicht verwendet (Infrastructure 0×, Domain nur in `ApiVersion.cs:59,71-73`). Query-Strings entstehen, indem Teile in eine `List<string>` gesammelt und mit `string.Join("&", parts)` verbunden werden. Booleans werden als `"true"`/`"false"` formatiert.

## Beispiele
- `Infra/*Query.cs`: 24/24 `ToString()`-Override mit `string.Join("&", parts)`
- `Infra/LendingQuery.cs:98`: `ToString` mit `AddText`/`AddNumber`
- `Infra/LendingQuery.cs:121`: `? "true" : "false"` (13 Stellen)

## Häufigkeit
sehr häufig

## Vermutete Regel
Query-Strings werden über eine Teile-Liste plus `string.Join` gebaut, nicht per String-Konkatenation. Weitere LINQ-Vorgaben sind nicht ableitbar.

## Konfidenz
mittel

## Gegenbeispiele
- Der Helfer `AddText` existiert nur in 4 von 24 Query-Klassen (Forum, InventoryObject, InventoryObjectGroup, Lending). Die übrigen 20 schreiben `if (!string.IsNullOrEmpty(…)) parts.Add(…)` inline.

---

# B-33 · URL-Bau · Query-Builder pro Ressource mit `FieldQuery`, Escaping uneinheitlich

## Beobachtung
Jede Ressource hat eine `internal class {Ressource}Query` mit:
- einer Konstanten `FieldQuery = "query={…}"` aus Fields-Konstanten
- Filter-Properties
- einem `ToString()`-Override

Werte sollen mit `Uri.EscapeDataString` codiert werden, das geschieht aber nicht überall.

## Beispiele
- `Infra/LendingQuery.cs:77` (`FieldQuery`), `:98` (`ToString`)
- `Infra/AnnouncementQuery.cs:34`: minimale Variante
- `Infra/ContactDetailsQuery.cs:95-101`: `Uri.EscapeDataString`
- Zählung: `FieldQuery` 24/24, `ToString`-Override 24/24

## Häufigkeit
sehr häufig

## Vermutete Regel
Jede Ressource hat eine Query-Klasse mit `FieldQuery` und `ToString()`. Nutzerwerte werden mit `Uri.EscapeDataString` codiert.

## Konfidenz
hoch für die Struktur. **niedrig** für das Escaping (siehe Gegenbeispiele).

## Gegenbeispiele
- Nicht escaped:
  - `Ordering` in 0/24 Query-Klassen
  - `Infra/EventQuery.cs`: 10 Werte (Name, Datumswerte, `IdIn`)
  - `Infra/CalendarQuery.cs`: 9 Werte (Name, Color, …)
  - `Infra/BookingQuery.cs`: Datumswerte
- **Fehler:** `Infra/ContactDetailsQuery.cs:101` schreibt den Filter `Name` in den Schlüssel `familyName=` (per Hand verifiziert)
- `Infra/ContactDetailsQuery.cs:18,23`: Default `string.Empty` statt `null`
- `Infra/InvoiceQuery.cs:13-23`: tote Filter-Properties (`MembershipNumber`, `Search`), die Klasse wird nie instanziiert

---

# B-34 · Kultur · `CultureInfo.InvariantCulture` beim Parsen und Formatieren

## Beobachtung
Die Converter verwenden bei kulturabhängigen Parse- und Format-Aufrufen `CultureInfo.InvariantCulture`. `DateTime.Now`/`UtcNow` kommt in `src` nicht vor. `ToLower`/`ToUpper` ebenfalls nicht.

## Beispiele
- 7 Treffer `InvariantCulture` in 3 Convertern (`Dom/Converters/FlexibleDecimalConverter.cs`, `FlexibleDateTimeConverter.cs`, `DateOnlyJsonConverter.cs`)
- `Tools/InventoryObjectTools.cs:192`: `DateTimeOffset.TryParse(…, CultureInfo.InvariantCulture, …)`
- `Dom/ValueObjects/ApiVersion.cs:72`: `StringComparison.Ordinal`

## Häufigkeit
häufig

## Vermutete Regel
Parsen und Formatieren von Zahlen und Datumswerten für die API erfolgt kulturunabhängig.

## Konfidenz
mittel

## Gegenbeispiele
- `long.TryParse` ohne Kultur: `Dom/Converters/FlexibleIdConverter.cs:29`, `Dom/Helpers/UrlReference.cs:20` (für Ganzzahlen unkritisch)
- Kulturabhängiges `DateTime.TryParse`/`int.TryParse`: `Tools/MemberTools.cs:133-141`, `AnnouncementTools.cs`, `BankAccountTools.cs`
- `ApiVersion.cs:79`: `==` statt `StringComparison`

---

# B-35 · Datentypen · `DateTime?` und `DateTimeOffset?` gemischt; `DateOnlyJsonConverter` für reine Datumsfelder

## Beobachtung
Zeitpunkte werden je nach Entstehungszeit des Endpoints als `DateTime?` (Member, Invoice, Event …) oder `DateTimeOffset?` (CustomFilter, Forum, InventoryObject, Lending …) modelliert. Reine Datumsfelder der API bekommen seit US-0029 den `DateOnlyJsonConverter`, der immer `yyyy-MM-dd` schreibt.

## Beispiele
- `Dom/Entities/Member.cs`, `Invoice.cs`: `DateTime?`
- `Dom/Entities/Forum.cs`, `CustomFilter.cs`: `DateTimeOffset?`
- `Dom/Entities/Lending.cs`, `InventoryObject.cs`: **beide** Typen in einer Klasse
- `Dom/Converters/DateOnlyJsonConverter.cs` (2 Verwendungen)

## Häufigkeit
häufig

## Vermutete Regel
Keine einheitliche Regel ableitbar. Tendenz in neuerem Code: `DateTimeOffset?` für Zeitstempel, `DateOnlyJsonConverter` für API-Felder vom Typ `date`. Der Typ `DateOnly` selbst wird nicht verwendet.

## Konfidenz
niedrig

## Gegenbeispiele
- `FlexibleDateTimeConverter.cs:20` (Literal `"yyyy-MM-dd"`) und `DateOnlyJsonConverter.cs:15` (Konstante) enthalten dieselbe Lese-Logik doppelt.

---

# B-36 · Methodendesign · Einheitliches Tool-Skelett

## Beobachtung
Jede Tool-Methode folgt demselben Skelett:

```
[McpServerTool(Name = "…"), Description("…")]
public async Task<string> Xxx(<beschriebene Parameter>, CancellationToken ct)
{
    <optionale Validierung → "ERROR: …">
    try { var r = await client.XxxAsync(…, ct); return JsonSerializer.Serialize(r, …); }
    catch (Exception ex) { return $"ERROR: …"; }
}
```

Optionale Parameter sind nullable **ohne** Default-Wert. Pflichtparameter sind nicht-nullable (`long id`).

## Beispiele
- `Tools/LendingTools.cs:24-65`, `Tools/InventoryObjectTools.cs:23-47`, `Tools/CustomTaxRateTools.cs:70-73`
- Zählung: 118/118 `Task<string>`, 118/118 mit `CancellationToken ct`

## Häufigkeit
sehr häufig

## Vermutete Regel
Tool-Methoden geben `Task<string>` zurück (JSON oder Meldung) und folgen dem Skelett „Validierung → try/Client-Aufruf/Serialisierung → catch-all".

## Konfidenz
hoch

## Gegenbeispiele
- `Tools/EventTools.cs:148`: gibt das Roh-JSON aus `UpdateEventAsync` durch (B-14).
- `Tools/InvoiceTools.cs:23`: `list_invoices` ohne Filter
- Formatierung: `Tools/MemberTools.cs:28,127` (`ct` in derselben Zeile, Leerzeichen vor dem Komma)

---

# B-37 · XML-Dokumentation · Englische `/// <summary>` an **allen** Membern inkl. private

## Beobachtung
Jeder Typ und jeder Member trägt einen englischen XML-Kommentar, auch private Felder, private Konstanten, private Konstruktoren und private Helfer. Das ist in `CLAUDE.md` („Code-Konventionen") als Regel festgelegt und im Produktivcode fast lückenlos umgesetzt.

## Beispiele
- `Client:17-24`: private Felder mit `<summary>`
- `Dom/ValueObjects/ApiVersion.cs:8,11,17`: private Konstante, Feld, Konstruktor
- `Dom/Converters/DateOnlyJsonConverter.cs:14`
- Zählung:

  | Bereich | ohne XML-Doku |
  |---|---|
  | Infrastructure (398 Member) | 0 |
  | Tools (118 Methoden + 44 private Member) | 0 |
  | Domain | 9 |

## Häufigkeit
sehr häufig

## Vermutete Regel
Jeder Member, auch `private`, hat einen englischen `/// <summary>`.

## Konfidenz
hoch

## Gegenbeispiele
- Domain ohne Doku: `Entities/AccountingPlan.cs:7,9`, `Location.cs:7,9,10`, `Member.cs:185-186`, `ValueObjects/MemberFields.cs:77`, `AccountingPlanFields.cs:9,11`, `LocationFields.cs:9,11,12`
- **Tests:** Nur `Server.Tests` dokumentiert Testmethoden (93× „Verifies that …"). Domain-, Application- und Infrastructure-Tests haben keine XML-Doku. `T.Infra:11` `CreateClient` und die Fake-Handler-Felder sind undokumentiert.
- Zwei Stile: Ältere Dateien haben `<param>`/`<returns>` (Announcement, Booking, Calendar, ContactDetails, Event, Invoice, Member, z. B. `Tools/InvoiceTools.cs:51-72`), neuere nur eine einzeilige `<summary>` und verlassen sich auf `[Description]` (18 Tool-Dateien). Im Interface fehlt `<returns>` bei 30/117 Methoden.
- Kleinigkeiten: „Maps to API field ' <c>x</c>'" mit Leerzeichen in `Member.cs`, `Event.cs`, `Booking.cs`. Trailing Whitespace in `Member.cs` (31 Zeilen), `Event.cs` (20).

---

# B-38 · XML-Dokumentation · `[Description]` an jedem Tool und Parameter

## Beobachtung
Die MCP-sichtbare Beschreibung steht in `[Description]` an der Methode **und** an jedem Parameter außer `ct`. Löschende Tools enden einheitlich mit „Only authorized users are able to perform this action!". Sensible Tools tragen den Präfix `SENSITIVE:`.

## Beispiele
- `Tools/LendingTools.cs:24-48`: 22 Filterparameter, jeder mit Beschreibung
- `Tools/GetTokenTools.cs:24-32`
- Zählung: 538 von 568 Parametern (ohne `ct`) mit `[Description]`. 21/21 Delete-Tools mit dem Berechtigungshinweis.

## Häufigkeit
sehr häufig

## Vermutete Regel
Jede Tool-Methode und jeder Tool-Parameter hat eine englische `[Description]`. Destruktive Tools weisen auf Berechtigung und Löschverhalten (Papierkorb vs. endgültig) hin.

## Konfidenz
hoch für `[Description]`. **mittel** für den Hinweis auf das Löschverhalten (siehe Gegenbeispiele).

## Gegenbeispiele
- 30 Parameter ohne `[Description]`: `Tools/ContactDetailsTools.cs` (11), `Tools/InvoiceTools.cs:43,74-91,282` (19)
- Löschverhalten nur bei 4 von 21 Delete-Tools als „wastebasket" beschrieben (CustomTaxRate, InventoryObjectGroup, InventoryObject, Lending). `CustomTaxRateTools.cs:126` meldet trotzdem „has been deleted". `CustomFilterTools.cs:129` sagt „permanently", die übrigen 16 sind neutral formuliert.
- Tippfehler „Delete a invoice" (`Tools/InvoiceTools.cs:281`), „automatically))" (`Tools/MemberTools.cs:74`)

---

# B-39 · Kommentare · Anforderungs-IDs und Live-Verifikation im Kommentar

## Beobachtung
Kommentare und XML-Doku verweisen auf Anforderungs-IDs (`FR-xxx`, `NFR-xxx`). Validierungen werden mit dem **live beobachteten** API-Verhalten begründet, teils mit Datum.

## Beispiele
- `Client:13`, `:42` („Authentifizierung (FR-002)"), `:2076`
- `Dom/ValueObjects/ApiVersion.cs:4` „(FR-010 to FR-015)", `App/Configuration/EasyVereinConfiguration.cs:8,48,74`
- `Tools/LendingTools.cs:105-106`: Pflichtfelder laut live HTTP 400
- `T.Server/LendingToolsTests.cs:81-84`: „verified live on 2026-09-22"

## Häufigkeit
häufig

## Vermutete Regel
Nicht offensichtliche Entscheidungen werden im Kommentar mit der Anforderung (FR-ID) oder dem beobachteten API-Verhalten begründet.

## Konfidenz
mittel

## Gegenbeispiele
- Viele Validierungen in älteren Tools sind nicht begründet (B-24).

---

# B-40 · SOLID/DRY · Copy-&-Paste-Skelett pro Endpoint wird in Kauf genommen

## Beobachtung
Querschnittslogik im **Client** ist zentralisiert (B-15, B-19, B-31). In den **Tools** und **Query-Klassen** wird dagegen dieselbe Hilfslogik pro Datei dupliziert, statt sie gemeinsam zu nutzen.

## Beispiele
- `HasValue`: 21× dupliziert (`Tools/*Tools.cs`)
- `new JsonSerializerOptions { WriteIndented = true }`: 93× (B-29)
- Fehler-Template im `catch`: 113× identisch (B-18)
- `AddText`/`AddNumber` in 4 Query-Klassen dupliziert, in 20 inline (B-32)
- PATCH-`StringContent` 22× inline statt `BuildJsonContent` (B-30)

## Häufigkeit
sehr häufig

## Vermutete Regel
Keine Regel im Sinne von „so soll es sein". Die Duplikation ist ein Nebeneffekt des Musters „neue Endpoints als Kopie des Vorgängers". Für die Regelbildung relevant: Zentral gelöste Querschnittslogik im Client zeigt, **wo** das Projekt Wiederverwendung erwartet.

## Konfidenz
hoch (als Befund), niedrig (als Regel)

## Gegenbeispiele
- `Client`: `SendWithErrorHandling`, `BuildJsonContent`, `HandleListResponseWithPagination` sind vorbildlich zentralisiert.

---

# B-41 · Tests · Ein Testprojekt pro Schicht, Klasse `<Subjekt>Tests`, eine Datei pro Klasse

## Beobachtung
Jede Schicht hat ihr Testprojekt. Testklassen heißen `<Subjekt>Tests` und liegen je in einer eigenen Datei.

## Beispiele
- `tests/MCP.EasyVerein.{Domain,Application,Infrastructure,Server}.Tests`
- `T.Server/LendingToolsTests.cs`, `T.Server/GetTokenToolsTests.cs`, `T.Dom/…/LendingEntityTests.cs`
- Gesamt 461 Testfälle (`CLAUDE.md`), 425 Testmethoden

## Häufigkeit
sehr häufig

## Vermutete Regel
Neue Klassen werden in der gespiegelten Testklasse `<Subjekt>Tests` getestet.

## Konfidenz
hoch

## Gegenbeispiele
- **Abdeckungslücke:** Nur 11 von 25 Tool-Klassen haben eigene Tests. Es fehlen Announcement, BankAccount, BillingAccount, BookingProject, Booking, Calendar, ChairmanLevel, ContactDetails, ContactDetailsGroup, ContactDetailsLog, CustomField, Event, InvoiceItem und Member.
- `T.Infra` ist **eine** Datei mit 4 250 Zeilen und 211 Tests in 28 Banner-Abschnitten. Sie spiegelt die monolithische Client-Klasse (B-15) und ist ein Ausreißer gegenüber den anderen Projekten.
- Leerer Ordner `tests/MCP.EasyVerein.Domain.Tests/Entities/`

---

# B-42 · Tests · Moq für das Interface, eigene Fake-Handler für HTTP

## Beobachtung
Server-Tests mocken `IEasyVereinApiClient` mit Moq und fangen Argumente per `.Callback` ab, um den Payload zu prüfen. Dass die API **nicht** aufgerufen wird, prüfen sie mit `Times.Never` oder `MockBehavior.Strict`.

Infrastructure-Tests verwenden **kein** Moq, obwohl das Paket referenziert ist. Sie nutzen handgeschriebene `HttpMessageHandler`-Fakes, die URI, Methode, Body, `Content-Length` und Chunked-Flag aufzeichnen.

## Beispiele
- `T.Server/LendingToolsTests.cs:58-60` (Callback), `:102` (`Times.Never`)
- `T.Server/GetTokenToolsTests.cs:77`: `MockBehavior.Strict` (17× insgesamt)
- `T.Infra:4146,4171,4201`: `FakeHttpHandler`, `MultiPageFakeHttpHandler`, `CapturingFakeHttpHandler`
- `T.Infra`: `LastRequestUri` 189×, `LastRequestMethod` 37×, `LastRequestBody` 54×
- `T.Server/DosbSportToolsTests.cs:68-78`: Reflection auf `McpServerToolAttribute`, prüft also „nur lesend"

## Häufigkeit
sehr häufig

## Vermutete Regel
Tool-Tests mocken das Client-Interface und prüfen den gesendeten Payload. Client-Tests prüfen Methode, Pfad und Body des HTTP-Requests über einen Fake-Handler. Echtes HTTP kommt in Unit-Tests nie vor.

## Konfidenz
hoch

## Gegenbeispiele
- `Moq` ist in `Infrastructure.Tests.csproj` referenziert, wird dort aber nicht verwendet.

---

# B-43 · Tests · Kein AAA-Kommentar, mehrere Asserts, keine gemeinsamen Fixtures

## Beobachtung
Tests gliedern sich über Leerzeilen, nicht über `// Arrange/Act/Assert`. Pro Test gibt es meist mehrere Asserts. Setup erfolgt über statische Factory-Helfer. Konstruktoren, `IClassFixture` oder geteilter Zustand kommen nicht vor, jeder Test erzeugt seine Mocks selbst. Assertions nutzen nur `Assert.*` von xUnit.

## Beispiele
- `T.Server/LendingToolsTests.cs:69-78`: 9 Asserts in einem Test
- `T.Infra:11`: `CreateClient`, `T.Server/InvoiceToolsTests.cs:14`: `CreateConfig`
- Zählung: `// Arrange` 0×. `[Fact]` 407×, `[Theory]` 18× (54 `InlineData`). `IClassFixture`/`IAsyncLifetime`/Test-Konstruktor 0×. FluentAssertions 0×.

## Häufigkeit
sehr häufig

## Vermutete Regel
Tests sind unabhängig voneinander (kein geteilter Zustand, keine Fixtures) und nutzen statische Factory-Helfer. Mehrere Asserts pro Test sind zulässig, wenn sie ein Verhalten prüfen.

## Konfidenz
hoch

## Gegenbeispiele
Keine gefunden.

---

# B-44 · Tests · Echte API-Antworten als Fixture, sonst Inline-JSON

## Beobachtung
Für die Deserialisierung realer API-Antworten gibt es JSON-Fixtures in `Domain.Tests/Fixtures`, pro API-Version, die ins Ausgabeverzeichnis kopiert werden. Die meisten Tests arbeiten aber mit Inline-Raw-Strings oder serialisierten anonymen Objekten.

## Beispiele
- `T.Dom/Fixtures/member-v1.7.json`, `member-v2.0.json`, `invoice-real-response.json`
- `tests/MCP.EasyVerein.Domain.Tests/MCP.EasyVerein.Domain.Tests.csproj:30-37` (`CopyToOutputDirectory`)
- `T.Dom/…/MemberEntityTests.cs:137,156`, `InvoiceRealResponseTests.cs:19`
- Inline: `T.Dom/…/LendingEntityTests.cs:11-29`

## Häufigkeit
gelegentlich (Fixtures), sehr häufig (Inline)

## Vermutete Regel
Reale API-Antworten, die schon einmal Deserialisierungsfehler verursacht haben, werden als versionierte Fixture-Datei abgelegt.

## Konfidenz
mittel

## Gegenbeispiele
- Neuere Endpoints (Lending, InventoryObject) haben keine Fixture, sondern nur Inline-JSON.

---

# B-45 · Tests · Regressionstests als benannte Abschnitte

## Beobachtung
Behobene Fehler bekommen eigene, benannte Testabschnitte.

## Beispiele
- `T.Infra:3276`: Abschnitt „HTTP transport/POST" (Chunked, 411)
- `T.Infra:3341`: Abschnitt „null-omission" (`WhenWritingNull`)
- `T.Server/GetTokenToolsTests.cs:89`: `GetToken_OnException_RedactsPassword`

## Häufigkeit
gelegentlich

## Vermutete Regel
Jeder Bugfix wird durch einen Regressionstest abgesichert, der das Fehlverhalten benennt.

## Konfidenz
mittel

## Gegenbeispiele
Keine gefunden.

---

# B-46 · Tooling · Dokumentierte Qualitätsregeln werden nicht technisch erzwungen

## Beobachtung
`CLAUDE.md` fordert 70 % Coverage, CI-Prüfung vor dem Merge und Pre-Commit-Hooks. Umgesetzt ist nur ein Teil davon:
- Die CI läuft auf 3 Betriebssystemen und **sammelt** Coverage, erzwingt aber keinen Schwellwert.
- Es gibt keinen Git-Hook.
- Es gibt keine `.editorconfig` und keine Analyzer-Konfiguration.

Alle Stilkonventionen werden also nur durch Disziplin gehalten.

## Beispiele
- `.github/workflows/build.yml:25-30` (Matrix), `:48` (`XPlat Code Coverage`), `:50-57` (Upload nur ubuntu, `continue-on-error`)
- `.github/workflows/nightly.yml`: Tests + Publish für 4 RIDs, Prerelease
- Git-Hooks: `.git/hooks` enthält nur `*.sample`, kein `core.hooksPath` (verifiziert), kein `.githooks`/`.husky`
- Keine `.editorconfig` im Repository

## Häufigkeit
Einzelfall (je Artefakt)

## Vermutete Regel
Keine Regel ableitbar. **Befund:** Zwischen den in `CLAUDE.md` dokumentierten Regeln und ihrer technischen Durchsetzung besteht eine Lücke.

## Konfidenz
hoch (als Befund)

## Gegenbeispiele
- `concurrency` mit `cancel-in-progress` (`build.yml:12-14`) und die kurze Artefakt-Aufbewahrung zeigen bewusstes Kostenmanagement der CI.

---

# B-47 · Konfiguration · Priorität CLI > Umgebung > Default über `IConfiguration`

## Beobachtung
Die Konfiguration wird über `IConfiguration` aufgelöst. Die Reihenfolge der Provider bestimmt die Priorität: Kommandozeile mit Switch-Mappings vor Umgebungsvariablen vor Defaults. Eine ungültige API-Version bricht den Start bewusst ab.

## Beispiele
- `Program.cs:13` (`--help` vor dem Start), `:26-32` (`AddCommandLine` mit Switch-Mappings)
- `App/Configuration/EasyVereinConfiguration.cs:81` (`FromConfiguration`), `:108` (`Resolve`), `:68,89,133` (`ApiVersionVO.Create`, „gewollt, FR-015")
- `CLAUDE.md` Abschnitt „Konfiguration (Priorität)"

## Häufigkeit
Einzelfall (eine Konfigurationsklasse)

## Vermutete Regel
Neue Einstellungen werden über `IConfiguration` mit Env-Variable `EASYVEREIN_*` und CLI-Switch ergänzt, nicht per direktem `Environment.GetEnvironmentVariable`.

## Konfidenz
mittel. Es gibt nur eine Fundstelle, die Regel ist aber in `CLAUDE.md` dokumentiert.

## Gegenbeispiele
- `EasyVereinConfiguration.cs:51`: `FromEnvironment()` (als „Legacy" markiert) liest direkt aus der Umgebung und **wirft** bei fehlendem Key. `FromConfiguration` liefert stattdessen `string.Empty` und eine (verlorene, B-23) Warnung.

---

# Bewusst vermiedene Anti-Patterns (Übersicht)

| Anti-Pattern | Befund | Beleg |
|---|---|---|
| `new HttpClient()` | 0×, Typed Client | B-13 |
| PUT (vollständiges Ersetzen) | 0× `PutAsync`, nur PATCH | B-27, `CLAUDE.md` |
| Chunked Request-Body / `PostAsJsonAsync` | vermieden, per Test abgesichert | B-30 |
| Sync-over-Async (`.Result`, `.Wait()`), `async void` | 0× in `src` und `tests` | B-25 |
| Exceptions an der MCP-Grenze | 113/118 abgefangen | B-18 |
| Leere / breite `catch` im Client, `throw ex` | 0× | B-19 |
| Logging auf stdout | Provider entfernt | B-23 |
| Klartext-Geheimnisse in Ausgaben | maskiert und getestet | B-23 |
| Magic Strings für API-Felder | Fields-Konstanten | B-09 |
| Senden von `""`/`"null"` bei PATCH | `HasValue`, getestet | B-27 |
| API-Aufruf mit bekannt ungültiger Eingabe (neuere Tools) | Vorab-Validierung, getestet | B-24 |
| Statische Tool-Klassen / verstecktes Singleton | 0× | B-11 |
| Echtes HTTP in Unit-Tests | Fake-Handler | B-42 |
| Geteilter Testzustand | 0 Fixtures/Konstruktoren | B-43 |
| `DateTime.Now` in Domain/Application | 0× | B-34 |
| Null-forgiving `!` ohne Prüfung | 0× in Infrastructure | B-16 |

---

# Keine oder unzureichende Evidenz

| Thema | Befund |
|---|---|
| **XML-Verarbeitung** | Nicht vorhanden. Kein `System.Xml`, `XDocument` oder `XmlSerializer` in `src` oder `tests` (verifiziert). |
| **Architekturtests** | Keine (z. B. keine NetArchTest). Die Schichtgrenzen stehen nur in den Projektreferenzen (B-01). |
| **Logging-Konventionen** | Nur ein `LogWarning` in der gesamten Codebasis, und das geht verloren. Zu Log-Leveln, strukturiertem Logging oder `ILogger<T>`-Injection lässt sich nichts ableiten. |
| **Retry / Resilienz / Rate-Limits** | 0× Polly, Retry oder 429-Behandlung, kein Timeout-Setting. |
| **Abbruchverhalten** | `ct` wird überall durchgereicht, aber nirgends getestet (B-25). |
| **Records / Immutability** | Kein `record` im Code. Nur `ContactDetails` nutzt `init`. Zu Unveränderlichkeit lässt sich nichts ableiten. |
| **Custom Exceptions** | Keine eigenen Exception-Typen. Zu Exception-Hierarchien lässt sich nichts ableiten. |

---

# Übersicht

| ID | Thema | Kat. | Häufigkeit | Konfidenz |
|---|---|---|---|---|
| B-01 | 4 Schichten, Referenzen nach innen | K1 | sehr häufig | hoch |
| B-02 | Ordner = Namespace, file-scoped, 1 Typ/Datei | K1 | sehr häufig | hoch |
| B-03 | Fünf-Schritte-Muster pro Endpoint | K1 | sehr häufig | hoch |
| B-04 | `Async`-Suffix, CRUD-Verben | K1 | sehr häufig | hoch |
| B-05 | Toolnamen `verb_ressource` explizit | K1 | sehr häufig | hoch |
| B-06 | `_camelCase` / Primärkonstruktor | K2 | häufig | mittel |
| B-07 | Properties spiegeln API-Namen | K2 | sehr häufig | mittel |
| B-08 | Testnamen `Methode_Szenario_Erwartung` | K2 | sehr häufig | hoch / mittel |
| B-09 | `*Fields`-Konstanten statt Literale | K1 | sehr häufig | hoch |
| B-10 | Entities: nullable POCO + `IHasId` | K1 | sehr häufig | hoch |
| B-11 | Tools: `public sealed`, Primärkonstruktor | K1 | sehr häufig | hoch |
| B-12 | `internal` / `sealed` | K2 | sehr häufig | mittel |
| B-13 | DI über Interface, Typed HttpClient | K1 | sehr häufig | hoch |
| B-14 | Signaturform Client-Interface | K1 | sehr häufig | hoch |
| B-15 | Monolithisches Client-Interface | K3 | Einzelfall | niedrig (Regel) |
| B-16 | Nullable an, kein `!` | K1 | sehr häufig | hoch |
| B-17 | Keine Argument-Guards | K3 | sehr häufig | niedrig |
| B-18 | Tools: catch-all → `ERROR:` | K1 | sehr häufig | hoch |
| B-19 | Zentrale HTTP-Fehlerbehandlung | K1 | sehr häufig | hoch |
| B-20 | 404 → `null` → not found | K1 | sehr häufig | hoch |
| B-21 | Converter-Fehlerverhalten | K3 | gelegentlich | niedrig |
| B-22 | Sprache Doku/Meldungen | K2/K3 | häufig | hoch / niedrig |
| B-23 | Kein stdout-Logging, Secrets maskiert | K1 | Einzelfall / gelegentlich | mittel |
| B-24 | Vorab-Validierung in Tools | K2 | häufig | mittel |
| B-25 | async + `ct` durchgereicht | K1 | sehr häufig | hoch |
| B-26 | Kein Dispose von HTTP-Objekten | K3 | sehr häufig | – (Befund) |
| B-27 | PATCH-Dictionary + `HasValue` | K1 | sehr häufig | hoch |
| B-28 | STJ, `[JsonPropertyName]`, Property-Converter | K1 | sehr häufig | hoch / niedrig |
| B-29 | Serializer-Optionen nicht geteilt | K3 | sehr häufig | niedrig |
| B-30 | Request-Body mit fester Länge | K1 | sehr häufig | hoch |
| B-31 | `IReadOnlyList<T>`, zentrale Paginierung | K1 | sehr häufig | hoch |
| B-32 | Teile-Liste + `string.Join`, kaum LINQ | K2 | sehr häufig | mittel |
| B-33 | Query-Builder, Escaping uneinheitlich | K1/K3 | sehr häufig | hoch / niedrig |
| B-34 | `InvariantCulture` | K2 | häufig | mittel |
| B-35 | `DateTime?` vs. `DateTimeOffset?` | K3 | häufig | niedrig |
| B-36 | Einheitliches Tool-Skelett | K1 | sehr häufig | hoch |
| B-37 | XML-Doku an allen Membern | K1 | sehr häufig | hoch |
| B-38 | `[Description]` überall | K1 | sehr häufig | hoch / mittel |
| B-39 | FR-IDs / Live-Verifikation im Kommentar | K2 | häufig | mittel |
| B-40 | Duplikation im Tool-Skelett | K3 | sehr häufig | niedrig (Regel) |
| B-41 | Testprojekt pro Schicht, `<Subjekt>Tests` | K1 | sehr häufig | hoch |
| B-42 | Moq für Interface, Fake-Handler für HTTP | K1 | sehr häufig | hoch |
| B-43 | Unabhängige Tests, statische Helfer | K2 | sehr häufig | hoch |
| B-44 | Versionierte API-Fixtures | K2 | gelegentlich | mittel |
| B-45 | Regressionstests als Abschnitte | K2 | gelegentlich | mittel |
| B-46 | Regeln nicht technisch erzwungen | K3 | Einzelfall | hoch (Befund) |
| B-47 | Konfiguration über `IConfiguration` | K1 | Einzelfall | mittel |

---

# Auffällige Einzelbefunde (keine Regeln, aber reviewrelevant)

Diese Stellen sind beim Sammeln aufgefallen. Sie sind **potenzielle Fehler**, keine Konventionen, und wurden hier nicht behoben (Auftrag: keinen Quellcode ändern).

| Fundstelle | Befund |
|---|---|
| `Infra/ContactDetailsQuery.cs:101` | Filter `Name` wird als `familyName=` gesendet (per Hand verifiziert). |
| `App/Configuration/EasyVereinConfiguration.cs:115` + `Program.cs:22` | Konfigurationswarnung wird geloggt, aber alle Provider sind entfernt, die Warnung geht also verloren. |
| `Client:1983-1998` | `UpdateEventAsync` gibt `string` zurück, die XML-Doku sagt Event. |
| `Client:1627-1635` | `CreateMemberAsync` macht zwei POSTs ohne Kompensation bei einem Fehler im zweiten. |
| `Dom/Entities/Member.cs:12`, `ContactDetails.cs:8` | `[JsonSourceGenerationOptions]` an POCOs ist wirkungslos. |
| `Tools/CustomTaxRateTools.cs:126` | Beschreibung sagt Papierkorb, Rückgabe sagt „has been deleted". |
| `Tools/MemberTools.cs:133-141` u. a. | Ungültige Datums- oder Zahleneingaben werden still verworfen (B-24). |
| `Infra/EventQuery.cs`, `CalendarQuery.cs`, `BookingQuery.cs` | Unescapte Nutzerwerte in URLs (B-33). |
