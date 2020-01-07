using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCraftLib.Entities.Constants
{
    public static class AffixGroupings
    {
        public static readonly List<string> PercentDefense = new List<string>
        {
            "Local Increased Evasion Rating Percent",
            "Local Increased Energy Shield Percent",
            "Local Increased Physical Damage Reduction Rating Percent",
            "Str Master Armour Percent Crafted",
            "Str Master Evasion Percent Crafted",
            "Local Increased Armour And Evasion",
            "Local Increased Energy Shield Percent",
            "Local Increased Evasion Rating Percent",
            "Local Increased Armour And Energy Shield",
            "Str Master Energy Shield Percent Crafted",
            "Local Increased Evasion And Energy Shield",
            "Local Increased Armour Evasion Energy Shield",
            "Str Master Armour And Evasion Percent Crafted",
            "Str Master Armour And Energy Shield Percent Crafted",
            "Str Master Evasion And Energy Shield Percent Crafted",
            "Local Increased Physical Damage Reduction Rating Percent"
        };

        public static readonly List<string> HybridDefense = new List<string>
        {
            "Local Increased Armour And Evasion And Stun Recovery",
            "Local Increased Energy Shield Percent And Stun Recovery",
            "Local Increased Evasion Rating Percent And Stun Recovery",
            "Local Increased Armour And Energy Shield And Stun Recovery",
            "Local Increased Evasion And Energy Shield And Stun Recovery",
            "Local Increased Armour Evasion Energy Shield Stun Recovery",
            "Local Increased Physical Damage Reduction Rating Percent And Stun Recovery"
        };

        public static readonly List<string> FlatDamage = new List<string>
        {
            "Local Added Chaos Damage",
            "Local Added Cold Damage",
            "Local Added Fire Damage",
            "Local Added Lightning Damage",
            "Local Added Physical Damage",
            "Local Added Chaos Damage Two Hand",
            "Local Added Cold Damage Two Hand",
            "Local Added Fire Damage Two Hand",
            "Local Added Lightning Damage Two Hand",
            "Local Added Physical Damage Two Hand",
        };

        public static List<string> EnergyShieldPercent => PercentDefense.Where(x => x.Contains("Energy Shield")).ToList();
        public static List<string> EvasionPercent => PercentDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> ArmourPercent => PercentDefense.Where(x => x.Contains("Armour") || x.Contains("Physical Damage Reduction Rating")).ToList();

        public static List<string> HybridEnergyShieldPercent => HybridDefense.Where(x => x.Contains("Energy Shield")).ToList();
        public static List<string> HybridEvasionPercent => HybridDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> HybridArmourPercent => HybridDefense.Where(x => x.Contains("Armour") || x.Contains("Physical Damage Reduction Rating")).ToList();

        public static List<string> FlatLightningDamage => FlatDamage.Where(x => x.Contains("Lightning")).ToList();
        public static List<string> FlatColdDamage => FlatDamage.Where(x => x.Contains("Cold")).ToList();
        public static List<string> FlatFireDamage => FlatDamage.Where(x => x.Contains("Fire")).ToList();
        public static List<string> FlatChaosDamage => FlatDamage.Where(x => x.Contains("Chaos")).ToList();
        public static List<string> FlatPhysicalDamage => FlatDamage.Where(x => x.Contains("Physical")).ToList();


    }
}
