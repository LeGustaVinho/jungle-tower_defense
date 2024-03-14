using System.Collections;
using Jungle.Scripts.Core;
using Jungle.Scripts.Mechanics;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Jungle.Scripts.Entities
{
    public class NpcEntity : Entity, IPoolable
    {
        [BoxGroup("Npc Entity")]
        [SerializeField] private NavMeshAgent NavMeshAgent;
        
        public override void Initialize(EntityConfig config, int level, ITimerManager timerManager)
        {
            base.Initialize(config, level, timerManager);
            
            CombatSystemComponent = new CombatSystem(this, timerManager, null, null);

            NavMeshAgent.speed = Attributes[EntityAttribute.Speed];
            NavMeshAgent.angularSpeed = Attributes[EntityAttribute.AngularSpeed];
            NavMeshAgent.acceleration = Attributes[EntityAttribute.Acceleration];
        }

        public void SetAgentTarget(Vector3 position)
        {
            StartCoroutine(DelayedNavMeshAgentSetDestination(position));
        }

        public void OnConstruct()
        {
            
        }

        public void OnCreate()
        {
            
        }

        public void OnRecycle()
        {
            NavMeshAgent.enabled = false;
        }
        
        /// <summary>
        /// Wait a frame and set destination to NavMeshAgent component, this is required because Unity NavMesh system needs one frame to initialize and place agent on mesh after turned on the Agent component
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayedNavMeshAgentSetDestination(Vector3 position)
        {
            NavMeshAgent.enabled = true;
            yield return new WaitForEndOfFrame();
            NavMeshAgent.SetDestination(position);
        }
    }
}
