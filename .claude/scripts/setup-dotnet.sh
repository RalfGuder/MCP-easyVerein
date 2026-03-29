#!/bin/bash
# SessionStart-Hook: .NET 8 SDK installieren (falls nicht vorhanden)
# Wird automatisch bei jedem Session-Start in Claude Code Web ausgeführt.

set -e

DOTNET_VERSION="8.0"
DOTNET_INSTALL_DIR="$HOME/.dotnet"

# Prüfen ob dotnet bereits installiert ist
if command -v dotnet &>/dev/null && dotnet --list-sdks | grep -q "^${DOTNET_VERSION}"; then
    echo "[setup-dotnet] .NET ${DOTNET_VERSION} SDK ist bereits installiert."
    exit 0
fi

echo "[setup-dotnet] Installiere .NET ${DOTNET_VERSION} SDK..."

# Microsoft dotnet-install.sh herunterladen und ausführen
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh
/tmp/dotnet-install.sh --channel ${DOTNET_VERSION} --install-dir "${DOTNET_INSTALL_DIR}"

# PATH ergänzen (für aktuelle und zukünftige Befehle in der Session)
export PATH="${DOTNET_INSTALL_DIR}:${PATH}"

# .bashrc ergänzen, falls der Eintrag noch nicht vorhanden ist
if ! grep -q "\.dotnet" "$HOME/.bashrc" 2>/dev/null; then
    echo "export PATH=\"${DOTNET_INSTALL_DIR}:\$PATH\"" >> "$HOME/.bashrc"
fi

echo "[setup-dotnet] .NET $(dotnet --version) erfolgreich installiert."
