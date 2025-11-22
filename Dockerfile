# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file config (nếu có)
COPY ./NuGet.Config ./

# Copy solution và các file csproj
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# Restore dependencies
RUN dotnet restore "./DAL_Empty/DAL_Empty.csproj"
RUN dotnet restore "./API/API.csproj"
RUN dotnet restore "./MVC/MVC.csproj"

# Copy toàn bộ source code
COPY . .

# Build và publish
# Lưu ý: MVC cần Views và wwwroot, dotnet publish sẽ tự động gom chúng nếu csproj cấu hình đúng
RUN dotnet publish "./API/API.csproj" -c Release -o /app/api --no-restore
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/mvc --no-restore

# ===========================================
# STAGE 2: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy build output
COPY --from=build /app/api ./api
COPY --from=build /app/mvc ./mvc

# 👇 QUAN TRỌNG: Copy tài nguyên tĩnh (CSS/JS) của MVC nếu publish chưa đủ
# Nếu trong folder /app/mvc/wwwroot đã có file thì dòng này thừa, nhưng cứ để cho chắc
COPY --from=build /src/MVC/wwwroot ./mvc/wwwroot 
COPY --from=build /src/MVC/Views ./mvc/Views

# ENV Connection string (Giữ nguyên)
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"

# 👇 SỬA 1: API chạy nội bộ (Localhost), MVC gọi vào đây
# Vì API và MVC chung 1 container nên gọi là 127.0.0.1
ENV ApiBaseUrl="http://127.0.0.1:5000/api/" 

# 👇 SỬA 2: ENTRYPOINT "BẺ LÁI"
# - API: Chạy ngầm (-d) hoặc chạy nền (&) ở cổng 5000 (Cổng nội bộ)
# - MVC: Chạy chính ở cổng $PORT (Cổng Render cấp - 10000) để người dùng truy cập được
ENTRYPOINT ["sh", "-c", "dotnet api/API.dll --urls http://127.0.0.1:5000 & \
                        dotnet mvc/MVC.dll --urls http://0.0.0.0:${PORT:-80} && \
                        wait"]