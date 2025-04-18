﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Inventory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Admin
{
    internal class Levels
    {

        static ConcurrentDictionary<string, int> acmds = new ConcurrentDictionary<string, int>();
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

        public async static Task<bool> LoadAdminCommands()
        {
            string query = $"SELECT command, adminLevel FROM `acmds`";
            using (MySqlConnection con = new MySqlConnection(await Database.DBCon.GetConString()))
            {
                try
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Prepare();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string command = Convert.ToString(reader["command"]);
                                int level = Convert.ToInt32(reader["adminLevel"]);
                                acmds.TryAdd(command, level);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

                con.Close();
            }
            return true;
        }



        public static bool IsPlayerAdmin(string command, int adminlevel)
        {
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
                con.ConnectionString = await Database.DBCon.GetConString();
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
                    con.CloseAsync();
                }
            }

            return false;
        }

        public static async Task<bool> SetAdminNick(uint accid, string adminnick)
        {
            bool state = false;
            string query = $"UPDATE `accounts` SET `adminNick` = @AdminNick WHERE `accounts`.`id` = @AccID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@AdminNick", adminnick);
                    command.Parameters.AddWithValue("@AccID", accid);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                await con.CloseAsync();
            }
            return state;
        }


        public static async Task<bool> SetAdminLevel(uint accid, int adminlevel)
        {
            bool state = false;
            string query = $"UPDATE `accounts` SET `adminLevel` = @AdminLevel WHERE `accounts`.`id` = @AccID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
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
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }

            return state;
        }


    }
}
