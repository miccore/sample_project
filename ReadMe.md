# Documentation de l'Architecture du Projet

Ce document décrit l'architecture technique, les choix de conception et le fonctionnement du projet `Miccore.Clean.Sample`.

## 🏗 Vue d'ensemble

Le projet suit les principes de la **Clean Architecture** (Architecture Hexagonale / Onion Architecture) pour assurer une séparation claire des responsabilités, une testabilité accrue et une indépendance vis-à-vis des frameworks externes.

### Structure de la Solution

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
-   **Interfaces** : Les contrats pour les repositories (`ISampleRepository`) et services (`ICacheService`).
-   **Exceptions** : Les exceptions personnalisées (`NotFoundException`, `ValidatorException`).
-   **ApiModels** : Les modèles de réponse standardisés (`ApiResponse<T>`, `ApiError`).

### 2. Application (Use Cases)
Cette couche orchestre la logique métier. Elle dépend de `Core`.
-   **Pattern CQRS** : Séparation des lectures (Queries) et écritures (Commands) via **MediatR**.
-   **Features** : Organisation verticale par fonctionnalité (ex: `Features/Samples/Commands/CreateSample`).
-   **Behaviors** : Pipelines transversaux pour MediatR :
    -   `ValidationBehavior` : Valide automatiquement les requêtes via FluentValidation.
    -   `LoggingBehavior` : Loggue les entrées/sorties et les performances.
-   **Handlers** : Classes de base `BaseCommandHandler` et `BaseQueryHandler` pour standardiser le traitement.
-   **Mappers** : Configuration AutoMapper pour la transformation Entité <-> DTO.

### 3. Infrastructure
Cette couche implémente les interfaces définies dans `Core`. Elle dépend de `Core`.
-   **Persistance** : Entity Framework Core avec `SampleApplicationDbContext`.
-   **Repositories** :
    -   `BaseRepository<T>` : Implémentation générique CRUD.
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
    -   Mappe l'entité créée en `SampleResponse`.
8.  **Repository (Infrastructure)** : `SampleRepository` (via `BaseRepository`) enregistre en BDD via EF Core.
9.  **Réponse** : Le résultat remonte la chaîne et est renvoyé au client en JSON standardisé.

---

## 🛠 Patterns Clés

### CQRS (Command Query Responsibility Segregation)
-   **Commands** : Modifient l'état (Create, Update, Delete). Retournent souvent l'ID ou l'objet créé.
-   **Queries** : Lisent l'état (Get, List). Ne modifient jamais les données.
-   Utilisation de `MediatR` pour découpler l'émetteur (Endpoint) du traitant (Handler).

### Repository & Decorator
-   L'accès aux données est abstrait via `IBaseRepository<T>`.
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

## 🧪 Stratégie de Tests

Le projet contient une suite de tests complète dans le dossier `test/` :

-   **Unit Tests** :
    -   `Core.Tests` : Teste les entités, extensions et helpers.
    -   `Application.Tests` : Teste les Handlers, Validators et Mappings (Mock des repositories).
    -   `Infrastructure.Tests` : Teste les implémentations de cache et repositories (souvent avec une BDD en mémoire ou SQLite).
    -   `Api.Tests` : Teste les Endpoints, Middlewares et le Mapping HTTP.

-   **Couverture de Code** :
    -   Objectif : > 70%.
    -   Outils : `coverlet` et `ReportGenerator`.

---

## 🚀 Démarrage

1.  **Prérequis** : .NET 9.0 SDK.
2.  **Configuration** : Vérifier `appsettings.json` (ConnectionStrings).
3.  **Lancement** :
    ```bash
    dotnet run --project src/Miccore.Clean.Sample.Api
    ```
4.  **Swagger** : Accessible via `/swagger` (en environnement Development).
