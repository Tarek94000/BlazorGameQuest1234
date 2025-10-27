# 📦 Version 2 – Modélisation et Base de Données

## 🎯 Objectif de la V2
Mettre en place la **modélisation du domaine** et une **base de données fonctionnelle** avec **Entity Framework Core (InMemory)**.  
Cette version introduit les entités principales du jeu (Joueur, Donjon, Salle, Partie, Administrateur) et expose leurs **endpoints CRUD** via une API REST testable dans Swagger.

---

## 🧱 Architecture mise à jour

```
BlazorGameQuest1234/
├── BlazorGame.Client/            # Interface Blazor WebAssembly
│
├── GameServices/                 # Web API (.NET 9 / EFCore)
│   ├── Controllers/
│   │   ├── JoueursController.cs
│   │   ├── DonjonsController.cs
│   │   ├── SallesController.cs
│   │   └── PartiesController.cs
│   ├── Data/
│   │   └── GameDbContext.cs
│   └── Program.cs                # Configuration EFCore + Swagger
│
├── SharedModels/                 # Bibliothèque de classes partagées
│   ├── Joueur.cs
│   ├── Administrateur.cs
│   ├── Donjon.cs
│   ├── Salle.cs
│   ├── Partie.cs
│   └── RoomType.cs
│
├── Tests/                        # Tests unitaires (xUnit)
│   ├── JoueurTests.cs
│   └── DbContextTests.cs
│
└── README.md
```

---

## ⚙️ Fonctionnalités implémentées

- ✅ **Entity Framework Core (InMemory)** configuré  
- ✅ **DbContext** central (`GameDbContext`) avec toutes les entités  
- ✅ **API CRUD** :  
  - `api/joueurs`  
  - `api/donjons`  
  - `api/salles`  
  - `api/parties`
- ✅ **Swagger** activé (`http://localhost:5297/swagger`)  
- ✅ **Tests unitaires** de validation des modèles et du contexte  
- ✅ **Projet compilable et exécutable sans erreur**

---

## 🧪 Exécution et test

### ▶️ Lancer l’API
```bash
dotnet run --project .\GameServices\
```
- **Swagger** : [http://localhost:5297/swagger](http://localhost:5297/swagger)

### 🧩 Lancer les tests
```bash
dotnet test
```
Résultat attendu :  
```
Récapitulatif du test : total : 10; échec : 0; réussi : 10; ignoré : 0
```

---

## 🧠 Entités principales

| Entité | Description |
|--------|--------------|
| `Joueur` | Représente un joueur lié à un compte Keycloak (Nom, Score, EstActif, KeycloakId) |
| `Administrateur` | Gère les joueurs (création, désactivation, export) |
| `Donjon` | Ensemble de salles avec un niveau de difficulté |
| `Salle` | Élément du donjon : combat, piège, coffre, etc. |
| `Partie` | Association d’un joueur et d’un donjon ; conserve le score final |

---

## 📘 Exemple d’entité

```csharp
public class Joueur
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public int Score { get; set; } = 0;
    public bool EstActif { get; set; } = true;
    public string KeycloakId { get; set; } = "";
    public List<Partie>? HistoriqueParties { get; set; }
}
```

---

## 🧾 Tests unitaires

### Exemple 1 – Test modèle
```csharp
[Fact]
public void Joueur_DefaultScore_IsZero()
{
    var joueur = new Joueur();
    Assert.Equal(0, joueur.Score);
}
```

### Exemple 2 – Test DbContext
```csharp
[Fact]
public void CanAddJoueurToDatabase()
{
    var options = new DbContextOptionsBuilder<GameDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;

    using var context = new GameDbContext(options);
    context.Joueurs.Add(new Joueur { Nom = "Tarek" });
    context.SaveChanges();

    var joueur = context.Joueurs.FirstOrDefault(j => j.Nom == "Tarek");
    Assert.NotNull(joueur);
}
```

---

## ✅ État de la version

| Élément | Statut |
|----------|--------|
| Modèles (SharedModels) | ✅ |
| Base de données EFCore | ✅ |
| API CRUD + Swagger | ✅ |
| Tests unitaires | ✅ |
| Documentation | ✅ |

---

## 🚀 Prochaine étape : Version 3

**Déroulement d’une partie et logique métier**  
- Génération aléatoire des donjons (suite de salles)  
- Choix interactifs du joueur (combattre, fuir, fouiller)  
- Calcul du score et sauvegarde de la partie  
- Tests de logique de jeu  
