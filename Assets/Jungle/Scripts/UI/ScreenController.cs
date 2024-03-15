using System;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;

namespace Jungle.Scripts.UI
{
    public class ScreenController
    {
        private IPlayer player;
        private ILevelController levelController;
        private StartScreen startScreen;
        private InGameScreen inGameScreen;
        private StructureBuilderConfig structureBuilderConfig;
        private Leaderboard leaderboard;
        
        public event Action<StructureConfig> OnStructureSelect
        {
            add => inGameScreen.OnStructureSelect += value;
            remove => inGameScreen.OnStructureSelect -= value;
        }

        public ScreenController(IPlayer player, ILevelController levelController, StartScreen startScreen, 
            InGameScreen inGameScreen, StructureBuilderConfig structureBuilderConfig, Leaderboard leaderboard)
        {
            this.player = player;
            this.levelController = levelController;
            this.startScreen = startScreen;
            this.inGameScreen = inGameScreen;
            this.structureBuilderConfig = structureBuilderConfig;
            this.leaderboard = leaderboard;

            player.OnChangeMoney += OnChangeMoney;
            player.OnChangePoints += OnChangePoints;
            player.OnChangeHealthPoints += OnChangeHealthPoints;
            player.OnLoseGame += OnLoseGame;

            startScreen.OnClickStart += OnClickStartGame;
            startScreen.UpdateLeaderBoard(this.leaderboard.LeaderboardEntries);
            inGameScreen.OnClickQuit += OnClickQuitGame;

            levelController.OnChangeLevel += OnChangedLevel;
            leaderboard.OnLeaderboardUpdate += OnLeaderboardUpdate;
        }

        private void OnLeaderboardUpdate(List<LeaderboardEntry> leaderboardEntries)
        {
            startScreen.UpdateLeaderBoard(leaderboardEntries);
        }

        private void OnChangedLevel(int level)
        {
            inGameScreen.UpdateLevel(level);
        }

        private void OnClickStartGame()
        {
            startScreen.gameObject.SetActive(false);
            inGameScreen.gameObject.SetActive(true);
            
            inGameScreen.UpdateHealth(player.CurrentHealthPoint, player.Config.StartingHealthPoints);
            inGameScreen.UpdatePoints(player.Points);
            inGameScreen.UpdateMoney(player.Money);
            inGameScreen.UpdateLevel(1);
            inGameScreen.SetTimeGetter(levelController.GetLevelTime);
            inGameScreen.GenerateStructureUI(structureBuilderConfig.AvailableStructures);
            
            levelController.StartPreparationTime();
        }
        
        private void OnClickQuitGame()
        {
            startScreen.gameObject.SetActive(true);
            inGameScreen.gameObject.SetActive(false);
            
            inGameScreen.SetTimeGetter(null);
            levelController.StopLevel();
        }

        private void OnChangeHealthPoints(int hp)
        {
            inGameScreen.UpdateHealth(hp, player.Config.StartingHealthPoints);
        }

        private void OnChangePoints(int points)
        {
            inGameScreen.UpdatePoints(points);
        }

        private void OnChangeMoney(float money)
        {
            inGameScreen.UpdateMoney(money);
        }
        
        private void OnLoseGame()
        {
            
        }
    }
}