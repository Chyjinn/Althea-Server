using System;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace Server.Data
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
                con.Open();
                Main.Log_Server("");
                isConnectionSetUp = true;
            }
            catch (Exception ex)
            {
                Main.Log_Server(ex.ToString());
                throw;
            }
        }

        public static void ExecuteNonQuery(string SqlStr)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand(SqlStr, con))
                {
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        public static void RegisterPlayer(string username, string email, string password)
        {

        }




    }
}
