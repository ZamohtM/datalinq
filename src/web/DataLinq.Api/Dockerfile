# Unter https://aka.ms/customizecontainer erfahren Sie, wie Sie Ihren Debugcontainer anpassen und wie Visual Studio dieses Dockerfile verwendet, um Ihre Images für ein schnelleres Debuggen zu erstellen.

# Diese Stufe wird verwendet, wenn sie von VS im Schnellmodus ausgeführt wird (Standardeinstellung für Debugkonfiguration).
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6000
EXPOSE 6001


# Diese Stufe wird zum Erstellen des Dienstprojekts verwendet.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web/DataLinq.Api/DataLinq.Api.csproj", "web/DataLinq.Api/"]
COPY ["nuget/E.DataLinq.Engine.MsSqlServer/E.DataLinq.Engine.MsSqlServer.csproj", "nuget/E.DataLinq.Engine.MsSqlServer/"]
COPY ["nuget/E.DataLinq.Core/E.DataLinq.Core.csproj", "nuget/E.DataLinq.Core/"]
COPY ["nuget/E.DataLinq.Engine.OracleClient/E.DataLinq.Engine.OracleClient.csproj", "nuget/E.DataLinq.Engine.OracleClient/"]
COPY ["nuget/E.DataLinq.Engine.Postgres/E.DataLinq.Engine.Postgres.csproj", "nuget/E.DataLinq.Engine.Postgres/"]
COPY ["nuget/E.DataLinq.Engine.SQLite/E.DataLinq.Engine.SQLite.csproj", "nuget/E.DataLinq.Engine.SQLite/"]
COPY ["nuget/E.DataLinq.Web/E.DataLinq.Web.csproj", "nuget/E.DataLinq.Web/"]
COPY ["nuget/E.DataLinq.LanguageEngine.Razor/E.DataLinq.LanguageEngine.Razor.csproj", "nuget/E.DataLinq.LanguageEngine.Razor/"]
RUN dotnet restore "./web/DataLinq.Api/DataLinq.Api.csproj"
COPY . .
WORKDIR "/src/web/DataLinq.Api"
RUN dotnet build "./DataLinq.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Diese Stufe wird verwendet, um das Dienstprojekt zu veröffentlichen, das in die letzte Phase kopiert werden soll.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataLinq.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Diese Stufe wird in der Produktion oder bei Ausführung von VS im regulären Modus verwendet (Standard, wenn die Debugkonfiguration nicht verwendet wird).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataLinq.Api.dll"]