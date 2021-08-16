using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibTwo.Models;

namespace LibTwo
{
    public class EmployeeJsonConverter : JsonConverter<Employee>
    {
        public override void Write(Utf8JsonWriter writer, Employee value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                JsonSerializer.Serialize(writer, default(Employee), options);
            }
            else
            {
                writer.WriteStartObject();
                WriteEmployeeObject(writer, value);
                writer.WriteEndObject();
            }
        }

        public override Employee Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.StartObject:
                    using (var token = JsonDocument.ParseValue(ref reader))
                    {
                        token.RootElement.TryGetProperty(nameof(Employee.Id), out var idElement);
                        token.RootElement.TryGetProperty(nameof(Employee.FirstName), out var firstNameElement);
                        token.RootElement.TryGetProperty(nameof(Employee.LastName), out var lastNameElement);
                        var id = idElement.GetInt64();
                        var firstName = firstNameElement.GetString();
                        var lastName = lastNameElement.GetString();
                        return Employee.Create(id, firstName, lastName, GetAddress(token));
                    }

                default:
                    throw new ArgumentOutOfRangeException(reader.TokenType.ToString());
            }
        }

        private static void WriteEmployeeObject(Utf8JsonWriter writer, Employee value)
        {
            writer.WriteNumber(JsonEncodedText.Encode(nameof(Employee.Id)), value.Id);
            writer.WriteString(JsonEncodedText.Encode(nameof(Employee.FirstName)), value.FirstName);
            writer.WriteString(JsonEncodedText.Encode(nameof(Employee.LastName)), value.LastName);

            if (value.Address is null)
            {
                writer.WriteNull(nameof(Address));
            }
            else
            {
                writer.WriteStartObject(nameof(Address));
                writer.WriteString(JsonEncodedText.Encode(nameof(Address.Street)), value.Address.Street);
                writer.WriteString(JsonEncodedText.Encode(nameof(Address.City)), value.Address.City);
                writer.WriteString(JsonEncodedText.Encode(nameof(Address.Country)), value.Address.Country);
                writer.WriteEndObject();
            }
        }

        private static Address GetAddress(JsonDocument document)
        {
            if (!document.RootElement.TryGetProperty(nameof(Address), out var addressElement) || addressElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            addressElement.TryGetProperty(nameof(Address.Street), out var streetElement);
            addressElement.TryGetProperty(nameof(Address.City), out var cityElement);
            addressElement.TryGetProperty(nameof(Address.Country), out var countryElement);
            var street = streetElement.GetString();
            var city = cityElement.GetString();
            var country = countryElement.GetString();
            return Address.Create(street, city, country);
        }
    }
}