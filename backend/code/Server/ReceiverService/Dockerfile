FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server/ReceiverService/ReceiverService.csproj", "Server/ReceiverService/"]
COPY ["Shared/Database/Database.csproj", "Shared/Database/"]
COPY ["Shared/DTOs/DTOs.csproj", "Shared/DTOs/"]
COPY ["Server/LogicImplements/LogicImplements.csproj", "Server/LogicImplements/"]
COPY ["Server/LogicInterfaces/LogicInterfaces.csproj", "Server/LogicInterfaces/"]
RUN dotnet restore "Server/ReceiverService/ReceiverService.csproj"
COPY . .
WORKDIR "/src/Server/ReceiverService"
RUN dotnet build "ReceiverService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReceiverService.csproj" -c Release -o /app/publish

# Changed from runtime to aspnet for ASP.NET Core applications
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReceiverService.dll"]

# Health check to ensure container is functioning properly
HEALTHCHECK --interval=30s --timeout=3s --retries=3 CMD curl -f http://localhost:8080/health || exit 1