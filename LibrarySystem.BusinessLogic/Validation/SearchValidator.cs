using LibrarySystem.Localization;

namespace LibrarySystem.BusinessLogic.Validation
{
    public static class SearchValidator
    {
        public static bool Validate(string keyword, out string error)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                error = Messages.SearchKeywordRequired;
                return false;
            }

            error = null;
            return true;
        }
    }

}
