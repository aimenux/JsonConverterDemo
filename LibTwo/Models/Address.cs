using System;

namespace LibTwo.Models
{
    public class Address : IEquatable<Address>
    {
        private Address(string street, string city, string country)
        {
            Street = street;
            City = city;
            Country = country;
        }

        public string Street { get; }

        public string City { get; }

        public string Country { get; }

        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Street == other.Street && City == other.City && Country == other.Country;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, Country);
        }

        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }

        public static Address Create(string street, string city, string country)
        {
            return new Address(street, city, country);
        }
    }
}