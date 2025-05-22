FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY . . 

USER $APP_UID

EXPOSE 6000
EXPOSE 6001

ENTRYPOINT ["dotnet", "DataLinq.Api.dll"]
