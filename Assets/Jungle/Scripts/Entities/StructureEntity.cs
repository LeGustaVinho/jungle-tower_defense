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
        public Structure Structure;
        [SerializeField] private TargetSystem targetSystem;
        [SerializeField] private ProjectileEntity projectileEntityPrefab;
        [SerializeField] private Transform ProjectileStartPoint;
        
        [Button]
        public void Initialize()
        {
            ITimerManager timerManager = new TimerManager();
            IProjectileSystem projectileSystem =
                new ProjectileSystem(projectileEntityPrefab, ProjectileStartPoint.position, Structure.CombatAttributes.ProjectileSpeed);
            
            CombatSystemComponent = new CombatSystem(this, Structure.CombatAttributes, timerManager, targetSystem, projectileSystem);
            
            timerManager.Initialize();
            CombatSystemComponent.Enable();
        }
    }
}