using System;
using Sirenix.OdinInspector;

namespace Jungle.Scripts.Core
{
    public interface IPlayer
    {
        PlayerConfig Config { get; }
        int CurrentHealthPoint { get; set; }
        int Points { get; set; }
        float Money { get; set; }
        event Action<int> OnChangeHealthPoints;
        event Action<float> OnChangeMoney;
        event Action<int> OnChangePoints;
        event Action OnLoseGame;
        void Reset();
    }

    public class Player : IPlayer
    {
        public PlayerConfig Config { private set; get; }

        [ShowInInspector]
        public int CurrentHealthPoint
        {
            get => currentHealthPoint;
            set
            {
                if (currentHealthPoint != value)
                {
                    currentHealthPoint = value;
                    OnChangeHealthPoints?.Invoke(currentHealthPoint);
                }

                if (currentHealthPoint <= 0)
                {
                    OnLoseGame?.Invoke();
                }
            }
        }

        [ShowInInspector]
        public int Points
        {
            get => points;
            set
            {
                if (points != value)
                {
                    points = value;
                    OnChangePoints?.Invoke(points);
                }
            }
        }

        [ShowInInspector]
        public float Money
        {
            get => money;
            set
            {
                if (money != value)
                {
                    money = value;
                    OnChangeMoney?.Invoke(money);
                }
            }
        }
        
        public event Action<int> OnChangeHealthPoints;
        public event Action<float> OnChangeMoney;
        public event Action<int> OnChangePoints;
        
        public event Action OnLoseGame;
        
        private int currentHealthPoint;
        private int points;
        private float money;

        public Player(PlayerConfig playerConfig)
        {
            Config = playerConfig;

            Reset();
        }

        public void Reset()
        {
            CurrentHealthPoint = Config.StartingHealthPoints;
            Money = Config.StartingMoney;
            Points = 0;
        }
    }
}
