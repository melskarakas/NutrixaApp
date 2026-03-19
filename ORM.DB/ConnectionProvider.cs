/// https://github.com/muratkuru/DapperInfrastructure/blob/master/DapperInfrastructure.Data/ConnectionProvider.cs

using Newtonsoft.Json;
using Npgsql;
//using System.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace ORM.DB
{
    public interface IConnectionProvider : IDisposable
    {
        IDbConnection GetConnection();
    }

    public class ConnectionProvider : IConnectionProvider
    {
        //private readonly SqlConnection connection;
        private readonly NpgsqlConnection connection;
        private readonly string connectionString;

        public string GetConnectionString()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "appsettings.json");
            Dictionary<string, dynamic> deger = new Dictionary<string, dynamic>();
            using (StreamReader file = File.OpenText(path))
            {
                deger = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(file.ReadToEnd());
            }
            return deger["AppConfiguration"]["ConStr"];
        }

        public ConnectionProvider(string connectionString = "")
        {
            if (connectionString == "")
            {
                connectionString = GetConnectionString();
            }

            this.connectionString = connectionString;
            connection = new NpgsqlConnection(connectionString);
        }

        public IDbConnection GetConnection()
        {
            return connection == null || string.IsNullOrEmpty(connection.ConnectionString)
                ? new NpgsqlConnection(connectionString)
                : connection;
        }

        #region Dispose
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                connection.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
