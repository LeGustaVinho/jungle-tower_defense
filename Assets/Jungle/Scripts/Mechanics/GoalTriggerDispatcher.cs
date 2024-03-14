using System;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public class GoalTriggerDispatcher : MonoBehaviour
    {
        public event Action<Entity> OnTriggerEnterEvent; 
        
        protected void OnTriggerEnter(Collider other)
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity != null)
            {
                OnTriggerEnterEvent?.Invoke(entity);
            }
        }
    }
}
