using Jungle.Scripts.Mechanics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jungle.Scripts.Entities
{
    public class Entity : MonoBehaviour, ICombatable
    {
        [SerializeField] 
        private Transform Transform;

        public Vector3 Position => Transform.position;
        
        public CombatSystem CombatSystemComponent { protected set; get; }
    }
}