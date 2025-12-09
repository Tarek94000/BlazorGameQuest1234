
# 🎮 Blazor Game Quest  
**Projet C# / Blazor WASM / API .NET — Jeu interactif avec donjons générés aléatoirement**

---

## 👥 Membres du projet  
- **Tarek EL MISSIRY**  
- **Ibrahim KHAN**

---

## 📝 Description générale  
Blazor Game Quest est un jeu d’aventure dans lequel un joueur traverse un donjon généré aléatoirement, fait des choix (combattre, fuir, fouiller), gagne des points et tente d’obtenir le meilleur score.

Le projet comporte :  
- un **frontend Blazor WebAssembly**  
- une **API .NET** (GameServices)  
- un **module d’authentification** (Keycloak)  
- une **base de données EF Core** (InMemory pour les V2–V4)  
- des **tests unitaires xUnit** avec couverture de code  

Le projet est développé en 5 versions successives.

---

# ⭐ Versions réalisées

---

## ✅ Version 1 – Base du frontend (Blazor)  
- Création d’une première interface Blazor  
- Page statique de démonstration du concept  
- Navigation simple  
- Structure initiale du projet

---

## ✅ Version 2 – Modélisation + Base de données (EF Core InMemory)  
### 📌 Réalisations  
- Création des entités :  
  - Joueur, Administrateur, Donjon, Salle, Partie, RoomType  
- Mise en place d’Entity Framework Core (InMemory)  
- Création du `GameDbContext`  
- Création des contrôleurs API CRUD :  
  - `JoueursController`  
  - `DonjonsController`  
  - `SallesController`  
  - `PartiesController`  
- Activation de Swagger pour tester l’API  
- Mise en place des premiers tests unitaires (xUnit)  

---

## ✅ Version 3 – Logique de jeu + interface dynamique  
### 📌 Réalisations  
- Création du **GameLogicService** avec :  
  - génération aléatoire d’un donjon (1 à 5 salles)  
  - description dynamique des salles  
  - calcul du score selon les actions  
  - passage automatique d’une salle à l’autre  
  - gestion de fin de partie (victoire/défaite)  
- Mise en place du **GameController** :  
  - `POST /api/game/start`  
  - `POST /api/game/{id}/choice`  
  - `GET /api/game/{id}`  
- Intégration dans Blazor :  
  - affichage de la salle courante  
  - score dynamique  
  - boutons interactifs (Combattre, Fouiller, Fuir)  
  - affichage fin de partie  
- **Tests unitaires enrichis**  
- Couverture de code > 80% atteinte

---

## ✅ Version 4 – Classement, Historique, Admin Dashboard  
### 📌 Réalisations  
### 👤 Historique du joueur  
- Endpoint : `GET /api/joueurs/{id}/parties`  
- Page Blazor : `/historique`  
- Affichage des anciennes parties (score, état, dates, donjon)

### 🏆 Classement général  
- Endpoint : `GET /api/classement`  
- Page Blazor : `/classement`  
- Tri des joueurs par meilleur score  
- Affichage du Top 10

### 🛠️ Dashboard Admin  
- Liste des joueurs  
- Désactivation / activation  
- Liste de toutes les parties  
- Export JSON / CSV  
- Page Blazor : `/admin`  

### 🧪 Tests V4  
- tests du classement  
- tests de l’historique  
- tests admin  
- couverture toujours conforme (> 80%)

---

## ✅ Version 5 – Authentification & Sécurité (Keycloak)  
### 📌 Réalisations  
- Installation et configuration de Keycloak  
- Création des rôles : **user**, **admin**  
- Sécurisation des endpoints :  
  - actions admin → rôle *admin*  
  - historique joueur → rôle *user*  
  - actions de jeu → joueur connecté  
- Intégration du login dans Blazor  
- Récupération du token dans les appels API  
- Redirection automatique si non authentifié  

---

# 🧪 Tests unitaires & Couverture  
### ✔ xUnit  
### ✔ coverlet.collector  
### ✔ couverture > 80% (objectif atteint)

Exécution :

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

# 🏗️ Architecture du projet

```
BlazorGameQuest1234/
│
├── BlazorGame.Client/        → Frontend Blazor WebAssembly
│   ├── Pages/
│   ├── Models/
│   └── Shared/
│
├── GameServices/             → API .NET + EF Core
│   ├── Controllers/
│   ├── Data/
│   └── Services/
│
├── AuthenticationServices/   → Module d’authentification (Keycloak)
│
├── SharedModels/             → Entités partagées
│
├── Tests/                    → Tests unitaires (.NET + xUnit)
│
└── README.md                 → Documentation du projet
```

---

# 🚀 Lancer le projet

## 1️⃣ Lancer l’API
```bash
dotnet run --project GameServices
```

## 2️⃣ Lancer le frontend Blazor
```bash
dotnet run --project BlazorGame.Client
```

## 3️⃣ Tester l’API avec Swagger  
👉 `http://localhost:5297/swagger`

---

# 🛡️ Sécurité (Version 5)
- Authentification via Keycloak  
- Token JWT utilisé dans les appels API  
- Rôles utilisés pour limiter l’accès :
  - user → jouer, voir l’historique  
  - admin → dashboard complet  

---

# 📄 API principales

### 🎮 Jeu
- `POST /api/game/start?joueurId=x`  
- `POST /api/game/{id}/choice`  
- `GET /api/game/{id}`  

### 👤 Joueur
- `GET /api/joueurs/{id}`
- `GET /api/joueurs/{id}/parties`

### 🏆 Classement
- `GET /api/classement`

### 🛠️ Admin
- `GET /api/admin/joueurs`
- `PUT /api/admin/joueurs/{id}/desactiver`
- `PUT /api/admin/joueurs/{id}/activer`
- `GET /api/admin/parties`
- `GET /api/admin/export/joueurs`

---

# ✔ Statut final du projet

| Version | Statut |
|--------|--------|
| V1 | ✔ Terminée |
| V2 | ✔ Terminée |
| V3 | ✔ Terminée |
| V4 | ✔ Terminée |
| V5 | ✔ Terminée |
| Tests + couverture | ✔ OK |

---

# 🎯 Projet 100% conforme au cahier des charges  
Ce projet respecte l’intégralité des objectifs pédagogiques et techniques demandés.
