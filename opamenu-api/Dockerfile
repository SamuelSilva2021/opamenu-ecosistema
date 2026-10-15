# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj do projeto compartilhado
COPY ["opamenu-commons/Opamenu.Infrastructure.Shared/Opamenu.Infrastructure.Shared.csproj", "opamenu-commons/Opamenu.Infrastructure.Shared/"]
COPY ["opamenu-commons/Opamenu.Commons/Opamenu.Commons.csproj", "opamenu-commons/Opamenu.Commons/"]

# Copiar csprojs do opamenu-api
COPY ["opamenu-api/Opamenu.Web/Opamenu.Web.csproj", "opamenu-api/Opamenu.Web/"]
COPY ["opamenu-api/Opamenu.Application/Opamenu.Application.csproj", "opamenu-api/Opamenu.Application/"]
COPY ["opamenu-api/Opamenu.Domain/Opamenu.Domain.csproj", "opamenu-api/Opamenu.Domain/"]
COPY ["opamenu-api/Opamenu.Infrastructure/Opamenu.Infrastructure.csproj", "opamenu-api/Opamenu.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "opamenu-api/Opamenu.Web/Opamenu.Web.csproj"

# Copiar todo o código fonte (Build Context deve ser a raiz do repositório)
COPY . .

# Build
WORKDIR "/src/opamenu-api/Opamenu.Web"
RUN dotnet build "Opamenu.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Opamenu.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

RUN mkdir -p wwwroot/uploads

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Opamenu.Web.dll"]
