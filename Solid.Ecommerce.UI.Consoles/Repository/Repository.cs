namespace Solid.Ecommerce.UI.Consoles.Repository;
public class Repository<T> : IRepository<T> where T : Person, new()
{
    private IList<T> data;

    public Repository()
    {
        data = new List<T>();
    }

    public void Add(T entity) => data.Add(entity);

    public void Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public IList<T> GetAll() => data;
    

    public T GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(T entity)
    {
        throw new NotImplementedException();
    }
}
