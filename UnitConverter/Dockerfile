# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UnitConverter.sln ./
COPY src/UnitConverter.API/UnitConverter.API.csproj src/UnitConverter.API/
COPY tests/UnitConverter.Tests/UnitConverter.Tests.csproj tests/UnitConverter.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/UnitConverter.API/UnitConverter.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UnitConverter.API.dll"]
