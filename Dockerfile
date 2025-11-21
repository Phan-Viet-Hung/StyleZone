# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file NuGet.Config (để sửa lỗi DevExpress/build)
COPY ./NuGet.Config ./

# Copy solution và các file csproj cần thiết
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/
COPY ["MVC/wwwroot", "wwwroot"]
# Restore dependencies
RUN dotnet restore "./DAL_Empty/DAL_Empty.csproj"
RUN dotnet restore "./API/API.csproj"
RUN dotnet restore "./MVC/MVC.csproj"

# Copy toàn bộ source code (Bao gồm file Migration mới và code C# đã sửa)
COPY . .



# Build và publish
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

# Expose ports
EXPOSE 8080 8081

# ENV Google OAuth
ENV GoogleKeys__ClientId="61253447531-7vpfhr4i45dcac1h9k6f0np2l6q89hmi.apps.googleusercontent.com"
ENV GoogleKeys__ClientSecret="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"
ENV JwtSettings__SecretKey="this_is_a_super_secure_key_1234567890"

# ENV Connection string (Đã đúng)
ENV ConnectionStrings__DefaultConnection="Server=stylezone-sql,1433;Database=StyleZoneDb;User Id=sa;Password=YourStrong@Passw0rd1!;TrustServerCertificate=True;"

# ENV URL API cho MVC (Đã đúng)
ENV ApiBaseUrl="http://stylezone-all:8081/api/"

# ===== SỬA LẠI HOÀN TOÀN ENTRYPOINT (Sửa Lỗi A - Port & Lỗi C - CSS 404) =====
# Dùng 'cd' để đảm bảo ContentRootPath (thư mục gốc) là chính xác
ENTRYPOINT ["sh", "-c", "(cd /app/api && ASPNETCORE_URLS=http://+:8081 dotnet API.dll) & (echo 'Đang chờ 5 giây cho API khởi động...' && sleep 5 && cd /app/mvc && ASPNETCORE_URLS=http://+:8080 dotnet MVC.dll) && wait"]