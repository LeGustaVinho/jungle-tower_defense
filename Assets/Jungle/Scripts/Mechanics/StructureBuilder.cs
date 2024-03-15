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
        public StructureBuilderConfig StructureBuilderConfig;

        public List<StructureEntity> StructuresBuilt = new List<StructureEntity>();

        private ScreenToWorldInfo structureBuilderRaycaster;
        private ScreenToWorldInfo structureUpgradeRaycaster;
        private ScreenToWorldInfo structureDestroyRaycaster;
        private ITimerManager timerManager;
        private Player player;
        private LevelController levelController;
        private ScreenController screenController;
        private NavMeshSurface navMeshSurface;

        [ShowInInspector]
        private StructureConfig selectedStructureConfig;

        public StructureBuilder(StructureBuilderConfig structureBuilderConfig, ITimerManager timerManager, 
            Player player, LevelController levelController, ScreenController screenController,
            ScreenToWorldInfo structureBuilderRaycaster, ScreenToWorldInfo structureUpgradeRaycaster, 
            ScreenToWorldInfo structureDestroyRaycaster)
        {
            this.timerManager = timerManager;
            this.player = player;
            this.levelController = levelController;
            this.screenController = screenController;
            StructureBuilderConfig = structureBuilderConfig;
            this.structureBuilderRaycaster = structureBuilderRaycaster;
            this.structureUpgradeRaycaster = structureUpgradeRaycaster;
            this.structureDestroyRaycaster = structureDestroyRaycaster;
            navMeshSurface = Object.FindObjectOfType<NavMeshSurface>();
            
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

            foreach (StructureEntity structure in StructuresBuilt)
            {
                Object.Destroy(structure.gameObject);
            }
            StructuresBuilt.Clear();
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
                                      .GetValueForLevel(1)) * StructureBuilderConfig.StructureDestroyRefundFactor;
            player.Money += refundAmount;

            StructuresBuilt.Remove(structureEntity);
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
            Collider[] colliders = Physics.OverlapBox(pointSnappedToGrid, 
                Vector3.one * (StructureBuilderConfig.GridSnappingDistance * 0.4f), Quaternion.identity, 
                StructureBuilderConfig.StructureLayer, QueryTriggerInteraction.Collide);

            if (colliders.Length != 0) return;
            
            StructureEntity newStructure = Object.Instantiate(selectedStructureConfig.Prefab, pointSnappedToGrid, Quaternion.identity);
            newStructure.Initialize(selectedStructureConfig, 1, timerManager);

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
                StructuresBuilt.Add(newStructure);
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
            float x = Mathf.Round(position.x / StructureBuilderConfig.GridSnappingDistance) * StructureBuilderConfig.GridSnappingDistance;
            float z = Mathf.Round(position.z / StructureBuilderConfig.GridSnappingDistance) * StructureBuilderConfig.GridSnappingDistance;

            return new Vector3(x, position.y, z);
        }
        
        void DrawCube(Vector3 origin, Vector3 size, Color color, float duration)
        {
            // Metade do tamanho para calcular os vértices a partir do centro
            Vector3 halfSize = size * 0.5f;

            // Calcula os vértices do cubo
            Vector3[] vertices = new Vector3[8];
            vertices[0] = origin + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            vertices[1] = origin + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            vertices[2] = origin + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            vertices[3] = origin + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            vertices[4] = origin + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            vertices[5] = origin + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            vertices[6] = origin + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            vertices[7] = origin + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            // Desenha as linhas do cubo
            Debug.DrawLine(vertices[0], vertices[1], color, duration);
            Debug.DrawLine(vertices[1], vertices[2], color, duration);
            Debug.DrawLine(vertices[2], vertices[3], color, duration);
            Debug.DrawLine(vertices[3], vertices[0], color, duration);

            Debug.DrawLine(vertices[4], vertices[5], color, duration);
            Debug.DrawLine(vertices[5], vertices[6], color, duration);
            Debug.DrawLine(vertices[6], vertices[7], color, duration);
            Debug.DrawLine(vertices[7], vertices[4], color, duration);

            Debug.DrawLine(vertices[0], vertices[4], color, duration);
            Debug.DrawLine(vertices[1], vertices[5], color, duration);
            Debug.DrawLine(vertices[2], vertices[6], color, duration);
            Debug.DrawLine(vertices[3], vertices[7], color, duration);
        }
    }
}