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

# 🔥 TUYỆT CHIÊU: Dùng lệnh này để sửa file DAL_Empty.csproj ngay trong Docker
# Nó sẽ tìm chữ "Microsoft.NET.Sdk.Web" và đổi thành "Microsoft.NET.Sdk"
RUN sed -i 's/Microsoft.NET.Sdk.Web/Microsoft.NET.Sdk/g' ./DAL_Empty/DAL_Empty.csproj

# Copy Tài nguyên tĩnh
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

# Cài đặt các công cụ cần thiết (nếu cần debug sau này)
RUN apt-get update && apt-get install -y curl

# Copy App đã build
COPY --from=build /app/api ./api
COPY --from=build /app/mvc ./mvc

# Copy đè wwwroot và Views vào đúng thư mục của MVC
COPY --from=build /src/MVC/wwwroot ./mvc/wwwroot
COPY --from=build /src/MVC/Views ./mvc/Views

# Cấu hình ENV (Database & API)
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"
ENV ApiBaseUrl="http://127.0.0.1:5000/api/"

# 🚀 ENTRYPOINT (Sửa lỗi CSS & API):
# Chạy 2 ứng dụng song song, mỗi cái ở đúng thư mục của nó
ENTRYPOINT ["sh", "-c", "(cd /app/api && dotnet API.dll --urls http://127.0.0.1:5000) & \
                        (cd /app/mvc && dotnet MVC.dll --urls http://0.0.0.0:${PORT:-80}) && \
                        wait"]