# MCP-easyVerein

Ein lokaler MCP-Server (Model Context Protocol) zur Anbindung der [easyVerein API](https://easyverein.com/api/). Ermöglicht KI-Assistenten wie Claude den direkten Zugriff auf Vereinsdaten.

## Branch-Strategie

Dieses Projekt verwendet **GitHub Flow** als Branch-Strategie.

### Regeln

1. **`main` ist immer stabil und deploybar**
2. Für jede User Story / jedes Issue wird ein eigener Branch erstellt
3. Änderungen kommen über **Pull Requests** zurück in `main`
4. CI/CD-Pipeline prüft Tests und Coverage vor dem Merge
5. Feature-Branches werden nach dem Merge gelöscht

### Branch-Namenskonvention

| Typ | Muster | Beispiel |
|-----|--------|----------|
| Feature | `feature/US-XXXX-kurzbeschreibung` | `feature/US-0001-easyverein-mcp-server` |
| Bugfix | `fix/kurzbeschreibung` | `fix/api-token-validation` |

### Workflow

```
main (immer stabil)
  ├── feature/US-0001-easyverein-mcp-server
  ├── feature/US-0002-rules-und-prompts
  └── fix/api-token-validation
```

1. Branch von `main` erstellen
2. Änderungen committen (TDD: Tests zuerst)
3. Pull Request erstellen
4. CI/CD prüft Tests + Coverage (≥ 70%)
5. Code Review + Merge in `main`
6. Feature-Branch löschen

## Lizenz

Dieses Projekt steht unter der [MIT-Lizenz](LICENSE).
