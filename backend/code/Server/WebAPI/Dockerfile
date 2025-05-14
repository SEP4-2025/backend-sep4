FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/WebAPI/WebAPI.csproj", "Server/WebAPI/"]
RUN dotnet restore "Server/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/Server/WebAPI"
RUN dotnet build "WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet build "WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS final
WORKDIR /app
# I have no idea what this is doing, but it seems to be necessary
COPY --from=publish /app/build/WebAPI.dll .
COPY --from=publish /app/build/WebAPI.runtimeconfig.json .
COPY --from=publish /app/build/*.deps.json .
COPY --from=publish /app/build/*.pdb .
COPY --from=publish /src/Server/WebAPI/appsettings*.json .
COPY --from=publish /app/build/*.dll .
ENTRYPOINT ["dotnet", "WebAPI.dll"]