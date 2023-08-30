using System;
using System.Threading;
using System.IO;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Server
{
    public class Main : Script
    {
        //PLAYER -811.6, 175.1, 76.7, 110
        //CAM -814 174.1 76.7, -70

        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            NAPI.Server.SetCommandErrorMessage("!{#ffffff}[!{##a83232}Hiba!{#ffffff}]Parancs nem található!");
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.World.SetWeather(Weather.EXTRASUNNY);
            NAPI.Server.SetGlobalServerChat(true);
            //AutosavePlayers();
            NAPI.Task.Run(() =>
            {
                Database.Log.Log_Server("admin parancsok betöltésének megkezdése...");
                Admin.Levels.LoadAcmds();
            },10000);
            
        }


        public void AutosavePlayers()
        {
            List<Player> players = NAPI.Pools.GetAllPlayers();
            NAPI.Chat.SendChatMessageToAll("AUTOSAVE: " + players.Count + " játékos.");
            foreach (var item in players)
            {
                if (item.HasData("player:charID"))
                {
                    Vector3 pos = item.Position;
                    Vector3 rot = item.Rotation;
                    uint charid = item.GetData<uint>("player:charID");
                    SavePlayerPos(charid, pos.X, pos.Y, pos.Z, rot.Z);
                }
            }
            NAPI.Task.Run(() =>
            {
                AutosavePlayers();
            }, 30000);
        }

        public async void SavePlayerPos(uint charid, float posX, float posY, float posZ, float rot)
        {
            await SavePlayerPosition(charid, posX, posY, posZ, rot);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public async void OnPlayerDisconnect(Player player, DisconnectionType type, string reason)
        {
            Vector3 pos = player.Position;
            Vector3 rot = player.Rotation;
            if (player.HasData("player:charID"))
            {
                uint charid = player.GetData<uint>("player:charID");
                if (!await SavePlayerPosition(charid, pos.X, pos.Y, pos.Z, rot.Z))
                {
                    Database.Log.Log_Server(player.Name + " nem lett mentve!");
                }
            }
        }

        public static async Task<bool> SavePlayerPosition(uint charid, float posX, float posY, float posZ, float rot)
        {

            string query = $"UPDATE `characters` SET `posX` = @PositionX, `posY` = @PositionY, `posZ` = @PositionZ, `rot` = @Rotation WHERE `characters`.`id` = @CharacterID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@PositionX", posX);
                    command.Parameters.AddWithValue("@PositionY", posY);
                    command.Parameters.AddWithValue("@PositionZ", posZ);
                    command.Parameters.AddWithValue("@Rotation", rot);
                    command.Parameters.AddWithValue("@CharacterID", charid);
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



        [ServerEvent(Event.PlayerSpawn)]
        public void PlayerSpawned(Player player)
        {
            //player.TriggerEvent("CreateAuthForms");
            //player.TriggerEvent("SetLoginCamera", -426.0f, 1117.0f, 350.0f, 0.0f, 0.0f, -163.0f, 70.0f);
        }

        [RemoteEvent("server:Fly")]
        public void Fly(Player player, int x, int y, int z)
        {
            GTANetworkAPI.Vector3 v = new GTANetworkAPI.Vector3(x, y, z);
            player.Position = v;
         
        }
    }
}