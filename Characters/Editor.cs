using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    public class Editor : Script
    {

        public static void StartNewCharEdit(Player player)//játékos név/ID alapján bővíteni majd
        {
            if (player.HasData("player:accID"))
            {
                player.TriggerEvent("client:SkyCam", true);
                NewChar(player);
            }
        }

        public static void NewChar(Player player)
        {
            Character c = new Character(0, "", DateTime.MinValue, "", 0, 0f, 0f, 0f, 0f);
            Appearance a = new Appearance(-1, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 50, 50, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0,0,0);
            c.Appearance = a;

            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(c);
                player.SetData("player:CharacterEditor", json);

                Appearance.HandleCharacterAppearance(player, c);


                player.Position = new Vector3(-811.68f, 175.2f, 76.74f);
                player.Rotation = new Vector3(0f, 0f, 110f);
                //player.TriggerEvent("client:SetCamera", -814.3f, 174.1f, 77f, -10f, 0f, -72f, 48f);

                player.SetSharedData("player:Frozen", true);
                player.TriggerEvent("client:SkyCam", false);

                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:CharEdit", true);
                    player.TriggerEvent("client:EditorCamera");
                }, 4000);

            }, 500);
        }

        public async void CreateNewCharacter(Player player, uint accountID)
        {
            if (await Data.AddNewCharacterToDatabase(player, accountID))
            {
                NAPI.Task.Run(() =>
                {
                    player.SetData<string>("player:CharacterEditor", "");
                    Selector.ProcessCharScreen(player);
                }, 5000);

            }
        }


        [RemoteEvent("server:CharEdit")]
        public static void EditChar(Player player, uint charID)//játékos név/ID alapján bővíteni majd
        {
            if (player.HasData("player:accID"))
            {
                uint accID = player.GetData<uint>("player:accID");
                player.TriggerEvent("client:DeleteCamera");
                player.TriggerEvent("client:hideCharScreen");
                player.TriggerEvent("client:SkyCam", true);
                SetupCharEditor(player, accID, charID);
            }
        }

        public async static void SetupCharEditor(Player player, uint accID, uint charID)
        {
            Character c = await Data.LoadCharacterData(accID, charID);
            Appearance a = await Data.LoadCharacterAppearance(c);
            c.Appearance = a;

            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(c);
                player.SetData("player:CharacterEditor", json);
                Appearance.HandleCharacterAppearance(player, c);


                //player.Position = new Vector3(-811.68f, 175.2f, 76.74f);
                //player.Rotation = new Vector3(0f, 0f, 110f);
                player.Position = new Vector3(167f, -979f, 30f);
                player.Rotation = new Vector3(0f, 0f, 130f);


                player.SetSharedData("player:Frozen", true);
                player.TriggerEvent("client:SkyCam", false);

                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:CharEdit", true);
                    player.TriggerEvent("client:EditorCamera");
                }, 4500);

            }, 3000);
        }

        [RemoteEvent("server:FinishEditing")]
        public async void FinishEditing(Player player)
        {
            Character c = await Data.GetCharacterData(player);
            uint accID = player.GetData<uint>("player:accID");
            player.TriggerEvent("client:CharEdit", false);
            player.TriggerEvent("client:DeleteCamera");
            player.TriggerEvent("client:SkyCam", true);
            if (c.Id != 0)//ha nem 0 akkor meglévő karakterről van szó
            {
                EditExistingCharacter(player, c.AppearanceID);
                
            }
            else//0 a karakter ID, szóval újat kell létrehoznunk.
            {
                //új karaktert hoz létre
                CreateNewCharacter(player, accID);
            }
        }




        public async void EditExistingCharacter(Player player, uint appearanceID)
        {
            if (await Data.EditExistingCharacterInDatabase(player, appearanceID))
            {
                NAPI.Task.Run(() =>
                {
                    player.SetData<string>("player:CharacterEditor", "");
                    player.SetSharedData("player:Frozen", false);
                    Selector.ProcessCharScreen(player);
                }, 5000);

            }
        }


        [RemoteEvent("server:EditAttribute")]
        public async void EditAttribute(Player player, int attributeid, string value)
        {
            Character character = await Data.GetCharacterData(player);
            switch (attributeid)
            {
                default:
                    break;
                case -2://Name
                    character.Name = value;
                    break;
                case -3://POB
                    character.POB = value;
                    break;
                case -4://DOB
                    character.DOB = Convert.ToDateTime(value);
                    break;
                case -12:
                    character.Appearance.HairStyle = Convert.ToInt32(value);
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
                    player.SendChatMessage("makeup " + value);
                    character.Appearance.MakeupId = Convert.ToByte(value);
                    break;
                case 44:
                    player.SendChatMessage("makeup opacity " + value);
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

            }
            //átírtuk a megváltoztatott értéket, beállítjuk a karakter kinézetét az új értékre

            //lementjük a változtatásokat a következőig
            string json = NAPI.Util.ToJson(character);
            player.SetData("player:CharacterEditor", json);
            Appearance.HandleCharacterAppearance(player, character);
        }



        [RemoteEvent("server:RotateCharRight")]
        public static void RotateCharRight(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z += 1f;
            player.Rotation = rot;
        }

        [RemoteEvent("server:RotateCharLeft")]
        public static void RotateCharLeft(Player player)
        {
            Vector3 rot = player.Rotation;
            rot.Z -= 1f;
            player.Rotation = rot;
        }

    }
}
