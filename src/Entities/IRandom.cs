namespace PoeCraftLib.Entities
{
    public interface IRandom
    {
        int Next();
        int Next(int max);
        int Next(int min, int max);
        double NextDouble();
    }
}
