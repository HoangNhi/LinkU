# Stage base: Cài SQL Server và ASP.NET Core Runtime
FROM ubuntu:22.04 AS base
ENV DEBIAN_FRONTEND=noninteractive
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=CongNgheMangNangCao

# Cài các công cụ cơ bản và SQL Server
RUN apt-get update && apt-get install -y \
    curl \
    gnupg \
    && curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg \
    && curl https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc \
    && curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | tee /etc/apt/sources.list.d/mssql-server-2022.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-server \
    && curl https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc \
    && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev \
    # Cài ASP.NET Core Runtime
    && curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --runtime aspnetcore \
    && rm -rf /var/lib/apt/lists/*

# Thiết lập PATH và biến môi trường
ENV PATH="$PATH:/opt/mssql-tools/bin:/root/.dotnet"
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=Aa123456.
ENV MSSQL_PID=Express

# Stage build: Build project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FE/FE.csproj", "FE/"]
COPY ["BE/BE.csproj", "BE/"]
COPY ["MODELS/MODELS.csproj", "MODELS/"]
COPY ["ENTITIES/ENTITIES.csproj", "ENTITIES/"]
RUN dotnet restore "./FE/FE.csproj"
RUN dotnet restore "./BE/BE.csproj"
COPY . .
WORKDIR "/src/FE"
RUN dotnet build "./FE.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR "/src/BE"
RUN dotnet build "./BE.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage publish: Publish project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet publish "./FE/FE.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "./BE/BE.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage final: Kết hợp base và publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./LINKU.bak /app/LINKU.bak
COPY ./start.sh /app/start.sh
RUN chmod +x /app/start.sh
CMD ["/app/start.sh"]