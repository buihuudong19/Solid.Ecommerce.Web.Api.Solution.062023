

namespace Solid.Ecommerce.UI.Consoles.Entities;

public class Person
{
    //Co gi: => thuoc tinh, Lam gi? phuong thuc
    public string Name { get; set; }
    public string Address { get; set; }

    public Person()
    {
    }

    public Person(string name, string address)
    {
        Name = name;
        Address = address;
    }

    public override string? ToString()
        => $"Full Name: {Name}, Address: {Address}";
   
}
