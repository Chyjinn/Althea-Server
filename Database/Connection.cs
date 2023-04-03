using System;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace Server.Connection
{
    class Connection
    {
        public static bool isConnectionSetUp = false;
        public static MySqlConnection con;
        public String Host { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }

        public Connection ()
        {
            this.Host = "localhost";
            this.Username = "root";
            this.Password = "1";
            this.Database = "althea";
        }
        public static void InitCon()
        {
            Connection sql = new Connection();
            string SqlString = $"SERVER={sql.Host};PASSWORD={sql.Password};UID={sql.Username};DATABASE={sql.Database};";
            con = new MySqlConnection(SqlString);

            try
            {
                con.Open();
                Main.Log_Server("Csatlakoztunk az adatbázishoz!");
                isConnectionSetUp = true;
            }
            catch (Exception ex)
            {
                Main.Log_Server(ex.ToString());
                throw;
            }
        }


    }
}
