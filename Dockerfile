# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy các file solution và các project cần thiết
COPY ./Empty.sln ./
COPY ./MVC ./MVC
COPY ./API ./API
COPY ./DAL_Empty ./DAL_Empty

# Restore dependencies cho solution
RUN dotnet restore "./Empty.sln"

# Build project MVC
RUN dotnet publish "./MVC/MVC.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MVC.dll"]
