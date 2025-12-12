# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY Directory.Build.props .
COPY src/Miccore.Clean.Sample.Api/*.csproj ./src/Miccore.Clean.Sample.Api/
COPY src/Miccore.Clean.Sample.Application/*.csproj ./src/Miccore.Clean.Sample.Application/
COPY src/Miccore.Clean.Sample.Core/*.csproj ./src/Miccore.Clean.Sample.Core/
COPY src/Miccore.Clean.Sample.Infrastructure/*.csproj ./src/Miccore.Clean.Sample.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ ./src/

# Build and publish
RUN dotnet publish src/Miccore.Clean.Sample.Api/Miccore.Clean.Sample.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" --uid 1000 appuser
USER appuser

# Copy published output
COPY --from=build /app/publish .

# Configure environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Miccore.Clean.Sample.Api.dll"]
