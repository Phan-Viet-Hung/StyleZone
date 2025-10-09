# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution và project files trước
COPY ./Empty.sln ./
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# Restore chỉ project MVC (không restore toàn bộ solution)
RUN dotnet restore "./MVC/MVC.csproj"

# Copy toàn bộ source code
COPY . .

# Publish chỉ MVC, loại bỏ các file appsettings.* trùng lặp
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/publish --no-restore \
    /p:ExcludeAppSettingsFiles=true \
    /p:CopyOutputSymbolsToPublishDirectory=false \
    /p:ErrorOnDuplicatePublishOutputFiles=false

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MVC.dll"]
