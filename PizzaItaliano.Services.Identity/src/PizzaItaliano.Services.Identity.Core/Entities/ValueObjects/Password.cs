using PizzaItaliano.Services.Identity.Core.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace PizzaItaliano.Services.Identity.Core.Entities.ValueObjects
{
    public class Password : IEquatable<Password>
    {
        public static readonly Regex PasswordRegex = new("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\w\\W]{8,}$");
                
        private readonly string _password;
        public string Value => _password;

        private Password(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidPasswordException();
            }

            if (!PasswordRegex.IsMatch(password))
            {
                throw new InvalidPasswordException();
            }

            _password = password;
        }

        public static Password From(string password) => new(password);

        public bool Equals(Password other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj as Password == null) return false;
            return Equals((Password)obj);
        }

        public override int GetHashCode()
        {
            return _password.GetHashCode();
        }
    }
}
