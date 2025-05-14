# Unter https://aka.ms/customizecontainer erfahren Sie, wie Sie Ihren Debugcontainer anpassen und wie Visual Studio dieses Dockerfile verwendet, um Ihre Images für ein schnelleres Debuggen zu erstellen.

# Diese Stufe wird verwendet, wenn sie von VS im Schnellmodus ausgeführt wird (Standardeinstellung für Debugkonfiguration).
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001


# Diese Stufe wird zum Erstellen des Dienstprojekts verwendet.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web/DataLinq.Code/DataLinq.Code.csproj", "web/DataLinq.Code/"]
COPY ["nuget/E.DataLinq.Code/E.DataLinq.Code.csproj", "nuget/E.DataLinq.Code/"]
COPY ["nuget/E.DataLinq.Web.Api.Client/E.DataLinq.Web.Api.Client.csproj", "nuget/E.DataLinq.Web.Api.Client/"]
COPY ["nuget/E.DataLinq.Core/E.DataLinq.Core.csproj", "nuget/E.DataLinq.Core/"]
RUN dotnet restore "./web/DataLinq.Code/DataLinq.Code.csproj"
COPY . .
WORKDIR "/src/web/DataLinq.Code"
RUN dotnet build "./DataLinq.Code.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Diese Stufe wird verwendet, um das Dienstprojekt zu veröffentlichen, das in die letzte Phase kopiert werden soll.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataLinq.Code.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Diese Stufe wird in der Produktion oder bei Ausführung von VS im regulären Modus verwendet (Standard, wenn die Debugkonfiguration nicht verwendet wird).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataLinq.Code.dll"]