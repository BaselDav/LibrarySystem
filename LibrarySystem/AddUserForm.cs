using System;
using System.Windows.Forms;
using LibrarySystem.Domain;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public partial class AddUserForm : BaseForm
    {
        private readonly User existingUser;
        private readonly User editingUser;

        public string FullName => txtFullName.Text.Trim();
        public string Email => txtEmail.Text.Trim();

        public AddUserForm()
        {
            InitializeComponent();
            this.Text = Labels.AddUserTitle;
            btnSave.Text = Labels.SaveUser;
            lblFullName.Text = Labels.FullNameLabel;
            lblEmail.Text = Labels.EmailLabel;
            btnSave.Click += BtnSave_Click;
        }

        
        public AddUserForm(User user = null)
        {
            InitializeComponent();
            editingUser = user;

            this.Text = user == null ? Labels.AddUserTitle : Labels.Edit;
            btnSave.Text = Labels.SaveUser;

            lblFullName.Text = Labels.FullNameLabel;
            lblEmail.Text = Labels.EmailLabel;

            if (user != null)
            {
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
            }

            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show(Messages.EnterFullName);
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                MessageBox.Show(Messages.EnterValidEmail);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
