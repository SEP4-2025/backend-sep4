FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server/SenderService/SenderService.csproj", "Server/SenderService/"]
RUN dotnet restore "Server/SenderService/SenderService.csproj"
COPY . .
WORKDIR "/src/Server/SenderService"
RUN dotnet build "SenderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SenderService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Install curl for potential health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "SenderService.dll"]