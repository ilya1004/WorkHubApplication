# Используем официальный образ ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Release

# Создаем директорию для сертификатов
RUN mkdir /https

# Настраиваем HTTPS
ENV HTTP_PORTS=5081
ENV HTTPS_PORTS=7081
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="HttpsPassword"

EXPOSE 5081 7081

# Сборка и публикация
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["Backend/ProjectsService/ProjectsService.API/ProjectsService.API.csproj", "ProjectsService.API/"]
RUN dotnet restore "ProjectsService.API/ProjectsService.API.csproj"

# Копируем исходники
COPY Backend/ProjectsService ProjectsService/
WORKDIR "/src/ProjectsService/ProjectsService.API"
RUN dotnet build "ProjectsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectsService.API.dll"]
