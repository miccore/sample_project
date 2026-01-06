# 📝 Guide Rapide - Publication Manuelle

## 🎯 Quand Utiliser la Publication Manuelle ?

| Scénario | Utiliser | Raison |
|----------|----------|---------|
| Release officielle v1.0.0 | ❌ Tag Git | Processus standard, traçabilité |
| Version beta/alpha | ✅ Manuel | Pas besoin de tag Git |
| Test rapide | ✅ Manuel | Option `skip-validation` |
| Correction urgente | ✅ Manuel | Plus rapide |
| Republier même version | ✅ Manuel | NuGet ignore les doublons |

## 🚀 Publication Manuelle en 3 Étapes

### Étape 1 : Ouvrir GitHub Actions

```
https://github.com/miccore/sample_project/actions
```

1. Cliquer sur **"Actions"** (onglet en haut)
2. Dans la liste de gauche, sélectionner **"Publish Manual - NuGet Template"**
3. Cliquer sur le bouton bleu **"Run workflow"** (à droite)

### Étape 2 : Configurer les Options

Un formulaire apparaît avec 4 options :

#### 1. Version ⭐ (Obligatoire)
```
Format: MAJOR.MINOR.PATCH[-suffixe]
```

**Exemples** :
- `1.0.0` - Release stable
- `1.0.0-beta` - Version beta
- `1.0.0-rc1` - Release candidate
- `2.0.0-alpha` - Version alpha

#### 2. Skip validation ⚠️
```
☐ Ignorer les tests de validation
```

**Quand cocher** :
- ✅ Test rapide, vous êtes sûr que ça marche
- ❌ **Ne PAS cocher** pour releases importantes

**Impact** :
- ✅ Coché : Publication en ~30 secondes
- ❌ Décoché : Publication en ~2-3 minutes (avec tests)

#### 3. Create release 📦
```
☑ Créer une GitHub Release
```

**Recommandation** : Toujours cocher

**Si coché** :
- Crée une release sur GitHub
- Version avec suffixe → marquée "Pre-release"
- Package .nupkg attaché à la release

**Si décoché** :
- Publie uniquement sur NuGet.org
- Pas de release GitHub

#### 4. Update nuspec 📝
```
☑ Mettre à jour le fichier .nuspec
```

**Recommandation** : Toujours cocher

**Si coché** :
- Met à jour automatiquement `<version>` dans le .nuspec
- Commit automatique (dans le workflow)

**Si décoché** :
- Utilise la version déjà dans le .nuspec
- À utiliser si vous avez déjà mis à jour manuellement

### Étape 3 : Lancer et Surveiller

1. **Lancer** : Cliquer sur le bouton vert **"Run workflow"** en bas du formulaire

2. **Surveiller** : Le workflow apparaît dans la liste
   - ⏳ Icône jaune : En cours
   - ✅ Icône verte : Succès
   - ❌ Icône rouge : Erreur

3. **Consulter les logs** : Cliquer sur le run pour voir les détails

4. **Durée** :
   - Avec validation : ~2-3 minutes
   - Sans validation : ~30 secondes

## 📋 Configurations Recommandées

### 🎯 Release Stable (Production)

```yaml
Version: 1.0.0
Skip validation: ☐ (décoché)
Create release: ☑ (coché)
Update nuspec: ☑ (coché)
```

**Résultat** :
- ✅ Tests complets (3 scénarios)
- ✅ GitHub Release v1.0.0
- ✅ Publié sur NuGet.org
- ✅ .nuspec mis à jour

---

### 🧪 Version Beta

```yaml
Version: 1.0.0-beta
Skip validation: ☐ (décoché)
Create release: ☑ (coché)
Update nuspec: ☑ (coché)
```

**Résultat** :
- ✅ Tests complets
- ✅ GitHub Release v1.0.0-beta (marquée Pre-release)
- ✅ Publié sur NuGet.org avec tag `-beta`
- ✅ .nuspec mis à jour

---

### ⚡ Publication Rapide (Test)

```yaml
Version: 1.0.1
Skip validation: ☑ (coché) ⚠️
Create release: ☐ (décoché)
Update nuspec: ☑ (coché)
```

**Résultat** :
- ⚠️ Pas de tests
- ❌ Pas de GitHub Release
- ✅ Publié sur NuGet.org
- ✅ .nuspec mis à jour

**⚠️ ATTENTION** : À utiliser uniquement si vous êtes absolument certain que le template fonctionne !

---

### 🔄 Republication

```yaml
Version: 1.0.0
Skip validation: ☐ (décoché)
Create release: ☐ (décoché)
Update nuspec: ☐ (décoché)
```

**Résultat** :
- ✅ Tests complets
- ❌ Pas de nouvelle GitHub Release
- ℹ️ NuGet.org ignore (version existe déjà)
- ℹ️ .nuspec inchangé

**Cas d'usage** : Vérifier que tout fonctionne sans modifier quoi que ce soit

## 🎬 Exemple Complet : Publier v1.0.0-beta

### 1️⃣ Ouvrir le Workflow

```
Actions > Publish Manual - NuGet Template > Run workflow
```

### 2️⃣ Configurer

```
Version: 1.0.0-beta
Skip validation: ☐
Create release: ☑
Update nuspec: ☑
```

### 3️⃣ Lancer

Cliquer **"Run workflow"**

### 4️⃣ Résultat Attendu

**Après 2-3 minutes** :

✅ **GitHub Release créée** :
```
https://github.com/miccore/sample_project/releases/tag/v1.0.0-beta
```
- Marquée "Pre-release" 🏷️
- Package .nupkg en téléchargement

✅ **NuGet.org publié** :
```
https://www.nuget.org/packages/Miccore.CleanArchitecture.Template/1.0.0-beta
```

✅ **Installation** :
```bash
dotnet new install Miccore.CleanArchitecture.Template::1.0.0-beta
```

## 🔍 Vérifier la Publication

### 1. Vérifier le Workflow

✅ Status : Succès (icône verte)
✅ Durée : ~2-3 minutes
✅ Toutes les étapes vertes

### 2. Vérifier NuGet.org

```bash
# Rechercher
dotnet new search miccore

# Installer
dotnet new install Miccore.CleanArchitecture.Template::1.0.0-beta

# Tester
dotnet new miccore-clean -n Test
```

### 3. Vérifier GitHub Release

```
Repository > Releases
```

Doit voir : **v1.0.0-beta** avec badge "Pre-release"

## 🆘 Problèmes Courants

### ❌ "NUGET_API_KEY not defined"

**Solution** :
```
Settings > Secrets and variables > Actions
Créer : NUGET_API_KEY
```

### ⚠️ "Package already exists"

**Normal !** NuGet.org ne permet pas de republier la même version.

Le workflow utilise `--skip-duplicate` donc aucune erreur, juste un avertissement.

### ❌ "Validation failed"

**Causes** :
- Template ne génère pas correctement
- Erreur de compilation

**Solution** :
```bash
# Tester localement
dotnet new install .
dotnet new miccore-clean -n Test
cd Test
dotnet build
```

## 💡 Astuces

### Tester Sans Publier

Impossible de "tester" la publication sans réellement publier.

**Alternatives** :
1. Utiliser une version `-test` : `1.0.0-test`
2. Tester localement avant : `dotnet new install .`
3. Utiliser `skip-validation: false` toujours

### Annuler une Publication

**Impossible !** Une fois publié sur NuGet.org, vous ne pouvez pas supprimer.

**Solutions** :
1. Publier une nouvelle version corrigée
2. Marquer comme deprecated sur NuGet.org
3. Tester minutieusement avant publication

### Publier Plusieurs Versions

Vous pouvez lancer plusieurs workflows en parallèle :

```
1.0.0-beta
1.0.0-rc1
1.0.0
```

Tous peuvent être publiés en même temps.

## 📚 Liens Utiles

- [Documentation Complète](.github/workflows/README.md)
- [Guide GitHub Actions](GITHUB_ACTIONS_QUICKSTART.md)
- [Semantic Versioning](https://semver.org/)
- [NuGet.org](https://www.nuget.org/)

---

**🎉 C'est tout !** La publication manuelle est maintenant simple et flexible.
