# buid and run from the src folder using cmd
# docker build -f MemQuran.Api\Dockerfile -t rehmanab/memquran.api:latest . && docker run -d -p 3123:3123 --env ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_URLS=https://*:3123;http://*:3122 -e ASPNETCORE_Kestrel__Certificates__Default__Password="password" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v %USERPROFILE%\.aspnet\https:/https/ --name memquran.api rehmanab/memquran.api:latest

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 3123
EXPOSE 3122

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MemQuran.Api/MemQuran.Api.csproj", "MemQuran.Api/"]
COPY ["MemQuran.Core/MemQuran.Core.csproj", "MemQuran.Core/"]
COPY ["MemQuran.Infrastructure/MemQuran.Infrastructure.csproj", "MemQuran.Infrastructure/"]
RUN dotnet restore "MemQuran.Api/MemQuran.Api.csproj"
COPY . .
WORKDIR "/src/MemQuran.Api"
RUN dotnet build "./MemQuran.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MemQuran.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MemQuran.Api.dll"]
