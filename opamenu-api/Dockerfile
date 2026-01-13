# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OpaMenu.Web/OpaMenu.Web.csproj", "OpaMenu.Web/"]
COPY ["OpaMenu.Application/OpaMenu.Application.csproj", "OpaMenu.Application/"]
COPY ["OpaMenu.Domain/OpaMenu.Domain.csproj", "OpaMenu.Domain/"]
COPY ["OpaMenu.Infrastructure/OpaMenu.Infrastructure.csproj", "OpaMenu.Infrastructure/"]

RUN dotnet restore "OpaMenu.Web/OpaMenu.Web.csproj"

COPY . .

WORKDIR "/src/OpaMenu.Web"
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
