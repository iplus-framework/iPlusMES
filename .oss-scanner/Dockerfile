# syntax=docker/dockerfile:1
#
# Build environment for Anthropic's OSS Scanner (https://red.anthropic.com/oss-scanner).
# Build context is the repository root (https://github.com/iplus-framework/iPlusMES).
#
# iPlusMES is layered on top of the iPlus framework: all MES projects reference
# the sibling checkout of the iPlus repository (see Directory.Build.props,
# which imports ../iPlus/Directory.Build.props). That framework checkout is
# cloned by this Dockerfile (network is available during build only), so the
# offline agent can rebuild and run everything without internet access.
FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive \
    LANG=C.UTF-8 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    NUGET_XMLDOC_MODE=skip

# Base tools. p7zip-full unpacks the sample databases (Database/*.7z).
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      ca-certificates curl git gnupg p7zip-full \
 && rm -rf /var/lib/apt/lists/*

# .NET 10 SDK (network is available during this build only).
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
 && bash /tmp/dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet \
 && rm /tmp/dotnet-install.sh \
 && ln -s /usr/share/dotnet/dotnet /usr/local/bin/dotnet

# Local SQL Server 2022 so the offline agent can start the headless MES console.
# Start it with: ACCEPT_EULA=Y MSSQL_SA_PASSWORD='<8+ chars, 3 of 4 categories>' sqlservr
RUN curl -fsSL https://packages.microsoft.com/keys/microsoft.asc \
      | gpg --dearmor > /usr/share/keyrings/microsoft-prod.gpg \
 && echo "deb [signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/22.04/mssql-server-2022 jammy main" \
      > /etc/apt/sources.list.d/mssql-server.list \
 && echo "deb [signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/22.04/prod jammy main" \
      > /etc/apt/sources.list.d/msprod.list \
 && apt-get update \
 && ACCEPT_EULA=Y NEEDRESTART_MODE=a apt-get install -y --no-install-recommends \
      mssql-server mssql-tools18 \
 && rm -rf /var/lib/apt/lists/*

ENV PATH="/opt/mssql-tools18/bin:${PATH}"

# scanner contract: the checkout lives inside the image, at /src
COPY . /src

# The iPlus framework must sit next to /src so that the relative ProjectReferences
# (../../iPlus/...) and the imported Directory.Build.props (../iPlus/...) resolve.
# Cloned during the build (network), used offline afterwards.
RUN git clone --depth 1 https://github.com/iplus-framework/iPlus.git /iPlus

WORKDIR /src

# Build the headless MES console exactly the way the framework's
# build/publish-release.sh does it (Android TFM off, CI build).
# Output lands in /src/bin/Release/net10.0.
RUN dotnet build gip.mes.console/gip.mes.console.csproj -c Release \
      -p:BuildAndroidTargetFramework=false \
      -p:ContinuousIntegrationBuild=true --nologo

# Helper for the agent: starts SQL Server, restores the sample databases from
# Database/*.7z and runs gip.mes.console (/U00 /P00). See the script header.
RUN chmod +x /src/.oss-scanner/run-console.sh
ENV IPLUS_CONSOLE_BIN="/src/bin/Release/net10.0"
