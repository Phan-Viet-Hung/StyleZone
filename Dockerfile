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

# Phục hồi các package (dependencies)
RUN dotnet restore "./MVC/MVC.csproj"

# Bây giờ mới copy toàn bộ mã nguồn còn lại
COPY . .

# Xóa các tệp appsettings.json gây xung đột trước khi publish
RUN rm -f ./DAL_Empty/appsettings.json ./DAL_Empty/appsettings.Development.json
RUN rm -f ./API/appsettings.json ./API/appsettings.Development.json
RUN rm -f ./MVC/appsettings.json ./MVC/appsettings.Development.json

# Build và publish dự án MVC
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
