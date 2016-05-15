using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace InkingAlphabets.DB
{
    public class SQLiteDbContext : DbContext
    {
        // This property defines the table
        public DbSet<Language> Languages { get; set; }

        // This method connects the context with the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = GetLocalDatabaseFile() };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
        private static string GetLocalDatabaseFile()
        {
            string localDirectory = string.Empty;
            try
            {
                localDirectory = ApplicationData.Current.LocalFolder.Path;                
            }
            catch (InvalidOperationException)
            { }

            return Path.Combine(localDirectory,App.LocalDbFolderName, App.LocalDbName);
        }
    }
}
