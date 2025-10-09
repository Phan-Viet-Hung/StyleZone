# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ source code
COPY . .

# Restore dependencies cho solution
RUN dotnet restore "./Empty.sln"

# Build project MVC (có thể đổi tên nếu khác)
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MVC.dll"]
