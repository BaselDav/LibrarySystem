using System.Collections.Generic;
using System.Threading.Tasks;
using LibrarySystem.Domain;

namespace LibrarySystem.BusinessLogic.Managers
{
    public interface ILibraryManager
    {
        Task<List<Book>> SearchBooksAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchBooksCountAsync(string keyword);
        Task<bool> BorrowBookAsync(int userId, int bookId);
        Task<List<Book>> GetBorrowedBooksAsync(int userId);
        Task<bool> ReturnBookAsync(int userId, int bookId);
        Task<List<User>> GetAllUsersAsync();
        Task<(bool success, string error)> AddUserAsync(string fullName, string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<(bool success, string error)> UpdateUserAsync(int userId, string fullName, string email);
        Task<int> GetUsersCountAsync();
        Task<List<User>> GetPagedUsersAsync(int page, int pageSize);
        Task<int> GetSearchUsersCountAsync(string keyword);
        Task<List<User>> SearchUsersAsync(string keyword, int page, int pageSize);

        Task<List<BorrowingRecord>> GetBorrowingHistoryAsync(int page, int pageSize);
        Task<int> GetBorrowingHistoryCountAsync();
        Task<List<Book>> GetBooksAsync(int page, int pageSize);
        Task<int> GetBooksCountAsync();
        Task<Book> GetBookByIdAsync(int bookId);
        Task<(bool success, string error)> AddBookAsync(string title, string author, string isbn);
        Task UpdateBookAsync(int bookId, string title, string author, string isbn);
        Task DeleteBookAsync(int bookId);
        Task<List<BorrowingRecord>> SearchBorrowingHistoryAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchBorrowingHistoryCountAsync(string keyword);
        Task<User> GetBookCurrentBorrowerAsync(int bookId);
    }
}
