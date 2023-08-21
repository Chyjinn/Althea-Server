using System;
using System.Security.Cryptography;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace Database
{
    class MySQL
    {
        public static MySqlConnection connection;
        public String Host { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }

        public MySQL ()
        {
            this.Host = "localhost";
            this.Username = "root";
            this.Password = "";
            this.Database = "althea";
        }

        public static void InitConnection()
        {
            MySQL sql = new MySQL();
            string sqlConnectString = $"SERVER={sql.Host};PASSWORD={sql.Password};UID={sql.Username};DATABASE={sql.Database};";
            connection = new MySqlConnection();
            connection.ConnectionString = sqlConnectString + "Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
            connection.OpenAsync();
        }


        public static bool CloseConnection()
        {
            connection.CloseAsync();
            return true;
        }


            






    }
}
