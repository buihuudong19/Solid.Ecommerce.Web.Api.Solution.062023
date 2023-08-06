namespace Solid.Ecommerce.Application.Base;
public interface IBaseRepo<T> where T : class
{
    /*Return ra mot collection data kieu T*/
    DbSet<T> Entities { get; }

}
