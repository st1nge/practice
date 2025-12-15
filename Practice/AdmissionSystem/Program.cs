using System;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Forms;

namespace LibrarySystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Инициализация базы данных
            DatabaseHelper.InitializeDatabase();

            // Запуск формы входа
            Application.Run(new LoginForm());
        }
    }
}
