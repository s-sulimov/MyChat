FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /src
COPY . .
RUN dotnet restore "Server/Server.csproj"
RUN dotnet build "Server/Server.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS authorization-build
WORKDIR /src
COPY . .
RUN dotnet restore "Server.Authorization/Server.Authorization.csproj"
RUN dotnet build "Server.Authorization/Server.Authorization.csproj" -c Release -o /app/build

FROM server-build AS server-publish
RUN dotnet publish "Server/Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM authorization-build AS authorization-publish
RUN dotnet publish "Server.Authorization/Server.Authorization.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS server-final
WORKDIR /app
COPY --from=server-publish /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]

FROM base AS authorization-final
WORKDIR /app
COPY --from=authorization-publish /app/publish .
ENTRYPOINT ["dotnet", "Server.Authorization.dll"]
