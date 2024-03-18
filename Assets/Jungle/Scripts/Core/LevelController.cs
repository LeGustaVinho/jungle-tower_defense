using System;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public interface ILevelController
    {
        int Level { get; }
        BoxCollider SpawnerArea { get; }
        BoxCollider GoalCollider { get; }
        event Action<int> OnChangeLevel;
        event Action<int> OnStartGame;
        event Action<int> OnFinishGame;
        void StartPreparationTime();
        void StartLevel();
        void StopLevel();
        float GetLevelTime();
    }

    public class LevelController : ILevelController
    {
        [ShowInInspector] 
        public int Level { get; private set; }
        public BoxCollider SpawnerArea { private set; get; }
        public BoxCollider GoalCollider { private set; get; }
        
        public event Action<int> OnChangeLevel;
        public event Action<int> OnStartGame;
        public event Action<int> OnFinishGame;
        
        private readonly LevelConfig levelConfig;
        private readonly ITimerManager timerManager;
        private readonly IPlayer player;
        private readonly IUnityEngineAPI unityEngineAPI;
        private GoalTriggerDispatcher goalTriggerDispatcher;

        [ShowInInspector]
        private bool isActive;
        private List<NpcEntity> activeNpcs = new List<NpcEntity>();
        
        [ShowInInspector] 
        private float LevelTime => levelTimer?.Time ?? 0;

        private Timer spawnNpcTimer;
        private Timer levelTimer;

        public LevelController(LevelConfig levelConfig, ITimerManager timerManager, BoxCollider spawnerArea, 
            GoalTriggerDispatcher goalTriggerDispatcher, IPlayer player, IUnityEngineAPI unityEngineAPI)
        {
            this.levelConfig = levelConfig;
            this.timerManager = timerManager;
            this.SpawnerArea = spawnerArea;
            this.goalTriggerDispatcher = goalTriggerDispatcher;
            this.player = player;
            this.unityEngineAPI = unityEngineAPI;

            GoalCollider = goalTriggerDispatcher.GetComponent<BoxCollider>();
            goalTriggerDispatcher.OnTriggerEnterEvent += OnGoalTriggerEnter;
            player.OnLoseGame += StopLevel;
        }

        [Button]
        public void StartPreparationTime()
        {
            player.Reset();
            levelTimer = timerManager.SetTimer(levelConfig.PreparationTime, StartLevel);
            OnStartGame?.Invoke(Level);
        }
        
        [Button]
        public void StartLevel()
        {
            Level = 0;
            isActive = true;
            SpawnNpc();
            
            levelTimer = timerManager.SetTimer(levelConfig.RoundTime, StartNextLevel);
        }
        
        [Button]
        public void StopLevel()
        {
            isActive = false;
            if (spawnNpcTimer != null)
            {
                timerManager.AbortTimer(spawnNpcTimer);
                spawnNpcTimer = null;
            }
            
            if (levelTimer != null)
            {
                timerManager.AbortTimer(levelTimer);
                levelTimer = null;
            }

            foreach (NpcEntity npc in activeNpcs)
            {
                Pool.Destroy(npc);
            }
            
            OnFinishGame?.Invoke(Level);
        }

        public float GetLevelTime()
        {
            return LevelTime;
        }

        private void StartNextLevel()
        {
            Level++;
            levelTimer = timerManager.SetTimer(levelConfig.RoundTime, StartNextLevel);
            OnChangeLevel?.Invoke(Level);
        }

        private void SpawnNpc()
        {
            RandomWeightNpcConfig randomNpcConfig = levelConfig.SpawnChance.GetRandomWeight();
            
            Vector3 randomSpawnPoint = SpawnerArea.bounds.RandomInsideBox();
            Vector3 randomTargetPoint = GoalCollider.bounds.RandomInsideBox();
            NpcEntity newNpc = Pool.Instantiate(randomNpcConfig.Config.Prefab, randomSpawnPoint, 
                Quaternion.LookRotation(randomTargetPoint - randomSpawnPoint));
            
            newNpc.Initialize(randomNpcConfig.Config, Level, timerManager, unityEngineAPI);
            newNpc.SetAgentTarget(randomTargetPoint);
            newNpc.CombatSystemComponent.OnDie += OnNpcDie;
            
            activeNpcs.Add(newNpc);

            if (isActive)
            {
                spawnNpcTimer = timerManager.SetTimer(levelConfig.SpawnInterval, SpawnNpc);
            }
        }
        
        private void OnGoalTriggerEnter(IEntity entity)
        {
            Pool.Destroy(entity as Entity);
            player.CurrentHealthPoint -= (int)entity.Attributes[EntityAttribute.PlayerDamage];
        }

        private void OnNpcDie(IEntity killed, IEntity killer)
        {
            killed.CombatSystemComponent.OnDie -= OnNpcDie;
            activeNpcs.Remove(killed as NpcEntity);
            Pool.Destroy(killed as Entity);
            player.Points += (int)killed.Attributes[EntityAttribute.Points];
            player.Money += (int)killed.Attributes[EntityAttribute.Money];
        }
    }
}
