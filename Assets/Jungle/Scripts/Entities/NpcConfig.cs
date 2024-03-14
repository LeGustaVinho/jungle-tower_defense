using System.Collections.Generic;
using Jungle.Scripts.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Entities
{
    [CreateAssetMenu(menuName = "Jungle/Create NpcConfig", fileName = "New NpcConfig")]
    public class NpcConfig : EntityConfig
    {
        public NpcEntity Prefab;
    }
}