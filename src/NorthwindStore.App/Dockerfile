#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["packages/", "packages/"]
COPY ["nuget.config", "."]
COPY ["NorthwindStore.App/NorthwindStore.App.csproj", "NorthwindStore.App/"]
COPY ["NorthwindStore.DAL/NorthwindStore.DAL.csproj", "NorthwindStore.DAL/"]
COPY ["NorthwindStore.BL/NorthwindStore.BL.csproj", "NorthwindStore.BL/"]
RUN dotnet restore "NorthwindStore.App/NorthwindStore.App.csproj"
COPY . .
WORKDIR "/src/NorthwindStore.App"
RUN dotnet build "NorthwindStore.App.csproj" -c Release -o /app/build

FROM node:16 as build-node
WORKDIR "/NorthwindStore.App"
COPY ["NorthwindStore.App/package.json", "."]
COPY ["NorthwindStore.App/package-lock.json", "."]
RUN npm ci
COPY ["/NorthwindStore.App/Styles/", "Styles/"]
RUN npm run build

FROM build AS publish
COPY --from=build-node /NorthwindStore.App/wwwroot wwwroot/
RUN dotnet publish "NorthwindStore.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthwindStore.App.dll"]