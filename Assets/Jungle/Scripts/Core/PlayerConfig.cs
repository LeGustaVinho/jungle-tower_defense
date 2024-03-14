using UnityEngine;

namespace Jungle.Scripts.Core
{
    [CreateAssetMenu(menuName = "Jungle/Create PlayerConfig", fileName = "New PlayerConfig", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        public int StartingMoney;
        public int StartingHealthPoints;
    }
}