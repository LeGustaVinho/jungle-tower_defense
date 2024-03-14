using UnityEngine;

namespace Jungle.Scripts.Entities
{
    [CreateAssetMenu(menuName = "Create StructureConfig", fileName = "New StructureConfig", order = 0)]
    public class StructureConfig : EntityConfig
    {
        public StructureEntity Prefab;
    }
}