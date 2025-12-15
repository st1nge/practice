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
        private TextBox txtCode;
        private TextBox txtDescription;
        private NumericUpDown numPlaces;

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

            // Название
            Label lblName = CreateLabel("Название:", yPosition);
            txtName = CreateTextBox(yPosition + 25);
            txtName.Text = isEditMode ? specialty.Name : string.Empty;
            yPosition += 70;

            // Код
            Label lblCode = CreateLabel("Код:", yPosition);
            txtCode = CreateTextBox(yPosition + 25);
            txtCode.Text = isEditMode ? specialty.Code : string.Empty;
            yPosition += 70;

            // Количество книг
            Label lblPlaces = CreateLabel("Количество книг:", yPosition);
            numPlaces = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(450, 30),
                Minimum = 1,
                Maximum = 10000,
                Value = isEditMode ? specialty.BooksCount : 100
            };
            yPosition += 70;

            // Описание
            Label lblDescription = CreateLabel("Описание:", yPosition);
            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, yPosition + 25),
                Size = new Size(450, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = isEditMode ? specialty.Description : string.Empty
            };
            yPosition += 150;

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
            btnSave.Click += BtnSave_Click;

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
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblCode);
            this.Controls.Add(txtCode);
            this.Controls.Add(lblPlaces);
            this.Controls.Add(numPlaces);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Введите код категории!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                specialty.Name = txtName.Text.Trim();
                specialty.Code = txtCode.Text.Trim();
                specialty.Description = txtDescription.Text.Trim();
                specialty.BooksCount = (int)numPlaces.Value;

                if (isEditMode)
                {
                    DatabaseHelper.UpdateBookCategory(specialty);
                }
                else
                {
                    DatabaseHelper.AddBookCategory(specialty);
                }

                MessageBox.Show($"Категория успешно {(isEditMode ? "обновлена" : "добавлена")}!",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
