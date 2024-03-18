using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public interface IEntity
    {
        EntityConfig Config { get; set; }
        Vector3 Position { get; }
        ICombatSystem CombatSystemComponent { get; }
        Dictionary<EntityAttribute, float> Attributes { get; }
        int Level { get; set; }
        bool IsAlive { get; }
        void Initialize(EntityConfig config, int level, ITimerManager timerManager, 
            IUnityEngineAPI unityEngineAPI);
        
        GameObject GameObject { get; }
    }

    public class Entity : MonoBehaviour, ICombatable, IEntity
    {
        [BoxGroup("Entity")][ShowInInspector]
        public EntityConfig Config { get; set; }
        
        [SerializeField] [BoxGroup("Entity")]
        private Transform Transform;

        public Vector3 Position => Transform != null ? Transform.position : Vector3.zero;
        
        public ICombatSystem CombatSystemComponent { protected set; get; }

        [ShowInInspector] [BoxGroup("Entity")]
        public Dictionary<EntityAttribute, float> Attributes { get; } = new Dictionary<EntityAttribute, float>();

        [BoxGroup("Entity")][ShowInInspector]
        public int Level { get; set; }

        [ShowInInspector]
        public bool IsAlive => Attributes[EntityAttribute.HealthPoints] > 0;

        public GameObject GameObject => this.gameObject;

        protected ITimerManager TimerManager;
        protected IUnityEngineAPI UnityEngineAPI;

        public virtual void Initialize(EntityConfig config, int level, ITimerManager timerManager, IUnityEngineAPI unityEngineAPI)
        {
            Config = config;
            Level = level;
            TimerManager = timerManager;
            UnityEngineAPI = unityEngineAPI;
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