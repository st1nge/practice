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
                        Phone TEXT,
                        Email TEXT,
                        Address TEXT,
                        ParentPhone TEXT,
                        RegistrationDate TEXT NOT NULL
                    )");

                // Если база уже существовала, убедимся, что новые колонки добавлены (миграция схемы)
                try
                {
                    var cols = connection.Query<dynamic>("PRAGMA table_info(Users);");
                    var colNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var c in cols)
                    {
                        // pragma returns 'name' field
                        colNames.Add((string)c.name);
                    }

                    if (!colNames.Contains("Phone"))
                        connection.Execute("ALTER TABLE Users ADD COLUMN Phone TEXT;");
                    if (!colNames.Contains("Email"))
                        connection.Execute("ALTER TABLE Users ADD COLUMN Email TEXT;");
                    if (!colNames.Contains("Address"))
                        connection.Execute("ALTER TABLE Users ADD COLUMN Address TEXT;");
                    if (!colNames.Contains("ParentPhone"))
                        connection.Execute("ALTER TABLE Users ADD COLUMN ParentPhone TEXT;");
                }
                catch
                {
                    // Если что-то пойдет не так при миграции, продолжаем — старые клиенты будут работать, но новые поля могут быть пустыми
                }
                try
                {
                    var colsBR = connection.Query<dynamic>("PRAGMA table_info(BookRequests);");
                    var colNamesBR = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var c in colsBR)
                    {
                        colNamesBR.Add((string)c.name);
                    }

                    if (!colNamesBR.Contains("Genre"))
                        connection.Execute("ALTER TABLE BookRequests ADD COLUMN Genre TEXT;");
                    if (!colNamesBR.Contains("Year"))
                        connection.Execute("ALTER TABLE BookRequests ADD COLUMN Year INTEGER;");
                }
                catch
                {
                    // Игнорируем ошибки миграции таблицы BookRequests
                }

                // Создание таблицы категорий книг
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS BookCategories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Code TEXT NOT NULL UNIQUE,
                        BooksCount INTEGER NOT NULL,
                        Description TEXT
                    )");

                // Создание таблицы книг (каталог)
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Books (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Genre TEXT,
                        Year INTEGER,
                        Description TEXT,
                        Count INTEGER DEFAULT 1,
                        ISBN TEXT,
                        CategoryId INTEGER NOT NULL,
                        FOREIGN KEY (CategoryId) REFERENCES BookCategories(Id)
                    )");

                // Попытка выполнить простую миграцию для таблицы Books (если потребуется в будущем)
                try
                {
                    var colsB = connection.Query<dynamic>("PRAGMA table_info(Books);");
                    var colNamesB = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var c in colsB)
                        colNamesB.Add((string)c.name);

                    if (!colNamesB.Contains("Genre"))
                        connection.Execute("ALTER TABLE Books ADD COLUMN Genre TEXT;");
                    if (!colNamesB.Contains("Year"))
                        connection.Execute("ALTER TABLE Books ADD COLUMN Year INTEGER;");
                    if (!colNamesB.Contains("Description"))
                        connection.Execute("ALTER TABLE Books ADD COLUMN Description TEXT;");
                    if (!colNamesB.Contains("Count"))
                        connection.Execute("ALTER TABLE Books ADD COLUMN Count INTEGER DEFAULT 1;");
                    if (!colNamesB.Contains("ISBN"))
                        connection.Execute("ALTER TABLE Books ADD COLUMN ISBN TEXT;");
                }
                catch
                {
                    // Игнорируем ошибки миграции Books
                }

                // Создание таблицы заявок на книги
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS BookRequests (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        BookCategoryId INTEGER NOT NULL,
                        BookTitle TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Genre TEXT,
                        Year INTEGER,
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
                            INSERT INTO Users (Login, Password, FullName, Role, Phone, Email, Address, ParentPhone, RegistrationDate)
                            VALUES ('admin', 'admin', 'Администратор', 'Admin', '', '', '', '', @date)",
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
                        INSERT INTO Users (Login, Password, FullName, Role, Phone, Email, Address, ParentPhone, RegistrationDate)
                        VALUES (@Login, @Password, @FullName, @Role, @Phone, @Email, @Address, @ParentPhone, @RegistrationDate)",
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
                    SET Login = @Login, Password = @Password, FullName = @FullName, Role = @Role,
                        Phone = @Phone, Email = @Email, Address = @Address, ParentPhone = @ParentPhone
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

        public static int AddBookCategory(BookCategory category)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    INSERT INTO BookCategories (Name, Code, BooksCount, Description)
                    VALUES (@Name, @Code, @BooksCount, @Description)",
                    category);
                // return last inserted id
                var id = connection.ExecuteScalar<long>("SELECT last_insert_rowid();");
                return (int)id;
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
                    (UserId, BookCategoryId, BookTitle, Author, Genre, Year, ISBN, RequestDate,
                     Status, SubmissionDate, Notes)
                    VALUES
                    (@UserId, @BookCategoryId, @BookTitle, @Author, @Genre, @Year, @ISBN, @RequestDate,
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
                        Genre = @Genre, Year = @Year, ISBN = @ISBN, RequestDate = @RequestDate, Status = @Status, Notes = @Notes
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

        public static BookCategory GetBookCategoryByCode(string code)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QueryFirstOrDefault<BookCategory>(
                    "SELECT * FROM BookCategories WHERE Code = @Code", new { Code = code });
            }
        }

        // Методы для работы с каталогом книг
        public static List<Book> GetBooksByCategory(int categoryId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<Book>("SELECT * FROM Books WHERE CategoryId = @CategoryId ORDER BY Title", new { CategoryId = categoryId }).ToList();
            }
        }

        public static void AddBook(Book book)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute(@"
                    INSERT INTO Books (Title, Author, Genre, Year, Description, Count, ISBN, CategoryId)
                    VALUES (@Title, @Author, @Genre, @Year, @Description, @Count, @ISBN, @CategoryId)", book);

                // increment the BooksCount on the category by the book.Count
                if (book.CategoryId > 0 && book.Count > 0)
                {
                    connection.Execute(@"UPDATE BookCategories SET BooksCount = BooksCount + @Delta WHERE Id = @Id", new { Delta = book.Count, Id = book.CategoryId });
                }
            }
        }

        public static void UpdateBook(Book book)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                // adjust category BooksCount if Count changed
                var existing = connection.QueryFirstOrDefault<Book>("SELECT * FROM Books WHERE Id = @Id", new { Id = book.Id });
                if (existing != null)
                {
                    int delta = book.Count - existing.Count;
                    if (delta != 0 && existing.CategoryId > 0)
                    {
                        connection.Execute("UPDATE BookCategories SET BooksCount = BooksCount + @Delta WHERE Id = @Id", new { Delta = delta, Id = existing.CategoryId });
                    }
                }

                connection.Execute(@"
                    UPDATE Books
                    SET Title = @Title, Author = @Author, Genre = @Genre, Year = @Year, Description = @Description, Count = @Count, ISBN = @ISBN
                    WHERE Id = @Id", book);
            }
        }

        public static void DeleteBook(int bookId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                // fetch book to know its count and category
                var b = connection.QueryFirstOrDefault<Book>("SELECT * FROM Books WHERE Id = @Id", new { Id = bookId });
                if (b != null)
                {
                    if (b.CategoryId > 0 && b.Count > 0)
                    {
                        connection.Execute("UPDATE BookCategories SET BooksCount = BooksCount - @Delta WHERE Id = @Id", new { Delta = b.Count, Id = b.CategoryId });
                    }
                    connection.Execute("DELETE FROM Books WHERE Id = @Id", new { Id = bookId });
                }
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
