FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["memquran-api/memquran-api.csproj", "memquran-api/"]
RUN dotnet restore "memquran-api/memquran-api.csproj"
COPY . .
WORKDIR "/src/memquran-api"
RUN dotnet build "memquran-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "memquran-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "memquran-api.dll"]
