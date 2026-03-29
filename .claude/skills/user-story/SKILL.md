---
name: user-story
description: Neues GitHub Issue als User Story formulieren, Akzeptanzkriterien iterativ erfragen und Markdown-Dokument anlegen
---

# User Story erstellen

Führe folgende Schritte aus:

## 1. Issue identifizieren

- Lies die offenen Issues im Repository `RalfGuder/MCP-easyVerein` mit `mcp__github__list_issues`
- Identifiziere das neueste Issue, das noch keine User Story enthält
- Falls $ARGUMENTS eine Issue-Nummer enthält, verwende dieses Issue

## 2. User Story formulieren

- Formuliere das Issue im Format: **Als** [Rolle], **möchte ich** [Funktion], **damit** [Nutzen]
- Orientiere dich am bestehenden Kontext des Projekts (easyVerein MCP-Server)

## 3. Akzeptanzkriterien iterativ erfragen

- Stelle dem Benutzer 2-3 gezielte Fragen mit `AskUserQuestion`, um die Akzeptanzkriterien zu erfassen
- Frage nach: Zielgruppe, technische Anforderungen, Durchsetzung, Priorität
- Wiederhole bei Bedarf, bis alle Kriterien klar sind

## 4. Issue aktualisieren

- Aktualisiere das GitHub Issue mit:
  - User Story
  - Link zum Dokument: `docs/user stories/XXX-kurzbeschreibung.md`
  - Akzeptanzkriterien als Checkliste
  - Aufgaben
  - Technische Hinweise
- Issue-Titel anpassen auf: `US-XXXX Kurzbeschreibung`

## 5. Markdown-Dokument anlegen

- Erstelle `docs/user stories/XXX-kurzbeschreibung.md`
- Nummerierung: nächste freie Nummer (prüfe vorhandene Dateien)
- Inhalt identisch zum Issue, mit Link zurück zum GitHub Issue
- Vorlage:

```markdown
# User Story XXX: Titel

> **GitHub Issue:** [#N – US-XXXX Titel](https://github.com/RalfGuder/MCP-easyVerein/issues/N)

## User Story

**Als** [Rolle],
**möchte ich** [Funktion],
**damit** [Nutzen].

## Akzeptanzkriterien

- [ ] Kriterium 1
- [ ] Kriterium 2

## Aufgaben

- Aufgabe 1
- Aufgabe 2

## Technische Hinweise

- Hinweis 1
```

## 6. Commit & Push

- Commit auf den aktuellen Feature-Branch
- Commit-Nachricht: `docs: User Story XXX – Titel hinzufügen`
- Push auf den Remote-Branch
