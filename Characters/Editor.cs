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
                    character.Appearance.HairHighlight = Convert.ToByte(value);
                    break;
                case 4:
                    character.Appearance.Parent1Face = Convert.ToByte(value);
                    break;
                case 5:
                    character.Appearance.Parent2Face = Convert.ToByte(value);
                    break;
                case 6:
                    character.Appearance.Parent3Face = Convert.ToByte(value);
                    break;
                case 7:
                    character.Appearance.Parent1Skin = Convert.ToByte(value);
                    break;
                case 8:
                    character.Appearance.Parent2Skin = Convert.ToByte(value);
                    break;
                case 9:
                    character.Appearance.Parent3Skin = Convert.ToByte(value);
                    break;
                case 10:
                    character.Appearance.FaceMix = Convert.ToByte(value);
                    break;
                case 11:
                    character.Appearance.SkinMix = Convert.ToByte(value);
                    break;
                case 12:
                    character.Appearance.OverrideMix = Convert.ToByte(value);
                    break;
                case 13:
                    character.Appearance.NoseWidth = Convert.ToSByte(value);
                    break;
                case 14:
                    character.Appearance.NoseHeight = Convert.ToSByte(value);
                    break;
                case 15:
                    character.Appearance.NoseLength = Convert.ToSByte(value);
                    break;
                case 16:
                    character.Appearance.NoseBridge = Convert.ToSByte(value);
                    break;
                case 17:
                    character.Appearance.NoseTip = Convert.ToSByte(value);
                    break;
                case 18:
                    character.Appearance.NoseBroken = Convert.ToSByte(value);
                    break;
                case 19:
                    character.Appearance.BrowHeight = Convert.ToSByte(value);
                    break;
                case 20:
                    character.Appearance.BrowWidth = Convert.ToSByte(value);
                    break;
                case 21:
                    character.Appearance.CheekboneHeight = Convert.ToSByte(value);
                    break;
                case 22:
                    character.Appearance.CheekboneWidth = Convert.ToSByte(value);
                    break;
                case 23:
                    character.Appearance.CheekWidth = Convert.ToSByte(value);
                    break;
                case 24:
                    character.Appearance.Eyes = Convert.ToSByte(value);
                    break;
                case 25:
                    character.Appearance.Lips = Convert.ToSByte(value);
                    break;
                case 26:
                    character.Appearance.JawWidth = Convert.ToSByte(value);
                    break;
                case 27:
                    character.Appearance.JawHeight = Convert.ToSByte(value);
                    break;
                case 28:
                    character.Appearance.ChinLength = Convert.ToSByte(value);
                    break;
                case 29:
                    character.Appearance.ChinPosition = Convert.ToSByte(value);
                    break;
                case 30:
                    character.Appearance.ChinWidth = Convert.ToSByte(value);
                    break;
                case 31:
                    character.Appearance.ChinShape = Convert.ToSByte(value);
                    break;
                case 32:
                    character.Appearance.NeckWidth = Convert.ToSByte(value);
                    break;
                case 33:
                    character.Appearance.BlemishId = Convert.ToByte(value);
                    break;
                case 34:
                    character.Appearance.BlemishOpacity = Convert.ToByte(value);
                    break;
                case 35:
                    character.Appearance.FacialHairId = Convert.ToByte(value);
                    break;
                case 36:
                    character.Appearance.FacialHairColor = Convert.ToByte(value);
                    break;
                case 37:
                    character.Appearance.FacialHairOpacity = Convert.ToByte(value);
                    break;
                case 38:
                    character.Appearance.EyeBrowId = Convert.ToByte(value);
                    break;
                case 39:
                    character.Appearance.EyeBrowColor = Convert.ToByte(value);
                    break;
                case 40:
                    character.Appearance.EyeBrowOpacity = Convert.ToByte(value);
                    break;
                case 41:
                    character.Appearance.AgeId = Convert.ToByte(value);
                    break;
                case 42:
                    character.Appearance.AgeOpacity = Convert.ToByte(value);
                    break;
                case 43:
                    character.Appearance.MakeupId = Convert.ToByte(value);
                    break;
                case 44:
                    character.Appearance.MakeupOpacity = Convert.ToByte(value);
                    break;
                case 45:
                    character.Appearance.BlushId = Convert.ToByte(value);
                    break;
                case 46:
                    character.Appearance.BlushColor = Convert.ToByte(value);
                    break;
                case 47:
                    character.Appearance.BlushOpacity = Convert.ToByte(value);
                    break;
                case 48:
                    character.Appearance.ComplexionId = Convert.ToByte(value);
                    break;
                case 49:
                    character.Appearance.ComplexionOpacity = Convert.ToByte(value);
                    break;
                case 50:
                    character.Appearance.SundamageId = Convert.ToByte(value);
                    break;
                case 51:
                    character.Appearance.SundamageOpacity = Convert.ToByte(value);
                    break;
                case 52:
                    character.Appearance.LipstickId = Convert.ToByte(value);
                    break;
                case 53:
                    character.Appearance.LipstickColor = Convert.ToByte(value);
                    break;
                case 54:
                    character.Appearance.LipstickOpacity = Convert.ToByte(value);
                    break;
                case 55:
                    character.Appearance.FrecklesId = Convert.ToByte(value);
                    break;
                case 56:
                    character.Appearance.FrecklesOpacity = Convert.ToByte(value);
                    break;
                case 57:
                    character.Appearance.ChestHairId = Convert.ToByte(value);
                    break;
                case 58:
                    character.Appearance.ChestHairColor = Convert.ToByte(value);
                    break;
                case 59:
                    character.Appearance.ChestHairOpacity = Convert.ToByte(value);
                    break;
                case 60:
                    character.Appearance.BodyBlemish1Id = Convert.ToByte(value);
                    break;
                case 61:
                    character.Appearance.BodyBlemish1Opacity = Convert.ToByte(value);
                    break;
                case 62:
                    character.Appearance.BodyBlemish2Id = Convert.ToByte(value);
                    break;
                case 63:
                    character.Appearance.BodyBlemish2Opacity = Convert.ToByte(value);
                    break;
                case -2: //Name
                    character.Name = value;
                    break;
                case -3://Date of birth
                    character.DOB = Convert.ToDateTime(value);
                    break;
                case -4://Place of birth
                    character.POB = value;
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
