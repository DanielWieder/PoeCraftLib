namespace PoeCraftLib.Data
{
    public interface IQueryObject<T>
    {
        T Execute();
    }
}
