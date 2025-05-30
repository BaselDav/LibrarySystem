using System;
using System.Windows.Forms;
using LibrarySystem.BusinessLogic.Managers;
using LibrarySystem.DataAccess.Repositories;
using LibrarySystem.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LibrarySystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string userConnectionString;

            if (System.IO.File.Exists("connection.txt"))
            {
                userConnectionString = System.IO.File.ReadAllText("connection.txt").Trim();
            }
            else
            {
                using (var connForm = new ConnectionForm())
                {
                    if (connForm.ShowDialog() != DialogResult.OK)
                        return;

                    userConnectionString = connForm.ConnectionString;
                }
            }

            DatabaseInitializer.EnsureDatabaseAndTables(userConnectionString);

            var services = new ServiceCollection();
            services.AddSingleton<IBookRepository>(provider => new BookRepository(userConnectionString));
            services.AddTransient<ILibraryManager, LibraryManager>();
            services.AddTransient<SearchForm>();

            var serviceProvider = services.BuildServiceProvider();

            var mainForm = serviceProvider.GetRequiredService<SearchForm>();
            Application.Run(mainForm);
        }

    }
}
