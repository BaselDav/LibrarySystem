using System.Data.SqlClient;


namespace LibrarySystem.Infrastructure
{
    public static class DatabaseInitializer
    {
        private const string DatabaseName = "LibraryDB";

        public static void EnsureDatabaseAndTables(string serverConnectionString)
        {
            using (var connection = new SqlConnection(serverConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{DatabaseName}')
                    BEGIN
                        CREATE DATABASE [{DatabaseName}];
                    END";
                    cmd.ExecuteNonQuery();
                }
            }

            var dbConnectionString = $"{serverConnectionString};Initial Catalog={DatabaseName}";

            using (var dbConn = new SqlConnection(dbConnectionString))
            {
                dbConn.Open();
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
                    BEGIN
                        CREATE TABLE Users (
                            UserId INT PRIMARY KEY IDENTITY(1,1),
                            FullName NVARCHAR(100) COLLATE Arabic_CI_AS NOT NULL,
                            Email NVARCHAR(100) COLLATE Arabic_CI_AS NOT NULL UNIQUE,
                            CONSTRAINT CK_Users_Email CHECK (Email LIKE '%@%.%')
                        );
                        CREATE INDEX IX_Users_FullName ON Users(FullName);
                    END;

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Books' AND xtype = 'U')
                    BEGIN
                        CREATE TABLE Books (
                            BookId INT PRIMARY KEY IDENTITY(1,1),
                            Title NVARCHAR(150) COLLATE Arabic_CI_AS NOT NULL,
                            Author NVARCHAR(100) COLLATE Arabic_CI_AS NOT NULL,
                            ISBN VARCHAR(20) NOT NULL UNIQUE,
                            IsAvailable BIT NOT NULL DEFAULT 1
                        );
                        CREATE INDEX IX_Books_Title ON Books(Title);
                        CREATE INDEX IX_Books_ISBN ON Books(ISBN);
                    END;

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Borrowings' AND xtype = 'U')
                    BEGIN
                        CREATE TABLE Borrowings (
                            BorrowingId INT PRIMARY KEY IDENTITY(1,1),
                            UserId INT NOT NULL,
                            BookId INT NOT NULL,
                            BorrowDate DATETIME NOT NULL DEFAULT GETDATE(),
                            ReturnDate DATETIME NULL,
                            CONSTRAINT FK_Borrowings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
                            CONSTRAINT FK_Borrowings_Books FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE
                        );
                        CREATE INDEX IX_Borrowings_UserId ON Borrowings(UserId);
                        CREATE INDEX IX_Borrowings_BookId ON Borrowings(BookId);
                        CREATE INDEX IX_Borrowings_BorrowDate ON Borrowings(BorrowDate);
                    END;";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
