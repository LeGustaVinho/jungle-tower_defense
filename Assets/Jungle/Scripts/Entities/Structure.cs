using System;
using Jungle.Scripts.Mechanics;
using Sirenix.OdinInspector;

namespace Jungle.Scripts.Entities
{
    [Serializable]
    public class Structure
    {
        public CombatAttributes CombatAttributes;

        [FoldoutGroup("Economy")]
        public float BaseUpradeCost;
    }
}
