using System;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public class LevelController
    {
        [ShowInInspector] 
        public int Level { get; private set; }
        
        public event Action<int> OnChangeLevel;
        public event Action<int> OnStartGame;
        public event Action<int> OnFinishGame;
        
        private LevelConfig levelConfig;
        private ITimerManager timerManager;

        private BoxCollider spawnerArea;
        private GoalTriggerDispatcher goalTriggerDispatcher;
        private BoxCollider goalCollider;

        [ShowInInspector]
        private bool isActive;
        private List<NpcEntity> activeNpcs = new List<NpcEntity>();
        
        [ShowInInspector] 
        private float LevelTime => levelTimer?.Time ?? 0;

        private Timer spawnNpcTimer;
        private Timer levelTimer;
        private Player player;

        public LevelController(LevelConfig levelConfig, ITimerManager timerManager, BoxCollider spawnerArea, 
            GoalTriggerDispatcher goalTriggerDispatcher, Player player)
        {
            this.levelConfig = levelConfig;
            this.timerManager = timerManager;
            this.spawnerArea = spawnerArea;
            this.goalTriggerDispatcher = goalTriggerDispatcher;
            this.player = player;

            goalCollider = goalTriggerDispatcher.GetComponent<BoxCollider>();
            goalTriggerDispatcher.OnTriggerEnterEvent += OnGoalTriggerEnter;
            player.OnLoseGame += StopLevel;
        }

        [Button]
        public void StartPreparationTime()
        {
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
            
            Vector3 randomSpawnPoint = spawnerArea.bounds.RandomInsideBox();
            Vector3 randomTargetPoint = goalCollider.bounds.RandomInsideBox();
            NpcEntity newNpc = Pool.Instantiate(randomNpcConfig.Config.Prefab, randomSpawnPoint, 
                Quaternion.LookRotation(randomTargetPoint - randomSpawnPoint));
            
            newNpc.Initialize(randomNpcConfig.Config, Level, timerManager);
            newNpc.SetAgentTarget(randomTargetPoint);
            newNpc.CombatSystemComponent.OnDie += OnNpcDie;
            
            activeNpcs.Add(newNpc);

            if (isActive)
            {
                spawnNpcTimer = timerManager.SetTimer(levelConfig.SpawnInterval, SpawnNpc);
            }
        }
        
        private void OnGoalTriggerEnter(Entity entity)
        {
            Pool.Destroy(entity);
            player.CurrentHealthPoint -= (int)entity.Attributes[EntityAttribute.PlayerDamage];
        }

        private void OnNpcDie(Entity killed, Entity killer)
        {
            killed.CombatSystemComponent.OnDie -= OnNpcDie;
            activeNpcs.Remove(killed as NpcEntity);
            Pool.Destroy(killed);
            player.Points += (int)killed.Attributes[EntityAttribute.Points];
            player.Money += (int)killed.Attributes[EntityAttribute.Money];
        }
    }
}
