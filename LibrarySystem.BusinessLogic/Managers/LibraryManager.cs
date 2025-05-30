using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibrarySystem.DataAccess.Repositories;
using LibrarySystem.Domain;
using LibrarySystem.Localization;

namespace LibrarySystem.BusinessLogic.Managers
{
    public class LibraryManager : ILibraryManager
    {
        private readonly IBookRepository bookRepository;
        //private List<User> cachedUsers;

        public LibraryManager(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<List<Book>> SearchBooksAsync(string keyword, int page, int pageSize)
            => await bookRepository.SearchBooksAsync(keyword, page, pageSize);

        public async Task<int> GetSearchBooksCountAsync(string keyword)
            => await bookRepository.GetSearchBooksCountAsync(keyword);


        public async Task<bool> BorrowBookAsync(int userId, int bookId)
        {
            var book = await bookRepository.GetBookByIdAsync(bookId);
            if (book == null || !book.IsAvailable)
                return false;

            await bookRepository.BorrowBookAsync(userId, bookId);
            return true;
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
            => await bookRepository.GetBorrowedBooksAsync(userId);

        public async Task<bool> ReturnBookAsync(int userId, int bookId)
            => await bookRepository.ReturnBookAsync(userId, bookId);

        public async Task<List<User>> GetAllUsersAsync()
            => await bookRepository.GetAllUsersAsync();

        public async Task<(bool success, string error)> AddUserAsync(string fullName, string email)
        {
            var users = await bookRepository.GetAllUsersAsync();

            if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, Messages.DuplicateEmail);

            if (users.Any(u => u.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)))
            {
                fullName = await bookRepository.GetAvailableFullNameAsync(fullName);
            }
            await bookRepository.AddUserAsync(fullName, email);
            return (true, null);
        }
        public async Task<User> GetUserByIdAsync(int userId)
            => await bookRepository.GetUserByIdAsync(userId);

        public async Task<(bool success, string error)> UpdateUserAsync(int userId, string fullName, string email)
        {
            var users = await bookRepository.GetAllUsersAsync();

            if (users.Any(u => u.UserId != userId && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, Messages.DuplicateEmail);

            if (users.Any(u => u.UserId != userId && u.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)))
            {
                fullName = await bookRepository.GetAvailableFullNameAsync(fullName);
            }

            await bookRepository.UpdateUserAsync(userId, fullName, email);
            return (true, null);
        }
        public async Task<int> GetUsersCountAsync()
            => await bookRepository.GetUsersCountAsync();

        public async Task<List<User>> GetPagedUsersAsync(int page, int pageSize)
            => await bookRepository.GetPagedUsersAsync(page, pageSize);

        public async Task<int> GetSearchUsersCountAsync(string keyword)
            => await bookRepository.GetSearchUsersCountAsync(keyword);

        public async Task<List<User>> SearchUsersAsync(string keyword, int page, int pageSize)
            => await bookRepository.SearchUsersAsync(keyword, page, pageSize);

        public async Task<List<BorrowingRecord>> GetBorrowingHistoryAsync(int page, int pageSize)
            => await bookRepository.GetBorrowingHistoryAsync(page, pageSize);

        public async Task<int> GetBorrowingHistoryCountAsync()
            => await bookRepository.GetBorrowingHistoryCountAsync();

        public async Task<List<Book>> GetBooksAsync(int page, int pageSize)
            => await bookRepository.GetBooksAsync(page, pageSize);

        public async Task<int> GetBooksCountAsync()
            => await bookRepository.GetBooksCountAsync();

        public async Task<Book> GetBookByIdAsync(int bookId)
            => await bookRepository.GetBookByIdAsync(bookId);

        public async Task<(bool success, string error)> AddBookAsync(string title, string author, string isbn)
        {
            if (await bookRepository.IsIsbnExistsAsync(isbn))
                return (false, Messages.DuplicateRecord);

            await bookRepository.AddBookAsync(title, author, isbn);
            return (true, null);
        }

        public async Task UpdateBookAsync(int bookId, string title, string author, string isbn)
        {
            await bookRepository.UpdateBookAsync(bookId, title, author, isbn);
        }

        public async Task DeleteBookAsync(int bookId)
        {
            await bookRepository.DeleteBookAsync(bookId);
        }
        public async Task<List<BorrowingRecord>> SearchBorrowingHistoryAsync(string keyword, int page, int pageSize)
            => await bookRepository.SearchBorrowingHistoryAsync(keyword, page, pageSize);

        public async Task<int> GetSearchBorrowingHistoryCountAsync(string keyword)
            => await bookRepository.GetSearchBorrowingHistoryCountAsync(keyword);
        public async Task<User> GetBookCurrentBorrowerAsync(int bookId)
            => await bookRepository.GetBookCurrentBorrowerAsync(bookId);

    }
}
