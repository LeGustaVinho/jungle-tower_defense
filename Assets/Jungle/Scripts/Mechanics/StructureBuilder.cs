using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using LegendaryTools.Input;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    [CreateAssetMenu(menuName = "Create StructureBuilder", fileName = "New StructureBuilder", order = 0)]
    public class StructureBuilder : ScriptableObject
    {
        public LayerMask StructureLayer;
        public float GridSnappingDistance = 0.5f;
        public List<StructureConfig> AvailableStructures = new List<StructureConfig>();

        public List<StructureEntity> StructuresBuilt = new List<StructureEntity>();

        private ITimerManager timerManager;

        public void Initialize(ITimerManager timerManager)
        {
            this.timerManager = timerManager;
            ScreenToWorldInfo.Instance.On3DHit += OnTryToBuild;
        }

        private void OnTryToBuild(RaycastHit hitinfo)
        {
            Vector3 pointSnappedToGrid = SnapToGridKeepY(hitinfo.point);
            Collider[] colliders = Physics.OverlapBox(pointSnappedToGrid, Vector3.one * (GridSnappingDistance * 0.25f), Quaternion.identity, StructureLayer);

            if (colliders.Length != 0) return;
            
            StructureEntity newStructure = Instantiate(AvailableStructures[0].Prefab, pointSnappedToGrid, Quaternion.identity);
            newStructure.Initialize(AvailableStructures[0], 1, timerManager);
        }

        public Vector3 SnapToGrid(Vector3 position)
        {
            // Arredonde cada componente do ponto de entrada para o múltiplo mais próximo de 'gridSpacing'.
            float x = Mathf.Round(position.x / GridSnappingDistance) * GridSnappingDistance;
            float y = Mathf.Round(position.y / GridSnappingDistance) * GridSnappingDistance;
            float z = Mathf.Round(position.z / GridSnappingDistance) * GridSnappingDistance;

            return new Vector3(x, y, z);
        }
        
        public Vector3 SnapToGridKeepY(Vector3 position)
        {
            // Arredonde cada componente do ponto de entrada para o múltiplo mais próximo de 'gridSpacing'.
            float x = Mathf.Round(position.x / GridSnappingDistance) * GridSnappingDistance;
            float z = Mathf.Round(position.z / GridSnappingDistance) * GridSnappingDistance;

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