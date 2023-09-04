using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Admin
{
    internal class Levels
    {

        static Dictionary<string, int> acmds = new Dictionary<string, int>();
        /*{
            {"setdimension", 1},
            {"fly", 1},
            {"ban", 2}
        };*/

        public static async void LoadAcmds()
        {
            acmds.Clear();
            DateTime timestamp1 = DateTime.Now;
            
            await LoadAdminCommands();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput("Admin parancsok betöltve " + LoadTime.Milliseconds + " ms alatt.");

        }

        public static async Task LoadAdminCommands()
        {
            string query = $"SELECT command, adminLevel FROM `acmds`";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string command = Convert.ToString(reader["command"]);
                                int level = Convert.ToInt32(reader["adminLevel"]);
                                acmds.Add(command, level);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }
        }



        public static bool IsPlayerAdmin(string command, int adminlevel)
        {
            return true;//placeholder, bejelentkezésig maradhat így
            if (acmds.ContainsKey(command))
            {
                acmds.TryGetValue(command, out int requiredlevel);
                if (adminlevel >= requiredlevel)
                {
                    return true;
                }
            }
                return false;
        }


        public static async Task<bool> SetCommandLevel(string adminCmd, int adminlevel)
        {

            string query = $"UPDATE `acmds` SET `adminLevel` = @AdminLevel WHERE `acmds`.`command` = @AdminCMD;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@AdminLevel", adminlevel);
                    command.Parameters.AddWithValue("@AdminCMD", adminCmd);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            LoadAcmds();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }

            return false;
        }



        public static async Task<bool> SetAdminLevel(uint accid, int adminlevel)
        {

            string query = $"UPDATE `accounts` SET `adminLevel` = @AdminLevel WHERE `accounts`.`id` = @AccID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@AdminLevel", adminlevel);
                    command.Parameters.AddWithValue("@AccID", accid);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }

            return false;
        }


    }
}
