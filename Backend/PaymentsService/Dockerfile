# Используем официальный образ ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Release

# Создаем директорию для сертификатов
RUN mkdir /https

# Настраиваем HTTPS
ENV HTTP_PORTS=5083
ENV HTTPS_PORTS=7083
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="HttpsPassword"

EXPOSE 5083 7083

# Сборка и публикация
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["Backend/PaymentsService/PaymentsService.API/PaymentsService.API.csproj", "PaymentsService.API/"]
RUN dotnet restore "PaymentsService.API/PaymentsService.API.csproj"

# Копируем исходники
COPY Backend/PaymentsService PaymentsService/
WORKDIR "/src/PaymentsService/PaymentsService.API"
RUN dotnet build "PaymentsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentsService.API.dll"]
