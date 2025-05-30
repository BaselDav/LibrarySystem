using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public partial class ConnectionForm : Form
    {
        public string ConnectionString { get; private set; }

        public ConnectionForm()
        {
            InitializeComponent();
            btnConnect.Text = Labels.ConnectButtonText; 
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            string input = txtConnectionString.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show(Messages.EnterConnectionString, Messages.WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled = false;
            btnConnect.Text = Labels.ConnectingText; 
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                using (var conn = new SqlConnection(input))
                {
                    await conn.OpenAsync();
                }

                ConnectionString = input;
                File.WriteAllText("connection.txt", input);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException)
            {
                MessageBox.Show(Messages.ConnectionFailed, Messages.ConnectionErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.UnexpectedError}\n{ex.Message}", Messages.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Enabled = true;
                btnConnect.Text = Labels.ConnectButtonText;
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
