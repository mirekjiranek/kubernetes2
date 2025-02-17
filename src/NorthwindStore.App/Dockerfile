# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["NorthwindStore.App/NorthwindStore.App.csproj", "NorthwindStore.App/"]
COPY ["NorthwindStore.BL/NorthwindStore.BL.csproj", "NorthwindStore.BL/"]
COPY ["NorthwindStore.DAL/NorthwindStore.DAL.csproj", "NorthwindStore.DAL/"]
COPY packages /src/packages
RUN dotnet restore "./NorthwindStore.App/NorthwindStore.App.csproj"
COPY . .
WORKDIR "/src/NorthwindStore.App"
RUN dotnet build "./NorthwindStore.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NorthwindStore.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:16 as build-node
WORKDIR "/NorthwindStore.App"
COPY ["NorthwindStore.App/package.json", "."]
COPY ["NorthwindStore.App/package-lock.json", "."]
RUN npm ci
COPY ["/NorthwindStore.App/Styles/", "Styles/"]
RUN npm run build

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-node /NorthwindStore.App/wwwroot wwwroot/
ENTRYPOINT ["dotnet", "NorthwindStore.App.dll"]