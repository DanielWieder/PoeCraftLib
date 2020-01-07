namespace PoeCraftLib.Data.Query
{
    public interface IQueryObject<T>
    {
        T Execute();
    }
}
