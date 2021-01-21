using System;

namespace SMDRReceiverService.SettingsObjects
{
    internal class SQLSettings
    {
        private static string database;
        private static string password;
        private static string server;
        private static string username;

        public string Database
        {
            get { return database; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("SQL Database cannot be blank. Check configuration file.");
                }
                else
                {
                    database = value;
                }
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("SQL Password cannot be blank. Check configuration file.");
                }
                else
                {
                    password = value;
                }
            }
        }

        public string Server
        {
            get { return server; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("SQL Server cannot be blank. Check configuration file.");
                }
                else
                {
                    server = value;
                }
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("SQL Username cannot be blank. Check configuration file.");
                }
                else
                {
                    username = value;
                }
            }
        }

        public string ConnectionString()
        {
            return $"Server={server};Database={database};UID={username};Password={password}";
        }
    }
}