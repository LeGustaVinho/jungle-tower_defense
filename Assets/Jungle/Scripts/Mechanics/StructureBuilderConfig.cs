using System.Collections.Generic;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    [CreateAssetMenu(menuName = "Jungle/Create StructureBuilderConfig", fileName = "New StructureBuilderConfig", order = 0)]
    public class StructureBuilderConfig : ScriptableObject
    {
        public LayerMask StructureLayer;
        public float GridSnappingDistance = 0.5f;
        public float StructureDestroyRefundFactor = 0.5f;
        public List<StructureConfig> AvailableStructures = new List<StructureConfig>();
    }
}