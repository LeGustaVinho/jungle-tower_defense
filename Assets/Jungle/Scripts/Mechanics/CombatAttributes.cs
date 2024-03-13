using System;
using Sirenix.OdinInspector;

namespace Jungle.Scripts.Mechanics
{
    [Serializable]
    public class CombatAttributes
    {
        public float Damage;
        public float AttackSpeed;

        [SuffixLabel("unity units"), PropertyRange(1, 200)]
        public float Range;

        [SuffixLabel("unity units"), MinValue(1)]
        public float ProjectileSpeed;

        [SuffixLabel("seconds")]
        public float ScanTargetInterval;

        public float MaxHealthPoints;

        [PropertyRange(0, "MaxHealthPoints")]
        public float CurrentHealthPoints;

        public CombatAttributes()
        {
            CurrentHealthPoints = MaxHealthPoints;
        }

        public bool IsAlive => CurrentHealthPoints > 0;
    }
}