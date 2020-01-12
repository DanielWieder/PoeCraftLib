namespace PoeCraftLib.Simulator.Model.Crafting
{
    // An affix restriction limits what modifiers can be applied to an item
    public class AffixRestriction
    {
        public enum AffixRestrictionType
        {
            Override,
            Modifier
        }

        public int Weight { get; set; }
        public int SpawnTagId { get; set; }
        public AffixRestrictionType Type { get; set; }
    }
}
