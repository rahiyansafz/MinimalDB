using MinimalDB.Exceptions;

namespace MinimalDB;
public class DbCollection<T> : List<T>, IDbCollection<T>
{
    public IEnumerable<T> FindByCondition(Func<T, bool> predicate)
    {
        var response = this.Where(predicate);

        if (response.Any())
            return response;
        else
            throw new NotFoundException($"{typeof(T)} which matches the condition given does not exist!");
    }
}