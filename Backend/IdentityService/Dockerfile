FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Release

ENV HTTP_PORTS=5080
ENV HTTPS_PORTS=7080

EXPOSE 5080 7080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Backend/IdentityService/IdentityService.API/IdentityService.API.csproj", "IdentityService.API/"]
RUN dotnet restore "IdentityService.API/IdentityService.API.csproj"

COPY Backend/IdentityService IdentityService/
WORKDIR "/src/IdentityService/IdentityService.API"
RUN dotnet build "IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IdentityService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
