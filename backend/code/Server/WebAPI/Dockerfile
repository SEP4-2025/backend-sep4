# Base image for runtime (small, optimized)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage (full SDK image)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire solution into /src
COPY . .

# Restore dependencies (correct project path)
RUN dotnet restore "Server/WebAPI/WebAPI.csproj"

# Build & publish the WebAPI project to /app/publish
RUN dotnet publish "Server/WebAPI/WebAPI.csproj" -c Release -o /app/publish

# Runtime stage (small ASP.NET runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Run the WebAPI
ENTRYPOINT ["dotnet", "WebAPI.dll"]
