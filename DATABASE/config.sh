#!/bin/bash
# entrypoint.sh

# Kiểm tra xem SQL Server đã được cấu hình chưa
if [ ! -f /var/opt/mssql/mssql.conf ]; then
    echo "Cấu hình SQL Server lần đầu tiên..."
    /opt/mssql/bin/mssql-conf setup run <<EOF
    $MSSQL_PID
    Y
    2
    $MSSQL_SA_PASSWORD
    $MSSQL_SA_PASSWORD
EOF
fi

/opt/mssql/bin/sqlservr &

# Đợi SQL Server khởi động
echo "Đợi SQL Server khởi động..."
WAIT 10
echo "SQL Server đã sẵn sàng."

# Kiểm tra file backup
if [ ! -f /app/LINKU.bak ]; then
    echo "Lỗi: File backup /app/LINKU.bak không tồn tại."
    exit 1
fi
if [ ! -r /app/LINKU.bak ]; then
    echo "Lỗi: Không có quyền đọc file /app/LINKU.bak."
    exit 1
fi

# Kiểm tra thư mục đích
if [ ! -d /var/opt/mssql/data ]; then
    echo "Lỗi: Thư mục /var/opt/mssql/data không tồn tại."
    exit 1
fi
if [ ! -w /var/opt/mssql/data ]; then
    echo "Lỗi: Không có quyền ghi vào /var/opt/mssql/data."
    exit 1
fi

# Restore database
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "RESTORE DATABASE LINKU FROM DISK = '/app/LINKU.bak' WITH 
    MOVE 'LINKU' TO '/var/opt/mssql/data/LINKU.mdf', 
    MOVE 'LINKU_log' TO '/var/opt/mssql/data/LINKU_log.ldf', 
    REPLACE;" -C > /tmp/restore_log.txt 2>&1

if [ $? -ne 0 ]; then
    echo "Lỗi khi khôi phục cơ sở dữ liệu LINKU. Xem chi tiết trong /tmp/restore_log.txt:"
    cat /tmp/restore_log.txt
    exit 1
else
    echo "Khôi phục cơ sở dữ liệu LINKU thành công."
fi

# Chạy SQL Server
exec "$@"