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

        private Entity self;
        private ITimerManager timerManager;
        private ITargetSystem targetSystem;
        private IProjectileSystem projectileSystem;

        private Timer scanTargetTimer;
        private Timer attackTargetTimer;

        public CombatSystem(Entity self, ITimerManager timerManager, ITargetSystem targetSystem, IProjectileSystem projectileSystem)
        {
            this.self = self;
            this.timerManager = timerManager;
            this.targetSystem = targetSystem;
            this.projectileSystem = projectileSystem;
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
            if (self.Attributes[EntityAttribute.HealthPoints] - damageToReceive <= 0)
            {
                self.Attributes[EntityAttribute.HealthPoints] = 0;
                OnDie?.Invoke(self, source);
            }
            else
            {
                self.Attributes[EntityAttribute.HealthPoints] -= damageToReceive;
                OnTakeDamage?.Invoke(self, source);
            }
        }
        
        public void DoDamage(Entity target, float damageToDo)
        {
            if (!target.IsAlive || !target.gameObject.activeInHierarchy) return;
            
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
            if (targetDistance > self.Attributes[EntityAttribute.Range])
            {
                ScanForTargets();
                return; //Out of range
            }

            if (!target.IsAlive || !target.gameObject.activeInHierarchy)
            {
                ScanForTargets();
                return; //Target is dead
            }
            
            projectileSystem.Shoot(target, () =>
            {
                DoDamage(target, self.Attributes[EntityAttribute.Damage]);
            });
            
            attackTargetTimer = timerManager.SetTimer(self.Attributes[EntityAttribute.AttackSpeed], () => AttackTarget(target)); //Wait until attack is available and attack same target
        }

        public void ScanForTargets()
        {
            if (attackTargetTimer != null)
            {
                timerManager.AbortTimer(attackTargetTimer);
                attackTargetTimer = null;
            }
            
            if(!IsEnable) return;

            List<Entity> nearbyTargets = targetSystem.FindEntitiesInAreaRadius<Entity>(self.Position, self.Attributes[EntityAttribute.Range]);
            Entity closestTarget = TargetSystem.FindNearestEntityToPoint(nearbyTargets, self.Position);

            if (closestTarget != null)
            {
                if (scanTargetTimer != null)
                {
                    timerManager.AbortTimer(scanTargetTimer);
                    scanTargetTimer = null;
                }

                AttackTarget(closestTarget);
                return;
            }
            
            if (IsEnable && nearbyTargets.Count == 0)
            {
                scanTargetTimer = timerManager.SetTimer(self.Attributes[EntityAttribute.ScanTargetInterval], ScanForTargets);
            }
        }
    }
}
