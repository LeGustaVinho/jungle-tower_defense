using System;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    public class ProjectileEntity : MonoBehaviour, IPoolable
    {
        public event Action OnHit;
        
        [ShowInInspector]
        private Entity Target;
        
        [ShowInInspector]
        private float Velocity = 5f;

        private Transform Transform;

        public void Initialize(Entity target, float velocity, Action onHit)
        {
            Transform = GetComponent<Transform>();

            Target = target;
            Velocity = velocity;
            OnHit = onHit;
        }
        
        private void Update()
        {
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            if (Target != null)
            {
                transform.position = Vector3.MoveTowards(Transform.position, Target.Position, Velocity * Time.deltaTime);
                
                if (Vector3.Distance(Transform.position, Target.Position) <= 0.5f)
                {
                    OnHit?.Invoke();
                    Pool.Destroy(gameObject);
                }
            }
            else
            {
                Pool.Destroy(gameObject);
            }
        }

        public void OnConstruct()
        {
        }

        public void OnCreate()
        {
        }

        public void OnRecycle()
        {
            Target = null;
            Velocity = 0;
            OnHit = null;
        }
    }
}