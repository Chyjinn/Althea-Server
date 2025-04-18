﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg.Sig;
using Server.Admin;
using Server.Interior;
using Server.Inventory;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Server.Characters
{
    internal class Selector : Script
    {
        static Dictionary<int, Vector3[]> scenes = new Dictionary<int, Vector3[]>()
        {
            //ID, POS, ROT
            {0, new Vector3[2] {new Vector3(-849,-66.8f,36.7f), new Vector3(0f,0f,-157.3f) } },
            {1, new Vector3[2] {new Vector3(392.5f,-355.7f,47f), new Vector3(0f,0f,-80f) } },
            {2, new Vector3[2] {new Vector3(-1351.8f,-1435.2f,3.6f), new Vector3(0f,0f,-71f) } },
        };


        //CHAR:
        //-811.8078, 175.06, 76.75, 0, 0, 104.9
        //CAM:
        //-814,07, 174.25, 76.74, 0, 0, -73
        [Command("changechar", Alias = "changecharacter")]
        public static void ProcessCharScreen(Player player)//bejelentkezés után ezt hívjuk meg, a logika itt lesz megvalósítva (van-e már karaktere, ha igen akkor betölteni)
        {
            player.TriggerEvent("client:Chat", false);
            player.TriggerEvent("client:BindKeys", false);

            uint accID = player.GetData<uint>("Player:AccID");
            player.Dimension = Convert.ToUInt32(accID);
            player.TriggerEvent("client:SkyCam", true);
            player.SetSharedData("Player:Frozen", false);
            SetCharacterDataForPlayer(player, accID);
        }





        public static async void SetCharacterDataForPlayer(Player player, uint accID)
        {
            Character[] characters = await Data.LoadAllCharacterData(accID);
            if (characters.Length > 0)//van legalább 1 karaktere
            {
                foreach (Character character in characters)
                {
                    Appearance a = await Data.LoadCharacterAppearance(character);
                    character.Appearance = a;
                }
                NAPI.Task.Run(() =>
                {
                    Random r = new Random();
                    int randomscene = r.Next(0, scenes.Count);

                    Vector3[] coords = scenes[randomscene];
                    player.SetSharedData("Player:Frozen", true);
                    NAPI.Player.SpawnPlayer(player, coords[0], coords[1].Z);

                    string json = NAPI.Util.ToJson(characters);
                    player.SetData("Player:CharacterSelector", json);

                    player.TriggerEvent("client:SkyCam", false);
                    
                    Appearance.HandleCharacterAppearanceById(player, characters[0].Id);
                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:showCharScreen", NAPI.Util.ToJson(characters));
                        player.TriggerEvent("client:InfrontCamera");
                    }, 2000);
                }, 2000);
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SetSharedData("Player:Frozen", true);
                    NAPI.Player.SpawnPlayer(player, new Vector3(-818f, 177f, 73f));
                    Editor.SetupCharEditor(player, accID);//nincs karaktere, bedobni karakter készítőbe
                }, 1000);

            }

        }



    [RemoteEvent("server:CharChange")]
    public static void HandleCharacterChange(Player player, int characterid)
    {
        player.SetSharedData("Player:Invisible", true);
        uint charid = Convert.ToUInt32(characterid);
        Appearance.HandleCharacterAppearanceById(player, charid);

            NAPI.Task.Run(() =>
            {
                player.SetSharedData("Player:Invisible", false);
            }, 150);
        }

    [RemoteEvent("server:CharSelect")]
    public async void SetPlayerCharacter(Player player, uint charid)//kiválasztotta a karakterét és be szeretne lépni
    {
            uint accID = player.GetData<uint>("Player:AccID");
            Character c = await Data.GetCharacterDataByID(player, charid);
            if (await Data.IsCharacterOwner(accID, charid))
            {
                AdminJail aj = await Admin.Commands.GetPlayerAdminJail(accID);
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:SkyCam", true);
                    player.TriggerEvent("client:DeleteCamera");
                    player.TriggerEvent("client:hideCharScreen");
                    player.SetData("Player:CharID",charid);
                    player.SetSharedData("Player:Gender", c.Appearance.Gender);
                    player.SetSharedData("Player:CharacterName",c.Name);
                    player.SetSharedData("Player:VisibleName",c.Name);
                    player.Dimension = 0;
                    player.Name = c.Name;

                    NAPI.Task.Run(() =>
                    {
                        if (aj != null)//ha adminjailben van
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SetData("AdminJail:OriginalPos", new Vector3(c.posX,c.posY,c.posZ));
                                player.SetData("AdminJail:Remaining", aj.Remaining);
                                player.SetData("AdminJail:ID", aj.ID);
                                NAPI.Player.SpawnPlayer(player, new Vector3(1395.5f, 1147.2f, 114.3f), -90f);
                                player.Dimension = player.Id;
                                player.SendChatMessage("[AdminJail] " + aj.AdminNick + " bebörtönzött téged " + aj.Time + " percre. Hátravan még: "+aj.Remaining+" perc. Indok: " + aj.Reason);
                                player.TriggerEvent("client:SkyCam", false);
                                Characters.Injuries.SetPlayerInjuries(player);
                            }); 
                        }
                        else
                        {
                            NAPI.Player.SpawnPlayer(player, new Vector3(c.posX, c.posY, c.posZ), c.Rot);
                            player.TriggerEvent("client:SkyCam", false);
                            Characters.Injuries.SetPlayerInjuries(player);
                        }

                        Properties.SendPropertiesToPlayer(player);
                        
                        player.SetSharedData("Player:Frozen", false);
                        player.SetData<string>("Player:CharacterSelector", null);
                        player.TriggerEvent("client:Chat", true);
                        player.TriggerEvent("client:HUD", true);
                        player.TriggerEvent("client:BindKeys", true);
                        Inventory.Items.LoadInventory(player);
                        Vehicles.Vehicles.LoadPlayerVehicles(player);
                    }, 2000);
                    
                });
            }
    }

    
}
}

