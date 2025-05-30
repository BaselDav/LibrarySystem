using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibrarySystem.BusinessLogic.Managers;
using LibrarySystem.Domain;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public partial class UserSelectionDialog : Form
    {
        private readonly ILibraryManager libraryManager;
        public User SelectedUser { get; private set; }

        public UserSelectionDialog(ILibraryManager libraryManager)
        {
            InitializeComponent();
            this.libraryManager = libraryManager;

            this.Text = Labels.SelectUserTitle;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = btnOK;

            this.Load += UserSelectionDialog_Load;
            txtSearch.TextChanged += txtSearch_TextChanged;
            btnOK.Click += btnOK_Click;

            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.ReadOnly = true; 
            dgvUsers.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    dgvUsers.Rows[e.RowIndex].Selected = true;
            };

        }

        private async void UserSelectionDialog_Load(object sender, EventArgs e)
        {
            AppStyle.StyleButton(btnOK);
            ApplySearchPlaceholder();
            await LoadUsersAsync();
            dgvUsers.Focus();
        }
        private void ApplySearchPlaceholder()
        {
            const int EM_SETMARGINS = 0xd3;
            const int EC_LEFTMARGIN = 0x0001;
            const int EC_RIGHTMARGIN = 0x0002;

            txtSearch.Text = Labels.SearchPlaceholder;
            txtSearch.ForeColor = Color.Gray;

            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == Labels.SearchPlaceholder)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };

            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = Labels.SearchPlaceholder;
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            SendMessage(txtSearch.Handle, EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN, (IntPtr)(5 + (5 << 16)));
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);


        private async Task LoadUsersAsync(string keyword = "")
        {
            List<User> users;
            if (!string.IsNullOrWhiteSpace(keyword))
                users = await libraryManager.SearchUsersAsync(keyword, 1, 100);
            else
                users = await libraryManager.GetPagedUsersAsync(1, 100);

            dgvUsers.DataSource = users;

            if (dgvUsers.Columns.Contains("UserId"))
                dgvUsers.Columns["UserId"].Visible = false;

            if (dgvUsers.Columns.Contains("FullName"))
                dgvUsers.Columns["FullName"].HeaderText = Labels.FullNameLabel;

            if (dgvUsers.Columns.Contains("Email"))
                dgvUsers.Columns["Email"].HeaderText = Labels.EmailLabel;

            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == Labels.SearchPlaceholder) keyword = "";
            await LoadUsersAsync(keyword);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                SelectedUser = dgvUsers.SelectedRows[0].DataBoundItem as User;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(Labels.SelectUserFirst, Labels.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

      
    }
}
