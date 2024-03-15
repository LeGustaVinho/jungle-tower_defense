﻿using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public class Entity : MonoBehaviour, ICombatable
    {
        [BoxGroup("Entity")]
        public EntityConfig Config;
        
        [SerializeField] [BoxGroup("Entity")]
        private Transform Transform;

        public Vector3 Position => Transform != null ? Transform.position : Vector3.zero;
        
        public ICombatSystem CombatSystemComponent { protected set; get; }
        
        [ShowInInspector][BoxGroup("Entity")]
        public Dictionary<EntityAttribute, float> Attributes = new Dictionary<EntityAttribute, float>();

        [BoxGroup("Entity")]
        public int Level;

        public bool IsAlive => Attributes[EntityAttribute.HealthPoints] > 0;

        protected ITimerManager TimerManager;

        public virtual void Initialize(EntityConfig config, int level, ITimerManager timerManager)
        {
            Config = config;
            Level = level;
            TimerManager = timerManager;
            Attributes.Clear();

            foreach (KeyValuePair<EntityAttribute, SimpleAttribute> pair in Config.LevelAttributes)
            {
                Attributes.Add(pair.Key, pair.Value.GetValueForLevel(level));
            }
        }

        protected virtual void OnDestroy()
        {
            
        }
    }
}