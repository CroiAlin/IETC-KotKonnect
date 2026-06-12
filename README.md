# KotConnect

Plateforme de mise en relation entre **étudiants** à la recherche d'un kot et **propriétaires** de biens à louer.
Projet réalisé dans le cadre du cours *Projet de développement web* — IETC, année académique 2025-2026.

| Couche | Technologie |
|---|---|
| Frontend | Angular 21 (composants standalone, nouvelle syntaxe `@if` / `@for`) |
| Backend | ASP.NET Core (.NET 10), Clean Architecture (3 projets : Api / Core / Infrastructure) |
| Base de données | MySQL 8, accès via **Dapper** |
| Authentification | JWT + refresh token, hashage BCrypt |

---

## 1. Prérequis

Installer les outils suivants (versions utilisées pour le développement) :

| Outil | Version | Lien |
|---|---|---|
| SDK .NET | 10.0 | https://dotnet.microsoft.com/download |
| Node.js | 20 LTS ou plus récent | https://nodejs.org |
| Angular CLI | 21 | `npm install -g @angular/cli` |
| MySQL Server | 8.0 | https://dev.mysql.com/downloads/installer/ |
| MySQL Workbench | 8.0 (optionnel mais recommandé) | inclus dans l'installeur MySQL |
| Visual Studio 2026 (ou `dotnet` CLI) | — | pour le backend |
| Visual Studio Code | — | pour le frontend |

Vérifier les installations :

```
dotnet --version     → 10.x
node --version       → v20.x ou +
ng version           → Angular CLI 21.x
```

## 2. Structure du dépôt

```
IETC-KotConnect/
├── backend/KotKonnect/            # Solution .NET
│   ├── KotKonnect.Api/            # Controllers, Program.cs (point d'entrée HTTP)
│   ├── KotKonnect.Core/           # Entités, DTOs, interfaces, logique métier
│   └── KotKonnect.Infrastructure/ # Repositories Dapper, BCrypt, JWT, accès MySQL
├── frontend/                      # Application Angular
├── database/                      # Scripts SQL (création + données de test)
└── README.md
```

## 3. Installation de la base de données

1. Démarrer MySQL Server (service `MySQL80` sous Windows — démarré automatiquement après installation).
2. Ouvrir **MySQL Workbench** et se connecter à l'instance locale (`127.0.0.1:3306`, utilisateur `root`).
3. Ouvrir le fichier `database/01_KotKonnect_schema.sql` (File → Open SQL Script).
4. Exécuter tout le script (icône éclair ⚡). La base `kotkonnect` est créée avec ses 12 tables.
5. Exécuter ensuite `database/02_KotKonnect_seed.sql` pour les données de test.

> Le script de schéma est réexécutable : il supprime et recrée entièrement la base à chaque exécution.

Alternative en ligne de commande :

```
mysql -u root -p < database/01_KotKonnect_schema.sql
mysql -u root -p < database/02_KotKonnect_seed.sql
```

## 4. Configuration du backend

Le fichier de configuration contenant les secrets n'est **pas versionné**. Il faut le créer :

1. Aller dans `backend/KotKonnect/KotKonnect.Api/`.
2. Créer un fichier nommé `appsettings.Development.json` avec ce contenu :

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=kotkonnect;User=root;Password=VOTRE_MOT_DE_PASSE_MYSQL"
  },
  "Jwt": {
    "Secret": "remplacez-par-une-chaine-secrete-aleatoire-d-au-moins-32-caracteres",
    "Issuer": "KotKonnect",
    "Audience": "KotKonnectAngular",
    "AccessTokenMinutes": 15
  }
}
```

3. Remplacer `VOTRE_MOT_DE_PASSE_MYSQL` par le mot de passe root choisi à l'installation de MySQL.
4. Remplacer la valeur de `Secret` par n'importe quelle chaîne d'au moins 32 caractères.

## 5. Lancement du backend

**Option A — Visual Studio :**
1. Ouvrir `backend/KotKonnect/KotKonnect.slnx`.
2. Définir `KotKonnect.Api` comme projet de démarrage (clic droit → *Set as Startup Project*).
3. F5 (profil `http`).

**Option B — ligne de commande :**
```
cd backend/KotKonnect/KotKonnect.Api
dotnet run
```

L'API écoute sur **http://localhost:5218**.

Test rapide : le fichier `KotKonnect.Api/KotKonnect.Api.http` contient des requêtes prêtes à l'emploi (exécutables depuis Visual Studio ou VS Code avec l'extension REST Client).

## 6. Lancement du frontend

```
cd frontend
npm install
ng serve
```

L'application est accessible sur **http://localhost:4200** (le backend doit tourner en parallèle).

## 7. Comptes de test

> ⚠️ Section à compléter : les comptes de test seront ajoutés dans `02_KotKonnect_seed.sql`
> au fur et à mesure de l'avancement du projet.

En attendant, créer un compte via l'API :

```http
POST http://localhost:5218/api/auth/register
Content-Type: application/json

{
  "email": "etudiant@test.be",
  "motDePasse": "Test1234!",
  "role": "ETUDIANT",
  "nom": "Dupont",
  "prenom": "Jean"
}
```

## 8. Fonctionnalités implémentées à ce stade

- ✅ Base de données complète (12 tables, contraintes d'intégrité, scripts réexécutables)
- ✅ Authentification backend : inscription, connexion, renouvellement de session
  (JWT 15 min + refresh token 7 jours avec rotation, mots de passe hashés BCrypt)
- 🔜 Authentification frontend, gestion des biens, candidatures, baux, paiements, messagerie

## 9. Endpoints disponibles

| Méthode | Route | Description | Réponses |
|---|---|---|---|
| POST | `/api/auth/register` | Inscription (ETUDIANT ou PROPRIETAIRE) | 200 + tokens, 409 si email pris |
| POST | `/api/auth/login` | Connexion | 200 + tokens, 401 si identifiants invalides |
| POST | `/api/auth/refresh` | Nouveau JWT via refresh token | 200 + tokens, 401 si token invalide/expiré |
