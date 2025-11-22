# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy config
COPY ./NuGet.Config ./

# Copy csproj files
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# 🔴 FIX LỖI CSS 404 TẠI ĐÂY:
# Phải copy wwwroot vào đúng thư mục chứa MVC.csproj
COPY ["MVC/wwwroot", "MVC/wwwroot"] 
COPY ["MVC/Views", "MVC/Views"]

# Restore
RUN dotnet restore "./DAL_Empty/DAL_Empty.csproj"
RUN dotnet restore "./API/API.csproj"
RUN dotnet restore "./MVC/MVC.csproj"

# Copy toàn bộ source còn lại
COPY . .

# Build & Publish
RUN dotnet publish "./API/API.csproj" -c Release -o /app/api --no-restore
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/mvc --no-restore

# ===========================================
# STAGE 2: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy kết quả build
COPY --from=build /app/api ./api
COPY --from=build /app/mvc ./mvc

# 👇 COPY THỦ CÔNG ĐỂ CHẮC CHẮN 100% CÓ FILE CSS
COPY --from=build /src/MVC/wwwroot ./mvc/wwwroot
COPY --from=build /src/MVC/Views ./mvc/Views

# Connection String
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"

# 🔴 FIX LỖI KẾT NỐI API (Lỗi 500 ở log của bạn):
# Log cho thấy MVC đang gọi localhost:8080 (là chính nó) -> Sai
# Phải gọi vào cổng 5000 (nơi API đang chạy)
ENV ApiBaseUrl="http://127.0.0.1:5000/api/"

# Entrypoint
# API chạy cổng 5000 (nội bộ)
# MVC chạy cổng $PORT (Render cấp - public)
ENTRYPOINT ["sh", "-c", "dotnet api/API.dll --urls http://127.0.0.1:5000 & \
                        dotnet mvc/MVC.dll --urls http://0.0.0.0:${PORT:-80} && \
                        wait"]