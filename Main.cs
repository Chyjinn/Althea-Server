﻿using System;
using System.Threading;
using System.IO;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.AccessControl;
using Server.Admin;
using Org.BouncyCastle.Asn1.X509;
using Server.Characters;

namespace Server
{
    public class Main : Script
    {
        //PLAYER -811.6, 175.1, 76.7, 110
        //CAM -814 174.1 76.7, -70

        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            NAPI.Server.SetCommandErrorMessage("!{#ffffff}[!{##a83232}Hiba!{#ffffff}] Parancs nem található!");
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.World.SetWeather(Weather.EXTRASUNNY);
            //NAPI.Server.SetGlobalServerChat(true);
            //AutosavePlayers();
            NAPI.Task.Run(() =>
            {
                Admin.Levels.LoadAcmds();
                Inventory.ItemList.InitiateItemList();
                Inventory.Items.PopulateGroundItems();
                Characters.Clothing.LoadClothingShops();
                Interior.Properties.InitiateInteriors();
                Auth.Auth.DeleteTokens();

                
            
            },10000);
            //SetServerTime();
        }

        [ServerEvent(Event.VehicleDamage)]
        public void VehicleDamage(Vehicle v, float bodyhp, float enginehp)
        {
            Database.Log.Log_Server(v.DisplayName + " HP: " + NAPI.Vehicle.GetVehicleHealth(v.Handle) + " ;Body: " + NAPI.Vehicle.GetVehicleBodyHealth(v.Handle) + " loss: " + bodyhp + " ;Engine: " + NAPI.Vehicle.GetVehicleEngineHealth(v.Handle)  + " loss: "+ enginehp);
        }

        [ServerEvent(Event.PlayerDamage)]
        public void PlayerDamage(Player v, float hploss, float armorloss)
        {
            NAPI.Chat.SendChatMessageToAll(v.Name + " sebződés: " + hploss);
        }

        [Command("vehdamage")]
        public void VehDamage(Player player, int hp, float engine, float body)
        {
            player.Vehicle.SetSharedData("vehicle:Health", hp);
            player.Vehicle.SetSharedData("vehicle:EngineHealth", engine);
            player.Vehicle.SetSharedData("vehicle:BodyHealth", body);
        }
        List<GTANetworkAPI.Object> objects = new List<GTANetworkAPI.Object>();

        [Command("objecttest")]
        public void ObjectTest(Player player, string objectname)
        {
            for (int i = -5; i <= 5; i++)
            {
                for (int j = -5; j <= 5; j++)
                {
                    Vector3 placement = new Vector3(player.Position.X + i * 1f, player.Position.Y + j * 1f, player.Position.Z);
                    GTANetworkAPI.Object obj = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objectname), placement,new Vector3(0f,0f,0f), 255, player.Dimension);
                    
                    objects.Add(obj);
                }
            }
            player.SendChatMessage("Objectszám: " + objects.Count);
            player.TriggerEvent("client:StreamServerObjects");
        }


        [Command("wiregame")]
        public void WireMinigame(Player player)
        {
            player.TriggerEvent("client:ShowWireGame");
        }



        [Command("getvehiclehp")]
        public void showHP(Player player)
        {
            player.SendChatMessage("ENGINE: " + NAPI.Vehicle.GetVehicleEngineHealth(player.Vehicle) + " ; BODY: " + NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle));
        }

        [Command("deformveh")]
        public void DeformVeh(Player player, float offsetX, float offsetY, float offsetZ, float damage, float radius , bool focusonmodel)
        {
            player.TriggerEvent("client:DamageVehicle", offsetX, offsetY, offsetZ, damage, radius, focusonmodel);
        }



        public async void SetServerTime()
        {
            NAPI.Task.Run(() =>
            {
                DateTime time = DateTime.Now;
                NAPI.World.SetTime(Convert.ToInt16(time.Hour), Convert.ToInt16(time.Minute), Convert.ToInt16(time.Second));
                SetServerTime();
            }, 60000);
        }


        public void AutosavePlayers()
        {
            List<Player> players = NAPI.Pools.GetAllPlayers();
            NAPI.Chat.SendChatMessageToAll("AUTOSAVE: " + players.Count + " játékos.");
            foreach (var item in players)
            {
                if (item.HasData("Player:CharID"))
                {
                    Vector3 pos = item.Position;
                    Vector3 rot = item.Rotation;
                    uint charid = item.GetData<uint>("Player:CharID");
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
            if (player.HasData("Player:CharID"))
            {
                uint charid = player.GetData<uint>("Player:CharID");

                if (player.HasData("AdminJail:Remaining") == false)//nem ül adminjailben, tehát menthetjük a poziját
                {
                    if (!await SavePlayerPosition(charid, pos.X, pos.Y, pos.Z, rot.Z))
                    {
                        Database.Log.Log_Server(player.Name + " nem lett mentve!");
                    }
                }

            }
        }

        public static async Task<bool> SavePlayerPosition(uint charid, float posX, float posY, float posZ, float rot)
        {

            string query = $"UPDATE `characters` SET `posX` = @PositionX, `posY` = @PositionY, `posZ` = @PositionZ, `rot` = @Rotation WHERE `characters`.`id` = @CharacterID;";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
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

        [RemoteEvent("server:Log")]
        public void ClientToLog(Player player, string text)
        {
            Database.Log.Log_Server(player.Name + " kliens log: " + text);
        }


    }
}