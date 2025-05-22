FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY . .  

USER $APP_UID

EXPOSE 5001

ENTRYPOINT ["dotnet", "DataLinq.Code.dll"]
