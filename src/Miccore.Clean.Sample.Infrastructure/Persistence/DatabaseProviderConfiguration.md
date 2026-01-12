# Guide de Configuration des Fournisseurs de Base de Données

Ce guide fournit des exemples de configuration pour les trois fournisseurs de base de données pris en charge par ce template Clean Architecture.

## Vue d'ensemble

Le template supporte trois fournisseurs de base de données :
- **MySQL/MariaDB** (via Pomelo.EntityFrameworkCore.MySql)
- **PostgreSQL** (via Npgsql.EntityFrameworkCore.PostgreSQL)
- **SQL Server** (via Microsoft.EntityFrameworkCore.SqlServer)

## Configuration Actuelle

Le template a été généré avec : **<!--#if (useMySql)-->MySQL<!--#endif--><!--#if (usePostgreSql)-->PostgreSQL<!--#endif--><!--#if (useSqlServer)-->SQL Server<!--#endif-->**

---

## 1. Configuration MySQL/MariaDB

### Référence du Package
\`\`\`xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
\`\`\`

### Chaîne de Connexion (appsettings.json)
\`\`\`json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 3306,
    "Database": "nom_de_votre_base",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
\`\`\`

### Configuration du DbContext
\`\`\`csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
}
\`\`\`

### Commandes de Migration
\`\`\`bash
# Créer une migration
dotnet ef migrations add InitialCreate --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api

# Mettre à jour la base de données
dotnet ef database update --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api
\`\`\`

### Considérations Spécifiques à MySQL
- **Longueur des chaînes** : Longueur maximale par défaut de 255 caractères dans les index
- **Stockage GUID** : Stocké en CHAR(36) par défaut, utilisez \`.HasColumnType("binary(16)")\` pour de meilleures performances
- **Sensibilité à la casse** : Les noms de tables/colonnes sont sensibles à la casse sous Linux, insensibles sous Windows
- **Support JSON** : Type JSON natif disponible (MySQL 5.7.8+)
- **Performance** : Excellent pour les charges de travail à forte lecture, bonne performance générale
- **Recommandé pour** : Applications web, microservices, scénarios à forte lecture

---

## 2. Configuration PostgreSQL

### Référence du Package
\`\`\`xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
\`\`\`

### Chaîne de Connexion (appsettings.json)
\`\`\`json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 5432,
    "Database": "nom_de_votre_base",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
\`\`\`

### Configuration du DbContext
\`\`\`csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseNpgsql(connectionString);
}
\`\`\`

### Commandes de Migration
\`\`\`bash
# Créer une migration
dotnet ef migrations add InitialCreate --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api

# Mettre à jour la base de données
dotnet ef database update --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api
\`\`\`

### Considérations Spécifiques à PostgreSQL
- **Longueur des chaînes** : Pas de limite pratique sur la longueur des chaînes dans les index (utilisez le type text)
- **Stockage GUID** : Type UUID natif avec \`.HasColumnType("uuid")\` pour un stockage optimal
- **Sensibilité à la casse** : Les identifiants sont insensibles à la casse par défaut (convertis en minuscules)
- **Support JSON** : Excellent support JSONB avec indexation et capacités de requêtage
- **Performance** : Excellent pour les requêtes complexes, les charges d'écriture intensives et la conformité ACID
- **Fonctionnalités avancées** : Recherche plein texte, tableaux, hstore, types personnalisés, fonctions de fenêtrage
- **Recommandé pour** : Applications complexes, entrepôts de données, analyses, données géospatiales (PostGIS)

### Exemple de Configuration d'Entité Spécifique à PostgreSQL
\`\`\`csharp
modelBuilder.Entity<VotreEntite>(entity =>
{
    entity.Property(e => e.Id)
        .HasColumnType("uuid")
        .HasDefaultValueSql("gen_random_uuid()");
    
    entity.Property(e => e.Data)
        .HasColumnType("jsonb");
});
\`\`\`

---

## 3. Configuration SQL Server

### Référence du Package
\`\`\`xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
\`\`\`

### Chaîne de Connexion (appsettings.json)
\`\`\`json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 1433,
    "Database": "nom_de_votre_base",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
\`\`\`

**Alternative : Sécurité Intégrée (Authentification Windows)**
\`\`\`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=nom_de_votre_base;Integrated Security=true;TrustServerCertificate=true;"
  }
}
\`\`\`

### Configuration du DbContext
\`\`\`csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var dbConfig = new DatabaseConfiguration();
    _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
    
    var connectionString = dbConfig.GetConnectionString();
    optionsBuilder.UseSqlServer(connectionString);
}
\`\`\`

### Commandes de Migration
\`\`\`bash
# Créer une migration
dotnet ef migrations add InitialCreate --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api

# Mettre à jour la base de données
dotnet ef database update --project src/VotreProjet.Infrastructure --startup-project src/VotreProjet.Api
\`\`\`

### Considérations Spécifiques à SQL Server
- **Longueur des chaînes** : Maximum 900 octets dans les index (pour nvarchar, ~450 caractères)
- **Stockage GUID** : Type uniqueidentifier natif, performance optimale
- **Sensibilité à la casse** : Configurable via collation (par défaut : insensible à la casse)
- **Support JSON** : Fonctions JSON disponibles (SQL Server 2016+)
- **Performance** : Excellente performance entreprise, idéal pour les environnements Windows
- **Fonctionnalités avancées** : Tables temporelles, OLTP en mémoire, index columnstore
- **Licence** : Nécessite une licence commerciale pour la production (sauf éditions Express/Developer)
- **Recommandé pour** : Applications d'entreprise, environnements Windows, applications .NET Framework héritées

### Exemple de Configuration d'Entité Spécifique à SQL Server
\`\`\`csharp
modelBuilder.Entity<VotreEntite>(entity =>
{
    entity.Property(e => e.Id)
        .HasDefaultValueSql("NEWID()");
    
    entity.Property(e => e.Name)
        .HasMaxLength(450) // Important pour les index
        .IsUnicode(true);
});
\`\`\`

---

## Matrice de Comparaison

| Fonctionnalité | MySQL/MariaDB | PostgreSQL | SQL Server |
|----------------|---------------|------------|------------|
| **Open Source** | ✅ Oui | ✅ Oui | ❌ Non (Express/Dev uniquement) |
| **Performance (Lecture)** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Performance (Écriture)** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Support JSON** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Recherche Plein Texte** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Fonctionnalités Avancées** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Conformité ACID** | ⭐⭐⭐⭐ (InnoDB) | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Facilité d'Installation** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Support Cloud** | Azure, AWS, GCP | Azure, AWS, GCP | Azure (natif) |
| **Multi-Plateforme** | ✅ Oui | ✅ Oui | ✅ Oui (2016+) |

---

## Changement de Fournisseur de Base de Données

Si vous devez changer de fournisseur de base de données après la création du projet :

1. **Mettre à jour la référence du package** dans \`Infrastructure.csproj\`
2. **Modifier la configuration du DbContext** dans \`SampleApplicationDbContext.cs\` (méthode OnConfiguring)
3. **Mettre à jour la chaîne de connexion** dans \`appsettings.json\`
4. **Supprimer le dossier Migrations existant** (si présent)
5. **Créer une nouvelle migration initiale**
   \`\`\`bash
   dotnet ef migrations add InitialCreate --project src/VotreProjet.Infrastructure
   \`\`\`
6. **Appliquer la migration**
   \`\`\`bash
   dotnet ef database update --project src/VotreProjet.Infrastructure
   \`\`\`

---

## Support Docker

### MySQL
\`\`\`yaml
services:
  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: votre_mot_de_passe
      MYSQL_DATABASE: votre_base
    ports:
      - "3306:3306"
\`\`\`

### PostgreSQL
\`\`\`yaml
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: votre_mot_de_passe
      POSTGRES_DB: votre_base
    ports:
      - "5432:5432"
\`\`\`

### SQL Server
\`\`\`yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: VotreMotDePasse@Fort123
    ports:
      - "1433:1433"
\`\`\`

---

## Ressources Supplémentaires

- [Documentation MySQL](https://dev.mysql.com/doc/)
- [Documentation PostgreSQL](https://www.postgresql.org/docs/)
- [Documentation SQL Server](https://learn.microsoft.com/fr-fr/sql/sql-server/)
- [Entity Framework Core](https://learn.microsoft.com/fr-fr/ef/core/)
