# Template NuGet - Aide Rapide

## 🚀 Installation et Utilisation

### Installation du Template

```bash
# Installer localement (développement)
dotnet new install /Users/manher/Projects/Miccore/sample_project

# Ou depuis le package (si créé)
dotnet new install ./Miccore.CleanArchitecture.Template.1.0.0.nupkg

# Ou depuis NuGet.org (après publication)
dotnet new install Miccore.CleanArchitecture.Template
```

### Créer un Nouveau Projet

```bash
# Basique (MySQL par défaut)
dotnet new miccore-clean -n MonProjet

# Avec nom composé (recommandé)
dotnet new miccore-clean -n Acme.Ecommerce.Catalog

# Avec PostgreSQL
dotnet new miccore-clean -n MonProjet --databaseProvider PostgreSQL

# Avec SQL Server
dotnet new miccore-clean -n MonProjet --databaseProvider SqlServer

# Sans tests
dotnet new miccore-clean -n MonProjet --includeTests false

# Sans Docker
dotnet new miccore-clean -n MonProjet --includeDocker false

# Combinaison
dotnet new miccore-clean -n Contoso.Crm.Api --databaseProvider PostgreSQL --includeTests false --includeDocker false
```

### Aide

```bash
# Voir tous les paramètres disponibles
dotnet new miccore-clean --help

# Lister tous les templates installés
dotnet new list

# Rechercher le template
dotnet new list | grep miccore
```

### Désinstallation

```bash
# Désinstaller le template
dotnet new uninstall Miccore.CleanArchitecture.Template

# Ou si installé localement
dotnet new uninstall /Users/manher/Projects/Miccore/sample_project
```

## 📦 Création et Publication

Voir le guide complet dans [TEMPLATE_PUBLISHING.md](TEMPLATE_PUBLISHING.md)

### Résumé Rapide

```bash
# 1. Installer NuGet (si nécessaire)
brew install nuget  # macOS
# ou télécharger depuis https://www.nuget.org/downloads

# 2. Créer le package
cd /Users/manher/Projects/Miccore/sample_project
nuget pack Miccore.CleanArchitecture.Template.nuspec

# 3. Tester localement
dotnet new install ./Miccore.CleanArchitecture.Template.1.0.0.nupkg

# 4. Publier sur NuGet.org
nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY
```

## 📝 Paramètres Disponibles

| Paramètre | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `-n\|--name` | string | (requis) | Nom du projet (ex: `MonProjet` ou `Acme.Service.Api`) |
| `--databaseProvider` | choice | MySQL | Provider de BDD : `MySQL`, `PostgreSQL`, `SqlServer` |
| `--includeTests` | bool | true | Inclure les projets de tests |
| `--includeDocker` | bool | true | Inclure Dockerfile et .dockerignore |

## 🗂 Structure Générée

```
MonProjet/
├── src/
│   ├── MonProjet.Api/           # API REST (FastEndpoints)
│   ├── MonProjet.Application/   # Logique métier (CQRS, MediatR)
│   ├── MonProjet.Core/          # Domaine (Entities, Interfaces)
│   └── MonProjet.Infrastructure/ # Implémentation (EF Core, Cache)
├── test/ (si includeTests=true)
│   ├── MonProjet.Api.Tests/
│   ├── MonProjet.Application.Tests/
│   ├── MonProjet.Core.Tests/
│   └── MonProjet.Infrastructure.Tests/
├── Dockerfile (si includeDocker=true)
├── .dockerignore
├── MonProjet.sln
├── global.json
├── Directory.Build.props
└── ReadMe.md
```

## 🔧 Après Génération

### 1. Configurer la Base de Données

Éditer `src/MonProjet.Api/appsettings.json` :

```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 3306,  // 5432 pour PostgreSQL, 1433 pour SQL Server
    "Database": "ma_base",
    "UserId": "utilisateur",
    "Password": "mot_de_passe"
  }
}
```

Voir [DatabaseProviderConfiguration.md](src/MonProjet.Infrastructure/Persistances/DatabaseProviderConfiguration.md) pour plus de détails.

### 2. Créer et Appliquer les Migrations

```bash
# Créer la migration initiale
dotnet ef migrations add InitialCreate \
  --project src/MonProjet.Infrastructure \
  --startup-project src/MonProjet.Api

# Appliquer la migration
dotnet ef database update \
  --project src/MonProjet.Infrastructure \
  --startup-project src/MonProjet.Api
```

### 3. Compiler et Exécuter

```bash
# Restaurer les dépendances
dotnet restore

# Compiler
dotnet build

# Exécuter l'API
dotnet run --project src/MonProjet.Api

# Ou avec watch mode
dotnet watch run --project src/MonProjet.Api
```

### 4. Accéder à Swagger

En mode Development : `https://localhost:5001/swagger`

### 5. Exécuter les Tests (si inclus)

```bash
# Tous les tests
dotnet test

# Avec couverture
dotnet test /p:CollectCoverage=true
```

## 🏗 Technologies Incluses

- **.NET 10.0**
- **FastEndpoints** - Pattern REPR (Request-Endpoint-Response)
- **MediatR** - CQRS et mediator pattern
- **Entity Framework Core** - ORM
- **AutoMapper** - Mapping objet-objet
- **FluentValidation** - Validation
- **Serilog** - Logging structuré
- **xUnit** - Framework de tests (si includeTests=true)
- **Moq** - Mocking (si includeTests=true)
- **FluentAssertions** - Assertions expressives (si includeTests=true)

## 📚 Documentation

- **[ReadMe.md](ReadMe.md)** - Documentation complète du projet généré
- **[DatabaseProviderConfiguration.md](src/Miccore.Clean.Sample.Infrastructure/Persistances/DatabaseProviderConfiguration.md)** - Guide des providers de BDD
- **[TEMPLATE_PUBLISHING.md](TEMPLATE_PUBLISHING.md)** - Guide de publication du template

## 🔗 Liens Utiles

- [Documentation des templates .NET](https://learn.microsoft.com/dotnet/core/tools/custom-templates)
- [FastEndpoints](https://fast-endpoints.com/)
- [MediatR](https://github.com/jbogard/MediatR)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 📧 Support

Pour toute question ou problème :
- Ouvrir une issue sur GitHub
- Consulter la documentation du template
- Vérifier les exemples de configuration
