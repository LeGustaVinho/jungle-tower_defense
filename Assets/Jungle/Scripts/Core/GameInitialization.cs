using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public class GameInitialization : MonoBehaviour
    {
        public StructureEntity StructurePrefab;
        public NpcEntity NpcPrefab;

        public BoxCollider SpawnerArea;
        public GoalTriggerDispatcher GoalTriggerDispatcher;
    }
}
