using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCraftLib.Entities.Constants
{
    public static class AffixTypesByStat
    {
        public static readonly List<string> PercentDefense = new List<string>
        {
            "LocalPhysicalDamageReductionRatingPercent",
            "LocalEvasionRatingIncreasePercent",
            "LocalEnergyShieldPercent",
            "LocalArmourAndEvasion",
            "LocalArmourAndEnergyShield",
            "LocalEvasionAndEnergyShield",
            "LocalArmourAndEvasionAndEnergyShield"
        };

        public static readonly List<string> PercentHybridDefense = new List<string>
        {
            "LocalPhysicalDamageReductionRatingAndStunRecoveryPercent",
            "LocalEvasionRatingAndStunRecoveryIncreasePercent",
            "LocalEnergyShieldAndStunRecoveryPercent",
            "LocalArmourAndEvasionAndStunRecovery",
            "LocalArmourAndEnergyShieldAndStunRecovery",
            "LocalEvasionAndEnergyShieldAndStunRecovery",
            "LocalArmourAndEvasionAndEnergyShieldAndStunRecovery"
        };

        public static readonly List<string> FlatDefense = new List<string>
        {
            "LocalPhysicalDamageReductionRating",
            "EvasionRating",
            "LocalEnergyShield",
            "LocalBaseArmourAndEvasionRating",
            "LocalBaseArmourAndEnergyShield",
            "LocalBaseEvasionRatingAndEnergyShield"
        };

        public static readonly List<string> FlatHybridDefense = new List<string>
        {
            "LocalBaseArmourAndLife",
            "LocalBaseEvasionRatingAndLife",
            "LocalBaseEnergyShieldAndLife"
        };

        public static readonly List<String> PercentDefenseSuffix = new List<string>
        {
            "LocalPhysicalDamageReductionRatingPercentSuffix",
            "LocalEvasionRatingIncreasePercentSuffix",
            "LocalEnergyShieldPercentSuffix",
            "LocalEvasionAndEnergyShieldSuffix",
            "LocalArmourAndEvasionSuffix",
            "LocalArmourAndEnergyShieldSuffix",
            "LocalArmourAndEvasionAndEnergyShieldSuffix"

        };

        public static readonly List<string> FlatDamage = new List<string>
        {
            "LocalFetchChaosDamage",
            "LocalFetchColdDamage",
            "LocalFetchFireDamage",
            "LocalFetchLightningDamage",
            "LocalFetchPhysicalDamage",
            "LocalFetchChaosDamageTwoHand",
            "LocalFetchColdDamageTwoHand",
            "LocalFetchFireDamageTwoHand",
            "LocalFetchLightningDamageTwoHand",
            "LocalFetchPhysicalDamageTwoHand",
        };

        public static List<string> EnergyShieldPercent => PercentDefense.Where(x => x.Contains("EnergyShield")).ToList();
        public static List<string> EvasionPercent => PercentDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> ArmourPercent => PercentDefense.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();

        public static List<string> EnergyShieldPercentSuffix => PercentDefenseSuffix.Where(x => x.Contains("EnergyShield")).ToList();
        public static List<string> EvasionPercentSuffix => PercentDefenseSuffix.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> ArmourPercentSuffix => PercentDefenseSuffix.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();


        public static List<string> HybridEnergyShieldPercent => PercentHybridDefense.Where(x => x.Contains("EnergyShield")).ToList();
        public static List<string> HybridEvasionPercent => PercentHybridDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> HybridArmourPercent => PercentHybridDefense.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();

        public static List<string> EnergyShieldFlat => FlatDefense.Where(x => x.Contains("EnergyShield")).ToList();
        public static List<string> EvasionFlat => FlatDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> ArmourFlat => FlatDefense.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();

        public static List<string> HybridEnergyShieldFlat => FlatHybridDefense.Where(x => x.Contains("EnergyShield")).ToList();
        public static List<string> HybridEvasionFlat => FlatHybridDefense.Where(x => x.Contains("Evasion")).ToList();
        public static List<string> HybridArmourFlat => FlatHybridDefense.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();


        public static List<string> FlatLightningDamage => FlatDamage.Where(x => x.Contains("Lightning")).ToList();
        public static List<string> FlatColdDamage => FlatDamage.Where(x => x.Contains("Cold")).ToList();
        public static List<string> FlatFireDamage => FlatDamage.Where(x => x.Contains("Fire")).ToList();
        public static List<string> FlatChaosDamage => FlatDamage.Where(x => x.Contains("Chaos")).ToList();
        public static List<string> FlatPhysicalDamage => FlatDamage.Where(x => x.Contains("Physical")).ToList();


    }
}
