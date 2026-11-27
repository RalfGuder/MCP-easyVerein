# Job-Image der Software Factory für MCP-easyVerein: Factory-Basis plus .NET 8 SDK.
# Der Runner baut es automatisch, sobald sich diese Datei oder das Basis-Image ändert.
FROM software-factory-job:latest

USER root
RUN apt-get update -qq \
 && apt-get install -y -qq --no-install-recommends dotnet-sdk-8.0 \
 && rm -rf /var/lib/apt/lists/*

USER pwuser
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
