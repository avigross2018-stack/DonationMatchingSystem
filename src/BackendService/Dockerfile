FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY BackendService.csproj ./
RUN dotnet restore BackendService.csproj

COPY . ./
RUN dotnet publish BackendService.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "BackendService.dll"]
