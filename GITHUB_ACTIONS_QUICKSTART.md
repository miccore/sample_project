# 🚀 Publication Automatique avec GitHub Actions - Guide Rapide

## Configuration Initiale (5 minutes)

### Étape 1 : Obtenir une Clé API NuGet

1. **Aller sur NuGet.org** :
   ```
   https://www.nuget.org/
   ```

2. **Se connecter ou créer un compte**

3. **Créer une clé API** :
   - Cliquer sur votre nom d'utilisateur (en haut à droite)
   - "API Keys"
   - "Create" / "+ Create"
   
4. **Configurer la clé** :
   - **Key Name** : `GitHub Actions - Miccore Template`
   - **Expiration** : 365 days (ou selon votre préférence)
   - **Scopes** :
     - ✅ Push
     - ✅ Push new packages and package versions
   - **Glob Pattern** : `Miccore.CleanArchitecture.Template` (ou `*` pour tous)
   - Cliquer "Create"

5. **Copier la clé** :
   ```
   ⚠️ IMPORTANT : Copiez la clé immédiatement !
   Elle ne sera plus affichée après fermeture.
   ```

### Étape 2 : Ajouter le Secret dans GitHub

1. **Aller dans votre repository GitHub** :
   ```
   https://github.com/miccore/sample_project
   ```

2. **Naviguer vers les Secrets** :
   ```
   Settings (onglet) > Secrets and variables > Actions
   ```

3. **Créer un nouveau secret** :
   - Cliquer "New repository secret"
   - **Name** : `NUGET_API_KEY`
   - **Secret** : Coller votre clé API NuGet
   - Cliquer "Add secret"

4. **Vérification** :
   - Le secret `NUGET_API_KEY` doit apparaître dans la liste
   - ✅ Secret ajouté avec succès !

---

## Publier une Nouvelle Version

### Méthode 1 : Via Git Tag (Recommandé - Production)

**Pour les releases officielles** - Utilise le workflow automatique `publish-template.yml`

```bash
# 1. Vérifier que tout est commit et push
git status
git add .
git commit -m "feat: ready for v1.0.0 release"
git push origin main

# 2. Créer et pousser le tag
git tag v1.0.0
git push origin v1.0.0

# 3. Le workflow se déclenche automatiquement !
# Aller voir : https://github.com/miccore/sample_project/actions
```

**Le workflow va** :
1. ✅ Installer .NET 10 et NuGet
2. ✅ Mettre à jour la version dans le .nuspec
3. ✅ Tester 3 scénarios (MySQL, PostgreSQL, sans tests)
4. ✅ Compiler tous les projets générés
5. ✅ Créer le package .nupkg
6. ✅ Publier sur NuGet.org
7. ✅ Créer une GitHub Release automatiquement

### Méthode 2 : Publication Manuelle avec Options (Nouveau ⭐)

**Pour plus de contrôle** - Utilise le workflow manuel `publish-manual.yml`

1. **Aller dans Actions** :
   ```
   https://github.com/miccore/sample_project/actions
   ```

2. **Sélectionner** :
   - "Publish Manual - NuGet Template" (dans la liste de gauche)

3. **Lancer le workflow** :
   - Cliquer "Run workflow" (bouton bleu à droite)
   - Configurer les options :
     ```
     Version: 1.0.0 (ou 1.0.0-beta, 2.0.0-rc1, etc.)
     Skip validation: ❌ (décoché par défaut - recommandé)
     Create release: ✅ (coché par défaut)
     Update nuspec: ✅ (coché par défaut)
     ```
   - Cliquer "Run workflow"

4. **Suivre l'exécution** :
   - Un nouveau run apparaît dans la liste
   - Cliquer dessus pour voir les logs en temps réel

#### Quand Utiliser la Publication Manuelle ?

| Scénario | Workflow à Utiliser |
|----------|---------------------|
| **Release officielle** | ✅ Tag Git (`publish-template.yml`) |
| **Version beta/alpha** | ⭐ Manuel (`publish-manual.yml`) |
| **Test rapide** | ⭐ Manuel avec `skip-validation: true` |
| **Republication** | ⭐ Manuel (même version) |
| **Sans créer de tag** | ⭐ Manuel |

#### Options de Publication Manuelle

**Standard (Recommandé)**
```
Version: 1.0.0
Skip validation: false
Create release: true
Update nuspec: true
```

**Version Beta**
```
Version: 1.0.0-beta
Skip validation: false
Create release: true (sera marquée pre-release)
Update nuspec: true
```

**Publication Rapide**
```
Version: 1.0.1
Skip validation: true ⚠️ (utiliser avec précaution)
Create release: false
Update nuspec: true
```

### Méthode 3 : Déclenchement Manuel Simplifié (Déprécié)

1. **Aller dans Actions** :
   ```
   https://github.com/miccore/sample_project/actions
   ```

2. **Sélectionner le workflow** :
   - "Publish NuGet Template" (dans la liste de gauche)

3. **Lancer manuellement** :
   - Cliquer "Run workflow" (bouton bleu à droite)
   - **Branch** : main
   - **Version** : 1.0.0 (entrer la version souhaitée)
   - Cliquer "Run workflow"

4. **Suivre l'exécution** :
   - Un nouveau run apparaît dans la liste
   - Cliquer dessus pour voir les logs en temps réel

---

## Convention de Versioning

Suivre le [Semantic Versioning](https://semver.org/) :

### Format : MAJOR.MINOR.PATCH

| Type | Quand ? | Exemple |
|------|---------|---------|
| **PATCH** | Corrections de bugs, mises à jour doc | `1.0.0` → `1.0.1` |
| **MINOR** | Nouvelles fonctionnalités compatibles | `1.0.1` → `1.1.0` |
| **MAJOR** | Changements incompatibles (breaking) | `1.1.0` → `2.0.0` |

### Exemples Concrets

```bash
# Correction d'un bug
git tag v1.0.1
git push origin v1.0.1

# Ajout d'un nouveau provider DB (compatible)
git tag v1.1.0
git push origin v1.1.0

# Changement de structure (breaking change)
git tag v2.0.0
git push origin v2.0.0
```

---

## Vérifier la Publication

### 1. Vérifier le Workflow

**Pendant l'exécution** :
```
GitHub > Actions > Cliquer sur le run en cours
```

**Étapes à surveiller** :
- ✅ Checkout code
- ✅ Setup .NET
- ✅ Validate template (3 tests)
- ✅ Create NuGet package
- ✅ Publish to NuGet.org
- ✅ Create GitHub Release

**Durée estimée** : 2-3 minutes

### 2. Vérifier sur NuGet.org

Attendre 2-5 minutes après publication, puis :

```
https://www.nuget.org/packages/Miccore.CleanArchitecture.Template/
```

Ou rechercher :
```bash
dotnet new search miccore
```

### 3. Tester l'Installation

```bash
# Installer depuis NuGet.org
dotnet new install Miccore.CleanArchitecture.Template

# Vérifier
dotnet new list | grep miccore

# Créer un projet test
mkdir test-install && cd test-install
dotnet new miccore-clean -n TestInstall
cd TestInstall
dotnet build

# Nettoyer
cd ../..
rm -rf test-install
dotnet new uninstall Miccore.CleanArchitecture.Template
```

### 4. Vérifier la GitHub Release

```
GitHub > Releases > Devrait voir v1.0.0
```

La release contient :
- 📦 Package .nupkg en attachement
- 📝 Notes de version générées automatiquement
- 📋 Commandes d'installation et d'utilisation

---

## Dépannage Rapide

### ❌ Erreur : "NUGET_API_KEY not defined"

**Solution** :
```
1. Vérifier que le secret existe : Settings > Secrets and variables > Actions
2. Le nom doit être exactement : NUGET_API_KEY (sensible à la casse)
3. Recréer le secret si nécessaire
```

### ❌ Erreur : "Package already exists"

**Cause** : Vous essayez de republier la même version

**Solution** :
```bash
# Incrémenter la version
git tag v1.0.1  # Au lieu de v1.0.0
git push origin v1.0.1
```

### ❌ Erreur : "Validation tests failed"

**Cause** : Le template ne génère pas de projets valides

**Solution** :
```bash
# Tester localement d'abord
dotnet new install .
dotnet new miccore-clean -n Test --databaseProvider PostgreSQL
cd Test
dotnet build
```

### ⏱️ Le package n'apparaît pas sur NuGet.org

**Normal !** La publication peut prendre 5-10 minutes.

**Vérifier** :
1. Workflow terminé avec succès ? (GitHub Actions)
2. Attendre 5 minutes supplémentaires
3. Vérifier sur votre dashboard NuGet.org

---

## Checklist Avant Publication

- [ ] Code committé et pushé
- [ ] Tests passent localement
- [ ] Documentation à jour
- [ ] Version incrémentée correctement
- [ ] CHANGELOG.md mis à jour
- [ ] Secret `NUGET_API_KEY` configuré

---

## Commandes Utiles

```bash
# Lister tous les tags
git tag -l

# Supprimer un tag local
git tag -d v1.0.0

# Supprimer un tag distant
git push origin :refs/tags/v1.0.0

# Voir l'historique des workflows
gh run list  # Nécessite GitHub CLI

# Voir les logs d'un workflow
gh run view <run-id> --log
```

---

## Ressources

- 📖 [Documentation complète](.github/workflows/README.md)
- 📚 [Guide de publication](TEMPLATE_PUBLISHING.md)
- 📝 [Changelog](TEMPLATE_CHANGELOG.md)
- 🔗 [Semantic Versioning](https://semver.org/)
- 🔗 [GitHub Actions Docs](https://docs.github.com/en/actions)

---

## Support

En cas de problème :
1. Consulter les logs du workflow sur GitHub Actions
2. Vérifier la documentation dans `.github/workflows/README.md`
3. Ouvrir une issue sur GitHub
