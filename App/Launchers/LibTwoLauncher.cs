using System;
using System.Text.Json;
using LibTwo;
using LibTwo.Models;
using Microsoft.Extensions.Logging;

namespace App.Launchers;

public class LibTwoLauncher : ILauncher
{
    private readonly ILogger<LibTwoLauncher> _logger;

    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new EmployeeJsonConverter() }
    };

    public LibTwoLauncher(ILogger<LibTwoLauncher> logger)
    {
            _logger = logger;
        }

    public string Name => "System.Text.Json";

    public void Launch()
    {
            ConsoleColor.Green.WriteLine($"Using {nameof(LibTwoLauncher)} based on {Name}");
            var employeeJson = SerializeEmployee(CreateEmployee());
            var employee = DeserializeEmployee(employeeJson);
            using (_logger.BeginScope(Name))
            {
                _logger.LogInformation("Employee: {employee}", employee);
                _logger.LogInformation("Json: {json}", employeeJson);
            }
        }

    private static Employee CreateEmployee()
    {
            var address = Address.Create("1", "Paris", "France");
            var employee = Employee.Create(1, "Jean", "Bryan", address);
            return employee;
        }

    private static string SerializeEmployee(Employee employee) => JsonSerializer.Serialize(employee, Options);

    private static Employee DeserializeEmployee(string employeeJson) => JsonSerializer.Deserialize<Employee>(employeeJson, Options);
}