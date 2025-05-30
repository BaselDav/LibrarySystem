using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibrarySystem.BusinessLogic.Managers;
using LibrarySystem.BusinessLogic.Validation;
using LibrarySystem.DataAccess.Helpers;
using LibrarySystem.Domain;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public partial class SearchForm : BaseForm
    {
        #region Fields and Constants
        private readonly ILibraryManager libraryManager;
        private const int PageSize = 20; // Page Size  from settings /////////////////////////////////////////////////////
        private int currentAllBooksPage = 1;
        private int currentBorrowReturnPage = 1;
        private int currentHistoryPage = 1;
        private int totalAllBooksPages = 1;
        private int totalBorrowReturnPages = 1;
        private int totalHistoryPages = 1;
        private string currentHistorySearchKeyword = "";
        private string currentBorrowSearchKeyword = "";
        private int currentUsersPage = 1;
        private int totalUsersPages = 1;
        private string currentUserSearchKeyword = "";
        #endregion

        #region Constructor and Initialization
        public SearchForm(ILibraryManager libraryManager)
        {
            InitializeComponent();
            this.libraryManager = libraryManager;

            ApplyLocalization();
            ApplyStyleToAllControls(this.Controls);
            RegisterEvents();

            cmbFilterBooks.Height = 40;
            cmbFilterBooks.DropDownStyle = ComboBoxStyle.DropDownList;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            //await HandleErrorsAsync(() => LoadUsersIntoComboBoxAsync());
            tabControl1.SelectedTab = tpBorrowReturn;
            await HandleErrorsAsync(() => LoadAllBooksToSearchTabAsync(currentBorrowReturnPage));
            await HandleErrorsAsync(() => LoadBorrowReturnBooksAsync(currentBorrowReturnPage));
            LoadBookFilterOptions();

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            dgvAllBooks.ClearSelection();
            dgvBorrowReturn.ClearSelection();
            dgvArchive.ClearSelection();
            ApplyPlaceholders();
            const int EM_SETMARGINS = 0xd3;
            const int EC_LEFTMARGIN = 0x0001;
            const int EC_RIGHTMARGIN = 0x0002;

            SendMessage(txtSearch.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(5 + (5 << 16)));
            SendMessage(txtBookSearch.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(5 + (5 << 16)));
            SendMessage(txtHistorySearch.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(5 + (5 << 16)));
            SendMessage(txtUserSearch.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(5 + (5 << 16)));


        }
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        #endregion

        #region Localization & Placeholders
        private void ApplyLocalization()
        {
            
            btnAddBook.Text = Labels.AddBook;
            tpBorrowReturn.Text = Labels.BorrowReturnTabTitle;
            tpHistory.Text = Labels.HistoryTabTitle;
            tpManageBooks.Text = Labels.BookTabTitle;
        }
        private void ApplyPlaceholders()
        {
            SetPlaceholder(txtSearch, Labels.SearchPlaceholder);
            SetPlaceholder(txtBookSearch, Labels.SearchPlaceholder);
            SetPlaceholder(txtHistorySearch, Labels.SearchPlaceholder);
            SetPlaceholder(txtUserSearch, Labels.SearchPlaceholder);
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray;
            }

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }
        #endregion

        #region Register Events
        private void RegisterEvents()
        {
            // -------- Tab Selection --------
            tabControl1.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl1.SelectedTab == tpHistory)
                {
                    await HandleErrorsAsync(() => LoadBorrowingHistoryAsync(currentHistoryPage));
                }
                else if (tabControl1.SelectedTab == tpManageBooks)
                {
                    await HandleErrorsAsync(() => LoadAllBooksAsync());
                }
                else if (tabControl1.SelectedTab == tpBorrowReturn)
                {
                    await HandleErrorsAsync(() => LoadAllBooksToSearchTabAsync());
                    await HandleErrorsAsync(() => LoadBorrowReturnBooksAsync(currentBorrowReturnPage));
                }
                else if (tabControl1.SelectedTab == tpUsers)
                {
                    await HandleErrorsAsync(() => LoadUsersPageAsync());
                }
            };

            // -------- Filter Dropdown --------
            cmbFilterBooks.SelectedIndexChanged += async (s, e) =>
            {
                currentBorrowReturnPage = 1;
                await HandleErrorsAsync(() => LoadBorrowReturnBooksAsync(currentBorrowReturnPage));
            };

            // -------- Add Buttons --------
            btnAddBook.Click += async (s, e) => await HandleErrorsAsync(() => AddBookAsync());
            btnAddNewUser.Click += async (s, e) => await HandleErrorsAsync(() => AddUserAsync());

            // -------- Borrow Tab: Search & Pagination --------
            txtSearch.TextChanged += async (s, e) =>
            {
                currentBorrowReturnPage = 1;
                await HandleErrorsAsync(() => SearchBooksAsync());
            };
            btnNextBorrow.Click += async (s, e) =>
            {
                if (currentBorrowReturnPage < totalBorrowReturnPages)
                {
                    currentBorrowReturnPage++;
                    if (!string.IsNullOrWhiteSpace(currentBorrowSearchKeyword))
                        await HandleErrorsAsync(() => LoadSearchBorrowReturnBooksAsync());
                    else
                        await HandleErrorsAsync(() => LoadAllBooksToSearchTabAsync(currentBorrowReturnPage));
                }
            };
            btnPrevBorrow.Click += async (s, e) =>
            {
                if (currentBorrowReturnPage > 1)
                {
                    currentBorrowReturnPage--;
                    if (!string.IsNullOrWhiteSpace(currentBorrowSearchKeyword))
                        await HandleErrorsAsync(() => LoadSearchBorrowReturnBooksAsync());
                    else
                        await HandleErrorsAsync(() => LoadAllBooksToSearchTabAsync(currentBorrowReturnPage));
                }
            };

            // -------- Manage Books Tab: Search & Pagination --------
            txtBookSearch.TextChanged += async (s, e) =>
            {
                currentAllBooksPage = 1;
                await HandleErrorsAsync(() => LoadAllBooksAsync(txtBookSearch.Text.Trim(), currentAllBooksPage));
            };
            btnNextAllBooks.Click += async (s, e) =>
            {
                if (currentAllBooksPage < totalAllBooksPages)
                {
                    currentAllBooksPage++;
                    await HandleErrorsAsync(() => LoadAllBooksAsync(txtBookSearch.Text.Trim(), currentAllBooksPage));
                }
            };
            btnPrevAllBooks.Click += async (s, e) =>
            {
                if (currentAllBooksPage > 1)
                {
                    currentAllBooksPage--;
                    await HandleErrorsAsync(() => LoadAllBooksAsync(txtBookSearch.Text.Trim(), currentAllBooksPage));
                }
            };

            // -------- History Tab: Search & Pagination --------
            txtHistorySearch.TextChanged += async (s, e) => await HandleErrorsAsync(() => SearchHistoryAsync());
            btnNextHistory.Click += async (s, e) =>
            {
                if (currentHistoryPage < totalHistoryPages)
                {
                    currentHistoryPage++;
                    if (!string.IsNullOrWhiteSpace(currentHistorySearchKeyword))
                        await HandleErrorsAsync(() => LoadSearchHistoryAsync());
                    else
                        await HandleErrorsAsync(() => LoadBorrowingHistoryAsync(currentHistoryPage));
                }
            };
            btnPrevHistory.Click += async (s, e) =>
            {
                if (currentHistoryPage > 1)
                {
                    currentHistoryPage--;
                    if (!string.IsNullOrWhiteSpace(currentHistorySearchKeyword))
                        await HandleErrorsAsync(() => LoadSearchHistoryAsync());
                    else
                        await HandleErrorsAsync(() => LoadBorrowingHistoryAsync(currentHistoryPage));
                }
            };

            // -------- Users Tab: Search & Pagination --------
            txtUserSearch.TextChanged += async (s, e) =>
            {
                currentUserSearchKeyword = txtUserSearch.Text.Trim();
                currentUsersPage = 1;
                await HandleErrorsAsync(() => LoadUsersPageAsync());
            };
            btnNextUsers.Click += async (s, e) =>
            {
                if (currentUsersPage < totalUsersPages)
                {
                    currentUsersPage++;
                    await HandleErrorsAsync(() => LoadUsersPageAsync());
                }
            };
            btnPrevUsers.Click += async (s, e) =>
            {
                if (currentUsersPage > 1)
                {
                    currentUsersPage--;
                    await HandleErrorsAsync(() => LoadUsersPageAsync());
                }
            };

            dgvUsers.CellContentClick += async (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (dgvUsers.Columns[e.ColumnIndex].Name == "Edit")
                {
                    var row = dgvUsers.Rows[e.RowIndex];
                    int userId = (int)row.Cells["UserId"].Value;
                    var user = await libraryManager.GetUserByIdAsync(userId);

                    var form = new AddUserForm(user);
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!UserValidator.Validate(form.FullName, form.Email, out string error))
                        {
                            MessageHelper.ShowInfo(error);
                            return;
                        }

                        var (success, err) = await libraryManager.UpdateUserAsync(userId, form.FullName, form.Email);
                        if (!success)
                        {
                            MessageHelper.ShowInfo(err);
                            return;
                        }

                        MessageHelper.ShowInfo(Messages.UserUpdatedSuccess);
                        await LoadUsersPageAsync();
                    }
                }
            };

            // -------- Grid Events --------
            dgvAllBooks.CellContentClick += async (s, e) =>
                await HandleErrorsAsync(() => dgvAllBooks_CellContentClickAsync(e));

            dgvBorrowReturn.CellContentClick += async (s, e) =>
                await HandleErrorsAsync(() => dgvBorrowReturn_CellContentClickAsync(e));

            dgvBorrowReturn.CellFormatting += dgvBorrowReturn_CellFormatting;
            dgvArchive.CellFormatting += dgvArchive_CellFormatting;

            // -------- Row Header Width --------
            dgvAllBooks.RowHeadersWidth = 30;
            dgvBorrowReturn.RowHeadersWidth = 30;
            dgvArchive.RowHeadersWidth = 30;
        }

        #endregion

        #region Books (Manage Books Tab)
        private async Task LoadAllBooksAsync(string keyword = "", int page = 1)
        {
            List<Book> books;
            if (string.IsNullOrWhiteSpace(keyword) || keyword == Labels.SearchPlaceholder)
            {
                int totalBooks = await libraryManager.GetBooksCountAsync();
                totalAllBooksPages = (int)Math.Ceiling((double)totalBooks / PageSize);
                books = await libraryManager.GetBooksAsync(page, PageSize);
            }
            else
            {
                int totalBooks = await libraryManager.GetSearchBooksCountAsync(keyword);
                totalAllBooksPages = (int)Math.Ceiling((double)totalBooks / PageSize);
                books = await libraryManager.SearchBooksAsync(keyword, page, PageSize);
            }

            dgvAllBooks.Columns.Clear();
            dgvAllBooks.DataSource = books;

            if (dgvAllBooks.Columns.Contains("BookId"))
                dgvAllBooks.Columns["BookId"].HeaderText = Labels.BookId;

            if (dgvAllBooks.Columns.Contains("Title"))
                dgvAllBooks.Columns["Title"].HeaderText = Labels.Title;

            if (dgvAllBooks.Columns.Contains("Author"))
                dgvAllBooks.Columns["Author"].HeaderText = Labels.Author;

            if (dgvAllBooks.Columns.Contains("ISBN"))
                dgvAllBooks.Columns["ISBN"].HeaderText = Labels.ISBN;

            if (dgvAllBooks.Columns.Contains("IsAvailable"))
                dgvAllBooks.Columns["IsAvailable"].HeaderText = Labels.IsAvailable;

            var editCol = new DataGridViewButtonColumn
            {
                HeaderText = Labels.Edit,
                Text = Labels.Edit,
                Name = "Edit",
                UseColumnTextForButtonValue = true
            };
            dgvAllBooks.Columns.Add(editCol);

            var deleteCol = new DataGridViewButtonColumn
            {
                HeaderText = Labels.Delete,
                Text = Labels.Delete,
                Name = "Delete",
                UseColumnTextForButtonValue = true
            };
            dgvAllBooks.Columns.Add(deleteCol);

            FormatDataGridViewHeadersAndAlignment(dgvAllBooks);
            lblPageAllBooks.Text = $"Page {currentAllBooksPage} of {totalAllBooksPages}";
            btnPrevAllBooks.Enabled = currentAllBooksPage > 1;
            btnNextAllBooks.Enabled = currentAllBooksPage < totalAllBooksPages;
            panelPaginationAllBooks.Visible = totalAllBooksPages > 1;
            dgvAllBooks.ClearSelection();
        }
        private async Task dgvAllBooks_CellContentClickAsync(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvAllBooks.Rows[e.RowIndex];
            int bookId = (int)row.Cells["BookId"].Value;

            if (dgvAllBooks.Columns[e.ColumnIndex].Name == "Delete")
            {
                var confirm = MessageHelper.ShowConfirm(Messages.ConfirmDeleteBook, Messages.ConfirmTitle);
                if (confirm == DialogResult.Yes)
                {
                    await libraryManager.DeleteBookAsync(bookId);
                    await LoadAllBooksAsync();
                }
            }
            else if (dgvAllBooks.Columns[e.ColumnIndex].Name == "Edit")
            {
                var book = await libraryManager.GetBookByIdAsync(bookId);
                var form = new AddBookForm(book);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    await libraryManager.UpdateBookAsync(bookId, form.BookTitle, form.Author, form.ISBN);
                    await LoadAllBooksAsync();
                }
            }
        }
        private async Task AddBookAsync()
        {
            var form = new AddBookForm();

            while (true)
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;


                if (!BookValidator.Validate(form.BookTitle, form.Author, form.ISBN, out string error))
                {
                    MessageHelper.ShowInfo(error);
                    continue;
                }

                var (success, err) = await libraryManager.AddBookAsync(form.BookTitle, form.Author, form.ISBN);
                if (!success)
                {
                    MessageHelper.ShowInfo(err);
                    continue;
                }

                MessageHelper.ShowInfo(Messages.BookAddedSuccess);
                await LoadAllBooksAsync();
                break;
            }
        }
        #endregion

        #region Borrow and Return (BorrowReturn Tab)
        private async Task dgvBorrowReturn_CellContentClickAsync(DataGridViewCellEventArgs e)
        {
            if (dgvBorrowReturn.Columns[e.ColumnIndex].Name != "Action" || e.RowIndex < 0)
                return;

            int bookId = (int)dgvBorrowReturn.Rows[e.RowIndex].Cells["BookId"].Value;
            bool isAvailable = (bool)dgvBorrowReturn.Rows[e.RowIndex].Cells["IsAvailable"].Value;
            string bookTitle = dgvBorrowReturn.Rows[e.RowIndex].Cells["Title"].Value.ToString();

            if (isAvailable)
            {
                var userDialog = new UserSelectionDialog(libraryManager);
                if (userDialog.ShowDialog(this) != DialogResult.OK || userDialog.SelectedUser == null)
                {
                    MessageHelper.ShowInfo(Messages.SelectUserFirst);
                    return;
                }

                var user = userDialog.SelectedUser;
                var confirm = MessageHelper.ShowConfirm(Messages.ConfirmBorrow(user.FullName, bookTitle));
                if (confirm != DialogResult.Yes)
                    return;

                bool result = await libraryManager.BorrowBookAsync(user.UserId, bookId);
                MessageHelper.ShowInfo(result ? Messages.OperationSuccess : Messages.OperationFailed);
            }
            else
            {
                var borrower = await libraryManager.GetBookCurrentBorrowerAsync(bookId);
                if (borrower == null)
                {
                    MessageHelper.ShowInfo("This book is not currently borrowed by any user.");
                    return;
                }

                var message = $"The book \"{bookTitle}\" is currently borrowed by {borrower.FullName}. Would you like to return it now?";
                var confirm = MessageBox.Show(message, "Return Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                    return;

                bool result = await libraryManager.ReturnBookAsync(borrower.UserId, bookId);
                MessageHelper.ShowInfo(result ? Messages.OperationSuccess : Messages.OperationFailed);
            }

            await LoadBorrowReturnBooksAsync(currentBorrowReturnPage);
        }
        private async Task LoadBorrowReturnBooksAsync(int page = 1)
        {
            string selectedFilter = (cmbFilterBooks.SelectedValue ?? "all").ToString();

            List<Book> allBooks = await libraryManager.GetBooksAsync(page, PageSize);
            List<Book> books = new List<Book>();
            int totalBooks = 0;

            if (selectedFilter == "available")
            {
                books = allBooks.Where(b => b.IsAvailable).ToList();
                totalBooks = books.Count;
            }
            else if (selectedFilter == "borrowed")
            {
                books = allBooks.Where(b => !b.IsAvailable).ToList();
                totalBooks = books.Count;
            }
            else
            {
                books = allBooks;
                totalBooks = await libraryManager.GetBooksCountAsync();
            }

            totalBorrowReturnPages = (int)Math.Ceiling((double)totalBooks / PageSize);

            BindBooksToBorrowGrid(books);

            lblPageBorrow.Text = $"Page {currentBorrowReturnPage} of {totalBorrowReturnPages}";
            btnPrevBorrow.Enabled = currentBorrowReturnPage > 1;
            btnNextBorrow.Enabled = currentBorrowReturnPage < totalBorrowReturnPages;
            panelPaginationBorrow.Visible = totalBorrowReturnPages > 1;
        }
        private async Task LoadAllBooksToSearchTabAsync(int page = 1)
        {
            int totalBooks = await libraryManager.GetBooksCountAsync();
            totalBorrowReturnPages = (int)Math.Ceiling((double)totalBooks / PageSize);

            var books = await libraryManager.GetBooksAsync(page, PageSize);
            BindBooksToBorrowGrid(books);

            lblPageBorrow.Text = $"Page {currentBorrowReturnPage} of {totalBorrowReturnPages}";
            btnPrevBorrow.Enabled = currentBorrowReturnPage > 1;
            btnNextBorrow.Enabled = currentBorrowReturnPage < totalBorrowReturnPages;
            panelPaginationBorrow.Visible = totalBorrowReturnPages > 1;
        }
        private void BindBooksToBorrowGrid(List<Book> books)
        {
            dgvBorrowReturn.Columns.Clear();
            dgvBorrowReturn.DataSource = books;

            dgvBorrowReturn.Columns["BookId"].HeaderText = Labels.BookId;
            dgvBorrowReturn.Columns["Title"].HeaderText = Labels.Title;
            dgvBorrowReturn.Columns["Author"].HeaderText = Labels.Author;
            dgvBorrowReturn.Columns["ISBN"].HeaderText = Labels.ISBN;
            dgvBorrowReturn.Columns["IsAvailable"].HeaderText = Labels.IsAvailable;

            FormatDataGridViewHeadersAndAlignment(dgvBorrowReturn);

            if (!dgvBorrowReturn.Columns.Contains("Action"))
            {
                var actionCol = new DataGridViewButtonColumn
                {
                    HeaderText = Labels.Action,
                    Name = "Action",
                    UseColumnTextForButtonValue = false,
                    DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
                };

                actionCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvBorrowReturn.Columns.Add(actionCol);
            }

            foreach (DataGridViewRow row in dgvBorrowReturn.Rows)
            {
                if (row.IsNewRow) continue;

                bool isAvailable = (bool)row.Cells["IsAvailable"].Value;
                row.Cells["Action"].Value = isAvailable ? Labels.Borrow : Labels.Return;
                row.DefaultCellStyle.BackColor = isAvailable ? Color.White : Color.LightSalmon;
            }

            dgvBorrowReturn.ClearSelection();
        }
        private async Task SearchBooksAsync()
        {
            currentBorrowSearchKeyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(currentBorrowSearchKeyword) || currentBorrowSearchKeyword == Labels.SearchPlaceholder)
            {
                currentBorrowSearchKeyword = "";
            }

            currentBorrowReturnPage = 1;
            await LoadSearchBorrowReturnBooksAsync();
        }
        private async Task LoadSearchBorrowReturnBooksAsync()
        {
            int totalBooks = await libraryManager.GetSearchBooksCountAsync(currentBorrowSearchKeyword);
            totalBorrowReturnPages = (int)Math.Ceiling((double)totalBooks / PageSize);

            var books = await libraryManager.SearchBooksAsync(currentBorrowSearchKeyword, currentBorrowReturnPage, PageSize);
            BindBooksToBorrowGrid(books);

            lblPageBorrow.Text = $"Page {currentBorrowReturnPage} of {totalBorrowReturnPages}";
            btnPrevBorrow.Enabled = currentBorrowReturnPage > 1;
            btnNextBorrow.Enabled = currentBorrowReturnPage < totalBorrowReturnPages;
            panelPaginationBorrow.Visible = totalBorrowReturnPages > 1;
        }
        #endregion

        #region History (Archive Tab)
        private async Task SearchHistoryAsync()
        {
            currentHistorySearchKeyword = txtHistorySearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(currentHistorySearchKeyword) || currentHistorySearchKeyword == Labels.SearchPlaceholder)
                currentHistorySearchKeyword = "";

            currentHistoryPage = 1;
            await LoadSearchHistoryAsync();
        }
        private async Task LoadSearchHistoryAsync()
        {
            int total = await libraryManager.GetSearchBorrowingHistoryCountAsync(currentHistorySearchKeyword);
            totalHistoryPages = (int)Math.Ceiling((double)total / PageSize);

            var pagedHistory = await libraryManager.SearchBorrowingHistoryAsync(currentHistorySearchKeyword, currentHistoryPage, PageSize);

            dgvArchive.Columns.Clear();
            dgvArchive.DataSource = pagedHistory;

            if (dgvArchive.Columns.Contains("UserName"))
                dgvArchive.Columns["UserName"].HeaderText = Labels.UserName;

            if (dgvArchive.Columns.Contains("BookTitle"))
                dgvArchive.Columns["BookTitle"].HeaderText = Labels.Title;

            if (dgvArchive.Columns.Contains("BorrowDate"))
                dgvArchive.Columns["BorrowDate"].HeaderText = Labels.BorrowDate;

            if (dgvArchive.Columns.Contains("ReturnDate"))
                dgvArchive.Columns["ReturnDate"].HeaderText = Labels.ReturnDate;

            FormatDataGridViewHeadersAndAlignment(dgvArchive);
            lblPageHistory.Text = $"Page {currentHistoryPage} of {totalHistoryPages}";
            btnPrevHistory.Enabled = currentHistoryPage > 1;
            btnNextHistory.Enabled = currentHistoryPage < totalHistoryPages;
            panelPaginationHistory.Visible = totalHistoryPages > 1;
            dgvArchive.ClearSelection();
        }
        private async Task LoadBorrowingHistoryAsync(int page = 1)
        {
            int total = await libraryManager.GetBorrowingHistoryCountAsync();
            totalHistoryPages = (int)Math.Ceiling((double)total / PageSize);

            var pagedHistory = await libraryManager.GetBorrowingHistoryAsync(page, PageSize);

            dgvArchive.Columns.Clear();
            dgvArchive.DataSource = pagedHistory;

            if (dgvArchive.Columns.Contains("UserName"))
                dgvArchive.Columns["UserName"].HeaderText = Labels.UserName;

            if (dgvArchive.Columns.Contains("BookTitle"))
                dgvArchive.Columns["BookTitle"].HeaderText = Labels.Title;

            if (dgvArchive.Columns.Contains("BorrowDate"))
                dgvArchive.Columns["BorrowDate"].HeaderText = Labels.BorrowDate;

            if (dgvArchive.Columns.Contains("ReturnDate"))
                dgvArchive.Columns["ReturnDate"].HeaderText = Labels.ReturnDate;

            FormatDataGridViewHeadersAndAlignment(dgvArchive);
            lblPageHistory.Text = $"Page {currentHistoryPage} of {totalHistoryPages}";
            btnPrevHistory.Enabled = currentHistoryPage > 1;
            btnNextHistory.Enabled = currentHistoryPage < totalHistoryPages;
            panelPaginationHistory.Visible = totalHistoryPages > 1;
            dgvArchive.ClearSelection();
        }
        #endregion

        #region Users (Users Tab)
        private async Task LoadUsersPageAsync()
        {
            List<User> users;

            bool isSearch = !string.IsNullOrWhiteSpace(currentUserSearchKeyword)
                            && currentUserSearchKeyword != Labels.SearchPlaceholder;

            int total;

            if (isSearch)
            {
                total = await libraryManager.GetSearchUsersCountAsync(currentUserSearchKeyword);
                users = await libraryManager.SearchUsersAsync(currentUserSearchKeyword, currentUsersPage, PageSize);
            }
            else
            {
                total = await libraryManager.GetUsersCountAsync();
                users = await libraryManager.GetPagedUsersAsync(currentUsersPage, PageSize);
            }

            totalUsersPages = (int)Math.Ceiling((double)total / PageSize);

            BindUsersToGrid(users);
        }
        private void BindUsersToGrid(List<User> users)
        {
            dgvUsers.Columns.Clear();
            dgvUsers.AutoGenerateColumns = true;

            dgvUsers.DataSource = users;

            dgvUsers.Columns["UserId"].HeaderText = Labels.UserId;
            dgvUsers.Columns["FullName"].HeaderText = Labels.FullNameLabel;
            dgvUsers.Columns["Email"].HeaderText = Labels.EmailLabel;

            var editCol = new DataGridViewButtonColumn
            {
                HeaderText = Labels.Edit,
                Text = Labels.Edit,
                Name = "Edit",
                UseColumnTextForButtonValue = true
            };

            dgvUsers.Columns.Add(editCol);
            FormatDataGridViewHeadersAndAlignment(dgvUsers);

            dgvUsers.ClearSelection();
            lblPageUsers.Text = $"Page {currentUsersPage} of {totalUsersPages}";
            btnPrevUsers.Enabled = currentUsersPage > 1;
            btnNextUsers.Enabled = currentUsersPage < totalUsersPages;
            panelPaginationUsers.Visible = totalUsersPages > 1;
        }
        //private async Task LoadUsersAsync()
        //{
        //    int total;
        //    List<User> users;

        //    if (string.IsNullOrWhiteSpace(currentUserSearchKeyword) || currentUserSearchKeyword == Labels.SearchPlaceholder)
        //    {
        //        total = await libraryManager.GetUsersCountAsync();
        //        users = await libraryManager.GetPagedUsersAsync(currentUsersPage, PageSize);
        //    }
        //    else
        //    {
        //        total = await libraryManager.GetSearchUsersCountAsync(currentUserSearchKeyword);
        //        users = await libraryManager.SearchUsersAsync(currentUserSearchKeyword, currentUsersPage, PageSize);
        //    }

        //    totalUsersPages = (int)Math.Ceiling((double)total / PageSize);

        //    dgvUsers.Columns.Clear();
        //    dgvUsers.DataSource = users;

        //    dgvUsers.Columns["UserId"].HeaderText = Labels.UserId;
        //    dgvUsers.Columns["FullName"].HeaderText = Labels.FullNameLabel;
        //    dgvUsers.Columns["Email"].HeaderText = Labels.EmailLabel;

        //    var editCol = new DataGridViewButtonColumn
        //    {
        //        HeaderText = Labels.Edit,
        //        Text = Labels.Edit,
        //        Name = "Edit",
        //        UseColumnTextForButtonValue = true
        //    };
        //    dgvUsers.Columns.Add(editCol);

        //    FormatDataGridViewHeadersAndAlignment(dgvUsers);

        //    lblPageUsers.Text = $"Page {currentUsersPage} of {totalUsersPages}";
        //    btnPrevUsers.Enabled = currentUsersPage > 1;
        //    btnNextUsers.Enabled = currentUsersPage < totalUsersPages;
        //    dgvUsers.ClearSelection();
        //}
        private async Task AddUserAsync()
        {
            var form = new AddUserForm();
            while (true)
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                if (!UserValidator.Validate(form.FullName, form.Email, out string error))
                {
                    MessageHelper.ShowInfo(error);
                    continue;
                }

                var (success, err) = await libraryManager.AddUserAsync(form.FullName, form.Email);
                if (!success)
                {
                    MessageHelper.ShowInfo(err);
                    continue;
                }

                await LoadUsersPageAsync();
                MessageHelper.ShowInfo(Messages.UserAddedSuccess);
                break;
            }
        }
        //private async Task EditUserAsync(int userId)
        //{
        //    var user = await libraryManager.GetUserByIdAsync(userId);
        //    var form = new AddUserForm(user);

        //    while (true)
        //    {
        //        if (form.ShowDialog(this) != DialogResult.OK)
        //            return;

        //        if (!UserValidator.Validate(form.FullName, form.Email, out string error))
        //        {
        //            MessageHelper.ShowInfo(error);
        //            continue;
        //        }

        //        var (success, err) = await libraryManager.UpdateUserAsync(userId, form.FullName, form.Email);
        //        if (!success)
        //        {
        //            MessageHelper.ShowInfo(err);
        //            continue;
        //        }

        //        MessageHelper.ShowInfo(Messages.OperationSuccess);
        //        await LoadUsersAsync(); 
        //        break;
        //    }
        //}
        #endregion

        #region Helpers
        private void LoadBookFilterOptions()
        {
            var items = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("all", "All"),
                new KeyValuePair<string, string>("borrowed", "Borrowed"),
                new KeyValuePair<string, string>("available", "Available")
            };

            cmbFilterBooks.DataSource = items;
            cmbFilterBooks.DisplayMember = "Value";
            cmbFilterBooks.ValueMember = "Key";
        }
        public static async Task HandleErrorsAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (ApplicationException ex)
            {
                Logger.Log(ex);
                MessageHelper.ShowInfo(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                MessageHelper.ShowInfo(Messages.GeneralError);
            }
        }
        #endregion

        #region Event Formatting Handlers
        private void dgvBorrowReturn_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBorrowReturn.Columns[e.ColumnIndex].Name == "Action" && e.RowIndex >= 0)
            {
                var row = dgvBorrowReturn.Rows[e.RowIndex];
                if (row.Cells["IsAvailable"].Value is bool isAvailable)
                {
                    e.Value = isAvailable ? Labels.Borrow : Labels.Return;
                    row.DefaultCellStyle.BackColor = isAvailable ? Color.White : Color.LightSalmon;
                }
            }
        }
        private void dgvArchive_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value is DateTime date)
            {
                e.Value = date.ToString("yyyy-MM-dd HH:mm");
                e.FormattingApplied = true;
            }
            else if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = Labels.State;
                e.FormattingApplied = true;
            }
        }
        #endregion


    }
}
