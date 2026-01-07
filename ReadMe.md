# Miccore.Clean.Sample

<!--#if (useMySql)-->
> 🚀 Clean Architecture microservice template with **MySQL** database provider
<!--#endif-->
<!--#if (usePostgreSql)-->
> 🚀 Clean Architecture microservice template with **PostgreSQL** database provider
<!--#endif-->
<!--#if (useSqlServer)-->
> 🚀 Clean Architecture microservice template with **SQL Server** database provider
<!--#endif-->

Ce projet est basé sur le template **Miccore Clean Architecture** qui implémente les meilleures pratiques d'architecture logicielle pour les microservices .NET.

## 📦 Installation du Template

Si vous souhaitez créer de nouveaux projets à partir de ce template :

```bash
# Installer le template depuis NuGet
dotnet new install Miccore.Net.CleanArchitectureTemplate

# Créer un nouveau projet (nom simple)
dotnet new miccore-clean -n MonProjet

# Créer un nouveau projet (nom composé - recommandé)
dotnet new miccore-clean -n Acme.Ecommerce.Catalog

# Avec PostgreSQL au lieu de MySQL
dotnet new miccore-clean -n MonProjet --databaseProvider PostgreSQL

# Voir tous les paramètres disponibles
dotnet new miccore-clean --help
```

### Paramètres Disponibles

| Paramètre | Type | Valeur par défaut | Description |
|-----------|------|-------------------|-------------|
| `--databaseProvider` | choice | MySQL | Fournisseur de base de données : `MySQL`, `PostgreSQL`, ou `SqlServer` |

### Recommandations de Nommage

Pour les projets d'entreprise, nous recommandons d'utiliser un format en 3 parties :
- **Format** : `Company.Service.Component`
- **Exemples** :
  - `Acme.Ecommerce.Catalog`
  - `Contoso.Crm.Api`
  - `Fabrikam.Inventory.Service`

Les noms simples sont également acceptés (exemple : `MonProjet`), mais le format composé améliore l'organisation dans les grandes structures.

---

## 🏗 Vue d'ensemble

Le projet suit les principes de la **Clean Architecture** (Architecture Hexagonale / Onion Architecture) pour assurer une séparation claire des responsabilités, une testabilité accrue et une indépendance vis-à-vis des frameworks externes.

### Structure de la Solution

```
Miccore.Clean.Sample/
├── 📁 src/
│   ├── 📁 Miccore.Clean.Sample.Api/           # Point d'entrée REST API
│   │   ├── Configuration/                      # Config (Serilog, Swagger, DI)
│   │   ├── Endpoints/                          # FastEndpoints (REPR pattern)
│   │   │   └── BaseEndpoint.cs
│   │   ├── Features/
│   │   │   └── Samples/                        # Endpoints par feature
│   │   ├── Middleware/                         # Exception, CorrelationId
│   │   └── Program.cs
│   │
│   ├── 📁 Miccore.Clean.Sample.Application/   # Logique métier (Use Cases)
│   │   ├── Behaviors/                          # MediatR Pipelines
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── ValidationBehavior.cs
│   │   ├── Features/
│   │   │   └── Samples/
│   │   │       ├── Commands/                   # Create, Update, Delete
│   │   │       │   └── CreateSample/
│   │   │       │       ├── CreateSampleCommand.cs
│   │   │       │       ├── CreateSampleCommandHandler.cs
│   │   │       │       └── CreateSampleValidator.cs
│   │   │       ├── Queries/                    # Get, GetAll
│   │   │       │   └── GetAllSamples/
│   │   │       │       ├── GetAllSamplesQuery.cs
│   │   │       │       └── GetAllSamplesQueryHandler.cs
│   │   │       ├── Mappers/
│   │   │       └── Responses/
│   │   └── Handlers/                           # Base handlers (Command/Query)
│   │
│   ├── 📁 Miccore.Clean.Sample.Core/          # Domaine (aucune dépendance)
│   │   ├── ApiModels/                          # ApiResponse<T>, ApiError
│   │   ├── Entities/                           # BaseEntity, SampleEntity
│   │   ├── Exceptions/                         # NotFoundException, ValidatorException
│   │   ├── Interfaces/                         # IUnitOfWork, ICacheService
│   │   └── Repositories/
│   │       ├── Base/
│   │       │   ├── IReadOnlyRepository.cs      # Queries (ISP)
│   │       │   └── IBaseRepository.cs          # Commands (hérite IReadOnlyRepository)
│   │       └── ISampleRepository.cs
│   │
│   └── 📁 Miccore.Clean.Sample.Infrastructure/ # Implémentation technique
│       ├── Caching/
│       │   ├── MemoryCacheService.cs
│       │   └── CachedRepositoryDecorator.cs    # Decorator Pattern
│       ├── Persistances/
│       │   ├── SampleApplicationDbContext.cs
│       │   └── UnitOfWork.cs
│       └── Repositories/
│           ├── Base/
│           │   └── BaseRepository.cs
│           └── SampleRepository.cs
│
├── 📁 test/
│   ├── Miccore.Clean.Sample.Api.Tests/
│   ├── Miccore.Clean.Sample.Application.Tests/
│   ├── Miccore.Clean.Sample.Core.Tests/
│   └── Miccore.Clean.Sample.Infrastructure.Tests/
│
├── 📁 .github/
│   ├── workflows/
│   │   ├── ci.yml                              # Build, Test, Code Quality, Security
│   │   ├── pr-check.yml                        # PR validation + Auto-labeling
│   │   └── dependency-review.yml
│   └── labeler.yml
│
├── .editorconfig
├── Directory.Build.props
├── Dockerfile
└── Miccore.Clean.Sample.sln
```

La solution est divisée en 4 couches principales :

1.  **Core** (`Miccore.Clean.Sample.Core`) : Le cœur du domaine.
2.  **Application** (`Miccore.Clean.Sample.Application`) : La logique métier et les cas d'utilisation.
3.  **Infrastructure** (`Miccore.Clean.Sample.Infrastructure`) : L'implémentation technique (BDD, Cache, etc.).
4.  **Api** (`Miccore.Clean.Sample.Api`) : Le point d'entrée de l'application (REST API).

---

## 🧩 Détail des Couches

### 1. Core (Domaine)
Cette couche ne dépend d'aucun autre projet. Elle contient :
-   **Entities** : Les objets métier persistants (ex: `SampleEntity`).
-   **Interfaces** : Les contrats pour les repositories et services.
    -   `IReadOnlyRepository<T>` : Opérations de lecture seule (ISP).
    -   `IBaseRepository<T>` : Opérations CRUD (hérite de `IReadOnlyRepository`).
    -   `IUnitOfWork` : Gestion des transactions.
    -   `ICacheService` : Abstraction du cache.
-   **Exceptions** : Les exceptions personnalisées (`NotFoundException`, `ValidatorException`).
-   **ApiModels** : Les modèles de réponse standardisés (`ApiResponse<T>`, `ApiError`).

### 2. Application (Use Cases)
Cette couche orchestre la logique métier. Elle dépend de `Core`.
-   **Pattern CQRS** : Séparation des lectures (Queries) et écritures (Commands) via **MediatR**.
    -   Les **Queries** injectent `IReadOnlyRepository<T>` (lecture seule).
    -   Les **Commands** injectent les repositories spécifiques + `IUnitOfWork`.
-   **Features** : Organisation verticale par fonctionnalité (ex: `Features/Samples/Commands/CreateSample`).
-   **Behaviors** : Pipelines transversaux pour MediatR :
    -   `ValidationBehavior` : Valide automatiquement les requêtes via FluentValidation.
    -   `LoggingBehavior` : Loggue les entrées/sorties et les performances.
-   **Handlers** : Classes de base `BaseCommandHandler` et `BaseQueryHandler` pour standardiser le traitement.
-   **Mappers** : Configuration AutoMapper pour la transformation Entité <-> DTO.

### 3. Infrastructure
Cette couche implémente les interfaces définies dans `Core`. Elle dépend de `Core`.
-   **Persistance** : Entity Framework Core avec `SampleApplicationDbContext`.
-   **Unit of Work** : `UnitOfWork` gère les transactions et expose `SaveChangesAsync`.
-   **Repositories** :
    -   `BaseRepository<T>` : Implémentation générique CRUD (implémente `IBaseRepository<T>`).
    -   `SampleRepository` : Implémentation spécifique.
-   **Caching** :
    -   `MemoryCacheService` : Wrapper autour de IMemoryCache.
    -   `CachedRepositoryDecorator<T>` : Implémente le pattern **Decorator** pour ajouter du cache (Cache-Aside) de manière transparente aux repositories.

### 4. Api (Présentation)
Le point d'entrée HTTP. Elle dépend de `Application` et `Infrastructure`.
-   **FastEndpoints** : Utilisation du pattern **REPR** (Request-Endpoint-Response) au lieu des contrôleurs MVC classiques. Chaque endpoint est une classe dédiée.
-   **Middleware** :
    -   `ExceptionHandlingMiddleware` : Capture globale des erreurs et formatage en `ProblemDetails`.
    -   `CorrelationIdMiddleware` : Ajoute un ID unique à chaque requête pour le traçage (Log Context).
-   **Configuration** : Configuration centralisée (Serilog, Swagger, DI).

---

## 🔄 Flux d'une Requête (Request Flow)

Prenons l'exemple d'une création (`CreateSample`) :

1.  **Client HTTP** : Envoie une requête `POST /api/samples`.
2.  **Middleware** :
    -   `CorrelationIdMiddleware` génère un ID de trace.
    -   `ExceptionHandlingMiddleware` enveloppe l'exécution.
3.  **Endpoint (Api)** : `CreateSampleEndpoint` reçoit la requête (`CreateSampleRequest`).
4.  **Mapping** : L'endpoint mappe la requête en commande `CreateSampleCommand`.
5.  **MediatR (Application)** : Envoie la commande.
6.  **Pipeline Behaviors** :
    -   `LoggingBehavior` loggue le début.
    -   `ValidationBehavior` exécute `CreateSampleValidator`. Si invalide -> `ValidatorException`.
7.  **Handler (Application)** : `CreateSampleCommandHandler` traite la commande.
    -   Appelle `ISampleRepository.AddAsync`.
    -   Appelle `IUnitOfWork.SaveChangesAsync` pour persister.
    -   Mappe l'entité créée en `SampleResponse`.
8.  **Repository (Infrastructure)** : `SampleRepository` (via `BaseRepository`) prépare l'entité pour EF Core.
9.  **Unit of Work** : Persiste les changements en BDD via `SaveChangesAsync`.
10. **Réponse** : Le résultat remonte la chaîne et est renvoyé au client en JSON standardisé.

---

## 🛠 Patterns Clés

### CQRS (Command Query Responsibility Segregation)
-   **Commands** : Modifient l'état (Create, Update, Delete). Utilisent `IUnitOfWork` pour persister.
-   **Queries** : Lisent l'état (Get, List). Utilisent `IReadOnlyRepository<T>` (lecture seule).
-   Utilisation de `MediatR` pour découpler l'émetteur (Endpoint) du traitant (Handler).

### Unit of Work
-   Centralise la gestion des transactions.
-   Les repositories n'appellent plus `SaveChangesAsync` directement.
-   Permet de regrouper plusieurs opérations en une seule transaction.

### Interface Segregation (ISP)
-   `IReadOnlyRepository<T>` : Méthodes de lecture (`GetAllAsync`, `GetByIdAsync`, etc.).
-   `IBaseRepository<T>` : Hérite de `IReadOnlyRepository` + méthodes d'écriture (`AddAsync`, `UpdateAsync`, `DeleteAsync`).
-   Les Queries n'ont accès qu'aux méthodes de lecture, renforçant le pattern CQRS.

### Repository & Decorator
-   L'accès aux données est abstrait via `IReadOnlyRepository<T>` et `IBaseRepository<T>`.
-   Le **Decorator Pattern** (`CachedRepositoryDecorator`) permet d'ajouter du cache sans modifier le code métier ni le repository SQL.
    -   *Lecture* : Vérifie le cache -> Si absent, appelle la BDD -> Met en cache.
    -   *Écriture* : Écrit en BDD -> Invalide le cache associé.

### FastEndpoints
-   Remplace les Controllers.
-   Chaque endpoint définit sa requête (`Request`), sa réponse (`Response`) et sa méthode `HandleAsync`.
-   Favorise le principe de responsabilité unique (SRP).

### Gestion des Erreurs
-   Pas de `try/catch` dans les contrôleurs/endpoints.
-   Les exceptions typées (`NotFoundException`, `ValidatorException`) sont lancées par le Core/Application.
-   Le Middleware global les capture et retourne les codes HTTP appropriés (404, 400, 500).

---

## 🧪 Tests

Le projet inclut une suite de tests complète :

```bash
# Exécuter tous les tests
dotnet test

# Avec rapport de couverture
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Exécuter les tests d'un projet spécifique
dotnet test test/Miccore.Clean.Sample.Application.Tests
```

### Structure des Tests

- **Core.Tests** : Tests unitaires des entités, extensions et helpers
- **Application.Tests** : Tests des handlers, validateurs et mappings (avec mocks)
- **Infrastructure.Tests** : Tests des repositories et du cache
- **Api.Tests** : Tests des endpoints et middleware

Objectif de couverture : **> 70%**
<!--#endif-->

---

## 📚 Documentation Complémentaire

- **[Architecture détaillée](docs/architecture.md)** - Vue approfondie de l'architecture (si applicable)
- **[Configuration de la base de données](src/Miccore.Clean.Sample.Infrastructure/Persistances/DatabaseProviderConfiguration.md)** - Guide complet des providers supportés
- **[Patterns utilisés](#-patterns-clés)** - Voir ci-dessous

---

## 🚀 Démarrage

### Prérequis

- **.NET 10.0 SDK** : [Télécharger ici](https://dotnet.microsoft.com/download/dotnet/10.0)
<!--#if (useMySql)-->
- **MySQL/MariaDB** : Serveur de base de données (ou via Docker)
<!--#endif-->
<!--#if (usePostgreSql)-->
- **PostgreSQL** : Serveur de base de données (ou via Docker)
<!--#endif-->
<!--#if (useSqlServer)-->
- **SQL Server** : Serveur de base de données (ou SQL Server Express/Docker)
<!--#endif-->

### Configuration de la Base de Données

1. **Configurer la chaîne de connexion** dans `src/Miccore.Clean.Sample.Api/appsettings.json` :

<!--#if (useMySql)-->
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 3306,
    "Database": "votre_base_de_donnees",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
```
<!--#endif-->
<!--#if (usePostgreSql)-->
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 5432,
    "Database": "votre_base_de_donnees",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
```
<!--#endif-->
<!--#if (useSqlServer)-->
```json
{
  "DatabaseConfiguration": {
    "Server": "localhost",
    "Port": 1433,
    "Database": "votre_base_de_donnees",
    "UserId": "votre_utilisateur",
    "Password": "votre_mot_de_passe"
  }
}
```
<!--#endif-->

2. **Créer et appliquer les migrations** :

```bash
# Créer la première migration
dotnet ef migrations add InitialCreate --project src/Miccore.Clean.Sample.Infrastructure --startup-project src/Miccore.Clean.Sample.Api

# Appliquer la migration à la base de données
dotnet ef database update --project src/Miccore.Clean.Sample.Infrastructure --startup-project src/Miccore.Clean.Sample.Api
```

> 💡 **Note** : Consultez [DatabaseProviderConfiguration.md](src/Miccore.Clean.Sample.Infrastructure/Persistances/DatabaseProviderConfiguration.md) pour des exemples détaillés de configuration selon votre fournisseur de base de données, y compris les différences de comportement et les meilleures pratiques.

### Lancement de l'Application

```bash
# Restaurer les dépendances
dotnet restore

# Compiler la solution
dotnet build

# Lancer l'API
dotnet run --project src/Miccore.Clean.Sample.Api
```

L'API sera accessible à l'adresse : `https://localhost:5001` (ou le port configuré)

### Documentation Swagger

En mode **Development**, Swagger est accessible via :
- URL : `https://localhost:5001/swagger`
- Documentation interactive de tous les endpoints disponibles

### Démarrage avec Docker

```bash
# Construire l'image
docker build -t miccore-clean-sample .

# Lancer le conteneur
docker run -p 8080:8080 miccore-clean-sample
```

---

## ✅ Principes SOLID

Ce projet respecte les 5 principes SOLID :

| Principe | Application |
|----------|-------------|
| **SRP** | Un handler par commande/requête, un endpoint par action |
| **OCP** | Behaviors MediatR, Decorator pour le cache |
| **LSP** | Tous les repositories sont interchangeables via leurs interfaces |
| **ISP** | `IReadOnlyRepository` vs `IBaseRepository`, interfaces spécifiques par feature |
| **DIP** | Injection de dépendances partout, aucune dépendance concrète dans Application/Core |

---

## 🤝 Contribution

Ce projet est un template. Pour contribuer au template lui-même :

1. Fork le repository
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

---

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

---

## 🔗 Ressources

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)

---

**Généré avec le template Miccore Clean Architecture** 🚀
