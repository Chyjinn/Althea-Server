﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Server.Characters
{
    class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AppearanceID { get; set; }
        public Appearance Appearance { get; set; }
        public Character(int charID, string charName, int charApperanceID) {
            Id = charID;
            Name = charName;
            AppearanceID = charApperanceID;
        }
    }

    public class Appearance
    {
        public int Id { get; set; }
        public bool Gender { get; set; }
        public byte EyeColor { get; set; }
        public byte HairColor { get; set; }
        public byte HairHighlight { get; set; }
        public byte Parent1Face { get; set; }
        public byte Parent2Face { get; set; }
        public byte Parent3Face { get; set; }
        public byte Parent1Skin { get; set; }
        public byte Parent2Skin { get; set; }
        public byte Parent3Skin { get; set; }
        public byte FaceMix { get; set; }
        public byte SkinMix { get; set; }
        public byte OverrideMix { get; set; }
        public sbyte NoseWidth { get; set; }
        public sbyte NoseHeight { get; set; }
        public sbyte NoseLength { get; set; }
        public sbyte NoseBridge { get; set; }
        public sbyte NoseTip { get; set; }
        public sbyte NoseBroken { get; set; }
        public sbyte BrowHeight { get; set; }
        public sbyte BrowWidth { get; set; }
        public sbyte CheekboneHeight { get; set; }
        public sbyte CheekboneWidth { get; set; }
        public sbyte CheekWidth { get; set; }
        public sbyte Eyes { get; set; }
        public sbyte Lips { get; set; }
        public sbyte JawWidth { get; set; }
        public sbyte JawHeight { get; set; }
        public sbyte ChinLength { get; set; }
        public sbyte ChinPosition { get; set; }
        public sbyte ChinWidth { get; set; }
        public sbyte ChinShape { get; set; }
        public sbyte NeckWidth { get; set; }

        public void Set(int id, bool gender, byte eyecolor, byte haircolor, byte hairhighlight, byte p1f, byte p2f, byte p3f, byte p1s, byte p2s, byte p3s, byte facemix, byte skinmix, byte overridemix, sbyte nosewidth, sbyte noseheight, sbyte noselength, sbyte nosebridge, sbyte nosetip, sbyte nosebroken, sbyte browheight, sbyte browwidth, sbyte cheekboneheight, sbyte cheekbonewidth, sbyte cheekwidth, sbyte eyes, sbyte lips, sbyte jawwidth, sbyte jawheight, sbyte chinlength, sbyte chinposition, sbyte chinwidth, sbyte chinshape, sbyte neckwidth)
        {
            Id = id;
            Gender = gender; // false-> female, true-> male
            EyeColor = eyecolor;
            HairColor = haircolor;
            HairHighlight = hairhighlight;
            Parent1Face = p1f;
            Parent2Face = p2f;
            Parent3Face = p3f;
            Parent1Skin = p1s;
            Parent2Skin = p2s;
            Parent3Skin = p3s;
            FaceMix = facemix;
            SkinMix = skinmix;
            OverrideMix = overridemix;
            NoseWidth = nosewidth;
            NoseHeight = noseheight;
            NoseLength = noselength;
            NoseBridge = nosebridge;
            NoseTip = nosetip;
            NoseBroken = nosebroken;
            BrowHeight = browheight;
            BrowWidth = browwidth;
            CheekboneHeight = cheekboneheight;
            CheekboneWidth = cheekbonewidth;
            CheekWidth = cheekwidth;
            Eyes = eyes;
            Lips = lips;
            JawWidth = jawwidth;
            JawHeight = jawheight;
            ChinLength = chinlength;
            ChinPosition = chinposition;
            ChinWidth = chinwidth;
            ChinShape = chinshape;
            NeckWidth = neckwidth;
        }

        public float[] GetFaceFeatures()
        {
            float[] features = new float[20]
            {
                NoseWidth/100,
                NoseHeight/100,
                NoseLength/100,
                NoseBridge/100,
                NoseTip/100,
                NoseBroken/100,
                BrowHeight/100,
                BrowWidth/100,
                CheekboneHeight/100,
                CheekboneWidth/100,
                CheekWidth/100,
                Eyes/100,
                Lips/100,
                JawWidth / 100,
                JawHeight / 100,
                ChinLength / 100,
                ChinPosition / 100,
                ChinWidth / 100,
                ChinShape / 100,
                NeckWidth / 100
            };
            return features;

        }
    }


    internal class CharacterScreen : Script
    {
        //CHAR:
        //-811.8078, 175.06, 76.75, 0, 0, 104.9
        //CAM:
        //-814,07, 174.25, 76.74, 0, 0, -73
        public static void ProcessCharScreen(Player player)
        {
            int accID = player.GetSharedData<int>("player:accID");
            SetCharacterDataForPlayer(player, accID);
        }

        public static async void SetCharacterDataForPlayer(Player player, int accID)
        {
            Character[] characters = await LoadCharacterData(accID);
            foreach (Character character in characters)
            {
                Appearance a = await LoadCharacterAppearance(character);
                character.Appearance = a;
            }
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(characters);
                player.SetData("characterData", json);
                player.TriggerEvent("client:SetCamera", -814.07f, 174.25f, 76.5f, 0f, 0f, -75f, 48f);
                player.TriggerEvent("client:showCharScreen");
                HandleCharacterAppearance(player, 0);
                SetPlayerToWalkIn(player);
            });
        }

        public static async Task<Character[]> LoadCharacterData(int accID)
        {
            string query = $"SELECT id,characterName,appearanceId FROM `characters` WHERE `accountId` = @accountID";
            List<Character> characters = new List<Character>();
            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@accountID", accID);
                cmd.Prepare();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Character c = new Character(Convert.ToInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToInt32(reader["appearanceId"]));
                            characters.Add(c);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Data.Log.Log_Server(ex.ToString());
                }
            }
            return characters.ToArray();
        }

        public static async Task<Appearance> LoadCharacterAppearance(Character c)
        {
            string query = $"SELECT id,gender,eyeColor,hairColor,hairHighlight, parent1face,parent2face,parent3face, parent1skin,parent2skin,parent3skin, faceMix,skinMix,thirdMix, noseWidth,noseHeight,noseLength,noseBridge,noseTip,noseBroken,browHeight,browWidth,cheekboneHeight,cheekboneWidth,cheekWidth,eyes,lips,jawWidth,jawHeight,chinLength,chinPosition,chinWidth,chinShape,neckWidth FROM `appearances` WHERE `id` = @appearanceID LIMIT 1";
            Appearance app = new Appearance();
            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@appearanceID", c.AppearanceID);
                cmd.Prepare();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            bool gender = false;
                            if (Convert.ToInt32(reader["gender"]) == 1)
                            {
                                gender = true;
                            }

                            app.Set(Convert.ToInt32(reader["id"]), gender, Convert.ToByte(reader["eyeColor"]), Convert.ToByte(reader["hairColor"]), Convert.ToByte(reader["hairHighlight"]), Convert.ToByte(reader["parent1face"]), Convert.ToByte(reader["parent2face"]), Convert.ToByte(reader["parent3face"]),
                                Convert.ToByte(reader["parent1skin"]), Convert.ToByte(reader["parent2skin"]), Convert.ToByte(reader["parent3skin"]),
                                Convert.ToByte(reader["faceMix"]), Convert.ToByte(reader["skinMix"]), Convert.ToByte(reader["thirdMix"]),
                                Convert.ToSByte(reader["noseWidth"]), Convert.ToSByte(reader["noseHeight"]), Convert.ToSByte(reader["noseLength"]), Convert.ToSByte(reader["noseBridge"]), Convert.ToSByte(reader["noseTip"]), Convert.ToSByte(reader["noseBroken"]),
                            Convert.ToSByte(reader["browHeight"]), Convert.ToSByte(reader["browWidth"]), Convert.ToSByte(reader["cheekboneHeight"]), Convert.ToSByte(reader["cheekboneWidth"]), Convert.ToSByte(reader["cheekWidth"]),
                            Convert.ToSByte(reader["eyes"]), Convert.ToSByte(reader["lips"]), Convert.ToSByte(reader["jawWidth"]), Convert.ToSByte(reader["jawHeight"]), Convert.ToSByte(reader["chinLength"]), Convert.ToSByte(reader["chinPosition"]), Convert.ToSByte(reader["chinWidth"]), Convert.ToSByte(reader["chinShape"]), Convert.ToSByte(reader["neckWidth"]));

                            //sbyte -128 - 127
                            //byte 0 - 255
                        }
                    }
                }
                catch (Exception ex)
                {
                    Data.Log.Log_Server(ex.ToString());
                }
                return app;
            }
        }




        public static async void HandleCharacterAppearance(Player player, int charid)
        {
            Character[] characters = await GetCharacterData(player);
            HeadBlend h = new HeadBlend();
            h.ShapeFirst = characters[charid].Appearance.Parent1Face;
            h.ShapeSecond = characters[charid].Appearance.Parent2Face;
            h.ShapeThird = characters[charid].Appearance.Parent3Face;
            h.SkinFirst = characters[charid].Appearance.Parent1Skin;
            h.SkinSecond = characters[charid].Appearance.Parent2Skin;
            h.SkinThird = characters[charid].Appearance.Parent3Skin;
            h.ShapeMix = characters[charid].Appearance.FaceMix;
            h.SkinMix = characters[charid].Appearance.SkinMix;
            h.ThirdMix = characters[charid].Appearance.OverrideMix;
            float[] FaceFeatures = characters[charid].Appearance.GetFaceFeatures();
            NAPI.Task.Run(() =>
            {
                Dictionary<int, HeadOverlay> overlays = new Dictionary<int, HeadOverlay>();
                Decoration[] decor = new Decoration[0];
                player.SetCustomization(characters[charid].Appearance.Gender, h, characters[charid].Appearance.EyeColor, characters[charid].Appearance.HairColor, characters[charid].Appearance.HairHighlight, FaceFeatures, overlays, decor);
            });
        }

        public static async Task<Character[]> GetCharacters(Player player, int accID)
        {
            Character[] characters = await GetCharacterData(player);
            return characters;
        }

        public static async Task<Character[]> GetCharacterData(Player player)
        {
            Character[] characters = NAPI.Util.FromJson<Character[]>(player.GetData<string>("characterData"));
            return characters;
        }





        [Command("char")]
    public static void HandleCharacterChange(Player player, int charid)
    {
        SetPlayerToWalkOut(player);
        NAPI.Task.Run(() =>
        {
            player.TriggerEvent("client:ChatStopWalk");
            //itt kell majd átváltani a karit
            HandleCharacterAppearance(player, charid);
            SetPlayerToWalkIn(player);
        }, 3000);
    }

    public static void SetPlayerToWalkIn(Player player)
    {
        Vector3 pos = new Vector3(-813.35, 173.24f, 76.74f);
        Vector3 rot = new Vector3(0f, 0f, -37f);
        player.Position = pos;
        player.Rotation = rot;
        NAPI.Task.Run(() =>
        {
            player.SendChatMessage("Karakter elindult");
            player.TriggerEvent("client:CharWalkIn");
        }, 750);
    }

    public static void SetPlayerToWalkOut(Player player)
    {
        NAPI.Task.Run(() =>
        {
            player.SendChatMessage("Karakter elindult");
            player.TriggerEvent("client:CharWalkOut");
        }, 750);
    }
}
}

