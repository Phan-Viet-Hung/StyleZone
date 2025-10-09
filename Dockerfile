# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ./Empty.sln ./

# Copy từng project cần thiết
COPY ./DAL_Empty/DAL_Empty.csproj ./DAL_Empty/
COPY ./API/API.csproj ./API/
COPY ./MVC/MVC.csproj ./MVC/

# Restore dependencies
RUN dotnet restore "Empty.sln"

# Copy toàn bộ source code (sau khi restore xong)
COPY . .

# Build và publish chỉ MVC
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/publish /p:GenerateRuntimeConfigurationFiles=true /p:CopyOutputSymbolsToPublishDirectory=false

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MVC.dll"]
