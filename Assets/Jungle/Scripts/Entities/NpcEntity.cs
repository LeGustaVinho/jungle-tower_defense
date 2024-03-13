using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using UnityEngine;
using UnityEngine.AI;

namespace Jungle.Scripts.Entities
{
    public class NpcEntity : Entity
    {
        public NavMeshAgent NavMeshAgent;
        public TargetSystem TargetSystem;
        public Npc Npc;

        public void SetAgentTarget(Vector3 position)
        {
            NavMeshAgent.SetDestination(position);
        }
        
        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            ITimerManager timerManager = new TimerManager();
            IProjectileSystem projectileSystem =
                new ProjectileSystem(null, Vector3.zero, 0);
            
            CombatSystemComponent = new CombatSystem(this, Npc.CombatAttributes, timerManager, TargetSystem, projectileSystem);
            
            timerManager.Initialize();
        }
    }
}
