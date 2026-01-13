# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj do projeto compartilhado
COPY ["opamenu-shared-infrastructure/OpaMenu.Infrastructure.Shared/OpaMenu.Infrastructure.Shared.csproj", "opamenu-shared-infrastructure/OpaMenu.Infrastructure.Shared/"]

# Copiar csprojs do opamenu-api
COPY ["opamenu-api/OpaMenu.Web/OpaMenu.Web.csproj", "opamenu-api/OpaMenu.Web/"]
COPY ["opamenu-api/OpaMenu.Application/OpaMenu.Application.csproj", "opamenu-api/OpaMenu.Application/"]
COPY ["opamenu-api/OpaMenu.Domain/OpaMenu.Domain.csproj", "opamenu-api/OpaMenu.Domain/"]
COPY ["opamenu-api/OpaMenu.Infrastructure/OpaMenu.Infrastructure.csproj", "opamenu-api/OpaMenu.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "opamenu-api/OpaMenu.Web/OpaMenu.Web.csproj"

# Copiar todo o código fonte (Build Context deve ser a raiz do repositório)
COPY . .

# Build
WORKDIR "/src/opamenu-api/OpaMenu.Web"
RUN dotnet build "OpaMenu.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpaMenu.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

RUN mkdir -p wwwroot/uploads

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "OpaMenu.Web.dll"]
