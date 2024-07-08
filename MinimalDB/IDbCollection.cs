namespace MinimalDB;
public interface IDbCollection<T> : IList<T>
{
    IEnumerable<T> FindByCondition(Func<T, bool> predicate);
}