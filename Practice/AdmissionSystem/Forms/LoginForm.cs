using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using LibrarySystem.Database;
using LibrarySystem.Models;
using LibrarySystem.UI;

namespace LibrarySystem.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private LinkLabel linkInstruction;
        private Panel panelLeft;
        private Panel panelRight;
        private CheckBox chkShowPassword;

        public LoginForm()
        {
            // Инициализируем базу данных в конструкторе
            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не показываем пользователю
                Console.WriteLine($"Ошибка инициализации базы данных: {ex.Message}");
                // Можно создать файл базы данных в памяти или использовать другой метод
            }
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1000, 650);
            this.Text = "Библиотека БППК - Вход";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = ModernUIHelper.DarkBackground;
            this.DoubleBuffered = true;

            // Левая декоративная панель с градиентом
            panelLeft = new Panel
            {
                Size = new Size(400, 650),
                Location = new Point(0, 0),
                BackColor = ModernUIHelper.SidebarBackground
            };
            panelLeft.Paint += PanelLeft_Paint;

            // Логотип и текст на левой панели
            Label lblLogo = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI", 72),
                ForeColor = ModernUIHelper.PrimaryAccent,
                Size = new Size(350, 120),
                Location = new Point(25, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblAppName = new Label
            {
                Text = "БИБЛИОТЕКА\nБППК",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = ModernUIHelper.TextPrimary,
                Size = new Size(350, 120),
                Location = new Point(25, 280),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblAppSubtitle = new Label
            {
                Text = "Система управления библиотекой",
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextSecondary,
                Size = new Size(350, 60),
                Location = new Point(25, 410),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            panelLeft.Controls.Add(lblLogo);
            panelLeft.Controls.Add(lblAppName);
            panelLeft.Controls.Add(lblAppSubtitle);

            // Правая панель с формой входа
            panelRight = new Panel
            {
                Size = new Size(600, 650),
                Location = new Point(400, 0),
                BackColor = ModernUIHelper.CardBackground
            };

            // Заголовок формы
            Label lblFormTitle = ModernUIHelper.CreateModernLabel(
                "ВХОД В СИСТЕМУ",
                new Point(80, 80),
                20,
                FontStyle.Bold,
                ModernUIHelper.TextPrimary
            );

            Label lblFormSubtitle = ModernUIHelper.CreateModernLabel(
                "Введите ваши учётные данные для продолжения",
                new Point(80, 120),
                10,
                FontStyle.Regular,
                ModernUIHelper.TextSecondary
            );

            // Логин
            Label lblLogin = ModernUIHelper.CreateModernLabel(
                "ЛОГИН",
                new Point(80, 190),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelLoginBox = new Panel
            {
                Location = new Point(80, 215),
                Size = new Size(440, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtLogin = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 12),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None
            };
            panelLoginBox.Controls.Add(txtLogin);

            // Пароль
            Label lblPassword = ModernUIHelper.CreateModernLabel(
                "ПАРОЛЬ",
                new Point(80, 285),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelPasswordBox = new Panel
            {
                Location = new Point(80, 310),
                Size = new Size(440, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtPassword = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 12),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None,
                UseSystemPasswordChar = true
            };
            panelPasswordBox.Controls.Add(txtPassword);

            // Показать пароль
            chkShowPassword = new CheckBox
            {
                Text = "Показать пароль",
                Font = new Font("Segoe UI", 9),
                Size = new Size(200, 25),
                Location = new Point(80, 365),
                ForeColor = ModernUIHelper.TextSecondary,
                BackColor = Color.Transparent
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };

            // Кнопка входа с градиентом
            btnLogin = ModernUIHelper.CreateGradientButton(
                "ВОЙТИ В СИСТЕМУ",
                new Point(80, 415),
                new Size(440, 50),
                ModernUIHelper.PrimaryAccent,
                ColorTranslator.FromHtml("#5f4dd4")
            );
            btnLogin.Click += BtnLogin_Click;

            // Кнопка регистрации
            btnRegister = ModernUIHelper.CreateGradientButton(
                "РЕГИСТРАЦИЯ",
                new Point(80, 480),
                new Size(440, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRegister.Click += BtnRegister_Click;

            // Ссылка на инструкцию
            linkInstruction = new LinkLabel
            {
                Text = "📖 Инструкция пользователя",
                Font = new Font("Segoe UI", 10),
                Size = new Size(440, 30),
                Location = new Point(80, 560),
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = ModernUIHelper.SecondaryAccent,
                ActiveLinkColor = ModernUIHelper.PrimaryAccent,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            linkInstruction.LinkClicked += LinkInstruction_LinkClicked;

            // Добавление элементов на правую панель
            panelRight.Controls.Add(lblFormTitle);
            panelRight.Controls.Add(lblFormSubtitle);
            panelRight.Controls.Add(lblLogin);
            panelRight.Controls.Add(panelLoginBox);
            panelRight.Controls.Add(lblPassword);
            panelRight.Controls.Add(panelPasswordBox);
            panelRight.Controls.Add(chkShowPassword);
            panelRight.Controls.Add(btnLogin);
            panelRight.Controls.Add(btnRegister);
            panelRight.Controls.Add(linkInstruction);

            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);

            // Enter для входа
            this.AcceptButton = btnLogin;
        }

        private void PanelLeft_Paint(object sender, PaintEventArgs e)
        {
            // Рисуем градиент на левой панели
            using (var brush = new LinearGradientBrush(
                panelLeft.ClientRectangle,
                ColorTranslator.FromHtml("#6c5ce7"),
                ColorTranslator.FromHtml("#0984e3"),
                45F))
            {
                e.Graphics.FillRectangle(brush, panelLeft.ClientRectangle);
            }

            // Добавляем декоративные круги
            using (var circleBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
            {
                e.Graphics.FillEllipse(circleBrush, -50, -50, 200, 200);
                e.Graphics.FillEllipse(circleBrush, 250, 450, 250, 250);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User user = DatabaseHelper.GetUser(login, password);

                if (user != null)
                {
                    this.Hide();

                    if (user.Role == "Admin")
                    {
                        AdminPanel adminPanel = new AdminPanel(user);
                        adminPanel.FormClosed += (s, args) => this.Close();
                        adminPanel.Show();
                    }
                    else
                    {
                        UserPanel userPanel = new UserPanel(user);
                        userPanel.FormClosed += (s, args) => this.Close();
                        userPanel.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к базе данных. Пожалуйста, попробуйте позже.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                RegisterForm registerForm = new RegisterForm();
                registerForm.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось открыть форму регистрации", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkInstruction_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Сначала ищем в корне проекта
            string rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Инструкция_пользователя.docx");
            string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Инструкция_пользователя.docx");

            string instructionPath = File.Exists(rootPath) ? rootPath : resourcesPath;

            if (File.Exists(instructionPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path.GetFullPath(instructionPath),
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть инструкцию: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Файл инструкции не найден!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
