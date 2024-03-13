using System;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jungle.Scripts.Entities
{
    [Serializable]
    public class Npc
    {
        [FoldoutGroup("Moviment")]
        public float Speed;
        [FoldoutGroup("Moviment")]
        public float AngularSpeed;
        [FoldoutGroup("Moviment")]
        public float Acceleration;

        public CombatAttributes CombatAttributes;
    }
}
