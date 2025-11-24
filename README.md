
# 📘 BlazorGameQuest – Version 3  
**Binôme : EL MISSIRY Tarek & KHAN Ibrahim**

## 🎯 Objectif de la Version 3  
La V3 porte sur **le déroulement complet d’une partie** avec :

- Génération aléatoire d’un/plusieurs donjons  
- Interface de jeu interactive via Blazor  
- Choix dans les salles  
- Calcul dynamique du score  
- Sauvegarde de la partie  
- Tests unitaires enrichis  
- Mesure de la couverture de code  

Cette version combine le **backend (API .NET)** et le **frontend (Blazor WebAssembly)**.

# 🏗️ Architecture du projet

```
BlazorGameQuest1234/
│
├── BlazorGame.Client/            → Frontend Blazor WebAssembly
│   ├── Pages/
│   │   ├── Play.razor            → Page de jeu interactive (V3)
│   ├── Models/
│   │   ├── GameState.cs
│   │   ├── SalleDto.cs
│   │   ├── ChoiceRequest.cs
│   └── Program.cs                → HttpClient configuré pour API
│
├── GameServices/                 → Backend API logique du jeu
│   ├── Controllers/
│   │   ├── GameController.cs     → API jeu (start, state, choice)
│   ├── Services/
│   │   └── GameLogicService.cs   → Logique métier V3
│   ├── Data/
│   │   └── GameDbContext.cs      → Base EF InMemory
│   └── Program.cs                → Swagger, CORS, seed invité
│
├── SharedModels/                 → Modèles partagés (entités)
│   ├── Joueur.cs
│   ├── Donjon.cs
│   ├── Salle.cs
│   ├── Partie.cs
│   ├── RoomType.cs
│
└── Tests/                        → Tests unitaires (xUnit)
    ├── GameLogicTests.cs
    ├── DbContextTests.cs
    ├── JoueurTests.cs
```

# 🧠 Fonctionnalités implémentées en V3

## Génération aléatoire du donjon  
Dans `GameLogicService` :

- Entre **1 et 5 salles aléatoires**
- Description dynamique selon le type (`Enemy`, `Chest`, `Trap`, etc.)
- Points gagnés/perdus aléatoires  
- Difficulté aléatoire

## Interface de jeu interactive  
Dans `Play.razor` :

- Affichage de la salle courante  
- Score courant  
- Choix interactifs :  
  - **Combattre**  
  - **Fuir**  
  - **Fouiller**  
- Mise à jour de l’état via API  
- Affichage fin de partie + score final  

L’UI utilise :  
`POST /api/game/start?joueurId=1`  
`GET /api/game/{partieId}`  
`POST /api/game/{partieId}/choice`  

## Calcul du score  
Dans `GameLogicService.AppliquerChoixAsync` :

- Combattre → gros gain/perte avec probabilité  
- Fuir → petit gain  
- Fouiller → trésor/piège  
- Score négatif = mort immédiate  
- Index de salle incrémenté  
- Fin de donjon = partie terminée  

## Sauvegarde de la partie  
Chaque action appelle :

`await _context.SaveChangesAsync();`

Données conservées :

- Date début  
- Date fin  
- Score courant & final  
- Mort / victoire  
- Salle courante  
- Historique via EF InMemory  

# 🧪 Tests unitaires (V3)

Les tests incluent :

## Tests du modèle & DbContext  
- Insertion joueur  
- Accès base InMemory  
- Relations Donjon → Salles  

## Tests du GameLogicService  
- Génération donjon + salles  
- Progression dans toutes les salles  
- Mort si score négatif  
- Fin de partie  

## Tests du GameController  
- Start renvoie 404 si joueur absent  
- Start renvoie un GameState valide  
- Choice renvoie BadRequest si choix vide  

Tests utilisent des DB InMemory **fraîches par test**.

# 📊 Couverture de code

Collecte :

```
dotnet test --collect:"XPlat Code Coverage"
```

Générer un rapport HTML :

```
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

# 🚀 Lancer le projet

## Backend API
```
dotnet run --project GameServices
```

Swagger :  
http://localhost:5297/swagger

## Frontend Blazor  
http://localhost:5000/

Page de jeu :  
http://localhost:5000/play?from=guest

# 📦 Conclusion

La Version 3 est **complètement réalisée** :

✔ Donjon aléatoire  
✔ Choix interactifs  
✔ Score dynamique  
✔ Sauvegarde partie  
✔ API opérationnelle  
✔ Blazor UI dynamique  
✔ Tests enrichis  
✔ Couverture respectée  

Prêt pour la **V4 : Historique, classement, admin** 🎉
