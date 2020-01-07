﻿using System.Collections.Generic;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Entities
{
    public class SimBaseItemInfo
    {
        public string ItemName { get; set; }
        public int ItemLevel { get; set; } = 84;
        public List<Influence> Influence { get; set; } = new List<Influence>();
        public double ItemCost { get; set; } = 0;
    }
}
