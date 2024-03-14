using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    [Serializable]
    public class SimpleAttribute
    {
        public float BaseValue;

        public float FlatValuePerLevel;
        [SuffixLabel("* 100 %")]
        public float FactorValuePerLevel;

        public float GetValueForLevel(int level)
        {
            return (BaseValue + FlatValuePerLevel * level) * (1 + FactorValuePerLevel * level);
        }
    }
}