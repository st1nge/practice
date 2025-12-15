using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibrarySystem.Database;
using LibrarySystem.Models;
using LibrarySystem.UI;

namespace LibrarySystem.Forms
{
    public partial class UserPanel : Form
    {
        private User currentUser;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Panel specialtiesPanel;
        private Panel applicationsPanel;
        private FlowLayoutPanel cardsFlowPanel;
        private DataGridView dgvSpecialties;
        private Button btnSpecialtiesNav;
        private Button btnApplicationsNav;
        private Label lblPageTitle;
        private Button btnRefreshCards;

        public UserPanel(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadData();
            ShowSpecialtiesPanel();
        }

        private void TakeBook()
        {
            if (dgvSpecialties.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите категорию книги для взятия!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var category = (BookCategory)dgvSpecialties.SelectedRows[0].DataBoundItem;

                if (category.BooksCount <= 0)
                {
                    MessageBox.Show("В этой категории нет доступных книг.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Создаём запись о выдаче книги — минимальные данные, статус "Выдано"
                var request = new BookRequest
                {
                    UserId = currentUser.Id,
                    BookCategoryId = category.Id,
                        BookTitle = $"(Категория) {category.Name}",
                    Author = string.Empty,
                    ISBN = string.Empty,
                        RequestDate = DateTime.Now,
                    Status = "Выдано",
                        SubmissionDate = DateTime.Now,
                    Notes = "Взятие книги пользователем"
                };

                DatabaseHelper.AddBookRequest(request);

                // Уменьшаем количество книг в категории
                category.BooksCount = Math.Max(0, category.BooksCount - 1);
                DatabaseHelper.UpdateBookCategory(category);

                MessageBox.Show("Книга выдана и отображена в ваших запросах.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Обновляем интерфейс
                LoadSpecialties();
                LoadApplicationsCards();
                ShowApplicationsPanel();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось выдать книгу. Попробуйте ещё раз.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1500, 900);
            this.Text = "Панель пользователя";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUIHelper.DarkBackground;
            this.DoubleBuffered = true;

            // Верхняя панель навигации (топбар)
            sidebarPanel = ModernUIHelper.CreateTopbar(new Size(1500, 100));

            // Логотип и приветствие (слева)
            Label lblLogo = new Label
            {
                Text = "БИБЛИО",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = ModernUIHelper.PrimaryAccent,
                Size = new Size(220, 60),
                Location = new Point(20, 18),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            // Заголовок убран по запросу (чтобы не мешал дизайну)

            Label lblUserName = new Label
            {
                Text = currentUser.FullName,
                Font = new Font("Segoe UI", 10),
                ForeColor = ModernUIHelper.TextSecondary,
                Size = new Size(260, 24),
                Location = new Point(1200, 38),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            // Навигационные кнопки
            Button btnCats = new Button
            {
                Text = "Категории книг",
                Location = new Point(520, 25),
                Size = new Size(160, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextPrimary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnCats.FlatAppearance.BorderSize = 0;
            btnCats.Click += (s, e) => ShowSpecialtiesPanel();
            btnSpecialtiesNav = btnCats;

            Button btnApps = new Button
            {
                Text = "Мои запросы",
                Location = new Point(690, 25),
                Size = new Size(140, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnApps.FlatAppearance.BorderSize = 0;
            btnApps.Click += (s, e) => ShowApplicationsPanel();
            btnApplicationsNav = btnApps;

            Button btnLogout = new Button
            {
                Text = "Выход",
                Location = new Point(1380, 28),
                Size = new Size(100, 44),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.DangerColor,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => this.Close();

            sidebarPanel.Controls.Add(lblLogo);
            sidebarPanel.Controls.Add(lblUserName);
            sidebarPanel.Controls.Add(btnCats);
            sidebarPanel.Controls.Add(btnApps);
            sidebarPanel.Controls.Add(btnLogout);

            // Панель контента (под топбаром)
            contentPanel = new Panel
            {
                Location = new Point(0, 100),
                Size = new Size(1500, 800),
                BackColor = ModernUIHelper.CardBackground
            };

            // Заголовок страницы
            lblPageTitle = new Label
            {
                Text = "ДОСТУПНЫЕ КАТЕГОРИИ КНИГ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = ModernUIHelper.TextPrimary,
                Size = new Size(1200, 60),
                Location = new Point(40, 30),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(lblPageTitle);

            // Создаем панели для разных разделов
            CreateSpecialtiesPanel();
            CreateApplicationsPanel();

            this.Controls.Add(sidebarPanel);
            this.Controls.Add(contentPanel);
        }

        private void CreateSpecialtiesPanel()
        {
            specialtiesPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(1160, 750),
                BackColor = Color.Transparent,
                Visible = true
            };

            // DataGridView для специальностей
            dgvSpecialties = new DataGridView
            {
                Location = new Point(0, 70),
                Size = new Size(1160, 550),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            ModernUIHelper.StyleDataGridView(dgvSpecialties);

            // Панель с кнопками
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, 640),
                Size = new Size(1160, 80),
                BackColor = Color.Transparent
            };

            Button btnSubmit = ModernUIHelper.CreateGradientButton(
                "ПОДАТЬ ЗАЯВЛЕНИЕ",
                new Point(0, 10),
                new Size(280, 50),
                ModernUIHelper.PrimaryAccent,
                ColorTranslator.FromHtml("#5f4dd4")
            );
            btnSubmit.Click += (s, e) => SubmitApplication();

            Button btnTake = ModernUIHelper.CreateGradientButton(
                "ВЗЯТЬ КНИГУ",
                new Point(300, 10),
                new Size(220, 50),
                ModernUIHelper.SuccessColor,
                ColorTranslator.FromHtml("#00a67d")
            );
            btnTake.Click += (s, e) => TakeBook();

            Button btnRefresh = ModernUIHelper.CreateGradientButton(
                "ОБНОВИТЬ",
                new Point(300, 10),
                new Size(220, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRefresh.Click += (s, e) => LoadSpecialties();

            buttonPanel.Controls.Add(btnSubmit);
            buttonPanel.Controls.Add(btnTake);
            buttonPanel.Controls.Add(btnRefresh);

            specialtiesPanel.Controls.Add(dgvSpecialties);
            specialtiesPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(specialtiesPanel);
        }

        private void CreateApplicationsPanel()
        {
            applicationsPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(1160, 750),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Заголовок панели заявлений
            Label lblAppsTitle = new Label
            {
                Text = "МОИ ЗАЯВЛЕНИЯ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ModernUIHelper.TextSecondary,
                Location = new Point(0, 10),
                Size = new Size(1160, 40),
                BackColor = Color.Transparent
            };

            // Контейнер для карточек с прокруткой
            Panel scrollPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(1140, 550),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // FlowLayoutPanel для автоматического расположения карточек
            cardsFlowPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(1120, 550),
                BackColor = Color.Transparent,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };

            scrollPanel.Controls.Add(cardsFlowPanel);

            // Панель с кнопками
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, 640),
                Size = new Size(1160, 80),
                BackColor = Color.Transparent
            };

            Button btnDelete = ModernUIHelper.CreateGradientButton(
                "УДАЛИТЬ",
                new Point(0, 10),
                new Size(220, 50),
                ModernUIHelper.DangerColor,
                ColorTranslator.FromHtml("#e66565")
            );
            btnDelete.Click += (s, e) => DeleteApplication();

            btnRefreshCards = ModernUIHelper.CreateGradientButton(
                "ОБНОВИТЬ",
                new Point(240, 10),
                new Size(220, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRefreshCards.Click += (s, e) => LoadApplicationsCards();

            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefreshCards);

            applicationsPanel.Controls.Add(lblAppsTitle);
            applicationsPanel.Controls.Add(scrollPanel);
            applicationsPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(applicationsPanel);
        }

        private void ShowSpecialtiesPanel()
        {
            specialtiesPanel.Visible = true;
            applicationsPanel.Visible = false;

            btnSpecialtiesNav.BackColor = ModernUIHelper.PrimaryAccent;
            btnSpecialtiesNav.ForeColor = ModernUIHelper.TextPrimary;
            btnApplicationsNav.BackColor = Color.Transparent;
            btnApplicationsNav.ForeColor = ModernUIHelper.TextSecondary;

            lblPageTitle.Text = "ДОСТУПНЫЕ СПЕЦИАЛЬНОСТИ";
        }

        private void ShowApplicationsPanel()
        {
            specialtiesPanel.Visible = false;
            applicationsPanel.Visible = true;

            btnSpecialtiesNav.BackColor = Color.Transparent;
            btnSpecialtiesNav.ForeColor = ModernUIHelper.TextSecondary;
            btnApplicationsNav.BackColor = ModernUIHelper.PrimaryAccent;
            btnApplicationsNav.ForeColor = ModernUIHelper.TextPrimary;

            lblPageTitle.Text = "МОИ ЗАЯВЛЕНИЯ";
            
            // Обновляем карточки при переходе на вкладку
            LoadApplicationsCards();
        }

        private void LoadData()
        {
            try
            {
                LoadSpecialties();
            }
            catch (Exception)
            {
                // Не показываем ошибку пользователю
            }
        }

        private void LoadSpecialties()
        {
            try
            {
                List<BookCategory> categories = DatabaseHelper.GetAllBookCategories();
                
                if (dgvSpecialties == null) return;
                
                dgvSpecialties.DataSource = null;
                dgvSpecialties.DataSource = categories;

                if (dgvSpecialties.Columns.Count > 0)
                {
                    if (dgvSpecialties.Columns.Contains("Id"))
                    {
                        var idColumn = dgvSpecialties.Columns["Id"];
                        if (idColumn != null)
                            idColumn.HeaderText = "ID";
                    }

                    if (dgvSpecialties.Columns.Contains("Name"))
                    {
                        var nameColumn = dgvSpecialties.Columns["Name"];
                        if (nameColumn != null)
                            nameColumn.HeaderText = "Название";
                    }

                    if (dgvSpecialties.Columns.Contains("Code"))
                    {
                        var codeColumn = dgvSpecialties.Columns["Code"];
                        if (codeColumn != null)
                            codeColumn.HeaderText = "Код";
                    }

                    if (dgvSpecialties.Columns.Contains("BooksCount"))
                    {
                        var booksColumn = dgvSpecialties.Columns["BooksCount"];
                        if (booksColumn != null)
                            booksColumn.HeaderText = "Кол-во книг";
                    }

                    if (dgvSpecialties.Columns.Contains("Description"))
                    {
                        var descColumn = dgvSpecialties.Columns["Description"];
                        if (descColumn != null)
                            descColumn.HeaderText = "Описание";
                    }
                }
            }
            catch (Exception)
            {
                // Не показываем ошибку пользователю
                if (dgvSpecialties != null)
                {
                    dgvSpecialties.DataSource = null;
                }
            }
        }

        private void LoadApplicationsCards()
        {
            // Очищаем старые карточки
            cardsFlowPanel.Controls.Clear();

            try
            {
                // Получаем заявления пользователя
                List<BookRequest> applications = DatabaseHelper.GetUserBookRequests(currentUser.Id);

                if (applications.Count == 0)
                {
                    // Сообщение если нет заявлений
                    Label lblNoApps = new Label
                    {
                        Text = "У вас пока нет заявлений.\nПерейдите в раздел 'Специальности' чтобы подать заявление.",
                        Font = new Font("Segoe UI", 12),
                        ForeColor = ModernUIHelper.TextSecondary,
                        Size = new Size(1100, 100),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    cardsFlowPanel.Controls.Add(lblNoApps);
                    return;
                }

                // Создаем карточки для каждого заявления
                foreach (var app in applications)
                {
                    var card = ModernUIHelper.CreateApplicationCard(app, (s, e) =>
                    {
                        // При клике на карточку открываем детали
                        ApplicationDetailsForm detailsForm = new ApplicationDetailsForm(app, false);
                        detailsForm.ShowDialog();
                        
                        // Обновляем карточки после закрытия формы (если статус изменился)
                        if (detailsForm.DialogResult == DialogResult.OK)
                        {
                            LoadApplicationsCards();
                        }
                    });
                    
                    cardsFlowPanel.Controls.Add(card);
                }
            }
            catch (Exception)
            {
                // Не показываем ошибку пользователю
                Label lblError = new Label
                {
                    Text = "Не удалось загрузить заявления",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = ModernUIHelper.TextSecondary,
                    Size = new Size(1100, 100),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                cardsFlowPanel.Controls.Add(lblError);
            }
        }

        private void SubmitApplication()
        {
            if (dgvSpecialties.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите специальность!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var specialty = (BookCategory)dgvSpecialties.SelectedRows[0].DataBoundItem;
                BookRequestForm form = new BookRequestForm(currentUser, specialty);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadApplicationsCards();
                    ShowApplicationsPanel();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось открыть форму подачи заявления", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteApplication()
        {
            // Находим выбранную карточку
            Panel selectedCard = null;
            foreach (Control control in cardsFlowPanel.Controls)
            {
                if (control is Panel panel && panel.BackColor == ColorTranslator.FromHtml("#21254d"))
                {
                    selectedCard = panel;
                    break;
                }
            }

            if (selectedCard == null)
            {
                MessageBox.Show("Выберите заявление для удаления (кликните на карточку)!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var application = (BookRequest)selectedCard.Tag;

            if (MessageBox.Show("Удалить выбранное заявление?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteBookRequest(application.Id);
                    LoadApplicationsCards();
                    MessageBox.Show("Заявление успешно удалено!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось удалить заявление", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
