using System;
using System.Text.RegularExpressions;
using SaveWise.DataLayer.Models;

namespace SaveWise.DataLayer
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed;
        private static string _connectionString;
        public static string DatabaseName { get; private set; }
        protected SaveWiseContext Context = new SaveWiseContext(_connectionString, DatabaseName);

        public static void Initialize(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            DatabaseName = databaseName;
        }
        
        public GenericRepository<T> GetRepository<T>() where T : Document
        {
            var collectionName = Regex.Replace(nameof(T), "(\\B[A-Z])", ".$1");
            return new GenericRepository<T>(Context.Database.GetCollection<T>(collectionName));
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Database = null;
                    Context = null;
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}