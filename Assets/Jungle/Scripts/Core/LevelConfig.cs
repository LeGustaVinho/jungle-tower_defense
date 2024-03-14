using System;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    [Serializable]
    public class RandomWeightNpcConfig : IRandomWeight
    {
        [SerializeField] private float weight;
        public float Weight => weight;
        public NpcConfig Config;
    }
    
    [CreateAssetMenu(menuName = "Jungle/Create LevelConfig", fileName = "New LevelConfig")]
    public class LevelConfig : SerializedScriptableObject
    {
        public StructureEntity StructurePrefab;

        public EntityConfig EntityConfig;

        public List<RandomWeightNpcConfig> SpawnChance = new List<RandomWeightNpcConfig>();

        [SuffixLabel("seconds")]
        public float RoundTime;

        [SuffixLabel("seconds")]
        public float SpawnInterval;
    }
}