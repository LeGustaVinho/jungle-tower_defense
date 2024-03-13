using System;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    [Serializable]
    public class CombatSystem
    {
        public event Action<Entity, Entity> OnTakeDamage;
        public event Action<Entity, Entity> OnDoDamage;
        public event Action<Entity, Entity> OnDie;

        [ShowInInspector]
        public bool IsEnable { private set; get; }

        public bool IsAlive => combatAttributes.IsAlive;

        private Entity self;
        private ITimerManager timerManager;
        private ITargetSystem targetSystem;
        private IProjectileSystem projectileSystem;
        private CombatAttributes combatAttributes;

        public CombatSystem(Entity self, CombatAttributes combatAttributes, ITimerManager timerManager, ITargetSystem targetSystem, IProjectileSystem projectileSystem)
        {
            this.self = self;
            this.timerManager = timerManager;
            this.targetSystem = targetSystem;
            this.projectileSystem = projectileSystem;
            this.combatAttributes = combatAttributes;
        }

        public void Enable()
        {
            IsEnable = true;
            ScanForTargets();
        }
        
        public void Disable()
        {
            IsEnable = false;
        }

        public void ReceiveDamage(float damageToReceive, Entity source)
        {
            if (combatAttributes.CurrentHealthPoints - damageToReceive <= 0)
            {
                combatAttributes.CurrentHealthPoints = 0;
                OnDie?.Invoke(self, source);
            }
            else
            {
                combatAttributes.CurrentHealthPoints -= damageToReceive;
                OnTakeDamage?.Invoke(self, source);
            }
        }
        
        public void DoDamage(Entity target, float damageToDo)
        {
            target.CombatSystemComponent.ReceiveDamage(damageToDo, self);
            OnDoDamage?.Invoke(self, target);
        }

        public void AttackTarget(Entity target)
        {
            if (!IsEnable)
            {
                return; //Combat is disabled
            }
            
            float targetDistance = Vector3.Distance(target.Position, self.Position);
            if (targetDistance > combatAttributes.Range)
            {
                ScanForTargets();
                return; //Out of range
            }

            if (!target.CombatSystemComponent.IsAlive)
            {
                ScanForTargets();
                return; //Target is dead
            }
            
            projectileSystem.Shoot(target, () =>
            {
                DoDamage(target, combatAttributes.Damage);
            });
            
            timerManager.SetTimer(combatAttributes.AttackSpeed, () => AttackTarget(target)); //Wait until attack is available and attack same target
        }

        public void ScanForTargets()
        {
            if(!IsEnable) return;

            List<Entity> nearbyTargets = targetSystem.FindEntitiesInAreaRadius(self.Position, combatAttributes.Range);
            Entity closestTarget = TargetSystem.FindNearestEntityToPoint(nearbyTargets, self.Position);

            if (closestTarget != null)
            {
                AttackTarget(closestTarget);
                return;
            }
            
            if (IsEnable && nearbyTargets.Count == 0)
            {
                timerManager.SetTimer(combatAttributes.ScanTargetInterval, ScanForTargets);
            }
        }
    }
}
