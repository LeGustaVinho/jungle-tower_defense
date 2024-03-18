using System;
using System.Collections;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public class StructureEntity : Entity
    {
        [SerializeField] [BoxGroup("Structure")]
        private Transform ProjectileStartPoint;

        [ShowInInspector]
        [BoxGroup("Structure")]
        public StructureConfig structureConfig { private set; get; }

        [SerializeField] [BoxGroup("Structure")]
        private TextMeshProUGUI levelText;

        [SerializeField] [BoxGroup("Structure")]
        private TextMeshProUGUI nameText;

        public override void Initialize(EntityConfig config, int level, ITimerManager timerManager, 
                IUnityEngineAPI unityEngineAPI)
        {
            base.Initialize(config, level, timerManager, unityEngineAPI);

            structureConfig = config as StructureConfig;

            IProjectileSystem projectileSystem =
                new ProjectileSystem(structureConfig.ProjectilePrefab, ProjectileStartPoint.position,
                    Attributes[EntityAttribute.ProjectileSpeed]);

            CombatSystemComponent =
                new CombatSystem(this, timerManager, structureConfig.TargetSystem, projectileSystem, unityEngineAPI);
            CombatSystemComponent.Enable();

            nameText.text = structureConfig.DisplayName;
            levelText.text = level.ToString();
        }

        public void Enable()
        {
            CombatSystemComponent.Enable();
        }
        
        public void Disable()
        {
            CombatSystemComponent.Disable();
        }

        public void Upgrade()
        {
            Level++;
            levelText.text = Level.ToString();
            Attributes.Clear();
            foreach (KeyValuePair<EntityAttribute, SimpleAttribute> pair in Config.LevelAttributes)
            {
                Attributes.Add(pair.Key, pair.Value.GetValueForLevel(Level));
            }
        }

        protected override void OnDestroy()
        {
            CombatSystemComponent?.Disable();
        }
    }
}