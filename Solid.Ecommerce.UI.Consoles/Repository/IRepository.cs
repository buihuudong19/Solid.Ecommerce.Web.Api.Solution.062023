
using Solid.Ecommerce.UI.Consoles.Entities;

namespace Solid.Ecommerce.UI.Consoles.Repository;

public interface IRepository<T> where T : Person
{
    IList<T> GetAll();
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    //bo sung phuong thuc display
  
}
