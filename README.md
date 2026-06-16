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
5. Exécuter ensuite `database/02_KotKonnect_seed.sql` pour charger un jeu de données de démonstration complet (comptes, biens avec photos, candidatures, baux, paiements…).

> Les deux scripts sont réexécutables : `01` supprime et recrée entièrement la base ; `02` vide les tables puis les recharge (idempotent). On peut donc les relancer à tout moment pour repartir d'une base propre et peuplée.

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
    "AccessTokenExpirationMinutes": 15
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

Pages disponibles :
- `/login`, `/register` — connexion / inscription
- `/` — accueil (nécessite d'être connecté ; sinon redirection vers `/login`)
- `/biens` — liste publique des kots ; `/biens/:id` exposé via l'API pour le détail
- `/mes-biens`, `/biens/nouveau`, `/biens/:id/modifier` — gestion des kots (propriétaires uniquement, protégé par `roleGuard`)

## 7. Comptes de test

Le seed (`02_KotKonnect_seed.sql`) charge **7 comptes prêts à l'emploi**. Mot de passe identique pour tous : **`Test1234!`**

| Rôle | Email |
|---|---|
| Étudiant | `alice.martin@etu.be` |
| Étudiant | `bruno.lefevre@etu.be` |
| Étudiant | `chloe.dubois@etu.be` |
| Étudiant | `lucas.gerard@etu.be` |
| Propriétaire | `marie.dupont@proprio.be` |
| Propriétaire | `jean.bernard@proprio.be` |
| Propriétaire | `sophie.lambert@proprio.be` |

Pour créer d'autres comptes : via l'interface (http://localhost:4200/register) ou via l'API ci-dessous. **Mot de passe requis : au moins 6 caractères, avec une minuscule, une majuscule et un chiffre** (validé côté frontend ET backend).

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

- ✅ Base de données complète (12 tables, contraintes d'intégrité, scripts réexécutables) + jeu de données de démonstration (seed idempotent peuplant toutes les tables)
- ✅ Authentification backend : inscription, connexion, renouvellement de session
  (JWT 15 min + refresh token 7 jours avec rotation, mots de passe hashés BCrypt,
  validation de la force du mot de passe)
- ✅ Authentification frontend (Angular) : pages de connexion et d'inscription
  (reactive forms + validation), `AuthService` avec état par signals, intercepteur JWT
  (ajout du token + renouvellement automatique sur 401), guards de route
  (`authGuard`, `roleGuard`), thème sombre responsive
- ✅ Gestion des biens (kots) : liste publique, détail, CRUD complet pour les propriétaires
  (création, édition, publication, suppression en soft delete), gestion des photos par URL,
  accès Dapper avec multi-mapping Bien+Photos, protection par rôle + vérification de propriété
- ✅ Barre de navigation globale (navigation entre pages, profil connecté, déconnexion)
- ✅ Candidatures (backend) : un étudiant postule à un bien publié (une seule candidature par bien — contrainte d'unicité, sinon 409), consultation côté étudiant (« mes candidatures ») et côté propriétaire (« candidatures reçues »), gestion des statuts (ENVOYE / VU / ACCEPTE / REFUSE) réservée au propriétaire du bien ; accès Dapper en multi-mapping (Candidature + Bien + Étudiant)
- 🔜 Candidatures (frontend), baux, paiements, messagerie

## 9. Endpoints disponibles

| Méthode | Route | Description | Réponses |
|---|---|---|---|
| POST | `/api/auth/register` | Inscription (ETUDIANT ou PROPRIETAIRE) | 200 + tokens, 400 si mot de passe trop faible, 409 si email pris |
| POST | `/api/auth/login` | Connexion | 200 + tokens, 401 si identifiants invalides |
| POST | `/api/auth/refresh` | Nouveau JWT via refresh token | 200 + tokens, 401 si token invalide/expiré |
| GET | `/api/biens` | Liste des biens publiés (public) | 200 |
| GET | `/api/biens/{id}` | Détail d'un bien (public) | 200, 404 si inexistant |
| GET | `/api/biens/mes-biens` | Biens du propriétaire connecté | 200, 401/403 |
| POST | `/api/biens` | Créer un bien (propriétaire) | 200 + bien, 401/403 |
| PUT | `/api/biens/{id}` | Modifier un bien (propriétaire owner) | 204, 403 si pas le sien, 404 |
| DELETE | `/api/biens/{id}` | Supprimer un bien — soft delete (owner) | 204, 403, 404 |
| POST | `/api/biens/{id}/photos` | Ajouter une photo (URL) au bien (owner) | 204, 403, 404 |
| DELETE | `/api/biens/{id}/photos/{photoId}` | Supprimer une photo (owner) | 204, 403, 404 |
| POST | `/api/candidatures` | Postuler à un bien publié (étudiant) | 200, 403, 404, 409 si déjà postulé |
| GET | `/api/candidatures/mes-candidatures` | Candidatures de l'étudiant connecté | 200, 401/403 |
| GET | `/api/candidatures/recues` | Candidatures reçues sur ses biens (propriétaire) | 200, 401/403 |
| PUT | `/api/candidatures/{id}/statut` | Changer le statut d'une candidature (propriétaire owner) | 204, 400 si statut invalide, 403, 404 |
