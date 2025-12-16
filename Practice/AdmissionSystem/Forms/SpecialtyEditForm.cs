using System;
using System.Drawing;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Models;
using LibrarySystem.UI;

namespace LibrarySystem.Forms
{
    public partial class SpecialtyEditForm : Form
    {
        private BookCategory specialty;
        private bool isEditMode;
    private TextBox txtName;
    private TextBox txtDescription;
    private NumericUpDown numPlaces;
    private System.Windows.Forms.DataGridView dgvBooks;
    private Button btnAddBook;
    private Button btnEditBook;
    private Button btnDeleteBook;

    // temp storage for books when creating a new category
    private System.Collections.Generic.List<Book> tempBooks = new System.Collections.Generic.List<Book>();

        public SpecialtyEditForm() : this(null) { }

        public SpecialtyEditForm(BookCategory existingSpecialty)
        {
            if (existingSpecialty != null)
            {
                specialty = existingSpecialty;
                isEditMode = true;
            }
            else
            {
                specialty = new BookCategory();
                isEditMode = false;
            }
            InitializeComponent();
            if (isEditMode)
                LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 500);
            this.Text = isEditMode ? "Редактирование категории" : "Добавление категории";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int yPosition = 20;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = isEditMode ? "РЕДАКТИРОВАНИЕ КАТЕГОРИИ" : "ДОБАВЛЕНИЕ КАТЕГОРИИ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ModernUIHelper.PrimaryAccent,
                Size = new Size(450, 40),
                Location = new Point(25, yPosition),
                TextAlign = ContentAlignment.MiddleCenter
            };
            yPosition += 50;

            // Поля категории (Название / Код / Количество книг / Описание)
            Label lblName = CreateLabel("Название:", yPosition);
            txtName = CreateTextBox(yPosition + 25);
            txtName.Text = isEditMode ? specialty.Name : string.Empty;
            txtName.Visible = true;
            lblName.Visible = true;
            yPosition += 70;

            // (Код категории скрыт — не требуется ввод пользователем)

            // Количество книг
            Label lblPlaces = CreateLabel("Количество книг:", yPosition);
            numPlaces = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(450, 30),
                Minimum = 0,
                Maximum = 10000,
                Value = isEditMode ? specialty.BooksCount : 0,
                Visible = true
            };
            lblPlaces.Visible = true;
            yPosition += 70;

            // Описание
            Label lblDescription = CreateLabel("Описание:", yPosition);
            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(450, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = isEditMode ? specialty.Description : string.Empty,
                Visible = true
            };
            lblDescription.Visible = true;
            yPosition += 110;

            // Секция: книги в категории
            Label lblBooksSection = CreateLabel("Книги в категории:", yPosition);
            yPosition += 25;

            dgvBooks = new System.Windows.Forms.DataGridView
            {
                Location = new Point(25, yPosition),
                Size = new Size(450, 120),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersVisible = true,
                RowHeadersVisible = false
            };
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ModernUIHelper.StyleDataGridView(dgvBooks);
            yPosition += 140;

            btnAddBook = new Button
            {
                Text = "Добавить книгу",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(140, 36),
                Location = new Point(25, yPosition),
                BackColor = ColorTranslator.FromHtml("#3a6b35"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddBook.FlatAppearance.BorderSize = 0;
            btnAddBook.Click += (s, e) => BtnAddBook_Click(s, e);

            btnEditBook = new Button
            {
                Text = "Редактировать",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(120, 36),
                Location = new Point(180, yPosition),
                BackColor = ColorTranslator.FromHtml("#b99a72"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEditBook.FlatAppearance.BorderSize = 0;
            btnEditBook.Click += (s, e) => BtnEditBook_Click(s, e);

            btnDeleteBook = new Button
            {
                Text = "Удалить",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Size = new Size(100, 36),
                Location = new Point(320, yPosition),
                BackColor = ColorTranslator.FromHtml("#dc3545"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteBook.FlatAppearance.BorderSize = 0;
            btnDeleteBook.Click += (s, e) => BtnDeleteBook_Click(s, e);
            yPosition += 50;

            // Кнопки
            Button btnSave = new Button
            {
                Text = "СОХРАНИТЬ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(25, yPosition),
                BackColor = ColorTranslator.FromHtml("#4caf50"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => BtnSave_Click(s, e);

            Button btnCancel = new Button
            {
                Text = "ОТМЕНА",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(275, yPosition),
                BackColor = ColorTranslator.FromHtml("#757575"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавление контролов
            this.Controls.Add(lblTitle);
            // Enable scrolling so controls placed below the initial client area are reachable
            this.AutoScroll = true;
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblPlaces);
            this.Controls.Add(numPlaces);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblBooksSection);

            // Inline inputs must be added to the form so they are visible
            this.Controls.Add(dgvBooks);
            // Ensure the form can scroll to show all created controls
            this.AutoScrollMinSize = new Size(0, yPosition + 80);
            this.Controls.Add(btnAddBook);
            this.Controls.Add(btnEditBook);
            this.Controls.Add(btnDeleteBook);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadBooks()
        {
            try
            {
                // Build a clean view with only the required columns: Title, Author, Genre, Year, Description
                dgvBooks.Columns.Clear();
                dgvBooks.AutoGenerateColumns = false;

                var colTitle = new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Название", Name = "colTitle", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                var colAuthor = new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "Автор", Name = "colAuthor", Width = 150 };
                var colGenre = new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "Жанр", Name = "colGenre", Width = 120 };
                var colYear = new DataGridViewTextBoxColumn { DataPropertyName = "Year", HeaderText = "Год", Name = "colYear", Width = 80 };
                var colDesc = new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "Описание", Name = "colDescription", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };

                dgvBooks.Columns.AddRange(new DataGridViewColumn[] { colTitle, colAuthor, colGenre, colYear, colDesc });

                System.Collections.Generic.List<Book> books = isEditMode ? DatabaseHelper.GetBooksByCategory(specialty.Id) : tempBooks;

                dgvBooks.DataSource = null;
                dgvBooks.DataSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    private void BtnAddBook_Click(object? sender, EventArgs e)
        {
            // Open modal BookEditForm to add a book
            var editor = new BookEditForm();
            var res = editor.ShowDialog(this);
            if (res != DialogResult.OK) return;

            var book = editor.GetBook();
            if (book == null) return;

            if (isEditMode && specialty.Id > 0)
            {
                book.CategoryId = specialty.Id;
                DatabaseHelper.AddBook(book);
                try { if (this.Owner is AdminPanel ap) ap.RefreshSpecialties(); } catch { }
            }
            else
            {
                var tempId = (tempBooks.Count > 0) ? (tempBooks[tempBooks.Count - 1].Id - 1) : -1;
                book.Id = tempId;
                book.CategoryId = 0;
                tempBooks.Add(book);
            }

            LoadBooks();
        }

    private void BtnEditBook_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите книгу для редактирования.", "Выберите книгу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            var book = row.DataBoundItem as Book;
            if (book == null) return;

            // If the book is from DB (has positive Id) we can pass a copy to the editor and then save changes.
            var bookCopy = new Book { Id = book.Id, Title = book.Title, Author = book.Author, Genre = book.Genre, Year = book.Year, Description = book.Description, Count = book.Count, CategoryId = book.CategoryId };
            var editor = new BookEditForm(bookCopy);
            var res = editor.ShowDialog(this);
            if (res != DialogResult.OK) return;

            var edited = editor.GetBook();
            if (edited == null) return;

            if (isEditMode && edited.Id > 0)
            {
                // save to DB
                DatabaseHelper.UpdateBook(edited);
                try { if (this.Owner is AdminPanel ap) ap.RefreshSpecialties(); } catch { }
            }
            else
            {
                // update temp list
                var idx = tempBooks.FindIndex(x => x.Id == edited.Id);
                if (idx >= 0)
                {
                    tempBooks[idx].Title = edited.Title;
                    tempBooks[idx].Author = edited.Author;
                    tempBooks[idx].Genre = edited.Genre;
                    tempBooks[idx].Year = edited.Year;
                    tempBooks[idx].Description = edited.Description;
                }
            }

            LoadBooks();
        }

    private void BtnDeleteBook_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите книгу для удаления.", "Выберите книгу", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            var book = row.DataBoundItem as Book;
            if (book == null) return;

            var res = MessageBox.Show($"Удалить книгу '{book.Title}'?", "Подтвердите", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                if (isEditMode && book.Id > 0)
                {
                    DatabaseHelper.DeleteBook(book.Id);
                    // Обновим список категорий в админ-панели, если она владелец формы
                    try
                    {
                        if (this.Owner is AdminPanel ap)
                        {
                            ap.RefreshSpecialties();
                        }
                    }
                    catch { }
                }
                else
                {
                    // remove from temp list
                    tempBooks.RemoveAll(x => x.Id == book.Id);
                }
                LoadBooks();
            }
        }

        // Inline inputs were replaced by a modal editor; no inline-clear required.

        private Label CreateLabel(string text, int yPosition)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(450, 20)
            };
        }

        private TextBox CreateTextBox(int yPosition)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition),
                Size = new Size(450, 30),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

    private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Валидация: пользователь должен ввести название и код категории
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                specialty.Name = txtName.Text.Trim();
                // Generate a hidden internal code for the category (not shown to user)
                if (string.IsNullOrWhiteSpace(specialty.Code))
                {
                    specialty.Code = "CAT" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                }
                specialty.Description = txtDescription.Text.Trim();
                // При редактировании используем значение из поля, при создании новой категории сбрасываем в 0,
                // потому что книги будут добавлены ниже (tempBooks) и тогда BooksCount посчитается корректно.
                if (isEditMode)
                    specialty.BooksCount = (int)numPlaces.Value;
                else
                    specialty.BooksCount = 0;

                if (isEditMode)
                {
                    DatabaseHelper.UpdateBookCategory(specialty);
                    MessageBox.Show("Категория успешно обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBooks();
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    var newId = DatabaseHelper.AddBookCategory(specialty);

                    // Получаем сохраненную запись (чтобы получить Id) и переключаем форму в режим редактирования
                    var saved = DatabaseHelper.GetBookCategoryById(newId);
                    if (saved != null)
                    {
                        specialty = saved;
                        isEditMode = true;
                        MessageBox.Show("Категория успешно добавлена! Теперь вы можете добавить книги.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Обновляем список книг (он будет пустым) — форма остаётся открытой для добавления книг
                        LoadBooks();
                        // Не закрываем форму: пользователь может сразу добавить книги
                        // Если были временные книги — сохраним их в БД
                        if (tempBooks != null && tempBooks.Count > 0)
                        {
                            foreach (var tb in tempBooks)
                            {
                                tb.CategoryId = specialty.Id;
                                // reset temporary negative id
                                tb.Id = 0;
                                DatabaseHelper.AddBook(tb);
                            }
                            tempBooks.Clear();
                            LoadBooks();
                                // Если форма была открыта из AdminPanel — попросим её обновить список категорий
                                try
                                {
                                    if (this.Owner is AdminPanel ap)
                                    {
                                        ap.RefreshSpecialties();
                                    }
                                }
                                catch { }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Категория добавлена, но не удалось получить её id.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Оставляем форму открытой
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
