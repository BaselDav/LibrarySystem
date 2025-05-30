

namespace LibrarySystem.Localization
{
    public static class Messages
    {
        //Confirmations
        public const string ConfirmDeleteBook = "Are you sure you want to delete this book?";
        public const string ConfirmTitle = "Delete Confirmation";
        public static string ConfirmBorrow(string userName, string bookTitle)
            => $"Do you want user {userName} to borrow the book titled {bookTitle}?";
        public static string ConfirmReturn(string userName, string bookTitle)
            => $"Do you want user {userName} to return the book titled {bookTitle}?";

        //Success messages
        public const string UserAddedSuccess = "User has been added successfully.";
        public const string UserUpdatedSuccess = "User has been updated successfully.";
        public const string BookAddedSuccess = "Book has been added successfully.";
        public const string OperationSuccess = "Operation completed successfully.";
        public const string DuplicateRecord = "A book with this ISBN already exists.";

        //failure messages
        public const string OperationFailed = "Operation failed.";
        public const string UserDidNotBorrowBook = "This user has not borrowed this book.";
        public const string BorrowVerificationError = "An error occurred while verifying the user's book borrowing status.";
        public const string GeneralError = "An unexpected error occurred. Please try again later.";
        public const string UnexpectedError = "An unexpected error occurred while attempting to connect:";
        public const string DuplicateEmail = "This email is already registered.";
        public const string SearchKeywordRequired = "Search keyword cannot be empty.";

        //Validation - User
        public const string InvalidEmail = "The email format is invalid.";
        public const string EmailRequired = "Email is required.";
        public const string NameRequired = "Full name is required.";
        public const string EnterFullName = "Please enter the full name.";
        public const string EnterValidEmail = "Please enter a valid email address.";

        //Validation - Book
        public const string BookTitleRequired = "Book title is required.";
        public const string BookAuthorRequired = "Author name is required.";
        public const string BookIsbnRequired = "ISBN is required.";
        public const string BookIsbnInvalid = "ISBN must contain only digits.";
        public const string BookIsbnTooLong = "ISBN must not exceed 20 digits.";

        // Warnings
        public const string SelectUserFirst = "Please select a user first.";
        public const string WarningTitle = "Warning";
        public const string ErrorTitle = "Error";
        public const string InfoTitle = "Info";

        // Database
        public const string DbError = "An unexpected database error occurred.";
        public const string SqlServerUnavailable = "Unable to connect to the database server. Please check your network connection.";
        public const string InvalidDatabase = "The database is invalid or does not exist.";
        public const string LoginFailed = "Database login failed.";
        public const string ConstraintViolation = "The operation cannot be performed due to a data constraint.";
        public const string MissingDatabaseObject = "One or more required database objects are missing. Please check your database integrity.";
        public const string EnterConnectionString = "Please enter the connection string.";
        public const string ConnectionFailed = "Failed to connect to the database. Please make sure the SQL Server is running and the connection string is valid.";
        public const string ConnectionErrorTitle = "Connection Error";




        //public const string ConfirmDeleteBook = "هل تريد حذف هذا الكتاب؟";
        //public const string ConfirmTitle = "تأكيد الحذف";
        //public const string UserAddedSuccess = "تمت إضافة المستخدم بنجاح.";
        //public const string BookAddedSuccess = "تمت إضافة الكتاب.";
        //public const string OperationSuccess = "تمت العملية بنجاح.";
        //public const string OperationFailed = "فشلت العملية.";
        //public const string SelectUserFirst = "يرجى اختيار مستخدم أولاً.";
        //public const string UserDidNotBorrowBook = "هذا المستخدم لم يستعر هذا الكتاب.";
        //public const string InvalidEmail = "صيغة البريد الإلكتروني غير صحيحة.";
        //public const string EmailRequired = "البريد الإلكتروني مطلوب.";
        //public const string NameRequired = "الاسم مطلوب.";
        //public const string EnterFullName = "يرجى إدخال الاسم الكامل.";
        //public const string EnterValidEmail = "يرجى إدخال بريد إلكتروني صالح.";
        //public const string BookTitleRequired = "عنوان الكتاب مطلوب.";
        //public const string BookAuthorRequired = "اسم المؤلف مطلوب.";
        //public const string BookIsbnRequired = "الرقم التسلسلي مطلوب.";
        //public const string BookIsbnInvalid =  "يجب أن يكون الرقم التسلسلي للكتاب أرقام فقط.";
        //public const string BookIsbnTooLong = "يجب ألا يزيد الرقم التسلسلي عن 20 رقمًا.";

        //public const string SearchKeywordRequired = "كلمة البحث لا يمكن أن تكون فارغة.";
        //public const string MissingDatabaseObject = "أحد الجداول أو الكائنات المطلوبة غير موجود في قاعدة البيانات. يرجى التحقق من سلامة قاعدة البيانات.";

        //public static string ConfirmBorrow(string userName, string bookTitle)
        //=> $"هل تريد أن يستعير المستخدم {userName} الكتاب بعنوان {bookTitle}؟";

        //public static string ConfirmReturn(string userName, string bookTitle)
        //    => $"هل تريد أن يُرجع المستخدم {userName} الكتاب بعنوان {bookTitle}؟";

        ////Db
        //public const string DbError = "حدث خطأ غير متوقع في قاعدة البيانات.";
        //public const string SqlServerUnavailable = "لا يمكن الاتصال بخادم قاعدة البيانات. تأكد من الاتصال بالشبكة.";
        //public const string InvalidDatabase = "قاعدة البيانات غير موجودة أو غير صالحة.";
        //public const string LoginFailed = "فشل تسجيل الدخول إلى قاعدة البيانات.";
        //public const string ConstraintViolation = "لا يمكن تنفيذ العملية بسبب قيود على البيانات.";
        //public const string DuplicateRecord = "هذا الرقم التسلسلي يتبع لكتاب مسجل مسبقًا.";
        //public const string DuplicateEmail = "هذا الايميل مسجل مسبقًا.";

        //public const string GeneralError = "حدث خطأ ما . يرجى المحاولة لاحقا";
    }
}
