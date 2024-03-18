using System;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public interface ICombatSystem
    {
        event Action<IEntity, IEntity> OnTakeDamage;
        event Action<IEntity, IEntity> OnDoDamage;
        event Action<IEntity, IEntity> OnDie;
        bool IsEnable { get; }
        void Enable();
        void Disable();
        void AttackTarget(IEntity target);
        void ReceiveDamage(float damageToReceive, IEntity source);
    }

    [Serializable]
    public class CombatSystem : ICombatSystem
    {
        public event Action<IEntity, IEntity> OnTakeDamage;
        public event Action<IEntity, IEntity> OnDoDamage;
        public event Action<IEntity, IEntity> OnDie;

        [ShowInInspector]
        public bool IsEnable { private set; get; }

        private IEntity self;
        private ITimerManager timerManager;
        private ITargetSystem targetSystem;
        private IProjectileSystem projectileSystem;
        private IUnityEngineAPI unityEngineAPI;

        private Timer scanTargetTimer;
        private Timer attackTargetTimer;

        public CombatSystem(IEntity self, ITimerManager timerManager, ITargetSystem targetSystem, 
            IProjectileSystem projectileSystem, IUnityEngineAPI unityEngineAPI)
        {
            this.self = self;
            this.timerManager = timerManager;
            this.targetSystem = targetSystem;
            this.projectileSystem = projectileSystem;
            this.unityEngineAPI = unityEngineAPI;
        }

        public void Enable()
        {
            IsEnable = true;
            ScanForTargets();
        }
        
        public void Disable()
        {
            IsEnable = false;
            
            if (attackTargetTimer != null)
            {
                timerManager.AbortTimer(attackTargetTimer);
                attackTargetTimer = null;
            }
            
            if (scanTargetTimer != null)
            {
                timerManager.AbortTimer(scanTargetTimer);
                scanTargetTimer = null;
            }
        }
        
        public void AttackTarget(IEntity target)
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

            if (!target.IsAlive || !target.GameObject.activeInHierarchy)
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

        public void ReceiveDamage(float damageToReceive, IEntity source)
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
        
        private void DoDamage(IEntity target, float damageToDo)
        {
            if (!target.IsAlive || !target.GameObject.activeInHierarchy) return;
            
            target.CombatSystemComponent.ReceiveDamage(damageToDo, self);
            OnDoDamage?.Invoke(self, target);
        }

        private void ScanForTargets()
        {
            if (attackTargetTimer != null)
            {
                timerManager.AbortTimer(attackTargetTimer);
                attackTargetTimer = null;
            }
            
            if(!IsEnable) return;

            try
            {
                List<IEntity> nearbyTargets = targetSystem.FindEntitiesInAreaRadius(unityEngineAPI, self.Position, 
                    self.Attributes[EntityAttribute.Range]);
                IEntity closestTarget = TargetSystem.FindNearestEntityToPoint(nearbyTargets, self.Position);
                
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
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
