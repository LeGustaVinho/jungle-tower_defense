﻿using Jungle.Scripts.Mechanics;

namespace Jungle.Scripts.Entities
{
    public interface ICombatable
    {
        public ICombatSystem CombatSystemComponent { get; }
    }
}