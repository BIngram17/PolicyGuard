FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Azure App Service for Linux and most container platforms can route traffic to port 8080.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file first so Docker can cache NuGet restore separately from source changes.
COPY backend/PolicyGuard.Api/PolicyGuard.Api.csproj backend/PolicyGuard.Api/
RUN dotnet restore backend/PolicyGuard.Api/PolicyGuard.Api.csproj

# Copy the API source after restore to keep rebuilds fast during normal development.
COPY backend/PolicyGuard.Api/ backend/PolicyGuard.Api/
WORKDIR /src/backend/PolicyGuard.Api
RUN dotnet publish PolicyGuard.Api.csproj --configuration Release --no-restore --output /app/publish

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PolicyGuard.Api.dll"]
