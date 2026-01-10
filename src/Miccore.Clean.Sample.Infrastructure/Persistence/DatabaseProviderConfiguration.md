# Database Provider Configuration Guide

This guide provides configuration examples for the three supported database providers in this Clean Architecture template.

## Overview

The template supports three database providers:
- **MySQL/MariaDB** (via Pomelo.EntityFrameworkCore.MySql)
- **PostgreSQL** (via Npgsql.EntityFrameworkCore.PostgreSQL)
- **SQL Server** (via Microsoft.EntityFrameworkCore.SqlServer)

## Current Configuration

The template has been generated with: **<!--#if (useMySql)-->MySQL<!--#endif--><!--#if (usePostgreSql)-->PostgreSQL<!--#endif--><!--#if (useSqlServer)-->SQL Server<!--#endif-->**

---

## 1. MySQL/MariaDB Configuration

### Package Reference
```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
```

### Connection String (appsettings.json)
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 3306,
    "Database": "your_database_name",
    "UserId": "your_username",
    "Password": "your_password"
  }
}
```

### DbContext Configuration
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
}
```

### Migration Commands
```bash
# Create migration
dotnet ef migrations add InitialCreate --project src/YourProject.Infrastructure --startup-project src/YourProject.Api

# Update database
dotnet ef database update --project src/YourProject.Infrastructure --startup-project src/YourProject.Api
```

### MySQL-Specific Considerations
- **String Length**: Default max length is 255 characters in indexes
- **GUID Storage**: Stored as CHAR(36) by default, use `.HasColumnType("binary(16)")` for better performance
- **Case Sensitivity**: Table/column names are case-sensitive on Linux, case-insensitive on Windows
- **JSON Support**: Native JSON type available (MySQL 5.7.8+)
- **Performance**: Excellent for read-heavy workloads, good general performance
- **Recommended For**: Web applications, microservices, high-read scenarios

---

## 2. PostgreSQL Configuration

### Package Reference
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

### Connection String (appsettings.json)
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 5432,
    "Database": "your_database_name",
    "UserId": "your_username",
    "Password": "your_password"
  }
}
```

### DbContext Configuration
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseNpgsql(connectionString);
}
```

### Migration Commands
```bash
# Create migration
dotnet ef migrations add InitialCreate --project src/YourProject.Infrastructure --startup-project src/YourProject.Api

# Update database
dotnet ef database update --project src/YourProject.Infrastructure --startup-project src/YourProject.Api
```

### PostgreSQL-Specific Considerations
- **String Length**: No practical limit on string length in indexes (use text type)
- **GUID Storage**: Native UUID type with `.HasColumnType("uuid")` for optimal storage
- **Case Sensitivity**: Identifiers are case-insensitive by default (converted to lowercase)
- **JSON Support**: Excellent JSONB support with indexing and querying capabilities
- **Performance**: Excellent for complex queries, write-heavy workloads, and ACID compliance
- **Advanced Features**: Full-text search, arrays, hstore, custom types, window functions
- **Recommended For**: Complex applications, data warehousing, analytics, geospatial data (PostGIS)

### PostgreSQL-Specific Entity Configuration Example
```csharp
modelBuilder.Entity<YourEntity>(entity =>
{
    entity.Property(e => e.Id)
        .HasColumnType("uuid")
        .HasDefaultValueSql("gen_random_uuid()");
    
    entity.Property(e => e.Data)
        .HasColumnType("jsonb");
});
```

---

## 3. SQL Server Configuration

### Package Reference
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
```

### Connection String (appsettings.json)
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 1433,
    "Database": "your_database_name",
    "UserId": "your_username",
    "Password": "your_password"
  }
}
```

**Alternative: Integrated Security (Windows Authentication)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=your_database_name;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

### DbContext Configuration
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseSqlServer(connectionString);
}
```

### Migration Commands
```bash
# Create migration
dotnet ef migrations add InitialCreate --project src/YourProject.Infrastructure --startup-project src/YourProject.Api

# Update database
dotnet ef database update --project src/YourProject.Infrastructure --startup-project src/YourProject.Api
```

### SQL Server-Specific Considerations
- **String Length**: Max 900 bytes in indexes (for nvarchar, ~450 characters)
- **GUID Storage**: Native uniqueidentifier type, optimal performance
- **Case Sensitivity**: Configurable via collation (default: case-insensitive)
- **JSON Support**: JSON functions available (SQL Server 2016+)
- **Performance**: Excellent enterprise performance, great for Windows environments
- **Advanced Features**: Temporal tables, in-memory OLTP, columnstore indexes
- **Licensing**: Requires commercial license for production (except Express/Developer editions)
- **Recommended For**: Enterprise applications, Windows-centric environments, .NET Framework legacy apps

### SQL Server-Specific Entity Configuration Example
```csharp
modelBuilder.Entity<YourEntity>(entity =>
{
    entity.Property(e => e.Id)
        .HasDefaultValueSql("NEWID()");
    
    entity.Property(e => e.Name)
        .HasMaxLength(450) // Important for indexes
        .IsUnicode(true);
});
```

---

## Comparison Matrix

| Feature | MySQL/MariaDB | PostgreSQL | SQL Server |
|---------|---------------|------------|------------|
| **Open Source** | ✅ Yes | ✅ Yes | ❌ No (Express/Dev only) |
| **Performance (Read)** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Performance (Write)** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **JSON Support** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Full-Text Search** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Advanced Features** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **ACID Compliance** | ⭐⭐⭐⭐ (InnoDB) | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Ease of Setup** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Cloud Support** | Azure, AWS, GCP | Azure, AWS, GCP | Azure (native) |
| **Cross-Platform** | ✅ Yes | ✅ Yes | ✅ Yes (2016+) |

---

## Switching Database Providers

If you need to switch to a different database provider after project creation:

1. **Update Package Reference** in `Infrastructure.csproj`
2. **Modify DbContext Configuration** in `SampleApplicationDbContext.cs` (OnConfiguring method)
3. **Update Connection String** in `appsettings.json`
4. **Delete Existing Migrations** folder (if any)
5. **Create New Initial Migration**
   ```bash
   dotnet ef migrations add InitialCreate --project src/YourProject.Infrastructure
   ```
6. **Apply Migration**
   ```bash
   dotnet ef database update --project src/YourProject.Infrastructure
   ```

---

## Docker Support

### MySQL
```yaml
services:
  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: your_password
      MYSQL_DATABASE: your_database
    ports:
      - "3306:3306"
```

### PostgreSQL
```yaml
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: your_password
      POSTGRES_DB: your_database
    ports:
      - "5432:5432"
```

### SQL Server
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourStrong@Passw0rd
    ports:
      - "1433:1433"
```

---

## Additional Resources

- [MySQL Documentation](https://dev.mysql.com/doc/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [SQL Server Documentation](https://learn.microsoft.com/en-us/sql/sql-server/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
