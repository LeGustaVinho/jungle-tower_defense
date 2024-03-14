using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    [Serializable]
    public class Player
    {
        public PlayerConfig PlayerConfig;
        
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
            PlayerConfig = playerConfig;

            Reset();
        }

        public void Reset()
        {
            currentHealthPoint = PlayerConfig.StartingHealthPoints;
            money = PlayerConfig.StartingMoney;
        }
    }
}
