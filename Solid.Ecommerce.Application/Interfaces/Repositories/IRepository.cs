
using Solid.Ecommerce.Application.Base;

namespace Solid.Ecommerce.Application.Interfaces.Repositories;
public interface IRepository<T>:
    IBaseRepo<T>,IBaseWriterRepository<T>,IBaseReaderRepository<T> where T : class
{
    
}
