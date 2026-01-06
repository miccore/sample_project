# Changelog - Transformation en Template NuGet

Toutes les modifications notables de ce template seront documentées dans ce fichier.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/).

## [Unreleased]

### Added
- 🚀 **Workflow GitHub Actions** pour publication automatique sur NuGet.org
  - Fichier `.github/workflows/publish-template.yml`
  - Validation automatique avant publication (3 scénarios de tests)
  - Support des tags de version (ex: `git tag v1.0.0`)
  - Support du déclenchement manuel depuis l'interface GitHub
  - Création automatique de GitHub Releases avec notes de version
  - Mise à jour automatique de la version dans le .nuspec
  - Upload du package comme artifact (90 jours de rétention)
- ⭐ **Workflow Manuel Avancé** pour publication avec options complètes
  - Fichier `.github/workflows/publish-manual.yml`
  - **Options configurables** :
    - `version` : Version du package (support versions beta/rc/alpha)
    - `skip-validation` : Ignorer les tests de validation (publication rapide)
    - `create-release` : Créer ou non une GitHub Release
    - `update-nuspec` : Mettre à jour automatiquement le fichier .nuspec
  - Validation du format de version (Semantic Versioning)
  - Vérification si la version existe déjà sur NuGet.org
  - Support des pre-releases (versions avec suffixe)
  - Résumé détaillé de la publication dans GitHub Actions
  - Cas d'usage : publications beta, tests rapides, republications
- 📚 **Documentation du workflow** dans `.github/workflows/README.md`
  - Guide complet de configuration (clé API NuGet)
  - Instructions d'utilisation (tags et manuel)
  - Comparaison des deux workflows
  - Exemples de cas d'usage
  - Guide de dépannage
  - Convention de versioning

### Changed
- 📖 **TEMPLATE_PUBLISHING.md** mis à jour avec section GitHub Actions
- 📖 **TEMPLATE_SUCCESS.md** mis à jour avec instructions GitHub Actions
- 📖 **GITHUB_ACTIONS_QUICKSTART.md** mis à jour avec workflow manuel
  - Ajout section "Méthode 2 : Publication Manuelle avec Options"
  - Tableau comparatif des workflows
  - Exemples d'utilisation pour chaque scénario

---

## Version 1.0.0 - Template Initial

### ✨ Nouvelles Fonctionnalités

#### Configuration du Template
- **Fichiers de configuration créés** :
  - `.template.config/template.json` - Configuration principale du template
  - `.template.config/ide.host.json` - Configuration pour Visual Studio
  - `.template.config/.templateignore` - Exclusions de fichiers
  - `.template.config/icon.png` - Placeholder pour l'icône (à remplacer)

#### Paramètres du Template
- **`--databaseProvider`** (choice, default: MySQL)
  - MySQL - Pomelo.EntityFrameworkCore.MySql 9.0.0
  - PostgreSQL - Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0
  - SqlServer - Microsoft.EntityFrameworkCore.SqlServer 10.0.1
  
- **`--includeTests`** (bool, default: true)
  - Inclure/exclure les 4 projets de tests
  
- **`--includeDocker`** (bool, default: true)
  - Inclure/exclure Dockerfile et .dockerignore

#### Remplacement de Noms
- **sourceName** : `Miccore.Clean.Sample`
  - Remplacé automatiquement dans tous les fichiers
  - Renommage des fichiers et dossiers contenant le sourceName
  - Support des noms simples (`MonProjet`) et composés (`Acme.Service.Api`)

- **Protection des packages** :
  - `Miccore.Net.*` packages préservés (non remplacés)
  - `Miccore.SharedKernel.*` packages préservés (non remplacés)

#### Exclusions
- **Toujours exclus** :
  - `.github/**/*` - Workflows GitHub Actions
  - `**/bin/**`, `**/obj/**` - Dossiers de build
  - `**/.vs/**`, `**/.vscode/**` - Fichiers IDE
  - `**/logs/**`, `**/*.user` - Logs et fichiers utilisateur
  - `**/.git/**` - Historique Git

- **Conditionnellement exclus** :
  - `test/**/*` si `--includeTests false`
  - `Dockerfile`, `.dockerignore` si `--includeDocker false`

#### GUIDs Régénérés
11 GUIDs de la solution sont automatiquement régénérés pour éviter les conflits :
- 2 Solution Folders (src, test)
- 4 Projets src
- 4 Projets test
- 1 Solution GUID

### 📝 Documentation Créée

#### Fichiers Principaux
- **`ReadMe.md`** - Mis à jour avec :
  - Instructions d'installation du template
  - Documentation des paramètres
  - Guide de démarrage rapide
  - Sections conditionnelles selon la configuration choisie
  - Recommandations de nommage

- **`DatabaseProviderConfiguration.md`** - Guide complet :
  - Configuration détaillée pour MySQL, PostgreSQL, SQL Server
  - Exemples de connection strings
  - Configuration DbContext pour chaque provider
  - Commandes de migration EF Core
  - Matrice de comparaison des providers
  - Considérations spécifiques (types de données, performance, syntaxe)
  - Exemples Docker Compose

#### Guides de Template
- **`TEMPLATE_QUICK_START.md`** - Aide rapide :
  - Installation et utilisation
  - Exemples de commandes
  - Paramètres disponibles
  - Structure générée
  - Étapes post-génération

- **`TEMPLATE_PUBLISHING.md`** - Guide de publication :
  - Test local sans package
  - Création du package NuGet
  - Publication sur NuGet.org
  - Feeds privés (Azure Artifacts, GitHub Packages)
  - Checklist de publication
  - Dépannage

#### Scripts
- **`create-template-package.sh`** - Script de packaging :
  - Copie des fichiers avec exclusions
  - Structure de package
  - Instructions d'utilisation

### 🔧 Modifications du Code Source

#### Infrastructure Layer
- **`Miccore.Clean.Sample.Infrastructure.csproj`** :
  - Ajout de références conditionnelles aux packages de base de données
  - `<!--#if (useMySql)-->` pour Pomelo.EntityFrameworkCore.MySql
  - `<!--#if (usePostgreSql)-->` pour Npgsql.EntityFrameworkCore.PostgreSQL
  - `<!--#if (useSqlServer)-->` pour Microsoft.EntityFrameworkCore.SqlServer

- **`SampleApplicationDbContext.cs`** :
  - Configuration conditionnelle de la méthode OnConfiguring
  - `UseMySql()` pour MySQL
  - `UseNpgsql()` pour PostgreSQL
  - `UseSqlServer()` pour SQL Server

### 📦 Packaging

#### NuSpec
- **`Miccore.CleanArchitecture.Template.nuspec`** :
  - Package ID: `Miccore.CleanArchitecture.Template`
  - Version: 1.0.0
  - Type: Template
  - Tags: dotnet-new, templates, clean-architecture, cqrs, microservice, fastendpoints, mediatr
  - License: MIT
  - Exclusions appropriées pour le packaging

### 🧪 Tests Effectués

#### Scénarios Validés
1. **Génération basique** :
   - ✅ Nom simple (`MonProjet`)
   - ✅ Nom composé (`Acme.Ecommerce.Catalog`)

2. **Providers de base de données** :
   - ✅ MySQL (défaut)
   - ✅ PostgreSQL
   - ✅ SQL Server

3. **Options** :
   - ✅ Avec tests (défaut)
   - ✅ Sans tests (`--includeTests false`)
   - ✅ Avec Docker (défaut)
   - ✅ Sans Docker (`--includeDocker false`)

4. **Compilation** :
   - ✅ Tous les scénarios compilent sans erreur
   - ✅ Aucun avertissement de compilation

5. **Remplacement de noms** :
   - ✅ Namespaces correctement remplacés
   - ✅ Noms de projets et solution renommés
   - ✅ Dossiers renommés
   - ✅ Packages Miccore.* préservés

### 🎯 Utilisation

```bash
# Installation locale (développement)
dotnet new install /Users/manher/Projects/Miccore/sample_project

# Création d'un projet
dotnet new miccore-clean -n Acme.Ecommerce.Catalog

# Avec PostgreSQL et sans tests
dotnet new miccore-clean -n MonProjet --databaseProvider PostgreSQL --includeTests false

# Voir l'aide
dotnet new miccore-clean --help
```

### 📋 Prochaines Étapes Recommandées

1. **Icône** : Remplacer `.template.config/icon.png` par une vraie icône 256x256
2. **Version** : Mettre à jour le numéro de version dans `.nuspec` selon les besoins
3. **Package** : Créer le package NuGet pour distribution
4. **Publication** : Publier sur NuGet.org ou feed privé
5. **CI/CD** : Ajouter workflow pour build/test/publish automatique du template

### 🐛 Problèmes Connus

Aucun problème connu à ce jour.

### 🙏 Remerciements

- Clean Architecture - Robert C. Martin
- FastEndpoints Team
- MediatR - Jimmy Bogard
- Entity Framework Core Team
- Communauté .NET

---

**Date de création** : 6 janvier 2026  
**Auteur** : Miccore  
**Version** : 1.0.0
