using System;
using LibOne.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibOne
{
    public class EmployeeJsonConverter : JsonConverter<Employee>
    {
        public override void WriteJson(JsonWriter writer, Employee value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var addressJsonObject = value.Address == null
                ? null
                : new JObject
                {
                    new JProperty(nameof(value.Address.Street), value.Address.Street),
                    new JProperty(nameof(value.Address.City), value.Address.City),
                    new JProperty(nameof(value.Address.Country), value.Address.Country)
                };

            var employeeJsonObject = new JObject
            {
                new JProperty(nameof(value.Id), value.Id),
                new JProperty(nameof(value.FirstName), value.FirstName),
                new JProperty(nameof(value.LastName), value.LastName),
                new JProperty(nameof(value.Address), addressJsonObject)
            };

            employeeJsonObject.WriteTo(writer);
        }

        public override Employee ReadJson(JsonReader reader, Type objectType, Employee existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var startTokenType = reader.TokenType;

            switch (startTokenType)
            {
                case JsonToken.Null:
                    return null;

                case JsonToken.StartObject:
                    var employeeObj = JObject.Load(reader);
                    var id = Convert.ToInt64(employeeObj[nameof(Employee.Id)]);
                    var firstName = Convert.ToString(employeeObj[nameof(Employee.FirstName)]);
                    var lastName = Convert.ToString(employeeObj[nameof(Employee.LastName)]);
                    var addressObj = employeeObj.SelectToken(nameof(Address));
                    if (addressObj is null || addressObj.Type == JTokenType.Null)
                    {
                        return Employee.Create(id, firstName, lastName, null);
                    }

                    var street = Convert.ToString(addressObj[nameof(Address.Street)]);
                    var city = Convert.ToString(addressObj[nameof(Address.City)]);
                    var country = Convert.ToString(addressObj[nameof(Address.Country)]);
                    var address = Address.Create(street, city, country);
                    return Employee.Create(id, firstName, lastName, address);

                default:
                    throw new ArgumentOutOfRangeException(startTokenType.ToString());
            }
        }
    }
}