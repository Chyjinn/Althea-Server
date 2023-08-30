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

            Appearance.HandleCharacterAppearance(player, c);
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

        public async void EditAttribute(Player player, int attributeid, string value)
        {
            Character character = await Data.GetCharacterData(player);
            switch (attributeid)
            {
                default:
                    break;
                case 0:
                    character.Appearance.Gender = Convert.ToBoolean(value);
                    break;
                case 1:
                    character.Appearance.EyeColor = Convert.ToByte(value);
                    break;
                case 2:
                    character.Appearance.HairColor = Convert.ToByte(value);
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:   
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    break;
                case 20:
                    break;
                case 21:
                    break;
                case 22:
                    break;
                case 23:
                    break;
                case 24:
                    break;
                case 25:
                    break;
                case 26:
                    break;
                case 27:
                    break;
                case 28:
                    break;
                case 29:
                    break;
                case 30:
                    break;
                case 31:
                    break;
                case 32:
                    break;
                case 33:
                    break;
                case 34:
                    break;
                case 35:
                    break;
                case 36:
                    break;
                case 37:
                    break;
                case 38:
                    break;
                case 39:
                    break;
                case 40:
                    break;
                case 41:
                    break;
                case 42:
                    break;
                case 43:
                    break;
                case 44:
                    break;
                case 45:
                    break;
                case 46:
                    break;
                case 47:
                    break;
                case 48:
                    break;
                case 49:
                    break;
                case 50:
                    break;
                case 51:
                    break;
                case 52:
                    break;
                case 53:
                    break;
                case 54:
                    break;
                case 55:
                    break;
                case 56:
                    break;
                case 57:
                    break;
                case 58:
                    break;
                case 59:
                    break;
                case 60:
                    break;
                case 61:
                    break;
                case 62:
                    break;
                case 63:
                    break;
                case -2: //Name
                    character.Name = value;
                    break;
                case -3://Date of birth
                    break;
                case -4://Place of birth
                    break;

            }

            //átírtuk a megváltoztatott értéket, beállítjuk a karakter kinézetét az új értékre
            Appearance.HandleCharacterAppearance(player, character);

            //lementjük a változtatásokat a következőig
            string json = NAPI.Util.ToJson(character);
            player.SetData("player:CharacterEditor", json);
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
