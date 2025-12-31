# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["MyNumberNET/MyNumberNET.csproj", "MyNumberNET/"]
COPY ["MyNumberNET_ApiServer/MyNumberNET_ApiServer.csproj", "MyNumberNET_ApiServer/"]
COPY ["Directory.Build.props", "."]
COPY ["global.json", "."]

# Restore dependencies
RUN dotnet restore "MyNumberNET_ApiServer/MyNumberNET_ApiServer.csproj"

# Copy source files
COPY ["MyNumberNET/", "MyNumberNET/"]
COPY ["MyNumberNET_ApiServer/", "MyNumberNET_ApiServer/"]

# Build application
RUN dotnet build "MyNumberNET_ApiServer/MyNumberNET_ApiServer.csproj" -c Release --no-restore

# Publish application
RUN dotnet publish "MyNumberNET_ApiServer/MyNumberNET_ApiServer.csproj" -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD dotnet /app/MyNumberNET_ApiServer.dll || exit 1

# Start application
ENTRYPOINT ["dotnet", "MyNumberNET_ApiServer.dll"]
