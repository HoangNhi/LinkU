# Sử dụng image chính thức của Microsoft SQL Server 2022
FROM mcr.microsoft.com/mssql/server:2022-latest
# Chuyển sang người dùng root để có quyền chỉnh sửa
USER root

# Cài đặt công cụ dòng lệnh (sqlcmd) - tùy chọn
RUN curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/msprod.list
RUN apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev

# Thiết lập biến môi trường cho SQL Server
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=Aa123456.
ENV PATH="$PATH:/opt/mssql-tools18/bin"

# Tạo thư mục cho dữ liệu SQL Server (tùy chọn)
RUN mkdir -p /var/opt/mssql/data

# Sao chép file backup và script cấu hình vào container
COPY DATABASE/LINKU.bak /app/LINKU.bak
COPY DATABASE/config.sh /usr/local/bin/config.sh

# Cấp quyền thực thi cho script
RUN chmod +x /usr/local/bin/config.sh
RUN /usr/local/bin/config.sh

# Mở cổng 1433 cho SQL Server
EXPOSE 1433

# Chạy SQL Server
CMD ["/opt/mssql/bin/sqlservr"]