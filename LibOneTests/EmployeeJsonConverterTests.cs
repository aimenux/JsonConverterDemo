using System.Collections.Generic;
using FluentAssertions;
using LibOne;
using LibOne.Models;
using Newtonsoft.Json;
using Xunit;

namespace LibOneTests
{
    public class EmployeeJsonConverterTests
    {
        [Fact]
        public void Should_Serialize_Null_Employee()
        {
            // arrange
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var json = JsonConvert.SerializeObject(default(Employee), settings);

            // assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("null");
        }

        [Fact]
        public void Should_Serialize_Employee_With_Null_Address()
        {
            // arrange
            var employee = Employee.Create(1, "Jean", "Snow", default(Address));
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var json = JsonConvert.SerializeObject(employee, settings);

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
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var json = JsonConvert.SerializeObject(employee, settings);

            // assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Be("{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":{\"Street\":\"1\",\"City\":\"paris\",\"Country\":\"france\"}}");
        }

        [Fact]
        public void Should_Deserialize_Null_Employee()
        {
            // arrange
            const string employeeJson = "null";
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var employee = JsonConvert.DeserializeObject<Employee>(employeeJson, settings);

            // assert
            employee.Should().BeNull();
        }

        [Fact]
        public void Should_Deserialize_Employee_With_Null_Address()
        {
            // arrange
            const string employeeJson = "{\"Id\":1,\"FirstName\":\"Jean\",\"LastName\":\"Snow\",\"Address\":null}";
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var employee = JsonConvert.DeserializeObject<Employee>(employeeJson, settings);

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
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new EmployeeJsonConverter()
                }
            };

            // act
            var employee = JsonConvert.DeserializeObject<Employee>(employeeJson, settings);

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
