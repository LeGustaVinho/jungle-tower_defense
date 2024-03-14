using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jungle.Scripts.Entities
{
    public class Entity : MonoBehaviour, ICombatable
    {
        public EntityConfig Config;
        
        [SerializeField] 
        private Transform Transform;

        public Vector3 Position => Transform.position;
        
        public CombatSystem CombatSystemComponent { protected set; get; }
        
        [ShowInInspector]
        public Dictionary<EntityAttribute, float> Attributes = new Dictionary<EntityAttribute, float>();

        public int Level;

        public bool IsAlive => Attributes[EntityAttribute.HealthPoints] > 0;

        protected ITimerManager timerManager;

        public virtual void Initialize(EntityConfig config, int level, ITimerManager timerManager)
        {
            Config = config;
            Level = level;
            this.timerManager = timerManager;
            Attributes.Clear();

            foreach (KeyValuePair<EntityAttribute, SimpleAttribute> pair in Config.LevelAttributes)
            {
                Attributes.Add(pair.Key, pair.Value.GetValueForLevel(level));
            }
        }
    }
}