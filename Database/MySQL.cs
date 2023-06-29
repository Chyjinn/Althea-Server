using System;
using System.Security.Cryptography;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace Database
{
    class MySQL
    {
        public static bool isConnectionSetUp = false;
        public static MySqlConnection con;
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
            string SqlCon = $"SERVER={sql.Host};PASSWORD={sql.Password};UID={sql.Username};DATABASE={sql.Database};";
            con = new MySqlConnection(SqlCon);

            try
            {
                con.OpenAsync();
                isConnectionSetUp = true;
            }
            catch (Exception ex)
            {
                //
                throw;
            }
        }






    }
}
