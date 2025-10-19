# ===========================================
# STAGE 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Chỉ copy các file project (.csproj) và solution (.sln) trước
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# Phục hồi các package (dependencies). Bước này sẽ được Docker cache lại
# nếu các file .csproj không thay đổi, giúp build nhanh hơn.
RUN dotnet restore "./MVC/MVC.csproj"

# Bây giờ mới copy toàn bộ mã nguồn còn lại
COPY . .

# Build và publish dự án MVC. Cờ --no-restore được dùng đúng ở đây
# vì việc restore đã được thực hiện ở một layer trước đó.
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/publish --no-restore

# ===========================================
# STAGE 2: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Thêm các biến môi trường cấu hình OAuth / ClientId
ENV Authentication__Google__ClientId="61253447531-7vpfhr4i45dcac1h9k6f0np2l6q89hmi.apps.googleusercontent.com"
ENV Authentication__Google__ClientSecret="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"

ENTRYPOINT ["dotnet", "MVC.dll"]
