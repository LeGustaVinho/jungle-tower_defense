using System;
using System.Collections;
using Jungle.Scripts.Entities;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public class LevelController : MonoBehaviour
    {
        public StructureEntity StructurePrefab;
        public NpcEntity NpcPrefab;

        public BoxCollider SpawnerArea;
        public GoalTriggerDispatcher GoalTriggerDispatcher;
        public BoxCollider GoalCollider;

        public float SpawnInterval;
        private Coroutine spawnRoutine;

        public void Start()
        {
            
        }

        [Button]
        public void StartLevel()
        {
            spawnRoutine = StartCoroutine(NpcSpawnRoutine());
        }
        
        [Button]
        public void StopLevel()
        {
            StopCoroutine(spawnRoutine);
        }

        private IEnumerator NpcSpawnRoutine()
        {
            Vector3 randomSpawnPoint = SpawnerArea.bounds.RandomInsideBox();
            NpcEntity newNpc = Instantiate(NpcPrefab, randomSpawnPoint, Quaternion.identity);
            Vector3 randomTargetPoint = GoalCollider.bounds.RandomInsideBox();
            newNpc.SetAgentTarget(randomTargetPoint);
            
            yield return new WaitForSeconds(SpawnInterval);
            
            spawnRoutine = StartCoroutine(NpcSpawnRoutine());
        }
    }
}
