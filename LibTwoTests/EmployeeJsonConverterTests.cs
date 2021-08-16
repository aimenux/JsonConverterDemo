using FluentAssertions;
using LibTwo;
using LibTwo.Models;
using System.Text.Json;
using Xunit;

namespace LibTwoTests
{
    public class EmployeeJsonConverterTests
    {
        [Fact]
        public void Should_Serialize_Null_Employee()
        {
            // arrange
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var json = JsonSerializer.Serialize(default(Employee), options);

            // assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("null");
        }

        [Fact]
        public void Should_Serialize_Employee_With_Null_Address()
        {
            // arrange
            var employee = Employee.Create(1, "Jean", "Snow", default(Address));
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var json = JsonSerializer.Serialize(employee, options);

            // assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":null}");
        }

        [Fact]
        public void Should_Serialize_Employee_With_NotNull_Address()
        {
            // arrange
            var address = Address.Create("1", "paris", "france");
            var employee = Employee.Create(1, "Jean", "Snow", address);
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var json = JsonSerializer.Serialize(employee, options);

            // assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":{\"Street\":\"1\",\"City\":\"paris\",\"Country\":\"france\"}}");
        }

        [Fact]
        public void Should_Deserialize_Null_Employee()
        {
            // arrange
            const string employeeJson = "null";
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var employee = JsonSerializer.Deserialize<Employee>(employeeJson, options);

            // assert
            employee.Should().BeNull();
        }

        [Fact]
        public void Should_Deserialize_Employee_With_Null_Address()
        {
            // arrange
            const string employeeJson = "{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":null}";
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var employee = JsonSerializer.Deserialize<Employee>(employeeJson, options);

            // assert
            employee.Should().NotBeNull();
            employee!.Id.Should().Be(1);
            employee.FirstName.Should().Be("Jean");
            employee.LastName.Should().Be("Snow");
            employee.Address.Should().BeNull();
        }

        [Fact]
        public void Should_Deserialize_Employee_With_NotNull_Address()
        {
            // arrange
            const string employeeJson = "{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":{\"Street\":\"1\",\"City\":\"paris\",\"Country\":\"france\"}}";
            var options = new JsonSerializerOptions
            {
                Converters = { new EmployeeJsonConverter() }
            };

            // act
            var employee = JsonSerializer.Deserialize<Employee>(employeeJson, options);

            // assert
            employee.Should().NotBeNull();
            employee!.Id.Should().Be(1);
            employee.FirstName.Should().Be("Jean");
            employee.LastName.Should().Be("Snow");
            employee.Address.Should().NotBeNull();
            employee.Address.Street.Should().Be("1");
            employee.Address.City.Should().Be("paris");
            employee.Address.Country.Should().Be("france");
        }
    }
}
