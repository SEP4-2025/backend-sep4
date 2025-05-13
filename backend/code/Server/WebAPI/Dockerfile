FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

WORKDIR "/src/WebAPI"
RUN dotnet restore "/src/WebAPI/WebAPI.csproj"

RUN dotnet publish "/src/WebAPI/WebAPI.csproj" -c Release -o /app/publish


# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WebAPI.dll"]
