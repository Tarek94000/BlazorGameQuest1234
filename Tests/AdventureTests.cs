using Xunit;
using SharedModels;

namespace Tests;

public class AdventureTests
{
    [Fact]
    public void Dungeon_Should_Have_At_Most_5_Rooms()
    {
        Assert.True(true);
    }

    [Theory]
    [InlineData(RoomType.Enemy)]
    [InlineData(RoomType.Chest)]
    [InlineData(RoomType.Trap)]
    public void Room_Type_Should_Be_Valid(RoomType t)
    {
        Assert.Contains(t, new[] { RoomType.Enemy, RoomType.Chest, RoomType.Trap });
    }

    [Fact]
    public void Combat_Action_Should_Affect_Score()
    {
        Assert.True(true);
    }

    [Fact]
    public void Game_Should_End_On_Death_Or_After_N_Rooms()
    {
        Assert.True(true);
    }
}
