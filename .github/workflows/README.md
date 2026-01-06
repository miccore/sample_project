# GitHub Actions Workflows

Ce dossier contient les workflows GitHub Actions pour ce repository.

## 📋 Workflows Disponibles

### `publish-template.yml` - Publication Automatique via Tags

Workflow automatique déclenché par les tags Git pour publier le template sur NuGet.org.

### `publish-manual.yml` - Publication Manuelle avec Options

Workflow manuel avec options avancées pour publier le template sur NuGet.org.

#### Déclenchement

**Uniquement manuel** via l'interface GitHub Actions.

#### Options Disponibles

| Option | Type | Défaut | Description |
|--------|------|--------|-------------|
| **version** | string | *(requis)* | Version du package (ex: 1.0.0, 2.0.0-beta) |
| **skip-validation** | boolean | false | Ignorer les tests de validation |
| **create-release** | boolean | true | Créer une GitHub Release |
| **update-nuspec** | boolean | true | Mettre à jour le .nuspec avec la version |

#### Utilisation

1. **Aller dans Actions** :
   - Repository > Actions > "Publish Manual - NuGet Template"

2. **Cliquer "Run workflow"**

3. **Configurer les options** :
   - **Version** : `1.0.0` (requis)
   - **Skip validation** : ❌ (recommandé: laisser décoché)
   - **Create release** : ✅ (recommandé: laisser coché)
   - **Update nuspec** : ✅ (recommandé: laisser coché)

4. **Lancer** : Cliquer "Run workflow"

#### Cas d'Usage

- 🔧 **Publication rapide** : Publication sans créer de tag Git
- 🧪 **Tests de publication** : Tester avec `skip-validation: true`
- 📦 **Versions beta/alpha** : Publier `1.0.0-beta`, `2.0.0-rc1`
- 🔄 **Republication** : Republier après correction (même version)
- 🎯 **Contrôle total** : Personnaliser chaque aspect de la publication

#### Exemples

**Publication Standard**
```
Version: 1.0.0
Skip validation: false
Create release: true
Update nuspec: true
```

**Publication Beta**
```
Version: 1.0.0-beta
Skip validation: false
Create release: true (sera marquée pre-release)
Update nuspec: true
```

**Publication Rapide (Sans Validation)**
```
Version: 1.0.1
Skip validation: true ⚠️
Create release: false
Update nuspec: true
```

**Test Sans Release**
```
Version: 1.0.0-test
Skip validation: false
Create release: false
Update nuspec: false
```

---

### `publish-template.yml` - Publication Automatique via Tags

Workflow automatique déclenché par les tags Git pour publier le template sur NuGet.org.

#### Déclencheurs

1. **Push sur un tag version** (automatique)
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. **Déclenchement manuel** (via l'interface GitHub)
   - Aller dans Actions > Publish NuGet Template > Run workflow
   - Spécifier la version (ex: 1.0.0)

#### Étapes du Workflow

1. ✅ Checkout du code
2. ✅ Installation de .NET 10.0
3. ✅ Installation de NuGet CLI
4. ✅ Extraction de la version (depuis le tag ou input manuel)
5. ✅ Mise à jour de la version dans le fichier `.nuspec`
6. ✅ Validation du template :
   - Test génération avec MySQL
   - Test génération avec PostgreSQL
   - Test génération sans tests
   - Compilation de chaque projet généré
7. ✅ Création du package `.nupkg`
8. ✅ Publication sur NuGet.org
9. ✅ Création d'une GitHub Release (si tag)
10. ✅ Upload du package comme artifact

#### Configuration Requise

##### Secret GitHub : NUGET_API_KEY

Vous devez ajouter votre clé API NuGet dans les secrets GitHub :

1. **Obtenir une clé API NuGet** :
   - Aller sur https://www.nuget.org/
   - Se connecter à votre compte
   - Aller dans Account Settings > API Keys
   - Créer une nouvelle clé API avec les permissions :
     - Push new packages and package versions
     - Sélectionner "All Packages" ou spécifier `Miccore.CleanArchitecture.Template`

2. **Ajouter le secret dans GitHub** :
   - Aller dans votre repository GitHub
   - Settings > Secrets and variables > Actions
   - Cliquer "New repository secret"
   - Nom : `NUGET_API_KEY`
   - Valeur : Coller votre clé API NuGet
   - Cliquer "Add secret"

#### Utilisation

##### Publier une Nouvelle Version

**Option 1 : Via Tag (Recommandé pour releases officielles)**

```bash
# Créer et pousser un tag de version
git tag v1.0.0
git push origin v1.0.0

# Le workflow se déclenchera automatiquement
```

**Option 2 : Déclenchement Manuel (via workflow_dispatch - DÉPRÉCIÉ)**

> ⚠️ **Note** : Pour les publications manuelles, utilisez maintenant le workflow `publish-manual.yml` qui offre plus d'options.

1. Aller dans l'onglet "Actions" du repository
2. Sélectionner "Publish NuGet Template"
3. Cliquer "Run workflow"
4. Entrer la version (ex: 1.0.0)
5. Cliquer "Run workflow"

##### Convention de Nommage des Versions

Suivez le [Semantic Versioning](https://semver.org/) :

- **MAJOR.MINOR.PATCH** (ex: 1.0.0)
- **MAJOR** : Changements incompatibles
- **MINOR** : Nouvelles fonctionnalités compatibles
- **PATCH** : Corrections de bugs

Exemples :
- `v1.0.0` - Version initiale
- `v1.1.0` - Ajout d'un nouveau provider DB
- `v1.1.1` - Correction d'un bug
- `v2.0.0` - Changements majeurs (breaking changes)

#### Résultats

Après exécution réussie :

1. **Package NuGet** :
   - Publié sur https://www.nuget.org/packages/Miccore.CleanArchitecture.Template/
   - Disponible via `dotnet new install Miccore.CleanArchitecture.Template`

2. **GitHub Release** (si déclenché par tag) :
   - Création automatique d'une release
   - Package `.nupkg` attaché à la release
   - Notes de version générées automatiquement

3. **Artifact** :
   - Package disponible dans les artifacts du workflow (90 jours)

#### Validation

Le workflow valide automatiquement le template en :
- Générant 3 projets test (MySQL, PostgreSQL, sans tests)
- Compilant chaque projet en mode Release
- Échouant si une génération ou compilation échoue

#### Dépannage

**Le workflow échoue sur "NUGET_API_KEY not defined"**
- Vérifier que le secret `NUGET_API_KEY` est bien ajouté dans Settings > Secrets

**Le workflow échoue sur "Package already exists"**
- NuGet.org ne permet pas de republier la même version
- Incrémenter le numéro de version
- Ou utiliser `--skip-duplicate` (déjà activé dans le workflow)

**Les tests de validation échouent**
- Vérifier que le template génère des projets valides localement
- Vérifier les logs détaillés dans l'exécution du workflow

**Le package n'apparaît pas sur NuGet.org**
- La publication peut prendre quelques minutes
- Vérifier le statut de la publication sur votre dashboard NuGet.org
- Vérifier les logs du workflow pour les erreurs

#### Monitoring

Pour surveiller les publications :

1. **GitHub Actions** :
   - Repository > Actions > Workflow runs
   - Voir l'historique et les logs détaillés

2. **NuGet.org** :
   - https://www.nuget.org/packages/Miccore.CleanArchitecture.Template/
   - Voir les statistiques de téléchargement

3. **GitHub Releases** :
   - Repository > Releases
   - Voir toutes les versions publiées

## 🔒 Sécurité

- ✅ La clé API NuGet est stockée dans les secrets GitHub (chiffrés)
- ✅ Ne jamais committer la clé API dans le code
- ✅ Limiter les permissions de la clé API au strict nécessaire
- ✅ Régénérer la clé API si elle est compromise

## 📚 Ressources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [NuGet.org API Keys](https://www.nuget.org/account/apikeys)
- [Semantic Versioning](https://semver.org/)
- [NuGet CLI Reference](https://learn.microsoft.com/nuget/reference/nuget-exe-cli-reference)
