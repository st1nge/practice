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
    public partial class AdminPanel : Form
    {
        private User currentUser;
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Panel applicationsPanel;
        private Panel specialtiesPanel;
        private Panel usersPanel;
        private FlowLayoutPanel appsCardsPanel;
        private DataGridView dgvUsers;
        private DataGridView dgvSpecialties;
        private Button btnApplicationsNav;
        private Button btnSpecialtiesNav;
        private Button btnUsersNav;
    private Label lblPageTitle;
    private Label lblUserName;
        private Panel selectedApplicationCard;

        public AdminPanel(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadData();
            ShowApplicationsPanel();
        }

        

        private void InitializeComponent()
        {
            this.Size = new Size(1500, 900);
            this.Text = "Панель администратора";
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

            // Имя пользователя справа — тоже сохраняем в поле для вычисления границ
            lblUserName = new Label
            {
                Text = currentUser.FullName,
                Font = new Font("Segoe UI", 10),
                ForeColor = ModernUIHelper.TextSecondary,
                Size = new Size(260, 24),
                Location = new Point(1200, 38),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };



            // Навигационные кнопки (расположены по центру топбара)
            Button btnApps = new Button
            {
                Text = "Запросы на книги",
                Location = new Point(520, 25),
                Size = new Size(160, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextPrimary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnApps.FlatAppearance.BorderSize = 0;
            btnApps.Click += (s, e) => ShowApplicationsPanel();
            // assign to field so other methods can reference the nav button
            btnApplicationsNav = btnApps;

            Button btnCats = new Button
            {
                Text = "Категории книг",
                Location = new Point(690, 25),
                Size = new Size(140, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnCats.FlatAppearance.BorderSize = 0;
            btnCats.Click += (s, e) => ShowSpecialtiesPanel();
            btnSpecialtiesNav = btnCats;

            Button btnUsers = new Button
            {
                Text = "Пользователи",
                Location = new Point(845, 25),
                Size = new Size(120, 48),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                ForeColor = ModernUIHelper.TextSecondary,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnUsers.FlatAppearance.BorderSize = 0;
            btnUsers.Click += (s, e) => ShowUsersPanel();
            btnUsersNav = btnUsers;

            // Кнопка выхода (справа)
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
            sidebarPanel.Controls.Add(btnApps);
            sidebarPanel.Controls.Add(btnCats);
            sidebarPanel.Controls.Add(btnUsers);
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
                Text = "УПРАВЛЕНИЕ ЗАПРОСАМИ НА КНИГИ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = ModernUIHelper.TextPrimary,
                Size = new Size(1200, 60),
                Location = new Point(40, 30),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(lblPageTitle);

            // Создаем панели для разных разделов
            CreateApplicationsPanel();
            CreateSpecialtiesPanel();
            CreateUsersPanel();

            this.Controls.Add(sidebarPanel);
            this.Controls.Add(contentPanel);
        }

        private void CreateApplicationsPanel()
        {
            applicationsPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(1160, 750),
                BackColor = Color.Transparent,
                Visible = true
            };

            // Заголовок панели заявлений
            Label lblAppsTitle = new Label
            {
                Text = "ВСЕ ЗАЯВКИ НА КНИГИ",
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
                Size = new Size(1140, 500),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // FlowLayoutPanel для автоматического расположения карточек
            appsCardsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(1120, 500),
                BackColor = Color.Transparent,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };

            scrollPanel.Controls.Add(appsCardsPanel);

            // Панель с кнопками действий
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, 570),
                Size = new Size(1160, 80),
                BackColor = Color.Transparent
            };

            Button btnApprove = ModernUIHelper.CreateGradientButton(
                "ОДОБРИТЬ",
                new Point(0, 10),
                new Size(200, 50),
                ModernUIHelper.SuccessColor,
                ColorTranslator.FromHtml("#00a67d")
            );
            btnApprove.Click += (s, e) => ChangeApplicationStatus("Одобрено");

            Button btnReject = ModernUIHelper.CreateGradientButton(
                "ОТКЛОНИТЬ",
                new Point(210, 10),
                new Size(200, 50),
                ModernUIHelper.DangerColor,
                ColorTranslator.FromHtml("#e66565")
            );
            btnReject.Click += (s, e) => ChangeApplicationStatus("Отклонено");

            Button btnDelete = ModernUIHelper.CreateGradientButton(
                "УДАЛИТЬ",
                new Point(420, 10),
                new Size(200, 50),
                ColorTranslator.FromHtml("#636e72"),
                ColorTranslator.FromHtml("#535c62")
            );
            btnDelete.Click += (s, e) => DeleteApplication();

            Button btnRefresh = ModernUIHelper.CreateGradientButton(
                "ОБНОВИТЬ",
                new Point(630, 10),
                new Size(200, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRefresh.Click += (s, e) => LoadApplicationsCards();

            // Фильтры
            ComboBox cmbFilter = new ComboBox
            {
                Location = new Point(840, 15),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbFilter.Items.AddRange(new object[] { "Все", "На рассмотрении", "Одобрено", "Отклонено" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadApplicationsCards();

            buttonPanel.Controls.Add(btnApprove);
            buttonPanel.Controls.Add(btnReject);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(cmbFilter);

            applicationsPanel.Controls.Add(lblAppsTitle);
            applicationsPanel.Controls.Add(scrollPanel);
            applicationsPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(applicationsPanel);
        }

        private void CreateSpecialtiesPanel()
        {
            specialtiesPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(1160, 750),
                BackColor = Color.Transparent,
                Visible = false
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

            Button btnAdd = ModernUIHelper.CreateGradientButton(
                "ДОБАВИТЬ",
                new Point(0, 10),
                new Size(220, 50),
                ModernUIHelper.SuccessColor,
                ColorTranslator.FromHtml("#00a67d")
            );
            btnAdd.Click += (s, e) => AddSpecialty();

            Button btnEdit = ModernUIHelper.CreateGradientButton(
                "ИЗМЕНИТЬ",
                new Point(240, 10),
                new Size(220, 50),
                ModernUIHelper.WarningColor,
                ColorTranslator.FromHtml("#f4c05e")
            );
            btnEdit.Click += (s, e) => EditSpecialty();

            Button btnDelete = ModernUIHelper.CreateGradientButton(
                "УДАЛИТЬ",
                new Point(480, 10),
                new Size(220, 50),
                ModernUIHelper.DangerColor,
                ColorTranslator.FromHtml("#e66565")
            );
            btnDelete.Click += (s, e) => DeleteSpecialty();

            Button btnRefresh = ModernUIHelper.CreateGradientButton(
                "ОБНОВИТЬ",
                new Point(720, 10),
                new Size(220, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRefresh.Click += (s, e) => LoadSpecialties();

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefresh);

            specialtiesPanel.Controls.Add(dgvSpecialties);
            specialtiesPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(specialtiesPanel);
        }

        private void CreateUsersPanel()
        {
            usersPanel = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(1160, 750),
                BackColor = Color.Transparent,
                Visible = false
            };

            // DataGridView для пользователей
            dgvUsers = new DataGridView
            {
                Location = new Point(0, 70),
                Size = new Size(1160, 550),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            ModernUIHelper.StyleDataGridView(dgvUsers);

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
            btnDelete.Click += (s, e) => DeleteUser();

            Button btnRefresh = ModernUIHelper.CreateGradientButton(
                "ОБНОВИТЬ",
                new Point(240, 10),
                new Size(220, 50),
                ModernUIHelper.SecondaryAccent,
                ColorTranslator.FromHtml("#00b5ad")
            );
            btnRefresh.Click += (s, e) => LoadUsers();

            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnRefresh);

            usersPanel.Controls.Add(dgvUsers);
            usersPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(usersPanel);
        }

        private void ShowApplicationsPanel()
        {
            applicationsPanel.Visible = true;
            specialtiesPanel.Visible = false;
            usersPanel.Visible = false;

            btnApplicationsNav.BackColor = ModernUIHelper.PrimaryAccent;
            btnApplicationsNav.ForeColor = ModernUIHelper.TextPrimary;
            btnSpecialtiesNav.BackColor = Color.Transparent;
            btnSpecialtiesNav.ForeColor = ModernUIHelper.TextSecondary;
            btnUsersNav.BackColor = Color.Transparent;
            btnUsersNav.ForeColor = ModernUIHelper.TextSecondary;

            lblPageTitle.Text = "УПРАВЛЕНИЕ ЗАЯВКАМИ НА КНИГИ";
            
            // Обновляем карточки при переходе на вкладку
            LoadApplicationsCards();
        }

        private void ShowSpecialtiesPanel()
        {
            applicationsPanel.Visible = false;
            specialtiesPanel.Visible = true;
            usersPanel.Visible = false;

            btnApplicationsNav.BackColor = Color.Transparent;
            btnApplicationsNav.ForeColor = ModernUIHelper.TextSecondary;
            btnSpecialtiesNav.BackColor = ModernUIHelper.PrimaryAccent;
            btnSpecialtiesNav.ForeColor = ModernUIHelper.TextPrimary;
            btnUsersNav.BackColor = Color.Transparent;
            btnUsersNav.ForeColor = ModernUIHelper.TextSecondary;

            lblPageTitle.Text = "УПРАВЛЕНИЕ СПЕЦИАЛЬНОСТЯМИ";
        }

        private void ShowUsersPanel()
        {
            applicationsPanel.Visible = false;
            specialtiesPanel.Visible = false;
            usersPanel.Visible = true;

            btnApplicationsNav.BackColor = Color.Transparent;
            btnApplicationsNav.ForeColor = ModernUIHelper.TextSecondary;
            btnSpecialtiesNav.BackColor = Color.Transparent;
            btnSpecialtiesNav.ForeColor = ModernUIHelper.TextSecondary;
            btnUsersNav.BackColor = ModernUIHelper.PrimaryAccent;
            btnUsersNav.ForeColor = ModernUIHelper.TextPrimary;

            lblPageTitle.Text = "УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ";
        }

        private void LoadData()
        {
            try
            {
                LoadSpecialties();
                LoadUsers();
            }
            catch (Exception)
            {
                // Не показываем ошибку пользователю
                // Можно добавить логирование если нужно
            }
        }

        private void LoadApplicationsCards()
        {
            // Очищаем старые карточки
            appsCardsPanel.Controls.Clear();
            selectedApplicationCard = null;

            try
            {
                // Получаем все заявления
                List<BookRequest> bookRequests = DatabaseHelper.GetAllBookRequests();

                if (bookRequests.Count == 0)
                {
                    // Сообщение если нет заявлений
                    Label lblNoApps = new Label
                    {
                        Text = "Заявлений пока нет.",
                        Font = new Font("Segoe UI", 12),
                        ForeColor = ModernUIHelper.TextSecondary,
                        Size = new Size(1100, 100),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    appsCardsPanel.Controls.Add(lblNoApps);
                    return;
                }

                // Создаем карточки для каждого заявления
                foreach (var app in bookRequests)
                {
                    // Создаем локальную копию для использования в лямбда-выражении
                    var currentApp = app;
                    
                    Panel card = ModernUIHelper.CreateApplicationCard(currentApp, (s, e) =>
                    {
                        var clickedCard = (Panel)s;
                        
                        // Снимаем выделение с предыдущей карточки
                        if (selectedApplicationCard != null && selectedApplicationCard != clickedCard)
                        {
                            selectedApplicationCard.BackColor = ModernUIHelper.CardBackground;
                            selectedApplicationCard.Refresh();
                        }

                        // Выделяем текущую карточку
                        clickedCard.BackColor = ColorTranslator.FromHtml("#21254d");
                        clickedCard.Refresh();
                        selectedApplicationCard = clickedCard;

                        // При клике открываем детали (без двойного клика)
                        ApplicationDetailsForm detailsForm = new ApplicationDetailsForm(currentApp, true);
                        detailsForm.ShowDialog();
                        
                        // Обновляем карточки после закрытия формы
                        if (detailsForm.DialogResult == DialogResult.OK)
                        {
                            LoadApplicationsCards();
                        }
                    });
                    
                    appsCardsPanel.Controls.Add(card);
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
                appsCardsPanel.Controls.Add(lblError);
            }
        }

        private void LoadSpecialties()
        {
            try
            {
                List<BookCategory> categories = DatabaseHelper.GetAllBookCategories();
                
                // Проверяем, что DataGridView инициализирован
                if (dgvSpecialties == null) return;
                
                dgvSpecialties.DataSource = null;
                dgvSpecialties.DataSource = categories;

                // Проверяем наличие столбцов перед доступом к ним
                if (dgvSpecialties.Columns.Count > 0)
                {
                    if (dgvSpecialties.Columns.Contains("Id"))
                    {
                        var idColumn = dgvSpecialties.Columns["Id"];
                        if (idColumn != null)
                        {
                            idColumn.HeaderText = "ID";
                        }
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

        private void LoadUsers()
        {
            try
            {
                List<User> users = DatabaseHelper.GetAllUsers();
                
                // Проверяем, что DataGridView инициализирован
                if (dgvUsers == null) return;
                
                dgvUsers.DataSource = null;
                dgvUsers.DataSource = users;

                // Проверяем наличие столбцов перед доступом к ним
                if (dgvUsers.Columns.Count > 0)
                {
                    // Используем проверку индексов для безопасного доступа
                    if (dgvUsers.Columns.Contains("Id"))
                    {
                        var idColumn = dgvUsers.Columns["Id"];
                        if (idColumn != null)
                        {
                            idColumn.HeaderText = "ID";
                        }
                    }
                    
                    if (dgvUsers.Columns.Contains("Login"))
                    {
                        var loginColumn = dgvUsers.Columns["Login"];
                        if (loginColumn != null)
                            loginColumn.HeaderText = "Логин";
                    }
                        
                    if (dgvUsers.Columns.Contains("Password"))
                    {
                        var passColumn = dgvUsers.Columns["Password"];
                        if (passColumn != null)
                            passColumn.Visible = false;
                    }
                        
                    if (dgvUsers.Columns.Contains("FullName"))
                    {
                        var nameColumn = dgvUsers.Columns["FullName"];
                        if (nameColumn != null)
                            nameColumn.HeaderText = "ФИО";
                    }
                        
                    if (dgvUsers.Columns.Contains("Role"))
                    {
                        var roleColumn = dgvUsers.Columns["Role"];
                        if (roleColumn != null)
                            roleColumn.HeaderText = "Роль";
                    }
                }
            }
            catch (Exception)
            {
                // Не показываем ошибку пользователю
                if (dgvUsers != null)
                {
                    dgvUsers.DataSource = null;
                }
            }
        }

        private void ChangeApplicationStatus(string status)
        {
            if (selectedApplicationCard == null)
            {
                MessageBox.Show("Выберите заявление (кликните на карточку)!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var application = (BookRequest)selectedApplicationCard.Tag;

            if (MessageBox.Show($"Изменить статус заявления на '{status}'?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.UpdateBookRequestStatus(application.Id, status);
                    LoadApplicationsCards();
                    MessageBox.Show("Статус успешно изменен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось изменить статус заявления", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteApplication()
        {
            if (selectedApplicationCard == null)
            {
                MessageBox.Show("Выберите заявление для удаления (кликните на карточку)!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var application = (BookRequest)selectedApplicationCard.Tag;

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

        private void AddSpecialty()
        {
            try
            {
                SpecialtyEditForm form = new SpecialtyEditForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadSpecialties();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось открыть форму добавления специальности", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSpecialty()
        {
            if (dgvSpecialties.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите специальность для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var specialty = (BookCategory)dgvSpecialties.SelectedRows[0].DataBoundItem;
                SpecialtyEditForm form = new SpecialtyEditForm(specialty);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadSpecialties();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось открыть форму редактирования специальности", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSpecialty()
        {
            if (dgvSpecialties.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите специальность для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var specialty = (BookCategory)dgvSpecialties.SelectedRows[0].DataBoundItem;

                if (MessageBox.Show($"Удалить специальность '{specialty.Name}'?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteBookCategory(specialty.Id);
                    LoadSpecialties();
                    MessageBox.Show("Специальность успешно удалена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить специальность", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var user = (User)dgvUsers.SelectedRows[0].DataBoundItem;

                if (user.Role == "Admin")
                {
                    MessageBox.Show("Невозможно удалить администратора!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя '{user.FullName}'?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteUser(user.Id);
                    LoadUsers();
                    MessageBox.Show("Пользователь успешно удален!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить пользователя", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
