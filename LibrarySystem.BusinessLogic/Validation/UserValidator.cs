using System.Text.RegularExpressions;
using LibrarySystem.Localization;

namespace LibrarySystem.BusinessLogic.Validation
{
    public static class UserValidator
    {
        public static bool Validate(string fullName, string email, out string error)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                error = Messages.NameRequired;
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                error = Messages.EmailRequired;
                return false;
            }

            if (!Regex.IsMatch(email, @"^\S+@\S+\.\S+$"))
            {
                error = Messages.InvalidEmail;
                return false;
            }

            error = null;
            return true;
        }
    }

}
