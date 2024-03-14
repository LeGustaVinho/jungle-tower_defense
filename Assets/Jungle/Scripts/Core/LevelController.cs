using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public class LevelController
    {
        private LevelConfig levelConfig;
        private ITimerManager timerManager;

        private BoxCollider spawnerArea;
        private GoalTriggerDispatcher goalTriggerDispatcher;
        private BoxCollider goalCollider;

        [ShowInInspector]
        private bool isActive;
        
        [ShowInInspector]
        private int level;

        [ShowInInspector] private float LevelTime => levelTimer?.Time ?? 0;

        private Timer spawnNpcTimer;
        private Timer levelTimer;

        public LevelController(LevelConfig levelConfig, ITimerManager timerManager, BoxCollider spawnerArea, GoalTriggerDispatcher goalTriggerDispatcher)
        {
            this.levelConfig = levelConfig;
            this.timerManager = timerManager;
            this.spawnerArea = spawnerArea;
            this.goalTriggerDispatcher = goalTriggerDispatcher;

            goalCollider = goalTriggerDispatcher.GetComponent<BoxCollider>();
            goalTriggerDispatcher.OnTriggerEnterEvent += OnGoalTriggerEnter;
        }

        private void OnGoalTriggerEnter(Entity entity)
        {
            Pool.Destroy(entity);
        }

        [Button]
        public void StartLevel()
        {
            level = 0;
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
        }

        private void StartNextLevel()
        {
            level++;
            levelTimer = timerManager.SetTimer(levelConfig.RoundTime, StartNextLevel);
        }

        private void SpawnNpc()
        {
            RandomWeightNpcConfig randomNpcConfig = levelConfig.SpawnChance.GetRandomWeight();
            
            Vector3 randomSpawnPoint = spawnerArea.bounds.RandomInsideBox();
            NpcEntity newNpc = Pool.Instantiate(randomNpcConfig.Config.Prefab, randomSpawnPoint, Quaternion.identity);
            Vector3 randomTargetPoint = goalCollider.bounds.RandomInsideBox();
            
            newNpc.Initialize(randomNpcConfig.Config, level, timerManager);
            newNpc.SetAgentTarget(randomTargetPoint);
            newNpc.CombatSystemComponent.OnDie += OnNpcDie;

            if (isActive)
            {
                spawnNpcTimer = timerManager.SetTimer(levelConfig.SpawnInterval, SpawnNpc);
            }
        }

        private void OnNpcDie(Entity killed, Entity killer)
        {
            Pool.Destroy(killed);
        }
    }
}
