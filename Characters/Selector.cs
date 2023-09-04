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
        static Dictionary<int, Vector3[]> scenes = new Dictionary<int, Vector3[]>()
        {
            //ID, POS, ROT
            {0, new Vector3[2] {new Vector3(-1051f,-1204f,3.9f), new Vector3(0f,0f,100f) } },
            {1, new Vector3[2] {new Vector3(393f,-355.7f,48f), new Vector3(0f,0f,-80f) } },
            {2, new Vector3[2] {new Vector3(-1351.8f,-1435.2f,4.3f), new Vector3(0f,0f,-71f) } },
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
            player.SetSharedData("player:Frozen", false);
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

                    NAPI.Player.SpawnPlayer(player, coords[0], coords[1].Z);

                    string json = NAPI.Util.ToJson(characters);
                    player.SetData("player:CharacterSelector", json);

                    player.TriggerEvent("client:SkyCam", false);
                    
                    Appearance.HandleCharacterAppearanceById(player, characters[0].Id);
                    NAPI.Task.Run(() =>
                    {
                        player.TriggerEvent("client:InfrontCamera");
                        player.TriggerEvent("client:showCharScreen", NAPI.Util.ToJson(characters));
                        player.SetSharedData("player:Frozen", true);
                    }, 300);
                    
                }, 2000);
            }
            else
            {
                    Editor.SetupCharEditor(player, accID);//nincs karaktere, bedobni karakter készítőbe
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
            if (await Data.IsCharacterOwner(accID, charid))
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

    
}
}

