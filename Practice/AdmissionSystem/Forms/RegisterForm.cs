using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Models;
using LibrarySystem.UI;

namespace LibrarySystem.Forms
{
    public partial class RegisterForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFullName;
        private Button btnRegister;
        private Button btnCancel;
        private CheckBox chkShowPassword;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(900, 750);
            this.Text = "Регистрация нового пользователя";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = ModernUIHelper.DarkBackground;
            this.DoubleBuffered = true;

            // Фоновый градиент
            this.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    ModernUIHelper.DarkBackground,
                    ColorTranslator.FromHtml("#16192e"),
                    90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Главная карточка
            Panel cardPanel = ModernUIHelper.CreateCard(new Point(100, 40), new Size(700, 670));

            // Заголовок с иконкой
            Label lblIcon = new Label
            {
                Text = "✨",
                Font = new Font("Segoe UI", 48),
                ForeColor = ModernUIHelper.SecondaryAccent,
                Size = new Size(660, 80),
                Location = new Point(20, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblTitle = ModernUIHelper.CreateModernLabel(
                "СОЗДАТЬ НОВЫЙ АККАУНТ",
                new Point(20, 110),
                18,
                FontStyle.Bold,
                ModernUIHelper.TextPrimary
            );
            lblTitle.Size = new Size(660, 35);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblSubtitle = ModernUIHelper.CreateModernLabel(
                "Заполните форму для регистрации в системе",
                new Point(20, 145),
                10,
                FontStyle.Regular,
                ModernUIHelper.TextSecondary
            );
            lblSubtitle.Size = new Size(660, 25);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;

            // Разделитель
            Panel divider1 = ModernUIHelper.CreateDivider(new Point(20, 185), 660);

            // ФИО
            Label lblFullName = ModernUIHelper.CreateModernLabel(
                "ПОЛНОЕ ИМЯ",
                new Point(50, 210),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelFullNameBox = new Panel
            {
                Location = new Point(50, 235),
                Size = new Size(600, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtFullName = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(570, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None
            };
            panelFullNameBox.Controls.Add(txtFullName);

            // Логин
            Label lblLogin = ModernUIHelper.CreateModernLabel(
                "ЛОГИН",
                new Point(50, 300),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelLoginBox = new Panel
            {
                Location = new Point(50, 325),
                Size = new Size(600, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtLogin = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(570, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None
            };
            panelLoginBox.Controls.Add(txtLogin);

            // Пароль
            Label lblPassword = ModernUIHelper.CreateModernLabel(
                "ПАРОЛЬ",
                new Point(50, 390),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelPasswordBox = new Panel
            {
                Location = new Point(50, 415),
                Size = new Size(290, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtPassword = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(260, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None,
                UseSystemPasswordChar = true
            };
            panelPasswordBox.Controls.Add(txtPassword);

            // Подтверждение пароля
            Label lblConfirmPassword = ModernUIHelper.CreateModernLabel(
                "ПОВТОРИТЕ ПАРОЛЬ",
                new Point(360, 390),
                9,
                FontStyle.Bold,
                ModernUIHelper.TextMuted
            );

            Panel panelConfirmPasswordBox = new Panel
            {
                Location = new Point(360, 415),
                Size = new Size(290, 45),
                BackColor = ModernUIHelper.SidebarBackground
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(15, 11),
                Size = new Size(260, 30),
                Font = new Font("Segoe UI", 11),
                BackColor = ModernUIHelper.SidebarBackground,
                ForeColor = ModernUIHelper.TextPrimary,
                BorderStyle = BorderStyle.None,
                UseSystemPasswordChar = true
            };
            panelConfirmPasswordBox.Controls.Add(txtConfirmPassword);

            // Показать пароль
            chkShowPassword = new CheckBox
            {
                Text = "Показать пароли",
                Font = new Font("Segoe UI", 9),
                Size = new Size(200, 25),
                Location = new Point(50, 475),
                ForeColor = ModernUIHelper.TextSecondary,
                BackColor = Color.Transparent
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };

            // Разделитель
            Panel divider2 = ModernUIHelper.CreateDivider(new Point(20, 520), 660);

            // Кнопки
            btnRegister = ModernUIHelper.CreateGradientButton(
                "ЗАРЕГИСТРИРОВАТЬСЯ",
                new Point(50, 555),
                new Size(600, 50),
                ModernUIHelper.SuccessColor,
                ColorTranslator.FromHtml("#00a67d")
            );
            btnRegister.Click += BtnRegister_Click;

            btnCancel = new Button
            {
                Text = "ОТМЕНА",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(600, 45),
                Location = new Point(50, 615),
                BackColor = Color.Transparent,
                ForeColor = ModernUIHelper.TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#2d3561");
            btnCancel.Click += (s, e) => this.Close();

            // Добавление элементов на карточку
            cardPanel.Controls.Add(lblIcon);
            cardPanel.Controls.Add(lblTitle);
            cardPanel.Controls.Add(lblSubtitle);
            cardPanel.Controls.Add(divider1);
            cardPanel.Controls.Add(lblFullName);
            cardPanel.Controls.Add(panelFullNameBox);
            cardPanel.Controls.Add(lblLogin);
            cardPanel.Controls.Add(panelLoginBox);
            cardPanel.Controls.Add(lblPassword);
            cardPanel.Controls.Add(panelPasswordBox);
            cardPanel.Controls.Add(lblConfirmPassword);
            cardPanel.Controls.Add(panelConfirmPasswordBox);
            cardPanel.Controls.Add(chkShowPassword);
            cardPanel.Controls.Add(divider2);
            cardPanel.Controls.Add(btnRegister);
            cardPanel.Controls.Add(btnCancel);

            this.Controls.Add(cardPanel);

            // Enter для регистрации
            this.AcceptButton = btnRegister;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Валидация
            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Введите ваше ФИО!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (login.Length < 3)
            {
                MessageBox.Show("Логин должен содержать минимум 3 символа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка уникальности логина
            if (DatabaseHelper.UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User newUser = new User
                {
                    Login = login,
                    Password = password,
                    FullName = fullName,
                    Role = "User"
                };

                DatabaseHelper.RegisterUser(newUser);

                MessageBox.Show("Регистрация прошла успешно!\nТеперь вы можете войти в систему.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
