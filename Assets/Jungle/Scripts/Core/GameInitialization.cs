using Jungle.Scripts.Mechanics;
using Jungle.Scripts.UI;
using LegendaryTools.Input;
using Sirenix.OdinInspector;
using UnityEngine;

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
        [SerializeField] private GridDrawer gridDrawer;
        [BoxGroup("SceneRefs")]
        [SerializeField] private BoxCollider spawnerArea;
        [BoxGroup("SceneRefs")]
        [SerializeField] private GoalTriggerDispatcher goalTriggerDispatcher;
        [BoxGroup("SceneRefs")]
        [SerializeField] private ScreenToWorldInfo structureBuildRaycaster;
        [BoxGroup("SceneRefs")]
        [SerializeField] private ScreenToWorldInfo structureUpgradeRaycaster;
        [BoxGroup("SceneRefs")]
        [SerializeField] private ScreenToWorldInfo structureDestroyRaycaster;

        [BoxGroup("Screens")]
        [SerializeField] private StartScreen startScreen;

        [BoxGroup("Screens")]
        [SerializeField] private InGameScreen inGameScreen;

        [ShowInInspector][BoxGroup("Systems")]
        private ILevelController levelController;
        
        [ShowInInspector][BoxGroup("Systems")]
        private ITimerManager timerManager;
        
        [ShowInInspector][BoxGroup("Systems")]
        private IPlayer player;
        
        [ShowInInspector][BoxGroup("Systems")]
        private StructureBuilder structureBuilder;
        
        [ShowInInspector][BoxGroup("Systems")]
        private Leaderboard leaderboard;

        [BoxGroup("Systems")] 
        private ScreenController screenController;
        
        public void Start()
        {
            gridDrawer.cellSize = structureBuilderConfig.GridSnappingDistance;
            gridDrawer.DrawGrid();

            IUnityEngineAPI unityEngineAPI = new UnityEngineAPI();
            timerManager = new TimerManager();
            player = new Player(playerConfig);
            levelController = new LevelController(levelConfig, timerManager, spawnerArea, goalTriggerDispatcher, player, unityEngineAPI);
            leaderboard = new Leaderboard(player, levelController);
            leaderboard.Load();
            screenController = new ScreenController(player, levelController, startScreen, inGameScreen, structureBuilderConfig, leaderboard);
            structureBuilder = new StructureBuilder(structureBuilderConfig,timerManager, 
                player, levelController, unityEngineAPI, screenController, structureBuildRaycaster, 
                structureUpgradeRaycaster, structureDestroyRaycaster);
            
            timerManager.Initialize();
            
        }
    }
}
