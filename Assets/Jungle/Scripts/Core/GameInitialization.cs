using System;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jungle.Scripts.Core
{
    public class GameInitialization : MonoBehaviour
    {
        [BoxGroup("Configs")]
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private StructureBuilder structureBuilder;
        
        [BoxGroup("SceneRefs")]
        [SerializeField] private BoxCollider spawnerArea;
        [BoxGroup("SceneRefs")]
        [SerializeField] private GoalTriggerDispatcher goalTriggerDispatcher;

        [ShowInInspector][BoxGroup("Systems")]
        private LevelController levelController;
        
        [ShowInInspector]
        private ITimerManager timerManager;
        
        public void Start()
        {
            timerManager = new TimerManager();
            levelController = new LevelController(levelConfig, timerManager, spawnerArea, goalTriggerDispatcher);
            
            timerManager.Initialize();
            structureBuilder.Initialize(timerManager);
        }
    }
}
