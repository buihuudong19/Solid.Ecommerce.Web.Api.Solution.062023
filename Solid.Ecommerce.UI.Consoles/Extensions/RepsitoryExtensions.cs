
namespace Solid.Ecommerce.UI.Consoles.Extensions;
public static class RepsitoryExtensions
{
    public static void Display<T>(this IRepository<T> repository) where T : Person
    {
        foreach (var item in repository.GetAll()) {
           Console.WriteLine(item);
        }
    }
}
