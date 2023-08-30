using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    internal class Editor : Script
    {
        //CHAR: -811.68, 175.2, 76.74, 0, 0, 109.73
        //CAM: -813.95, 174.2, 76.78, 0, 0, -69

        public void NewChar(Player player)
        {
            Character c = new Character(0, "", DateTime.MinValue, "", -1, 0f, 0f, 0f, 0f);
            Appearance a = new Appearance(-1, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0);
            c.Appearance = a;

            string json = NAPI.Util.ToJson(c);
            player.SetData("player:CharacterEditor", json);

            Appearance.HandleCharacterAppearance(player);
            //bedobjuk a játékost szerkesztőbe
            //létrehozunk egy alap karaktert mivel nincs még neki
            //a karaktert beállítjuk és átküldjük
            player.SetSharedData("player:Frozen", true);
            player.Position = new Vector3(-811.68f, 175.2f, 76.74f);
            player.Rotation = new Vector3(0f, 0f, 110f);
            player.TriggerEvent("client:SetCamera", -814.3f, 174.1f, 77f, -10f, 0f, -72f, 48f);
            player.TriggerEvent("client:CharEdit");
            
            //Appearance.HandleCharacterAppearanceById(player, 0);
        }

        public async void EditAttribute(Player player, int attributeid, int value)
        {
            Character character = await Data.GetCharacterData(player);
            character.
        }

        [Command("charedit", Alias = "chareditor")]
        public async void CharEdit(Player player)
        {
            uint accID = player.GetData<uint>("player:accID");
            uint charID = player.GetData<uint>("player:charID");
            Character c = await Data.LoadCharacterData(accID, charID);
            player.SetSharedData("player:Frozen", true);
            player.Position = new Vector3(-811.68f, 175.2f, 76.74f);
            player.Rotation = new Vector3(0f, 0f, 110f);
            player.TriggerEvent("client:SetCamera", -814.3f, 174.1f, 77f, -10f, 0f, -72f, 48f);
            player.TriggerEvent("client:CharEdit");
            
        }

        [RemoteEvent("server:RotateCharRight")]
        public static void RotateCharRight(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z += 0.5f;
            player.Rotation = rot;
        }

        [RemoteEvent("server:RotateCharLeft")]
        public static void RotateCharLeft(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z -= 0.5f;
            player.Rotation = rot;
        }

    }
}
