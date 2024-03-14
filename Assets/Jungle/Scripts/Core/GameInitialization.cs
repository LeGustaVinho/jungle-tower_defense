using System;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using LegendaryTools.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jungle.Scripts.Core
{
    public class GameInitialization : SerializedMonoBehaviour
    {
        [BoxGroup("Configs")]
        [SerializeField] private LevelConfig levelConfig;
        [BoxGroup("Configs")]
        [SerializeField] private StructureBuilderConfig structureBuilderConfig;
        [BoxGroup("Configs")]
        [SerializeField] private PlayerConfig playerConfig;
        
        [BoxGroup("SceneRefs")]
        [SerializeField] private BoxCollider spawnerArea;
        [BoxGroup("SceneRefs")]
        [SerializeField] private GoalTriggerDispatcher goalTriggerDispatcher;
        [BoxGroup("SceneRefs")]
        [SerializeField] private ScreenToWorldInfo structureBuildRaycaster;
        [BoxGroup("SceneRefs")]
        [SerializeField] private ScreenToWorldInfo structureUpgradeRaycaster;

        [ShowInInspector][BoxGroup("Systems")]
        private LevelController levelController;
        
        [ShowInInspector][BoxGroup("Systems")]
        private ITimerManager timerManager;
        
        [ShowInInspector][BoxGroup("Systems")]
        private Player player;
        
        [ShowInInspector][BoxGroup("Systems")]
        private StructureBuilder structureBuilder;
        
        public void Start()
        {
            timerManager = new TimerManager();
            player = new Player(playerConfig);
            structureBuilder = new StructureBuilder(structureBuilderConfig,timerManager, player, 
                structureBuildRaycaster, structureUpgradeRaycaster);
            levelController = new LevelController(levelConfig, timerManager, spawnerArea, goalTriggerDispatcher, player);
            
            timerManager.Initialize();
        }
    }
}
