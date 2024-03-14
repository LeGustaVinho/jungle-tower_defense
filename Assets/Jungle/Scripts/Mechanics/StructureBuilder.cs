using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using LegendaryTools.Input;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public class StructureBuilder
    {
        public StructureBuilderConfig StructureBuilderConfig;

        public List<StructureEntity> StructuresBuilt = new List<StructureEntity>();

        private ScreenToWorldInfo structureBuilderRaycaster;
        private ScreenToWorldInfo structureUpgradeRaycaster;
        private ITimerManager timerManager;
        private Player player;

        public StructureBuilder(StructureBuilderConfig structureBuilderConfig, ITimerManager timerManager, 
            Player player, ScreenToWorldInfo structureBuilderRaycaster, ScreenToWorldInfo structureUpgradeRaycaster)
        {
            this.timerManager = timerManager;
            this.player = player;
            StructureBuilderConfig = structureBuilderConfig;
            this.structureBuilderRaycaster = structureBuilderRaycaster;
            this.structureUpgradeRaycaster = structureUpgradeRaycaster;
            structureBuilderRaycaster.On3DHit += OnTryToBuild;
            structureUpgradeRaycaster.On3DHit += OnTryToUpgrade;
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

        private void OnTryToBuild(RaycastHit hitinfo)
        {
            float buildCost = StructureBuilderConfig.AvailableStructures[0].LevelAttributes[EntityAttribute.Cost]
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
            
            StructureEntity newStructure = Object.Instantiate(StructureBuilderConfig.AvailableStructures[0].Prefab, pointSnappedToGrid, Quaternion.identity);
            newStructure.Initialize(StructureBuilderConfig.AvailableStructures[0], 1, timerManager);
        }

        public Vector3 SnapToGrid(Vector3 position)
        {
            float x = Mathf.Round(position.x / StructureBuilderConfig.GridSnappingDistance) * StructureBuilderConfig.GridSnappingDistance;
            float y = Mathf.Round(position.y / StructureBuilderConfig.GridSnappingDistance) * StructureBuilderConfig.GridSnappingDistance;
            float z = Mathf.Round(position.z / StructureBuilderConfig.GridSnappingDistance) * StructureBuilderConfig.GridSnappingDistance;

            return new Vector3(x, y, z);
        }
        
        public Vector3 SnapToGridKeepY(Vector3 position)
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