# 🕹️ BlazorGameQuest

**Projet en continu – Développement Agile (.NET & C# / Efrei 2025-2026)**  
Cours : *Environnement .NET et C# (ALTN71)*  
Enseignant : **Thierry TAGNE**

---

## 🎯 Objectif du projet

Développer un **jeu d’aventure Blazor** où les joueurs explorent des donjons aléatoires, affrontent des ennemis, et gagnent des points selon leurs choix.  
Le projet repose sur une architecture **.NET multi-projets** (Client, API, Modèles partagés, Tests unitaires) et sera développé en plusieurs rendus successifs (V1 → V5).

---

## 🧱 Architecture du projet

```
BlazorGameQuest1234/
├── BlazorGame.Client/          # Frontend Blazor WebAssembly (interface du jeu)
│   ├── Pages/                  # Pages principales : Home, Play, Scores, Admin
│   ├── Components/             # Composant de salle de jeu (RoomView)
│   └── Layout/                 # Layout et navigation
│
├── AuthenticationServices/     # Backend Web API (.NET 9)
│   ├── Controllers/
│   │   ├── PingController.cs   # Endpoint de test
│   │   └── JoueurController.cs # Endpoint listant des joueurs
│   └── Program.cs              # Configuration API + Swagger
│
├── SharedModels/               # Bibliothèque de classes partagées
│   ├── Joueur.cs               # Modèle Joueur (Nom, Score)
│   └── RoomType.cs             # Types de salles (Enemy, Trap, Chest…)
│
├── BlazorGame.Tests/           # Tests unitaires (xUnit)
│   └── JoueurTests.cs
│
└── Readme.md                   # Documentation du projet
```

---

## 🧪 Fonctionnalités de la Version 1 (V1)

- ✅ Création complète de la **solution .NET** (4 projets)
- ✅ Mise en place du **frontend Blazor WebAssembly**
- ✅ **Routing**, **layout**, et pages principales (`Home`, `Play`, `Scores`, `Admin`)
- ✅ Composant Blazor statique (`RoomView.razor`)
- ✅ Création du **backend Web API** :
  - `PingController` (test de disponibilité)
  - `JoueurController` (liste de joueurs simulée)
  - **Swagger** activé pour la documentation
- ✅ Projet **SharedModels** (modèles partagés entre client et API)
- ✅ Projet **de tests unitaires** (`xUnit`)
- ✅ Configuration **port 5000** pour le client, **5050** pour l’API
- ✅ Documentation initiale (README)

---

## ▶️ Lancement du projet

### 1️⃣ Lancer l’API
```bash
dotnet run --project .\AuthenticationServices\ --urls "http://localhost:5050"
```
- Test API : [http://localhost:5050/ping](http://localhost:5050/ping)
- Liste des joueurs : [http://localhost:5050/joueur](http://localhost:5050/joueur)
- Swagger : [http://localhost:5050/swagger](http://localhost:5050/swagger)

### 2️⃣ Lancer le client Blazor
```bash
dotnet run --project .\BlazorGame.Client\
```
- Accès au site : [http://localhost:5000](http://localhost:5000)

> 💡 L’API doit rester en marche pendant que le client tourne.

---

## ⚙️ Technologies utilisées

- **.NET 9 / C# 12**
- **Blazor WebAssembly**
- **ASP.NET Core Web API**
- **Entity Framework Core (InMemory à venir)**
- **Swagger / OpenAPI**
- **xUnit** (tests unitaires)
- **Git / GitLab** (branche `main` → `prod`)

---

## 👥 Équipe

| Nom | Rôle |
|------|------|
| Tarek | Développeur C# |
| Ibrahim | Développeur C# |

---

## 🧾 Version actuelle

> **Version 1 – Structure du projet + tests initiaux + premières pages Blazor**  
>  
> ✅ Architecture fonctionnelle  
> ✅ Communication API–Client prête pour V2  
> ⏳ Prochaine étape : EFCore, Keycloak et API Gateway (V2-V3)

---

## 📅 Plan des prochaines versions

| Version | Objectif principal | Date prévue |
|----------|--------------------|--------------|
| V1 | Structure, tests, Blazor base | ✅ 13/10/2025 |
| V2 | EFCore + stockage InMemory | 06/11/2025 |
| V3 | Auth Keycloak (OAuth2) | 10/11/2025 |
| V4 | Gateway + rôles + Docker | 17/11/2025 |
| V5 | Finalisation & déploiement | 27/11/2025 |

---

## 🧩 Auteur

> Projet réalisé dans le cadre du cours **Environnement .NET et C# (ALTN71)** – Efrei Paris  
> Année universitaire **2025–2026**  
