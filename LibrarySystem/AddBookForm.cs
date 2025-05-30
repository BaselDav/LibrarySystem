using System;
using System.Windows.Forms;
using LibrarySystem.BusinessLogic.Validation;
using LibrarySystem.Domain;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public partial class AddBookForm : BaseForm
    {
        private readonly Book _existingBook;

        public string BookTitle => txtTitle.Text.Trim();
        public string Author => txtAuthor.Text.Trim();
        public string ISBN => txtISBN.Text.Trim();

        public AddBookForm()
        {
            InitializeComponent();
            this.Text = Labels.AddBookTitle;
            btnAdd.Text = Labels.Add;
            lblTitle.Text = Labels.BookTitleLabel;
            lblAuthor.Text = Labels.AuthorLabel;
            lblISBN.Text = Labels.ISBNLabel;
            btnAdd.Click += BtnAdd_Click;
        }

        public AddBookForm(Book book) : this()
        {
            _existingBook = book;
            this.Text = Labels.EditBookTitle;
            btnAdd.Text = Labels.Save;

            txtTitle.Text = book.Title;
            txtAuthor.Text = book.Author;
            txtISBN.Text = book.ISBN;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!BookValidator.Validate(BookTitle, Author, ISBN, out string error))
            {
                MessageBox.Show(error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

       
    }
}
