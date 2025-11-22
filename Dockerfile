# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Config & Solution
COPY ./NuGet.Config ./
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# ⚠️ QUAN TRỌNG: Copy tài nguyên tĩnh NGAY TỪ LÚC BUILD
COPY ["MVC/wwwroot", "MVC/wwwroot"]
COPY ["MVC/Views", "MVC/Views"]

# Restore
RUN dotnet restore "./DAL_Empty/DAL_Empty.csproj"
RUN dotnet restore "./API/API.csproj"
RUN dotnet restore "./MVC/MVC.csproj"

# Copy Code
COPY . .

# Publish
RUN dotnet publish "./API/API.csproj" -c Release -o /app/api --no-restore
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/mvc --no-restore

# ===========================================
# STAGE 2: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy App đã build
COPY --from=build /app/api ./api
COPY --from=build /app/mvc ./mvc

# ⚠️ QUAN TRỌNG: Copy đè lại wwwroot một lần nữa vào thư mục chạy của MVC
# Để đảm bảo cấu trúc /app/mvc/wwwroot/... là chính xác
COPY --from=build /src/MVC/wwwroot ./mvc/wwwroot
COPY --from=build /src/MVC/Views ./mvc/Views

# Cấu hình Database
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"

# 🔴 CHỐT HẠ: Cấu hình đường dẫn API
# Bắt buộc MVC phải gọi vào 127.0.0.1:5000
ENV ApiBaseUrl="http://127.0.0.1:5000/api/"

# Entrypoint
# - API: Chạy cổng 5000 (Nội bộ)
# - MVC: Chạy cổng $PORT (Public)
ENTRYPOINT ["sh", "-c", "dotnet api/API.dll --urls http://127.0.0.1:5000 & \
                        dotnet mvc/MVC.dll --urls http://0.0.0.0:${PORT:-80} && \
                        wait"]