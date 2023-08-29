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
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string POB { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float Rot { get; set; }
        public int AppearanceID { get; set; }
        public Appearance Appearance { get; set; }
        public Character(uint Id, string Name, DateTime DOB, string POB, int AppearanceID, float posX, float posY, float posZ, float rot)
        {
            this.DOB = DOB;
            this.POB = POB;
            this.Id = Id;
            this.Name = Name;
            this.AppearanceID = AppearanceID;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.Rot = rot;  
        }
    }

    public class Appearance
    {
        public int Id { get; set; }
        public bool Gender { get; set; }
        public byte EyeColor { get; set; }
        public byte HairColor { get; set; }
        public byte HairHighlight { get; set; }
        //PARENTS
        public byte Parent1Face { get; set; }
        public byte Parent2Face { get; set; }
        public byte Parent3Face { get; set; }
        public byte Parent1Skin { get; set; }
        public byte Parent2Skin { get; set; }
        public byte Parent3Skin { get; set; }
        //MIX
        public byte FaceMix { get; set; }
        public byte SkinMix { get; set; }
        public byte OverrideMix { get; set; }
        //FACE
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
        //OVERLAYS - 13 db, összesen 30 v 31
        public byte BlemishId { get; set; }
        public byte BlemishOpacity { get; set; }
        public byte FacialHairId { get; set; }
        public byte FacialHairColor { get; set; }
        public byte FacialHairOpacity { get; set; }
        public byte EyeBrowId { get; set; }
        public byte EyeBrowColor { get; set; }
        public byte EyeBrowOpacity { get; set; }
        public byte AgeId { get; set; }
        public byte AgeOpacity { get; set; }
        public byte MakeupId { get; set; }
        public byte MakeupOpacity { get; set; }
        public byte BlushId { get; set; }
        public byte BlushColor { get; set; }
        public byte BlushOpacity { get; set; }
        public byte ComplexionId { get; set; }
        public byte ComplexionOpacity { get; set; }
        public byte SundamageId { get; set; }
        public byte SundamageOpacity { get; set; }
        public byte LipstickId { get; set; }
        public byte LipstickColor { get; set; }
        public byte LipstickOpacity { get; set; }
        public byte FrecklesId { get; set; }
        public byte FrecklesOpacity { get; set; }
        public byte ChestHairId { get; set; }
        public byte ChestHairColor { get; set; }
        public byte ChestHairOpacity { get; set; }
        public byte BodyBlemish1Id { get; set; }
        public byte BodyBlemish1Opacity { get; set; }
        public byte BodyBlemish2Id { get; set; }
        public byte BodyBlemish2Opacity { get; set; }

        public Appearance(int id, bool gender,
            byte eyecolor, byte haircolor, byte hairhighlight,
            byte p1f, byte p2f, byte p3f, byte p1s, byte p2s, byte p3s,
            byte facemix, byte skinmix, byte overridemix,
            sbyte nosewidth, sbyte noseheight, sbyte noselength,
            sbyte nosebridge, sbyte nosetip, sbyte nosebroken,
            sbyte browheight, sbyte browwidth, sbyte cheekboneheight,
            sbyte cheekbonewidth, sbyte cheekwidth, sbyte eyes, sbyte lips,
            sbyte jawwidth, sbyte jawheight, sbyte chinlength, sbyte chinposition,
            sbyte chinwidth, sbyte chinshape, sbyte neckwidth,
            byte blemishid, byte blemishopacity,
            byte facialhairid, byte facialhaircolor, byte facialhairopacity,
            byte eyebrowid, byte eyebrowcolor, byte eyebrowopacity,
            byte ageid, byte ageopacity,
            byte makeupid, byte makeupopacity,
            byte blushid, byte blushcolor, byte blushopacity,
            byte complexionid, byte complexionopacity,
            byte sundamageid, byte sundamageopacity,
            byte lipstickid, byte lipstickcolor, byte lipstickopacity,
            byte frecklesid, byte frecklesopacity,
            byte chesthairid, byte chesthaircolor, byte chesthairopacity,
            byte bodyblemish1id, byte bodyblemish1opacity,
            byte bodyblemish2id, byte bodyblemish2opacity)
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
            BlemishId = blemishid;
            BlemishOpacity = blemishopacity;
            FacialHairId = facialhairid;
            FacialHairColor = facialhaircolor;
            FacialHairOpacity = facialhairopacity;
            EyeBrowId = eyebrowid;
            EyeBrowColor = eyebrowcolor;
            EyeBrowOpacity = eyebrowopacity;
            AgeId = ageid;
            AgeOpacity = ageopacity;
            MakeupId = makeupid;
            MakeupOpacity = makeupopacity;
            BlushId = blushid;
            BlushColor = blushcolor;
            BlushOpacity = blushopacity;
            ComplexionId = complexionid;
            ComplexionOpacity = complexionopacity;
            SundamageId = sundamageid;
            SundamageOpacity = sundamageopacity;
            LipstickId = lipstickid;
            LipstickColor = lipstickcolor;
            LipstickOpacity = lipstickopacity;
            FrecklesId = frecklesid;
            FrecklesOpacity = frecklesopacity;
            ChestHairId = chesthairid;
            ChestHairColor = chesthaircolor;
            ChestHairOpacity = chesthairopacity;
            BodyBlemish1Id = bodyblemish1id;
            BodyBlemish1Opacity = bodyblemish1opacity;
            BodyBlemish2Id = bodyblemish2id;
            BodyBlemish2Opacity = bodyblemish2opacity;
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


        public Dictionary<int, HeadOverlay> GetHeadOverlays()
        {
            Dictionary<int, HeadOverlay> overlays = new Dictionary<int, HeadOverlay>();
            return overlays;
        }

    }

    internal class Selector : Script
    {
        static Dictionary<int, float[]> scenes = new Dictionary<int, float[]>()
        {
            //ID, float tömb: Kamera X-Y-Z-ROT, Kezdő X-Y-Z-ROT, Vég X-Y-Z-ROT
            {0,new float[12] {195.7f, -938.3f, 30f, 147.0f, 189.3f, -934.15f, 30.69f, -163.45f, 190.75f, -940.2f, 30.7f, -38f } },
        };


        //CHAR:
        //-811.8078, 175.06, 76.75, 0, 0, 104.9
        //CAM:
        //-814,07, 174.25, 76.74, 0, 0, -73
        public static void ProcessCharScreen(Player player)//bejelentkezés után ezt hívjuk meg, a logika itt lesz megvalósítva (van-e már karaktere, ha igen akkor betölteni)
        {
            uint accID = player.GetData<uint>("player:accID");
            player.Dimension = Convert.ToUInt32(accID);
            SetCharacterDataForPlayer(player, accID);
        }

        public static async void SetCharacterDataForPlayer(Player player, uint accID)
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
                    



                    HandleCharacterAppearance(player, characters[0].Id);
                    NAPI.Task.Run(() =>
                    {
                        player.SetSharedData("player:Frozen", false);
                        player.TriggerEvent("client:SetCamera", coords[0], coords[1], coords[2], 0f, 0f, coords[3], 48f);
                        player.TriggerEvent("client:CharWalkIn", coords[8], coords[9], coords[10], coords[11]);
                        NAPI.Task.Run(() =>
                        {
                            player.TriggerEvent("client:showCharScreen", NAPI.Util.ToJson(characters));
                        }, 1000);
                    }, 1000);


                    
                    
                    
                    
                });
            }
            else
            {
                //TODO: nincs karaktere, bedobni karakter készítőbe
            }

        }

        public static async Task<Character[]> LoadCharacterData(uint accID)
        {
            string query = $"SELECT id,characterName,dob,pob,appearanceId,posX,posY,posZ,rot FROM `characters` WHERE `accountId` = @accountID";
            List<Character> characters = new List<Character>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@accountID", accID);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Character c = new Character(Convert.ToUInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToDateTime(reader["dob"]), reader["pob"].ToString(), Convert.ToInt32(reader["appearanceId"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]), Convert.ToSingle(reader["rot"]));
                                characters.Add(c);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }
            return characters.ToArray();
        }

        public static async Task<Appearance> LoadCharacterAppearance(Character c)
        {
            string query = $"SELECT * FROM `appearances` WHERE `id` = @appearanceID LIMIT 1";
            
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
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
                                Appearance app = new Appearance(Convert.ToInt32(reader["id"]), gender, Convert.ToByte(reader["eyeColor"]), Convert.ToByte(reader["hairColor"]), Convert.ToByte(reader["hairHighlight"]), Convert.ToByte(reader["parent1face"]), Convert.ToByte(reader["parent2face"]), Convert.ToByte(reader["parent3face"]),
                                    Convert.ToByte(reader["parent1skin"]), Convert.ToByte(reader["parent2skin"]), Convert.ToByte(reader["parent3skin"]),
                                    Convert.ToByte(reader["faceMix"]), Convert.ToByte(reader["skinMix"]), Convert.ToByte(reader["thirdMix"]),
                                    Convert.ToSByte(reader["noseWidth"]), Convert.ToSByte(reader["noseHeight"]), Convert.ToSByte(reader["noseLength"]), Convert.ToSByte(reader["noseBridge"]), Convert.ToSByte(reader["noseTip"]), Convert.ToSByte(reader["noseBroken"]),
                                Convert.ToSByte(reader["browHeight"]), Convert.ToSByte(reader["browWidth"]), Convert.ToSByte(reader["cheekboneHeight"]), Convert.ToSByte(reader["cheekboneWidth"]), Convert.ToSByte(reader["cheekWidth"]),
                                Convert.ToSByte(reader["eyes"]), Convert.ToSByte(reader["lips"]), Convert.ToSByte(reader["jawWidth"]), Convert.ToSByte(reader["jawHeight"]), Convert.ToSByte(reader["chinLength"]), Convert.ToSByte(reader["chinPosition"]), Convert.ToSByte(reader["chinWidth"]), Convert.ToSByte(reader["chinShape"]), Convert.ToSByte(reader["neckWidth"]),
                                Convert.ToByte(reader["blemishId"]), Convert.ToByte(reader["blemishOpacity"]),
                                Convert.ToByte(reader["facialhairId"]), Convert.ToByte(reader["facialhairColor"]), Convert.ToByte(reader["facialhairOpacity"]),
                                Convert.ToByte(reader["eyebrowId"]), Convert.ToByte(reader["eyebrowColor"]), Convert.ToByte(reader["eyebrowOpacity"]),
                                Convert.ToByte(reader["ageId"]), Convert.ToByte(reader["ageOpacity"]),
                                Convert.ToByte(reader["makeupId"]), Convert.ToByte(reader["makeupOpacity"]),
                                Convert.ToByte(reader["blushId"]), Convert.ToByte(reader["blushColor"]), Convert.ToByte(reader["blushOpacity"]),
                                Convert.ToByte(reader["complexionId"]), Convert.ToByte(reader["complexionOpacity"]),
                                Convert.ToByte(reader["sundamageId"]), Convert.ToByte(reader["sundamageOpacity"]),
                                Convert.ToByte(reader["lipstickId"]), Convert.ToByte(reader["lipstickColor"]), Convert.ToByte(reader["lipstickOpacity"]),
                                Convert.ToByte(reader["frecklesId"]), Convert.ToByte(reader["frecklesOpacity"]),
                                Convert.ToByte(reader["chesthairId"]), Convert.ToByte(reader["chesthairColor"]), Convert.ToByte(reader["chesthairOpacity"]),
                                Convert.ToByte(reader["bodyblemishId"]), Convert.ToByte(reader["bodyblemishOpacity"]),
                                Convert.ToByte(reader["bodyblemish2Id"]), Convert.ToByte(reader["bodyblemish2Opacity"]));
                                
                                
                                return app;
                                //sbyte -128 - 127
                                //byte 0 - 255
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                    
                }
            }
            return null;
        }

        public static async void HandleCharacterAppearance(Player player, uint charid)
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
            Dictionary<int, HeadOverlay> overlays = character.Appearance.GetHeadOverlays();
            NAPI.Task.Run(() =>
            {
                
                Decoration[] decor = new Decoration[0];
                player.SetCustomization(character.Appearance.Gender, h, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            });
        }

        public static async Task<Character> GetCharacterDataByID(Player player, uint charid)//karakter ID alapján egy karaktert ad vissza
        {
            Character[] characters = NAPI.Util.FromJson<Character[]>(player.GetData<string>("player:CharacterSelector"));
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
    public static void HandleCharacterChange(Player player, int characterid)
    {
        player.SetSharedData("player:Frozen", false);
        uint charid = Convert.ToUInt32(characterid);
        SetPlayerToWalkOut(player);
        NAPI.Task.Run(() =>
        {
            player.TriggerEvent("client:ChatStopWalk");
            //itt kell majd átváltani a karit
            HandleCharacterAppearance(player, charid);
        }, 2000);
    }

    [RemoteEvent("server:CharSelect")]
    public async void SetPlayerCharacter(Player player, uint charid)//kiválasztotta a karakterét és be szeretne lépni
    {
            uint accID = player.GetData<uint>("player:accID");
            Character c = await GetCharacterDataByID(player, charid);
            if (await IsCharacterOwner(accID, charid))
            {
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:ChatStopWalk");
                    player.TriggerEvent("client:DeleteCamera");
                    player.TriggerEvent("client:hideCharScreen");
                    player.SetSharedData("player:Frozen", false);
                    player.SetData("player:charID",charid);
                    player.SetSharedData("player:CharacterName",c.Name);
                    player.SetSharedData("player:VisibleName",c.Name);
                    player.Dimension = 0;
                    NAPI.Player.SpawnPlayer(player, new Vector3(c.posX, c.posY, c.posZ), c.Rot);
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

