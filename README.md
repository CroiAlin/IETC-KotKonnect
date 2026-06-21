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
| Docker Desktop | récent (Compose v2) | https://www.docker.com/products/docker-desktop/ |
| Visual Studio 2026 (ou `dotnet` CLI) | — | pour le backend |
| Visual Studio Code | — | pour le frontend |

Vérifier les installations :

```
dotnet --version       → 10.x
node --version         → v20.x ou +
ng version             → Angular CLI 21.x
docker compose version → Docker Compose v2.x
```

## 2. Structure du dépôt

```
IETC-KotConnect/
├── backend/KotKonnect/            # Solution .NET — Clean Architecture (API → CORE ← INFRASTRUCTURE)
│   ├── KotKonnect.Api/            # EndPoints (Minimal API), Middleware, Program.cs (point d'entrée HTTP)
│   ├── KotKonnect.Core/           # Models, IGateways (ports), UseCases (logique métier) — ne dépend de RIEN
│   └── KotKonnect.Infrastructure/ # Gateways + Repositories (Dapper), Security (BCrypt/JWT), accès MySQL
├── frontend/                      # Application Angular (pages/ + components/ + services/api/)
├── database/                      # Scripts SQL (création + données de test)
└── README.md
```

## 3. Base de données (Docker)

La base tourne dans un **conteneur Docker** : pas besoin d'installer MySQL. Au tout premier démarrage, le conteneur crée la base `kotkonnect` **et** joue automatiquement le schéma (`01`) puis le jeu de données de démonstration (`02`).

> ⚠️ Le port **3306** doit être libre. Si un MySQL local tourne déjà (service `MySQL80` sous Windows), arrête-le — PowerShell administrateur : `net stop MySQL80` — ou change le port hôte dans `docker-compose.yml` (ex. `"3307:3306"`).

1. Lancer **Docker Desktop**.
2. À la racine du dépôt :
   ```
   docker compose up -d
   ```
3. Attendre que le conteneur soit `healthy` :
   ```
   docker compose ps
   ```

La base est alors disponible sur `localhost:3306` — utilisateur `root`, mot de passe `kotkonnectdev` — déjà peuplée (comptes de test, biens, candidatures…).

**Commandes utiles :**
```
docker compose stop      # arrêter (données conservées)
docker compose up -d     # redémarrer
docker compose down      # supprimer le conteneur (le volume de données reste)
docker compose down -v   # tout supprimer, volume compris -> base recréée au prochain up
```

> 💡 Les scripts `01`/`02` ne sont rejoués **qu'au premier démarrage** (volume vide). Si tu modifies un script, fais `docker compose down -v` puis `docker compose up -d` pour forcer le rechargement.

**Alternative sans Docker :** exécuter les deux scripts à la main dans n'importe quel client MySQL (Workbench, ou `mysql -u root -p < database/01_KotKonnect_schema.sql` puis `02`).

## 4. Configuration du backend

Le fichier de configuration contenant les secrets n'est **pas versionné**. Il faut le créer :

1. Aller dans `backend/KotKonnect/KotKonnect.Api/`.
2. Créer un fichier nommé `appsettings.Development.json` avec ce contenu :

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=kotkonnect;User=root;Password=kotkonnectdev"
  },
  "Jwt": {
    "Secret": "remplacez-par-une-chaine-secrete-aleatoire-d-au-moins-32-caracteres",
    "Issuer": "KotKonnect",
    "Audience": "KotKonnectAngular",
    "AccessTokenExpirationMinutes": 15
  }
}
```

3. Le mot de passe `kotkonnectdev` correspond à celui défini dans `docker-compose.yml` — ne pas le changer si tu utilises Docker. (Si tu as installé MySQL toi-même, mets ton propre mot de passe root à la place.)
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
- `/biens` — liste publique des kots ; `/biens/:id` — détail d'un kot avec galerie photos
- `/mes-biens`, `/biens/nouveau`, `/biens/:id/modifier` — gestion des kots (propriétaires uniquement, protégé par `roleGuard`)
- `/mes-candidatures` — candidatures envoyées (étudiant) ; `/candidatures-recues` — candidatures reçues (propriétaire)
- `/mon-profil` — consultation / modification de son profil ; `/profils/:id` — profil d'un candidat (propriétaire)

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
- ✅ Candidatures : un étudiant postule à un bien publié (une seule candidature par bien — contrainte d'unicité, sinon 409), consultation côté étudiant (« mes candidatures ») et côté propriétaire (« candidatures reçues »), gestion des statuts (ENVOYE / VU / ACCEPTE / REFUSE) réservée au propriétaire du bien ; accès Dapper en multi-mapping (Candidature + Bien + Étudiant). Frontend complet avec notifications **ngx-toastr** (bibliothèque tierce)
- ✅ Profils : création automatique du profil à l'inscription, page « Mon profil » (consultation + modification via reactive form), nom/prénom et badge de rôle affichés dans la navbar ; un propriétaire peut consulter le profil d'un candidat **uniquement** si celui-ci a postulé à un de ses biens (autorisation métier vérifiée en base, 403 sinon)
- 🔜 Baux, paiements, messagerie

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
| GET | `/api/profils/me` | Profil de l'utilisateur connecté | 200, 401, 404 |
| PUT | `/api/profils/me` | Modifier son propre profil | 200 + profil, 401, 404 |
| GET | `/api/profils/{id}` | Profil d'un candidat (propriétaire, si l'étudiant a postulé chez lui) | 200, 403 sinon, 404 |
