using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryBPPK
{
    class Program
    {
        static void Main(string[] args)
        {
            LibraryManager manager = new LibraryManager();
            
            while (true)
            {
                Console.WriteLine("\n=== Библиотека БППК ===");
                Console.WriteLine("1. Добавить книгу");
                Console.WriteLine("2. Показать все книги");
                Console.WriteLine("3. Найти книгу");
                Console.WriteLine("4. Выдать книгу читателю");
                Console.WriteLine("5. Вернуть книгу");
                Console.WriteLine("6. Выйти");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.AddBook();
                        break;
                    case "2":
                        manager.ShowAllBooks();
                        break;
                    case "3":
                        manager.FindBook();
                        break;
                    case "4":
                        manager.LendBook();
                        break;
                    case "5":
                        manager.ReturnBook();
                        break;
                    case "6":
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }
    }

    class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Borrower { get; set; } = "";

        public override string ToString()
        {
            string status = IsAvailable ? "Доступна" : $"Выдана: {Borrower}";
            return $"ID: {Id} | {Title} | {Author} | {Year} | {ISBN} | Статус: {status}";
        }
    }

    class LibraryManager
    {
        private List<Book> books = new List<Book>();
        private string booksFile = "books.txt";

        public LibraryManager()
        {
            LoadBooksFromFile();
        }

        // Загрузка книг из файла
        private void LoadBooksFromFile()
        {
            if (File.Exists(booksFile))
            {
                books.Clear();
                string[] lines = File.ReadAllLines(booksFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length >= 7)
                    {
                        books.Add(new Book
                        {
                            Id = int.Parse(parts[0]),
                            Title = parts[1],
                            Author = parts[2],
                            ISBN = parts[3],
                            Year = int.Parse(parts[4]),
                            IsAvailable = bool.Parse(parts[5]),
                            Borrower = parts[6]
                        });
                    }
                }
                Console.WriteLine($"Загружено {books.Count} книг из файла.");
            }
        }

        // Сохранение книг в файл
        private void SaveBooksToFile()
        {
            List<string> lines = new List<string>();
            foreach (Book book in books)
            {
                lines.Add($"{book.Id}|{book.Title}|{book.Author}|{book.ISBN}|{book.Year}|{book.IsAvailable}|{book.Borrower}");
            }
            File.WriteAllLines(booksFile, lines);
        }

        // Добавить книгу
        public void AddBook()
        {
            Console.WriteLine("\n--- Добавление новой книги ---");
            
            int newId = books.Count > 0 ? books[books.Count - 1].Id + 1 : 1;
            
            Console.Write("Название: ");
            string title = Console.ReadLine();
            
            Console.Write("Автор: ");
            string author = Console.ReadLine();
            
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine();
            
            Console.Write("Год издания: ");
            int year = int.Parse(Console.ReadLine());

            books.Add(new Book
            {
                Id = newId,
                Title = title,
                Author = author,
                ISBN = isbn,
                Year = year,
                IsAvailable = true
            });

            SaveBooksToFile();
            Console.WriteLine("Книга успешно добавлена!");
        }

        // Показать все книги
        public void ShowAllBooks()
        {
            Console.WriteLine("\n--- Список всех книг ---");
            if (books.Count == 0)
            {
                Console.WriteLine("В библиотеке пока нет книг.");
                return;
            }

            foreach (Book book in books)
            {
                Console.WriteLine(book);
            }
        }

        // Найти книгу
        public void FindBook()
        {
            Console.WriteLine("\n--- Поиск книги ---");
            Console.Write("Введите название или автора для поиска: ");
            string search = Console.ReadLine().ToLower();

            bool found = false;
            foreach (Book book in books)
            {
                if (book.Title.ToLower().Contains(search) || book.Author.ToLower().Contains(search))
                {
                    Console.WriteLine(book);
                    found = true;
                }
            }

            if (!found)
                Console.WriteLine("Книги не найдены.");
        }

        // Выдать книгу
        public void LendBook()
        {
            Console.WriteLine("\n--- Выдача книги ---");
            Console.Write("ID книги для выдачи: ");
            int id = int.Parse(Console.ReadLine());

            Book book = books.Find(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine("Книга не найдена.");
                return;
            }

            if (!book.IsAvailable)
            {
                Console.WriteLine($"Книга уже выдана: {book.Borrower}");
                return;
            }

            Console.Write("ФИО читателя: ");
            string borrower = Console.ReadLine();

            book.IsAvailable = false;
            book.Borrower = borrower;
            SaveBooksToFile();

            Console.WriteLine($"Книга '{book.Title}' выдана {borrower}.");
        }

        // Вернуть книгу
        public void ReturnBook()
        {
            Console.WriteLine("\n--- Возврат книги ---");
            Console.Write("ID книги для возврата: ");
            int id = int.Parse(Console.ReadLine());

            Book book = books.Find(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine("Книга не найдена.");
                return;
            }

            if (book.IsAvailable)
            {
                Console.WriteLine("Книга уже доступна в библиотеке.");
                return;
            }

            Console.WriteLine($"Возвращена книга '{book.Title}' от {book.Borrower}");
            book.IsAvailable = true;
            book.Borrower = "";
            SaveBooksToFile();

            Console.WriteLine("Книга успешно возвращена!");
        }
    }
}