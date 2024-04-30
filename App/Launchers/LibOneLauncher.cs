using System;
using System.Collections.Generic;
using LibOne;
using LibOne.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace App.Launchers;

public class LibOneLauncher : ILauncher
{
    private readonly ILogger<LibOneLauncher> _logger;

    private static readonly JsonSerializerSettings Settings = new()
    {
        Converters = new List<JsonConverter>
        {
            new EmployeeJsonConverter()
        }
    };

    public LibOneLauncher(ILogger<LibOneLauncher> logger)
    {
        _logger = logger;
    }

    public string Name => "Json.Net";
    
    public string[] DependsOn => [];

    public void Launch()
    {
        ConsoleColor.Green.WriteLine($"Using {nameof(LibOneLauncher)} based on {Name}");
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

    private static string SerializeEmployee(Employee employee) => JsonConvert.SerializeObject(employee, Settings);

    private static Employee DeserializeEmployee(string employeeJson) => JsonConvert.DeserializeObject<Employee>(employeeJson, Settings);
}