using System.Collections;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using Jungle.Scripts.UI;
using LegendaryTools;
using LegendaryTools.Input;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Jungle.Scripts.Mechanics
{
    public class StructureBuilder
    {
        private StructureBuilderConfig structureBuilderConfig;
        private List<StructureEntity> structuresBuilt = new List<StructureEntity>();
        private ScreenToWorldInfo structureBuilderRaycaster;
        private ScreenToWorldInfo structureUpgradeRaycaster;
        private ScreenToWorldInfo structureDestroyRaycaster;
        private ITimerManager timerManager;
        private IPlayer player;
        private ILevelController levelController;
        private IUnityEngineAPI unityEngineAPI;
        private ScreenController screenController;

        [ShowInInspector]
        private StructureConfig selectedStructureConfig;

        public StructureBuilder(StructureBuilderConfig structureBuilderConfig, ITimerManager timerManager, 
            IPlayer player, ILevelController levelController, IUnityEngineAPI unityEngineAPI,
            ScreenController screenController, ScreenToWorldInfo structureBuilderRaycaster, 
            ScreenToWorldInfo structureUpgradeRaycaster, ScreenToWorldInfo structureDestroyRaycaster)
        {
            this.timerManager = timerManager;
            this.player = player;
            this.levelController = levelController;
            this.unityEngineAPI = unityEngineAPI;
            this.screenController = screenController;
            this.structureBuilderConfig = structureBuilderConfig;
            this.structureBuilderRaycaster = structureBuilderRaycaster;
            this.structureUpgradeRaycaster = structureUpgradeRaycaster;
            this.structureDestroyRaycaster = structureDestroyRaycaster;
            
            structureBuilderRaycaster.On3DHit += OnTryToBuild;
            structureUpgradeRaycaster.On3DHit += OnTryToUpgrade;
            structureDestroyRaycaster.On3DHit += OnTryToDestroy;

            levelController.OnStartGame += OnStartGame;
            levelController.OnFinishGame += OnFinishGame;

            screenController.OnStructureSelect += OnStructureSelect;
        }

        private void OnStartGame(int level)
        {
            structureBuilderRaycaster.CanInput = true;
            structureUpgradeRaycaster.CanInput = true;
            structureDestroyRaycaster.CanInput = true;
        }

        private void OnFinishGame(int level)
        {
            structureBuilderRaycaster.CanInput = false;
            structureUpgradeRaycaster.CanInput = false;
            structureDestroyRaycaster.CanInput = false;

            foreach (StructureEntity structure in structuresBuilt)
            {
                Object.Destroy(structure.gameObject);
            }
            structuresBuilt.Clear();
        }

        private void OnTryToUpgrade(RaycastHit hitinfo)
        {
            StructureEntity structureEntity = hitinfo.transform.GetComponent<StructureEntity>();
            if (structureEntity == null)
            {
                return;
            }
            
            float upgradeCost = structureEntity.structureConfig.LevelAttributes[EntityAttribute.UpgradeCost]
                .GetValueForLevel(structureEntity.Level + 1);
            if (player.Money > upgradeCost)
            {
                player.Money -= Mathf.FloorToInt(upgradeCost);
                structureEntity.Upgrade();
            }
            else
            {
                return;
            }
        }
        
        private void OnTryToDestroy(RaycastHit hitinfo)
        {
            StructureEntity structureEntity = hitinfo.transform.GetComponent<StructureEntity>();
            if (structureEntity == null)
            {
                return;
            }

            float refundAmount = (structureEntity.structureConfig.LevelAttributes[EntityAttribute.UpgradeCost]
                                      .GetValueForLevel(structureEntity.Level) +
                                  structureEntity.structureConfig.LevelAttributes[EntityAttribute.Cost]
                                      .GetValueForLevel(1)) * structureBuilderConfig.StructureDestroyRefundFactor;
            player.Money += refundAmount;

            structuresBuilt.Remove(structureEntity);
            Object.Destroy(structureEntity.gameObject);
        }

        private void OnTryToBuild(RaycastHit hitinfo)
        {
            if (selectedStructureConfig == null)
                return;
            
            float buildCost = selectedStructureConfig.LevelAttributes[EntityAttribute.Cost]
                .GetValueForLevel(0);
            if (player.Money > buildCost)
            {
                player.Money -= Mathf.FloorToInt(buildCost);
            }
            else
            {
                return;
            }
            
            Vector3 pointSnappedToGrid = SnapToGridKeepY(hitinfo.point);
            Collider[] colliders = unityEngineAPI.OverlapBox(pointSnappedToGrid, 
                Vector3.one * (structureBuilderConfig.GridSnappingDistance * 0.4f), Quaternion.identity, 
                structureBuilderConfig.StructureLayer, QueryTriggerInteraction.Collide);

            if (colliders.Length != 0) return;
            
            StructureEntity newStructure = Object.Instantiate(selectedStructureConfig.Prefab, pointSnappedToGrid, Quaternion.identity);
            newStructure.Initialize(selectedStructureConfig, 1, timerManager, unityEngineAPI);

            structureBuilderRaycaster.CanInput = false;
            MonoBehaviourFacade.Instance.StartCoroutine(CheckNavMeshPathBlocking(newStructure));
        }

        private IEnumerator CheckNavMeshPathBlocking(StructureEntity newStructure)
        {
            //Wait for NavMesh rebuild
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            
            Vector3 randomSpawnPoint = levelController.SpawnerArea.bounds.RandomInsideBox();
            Vector3 randomTargetPoint = levelController.GoalCollider.bounds.RandomInsideBox();
            
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(randomSpawnPoint, randomTargetPoint, NavMesh.AllAreas, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                newStructure.Enable();
                structuresBuilt.Add(newStructure);
            }
            else
            {
                Object.Destroy(newStructure.gameObject);
            }
            structureBuilderRaycaster.CanInput = true;
        }
        
        private void OnStructureSelect(StructureConfig structureConfig)
        {
            selectedStructureConfig = structureConfig;
        }
        
        private Vector3 SnapToGridKeepY(Vector3 position)
        {
            float x = Mathf.Round(position.x / structureBuilderConfig.GridSnappingDistance) * structureBuilderConfig.GridSnappingDistance;
            float z = Mathf.Round(position.z / structureBuilderConfig.GridSnappingDistance) * structureBuilderConfig.GridSnappingDistance;

            return new Vector3(x, position.y, z);
        }
    }
}