using System;
using System.Collections;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public class StructureEntity : Entity
    {
        [SerializeField] private TargetSystem targetSystem;
        [SerializeField] private ProjectileEntity projectileEntityPrefab;
        [SerializeField] private Transform ProjectileStartPoint;

        public override void Initialize(EntityConfig config, int level, ITimerManager timerManager)
        {
            base.Initialize(config, level, timerManager);
            
            IProjectileSystem projectileSystem =
                new ProjectileSystem(projectileEntityPrefab, ProjectileStartPoint.position, Attributes[EntityAttribute.ProjectileSpeed]);
            
            CombatSystemComponent = new CombatSystem(this, timerManager, targetSystem, projectileSystem);
            CombatSystemComponent.Enable();
        }
    }
}