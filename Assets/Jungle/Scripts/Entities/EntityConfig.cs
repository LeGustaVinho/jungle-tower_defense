using System.Collections.Generic;
using Jungle.Scripts.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public enum EntityAttribute
    {
        Speed,
        AngularSpeed,
        Acceleration,
        Damage,
        AttackSpeed,
        Range,
        HealthPoints,
        ScanTargetInterval,
        ProjectileSpeed,
    }
    
    public enum EntityType
    {
        Npc,
        Structure,
    }
    
    [CreateAssetMenu(menuName = "Jungle/Create EntityConfig", fileName = "New EntityConfig", order = 0)]
    public class EntityConfig : SerializedScriptableObject
    {
        public EntityType Type;
        
        [ShowInInspector]
        public Dictionary<EntityAttribute, SimpleAttribute> LevelAttributes = new Dictionary<EntityAttribute, SimpleAttribute>();
    }
}