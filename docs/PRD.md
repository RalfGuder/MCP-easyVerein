# Produkt: MCP-easyVerein

> Kurzfassung für die Software Factory. Ausführlich: `README.md`, `CLAUDE.md` und die User Stories unter `easyVerein_Vault/001 User Stories/`.

## Ziel

Ein lokaler MCP-Server, über den KI-Assistenten (z. B. Claude Code) die Vereinsverwaltung easyVerein bedienen: Mitglieder, Termine, Buchhaltung, Inventar und weitere Ressourcen lesen und pflegen.

## Nutzer

Vorstand und Kassenwart eines Vereins, die Routinearbeiten in easyVerein (Belege, Buchungen, Kontierung, Termine) mit einem Assistenten erledigen.

## Grundsätze

- Jede easyVerein-Ressource wird als eigener Satz MCP-Tools angebunden (list, get, create, update, delete, soweit die API es erlaubt).
- Antworten sind kompakt und token-sparsam.
- Schreibende Tools ändern echte Vereinsdaten; Tool-Beschreibungen weisen auf Folgen hin (z. B. endgültiges Löschen).
- Unterstützte API-Versionen v1.4 bis v2.0, Standard v2.0.

## Stand

26 Endpoints, siehe Tabelle „Implementierte Endpoints" in `CLAUDE.md`. Nächste Themen stehen in den offenen GitHub-Issues.
