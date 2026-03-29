FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Solution und Projektdateien kopieren
COPY MCP.EasyVerein.sln .
COPY src/MCP.EasyVerein.Domain/MCP.EasyVerein.Domain.csproj src/MCP.EasyVerein.Domain/
COPY src/MCP.EasyVerein.Application/MCP.EasyVerein.Application.csproj src/MCP.EasyVerein.Application/
COPY src/MCP.EasyVerein.Infrastructure/MCP.EasyVerein.Infrastructure.csproj src/MCP.EasyVerein.Infrastructure/
COPY src/MCP.EasyVerein.Server/MCP.EasyVerein.Server.csproj src/MCP.EasyVerein.Server/
RUN dotnet restore src/MCP.EasyVerein.Server/MCP.EasyVerein.Server.csproj

# Quellcode kopieren und bauen
COPY src/ src/
RUN dotnet publish src/MCP.EasyVerein.Server/MCP.EasyVerein.Server.csproj \
    -c Release -o /app/publish --no-restore

# Runtime-Image
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV EASYVEREIN_API_TOKEN=""
ENV EASYVEREIN_BASE_URL="https://easyverein.com/api"
ENV EASYVEREIN_API_VERSION="v1.7"

ENTRYPOINT ["dotnet", "MCP.EasyVerein.Server.dll"]
