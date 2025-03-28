FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Release

ENV HTTP_PORTS=5083
ENV HTTPS_PORTS=7083

EXPOSE 5083 7083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Backend/PaymentsService/PaymentsService.API/PaymentsService.API.csproj", "PaymentsService.API/"]
RUN dotnet restore "PaymentsService.API/PaymentsService.API.csproj"

COPY Backend/PaymentsService PaymentsService/
WORKDIR "/src/PaymentsService/PaymentsService.API"
RUN dotnet build "PaymentsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentsService.API.dll"]
