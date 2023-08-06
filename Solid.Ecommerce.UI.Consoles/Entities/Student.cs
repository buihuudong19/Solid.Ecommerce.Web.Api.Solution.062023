
namespace Solid.Ecommerce.UI.Consoles.Entities;

public class Student:Person
{
    public int StudentId { get; set; }
    public double Mark { get; set; }

    public Student()
    {
    }

    public Student(int studentId, double mark)
    {
        StudentId = studentId;
        Mark = mark;
    }
    public Student(int studentId, string name, string address, double mark):base(name,address)
    {
        StudentId = studentId;
        Mark = mark;
    }

    public override string? ToString()
        => $"Id: {StudentId} {base.ToString()} and Mark: {Mark}";


    
}
