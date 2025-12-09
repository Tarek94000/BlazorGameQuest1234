using SharedModels;
using Xunit;

namespace Tests;

public class JoueurTests
{
    [Fact]
    public void Joueur_DefaultScore_IsZero()
    {
        var joueur = new Joueur();
        Assert.Equal(0, joueur.Score);
    }

    [Fact]
    public void Joueur_CanChangeState()
    {
        var joueur = new Joueur { EstActif = false };
        joueur.EstActif = true;
        Assert.True(joueur.EstActif);
    }
}
