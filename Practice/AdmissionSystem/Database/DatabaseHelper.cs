using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using LibrarySystem.Models;

namespace LibrarySystem.Database
{
    public static class DatabaseHelper
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "library.db");
        private static string ConnectionString => $"Data Source={dbPath};Version=3;";

        public static void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(dbPath);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Создание таблицы пользователей
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        FullName TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        RegistrationDate TEXT NOT NULL
                    )");

                // Создание таблицы категорий книг
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS BookCategories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Code TEXT NOT NULL UNIQUE,
                        BooksCount INTEGER NOT NULL,
                        Description TEXT
                    )");

                // Создание таблицы заявок на книги
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS BookRequests (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        BookCategoryId INTEGER NOT NULL,
                        BookTitle TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        ISBN TEXT,
                        RequestDate TEXT NOT NULL,
                        Status TEXT NOT NULL,
                        SubmissionDate TEXT NOT NULL,
                        Notes TEXT,
                        FOREIGN KEY (UserId) REFERENCES Users(Id),
                        FOREIGN KEY (BookCategoryId) REFERENCES BookCategories(Id)
                    )");

                // Создание администратора по умолчанию
                if (isNewDatabase)
                {
                    var adminExists = connection.ExecuteScalar<int>(
                        "SELECT COUNT(*) FROM Users WHERE Login = 'admin'");

                    if (adminExists == 0)
                    {
                        connection.Execute(@"
                            INSERT INTO Users (Login, Password, FullName, Role, RegistrationDate)
                            VALUES ('admin', 'admin', 'Администратор', 'Admin', @date)",
                            new { date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    }

                    // Добавление категорий книг
                    connection.Execute(@"
                        INSERT INTO BookCategories (Name, Code, BooksCount, Description)
                        VALUES
                        ('Художественная литература', 'ART', 100, 'Романы, повести, рассказы'),
                        ('Научная литература', 'SCI', 50, 'Учебники, монографии, научные статьи'),
                        ('Техническая литература', 'TECH', 75, 'Руководства, справочники по технологиям'),
                        ('Детская литература', 'CHILD', 60, 'Книги для детей и подростков')");
                }
            }
        }

        // Методы для работы с пользователями
        public static User GetUser(string login, string password)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE Login = @Login AND Password = @Password",
                    new { Login = login, Password = password });
            }
        }

        public static bool RegisterUser(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Execute(@"
                        INSERT INTO Users (Login, Password, FullName, Role, RegistrationDate)
                        VALUES (@Login, @Password, @FullName, @Role, @RegistrationDate)",
                        user);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<User> GetAllUsers()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<User>("SELECT * FROM Users").ToList();
            }
        }

        public static void UpdateUser(User user)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    UPDATE Users
                    SET Login = @Login, Password = @Password, FullName = @FullName, Role = @Role
                    WHERE Id = @Id", user);
            }
        }

        public static void DeleteUser(int userId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = userId });
            }
        }

        // Методы для работы с категориями книг
        public static List<BookCategory> GetAllBookCategories()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<BookCategory>("SELECT * FROM BookCategories").ToList();
            }
        }

        public static void AddBookCategory(BookCategory category)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    INSERT INTO BookCategories (Name, Code, BooksCount, Description)
                    VALUES (@Name, @Code, @BooksCount, @Description)",
                    category);
            }
        }

        public static void UpdateBookCategory(BookCategory category)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    UPDATE BookCategories
                    SET Name = @Name, Code = @Code, BooksCount = @BooksCount, Description = @Description
                    WHERE Id = @Id", category);
            }
        }

        public static void DeleteBookCategory(int categoryId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("DELETE FROM BookCategories WHERE Id = @Id", new { Id = categoryId });
            }
        }

        // Методы для работы с заявками на книги
        public static List<BookRequest> GetAllBookRequests()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<BookRequest>(@"
                    SELECT br.*, bc.Name as CategoryName
                    FROM BookRequests br
                    LEFT JOIN BookCategories bc ON br.BookCategoryId = bc.Id").ToList();
            }
        }

        public static List<BookRequest> GetUserBookRequests(int userId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<BookRequest>(@"
                    SELECT br.*, bc.Name as CategoryName
                    FROM BookRequests br
                    LEFT JOIN BookCategories bc ON br.BookCategoryId = bc.Id
                    WHERE br.UserId = @UserId",
                    new { UserId = userId }).ToList();
            }
        }

        public static void AddBookRequest(BookRequest request)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    INSERT INTO BookRequests
                    (UserId, BookCategoryId, BookTitle, Author, ISBN, RequestDate,
                     Status, SubmissionDate, Notes)
                    VALUES
                    (@UserId, @BookCategoryId, @BookTitle, @Author, @ISBN, @RequestDate,
                     @Status, @SubmissionDate, @Notes)",
                    request);
            }
        }

        public static void UpdateBookRequest(BookRequest request)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    UPDATE BookRequests
                    SET BookCategoryId = @BookCategoryId, BookTitle = @BookTitle, Author = @Author,
                        ISBN = @ISBN, RequestDate = @RequestDate, Status = @Status, Notes = @Notes
                    WHERE Id = @Id", request);
            }
        }

        public static void DeleteBookRequest(int requestId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("DELETE FROM BookRequests WHERE Id = @Id", new { Id = requestId });
            }
        }

        public static BookCategory GetBookCategoryById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<BookCategory>(
                    "SELECT * FROM BookCategories WHERE Id = @Id", new { Id = id });
            }
        }

        public static bool UserExists(string login)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var count = connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Users WHERE Login = @Login",
                    new { Login = login });
                return count > 0;
            }
        }

        public static void UpdateBookRequestStatus(int requestId, string status)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    UPDATE BookRequests
                    SET Status = @Status
                    WHERE Id = @Id",
                    new { Id = requestId, Status = status });
            }
        }
    }
}
