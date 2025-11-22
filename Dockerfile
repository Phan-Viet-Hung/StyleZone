# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Config
COPY ./NuGet.Config ./
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# Copy Tài nguyên tĩnh (Quan trọng)
COPY ["MVC/wwwroot", "MVC/wwwroot"]
COPY ["MVC/Views", "MVC/Views"]

# Restore & Publish
RUN dotnet restore "./DAL_Empty/DAL_Empty.csproj"
RUN dotnet restore "./API/API.csproj"
RUN dotnet restore "./MVC/MVC.csproj"

# Lưu ý: Publish vào thư mục con /app/api và /app/mvc
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

# Copy đè wwwroot và Views vào đúng thư mục của MVC
COPY --from=build /src/MVC/wwwroot ./mvc/wwwroot
COPY --from=build /src/MVC/Views ./mvc/Views

# Cấu hình ENV
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"
ENV ApiBaseUrl="http://127.0.0.1:5000/api/"

# 🚀 ENTRYPOINT ĐÃ SỬA (QUAN TRỌNG NHẤT):
# 1. cd /app/api -> Chạy API tại chỗ (để nó nhận diện config tại chỗ)
# 2. cd /app/mvc -> Chạy MVC tại chỗ (để nó nhận diện wwwroot tại chỗ)
ENTRYPOINT ["sh", "-c", "(cd /app/api && dotnet API.dll --urls http://127.0.0.1:5000) & \
                        (cd /app/mvc && dotnet MVC.dll --urls http://0.0.0.0:${PORT:-80}) && \
                        wait"]