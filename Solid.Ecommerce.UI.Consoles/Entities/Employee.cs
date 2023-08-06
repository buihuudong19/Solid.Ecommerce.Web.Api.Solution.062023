

namespace Solid.Ecommerce.UI.Consoles.Entities;
public class Employee:Person
{
   public int EmpId { get; set; }
   public double Salary { get;set; }

    public Employee()
    {
    }

    public Employee(int id, double salary)
    {
        EmpId = id;
        Salary = salary;
    }
    public Employee(int id, string name, string address, double salary):base(name,address)
    {
        EmpId = id;
        Salary = salary;
    }
    public override string? ToString()
        => $"Id: {EmpId} {base.ToString()} and Salary: {Salary}";
}
