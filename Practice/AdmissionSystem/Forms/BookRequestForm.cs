using System;
using System.Drawing;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Models;
using LibrarySystem.UI;

namespace LibrarySystem.Forms
{
    public partial class BookRequestForm : Form
    {
        private User currentUser;
        private BookCategory category;

        private TextBox txtBookTitle;
        private TextBox txtAuthor;
        private TextBox txtISBN;
        private ComboBox cmbCategory;
        private Button btnSubmit;
        private Button btnCancel;

        public BookRequestForm(User user)
        {
            currentUser = user;
            InitializeComponent();
        }

        // Новый конструктор: позволяет передать заранее выбранную категорию
        public BookRequestForm(User user, BookCategory presetCategory)
        {
            currentUser = user;
            category = presetCategory;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 400);
            this.Text = "Запрос на книгу";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int yPosition = 20;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "ЗАПРОС НА КНИГУ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ModernUIHelper.PrimaryAccent,
                Size = new Size(450, 40),
                Location = new Point(25, yPosition),
                TextAlign = ContentAlignment.MiddleCenter
            };
            yPosition += 50;

            // Категория
            Label lblCategory = new Label
            {
                Text = "Категория:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(100, 25)
            };
            cmbCategory = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(130, yPosition),
                Size = new Size(320, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Загрузка категорий
            var categories = DatabaseHelper.GetAllBookCategories();
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            // Если передали preset категорию, выбираем её
            if (category != null)
            {
                cmbCategory.SelectedValue = category.Id;
            }
            yPosition += 40;

            // Название книги
            Label lblBookTitle = new Label
            {
                Text = "Название книги:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(120, 25)
            };
            txtBookTitle = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(425, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPosition += 70;

            // Автор
            Label lblAuthor = new Label
            {
                Text = "Автор:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(100, 25)
            };
            txtAuthor = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(425, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPosition += 70;

            // ISBN
            Label lblISBN = new Label
            {
                Text = "ISBN (необязательно):",
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(150, 25)
            };
            txtISBN = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(425, 25),
                BorderStyle = BorderStyle.FixedSingle
            };
            yPosition += 70;

            // Кнопки
            btnSubmit = new Button
            {
                Text = "ОТПРАВИТЬ ЗАПРОС",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 40),
                Location = new Point(25, yPosition),
                BackColor = ModernUIHelper.SuccessColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;

            btnCancel = new Button
            {
                Text = "ОТМЕНА",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 40),
                Location = new Point(250, yPosition),
                BackColor = ModernUIHelper.SecondaryAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавление контролов
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblCategory);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(lblBookTitle);
            this.Controls.Add(txtBookTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(txtAuthor);
            this.Controls.Add(lblISBN);
            this.Controls.Add(txtISBN);
            this.Controls.Add(btnSubmit);
            this.Controls.Add(btnCancel);
        }

        private Label CreateLabel(string text, int yPosition)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(550, 20)
            };
        }

        private TextBox CreateTextBox(int yPosition)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(550, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtBookTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                BookRequest request = new BookRequest
                {
                    UserId = currentUser.Id,
                    BookCategoryId = ((BookCategory)cmbCategory.SelectedItem).Id,
                    BookTitle = txtBookTitle.Text.Trim(),
                    Author = txtAuthor.Text.Trim(),
                    ISBN = txtISBN.Text.Trim()
                };

                DatabaseHelper.AddBookRequest(request);

                MessageBox.Show("Заявка на книгу успешно подана!\nОжидайте рассмотрения администратором.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подаче заявки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
