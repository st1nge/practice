using System;
using System.Drawing;
using System.Windows.Forms;
using LibrarySystem.Models;

namespace LibrarySystem.Forms
{
    public class BookEditForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtGenre;
        private TextBox txtYear;
    private TextBox txtDescription;

        private Book book;
        private bool isEditMode;

        public BookEditForm(Book existing = null)
        {
            isEditMode = existing != null;
            book = existing ?? new Book();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(420, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = isEditMode ? "Редактирование книги" : "Добавление книги";

            int y = 20;

            Label lblTitle = new Label { Text = "Название:", Location = new Point(20, y), Size = new Size(360, 20) };
            txtTitle = new TextBox { Location = new Point(20, y + 25), Size = new Size(360, 28), Text = book.Title };
            y += 65;

            Label lblAuthor = new Label { Text = "Автор:", Location = new Point(20, y), Size = new Size(360, 20) };
            txtAuthor = new TextBox { Location = new Point(20, y + 25), Size = new Size(360, 28), Text = book.Author };
            y += 65;

            Label lblGenre = new Label { Text = "Жанр:", Location = new Point(20, y), Size = new Size(360, 20) };
            txtGenre = new TextBox { Location = new Point(20, y + 25), Size = new Size(360, 28), Text = book.Genre };
            y += 65;

            Label lblYear = new Label { Text = "Год издания:", Location = new Point(20, y), Size = new Size(360, 20) };
            txtYear = new TextBox { Location = new Point(20, y + 25), Size = new Size(160, 28), Text = book.Year > 0 ? book.Year.ToString() : string.Empty };
            y += 65;

            Label lblDesc = new Label { Text = "Описание:", Location = new Point(20, y), Size = new Size(360, 20) };
            txtDescription = new TextBox { Location = new Point(20, y + 25), Size = new Size(360, 120), Multiline = true, ScrollBars = ScrollBars.Vertical, Text = book.Description };
            y += 150;

            Button btnOk = new Button { Text = "Сохранить", Location = new Point(40, y), Size = new Size(140, 36), BackColor = ColorTranslator.FromHtml("#4caf50"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;

            Button btnCancel = new Button { Text = "Отмена", Location = new Point(220, y), Size = new Size(140, 36), BackColor = ColorTranslator.FromHtml("#757575"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(txtAuthor);
            this.Controls.Add(lblGenre);
            this.Controls.Add(txtGenre);
            this.Controls.Add(lblYear);
            this.Controls.Add(txtYear);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Введите автора.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            book.Title = txtTitle.Text.Trim();
            book.Author = txtAuthor.Text.Trim();
            book.Genre = txtGenre.Text.Trim();

            book.Description = txtDescription.Text.Trim();

            if (int.TryParse(txtYear.Text.Trim(), out int y))
                book.Year = y;
            else
                book.Year = 0;

            this.DialogResult = DialogResult.OK;
        }

        public Book GetBook() => book;
    }
}
