using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LibrarySystem.DataAccess.Helpers;
using LibrarySystem.Domain;

namespace LibrarySystem.DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string connectionString;
        

        public BookRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<Book>> SearchBooksAsync(string keyword, int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var books = new List<Book>();
                string query = @"
            SELECT * FROM Books
            WHERE Title LIKE @kw OR Author LIKE @kw OR ISBN LIKE @kw
            ORDER BY BookId
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = (int)reader["BookId"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            });
                        }
                    }
                }

                return books;
            });
        }

        public async Task<int> GetSearchBooksCountAsync(string keyword)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = @"SELECT COUNT(*) FROM Books WHERE Title LIKE @kw OR Author LIKE @kw OR ISBN LIKE @kw";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }


        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT * FROM Books WHERE BookId = @id";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", bookId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Book
                            {
                                BookId = (int)reader["BookId"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            };
                        }
                    }
                }

                return null;
            });
        }

        public async Task BorrowBookAsync(int userId, int bookId)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string insertQuery = "INSERT INTO Borrowings (UserId, BookId, BorrowDate) VALUES (@userId, @bookId, GETDATE())";
                            using (var insertCmd = new SqlCommand(insertQuery, conn, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@userId", userId);
                                insertCmd.Parameters.AddWithValue("@bookId", bookId);
                                await insertCmd.ExecuteNonQueryAsync();
                            }

                            string updateQuery = "UPDATE Books SET IsAvailable = 0 WHERE BookId = @bookId";
                            using (var updateCmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@bookId", bookId);
                                await updateCmd.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            });
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var books = new List<Book>();
                string query = @"
                    SELECT b.BookId, b.Title, b.Author, b.ISBN, b.IsAvailable
                    FROM Borrowings br
                    INNER JOIN Books b ON br.BookId = b.BookId
                    WHERE br.UserId = @userId AND br.ReturnDate IS NULL";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = (int)reader["BookId"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            });
                        }
                    }
                }

                return books;
            });
        }

        public async Task<bool> ReturnBookAsync(int userId, int bookId)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string updateBorrowing = @"
                                UPDATE Borrowings SET ReturnDate = GETDATE()
                                WHERE UserId = @userId AND BookId = @bookId AND ReturnDate IS NULL";

                            using (var cmd = new SqlCommand(updateBorrowing, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@userId", userId);
                                cmd.Parameters.AddWithValue("@bookId", bookId);

                                if (await cmd.ExecuteNonQueryAsync() == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            string updateBook = "UPDATE Books SET IsAvailable = 1 WHERE BookId = @bookId";
                            using (var updateCmd = new SqlCommand(updateBook, conn, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@bookId", bookId);
                                await updateCmd.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            });
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var users = new List<User>();
                string query = "SELECT * FROM Users ORDER BY FullName";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString()
                            });
                        }
                    }
                }

                return users;
            });
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT * FROM Users WHERE UserId = @id";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }

                return null;
            });
        }

        public async Task UpdateUserAsync(int userId, string fullName, string email)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "UPDATE Users SET FullName = @fullName, Email = @email WHERE UserId = @id";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@fullName", fullName);
                    cmd.Parameters.AddWithValue("@email", email);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }


        public async Task AddUserAsync(string fullName, string email)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "INSERT INTO Users (FullName, Email) VALUES (@fullName, @email)";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fullName", fullName);
                    cmd.Parameters.AddWithValue("@email", email);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }
        public async Task<int> GetUsersCountAsync()
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT COUNT(*) FROM Users";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }
        public async Task<List<User>> GetPagedUsersAsync(int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var users = new List<User>();
                string query = @"
            SELECT * FROM Users
            ORDER BY FullName
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString()
                            });
                        }
                    }
                }

                return users;
            });
        }
        public async Task<int> GetSearchUsersCountAsync(string keyword)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT COUNT(*) FROM Users WHERE FullName LIKE @kw OR Email LIKE @kw";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }
        public async Task<List<User>> SearchUsersAsync(string keyword, int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var users = new List<User>();
                string query = @"
            SELECT * FROM Users
            WHERE FullName LIKE @kw OR Email LIKE @kw
            ORDER BY FullName
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString()
                            });
                        }
                    }
                }

                return users;
            });
        }


        public async Task<List<BorrowingRecord>> GetBorrowingHistoryAsync(int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var records = new List<BorrowingRecord>();
                string query = @"
            SELECT u.FullName, b.Title, br.BorrowDate, br.ReturnDate
            FROM Borrowings br
            INNER JOIN Books b ON br.BookId = b.BookId
            INNER JOIN Users u ON br.UserId = u.UserId
            ORDER BY br.BorrowDate DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new BorrowingRecord
                            {
                                UserName = reader["FullName"].ToString(),
                                BookTitle = reader["Title"].ToString(),
                                BorrowDate = (DateTime)reader["BorrowDate"],
                                ReturnDate = reader["ReturnDate"] == DBNull.Value
                                    ? (DateTime?)null
                                    : (DateTime)reader["ReturnDate"]
                            });
                        }
                    }
                }

                return records;
            });
        }

        public async Task<int> GetBorrowingHistoryCountAsync()
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT COUNT(*) FROM Borrowings";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }


        public async Task<List<Book>> GetBooksAsync(int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var books = new List<Book>();
                string query = @"
            SELECT * FROM Books
            ORDER BY BookId
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                BookId = (int)reader["BookId"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                ISBN = reader["ISBN"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            });
                        }
                    }
                }

                return books;
            });
        }

        public async Task<int> GetBooksCountAsync()
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "SELECT COUNT(*) FROM Books";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }


        public async Task AddBookAsync(string title, string author, string isbn)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "INSERT INTO Books (Title, Author, ISBN, IsAvailable) VALUES (@title, @auth, @isbn, 1)";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@auth", author);
                    cmd.Parameters.AddWithValue("@isbn", isbn);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task<bool> IsIsbnExistsAsync(string isbn)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Books WHERE ISBN = @isbn", conn))
                {
                    cmd.Parameters.AddWithValue("@isbn", isbn);
                    await conn.OpenAsync();
                    int count = (int)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            });
        }

        public async Task UpdateBookAsync(int bookId, string title, string author, string isbn)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                string query = "UPDATE Books SET Title = @title, Author = @auth, ISBN = @isbn WHERE BookId = @id";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", bookId);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@auth", author);
                    cmd.Parameters.AddWithValue("@isbn", isbn);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task DeleteBookAsync(int bookId)
        {
            await DbExecutor.ExecuteAsync(async () =>
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string deleteBorrowings = "DELETE FROM Borrowings WHERE BookId = @id";
                            using (var cmd1 = new SqlCommand(deleteBorrowings, conn, transaction))
                            {
                                cmd1.Parameters.AddWithValue("@id", bookId);
                                await cmd1.ExecuteNonQueryAsync();
                            }

                            string deleteBook = "DELETE FROM Books WHERE BookId = @id";
                            using (var cmd2 = new SqlCommand(deleteBook, conn, transaction))
                            {
                                cmd2.Parameters.AddWithValue("@id", bookId);
                                await cmd2.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            });
        }
        public async Task<string> GetAvailableFullNameAsync(string baseName)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string newName = baseName;
                int suffix = 2;

                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    while (true)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE FullName = @name";
                        using (var cmd = new SqlCommand(checkQuery, conn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@name", newName);

                            int count = (int)await cmd.ExecuteScalarAsync();
                            if (count == 0)
                                return newName;

                            newName = $"{baseName}_{suffix++}";
                        }
                    }
                }
            });
        }
        public async Task<List<BorrowingRecord>> SearchBorrowingHistoryAsync(string keyword, int page, int pageSize)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                var records = new List<BorrowingRecord>();
                string query = @"
            SELECT u.FullName, b.Title, br.BorrowDate, br.ReturnDate
            FROM Borrowings br
            INNER JOIN Books b ON br.BookId = b.BookId
            INNER JOIN Users u ON br.UserId = u.UserId
            WHERE u.FullName LIKE @kw OR b.Title LIKE @kw
            ORDER BY br.BorrowDate DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new BorrowingRecord
                            {
                                UserName = reader["FullName"].ToString(),
                                BookTitle = reader["Title"].ToString(),
                                BorrowDate = (DateTime)reader["BorrowDate"],
                                ReturnDate = reader["ReturnDate"] == DBNull.Value
                                    ? (DateTime?)null
                                    : (DateTime)reader["ReturnDate"]
                            });
                        }
                    }
                }

                return records;
            });
        }

        public async Task<int> GetSearchBorrowingHistoryCountAsync(string keyword)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                string query = @"
            SELECT COUNT(*)
            FROM Borrowings br
            INNER JOIN Books b ON br.BookId = b.BookId
            INNER JOIN Users u ON br.UserId = u.UserId
            WHERE u.FullName LIKE @kw OR b.Title LIKE @kw";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
                    await conn.OpenAsync();
                    return (int)await cmd.ExecuteScalarAsync();
                }
            });
        }

        public async Task<User> GetBookCurrentBorrowerAsync(int bookId)
        {
            return await DbExecutor.ExecuteAsync(async () =>
            {
                const string query = @"
            SELECT u.UserId, u.FullName, u.Email
            FROM Borrowings br
            INNER JOIN Users u ON br.UserId = u.UserId
            WHERE br.BookId = @bookId AND br.ReturnDate IS NULL";

                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }

                return null; 
            });
        }

    }
}
