#!/bin/bash
/opt/mssql/bin/sqlservr &

# Đợi SQL Server khởi động (khoảng 15-20 giây để đảm bảo)
sleep 20s

# Restore database từ file .bak
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Aa123456. -Q "RESTORE DATABASE LINKU FROM DISK='/app/LINKU.bak' WITH REPLACE"

# Khởi động BE trong background
dotnet /app/publish/BE/BE.dll &

# Khởi động FE trong background
dotnet /app/publish/FE/FE.dll &

# Giữ container chạy
wait