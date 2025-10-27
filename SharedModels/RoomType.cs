namespace SharedModels;

public enum RoomType
{
    Enemy,     // salle avec un ennemi à combattre
    Chest,     // salle avec un coffre
    Trap,      // salle piégée
    Puzzle,    // (optionnel) salle d’énigme
    Empty      // salle vide
}
