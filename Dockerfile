# Stage: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=${BUILD_CONFIGURATION}

WORKDIR /app

# Restore
COPY . /app
RUN dotnet restore NodeTrees.sln

# Build
RUN dotnet publish "src/NodeTrees.Server/NodeTrees.Server.csproj" -c $BUILD_CONFIGURATION -o /app/hub /p:UseAppHost=false

# Stage: run
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/hub .

ENTRYPOINT ["dotnet", "NodeTrees.Server.dll"]