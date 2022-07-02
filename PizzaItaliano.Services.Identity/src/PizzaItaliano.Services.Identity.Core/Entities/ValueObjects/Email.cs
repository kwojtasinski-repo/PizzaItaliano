using PizzaItaliano.Services.Identity.Core.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace PizzaItaliano.Services.Identity.Core.Entities.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        public static readonly Regex EmailRegex = new Regex(
           @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
           @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
           RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
       
        private readonly string _email;
        public string Value => _email;

        private Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidEmailException(email);
            }

            if (!EmailRegex.IsMatch(email))
            {
                throw new InvalidEmailException(email);
            }

            _email = email.ToLowerInvariant();
        }

        public static Email From(string email) => new(email);

        public bool Equals(Email other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj as Email == null) return false;
            return Equals((Email)obj);
        }

        public override int GetHashCode()
        {
            return _email.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
