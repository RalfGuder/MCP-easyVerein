# User Story 002: Rules und Prompts

> **GitHub Issue:** [#2 – US-0002 Rules und Prompts](https://github.com/RalfGuder/MCP-easyVerein/issues/2)

## User Story

**Als** Nutzer des easyVerein MCP-Servers,
**möchte ich** vorgefertigte MCP Prompt Templates mit eingebetteten Regeln für wiederkehrende Aufgaben nutzen können,
**damit** Aufgaben wie Mitgliederverwaltung, Rechnungen, Veranstaltungen, Sitzungen, Protokolle und E-Mails/Serienbriefe immer nach einheitlichen Format-, Workflow- und Validierungsregeln ausgeführt werden.

## Akzeptanzkriterien

- [ ] Prompts werden als MCP Prompt Templates über `prompts/list` und `prompts/get` bereitgestellt
- [ ] Prompts unterstützen Parameter (z.B. `{{mitgliedsname}}`, `{{rechnungsnummer}}`)
- [ ] Jeder Prompt enthält eingebettete Regeln für Formatvorgaben, Workflow-Schritte und Validierung
- [ ] Prompts sind mehrsprachig verfügbar (mindestens DE + EN)
- [ ] Prompts sind fest im MCP-Server eingebaut
- [ ] Erste Umsetzung priorisiert den Bereich Mitgliederverwaltung
- [ ] Folgende Bereiche sind vorgesehen: Mitgliederverwaltung, Rechnungen/Finanzen, Veranstaltungen, Sitzungen, Protokolle, E-Mails/Serienbriefe

## Aufgaben

- **MCP Prompt Templates implementieren** über den MCP-Standard (`prompts/list`, `prompts/get`)
- **Regeln einbetten** in jeden Prompt: Formatvorgaben, Workflow-Schritte und Validierungsregeln
- **Parametrisierung** der Prompts mit Platzhaltern (z.B. `{{mitgliedsname}}`)
- **Mehrsprachigkeit** umsetzen (mindestens DE + EN)
- **Prompt-Bereiche** schrittweise umsetzen:
  1. Mitgliederverwaltung (Priorität)
  2. Rechnungen / Finanzen
  3. Veranstaltungen
  4. Sitzungen
  5. Protokolle
  6. E-Mails und Serienbriefe

## Technische Hinweise

- Prompts werden fest im MCP-Server eingebaut (kein externes Konfigurationsformat)
- Bereitstellung über das MCP-Protokoll gemäß [MCP-Standard](https://modelcontextprotocol.io/)
- Jeder Prompt definiert seine erwarteten Parameter mit Beschreibung und Typ
