# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Crea usuario no-root
RUN adduser --disabled-password --gecos "" appuser
USER appuser

# Expón puerto http (y opcional https)
EXPOSE 8080
# EXPOSE 8443

# Fuerza la app a escuchar en 8080
ENV ASPNETCORE_URLS=http://+:8080
# (opcional) entorno
# ENV ASPNETCORE_ENVIRONMENT=Production

# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia solo archivos de proyecto para cachear restore
COPY productHub.Api/productHub.Api.csproj productHub.Api/
COPY productHub.Application/productHub.Application.csproj productHub.Application/
COPY productHub.Domain/productHub.Domain.csproj productHub.Domain/
COPY productHub.Infrastructure/productHub.Infrastructure.csproj productHub.Infrastructure/
# (si usas Directory.Packages.props, NuGet.config, etc., cópialos aquí también)
# COPY Directory.Packages.props ./
# COPY NuGet.config ./

RUN dotnet restore productHub.Api/productHub.Api.csproj

# Ahora copia el resto del código
COPY . .

WORKDIR /src/productHub.Api
RUN dotnet build productHub.Api.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore

# ===== PUBLISH =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish productHub.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# ===== FINAL =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "productHub.Api.dll"]
