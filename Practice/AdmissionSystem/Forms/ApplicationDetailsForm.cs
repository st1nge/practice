using System;
using System.Drawing;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Models;

namespace LibrarySystem.Forms
{
    public partial class ApplicationDetailsForm : Form
    {
        private BookRequest application;
        private bool isAdminMode;

        public ApplicationDetailsForm(BookRequest app, bool isAdmin = false)
        {
            application = app;
            isAdminMode = isAdmin;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Детали запроса на книгу";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = ColorTranslator.FromHtml("#0a0e27");
            this.Padding = new Padding(20);

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "ПОДРОБНАЯ ИНФОРМАЦИЯ О ЗАПРОСЕ НА КНИГУ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = UI.ModernUIHelper.TextPrimary,
                Size = new Size(550, 40),
                Location = new Point(0, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Основная панель с информацией
            Panel infoPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(560, 500),
                BackColor = UI.ModernUIHelper.CardBackground,
                Padding = new Padding(20)
            };

            int yPos = 20;
            int labelWidth = 150;
            int valueWidth = 350;

            // Название книги
            AddInfoRow(infoPanel, "Название книги:", application.BookTitle, ref yPos, labelWidth, valueWidth);
            
            // Автор
            AddInfoRow(infoPanel, "Автор:", application.Author, ref yPos, labelWidth, valueWidth);
            // Жанр
            AddInfoRow(infoPanel, "Жанр:", application.Genre, ref yPos, labelWidth, valueWidth);
            // Год издания
            AddInfoRow(infoPanel, "Год издания:", application.Year > 0 ? application.Year.ToString() : "", ref yPos, labelWidth, valueWidth);
            
            // ISBN
            AddInfoRow(infoPanel, "ISBN:", application.ISBN, ref yPos, labelWidth, valueWidth);
            
            // Категория
            AddInfoRow(infoPanel, "Категория:", application.CategoryName, ref yPos, labelWidth, valueWidth);
            
            // Дата заявки
            AddInfoRow(infoPanel, "Дата заявки:", application.SubmittedAt, ref yPos, labelWidth, valueWidth);
            
            // Статус
            Color statusColor = application.Status == "Одобрено" ? UI.ModernUIHelper.SuccessColor :
                               application.Status == "Отклонено" ? UI.ModernUIHelper.DangerColor : UI.ModernUIHelper.WarningColor;
            AddInfoRow(infoPanel, "Статус:", application.Status, ref yPos, labelWidth, valueWidth, statusColor);
            
            // Заметки (только для админа)
            if (!string.IsNullOrEmpty(application.Notes) && isAdminMode)
            {
                AddInfoRow(infoPanel, "Заметки:", application.Notes, ref yPos, labelWidth, valueWidth);
            }

            // Кнопки
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, 570),
                Size = new Size(560, 80),
                BackColor = Color.Transparent
            };

            Button btnClose = UI.ModernUIHelper.CreateGradientButton(
                "ЗАКРЫТЬ",
                new Point(200, 20),
                new Size(160, 45),
                UI.ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnClose.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Кнопки действий для админа
            if (isAdminMode && application.Status == "На рассмотрении")
            {
                Button btnApprove = UI.ModernUIHelper.CreateGradientButton(
                    "ОДОБРИТЬ",
                    new Point(20, 20),
                    new Size(160, 45),
                    UI.ModernUIHelper.SuccessColor,
                    ColorTranslator.FromHtml("#00a67d")
                );
                btnApprove.Click += (s, e) => ApproveApplication();

                Button btnReject = UI.ModernUIHelper.CreateGradientButton(
                    "ОТКЛОНИТЬ",
                    new Point(380, 20),
                    new Size(160, 45),
                    UI.ModernUIHelper.DangerColor,
                    ColorTranslator.FromHtml("#e66565")
                );
                btnReject.Click += (s, e) => RejectApplication();

                buttonPanel.Controls.Add(btnApprove);
                buttonPanel.Controls.Add(btnReject);
            }
            else
            {
                btnClose.Location = new Point(200, 20);
            }

            buttonPanel.Controls.Add(btnClose);

            // Добавляем контролы
            this.Controls.Add(lblTitle);
            this.Controls.Add(infoPanel);
            this.Controls.Add(buttonPanel);
        }

        private void AddInfoRow(Panel panel, string label, string value, ref int yPos, int labelWidth, int valueWidth, Color? valueColor = null)
        {
            // Метка
            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = UI.ModernUIHelper.TextSecondary,
                Location = new Point(0, yPos),
                Size = new Size(labelWidth, 25),
                BackColor = Color.Transparent
            };

            // Значение
            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10),
                ForeColor = valueColor ?? UI.ModernUIHelper.TextPrimary,
                Location = new Point(labelWidth, yPos),
                Size = new Size(valueWidth, 25),
                BackColor = Color.Transparent
            };

            panel.Controls.Add(lblLabel);
            panel.Controls.Add(lblValue);
            yPos += 30;
        }

        private void ApproveApplication()
        {
            if (MessageBox.Show("Одобрить это заявление?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.UpdateBookRequestStatus(application.Id, "Одобрено");
                    application.Status = "Одобрено";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось одобрить заявку", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RejectApplication()
        {
            if (MessageBox.Show("Отклонить это заявление?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.UpdateBookRequestStatus(application.Id, "Отклонено");
                    application.Status = "Отклонено";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось отклонить заявление", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
