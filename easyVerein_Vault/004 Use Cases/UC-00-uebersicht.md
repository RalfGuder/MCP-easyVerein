# Use Cases – Übersicht

> **Stand:** 2026-04-04

## Akteure

| Akteur | Beschreibung |
|--------|-------------|
| **Vereinsadministrator** | Menschlicher Benutzer, der den KI-Assistenten für die Vereinsverwaltung nutzt |
| **KI-Assistent** | MCP-Client (z.B. Claude Desktop), der über das MCP-Protokoll mit dem Server kommuniziert |
| **easyVerein API** | Externe REST-API von easyVerein, die die Vereinsdaten bereitstellt |

## Use-Case-Diagramm (textuell)

```
┌─────────────────────────────────────────────────────────────────┐
│                    MCP-easyVerein Server                        │
│                                                                 │
│  ┌──────────────────────┐    ┌──────────────────────────────┐  │
│  │ UC-01: Server         │    │ UC-06: API-Version            │  │
│  │ einrichten & starten  │    │ konfigurieren                 │  │
│  └──────────────────────┘    └──────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────────┐    ┌──────────────────────────────┐  │
│  │ UC-02: Mitglieder     │    │ UC-03: Kontaktdaten           │  │
│  │ verwalten             │    │ verwalten                     │  │
│  └──────────────────────┘    └──────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────────┐    ┌──────────────────────────────┐  │
│  │ UC-04: Rechnungen     │    │ UC-05: Veranstaltungen        │  │
│  │ verwalten             │    │ verwalten                     │  │
│  └──────────────────────┘    └──────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────────┐    ┌──────────────────────────────┐  │
│  │ UC-07: Fehler-        │    │ UC-08: Weitere Ressourcen     │  │
│  │ behandlung            │    │ verwalten (geplant)           │  │
│  └──────────────────────┘    └──────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
       ▲                                              ▲
       │                                              │
  ┌────┴─────┐   ┌──────────────┐          ┌─────────┴────────┐
  │ Vereins-  │   │ KI-Assistent │          │ easyVerein API   │
  │ admin     │──▶│ (MCP-Client) │──────────│ (extern)         │
  └──────────┘   └──────────────┘          └──────────────────┘
```

## Use-Case-Index

| ID | Titel | Status | Herkunft |
|----|-------|--------|----------|
| [UC-01](UC-01-server-einrichten.md) | Server einrichten und starten | Implementiert | US-001, US-005, US-006, US-007 |
| [UC-02](UC-02-mitglieder-verwalten.md) | Mitglieder verwalten | Implementiert | US-001, US-008 |
| [UC-03](UC-03-kontaktdaten-verwalten.md) | Kontaktdaten verwalten | Implementiert | US-001, US-008 |
| [UC-04](UC-04-rechnungen-verwalten.md) | Rechnungen verwalten | Teilweise | US-001, US-008 |
| [UC-05](UC-05-veranstaltungen-verwalten.md) | Veranstaltungen verwalten | Teilweise | US-001, US-008 |
| [UC-06](UC-06-api-version-konfigurieren.md) | API-Version konfigurieren | Implementiert | US-005 |
| [UC-07](UC-07-fehlerbehandlung.md) | Fehlerbehandlung | Implementiert | US-001 |
| [UC-08](UC-08-weitere-ressourcen-verwalten.md) | Weitere Ressourcen verwalten | Geplant | US-009 bis US-055 |

## Beziehungen zwischen Use Cases

| Beziehung | Von | Zu | Typ |
|-----------|-----|-----|-----|
| UC-02 bis UC-05, UC-08 | — | UC-07 | `«include»` (Fehlerbehandlung in allen CRUD-Operationen) |
| UC-01 | — | UC-06 | `«include»` (Versionskonfiguration beim Start) |
| UC-02 | — | UC-03 | `«include»` (Mitglied anlegen erzeugt Kontaktdaten) |

## Abgrenzung

- **In Scope:** Alle CRUD-Operationen auf easyVerein-Ressourcen über MCP-Tools, Konfiguration und Fehlerbehandlung
- **Out of Scope:** MCP Prompt Templates (US-002, REQ-04), SSE-Transport (FR-016), Konfigurationsdatei (FR-009)
