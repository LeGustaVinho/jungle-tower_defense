using Jungle.Scripts.Core;

namespace Jungle.Scripts.UI
{
    public class ScreenController
    {
        private Player player;
        private LevelController levelController;
        private StartScreen startScreen;
        private InGameScreen inGameScreen;

        public ScreenController(Player player, LevelController levelController, StartScreen startScreen, InGameScreen inGameScreen)
        {
            this.player = player;
            this.levelController = levelController;
            this.startScreen = startScreen;
            this.inGameScreen = inGameScreen;

            player.OnChangeMoney += OnChangeMoney;
            player.OnChangePoints += OnChangePoints;
            player.OnChangeHealthPoints += OnChangeHealthPoints;
            player.OnLoseGame += OnLoseGame;

            startScreen.OnClickStart += OnClickStartGame;
            inGameScreen.OnClickQuit += OnClickQuitGame;

            levelController.OnChangeLevel += OnChangedLevel;
        }

        private void OnChangedLevel(int level)
        {
            inGameScreen.UpdateLevel(level);
        }

        private void OnClickStartGame()
        {
            startScreen.gameObject.SetActive(false);
            inGameScreen.gameObject.SetActive(true);
            
            inGameScreen.UpdateHealth(player.CurrentHealthPoint, player.PlayerConfig.StartingHealthPoints);
            inGameScreen.UpdatePoints(player.Points);
            inGameScreen.UpdateMoney(player.Money);
            inGameScreen.UpdateLevel(1);
        }
        
        private void OnClickQuitGame()
        {
            startScreen.gameObject.SetActive(true);
            inGameScreen.gameObject.SetActive(false);
        }

        private void OnChangeHealthPoints(int hp)
        {
            inGameScreen.UpdateHealth(hp, player.PlayerConfig.StartingHealthPoints);
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