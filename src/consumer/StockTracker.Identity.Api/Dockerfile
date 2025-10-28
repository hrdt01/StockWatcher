FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-jammy-chiseled
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
 
COPY . .
ENTRYPOINT ["dotnet", "StockTracker.Identity.Api.dll"]
