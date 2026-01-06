# Guide de Publication du Template NuGet

Ce guide explique comment packager et publier le template Miccore Clean Architecture.

## 📋 Prérequis

- .NET 10.0 SDK
- Compte NuGet.org (pour publication publique)
- NuGet CLI (optionnel, pour créer le package .nupkg)

## 🧪 Test Local (Sans Package)

La méthode la plus simple pour tester le template :

```bash
# Installer le template directement depuis le dossier
dotnet new install /Users/manher/Projects/Miccore/sample_project

# Tester la génération
cd /tmp
mkdir test-template && cd test-template
dotnet new miccore-clean -n MyTest.Project

# Désinstaller
dotnet new uninstall /Users/manher/Projects/Miccore/sample_project
```

## 📦 Création du Package NuGet

### Option 1 : Utiliser NuGet CLI (Recommandé)

1. **Télécharger NuGet.exe** :
   ```bash
   # macOS/Linux
   curl -o nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
   
   # Ou via Homebrew (macOS)
   brew install nuget
   ```

2. **Créer le package** :
   ```bash
   cd /Users/manher/Projects/Miccore/sample_project
   nuget pack Miccore.CleanArchitecture.Template.nuspec
   ```

   Cela créera : `Miccore.CleanArchitecture.Template.1.0.0.nupkg`

### Option 2 : Package Manuel

Si nuget.exe n'est pas disponible, le package .nupkg est simplement un fichier ZIP avec une structure spécifique :

```bash
cd /Users/manher/Projects/Miccore/sample_project

# Créer la structure
mkdir -p temp-package/content
rsync -av --exclude='bin/' --exclude='obj/' --exclude='.git/' --exclude='.github/' ./ temp-package/content/

# Copier le nuspec à la racine
cp Miccore.CleanArchitecture.Template.nuspec temp-package/

# Créer le fichier .nupkg (c'est un ZIP)
cd temp-package
zip -r ../Miccore.CleanArchitecture.Template.1.0.0.nupkg * -x "*.DS_Store"
cd ..
rm -rf temp-package
```

## 🧪 Test du Package Localement

```bash
# Installer depuis le package local
dotnet new install ./Miccore.CleanArchitecture.Template.1.0.0.nupkg

# Vérifier l'installation
dotnet new list | grep miccore

# Tester la génération
mkdir test-project && cd test-project
dotnet new miccore-clean -n Acme.Test.Service --databaseProvider PostgreSQL

# Compiler pour vérifier
cd Acme.Test.Service
dotnet build

# Désinstaller
dotnet new uninstall Miccore.CleanArchitecture.Template
```

## 🚀 Publication sur NuGet.org

### Méthode 1 : Publication Automatique via GitHub Actions (Recommandé)

Le repository inclut un workflow GitHub Actions pour automatiser la publication.

#### Configuration Initiale (Une seule fois)

1. **Créer un compte NuGet.org** :
   - Aller sur https://www.nuget.org/
   - Créer un compte ou se connecter

2. **Générer une clé API** :
   - Aller dans Account Settings > API Keys
   - Créer une nouvelle clé avec les permissions :
     - ✅ Push new packages and package versions
     - Sélectionner "All Packages" ou `Miccore.CleanArchitecture.Template`
   - Copier la clé (elle ne sera affichée qu'une fois !)

3. **Ajouter le secret dans GitHub** :
   - Aller dans votre repository GitHub
   - Settings > Secrets and variables > Actions
   - Cliquer "New repository secret"
   - Nom : `NUGET_API_KEY`
   - Valeur : Coller votre clé API NuGet
   - Cliquer "Add secret"

#### Publier une Nouvelle Version

**Option A : Via Git Tag (Recommandé)**

```bash
# Créer et pousser un tag de version
git tag v1.0.0
git push origin v1.0.0

# Le workflow GitHub Actions se déclenchera automatiquement et :
# - Validera le template (génération de 3 projets test + compilation)
# - Créera le package NuGet
# - Le publiera sur NuGet.org
# - Créera une GitHub Release
```

**Option B : Déclenchement Manuel**

1. Aller dans l'onglet "Actions" du repository
2. Sélectionner "Publish NuGet Template"
3. Cliquer "Run workflow"
4. Entrer la version (ex: 1.0.0)
5. Cliquer "Run workflow"

#### Avantages de GitHub Actions

- ✅ Validation automatique avant publication (3 scénarios de tests)
- ✅ Pas besoin d'installer NuGet CLI localement
- ✅ Historique complet des publications
- ✅ GitHub Release automatique avec notes de version
- ✅ Package sauvegardé comme artifact (90 jours)
- ✅ Clé API sécurisée dans les secrets GitHub

Voir [.github/workflows/README.md](.github/workflows/README.md) pour plus de détails.

---

### Méthode 2 : Publication Manuelle

Si vous préférez publier manuellement :

#### 1. Créer un Compte NuGet.org

- Aller sur https://www.nuget.org/
- Créer un compte ou se connecter
- Générer une clé API dans les paramètres du compte

#### 2. Publier le Package

```bash
# Via NuGet CLI
nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY_HERE

# Ou via dotnet CLI
dotnet nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg --api-key YOUR_API_KEY_HERE --source https://api.nuget.org/v3/index.json
```

#### 3. Vérification

Après publication (peut prendre quelques minutes) :

```bash
# Rechercher le template
dotnet new search miccore

# Installer depuis NuGet
dotnet new install Miccore.CleanArchitecture.Template

# Utiliser
dotnet new miccore-clean -n MyProject
```

## 📝 Mise à Jour du Template

Pour publier une nouvelle version :

1. **Mettre à jour la version** dans `Miccore.CleanArchitecture.Template.nuspec` :
   ```xml
   <version>1.1.0</version>
   ```

2. **Recréer le package** :
   ```bash
   nuget pack Miccore.CleanArchitecture.Template.nuspec
   ```

3. **Republier** :
   ```bash
   nuget push Miccore.CleanArchitecture.Template.1.1.0.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY_HERE
   ```

## 🔒 Publication Privée (Feed Interne)

### Azure Artifacts

```bash
# Ajouter la source
dotnet nuget add source "https://pkgs.dev.azure.com/yourorg/_packaging/yourfeed/nuget/v3/index.json" --name "YourFeed"

# Publier
dotnet nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg --source "YourFeed" --api-key az
```

### GitHub Packages

```bash
# Ajouter la source
dotnet nuget add source "https://nuget.pkg.github.com/OWNER/index.json" --name github --username OWNER --password GITHUB_TOKEN --store-password-in-clear-text

# Publier
dotnet nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg --source github
```

## ✅ Checklist Avant Publication

- [ ] Tester le template localement avec plusieurs configurations
- [ ] Vérifier que tous les providers de base de données fonctionnent
- [ ] Tester avec et sans tests
- [ ] Tester avec et sans Docker
- [ ] Vérifier que la compilation réussit pour tous les scénarios
- [ ] Mettre à jour le numéro de version
- [ ] Mettre à jour les release notes dans le .nuspec
- [ ] Vérifier que l'icône est présente (icon.png)
- [ ] Tester le package localement avant publication
- [ ] Documenter les changements dans un CHANGELOG

## 📚 Ressources

- [Documentation officielle des templates .NET](https://learn.microsoft.com/dotnet/core/tools/custom-templates)
- [Spécification template.json](https://github.com/dotnet/templating/wiki/Reference-for-template.json)
- [Guide de publication NuGet](https://learn.microsoft.com/nuget/nuget-org/publish-a-package)
- [NuGet CLI Reference](https://learn.microsoft.com/nuget/reference/nuget-exe-cli-reference)

## 🐛 Dépannage

### Le template ne s'installe pas

```bash
# Nettoyer le cache des templates
dotnet new --debug:reinit

# Réessayer l'installation
dotnet new install ./Miccore.CleanArchitecture.Template.1.0.0.nupkg
```

### Les symboles conditionnels ne fonctionnent pas

Vérifier que les conditions utilisent la syntaxe correcte dans template.json :
```json
"useMySql": {
  "type": "computed",
  "value": "(databaseProvider == \"MySQL\")"
}
```

### Les fichiers ne sont pas exclus correctement

Vérifier les patterns dans la section `sources/modifiers/exclude` du template.json.
