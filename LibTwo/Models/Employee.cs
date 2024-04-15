namespace LibTwo.Models;

public class Employee
{
    private Employee(long id, string firstName, string lastName, Address address)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Address = address;
    }

    public long Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Address Address { get; }

    public override string ToString()
    {
        return $"{Id} - {FirstName} {LastName}";
    }

    public static Employee Create(long id, string firstName, string lastName, Address address)
    {
        return new Employee(id, firstName, lastName, address);
    }
}