using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Server.Characters
{
    internal class Selector : Script
    {
        static Dictionary<int, float[]> scenes = new Dictionary<int, float[]>()
        {
            //ID, float tömb: Kamera X-Y-Z-ROT, Kezdő X-Y-Z-ROT, Vég X-Y-Z-ROT
            //{0,new float[12] {-1030.5f, -1090.2f, 1.9f, 92.7f, -1033.1f, -1096.1f, 1.94f, -30f, -1031.9f, -1090.4f, 2.1f, -99.2f } },
            {0,new float[12] {-1053.5f, -1206.7f, 4.5f, -72f, -1052.9f, -1200.1f, 4.2f, -152f, -1050.6f, -1205.2f, 4f, 105.3f } },
        };


        //CHAR:
        //-811.8078, 175.06, 76.75, 0, 0, 104.9
        //CAM:
        //-814,07, 174.25, 76.74, 0, 0, -73
        [Command("changechar", Alias = "changecharacter")]
        public static void ProcessCharScreen(Player player)//bejelentkezés után ezt hívjuk meg, a logika itt lesz megvalósítva (van-e már karaktere, ha igen akkor betölteni)
        {
            uint accID = player.GetData<uint>("player:accID");
            player.Dimension = Convert.ToUInt32(accID);
            player.TriggerEvent("client:SkyCam", true);
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

                    float[] coords = scenes[randomscene];

                    Vector3 pos = new Vector3(coords[4], coords[5], coords[6]);
                    Vector3 rot = new Vector3(0f, 0f, coords[7]);
                    player.Position = pos;
                    player.Rotation = rot;

                    string json = NAPI.Util.ToJson(characters);
                    player.SetData("player:CharacterSelector", json);

                    player.TriggerEvent("client:SkyCam", false);
                    
                    Appearance.HandleCharacterAppearanceById(player, characters[0].Id);
                    NAPI.Task.Run(() =>
                    {
                        player.SetSharedData("player:Frozen", false);
                        player.TriggerEvent("client:SetCamera", coords[0], coords[1], coords[2], 0f, 0f, coords[3], 48f);
                        player.TriggerEvent("client:CharWalkIn", coords[8], coords[9], coords[10], coords[11]);
                        NAPI.Task.Run(() =>
                        {
                            player.TriggerEvent("client:showCharScreen", NAPI.Util.ToJson(characters));
                            player.SetSharedData("player:Frozen", true);
                        }, 5000);
                    }, 300);
                    
                }, 2000);
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    Editor.StartNewCharEdit(player);
                }, 500);

                //TODO: nincs karaktere, bedobni karakter készítőbe
            }

        }






    [RemoteEvent("server:CharChange")]
    public static void HandleCharacterChange(Player player, int characterid)
    {
        player.SetSharedData("player:Invisible", true);
        uint charid = Convert.ToUInt32(characterid);
        Appearance.HandleCharacterAppearanceById(player, charid);
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("player:Invisible", false);
            }, 150);
        }

    [RemoteEvent("server:CharSelect")]
    public async void SetPlayerCharacter(Player player, uint charid)//kiválasztotta a karakterét és be szeretne lépni
    {
            uint accID = player.GetData<uint>("player:accID");
            Character c = await Data.GetCharacterDataByID(player, charid);
            if (await IsCharacterOwner(accID, charid))
            {
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:SkyCam", true);
                    player.TriggerEvent("client:DeleteCamera");
                    player.TriggerEvent("client:hideCharScreen");
                    player.SetData("player:charID",charid);
                    player.SetSharedData("player:CharacterName",c.Name);
                    player.SetSharedData("player:VisibleName",c.Name);
                    player.Dimension = 0;
                    player.Name = c.Name;
                    NAPI.Task.Run(() =>
                    {
                        NAPI.Player.SpawnPlayer(player, new Vector3(c.posX, c.posY, c.posZ), c.Rot);
                        player.TriggerEvent("client:SkyCam", false);
                        player.SetSharedData("player:Frozen", false);
                        player.SetData<string>("player:CharacterSelector", null);

                    }, 2000);
                    
                });
            }
    }

        public static async Task<bool> IsCharacterOwner(uint accid, uint charid)//ha az adott account-hoz tartozik a karakter akkor true, különben false
        {
            string query = $"SELECT COUNT(id) FROM `characters` WHERE `accountId` = @AccID AND `id` = @CharID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accid);
                    cmd.Parameters.AddWithValue("@CharID", charid);
                    try
                    {
                        var count = await cmd.ExecuteScalarAsync();
                        if (Convert.ToInt32(count) > 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log.Log_Server(ex.ToString());
                    }
                }
            }
            return false;
        }


    public static void SetPlayerToWalkOut(Player player)
    {
        NAPI.Task.Run(() =>
        {
            player.TriggerEvent("client:CharWalkOut");
        }, 100);
    }

    
}
}

