using GTANetworkAPI;
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
        public DateTime DOB { get; set; }
        public string POB { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public int AppearanceID { get; set; }
        public Appearance Appearance { get; set; }
        public Character(int Id, string Name, DateTime DOB, string POB, int AppearanceID, float posX, float posY, float posZ)
        {
            this.DOB = DOB;
            this.POB = POB;
            this.Id = Id;
            this.Name = Name;
            this.AppearanceID = AppearanceID;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
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
            Id = id;//adatbázis id
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

        public float[] GetFaceFeatures()//RAGE API SetCustomization-hoz szükséges float tömböt ad vissza
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

    internal class Selector : Script
    {
        //CHAR:
        //-811.8078, 175.06, 76.75, 0, 0, 104.9
        //CAM:
        //-814,07, 174.25, 76.74, 0, 0, -73
        public static void ProcessCharScreen(Player player)//bejelentkezés után ezt hívjuk meg, a logika itt lesz megvalósítva (van-e már karaktere, ha igen akkor betölteni)
        {
            int accID = player.GetSharedData<int>("player:accID");
            player.Dimension = Convert.ToUInt32(accID);
            player.SendChatMessage("Dimenzió: " + accID);
            SetCharacterDataForPlayer(player, accID);//adatbázis műveletek miatt átmegyünk async-ba
        }

        public static async void SetCharacterDataForPlayer(Player player, int accID)//betöltjük a karakter adatait, kinézetét, ezt egy Character osztályba mentjük 
        {
            Character[] characters = await LoadCharacterData(accID);
            if (characters.Length > 0)//van legalább 1 karaktere
            {
                foreach (Character character in characters)
                {
                    Appearance a = await LoadCharacterAppearance(character);
                    character.Appearance = a;
                }
                NAPI.Task.Run(() =>
                {
                    //ameddig beállítjuk a karakter adatait, kamerát, stb. addig megnyithatnánk egy betöltő videót hogy látványos legyen
                    string json = NAPI.Util.ToJson(characters);
                    player.SetData("characterData", json);

                    SetPlayerToWalkIn(player);
                    player.TriggerEvent("client:showCharScreen", NAPI.Util.ToJson(characters));
                    
                    HandleCharacterAppearance(player, characters[0].Id);
                    player.TriggerEvent("client:SetCamera", -814.3f, 174.1f, 77f, -10f, 0f, -72f, 48f);
                });
            }
            else
            {
                //TODO: nincs karaktere, bedobni karakter készítőbe
            }

        }

        public static async Task<Character[]> LoadCharacterData(int accID)
        {
            string query = $"SELECT id,characterName,dob,pob,appearanceId,posX,posY,posZ FROM `characters` WHERE `accountId` = @accountID";
            List<Character> characters = new List<Character>();
            using (MySqlCommand cmd = new MySqlCommand(query, Data.Connection.con))
            {
                cmd.Parameters.AddWithValue("@accountID", accID);
                cmd.Prepare();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Character c = new Character(Convert.ToInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToDateTime(reader["dob"]), reader["pob"].ToString(), Convert.ToInt32(reader["appearanceId"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"])) ;
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
            using (MySqlCommand cmd = new MySqlCommand(query, Data.Connection.con))
            {
                cmd.Parameters.AddWithValue("@appearanceID", c.AppearanceID);
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
            Character character = await GetCharacterDataByID(player, charid);
            HeadBlend h = new HeadBlend();
            h.ShapeFirst = character.Appearance.Parent1Face;
            h.ShapeSecond = character.Appearance.Parent2Face;
            h.ShapeThird = character.Appearance.Parent3Face;
            h.SkinFirst = character.Appearance.Parent1Skin;
            h.SkinSecond = character.Appearance.Parent2Skin;
            h.SkinThird = character.Appearance.Parent3Skin;
            h.ShapeMix = character.Appearance.FaceMix;
            h.SkinMix = character   .Appearance.SkinMix;
            h.ThirdMix = character.Appearance.OverrideMix;
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();
            NAPI.Task.Run(() =>
            {
                Dictionary<int, HeadOverlay> overlays = new Dictionary<int, HeadOverlay>();
                Decoration[] decor = new Decoration[0];
                player.SetCustomization(character.Appearance.Gender, h, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            });
        }

        public static async Task<Character> GetCharacterDataByID(Player player, int charid)//karakter ID alapján egy karaktert ad vissza
        {
            Character[] characters = NAPI.Util.FromJson<Character[]>(player.GetData<string>("characterData"));
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].Id == charid)
                {
                    return characters[i];
                }
            }
            return characters[0];
        }



    [RemoteEvent("server:CharChange")]
    public static void HandleCharacterChange(Player player, int charid)
    {
        SetPlayerToWalkOut(player);
        NAPI.Task.Run(() =>
        {
            player.TriggerEvent("client:ChatStopWalk");
            //itt kell majd átváltani a karit
            HandleCharacterAppearance(player, charid);
            SetPlayerToWalkIn(player);
        }, 2000);
    }

    public static void SetPlayerToWalkIn(Player player)
    {
        Vector3 pos = new Vector3(-813.35, 173.24f, 76.74f);
        Vector3 rot = new Vector3(0f, 0f, -37f);
        player.Position = pos;
        player.Rotation = rot;
        NAPI.Task.Run(() =>
        {
            player.TriggerEvent("client:CharWalkIn");
        }, 500);
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

