using System.Collections.Generic;
using System.Threading.Tasks;
using LibrarySystem.Domain;

namespace LibrarySystem.DataAccess.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> SearchBooksAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchBooksCountAsync(string keyword);
        Task<Book> GetBookByIdAsync(int bookId);
        Task BorrowBookAsync(int userId, int bookId);
        Task<List<Book>> GetBorrowedBooksAsync(int userId);
        Task<bool> ReturnBookAsync(int userId, int bookId);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(int userId, string fullName, string email);
        Task<User> GetUserByIdAsync(int userId);
        Task AddUserAsync(string fullName, string email);
        Task<int> GetUsersCountAsync();
        Task<List<User>> GetPagedUsersAsync(int page, int pageSize);
        Task<int> GetSearchUsersCountAsync(string keyword);
        Task<List<User>> SearchUsersAsync(string keyword, int page, int pageSize);
        Task<List<BorrowingRecord>> GetBorrowingHistoryAsync(int page, int pageSize);
        Task<int> GetBorrowingHistoryCountAsync();

        Task<List<Book>> GetBooksAsync(int page, int pageSize);
        Task<int> GetBooksCountAsync();
        Task AddBookAsync(string title, string author, string isbn);
        Task UpdateBookAsync(int bookId, string title, string author, string isbn);
        Task DeleteBookAsync(int bookId);
        Task<bool> IsIsbnExistsAsync(string isbn);
        Task<string> GetAvailableFullNameAsync(string baseName);
        Task<List<BorrowingRecord>> SearchBorrowingHistoryAsync(string keyword, int page, int pageSize);
        Task<int> GetSearchBorrowingHistoryCountAsync(string keyword);
        Task<User> GetBookCurrentBorrowerAsync(int bookId);

    }
}
