namespace PoeCrafting.Data
{
    public interface IQueryObject<T>
    {
        T Execute();
    }
}
