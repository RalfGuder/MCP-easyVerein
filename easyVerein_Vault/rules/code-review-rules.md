# Code-Review-Regeln 

# Benennung

## CR-001: PascalCase für Typen, Methoden und Properties

### Regel
Klassen, Records, Interfaces, Enums, Methoden und Properties werden in PascalCase benannt. Unterstriche oder kleiner Anfangsbuchstabe sind ein Befund.

### Begründung
Eigener Abschnitt „Use consistent capitalization" (135-171) und durchgängige Verwendung in den Good-Snippets aller Kapitel. Beobachtung B-02, Konfidenz hoch.

### Positives Beispiel

  ```csharp
  bool EraseDatabase() {}
  bool RestoreDatabase() {}

  class Animal {}
  class Alpaca {}
  ```

  ```csharp
  public class Employee
  {
      public Datetime StartWorkingDate { get; set; }
      public Datetime ModificationTime { get; set; }
  }
  ```

### Negatives Beispiel

  ```csharp
  bool Restore_database() {}
  ```

  ```csharp
  class animal {}
  ```

  ```csharp
  public Datetime sWorkDate { get; set; } // what the heck is this
  public Datetime modTime { get; set; } // same here
  ```

### Review-Frage
Beginnt jeder neue Typ-, Methoden- und Property-Name mit einem Großbuchstaben, ohne Unterstriche?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Namensregeln in `.editorconfig` + IDE1006.

---

## CR-002: Konstanten in PascalCase, nicht SCREAMING_CASE

### Regel
`const`-Felder werden in PascalCase benannt (`DaysInWeek`), nicht in `UPPER_SNAKE_CASE`.

### Begründung
Explizites Bad/Good-Paar (144 → 160) und eigene Regel `constant_fields_should_be_pascal_case` in der README-`.editorconfig` (2807-2815). Beobachtung B-02, Teil Konstanten, Konfidenz **mittel**, weil zwei Good-Snippets dagegen verstoßen.

### Positives Beispiel

  ```csharp
  const int DaysInWeek = 7;
  const int DaysInMonth = 30;
  ```

  ```csharp
  const string Address = "One Infinite Loop, Cupertino 95014";
  ```

  ```csharp
  const string Address = "One Infinite Loop, Cupertino 95014";
  ```

### Negatives Beispiel

  ```csharp
  const int DAYS_IN_WEEK = 7;
  ```

  ```csharp
  const string ADMIN_ROLE = "Admin"
  ```

  ```csharp
  ACCESS_READ = 1,
  ACCESS_CREATE = 2,
  ACCESS_UPDATE = 4,
  ACCESS_DELETE = 8
  ```

### Review-Frage
Sind neue Konstanten und Enum-Member in PascalCase benannt?

### Schweregrad
CONSIDER. Die Quelle ist hier selbst inkonsistent.

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `.editorconfig`-Namensregel wie in `README.md:2807-2815` + IDE1006.

---

## CR-003: camelCase für lokale Variablen und Parameter

### Regel
Lokale Variablen und Parameter werden in camelCase benannt, jedes Folgewort beginnt groß (`workingDays`, nicht `workingdays`).

### Begründung
Eigener Abschnitt „Use Camelcase notation" (206-231) und durchgängig in allen Good-Snippets. Beobachtung B-03, Konfidenz hoch.

### Positives Beispiel

  ```csharp
  public double CalculateSalary(int workingDays, int workingHours)
  ```

  ```csharp
  var cityZipCodeWithGroupRegex = @"/^[^,\]+[,\\s]+(?<city>.+?)\s*(?<zipCode>\d{5})?$/";
  var matchesWithGroup = Regex.Match(Address, cityZipCodeWithGroupRegex);
  var cityGroup = matchesWithGroup.Groups["city"];
  var zipCodeGroup = matchesWithGroup.Groups["zipCode"];
  ```

### Negatives Beispiel

  ```csharp
  var employeephone;

  public double CalculateSalary(int workingdays, int workinghours)
  ```

  ```csharp
  var Artists = new List<string> { 'ACDC', 'Led Zeppelin', 'The Beatles' };
  ```

### Review-Frage
Sind alle neuen Parameter und lokalen Variablen in camelCase mit erkennbaren Wortgrenzen benannt?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Die Groß-/Kleinschreibung am Anfang lässt sich prüfen, fehlende Wortgrenzen (`workingdays`) nicht. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE1006.

---

## CR-004: Private Felder als `_camelCase`

### Regel
Private und interne Instanzfelder beginnen mit `_`, danach folgt camelCase (`_balance`).

### Begründung
Regel `camel_case_underscore_style` in der README-`.editorconfig` (2828-2837) und 8 Fundstellen in Snippets unterschiedlicher Kapitel. Beobachtung B-04, Konfidenz **mittel**.

### Positives Beispiel

  ```csharp
  private Dictionary<string, string> _configuration;
  ```

  ```csharp
  private double _balance = 0.0D;
  ```

  ```csharp
  private readonly UserAuth _auth = new(user);
  ```

### Negatives Beispiel

  ```csharp
  private User User;
  ```

  ```csharp
  private readonly IAdapter Adapter;
  ```

  ```csharp
  private double Length = 0;
  ```

### Review-Frage
Beginnt jedes neue private Feld mit `_` und danach einem Kleinbuchstaben?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `.editorconfig`-Namensregel wie in `README.md:2828-2837` + IDE1006.

> Statische Felder (`s_`-Präfix laut `README.md:2817-2826`) sind bewusst **nicht** Teil dieser Regel. Alle Fundstellen verwenden `_instance`.

---

## CR-005: Keine Ungarische Notation, keine Kürzel, keine Ein-Buchstaben-Namen

### Regel
Namen enthalten keine Typpräfixe (`str`, `i`, `p`, `d`), keine unaussprechbaren Abkürzungen (`ymdstr`, `modTime`) und außerhalb von Schleifenzählern und kurzen Lambdas keine Ein-Buchstaben-Namen.

### Begründung
Fünf eigenständige Abschnitte mit Bad/Good-Paaren: 48-62, 89-128, 177-200, 395-434, 505-518. Beobachtung B-05, Konfidenz hoch. Kurze Lambda-Parameter (`d`, `s`) kommen in Good-Snippets vor (333, 1166) und sind deshalb ausgenommen.

### Positives Beispiel
- `README.md:105-107` – `counter`, `fullName`, `modifiedDate`

  *Good – README.md:105-107*

  ```csharp
  int counter;
  string fullName;
  DateTime modifiedDate;
  ```

- `README.md:422-432` – `locations`, `location`

  *Good – README.md:422-432*

  ```csharp
  var locations = ["Austin", "New York", "San Francisco"];

  foreach (var location in locations)
  {
      DoStuff();
      DoSomeOtherStuff();

      // ...
      // ...
      // ...
      Dispatch(location);
  ```

- `README.md:517` – `currentDate`

  *Good – README.md:517*

  ```csharp
  var currentDate = DateTime.UtcNow.ToString("MMMM dd, yyyy");
  ```

### Negatives Beispiel
- `README.md:97-99` – `iCounter`, `strFullName`, `dModifiedDate` (Bad)

  *Bad – README.md:97-99*

  ```csharp
  int iCounter;
  string strFullName;
  DateTime dModifiedDate;
  ```

- `README.md:115` – `pDay`, `pAmount` (Bad)

  *Bad – README.md:115*

  ```csharp
  public bool IsShopOpen(string pDay, int pAmount)
  ```

- `README.md:403-407` – `l`, `li` (Bad)

  *Bad – README.md:403-407*

  ```csharp
  var l = new[] { "Austin", "New York", "San Francisco" };

  for (var i = 0; i < l.Count(); i++)
  {
      var li = l[i];
  ```

### Review-Frage
Versteht man jeden neuen Namen ohne Kontext beim Vorlesen, und enthält er keinen Hinweis auf seinen Datentyp?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Typpräfixe und Ein-Buchstaben-Namen lassen sich per Muster erkennen, die Aussagekraft eines Namens nicht.

---

## CR-006: Methodennamen beschreiben die Wirkung mit einem Verb

### Regel
Methodennamen beginnen mit einem Verb und benennen die konkrete Wirkung. Generische Namen ohne Aussage (`Handle`) sind ein Befund. Varianten eines Verhaltens werden als eigene, sprechend benannte Methoden angelegt (`CreateFile` / `CreateTempFile`).

### Begründung
Eigener Abschnitt „Function names should say what they do" (1174-1211) und durchgängige Verb-Namen in Good-Snippets. Beobachtung B-08, Konfidenz hoch.

### Positives Beispiel
- `README.md:1202` – `Email.Send()`

  *Good – README.md:1202*

  ```csharp
  public void Send()
  ```

- `README.md:960-968` – `CreateFile`, `CreateTempFile`

  *Good – README.md:960-968*

  ```csharp
  public void CreateFile(string name)
  {
      Touch(name);
  }

  public void CreateTempFile(string name)
  {
      Touch("./temp/"  + name);
  }
  ```

- `README.md:1577`, `1587` – `WithdrawBalance`, `DepositBalance`

  *Good – README.md:1577*

  ```csharp
  public void WithdrawBalance(int amount)
  ```

  *Good – README.md:1587*

  ```csharp
  public void DepositBalance(int amount)
  ```

### Negatives Beispiel
- `README.md:1184` – `Email.Handle()` (Bad)

  *Bad – README.md:1184*

  ```csharp
  public void Handle()
  ```

- Abweichung in einem Good-Snippet: `README.md:1422` – `PerfReview()`

  *Good – README.md:1422*

  ```csharp
  public PerfReviewData PerfReview()
  ```

### Review-Frage
Kann man aus dem Methodennamen ablesen, was nach dem Aufruf anders ist oder was zurückgegeben wird?

### Schweregrad
SHOULD

### Automatisierbar
Nein

---

## CR-007: Boolesche Namen positiv formulieren; Zustandsprädikate mit `Is…`

### Regel
1. Boolesche Methoden und Properties enthalten keine Verneinung im Namen (`IsNodePresent`, nicht `IsNodeNotPresent`).
2. Methoden, die einen **Zustand** abfragen, heißen `Is…`.

Methoden, die eine Aktion ausführen und Erfolg als `bool` melden (`Fetch`, `Request`), fallen **nicht** unter Punkt 2.

### Begründung
Punkt 1: eigenes Bad/Good-Paar „Avoid negative conditionals" (748-777). In keinem Good-Snippet steht ein verneinter Name.
Punkt 2: `IsShopOpen` (325), `IsDOMNodePresent` (768), `IsPublished` (1480), `IsActive` (1147).
Die erneute Prüfung zeigt `bool`-Methoden ohne `Is` für Aktionen (1939, 1962). Beobachtung B-09, eingeschränkt, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:768`, `773` – `IsDOMNodePresent` + `if (IsDOMNodePresent(node))`

  *Good – README.md:768*

  ```csharp
  public bool IsDOMNodePresent(string node)
  ```

  *Good – README.md:773*

  ```csharp
  if (IsDOMNodePresent(node))
  ```

- `README.md:1480` – `article.IsPublished()`

  *Good – README.md:1480*

  ```csharp
  if (article.IsPublished())
  ```

### Negatives Beispiel
- `README.md:754`, `759` – `IsDOMNodeNotPresent` + `if (!IsDOMNodeNotPresent(node))` (Bad)

  *Bad – README.md:754*

  ```csharp
  public bool IsDOMNodeNotPresent(string node)
  ```

  *Bad – README.md:759*

  ```csharp
  if (!IsDOMNodeNotPresent(node))
  ```

- Grenzfall in einem Good-Snippet: `README.md:1836` – das Prädikat `VerifyCredentials()` ohne `Is`

  *Good – README.md:1836*

  ```csharp
  public bool VerifyCredentials()
  ```

### Review-Frage
Enthält ein boolescher Name ein „Not"/„No", und wird er dadurch an der Aufrufstelle doppelt verneint?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `Not`/`No` in booleschen Namen lässt sich per Muster finden. Ob etwas ein Zustandsprädikat ist, nicht.

---

## CR-008: Interfaces tragen den Präfix `I`

### Regel
Jedes Interface wird mit `I` + PascalCase benannt.

### Begründung
Alle 7 Interface-Deklarationen im Repository folgen dem Muster (814, 1932, 2126, 2164, 2169, 2174, 2265). Es gibt kein Gegenbeispiel. Beobachtung B-10, Konfidenz hoch.

### Positives Beispiel
- `README.md:814` – `IAirplane`

  *Good – README.md:814*

  ```csharp
  interface IAirplane
  ```

- `README.md:1932` – `IAdapter`

  *Good – README.md:1932*

  ```csharp
  interface IAdapter
  ```

- `README.md:2164-2174` – `IWorkable`, `IFeedable`, `IEmployee`

  *Good – README.md:2164-2174*

  ```csharp
  public interface IWorkable
  {
      void Work();
  }

  public interface IFeedable
  {
      void Eat();
  }

  public interface IEmployee : IFeedable, IWorkable
  ```

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Beginnt jedes neue Interface mit `I`?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE1006 mit einer Interface-Namensregel, CA1715.

---

## CR-009: Ein Begriff pro fachlichem Konzept

### Regel
Für dasselbe Konzept wird im gesamten Code derselbe Begriff verwendet. Synonyme wie `GetUserInfo`/`GetUserData`/`GetUserRecord` für dieselbe Abfrage sind ein Befund.

### Begründung
Eigenes Bad/Good-Paar (524-541). Der Präfix `Get…` wird durchgängig für Abfragen verwendet (82, 1164, 2067, 818). Beobachtung B-07, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:539` – `GetUser()`

  *Good – README.md:539*

  ```csharp
  GetUser();
  ```

### Negatives Beispiel
- `README.md:530-533` – `GetUserInfo`, `GetUserData`, `GetUserRecord`, `GetUserProfile` (Bad)

  *Bad – README.md:530-533*

  ```csharp
  GetUserInfo();
  GetUserData();
  GetUserRecord();
  GetUserProfile();
  ```

- Abweichung in einem Good-Snippet: `README.md:1435`, `1446` – `Lookup…` neben `Get…` in derselben Klasse

  *Good – README.md:1435*

  ```csharp
  private IEnumerable<PeersData> LookupPeers()
  ```

  *Good – README.md:1446*

  ```csharp
  private ManagerData LookupManager()
  ```

### Review-Frage
Gibt es im geänderten Code oder in seiner Umgebung schon einen etablierten Begriff für dieses Konzept, und wird er hier verwendet?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

## CR-010: Abstrakte Basisklassen enden auf `Base`

### Regel
Abstrakte Basisklassen werden `…Base` benannt.

### Begründung
Alle drei abstrakten Klassen im Repository folgen dem Muster (`AdapterBase` 1868, `ShapeBase` 2042, `EmployeeBase` 2219). Das Muster wird allerdings nie thematisiert. Beobachtung B-11, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:2042` – `abstract class ShapeBase`

  *Good – README.md:2042*

  ```csharp
  abstract class ShapeBase
  ```

### Negatives Beispiel
Im Repository nicht vorhanden.

### Review-Frage
Ist die neue abstrakte Klasse als `…Base` benannt?

### Schweregrad
CONSIDER

### Automatisierbar
Ja. Das lässt sich per Muster über `abstract class` prüfen.

> *Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet:* Die .NET Framework Design Guidelines raten für öffentliche APIs vom Suffix `Base` ab. Vor der Übernahme in ein Team-Regelwerk bewusst entscheiden.

---

# Methodendesign

## CR-011: Guard Clauses und früher Return statt tiefer Verschachtelung

### Regel
Vorbedingungen und Sonderfälle werden am Methodenanfang mit `if (...) return/throw` behandelt. Nach einem `return` oder `throw` folgt kein `else`. Tief verschachtelte `if/else`-Kaskaden sind ein Befund.

### Begründung
Eigener Abschnitt „Avoid nesting too deeply and return early" mit zwei Paaren (285-389). Unabhängig davon verwendet `BankAccount.WithdrawBalance` eine Guard Clause (1579-1582). Beobachtung B-12, Konfidenz hoch.

### Positives Beispiel
- `README.md:325-334` – `IsShopOpen`

  *Good – README.md:325-334*

  ```csharp
  public bool IsShopOpen(string day)
  {
      if (string.IsNullOrEmpty(day))
      {
          return false;
      }

      string[] openingDays = ["friday", "saturday", "sunday"];
      return openingDays.Any(d => d == day.ToLower());
  }
  ```

- `README.md:370-388` – `Fibonacci`

  *Good – README.md:370-388*

  ```csharp
  public long Fibonacci(int n)
  {
      if (n == 0)
      {
          return 0;
      }

      if (n == 1)
      {
          return 1;
      }

      if (n > 50)
      {
          throw new System.Exception("Not supported");
      }

      return Fibonacci(n - 1) + Fibonacci(n - 2);
  }
  ```

- `README.md:1579-1584` – `WithdrawBalance`

  *Good – README.md:1579-1584*

  ```csharp
  if (amount > _balance)
  {
      throw new Exception('Amount greater than available balance.');
  }

  _balance -= amount;
  ```

### Negatives Beispiel
- `README.md:292-319` – `IsShopOpen` mit Verschachtelung (Bad)

  *Bad – README.md:292-319*

  ```csharp
  public bool IsShopOpen(string day)
  {
      if (!string.IsNullOrEmpty(day))
      {
          day = day.ToLower();
          if (day == "friday")
          {
              return true;
          }
          else if (day == "saturday")
          {
              return true;
          }
          else if (day == "sunday")
          {
              return true;
          }
          else
          {
              return false;
          }
      }
      else
      {
          return false;
      }

  }
  ```

- `README.md:340-364` – `Fibonacci` mit drei Ebenen (Bad)

  *Bad – README.md:340-364*

  ```csharp
  public long Fibonacci(int n)
  {
      if (n < 50)
      {
          if (n != 0)
          {
              if (n != 1)
              {
                  return Fibonacci(n - 1) + Fibonacci(n - 2);
              }
              else
              {
                  return 1;
              }
          }
          else
          {
              return 0;
          }
      }
      else
      {
          throw new System.Exception("Not supported");
      }
  }
  ```

### Review-Frage
Lässt sich eine verschachtelte Bedingung durch einen frühen `return` oder `throw` flacher machen?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Verschachtelungstiefe und `else` nach `return` sind messbar. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Sonar S1066/S134, Roslynator RCS1211.

---

## CR-012: Höchstens zwei Parameter, sonst ein Parameterobjekt

### Regel
Methoden haben idealerweise 0–2 Parameter. Bei 3 Parametern soll geprüft werden, ob ein Parameterobjekt passt. Ab 4 Parametern werden die Werte zu einem `record` zusammengefasst.

### Begründung
Eigener Abschnitt „Function arguments (2 or fewer ideally)" mit dem Text „three should be avoided. Anything more than that should be consolidated" (1097). Die erneute Prüfung zeigt: Deklarationen mit ≥ 3 Parametern gibt es nur in Bad-Snippets (1102, 1749). Beobachtung B-14, Konfidenz hoch.

### Positives Beispiel
- `README.md:1111-1126` – `record MenuConfig(...)` + `CreateMenu(MenuConfig config)`

  *Good – README.md:1111-1126*

  ```csharp
  public record MenuConfig(
      string Title,
      string Body,
      string ButtonText,
      bool Cancellable);

  var config = new MenuConfig(
      Title: "Foo",
      Body: "Bar",
      ButtonText: "Baz",
      Cancellable: true);

  public void CreateMenu(MenuConfig config)
  {
      // ...
  }
  ```

### Negatives Beispiel
- `README.md:1102` – `CreateMenu(string title, string body, string buttonText, bool cancellable)` (Bad)

  *Bad – README.md:1102*

  ```csharp
  public void CreateMenu(string title, string body, string buttonText, bool cancellable)
  ```

### Review-Frage
Hat eine neue oder geänderte Methode mehr als zwei Parameter, und gehören diese fachlich zu einem gemeinsamen Objekt?

### Schweregrad
SHOULD bei 3 Parametern. Bei ≥ 4 ebenfalls SHOULD, dann aber mit Pflicht zur Begründung.

### Automatisierbar
Ja, die Parameteranzahl ist zählbar. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Sonar S107.

---

## CR-013: Keine booleschen Flag-Parameter

### Regel
Ein `bool`-Parameter, der zwischen zwei Verhaltensweisen umschaltet, wird durch zwei eigene, sprechend benannte Methoden ersetzt.

### Begründung
Eigenes Bad/Good-Paar „Avoid flags in method parameters" mit der Begründung „A flag indicates that the method has more than one responsibility" (939). Das deckt sich mit CR-014. Beobachtung B-15, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:960-968` – `CreateFile(name)` + `CreateTempFile(name)`

  *Good – README.md:960-968*

  ```csharp
  public void CreateFile(string name)
  {
      Touch(name);
  }

  public void CreateTempFile(string name)
  {
      Touch("./temp/"  + name);
  }
  ```

### Negatives Beispiel
- `README.md:944-954` – `CreateFile(string name, bool temp = false)` (Bad)

  *Bad – README.md:944-954*

  ```csharp
  public void CreateFile(string name, bool temp = false)
  {
      if (temp)
      {
          Touch("./temp/" + name);
      }
      else
      {
          Touch(name);
      }
  }
  ```

### Review-Frage
Verzweigt die Methode anhand eines `bool`-Parameters in zwei unterschiedliche Abläufe?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `bool`-Parameter, die direkt in einem `if` ausgewertet werden, lassen sich finden. Ob das Flag das Verhalten umschaltet, muss ein Mensch beurteilen.

---

## CR-014: Eine Methode – eine Aufgabe – eine Abstraktionsebene

### Regel
Eine Methode erledigt genau eine Aufgabe auf einer Abstraktionsebene. Methoden, die mehrere Phasen hintereinander ausführen (z. B. Tokenisieren, Lexen und Parsen oder Laden, Filtern und Senden), werden aufgeteilt. Eine Klasse hat genau einen Änderungsgrund.

### Begründung
Drei unabhängige Abschnitte: „Functions should do one thing" (1133-1168), der als „Not finished yet" markierte Abschnitt „One level of abstraction" (1217-1356) und SRP (1797-1854). Beobachtung B-16, Konfidenz hoch.

### Positives Beispiel
- `README.md:1158-1167` – `SendEmailToListOfClients` + `GetActiveClients`

  *Good – README.md:1158-1167*

  ```csharp
  public void SendEmailToListOfClients(string[] clients)
  {
      var activeClients = GetActiveClients(clients);
      // Do some logic
  }

  public List<Client> GetActiveClients(string[] clients)
  {
      return db.Find(clients).Where(s => s.Status == "Active");
  }
  ```

- `README.md:1308-1355` – `Tokenizer`, `Lexer`, `BetterJSAlternative`

  <details><summary>Good – README.md:1308-1355 (48 Zeilen)</summary>

  ```csharp
  class Tokenizer
  {
      public string Tokenize(string code)
      {
          var regexes = new string[] {
              // ...
          };

          var statements = explode(" ", code);
          var tokens = new string[] {};
          foreach (var regex in regexes)
          {
              foreach (var statement in statements)
              {
                  tokens[] = /* ... */;
              }
          }

          return tokens;
      }
  }

  class Lexer
  {
      public string Lexify(string[] tokens)
      {
          var ast = new[] {};
          foreach (var token in tokens)
          {
              ast[] = /* ... */;
          }

          return ast;
      }
  }

  class BetterJSAlternative(Tokenizer tokenizer, Lexer lexer)
  {
      public string Parse(string code)
      {
          var tokens = tokenizer.Tokenize(code);
          var ast = lexer.Lexify(tokens);
          foreach (var node in ast)
          {
              // parse...
          }
      }
  }
  ```

  </details>

- `README.md:1834-1853` – `UserAuth` + `UserSettings`

  *Good – README.md:1834-1853*

  ```csharp
  class UserAuth(User user)
  {
      public bool VerifyCredentials()
      {
          // ...
      }
  }

  class UserSettings(User user)
  {
      private readonly UserAuth _auth = new(user);

      public void ChangeSettings(Settings settings)
      {
          if (_auth.VerifyCredentials())
          {
              // ...
          }
      }
  }
  ```

### Negatives Beispiel
- `README.md:1227-1253` – `ParseBetterJSAlternative` (Bad)

  *Bad – README.md:1227-1253*

  ```csharp
  public string ParseBetterJSAlternative(string code)
  {
      var regexes = [
          // ...
      ];

      var statements = explode(" ", code);
      var tokens = new string[] {};
      foreach (var regex in regexes)
      {
          foreach (var statement in statements)
          {
              // ...
          }
      }

      var ast = new string[] {};
      foreach (var token in tokens)
      {
          // lex...
      }

      foreach (var node in ast)
      {
          // parse...
      }
  }
  ```

- `README.md:1807-1828` – `UserSettings` mit Authentifizierung (Bad)

  *Bad – README.md:1807-1828*

  ```csharp
  class UserSettings
  {
      private User User;

      public UserSettings(User user)
      {
          User = user;
      }

      public void ChangeSettings(Settings settings)
      {
          if (verifyCredentials())
          {
              // ...
          }
      }

      private bool VerifyCredentials()
      {
          // ...
      }
  }
  ```

### Review-Frage
Lässt sich die Methode bzw. Klasse in einem Satz ohne „und" beschreiben?

### Schweregrad
SHOULD

### Automatisierbar
Nein. Methodenlänge und Komplexität sind nur Indizien.

---

## CR-015: Kein Schreiben auf globalen oder geteilten Zustand

### Regel
Methoden erhalten ihre Eingaben als Parameter und liefern Ergebnisse als Rückgabewert. Sie verändern keine globalen oder statischen Variablen. Globale Hilfsfunktionen für Konfiguration o. Ä. werden durch Instanzen ersetzt, die übergeben werden.

### Begründung
Zwei unabhängige Bad/Good-Paare: „Avoid side effects" (700-742) und „Don't write to global functions" (975-1017). Dazu der Text: „Have one service that does it. One and only one." (705). Beobachtung B-17, Konfidenz hoch.

### Positives Beispiel
- `README.md:731-735` – `SplitAndEnrichFullName(string name)` gibt einen String zurück

  *Good – README.md:731-735*

  ```csharp
  public string SplitAndEnrichFullName(string name)
  {
      var temp = name.Split(" ");
      return $"His first name is {temp[0]}, and his last name is {temp[1]}";
  }
  ```

- `README.md:995-1008` – Klasse `Configuration` mit injiziertem Dictionary

  *Good – README.md:995-1008*

  ```csharp
  class Configuration
  {
      private Dictionary<string, string> _configuration;

      public Configuration(Dictionary<string, string> configuration)
      {
          _configuration = configuration;
      }

      public string[] Get(string key)
      {
          return _configuration.ContainsKey(key) ? _configuration[key] : null;
      }
  }
  ```

### Negatives Beispiel
- `README.md:715-723` – globales `name` wird überschrieben (Bad)

  *Bad – README.md:715-723*

  ```csharp
  var name = "Ryan McDermott";

  public void SplitAndEnrichFullName()
  {
      var temp = name.Split(" ");
      name = $"His first name is {temp[0]}, and his last name is {temp[1]}"; // side effect
  }

  SplitAndEnrichFullName();
  ```

- `README.md:984-989` – globale Funktion `Config()` (Bad)

  *Bad – README.md:984-989*

  ```csharp
  public Dictionary<string, string> Config()
  {
      return new Dictionary<string,string>(){
          ["foo"] = "bar"
      };
  }
  ```

### Review-Frage
Verändert die Methode Zustand, der nicht zu ihrem eigenen Objekt gehört oder nicht als Parameter hereinkam?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Schreibzugriffe auf `static`-Felder lassen sich finden.

> Ausnahme im Repository: Fluent-Extension-Methods verändern bewusst ihr Argument (`README.md:1653-1690`, B-51). Diese Ausnahme ist nicht als Regel übernommen.

---

## CR-016: Aufrufende Methode steht über der aufgerufenen

### Regel
Innerhalb einer Klasse stehen Methoden in Aufrufreihenfolge: Die öffentliche Einstiegsmethode steht oben, die von ihr aufgerufenen privaten Methoden folgen direkt darunter.

### Begründung
Eigenes Bad/Good-Paar „Function callers and callees should be close" (1362-1459). Unabhängig davon folgt `Hash` → `ConvertTo32BitInt` (3094-3114) demselben Muster. Beobachtung B-18, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:1420-1455` – `PerformanceReview`

  *Good – README.md:1420-1455*

  ```csharp
  class PerformanceReview(Employee employee)
  {
      public PerfReviewData PerfReview()
      {
          GetPeerReviews();
          GetManagerReview();
          GetSelfReview();
      }

      private IEnumerable<PeerReviews> GetPeerReviews()
      {
          var peers = LookupPeers();
          // ...
      }

      private IEnumerable<PeersData> LookupPeers()
      {
          return db.lookup(employee, 'peers');
      }

      private ManagerData GetManagerReview()
      {
          var manager = LookupManager();
          return manager;
      }

      private ManagerData LookupManager()
      {
          return db.lookup(employee, 'manager');
      }

      private EmployeeData GetSelfReview()
      {
          // ...
      }
  }
  ```

### Negatives Beispiel
- `README.md:1370-1411` – `PerformanceReview`, Einstiegsmethode in der Mitte (Bad)

  <details><summary>Bad – README.md:1370-1411 (42 Zeilen)</summary>

  ```csharp
  class PerformanceReview
  {
      private readonly Employee _employee;

      public PerformanceReview(Employee employee)
      {
          _employee = employee;
      }

      private IEnumerable<PeersData> LookupPeers()
      {
          return db.lookup(_employee, 'peers');
      }

      private ManagerData LookupManager()
      {
          return db.lookup(_employee, 'manager');
      }

      private IEnumerable<PeerReviews> GetPeerReviews()
      {
          var peers = LookupPeers();
          // ...
      }

      public PerfReviewData PerfReview()
      {
          GetPeerReviews();
          GetManagerReview();
          GetSelfReview();
      }

      public ManagerData GetManagerReview()
      {
          var manager = LookupManager();
      }

      public EmployeeData GetSelfReview()
      {
          // ...
      }
  }
  ```

  </details>

### Review-Frage
Muss man beim Lesen der neuen Methode nach oben springen, um die aufgerufenen Hilfsmethoden zu finden?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

## CR-017: Fachliche Literale als Konstante oder Enum benennen

### Regel
String- und Zahlenliterale mit fachlicher Bedeutung (Rollen, Status, Berechtigungen, Grenzwerte) werden in Vergleichen und Verzweigungen nicht roh verwendet, sondern als `const` oder `enum` benannt.

### Begründung
Zwei unabhängige Paare: „Avoid magic string" (440-462) und „Use searchable names (part 2)" (590-627). Beobachtung B-21, Konfidenz **mittel**, weil Good-Snippets häufig Literale enthalten (1166, 967, 1437).

### Positives Beispiel
- `README.md:457-458` – `const … = "Admin"` + Vergleich

  *Good – README.md:457-458*

  ```csharp
  const string ADMIN_ROLE = "Admin"
  if (userRole == ADMIN_ROLE)
  ```

- `README.md:608-623` – `enum PersonAccess` statt `4`

  *Good – README.md:608-623*

  ```csharp
  public enum PersonAccess : int
  {
      ACCESS_READ = 1,
      ACCESS_CREATE = 2,
      ACCESS_UPDATE = 4,
      ACCESS_DELETE = 8
  }

  var person = new Person
  {
      Name = "John",
      Age = 42,
      PersonAccess= PersonAccess.ACCESS_CREATE
  };

  if (person.PersonAccess == PersonAccess.ACCESS_UPDATE)
  ```

### Negatives Beispiel
- `README.md:448` – `if (userRole == "Admin")` (Bad)

  *Bad – README.md:448*

  ```csharp
  if (userRole == "Admin")
  ```

- `README.md:599` – `if (data.PersonAccess == 4)` (Bad)

  *Bad – README.md:599*

  ```csharp
  if (data.PersonAccess == 4)
  ```

- Abweichung in einem Good-Snippet: `README.md:1166` – `s.Status == "Active"`

  *Good – README.md:1166*

  ```csharp
  return db.Find(clients).Where(s => s.Status == "Active");
  ```

### Review-Frage
Steht in einer Bedingung ein Literal, dessen Bedeutung man nur aus dem Kontext erschließen kann oder das an anderer Stelle wiederholt wird?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Literale in Vergleichen lassen sich finden, ob sie fachliche Bedeutung haben, nicht.

---

## CR-018: Fachliche Bedingungen hinter sprechenden Namen kapseln

### Regel
Fachliche Bedingungen (Statusvergleiche, Rechteprüfungen) stehen hinter einer benannten Methode oder erklärenden Variable, nicht als roher Ausdruck im `if`.

### Begründung
Eigenes Paar „Encapsulate conditionals" (1465-1484) und „Use explanatory variables" (633-662). Dazu `_auth.VerifyCredentials()` (1848). Beobachtung B-22, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:1480` – `if (article.IsPublished())`

  *Good – README.md:1480*

  ```csharp
  if (article.IsPublished())
  ```

- `README.md:656-658` – `cityGroup`, `zipCodeGroup` als erklärende Variablen

  *Good – README.md:656-658*

  ```csharp
  var cityGroup = matchesWithGroup.Groups["city"];
  var zipCodeGroup = matchesWithGroup.Groups["zipCode"];
  if(cityGroup.Success == true && zipCodeGroup.Success == true)
  ```

### Negatives Beispiel
- `README.md:1471` – `if (article.state == "published")` (Bad)

  *Bad – README.md:1471*

  ```csharp
  if (article.state == "published")
  ```

- `README.md:642` – `matches[0]`, `matches[1]` (Bad)

  *Bad – README.md:642*

  ```csharp
  if (matches[0].Success == true && matches[1].Success == true)
  ```

### Review-Frage
Muss man eine Bedingung erst gedanklich übersetzen, um zu verstehen, was fachlich geprüft wird?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

## CR-019: Konkrete Typen statt `dynamic`/`object` für Parameter

### Regel
Parameter werden so konkret wie möglich typisiert. `dynamic` als Parametertyp ist ein Befund. `object` ist nur mit Begründung zulässig.

### Begründung
Eigenes Paar „Avoid type-checking (part 2)" (905-930) und `Traveler` statt `object` in Part 1 (878). Beobachtung B-20, Konfidenz **mittel**, weil die Pattern-Matching-Alternative (888) weiter `object` verwendet.

### Positives Beispiel
- `README.md:926` – `Combine(int val1, int val2)`

  *Good – README.md:926*

  ```csharp
  public int Combine(int val1, int val2)
  ```

- `README.md:878` – `TravelToTexas(Traveler vehicle)`

  *Good – README.md:878*

  ```csharp
  public Path TravelToTexas(Traveler vehicle)
  ```

### Negatives Beispiel
- `README.md:911` – `Combine(dynamic val1, dynamic val2)` (Bad)

  *Bad – README.md:911*

  ```csharp
  public int Combine(dynamic val1, dynamic val2)
  ```

- `README.md:862` – `TravelToTexas(object vehicle)` + `GetType()` (Bad)

  *Bad – README.md:862*

  ```csharp
  public Path TravelToTexas(object vehicle)
  ```

### Review-Frage
Ist ein Parameter als `dynamic` oder `object` typisiert, obwohl die Menge der erlaubten Typen bekannt ist?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `dynamic` und `object` in Signaturen sind leicht zu finden.

---

# Klassen- und Objektdesign

## CR-020: Polymorphie statt Verzweigung über Typ oder Typkennung

### Regel
`switch`- oder `if`-Ketten, die nach Typ (`GetType()`, `is`) oder nach einer Typkennung (String- oder Enum-Feld, das den Typ repräsentiert) verzweigen, werden durch ein Interface bzw. eine abstrakte Methode mit einer Implementierung pro Variante ersetzt. Neue Varianten entstehen dann durch Hinzufügen, nicht durch Ändern.

### Begründung
Drei unabhängige Abschnitte: „Avoid conditionals" (783-850), „Avoid type-checking (part 1)" (856-882) und OCP (1860-1967). Beobachtung B-19, Konfidenz hoch.

### Positives Beispiel
- `README.md:814-849` – `IAirplane` mit `Boeing777`, `AirForceOne`, `Cessna`

  *Good – README.md:814-849*

  ```csharp
  interface IAirplane
  {
      // ...

      double GetCruisingAltitude();
  }

  class Boeing777 : IAirplane
  {
      // ...

      public double GetCruisingAltitude()
      {
          return GetMaxAltitude() - GetPassengerCount();
      }
  }

  class AirForceOne : IAirplane
  {
      // ...

      public double GetCruisingAltitude()
      {
          return GetMaxAltitude();
      }
  }

  class Cessna : IAirplane
  {
      // ...

      public double GetCruisingAltitude()
      {
          return GetMaxAltitude() - GetFuelExpenditure();
      }
  }
  ```

- `README.md:1932-1966` – `IAdapter` + `HttpRequester.Fetch` ohne Verzweigung

  *Good – README.md:1932-1966*

  ```csharp
  interface IAdapter
  {
      bool Request(string url);
  }

  class AjaxAdapter : IAdapter
  {
      public bool Request(string url)
      {
          // request and return promise
      }
  }

  class NodeAdapter : IAdapter
  {
      public bool Request(string url)
      {
          // request and return promise
      }
  }

  class HttpRequester
  {
      private readonly IAdapter Adapter;

      public HttpRequester(IAdapter adapter)
      {
          Adapter = adapter;
      }

      public bool Fetch(string url)
      {
          return Adapter.Request(url);
      }
  }
  ```

### Negatives Beispiel
- `README.md:798-806` – `switch (_type)` (Bad)

  *Bad – README.md:798-806*

  ```csharp
  switch (_type)
  {
      case '777':
          return GetMaxAltitude() - GetPassengerCount();
      case 'Air Force One':
          return GetMaxAltitude();
      case 'Cessna':
          return GetMaxAltitude() - GetFuelExpenditure();
  }
  ```

- `README.md:1905-1914` – `if (adapterName == "ajaxAdapter")` (Bad)

  *Bad – README.md:1905-1914*

  ```csharp
  var adapterName = Adapter.GetName();

  if (adapterName == "ajaxAdapter")
  {
      return MakeAjaxCall(url);
  }
  else if (adapterName == "httpNodeAdapter")
  {
      return MakeHttpCall(url);
  }
  ```

### Review-Frage
Muss diese Verzweigung angepasst werden, sobald eine neue Variante hinzukommt?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. `GetType() ==`, `typeof` in Vergleichen und `switch` über `string`-Felder lassen sich finden.

> Einschränkung aus dem Repository: Pattern Matching (`is Bicycle bicycle`) wird in `README.md:886-899` ausdrücklich als zulässige Alternative gezeigt. Es ist daher **kein** automatischer Befund, sondern ein Diskussionspunkt.

---

## CR-021: Keine öffentlichen Felder; Zustandsänderung über fachliche Methoden

### Regel
Klassen haben keine `public`-Felder. Bei Objekten mit Invarianten (z. B. Kontostand) ist der Zustand von außen nur lesbar (`{ get; }`, `{ get; private set; }`), Änderungen laufen über Methoden, die prüfen.

### Begründung
Zwei Paare: „Use getters and setters" (1530-1600) und „Make objects have private/protected members" (1606-1636). Dazu `Employee.TaxData { get; private set; }` + `SetTax` (1767-1772). Beobachtung B-24, Konfidenz **mittel**, weil reine Datenklassen in Naming-Beispielen `{ get; set; }` nutzen (195, 491).

### Positives Beispiel
- `README.md:1562-1591` – `BankAccount` mit `_balance`, `WithdrawBalance`, `DepositBalance`

  *Good – README.md:1562-1591*

  ```csharp
  class BankAccount
  {
      private double _balance = 0.0D;

      pubic double Balance {
          get {
              return _balance;
          }
      }

      public BankAccount(balance = 1000)
      {
         _balance = balance;
      }

      public void WithdrawBalance(int amount)
      {
          if (amount > _balance)
          {
              throw new Exception('Amount greater than available balance.');
          }

          _balance -= amount;
      }

      public void DepositBalance(int amount)
      {
          _balance += amount;
      }
  }
  ```

- `README.md:1629-1632` – `Employee(string name)` mit `Name { get; }`

  *Good – README.md:1629-1632*

  ```csharp
  class Employee(string name)
  {
      public string Name { get; } = name;
  }
  ```

### Negatives Beispiel
- `README.md:1550` – `public double Balance = 1000;` (Bad)

  *Bad – README.md:1550*

  ```csharp
  public double Balance = 1000;
  ```

- `README.md:1614` – `public string Name { get; set; }` (Bad)

  *Bad – README.md:1614*

  ```csharp
  public string Name { get; set; }
  ```

### Review-Frage
Kann aufrufender Code den Zustand des Objekts so verändern, dass eine fachliche Regel umgangen wird?

### Schweregrad
SHOULD. Öffentliche Felder sind SHOULD, beschreibbare Properties an Objekten mit Invarianten CONSIDER.

### Automatisierbar
Teilweise. Öffentliche Felder sind erkennbar, *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* CA1051. Ob ein Objekt Invarianten hat, muss ein Mensch beurteilen.

---

## CR-022: Komposition vor Vererbung

### Regel
Vererbung ist nur bei einer echten „ist-ein"-Beziehung zulässig. „Hat-ein"-Beziehungen (z. B. Mitarbeiter hat Steuerdaten) werden als Property oder Feld modelliert.

### Begründung
Eigener Abschnitt „Prefer composition over inheritance" mit Kriterien (1709-1776). Dazu passen DIP (2216-2296) und SRP (1834-1853), die Komposition verwenden. Beobachtung B-30, Konfidenz hoch.

### Positives Beispiel
- `README.md:1761-1772` – `Employee` hält `EmployeeTaxData? TaxData`

  *Good – README.md:1761-1772*

  ```csharp
  record EmployeeTaxData(string Ssn, string Salary);

  class Employee(string name, string email)
  {
      public string Name { get; } = name;
      public string Email { get; } = email;
      public EmployeeTaxData? TaxData { get; private set; }

      public void SetTax(string ssn, double salary)
      {
          TaxData = new EmployeeTaxData(ssn, salary);
      }
  ```

- `README.md:1842-1844` – `UserSettings` hält `UserAuth`

  *Good – README.md:1842-1844*

  ```csharp
  class UserSettings(User user)
  {
      private readonly UserAuth _auth = new(user);
  ```

### Negatives Beispiel
- `README.md:1744` – `class EmployeeTaxData : Employee` (Bad)

  *Bad – README.md:1744*

  ```csharp
  class EmployeeTaxData : Employee
  ```

- `README.md:1894` – `class HttpRequester : AdapterBase` (Bad)

  *Bad – README.md:1894*

  ```csharp
  class HttpRequester : AdapterBase
  ```

### Review-Frage
Lässt sich der Satz „<Unterklasse> ist ein <Basisklasse>" fachlich ohne Einschränkung bejahen?

### Schweregrad
SHOULD

### Automatisierbar
Nein

---

## CR-023: Kleine, rollenbezogene Interfaces

### Regel
Interfaces beschreiben eine Rolle mit wenigen Membern. Implementierungen, die einen Interface-Member leer lassen oder nur eine Exception werfen, weil sie ihn nicht brauchen, sind ein Befund. Das Interface wird dann aufgeteilt.

### Begründung
Eigener ISP-Abschnitt (2115-2199). Alle Interfaces in Good-Snippets haben genau einen Member (814, 1932, 2164, 2169, 2265). Beobachtung B-28, Konfidenz hoch.

### Positives Beispiel
- `README.md:2164-2176` – `IWorkable`, `IFeedable`, `IEmployee : IFeedable, IWorkable`

  *Good – README.md:2164-2176*

  ```csharp
  public interface IWorkable
  {
      void Work();
  }

  public interface IFeedable
  {
      void Eat();
  }

  public interface IEmployee : IFeedable, IWorkable
  {
  }
  ```

- `README.md:2192` – `Robot : IWorkable`

  *Good – README.md:2192*

  ```csharp
  public class Robot : IWorkable
  ```

### Negatives Beispiel
- `README.md:2152-2155` – `Robot.Eat()` mit dem Kommentar „robot can't eat, but it must implement this method" (Bad)

  *Bad – README.md:2152-2155*

  ```csharp
  public void Eat()
  {
      //.... robot can't eat, but it must implement this method
  }
  ```

### Review-Frage
Gibt es eine Implementierung, die einen Interface-Member nicht sinnvoll umsetzen kann?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Leere Methodenrümpfe und `throw new NotImplementedException()` in Interface-Implementierungen lassen sich finden.

---

## CR-024: Abhängigkeiten per Konstruktor als Abstraktion injizieren

### Regel
Eine Klasse erhält ihre Abhängigkeiten über den Konstruktor. Bei austauschbaren Abhängigkeiten ist der Parametertyp ein Interface, nicht die konkrete Klasse. Mehrere gleichartige Abhängigkeiten werden als `IEnumerable<IFoo>` injiziert, nicht als einzelne konkrete Parameter.

### Begründung
DIP-Abschnitt (2205-2296), OCP-Lösung `HttpRequester(IAdapter)` (1957) und `DBConnection(IOptions<…>)` (1070). Beobachtung B-27, Konfidenz hoch für Konstruktor-Injection, **mittel** für „Interface statt konkreter Typ".

### Positives Beispiel
- `README.md:2286` – `Manager(IEnumerable<IEmployee> employees)`

  *Good – README.md:2286*

  ```csharp
  public class Manager(IEnumerable<IEmployee> employees)
  ```

- `README.md:1957` – `HttpRequester(IAdapter adapter)`

  *Good – README.md:1957*

  ```csharp
  public HttpRequester(IAdapter adapter)
  ```

### Negatives Beispiel
- `README.md:2248` – `Manager(Robot robot, Human human)` (Bad)

  *Bad – README.md:2248*

  ```csharp
  public Manager(Robot robot, Human human)
  ```

- Abweichung in einem Good-Snippet: `README.md:1844` – `private readonly UserAuth _auth = new(user);` erzeugt die Abhängigkeit selbst

  *Good – README.md:1844*

  ```csharp
  private readonly UserAuth _auth = new(user);
  ```

### Review-Frage
Erzeugt die Klasse eine Abhängigkeit selbst (`new`) oder hängt sie von einer konkreten Klasse ab, die man im Test austauschen müsste?

### Schweregrad
SHOULD

### Automatisierbar
Nein. `new` für Service-Typen lässt sich nur heuristisch erkennen.

---

## CR-025: Kein selbstverwaltetes Singleton

### Regel
Klassen verwalten keine eigene statische Instanz (`static _instance` + `GetInstance()`). Die Lebensdauer wird außerhalb festgelegt (IoC-Container) und die Instanz per Konstruktor übergeben.

### Begründung
Eigener Abschnitt „Don't use a Singleton pattern" mit vier Begründungen: versteckte Abhängigkeiten, SRP, Kopplung, Testreihenfolge (1025-1086). Beobachtung B-26, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:1068-1083` – `DBConnection(IOptions<DbConnectionOption>)`, Auflösung „from IOC"

  *Good – README.md:1068-1076*

  ```csharp
  class DBConnection
  {
      public DBConnection(IOptions<DbConnectionOption> options)
      {
          // ...
      }

      // ...
  }
  ```

  *Good – README.md:1082-1083*

  ```csharp
  var options = <resolve from IOC>;
  var connection = new DBConnection(options);
  ```

### Negatives Beispiel
- `README.md:1040-1062` – `DBConnection.GetInstance()` (Bad)

  *Bad – README.md:1040-1062*

  ```csharp
  class DBConnection
  {
      private static DBConnection _instance;

      private DBConnection()
      {
          // ...
      }

      public static GetInstance()
      {
          if (_instance == null)
          {
              _instance = new DBConnection();
          }

          return _instance;
      }

      // ...
  }

  var singleton = DBConnection.GetInstance();
  ```

- **Widerspruch in einem Good-Snippet:** `README.md:244-276` – `SingleObject.GetInstance()` als Positivbeispiel für Benennung

  *Good – README.md:245-275*

  ```csharp
  public class SingleObject
  {
      // create an object of SingleObject
      private static SingleObject _instance = new SingleObject();

      // make the constructor private so that this class cannot be instantiated
      private SingleObject() {}

      // get the only object available
      public static SingleObject GetInstance()
      {
          return _instance;
      }

      public string ShowMessage()
      {
          return "Hello World!";
      }
  }

  public static void main(String[] args)
  {
      // illegal construct
      // var object = new SingleObject();

      // Get the only object available
      var singletonObject = SingleObject.GetInstance();

      // show the message
      singletonObject.ShowMessage();
  }
  ```

### Review-Frage
Gibt es eine `static`-Eigenschaft oder -Methode, die eine Instanz der eigenen Klasse liefert?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Das Muster `private static <EigenerTyp>` + `GetInstance`/`Instance` lässt sich finden.

---

## CR-026: Hilfsmethoden sind `private`

### Regel
Methoden, die nur innerhalb der Klasse aufgerufen werden, sind `private`. Öffentlich ist nur, was aufrufender Code braucht.

### Begründung
In `PerformanceReview` werden `GetManagerReview`/`GetSelfReview` beim Übergang von Bad (1402, 1407 `public`) zu Good (1440, 1451) gezielt `private` gemacht. Dazu passt `private int ConvertTo32BitInt` (3111). Beobachtung B-32, Teil Methoden, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:1429-1454` – alle Hilfsmethoden von `PerformanceReview` sind `private`

  *Good – README.md:1429-1454*

  ```csharp
  private IEnumerable<PeerReviews> GetPeerReviews()
  {
      var peers = LookupPeers();
      // ...
  }

  private IEnumerable<PeersData> LookupPeers()
  {
      return db.lookup(employee, 'peers');
  }

  private ManagerData GetManagerReview()
  {
      var manager = LookupManager();
      return manager;
  }

  private ManagerData LookupManager()
  {
      return db.lookup(employee, 'manager');
  }

  private EmployeeData GetSelfReview()
  {
      // ...
  }
  ```

### Negatives Beispiel
- `README.md:1402-1410` – öffentliche Hilfsmethoden (Bad)

  *Bad – README.md:1402-1410*

  ```csharp
  public ManagerData GetManagerReview()
  {
      var manager = LookupManager();
  }

  public EmployeeData GetSelfReview()
  {
      // ...
  }
  ```

### Review-Frage
Wird die neue `public`-Methode tatsächlich außerhalb der Klasse aufgerufen?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Öffentliche Methoden ohne externe Aufrufer lassen sich per Analyse finden.

> Für **Klassen**-Modifier (`public`/`internal`/`sealed`) gibt es keine Regel. Die Verwendung ist inkonsistent, und `sealed` kommt nicht vor (B-32).

---

## CR-027: Keine duplizierte Logik (DRY)

### Regel
Gleichartige Abläufe, die sich nur in Typ oder Datenquelle unterscheiden, werden in einer gemeinsamen Methode oder Abstraktion zusammengeführt, statt sie zu kopieren.

### Begründung
Eigener Abschnitt „Don't repeat yourself" mit Bad/Good/Very-good (2302-2392). Die Quelle warnt ausdrücklich: „Bad abstractions can be worse than duplicate code" (2313). Konfidenz **mittel**, weil das Good-Snippet fehlerhaft ist (2361).

### Positives Beispiel
- `README.md:2381-2391` – `ShowList(...)` mit einer Schleife für alle Mitarbeitertypen

  *Very good – README.md:2381-2391*

  ```csharp
  public List<EmployeeData> ShowList(Employee employees)
  {
      foreach (var employee in employees)
      {
          render([
              employee.CalculateExpectedSalary(),
              employee.GetExperience(),
              employee.GetGithubLink()
          ]);
      }
  }
  ```

### Negatives Beispiel
- `README.md:2318-2351` – `ShowDeveloperList` / `ShowManagerList` nahezu identisch (Bad)

  *Bad – README.md:2318-2351*

  ```csharp
  public List<EmployeeData> ShowDeveloperList(Developers developers)
  {
      foreach (var developers in developer)
      {
          var expectedSalary = developer.CalculateExpectedSalary();
          var experience = developer.GetExperience();
          var githubLink = developer.GetGithubLink();
          var data = new[] {
              expectedSalary,
              experience,
              githubLink
          };

          Render(data);
      }
  }

  public List<ManagerData> ShowManagerList(Manager managers)
  {
      foreach (var manager in managers)
      {
          var expectedSalary = manager.CalculateExpectedSalary();
          var experience = manager.GetExperience();
          var githubLink = manager.GetGithubLink();
          var data =
          new[] {
              expectedSalary,
              experience,
              githubLink
          };

          render(data);
      }
  }
  ```

### Review-Frage
Gibt es einen fast identischen Block an anderer Stelle, und würde eine Änderung an beiden Stellen nötig?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Duplikaterkennung (z. B. Sonar CPD). Ob eine Abstraktion sinnvoll ist, bleibt eine Review-Entscheidung.

---

## CR-028: `record` für unveränderliche Daten- und Parameterobjekte

### Regel
Reine Datenträger ohne Verhalten, insbesondere Parameterobjekte (CR-012) und abhängige Wertobjekte, werden als positionaler `record` modelliert.

### Begründung
Die beiden einzigen `record`-Vorkommen sind genau solche Datenträger (1111, 1761). Beide stammen aus dem bewussten Modernisierungs-Commit `a604cf9`. In `README-zh.md` gibt es 0 `record`. Beobachtung B-31, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:1111-1115` – `record MenuConfig(...)`

  *Good – README.md:1111-1115*

  ```csharp
  public record MenuConfig(
      string Title,
      string Body,
      string ButtonText,
      bool Cancellable);
  ```

- `README.md:1761` – `record EmployeeTaxData(string Ssn, string Salary)`

  *Good – README.md:1761*

  ```csharp
  record EmployeeTaxData(string Ssn, string Salary);
  ```

### Negatives Beispiel
Im Repository nicht als Bad-Snippet vorhanden.

### Review-Frage
Ist die neue Klasse ein reiner Datenträger ohne Verhalten, der als `record` klarer wäre?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

---

## CR-029: Primärkonstruktoren für Klassen, die nur Abhängigkeiten oder Initialwerte übernehmen

### Regel
Klassen, deren Konstruktor ausschließlich Parameter in Felder oder Properties überträgt, verwenden einen Primärkonstruktor (C# 12).

### Begründung
7 Primärkonstruktoren in Good-Snippets (1344, 1420, 1629, 1763, 1834, 1842, 2286), alle eingeführt durch Commit `a604cf9` („class constructors to use modern C# syntax"). Die ältere `README-zh.md` enthält keinen. Beobachtung B-25, Konfidenz **mittel**, weil die Umstellung unvollständig ist (997, 1070, 1957).

### Positives Beispiel
- `README.md:2286` – `class Manager(IEnumerable<IEmployee> employees)`

  *Good – README.md:2286*

  ```csharp
  public class Manager(IEnumerable<IEmployee> employees)
  ```

- `README.md:1629-1632` – `class Employee(string name) { public string Name { get; } = name; }`

  *Good – README.md:1629-1632*

  ```csharp
  class Employee(string name)
  {
      public string Name { get; } = name;
  }
  ```

### Negatives Beispiel
- `README.md:1372-1377` – klassischer Konstruktor mit Feldzuweisung (Bad-Snippet, dort aber wegen der Reihenfolge kritisiert)

  *Bad – README.md:1372-1377*

  ```csharp
  private readonly Employee _employee;

  public PerformanceReview(Employee employee)
  {
      _employee = employee;
  }
  ```

- Nicht umgestellt, obwohl Good: `README.md:1955-1960` – `HttpRequester`

  *Good – README.md:1955-1960*

  ```csharp
  private readonly IAdapter Adapter;

  public HttpRequester(IAdapter adapter)
  {
      Adapter = adapter;
  }
  ```

### Review-Frage
Besteht der Konstruktor nur aus Zuweisungen von Parametern, sodass ein Primärkonstruktor ihn ersetzen kann?

### Schweregrad
CONSIDER

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE0290.

> *Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet:* Parameter eines Primärkonstruktors sind veränderlich und nicht `readonly`. Soll Unveränderlichkeit garantiert sein, wird der Parameter in ein `private readonly`-Feld übernommen.

---

## CR-030: Collection Expressions für Initialisierungen

### Regel
Arrays und Listen werden mit Collection Expressions (`[a, b, c]`) initialisiert statt mit `new T[] { … }` bzw. `new List<T> { … }`.

### Begründung
Laut Commit-Message von `a604cf9` („Refactor array initialization") bewusst eingeführt. Fundstellen: 332, 422, 1694, 2107, 2365, 2385. Beobachtung B-33, Konfidenz **mittel**, weil die Umstellung unvollständig ist (163, 1312, 1334, 1014).

### Positives Beispiel
- `README.md:332` – `string[] openingDays = ["friday", "saturday", "sunday"];`

  *Good – README.md:332*

  ```csharp
  string[] openingDays = ["friday", "saturday", "sunday"];
  ```

- `README.md:1694` – `List<int> list = [1, 2, 3, 4, 5];`

  *Good – README.md:1694*

  ```csharp
  List<int> list = [1, 2, 3, 4, 5];
  ```

### Negatives Beispiel
- Nicht umgestellt, obwohl Good: `README.md:163` – `new List<string> { … }`, `README.md:1312` – `new string[] { … }`

  *Good – README.md:163*

  ```csharp
  var songs = new List<string> { 'Back In Black', 'Stairway to Heaven', 'Hey Jude' };
  ```

  *Good – README.md:1312*

  ```csharp
  var regexes = new string[] {
  ```

### Review-Frage
Wird eine Collection mit `new …[] { }` oder `new List<T> { }` initialisiert, wo `[...]` möglich wäre?

### Schweregrad
CONSIDER

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE0300–IDE0305.

---

## CR-031: `foreach` bzw. LINQ statt Indexschleife

### Regel
Collections werden mit `foreach` und sprechender Schleifenvariable oder mit LINQ (`Any`, `Where`) durchlaufen. Eine `for`-Schleife mit Index ist nur zulässig, wenn der Index fachlich gebraucht wird.

### Begründung
Eigenes Paar „Avoid mental mapping" (403-433), außerdem `Any` statt `if`-Kette (333), `Where` (1166) und `foreach` über injizierte Collection (2290). Beobachtung B-35, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:424-433` – `foreach (var location in locations)`

  *Good – README.md:424-433*

  ```csharp
  foreach (var location in locations)
  {
      DoStuff();
      DoSomeOtherStuff();

      // ...
      // ...
      // ...
      Dispatch(location);
  }
  ```

- `README.md:332-333` – `openingDays.Any(...)`

  *Good – README.md:332-333*

  ```csharp
  string[] openingDays = ["friday", "saturday", "sunday"];
  return openingDays.Any(d => d == day.ToLower());
  ```

### Negatives Beispiel
- `README.md:405-407` – `for (var i = 0; i < l.Count(); i++) { var li = l[i]; … }` (Bad)

  *Bad – README.md:405-407*

  ```csharp
  for (var i = 0; i < l.Count(); i++)
  {
      var li = l[i];
  ```

### Review-Frage
Wird der Schleifenindex außer zum Elementzugriff für irgendetwas gebraucht?

### Schweregrad
CONSIDER

### Automatisierbar
Teilweise. `for`-Schleifen, deren Index nur als `x[i]` verwendet wird, lassen sich erkennen.

---

# Fehlerbehandlung

## CR-032: Beim Weiterwerfen `throw;` verwenden, nie `throw ex;`

### Regel
Eine gefangene Exception wird mit `throw;` weitergeworfen. `throw ex;` (bzw. `throw e;`) mit der gefangenen Variable ist verboten.

### Begründung
Zwei eigene Abschnitte mit derselben Aussage (2568-2601, 2693-2725). Begründung dort: Der Stacktrace geht verloren. `throw ex;` kommt nur in Bad-Snippets vor (2585, 2709). Beobachtung B-38, Konfidenz hoch.

### Positives Beispiel
- `README.md:2599` – `throw;`

  *Good – README.md:2599*

  ```csharp
  throw;
  ```

- `README.md:2723` – `logger.LogInfo(error); throw;`

  *Good – README.md:2723*

  ```csharp
  throw;
  ```

### Negatives Beispiel
- `README.md:2585`, `2709` – `throw ex;` (Bad)

  *Bad – README.md:2585*

  ```csharp
  throw ex;
  ```

  *Bad – README.md:2709*

  ```csharp
  throw ex;
  ```

### Review-Frage
Wird im `catch` die gefangene Variable erneut geworfen?

### Schweregrad
MUST

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* CA2200.

---

## CR-033: Keine leeren oder stillen `catch`-Blöcke

### Regel
Jeder `catch`-Block behandelt den Fehler erkennbar: Benutzer benachrichtigen, an einen Service melden oder loggen und weiterwerfen. Ein `catch` ohne Anweisung, oder nur mit einem Kommentar, ist verboten.

### Begründung
Eigener Abschnitt „Don't ignore caught errors" mit der Begründung „you should have a plan, or create a code path, for when it occurs" (2610). Alle ausformulierten Good-`catch`-Blöcke enthalten Anweisungen (2632-2638, 2720-2724, 2734-2738). Beobachtung B-40, Konfidenz hoch.

### Positives Beispiel
- `README.md:2632-2638` – `NotifyUserOfError(error)` / `ReportErrorToService(error)`

  *Good – README.md:2632-2638*

  ```csharp
  catch (Exception error)
  {
      NotifyUserOfError(error);

      // Another option
      ReportErrorToService(error);
  }
  ```

### Negatives Beispiel
- `README.md:2619-2622` – `catch (Exception ex) { // silent exception }` (Bad)

  *Bad – README.md:2619-2622*

  ```csharp
  catch (Exception ex)
  {
      // silent exception
  }
  ```

### Review-Frage
Was passiert nachvollziehbar, wenn dieser `catch`-Block erreicht wird?

### Schweregrad
MUST

### Automatisierbar
Teilweise. Leere `catch`-Blöcke lassen sich finden, *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Sonar S108/S2486. Ob die vorhandene Behandlung ausreicht, muss ein Mensch beurteilen.

> Nicht aus dem Repository ableitbar und daher **keine** Regel: ein Verbot, `catch (Exception)` pauschal zu fangen. Alle Good-Snippets außer einem tun genau das (B-41).

---

# Kommentare

## CR-034: Kein auskommentierter Code, keine Journal-Kommentare, kein toter Code

### Regel
Auskommentierter Code, Änderungsprotokolle in Kommentaren (Datum/Autor/Änderung) und nicht mehr aufgerufene Methoden werden entfernt. Die Historie gehört in die Versionsverwaltung.

### Begründung
Drei unabhängige Abschnitte: „Remove dead code" (1490-1522), „Don't leave commented out code" (2984-3002), „Don't have journal comments" (3008-3035). Beobachtung B-46, Konfidenz hoch.

### Positives Beispiel
- `README.md:1515-1521` – nur noch `RequestModule`

  *Good – README.md:1515-1521*

  ```csharp
  public void RequestModule(string url)
  {
      // ...
  }

  var request = RequestModule(requestUrl);
  InventoryTracker("apples", request, "www.inventory-awesome.io");
  ```

- `README.md:3031-3034` – `Combine` ohne Journal

  *Good – README.md:3031-3034*

  ```csharp
  public int Combine(int a,int b)
  {
      return a + b;
  }
  ```

### Negatives Beispiel
- `README.md:2993-2995` – `// doOtherStuff();` usw. (Bad)

  *Bad – README.md:2993-2995*

  ```csharp
  // doOtherStuff();
  // doSomeMoreStuff();
  // doSoMuchStuff();
  ```

- `README.md:3016-3021` – Journal-Block (Bad)

  *Bad – README.md:3016-3021*

  ```csharp
  /**
   * 2018-12-20: Removed monads, didn't understand them (RM)
   * 2017-10-01: Improved using special monads (JP)
   * 2016-02-03: Removed type-checking (LI)
   * 2015-03-14: Added combine with type-checking (JR)
   */
  ```

- `README.md:1498-1506` – `OldRequestModule` neben `NewRequestModule` (Bad)

  *Bad – README.md:1498-1506*

  ```csharp
  public void OldRequestModule(string url)
  {
      // ...
  }

  public void NewRequestModule(string url)
  {
      // ...
  }
  ```

- Abweichung in einem Good-Snippet: `README.md:268` – `// var object = new SingleObject();`

  *Good – README.md:268*

  ```csharp
  // var object = new SingleObject();
  ```

### Review-Frage
Enthält der Diff Code in Kommentaren, Datums-/Autorenvermerke oder Methoden ohne Aufrufer?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* Sonar S125 für auskommentierten Code, IDE0051 für ungenutzte private Member. Journal-Kommentare lassen sich per Datumsmuster finden.

---

## CR-035: Keine Positionsmarker und keine `#region`

### Regel
Trennkommentare (`/////…`) und `#region`/`#endregion` werden nicht verwendet, um Code zu gliedern.

### Begründung
Eigener Abschnitt „Avoid positional markers" mit zwei Bad-Varianten (2917-2978). Kein Good-Snippet im Repository enthält `#region`. Die erneute Prüfung bestätigt: `#region` kommt nur in 2947 und 2956 vor (Bad). Beobachtung B-47, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:2968-2977` – dieselbe Struktur ohne Marker

  *Good – README.md:2968-2977*

  ```csharp
  var model = new[]
  {
      menu: 'foo',
      nav: 'bar'
  };

  void Actions()
  {
      // ...
  };
  ```

### Negatives Beispiel
- `README.md:2925-2936` – `////…` Trennlinien (Bad)

  *Bad – README.md:2925-2936*

  ```csharp
  ////////////////////////////////////////////////////////////////////////////////
  // Scope Model Instantiation
  ////////////////////////////////////////////////////////////////////////////////
  var model = new[]
  {
      menu: 'foo',
      nav: 'bar'
  };

  ////////////////////////////////////////////////////////////////////////////////
  // Action setup
  ////////////////////////////////////////////////////////////////////////////////
  ```

- `README.md:2947-2962` – `#region` (Bad)

  *Bad – README.md:2947-2962*

  ```csharp
  #region Scope Model Instantiation

  var model = {
      menu: 'foo',
      nav: 'bar'
  };

  #endregion

  #region Action setup

  void Actions() {
      // ...
  };

  #endregion
  ```

### Review-Frage
Wird `#region` oder ein Trennkommentar eingesetzt, wo eine Aufteilung in Methoden oder Klassen die Struktur tragen könnte?

### Schweregrad
SHOULD

### Automatisierbar
Ja, per Textsuche nach `#region` und Zeilen, die nur aus `/` oder `*` bestehen.

---

## CR-036: Kommentare begründen das Warum, nicht das Was

### Regel
Kommentare, die nur nacherzählen, was die nächste Zeile tut, werden durch bessere Namen oder eine ausgelagerte Methode ersetzt. Erhalten bleiben Kommentare, die eine Entscheidung begründen, die im Code nicht ausdrückbar ist.

### Begründung
Eigener Abschnitt mit Bad / „Better but still Bad" / Good (3041-3115). Das Good-Snippet behält genau einen Warum-Kommentar (Wahl von djb2) und ersetzt einen Was-Kommentar durch `ConvertTo32BitInt`. Beobachtung B-48, Konfidenz **mittel**.

### Positives Beispiel
- `README.md:3102-3106` – Warum-Kommentar zu djb2 + `ConvertTo32BitInt(hash)`

  *Good – README.md:3102-3106*

  ```csharp
  // use of djb2 hash algorithm as it has a good compromise
  // between speed and low collision with a very simple implementation
  hash = ((hash << 5) - hash) + character;

  hash = ConvertTo32BitInt(hash);
  ```

### Negatives Beispiel
- `README.md:3051-3065` – `// The hash`, `// Length of string` usw. (Bad)

  *Bad – README.md:3051-3065*

  ```csharp
  // The hash
  var hash = 0;

  // Length of string
  var length = data.length;

  // Loop through every character in data
  for (var i = 0; i < length; i++)
  {
      // Get character code.
      const char = data.charCodeAt(i);
      // Make the hash
      hash = ((hash << 5) - hash) + char;
      // Convert to 32-bit integer
      hash &= hash;
  ```

- Abweichung in einem Good-Snippet: `README.md:247-273` – Was-Kommentare in `SingleObject`

  *Good – README.md:247-273*

  ```csharp
      // create an object of SingleObject
      private static SingleObject _instance = new SingleObject();

      // make the constructor private so that this class cannot be instantiated
      private SingleObject() {}

      // get the only object available
      public static SingleObject GetInstance()
      {
          return _instance;
      }

      public string ShowMessage()
      {
          return "Hello World!";
      }
  }

  public static void main(String[] args)
  {
      // illegal construct
      // var object = new SingleObject();

      // Get the only object available
      var singletonObject = SingleObject.GetInstance();

      // show the message
  ```

### Review-Frage
Würde der Kommentar überflüssig, wenn die Variable oder Methode besser benannt wäre?

### Schweregrad
CONSIDER

### Automatisierbar
Nein

> Ausnahme im Repository: Strukturkommentare `// Arrange`, `// Act`, `// Assert` in Tests (CR-039).

---

# Formatierung

## CR-037: Geschweifte Klammern immer, im Allman-Stil

### Regel
`if`, `else`, `for`, `foreach`, `while` und `try`/`catch` haben immer geschweifte Klammern, auch bei einer einzigen Anweisung. Öffnende Klammern stehen auf einer eigenen Zeile.

### Begründung
Durchgängig in nahezu allen Snippets und in der README-`.editorconfig` festgelegt (`csharp_new_line_before_open_brace = all`, 2773). `Program.cs` folgt dem ebenfalls. Beobachtung B-13, Konfidenz hoch.

### Positives Beispiel
- `README.md:327-330` – Einzeiler-Guard mit Klammern über mehrere Zeilen

  *Good – README.md:327-330*

  ```csharp
  if (string.IsNullOrEmpty(day))
  {
      return false;
  }
  ```

- `Program.cs:4-11`

  *Program.cs:4-11*

  ```csharp
  {
      internal class Program
      {
          private static void Main(string[] args)
          {
              Console.WriteLine("This is C# repository.");
          }
      }
  ```

### Negatives Beispiel
- `README.md:2958` – `void Actions() {` (Bad-Snippet)

  *Bad – README.md:2958*

  ```csharp
  void Actions() {
  ```

- Abweichung in einem Good-Snippet: `README.md:1566` – `Balance {`

  *Good – README.md:1566*

  ```csharp
  pubic double Balance {
  ```

### Review-Frage
Fehlen irgendwo Klammern oder steht eine öffnende Klammer am Zeilenende?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* IDE0011 + `csharp_new_line_before_open_brace = all` + `dotnet format`.

---

## CR-038: Stilvorgaben liegen in einer `.editorconfig`

### Regel
Das Repository enthält eine `.editorconfig`, die mindestens Einrückung, Zeilenende, Klammerstil und Namensregeln festlegt. Stilfragen, die dort geregelt sind, werden per Werkzeug geprüft und nicht im Review diskutiert.

### Begründung
Eigener Abschnitt „Uses .editorconfig file" mit vollständiger Beispielkonfiguration (2747-2909). Beobachtung B-50, Konfidenz **mittel**, weil das Repository selbst **keine** `.editorconfig` hat (erneut geprüft) und die Snippets den dort gezeigten Regeln teils widersprechen.

### Positives Beispiel
- `README.md:2758-2908` – Beispielkonfiguration

  <details><summary>Good – README.md:2759-2907 (149 Zeilen)</summary>

  ```ini
  root = true

  [*]
  indent_style = space
  indent_size = 2
  end_of_line = lf
  charset = utf-8
  trim_trailing_whitespace = true
  insert_final_newline = true

  # C# files
  [*.cs]
  indent_size = 4
  # New line preferences
  csharp_new_line_before_open_brace = all
  csharp_new_line_before_else = true
  csharp_new_line_before_catch = true
  csharp_new_line_before_finally = true
  csharp_new_line_before_members_in_object_initializers = true
  csharp_new_line_before_members_in_anonymous_types = true
  csharp_new_line_within_query_expression_clauses = true

  # Code files
  [*.{cs,csx,vb,vbx}]
  indent_size = 4

  # Indentation preferences
  csharp_indent_block_contents = true
  csharp_indent_braces = false
  csharp_indent_case_contents = true
  csharp_indent_switch_labels = true
  csharp_indent_labels = one_less_than_current

  # avoid this. unless absolutely necessary
  dotnet_style_qualification_for_field = false:suggestion
  dotnet_style_qualification_for_property = false:suggestion
  dotnet_style_qualification_for_method = false:suggestion
  dotnet_style_qualification_for_event = false:suggestion

  # only use var when it's obvious what the variable type is
  # csharp_style_var_for_built_in_types = false:none
  # csharp_style_var_when_type_is_apparent = false:none
  # csharp_style_var_elsewhere = false:suggestion

  # use language keywords instead of BCL types
  dotnet_style_predefined_type_for_locals_parameters_members = true:suggestion
  dotnet_style_predefined_type_for_member_access = true:suggestion

  # name all constant fields using PascalCase
  dotnet_naming_rule.constant_fields_should_be_pascal_case.severity = suggestion
  dotnet_naming_rule.constant_fields_should_be_pascal_case.symbols  = constant_fields
  dotnet_naming_rule.constant_fields_should_be_pascal_case.style    = pascal_case_style

  dotnet_naming_symbols.constant_fields.applicable_kinds   = field
  dotnet_naming_symbols.constant_fields.required_modifiers = const

  dotnet_naming_style.pascal_case_style.capitalization = pascal_case

  # static fields should have s_ prefix
  dotnet_naming_rule.static_fields_should_have_prefix.severity = suggestion
  dotnet_naming_rule.static_fields_should_have_prefix.symbols  = static_fields
  dotnet_naming_rule.static_fields_should_have_prefix.style    = static_prefix_style

  dotnet_naming_symbols.static_fields.applicable_kinds   = field
  dotnet_naming_symbols.static_fields.required_modifiers = static

  dotnet_naming_style.static_prefix_style.required_prefix = s_
  dotnet_naming_style.static_prefix_style.capitalization = camel_case

  # internal and private fields should be _camelCase
  dotnet_naming_rule.camel_case_for_private_internal_fields.severity = suggestion
  dotnet_naming_rule.camel_case_for_private_internal_fields.symbols  = private_internal_fields
  dotnet_naming_rule.camel_case_for_private_internal_fields.style    = camel_case_underscore_style

  dotnet_naming_symbols.private_internal_fields.applicable_kinds = field
  dotnet_naming_symbols.private_internal_fields.applicable_accessibilities = private, internal

  dotnet_naming_style.camel_case_underscore_style.required_prefix = _
  dotnet_naming_style.camel_case_underscore_style.capitalization = camel_case

  # Code style defaults
  dotnet_sort_system_directives_first = true
  csharp_preserve_single_line_blocks = true
  csharp_preserve_single_line_statements = false

  # Expression-level preferences
  dotnet_style_object_initializer = true:suggestion
  dotnet_style_collection_initializer = true:suggestion
  dotnet_style_explicit_tuple_names = true:suggestion
  dotnet_style_coalesce_expression = true:suggestion
  dotnet_style_null_propagation = true:suggestion

  # Expression-bodied members
  csharp_style_expression_bodied_methods = false:none
  csharp_style_expression_bodied_constructors = false:none
  csharp_style_expression_bodied_operators = false:none
  csharp_style_expression_bodied_properties = true:none
  csharp_style_expression_bodied_indexers = true:none
  csharp_style_expression_bodied_accessors = true:none

  # Pattern matching
  csharp_style_pattern_matching_over_is_with_cast_check = true:suggestion
  csharp_style_pattern_matching_over_as_with_null_check = true:suggestion
  csharp_style_inlined_variable_declaration = true:suggestion

  # Null checking preferences
  csharp_style_throw_expression = true:suggestion
  csharp_style_conditional_delegate_call = true:suggestion

  # Space preferences
  csharp_space_after_cast = false
  csharp_space_after_colon_in_inheritance_clause = true
  csharp_space_after_comma = true
  csharp_space_after_dot = false
  csharp_space_after_keywords_in_control_flow_statements = true
  csharp_space_after_semicolon_in_for_statement = true
  csharp_space_around_binary_operators = before_and_after
  csharp_space_around_declaration_statements = do_not_ignore
  csharp_space_before_colon_in_inheritance_clause = true
  csharp_space_before_comma = false
  csharp_space_before_dot = false
  csharp_space_before_open_square_brackets = false
  csharp_space_before_semicolon_in_for_statement = false
  csharp_space_between_empty_square_brackets = false
  csharp_space_between_method_call_empty_parameter_list_parentheses = false
  csharp_space_between_method_call_name_and_opening_parenthesis = false
  csharp_space_between_method_call_parameter_list_parentheses = false
  csharp_space_between_method_declaration_empty_parameter_list_parentheses = false
  csharp_space_between_method_declaration_name_and_open_parenthesis = false
  csharp_space_between_method_declaration_parameter_list_parentheses = false
  csharp_space_between_parentheses = false
  csharp_space_between_square_brackets = false

  [*.{asm,inc}]
  indent_size = 8

  # Xml project files
  [*.{csproj,vcxproj,vcxproj.filters,proj,nativeproj,locproj}]
  indent_size = 2

  # Xml config files
  [*.{props,targets,config,nuspec}]
  indent_size = 2

  [CMakeLists.txt]
  indent_size = 2

  [*.cmd]
  indent_size = 2
  ```

  </details>

### Negatives Beispiel
- Im Text beschrieben: „indent style is `space` and `tab` mixed in the project" (2752)
- Das Repository-Root selbst enthält keine `.editorconfig`.

### Review-Frage
Deckt die `.editorconfig` die geänderten Dateitypen ab, und läuft die Formatprüfung im Build oder in der CI?

### Schweregrad
SHOULD

### Automatisierbar
Ja. *Werkzeughinweis (Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet):* `dotnet format --verify-no-changes`, `EnforceCodeStyleInBuild`.

---

# Tests

## CR-039: Ein Konzept pro Test, gegliedert nach Arrange/Act/Assert

### Regel
Jede Testmethode prüft genau ein Verhalten bzw. Szenario und ist in Arrange, Act und Assert gegliedert. Der Methodenname benennt das Szenario. Mehrere unabhängige Szenarien in einer Testmethode sind ein Befund.

### Begründung
Eigener Abschnitt „Single concept per test" mit Verweis auf das AAA-Muster (2410-2486). Das Good-Snippet teilt einen Test mit drei Szenarien in drei Tests auf, jeweils mit `// Arrange`/`// Act`/`// Assert` und genau einem `Assert`. Beobachtung B-45, nach erneuter Prüfung Konfidenz **mittel**.

### Positives Beispiel
- `README.md:2444-2484` – `Handle30DayMonths`, `HandleLeapYear`, `HandleNonLeapYear`

  <details><summary>Good – README.md:2444-2484 (41 Zeilen)</summary>

  ```csharp
  public class MakeDotNetGreatAgainTests
  {
      [Fact]
      public void Handle30DayMonths()
      {
          // Arrange
          var date = new MyDateTime("1/1/2015");

          // Act
          date.AddDays(30);

          // Assert
          Assert.Equal("1/31/2015", date);
      }

      [Fact]
      public void HandleLeapYear()
      {
          // Arrange
          var date = new MyDateTime("2/1/2016");

          // Act
          date.AddDays(28);

          // Assert
          Assert.Equal("02/29/2016", date);
      }

      [Fact]
      public void HandleNonLeapYear()
      {
          // Arrange
          var date = new MyDateTime("2/1/2015");

          // Act
          date.AddDays(28);

          // Assert
          Assert.Equal("03/01/2015", date);
      }
  }
  ```

  </details>

### Negatives Beispiel
- `README.md:2421-2435` – `HandleDateBoundaries` mit drei Szenarien (Bad)

  *Bad – README.md:2421-2435*

  ```csharp
  [Fact]
  public void HandleDateBoundaries()
  {
      var date = new MyDateTime("1/1/2015");
      date.AddDays(30);
      Assert.Equal("1/31/2015", date);

      date = new MyDateTime("2/1/2016");
      date.AddDays(28);
      Assert.Equal("02/29/2016", date);

      date = new MyDateTime("2/1/2015");
      date.AddDays(28);
      Assert.Equal("03/01/2015", date);
  }
  ```

### Review-Frage
Wenn dieser Test fehlschlägt: Weiß man allein aus seinem Namen, welches Verhalten kaputt ist?

### Schweregrad
SHOULD

### Automatisierbar
Teilweise. Die Anzahl der Act/Assert-Blöcke pro Test lässt sich heuristisch prüfen.

> Nicht aus dem Repository ableitbar: ein Namensschema für Testmethoden (z. B. `Method_Scenario_Expected`), Regeln zu Mocks oder Parametrisierung (`[Theory]`), eine Coverage-Schwelle. Das README fordert nur Tests „for every new feature/module" (2406).

---

# Anhang: Allgemeine C#-Empfehlungen

**Allgemeine C#-Empfehlung – nicht aus dem Repository abgeleitet.**

Die folgenden Punkte betreffen Prüfthemen, für die das Repository keinen oder keinen belastbaren Code enthält (siehe Abschnitt „Keine oder unzureichende Evidenz" in den Beobachtungen). Sie sind **keine** CR-Regeln und stehen hier nur, damit die Lücke sichtbar bleibt.

| Thema | Empfehlung | Bezug im Repository |
|---|---|---|
| async/await | Kein `async void` außer Event-Handlern. Kein `.Result`/`.Wait()` in asynchronem Code. `CancellationToken` durchreichen. | Nur Prosa-Tabellen (`README.md:2501-2551`), kein Code (B-44) |
| IDisposable | `IDisposable`-Objekte mit `using`/`using var` freigeben | Good-Snippet `README.md:576-583` gibt `MemoryStream`/`StreamReader` **nicht** frei |
| Nullable | `<Nullable>enable</Nullable>`, Argumente mit `ArgumentNullException.ThrowIfNull` prüfen | Nur ein `?`-Fund (B-37) |
| Exception-Typen | Spezifische Typen werfen (`ArgumentOutOfRangeException`, `InvalidOperationException`), nicht `System.Exception` | Good-Snippets werfen `Exception` (384, 1581), B-42 |
| Logging | `ILogger<T>` injizieren, Message-Templates verwenden, Exceptions auf Error-Level loggen | Nur `logger.LogInfo(ex)` (B-43) |
| Architektur/Projekte | Schichtgrenzen über Projektreferenzen absichern | Kein `.sln`/`.csproj` im Repository |
