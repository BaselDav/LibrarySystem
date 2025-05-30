using System.Linq;
using LibrarySystem.Localization;

namespace LibrarySystem.BusinessLogic.Validation
{
    public static class BookValidator
    {
        public static bool Validate(string title, string author, string isbn, out string error)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                error = Messages.BookTitleRequired;
                return false;
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                error = Messages.BookAuthorRequired;
                return false;
            }

            if (string.IsNullOrWhiteSpace(isbn))
            {
                error = Messages.BookIsbnRequired;
                return false;
            }

            if (!isbn.All(char.IsDigit))
            {
                error = Messages.BookIsbnInvalid;
                return false;
            }

            if (isbn.Length > 20)
            {
                error = Messages.BookIsbnTooLong;
                return false;
            }

            error = null;
            return true;
        }
    }

}
