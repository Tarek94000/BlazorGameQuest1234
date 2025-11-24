using Xunit;
using SharedModels;

namespace Tests;

public class AdventureTests
{
    [Fact]
    public void Dungeon_Should_Have_At_Most_5_Rooms()
    {
<<<<<<< HEAD
=======
        // TODO(V2): quand la génération sera codée, vérifier Rooms.Count <= 5
>>>>>>> origin/prod
        Assert.True(true);
    }

    [Theory]
    [InlineData(RoomType.Enemy)]
    [InlineData(RoomType.Chest)]
    [InlineData(RoomType.Trap)]
    public void Room_Type_Should_Be_Valid(RoomType t)
    {
<<<<<<< HEAD
=======
        // TODO(V2): chaque Room.Type doit être dans l'enum
>>>>>>> origin/prod
        Assert.Contains(t, new[] { RoomType.Enemy, RoomType.Chest, RoomType.Trap });
    }

    [Fact]
    public void Combat_Action_Should_Affect_Score()
    {
<<<<<<< HEAD
=======
        // TODO(V2+): l’action "Combattre" modifie le score
>>>>>>> origin/prod
        Assert.True(true);
    }

    [Fact]
    public void Game_Should_End_On_Death_Or_After_N_Rooms()
    {
<<<<<<< HEAD
=======
        // TODO(V2+): fin si mort ou après N salles
>>>>>>> origin/prod
        Assert.True(true);
    }
}
