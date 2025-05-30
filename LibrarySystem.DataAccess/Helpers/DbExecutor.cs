using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LibrarySystem.Localization;

namespace LibrarySystem.DataAccess.Helpers
{
    public static class DbExecutor
    {
        public static T Execute<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (SqlException ex)
            {
                Logger.Log(ex);
                HandleSqlException(ex);
                throw new ApplicationException(Messages.DbError, ex);
            }
        }

        public static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (SqlException ex)
            {
                Logger.Log(ex);
                HandleSqlException(ex);
                throw new ApplicationException(Messages.DbError, ex);
            }
        }

        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (SqlException ex)
            {
                Logger.Log(ex);
                HandleSqlException(ex);
                throw new ApplicationException(Messages.DbError, ex);
            }
        }

        public static async Task ExecuteAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (SqlException ex)
            {
                Logger.Log(ex);
                HandleSqlException(ex);
                throw new ApplicationException(Messages.DbError, ex);
            }
        }

        private static void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case -1:
                    throw new ApplicationException(Messages.SqlServerUnavailable, ex);
                case 4060:
                    throw new ApplicationException(Messages.InvalidDatabase, ex);
                case 18456:
                    throw new ApplicationException(Messages.LoginFailed, ex);
                case 547:
                    throw new ApplicationException(Messages.ConstraintViolation, ex);
                case 2627:
                case 2601:
                    throw new ApplicationException(Messages.DuplicateRecord, ex);
                default:
                    
                    throw new ApplicationException(Messages.MissingDatabaseObject, ex);
                    
                    
            }
        }
    }
}
