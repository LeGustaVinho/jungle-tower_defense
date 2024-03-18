using System.Linq;
using Jungle.Scripts.Core;
using Moq;
using NUnit.Framework;

public class LeaderboardTest
{
    //A Test behaves as an ordinary method
    private Leaderboard leaderboard;
    private Mock<IPlayer> mockPlayer;
    private Mock<ILevelController> mockLevelController;
    
    [SetUp]
    public void SetUp()
    {
        mockPlayer = new Mock<IPlayer>();
        mockLevelController = new Mock<ILevelController>();
    
        leaderboard = new Leaderboard(mockPlayer.Object, mockLevelController.Object);
    }
    
    [Test]
    public void AddsEntryOnLoseGame()
    {
        mockPlayer.SetupGet(p => p.Points).Returns(100);
        mockLevelController.SetupGet(lc => lc.Level).Returns(1);
    
        // Simulate losing a game
        mockPlayer.Raise(p => p.OnLoseGame += null);
    
        Assert.AreEqual(1, leaderboard.LeaderboardEntries.Count);
        Assert.AreEqual(100, leaderboard.LeaderboardEntries.First().Points);
        Assert.AreEqual(1, leaderboard.LeaderboardEntries.First().Level);
    }
}
