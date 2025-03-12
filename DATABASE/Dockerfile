# Sử dụng image cơ bản Ubuntu 22.04
FROM ubuntu:22.04

# Cài đặt các gói phụ thuộc cần thiết
RUN apt-get update && apt-get install -y \
    curl \
    gnupg \
    apt-transport-https \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/*

# Thêm khóa GPG và kho lưu trữ của Microsoft SQL Server
RUN curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
RUN curl https://packages.microsoft.com/keys/microsoft.asc | tee /etc/apt/trusted.gpg.d/microsoft.asc
RUN curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | tee /etc/apt/sources.list.d/mssql-server-2022.list

# Cập nhật và cài đặt SQL Server 2022
RUN apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-server

# Cài đặt công cụ dòng lệnh (sqlcmd) - tùy chọn
RUN curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/msprod.list
RUN apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev

# Thiết lập biến môi trường cho SQL Server
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=Aa123456.
ENV PATH="$PATH:/opt/mssql-tools18/bin"

# Tạo thư mục cho dữ liệu SQL Server (tùy chọn)
RUN mkdir -p /var/opt/mssql

# Cấu hình SQL Server khi container khởi động
COPY /DATABASE/config.sh /usr/local/bin/config.sh
COPY /DATABASE/LINKU.bak /app/LINKU.bak

#COPY config.sh /usr/local/bin/config.sh
RUN chmod +x /usr/local/bin/config.sh
RUN /usr/local/bin/config.sh

# Mở cổng 1433 cho SQL Server
EXPOSE 1433

# Chạy SQL Server
CMD ["/opt/mssql/bin/sqlservr"]