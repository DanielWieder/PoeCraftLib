﻿using System;
using System.Collections.Generic;

namespace PoeCraftLib.Entities.Items
{
    public class Fossil
    {
        // Several properties are unused because they are only used for fossils that use unsupported features
        // Forced Mods is used for Bloodstained (Implicits) and Hallow (Socket)
        // ChangesQuality is used for Perfect (Quality)
        // Mirrors is used for Fractured (Mirror)

        public string FullName { get; set; }

        public List<Affix> AddedAffixes { get; set; }

        public Dictionary<string, int> ModWeightModifiers { get; set; }

        public int CorruptedEssenceChance { get; set; }

        public List<String> AllowedTags { get; set; }

        public bool RollsLucky { get; set; }

        public HashSet<String> ForbiddenItemClasses { get; set; }
        public string Name { get; set; }
    }
}
