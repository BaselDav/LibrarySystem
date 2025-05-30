Library System 📚  
A simple library application developed using the 3-Tier Architecture, designed to manage book borrowing and returning operations in a smooth and effective way. The system is intended to be used by library staff.

🛠️ Technologies Used  
✅ C# - Windows Forms (WinForms)  
✅ .NET Framework 4.7.2  
✅ ADO.NET for database operations (without Entity Framework)  
✅ SQL Server  
✅ Clean separation through 3-tier architecture:  
- **Presentation Layer**: User interface  
- **Business Logic Layer (BLL)**: Application logic  
- **Data Access Layer (DAL)**: Database operations  

⚙️ Core Features  
🔍 Search for books by title, author, or ISBN  
📄 **Pagination support** in books, users, and history views to improve performance and navigation  
👤 Add users with email uniqueness check and automatic username suffixing in case of duplicates  
📥 Borrow books for selected users  
📤 Return books and update their availability status  
📝 View borrowing and return history  
👥 Manage user records (add, validate)  
📚 Manage book catalog (add, edit, delete)  

🧠 Notes  
- The application is intended to be used only by library staff.  
- Input validation and informative feedback messages are implemented.  
- Common runtime and connection errors are handled gracefully.  
- ⚠️ This project was built entirely from scratch without using any external libraries, third-party tools, or UI frameworks — relying solely on core .NET and ADO.NET.
