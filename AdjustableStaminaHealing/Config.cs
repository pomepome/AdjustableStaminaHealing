﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustableStaminaHealing
{
    public class Config
    {
        public double HealingValuePerSeconds { get; set; } = 0.5f;
        public bool NotHealWhileMoving { get; set; } = true;
        public Keys IncreaseKey{ get; set; } = Keys.O;
        public Keys DecreaseKey { get; set; } = Keys.P;
    }
}
