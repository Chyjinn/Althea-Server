using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Characters
{
    public class Character
    {
        public uint Id { get; set; }//-1
        public string Name { get; set; }//-2
        public DateTime DOB { get; set; }//-3
        public string POB { get; set; }//-4
        public float posX { get; set; }//-5
        public float posY { get; set; }//-6
        public float posZ { get; set; }//-7
        public float Rot { get; set; }//-8
        public int AppearanceID { get; set; }//-9
        public Appearance Appearance { get; set; }//-10
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
            this.Appearance = null;
        }
    }

    public class Appearance
    {
        public int Id { get; set; }//-11
        public bool Gender { get;  set; }//0
        public byte EyeColor { get; set; }//1
        public byte HairColor { get; set; }//2
        public byte HairHighlight { get; set; }//3
        //PARENTS
        public byte Parent1Face { get; set; }//4
        public byte Parent2Face { get; set; }//5
        public byte Parent3Face { get; set; }//6
        public byte Parent1Skin { get; set; }//7
        public byte Parent2Skin { get; set; }//8
        public byte Parent3Skin { get; set; }//9
        //MIX
        public byte FaceMix { get; set; }//10
        public byte SkinMix { get; set; }//11
        public byte OverrideMix { get; set; }//12
        public int HairStyle { get; set; } //-12
        //FACE
        public sbyte NoseWidth { get; set; }//13
        public sbyte NoseHeight { get; set; }//14
        public sbyte NoseLength { get; set; }//15
        public sbyte NoseBridge { get; set; }//16
        public sbyte NoseTip { get; set; }//17
        public sbyte NoseBroken { get; set; }//18
        public sbyte BrowHeight { get; set; }//19
        public sbyte BrowWidth { get; set; }//20
        public sbyte CheekboneHeight { get; set; }//21
        public sbyte CheekboneWidth { get; set; }//22
        public sbyte CheekWidth { get; set; }//23
        public sbyte Eyes { get; set; }//24
        public sbyte Lips { get; set; }//25
        public sbyte JawWidth { get; set; }//26
        public sbyte JawHeight { get; set; }//27
        public sbyte ChinLength { get; set; }//28
        public sbyte ChinPosition { get; set; }//29
        public sbyte ChinWidth { get; set; }//30
        public sbyte ChinShape { get; set; }//31
        public sbyte NeckWidth { get; set; }//32
        //OVERLAYS - 13 db, összesen 30 v 31
        public byte BlemishId { get; set; }//33
        public byte BlemishOpacity { get; set; }//34
        public byte FacialHairId { get; set; }//35
        public byte FacialHairColor { get; set; }//36
        public byte FacialHairOpacity { get; set; }//37
        public byte EyeBrowId { get; set; }//38
        public byte EyeBrowColor { get; set; }//39
        public byte EyeBrowOpacity { get; set; }//40
        public byte AgeId { get; set; }//41
        public byte AgeOpacity { get; set; }//42
        public byte MakeupId { get; set; }//43
        public byte MakeupOpacity { get; set; }//44
        public byte BlushId { get; set; }//45
        public byte BlushColor { get; set; }//46
        public byte BlushOpacity { get; set; }//47
        public byte ComplexionId { get; set; }//48
        public byte ComplexionOpacity { get; set; }//49
        public byte SundamageId { get; set; }//50
        public byte SundamageOpacity { get; set; }//51
        public byte LipstickId { get; set; }//52
        public byte LipstickColor { get; set; }//53
        public byte LipstickOpacity { get; set; }//54
        public byte FrecklesId { get; set; }//55
        public byte FrecklesOpacity { get; set; }//56
        public byte ChestHairId { get; set; }//57
        public byte ChestHairColor { get; set; }//58
        public byte ChestHairOpacity { get; set; }//59
        public byte BodyBlemish1Id { get; set; }//60
        public byte BodyBlemish1Opacity { get; set; }//61
        public byte BodyBlemish2Id { get; set; }//62
        public byte BodyBlemish2Opacity { get; set; }//63

        public Appearance(int id, bool gender,
            byte eyecolor, int hairstyle, byte haircolor, byte hairhighlight,
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
            HairStyle = hairstyle;
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
                (float)NoseWidth/100f,
                (float)NoseHeight/100f,
                (float)NoseLength/100f,
                (float)NoseBridge/100f,
                (float)NoseTip/100f,
                (float)NoseBroken/100f,
                (float)BrowHeight/100f,
                (float)BrowWidth/100f,
                (float)CheekboneHeight/100f,
                (float)CheekboneWidth/100f,
                (float)CheekWidth/100f,
                (float)Eyes/100f,
                (float)Lips/100f,
                (float)JawWidth / 100f,
                (float)JawHeight / 100f,
                (float)ChinLength / 100f,
                (float)ChinPosition / 100f,
                (float)ChinWidth / 100f,
                (float)ChinShape / 100f,
                (float)NeckWidth / 100f
            };
            return features;
        }


        public Dictionary<int, HeadOverlay> GetHeadOverlays()
        {
            Dictionary<int, HeadOverlay> overlays = new Dictionary<int, HeadOverlay>();

            HeadOverlay Blemishes = new HeadOverlay();
            Blemishes.Index = BlemishId;
            Blemishes.Opacity = (float)BlemishOpacity / 100f;

            HeadOverlay FacialHair = new HeadOverlay();
            FacialHair.Index = FacialHairId;
            FacialHair.Color = FacialHairColor;
            FacialHair.Opacity = (float)FacialHairOpacity / 100f;

            HeadOverlay EyeBrows = new HeadOverlay();
            EyeBrows.Index = EyeBrowId;
            EyeBrows.Color = EyeBrowColor;
            EyeBrows.Opacity = (float)EyeBrowOpacity / 100f;

            HeadOverlay Ageing = new HeadOverlay();
            Ageing.Index = AgeId;
            Ageing.Opacity = (float)AgeOpacity / 100f;

            HeadOverlay Makeup = new HeadOverlay();
            Makeup.Index = MakeupId;
            Makeup.Opacity = (float)MakeupOpacity / 100f;

            HeadOverlay Blush = new HeadOverlay();
            Blush.Index = BlushId;
            Blush.Color = BlushColor;
            Blush.Opacity = (float)BlushOpacity / 100f;

            HeadOverlay Complexion = new HeadOverlay();
            Complexion.Index = ComplexionId;
            Complexion.Opacity = (float)ComplexionOpacity / 100f;

            HeadOverlay SunDamage = new HeadOverlay();
            SunDamage.Index = SundamageId;
            SunDamage.Opacity = (float)SundamageOpacity / 100f;

            HeadOverlay Lipstick = new HeadOverlay();
            Lipstick.Index = LipstickId;
            Lipstick.Color = LipstickColor;
            Lipstick.Opacity = (float)LipstickOpacity / 100f;

            HeadOverlay Freckles = new HeadOverlay();
            Freckles.Index = FrecklesId;
            Freckles.Opacity = (float)FrecklesOpacity / 100f;

            HeadOverlay ChestHair = new HeadOverlay();
            ChestHair.Index = ChestHairId;
            ChestHair.Color = ChestHairColor;
            ChestHair.Opacity = (float)ChestHairOpacity / 100f;

            HeadOverlay BodyBlemishes1 = new HeadOverlay();
            BodyBlemishes1.Index = BodyBlemish1Id;
            BodyBlemishes1.Opacity = (float)BodyBlemish1Opacity / 100f;

            HeadOverlay BodyBlemishes2 = new HeadOverlay();
            BodyBlemishes2.Index = BodyBlemish2Id;
            BodyBlemishes2.Opacity = (float)BodyBlemish2Opacity / 100f;

            overlays.Add(0, Blemishes);
            overlays.Add(1, FacialHair);
            overlays.Add(2, EyeBrows);
            overlays.Add(3, Ageing);
            overlays.Add(4, Makeup);
            overlays.Add(5, Blush);
            overlays.Add(6, Complexion);
            overlays.Add(7, SunDamage);
            overlays.Add(8, Lipstick);
            overlays.Add(9, Freckles);
            overlays.Add(10, ChestHair);
            overlays.Add(11, BodyBlemishes1);
            overlays.Add(12, BodyBlemishes2);

            return overlays;
        }

        public HeadBlend GetHeadBlend()
        {
            HeadBlend h = new HeadBlend();
            h.ShapeFirst = Parent1Face;
            h.ShapeSecond = Parent2Face;
            h.ShapeThird = Parent3Face;
            h.SkinFirst = Parent1Skin;
            h.SkinSecond = Parent2Skin;
            h.SkinThird = Parent3Skin;
            h.ShapeMix = Convert.ToSingle(FaceMix)/100f;//-100 és +100 között tároljuk, de -1f és +1f közöttit vár
            h.SkinMix = Convert.ToSingle(SkinMix)/100f;
            h.ThirdMix = Convert.ToSingle(OverrideMix)/100f;
            return h;
        }

        public static void HandleCharacterAppearance(Player player, Character character)
        {
            HeadBlend HeadBlend = character.Appearance.GetHeadBlend();
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();
            Dictionary<int, HeadOverlay> overlays = character.Appearance.GetHeadOverlays();
            Decoration[] decor = new Decoration[0];
            player.SetCustomization(character.Appearance.Gender, HeadBlend, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            for (int i = 0; i < 19; i++)
            {
                player.SetFaceFeature(i, FaceFeatures[i]);
            }

            player.SetClothes(2, character.Appearance.HairStyle, 0);
        }

        public static async void HandleCharacterAppearanceById(Player player, uint charid)
        {
            Character character = await Data.GetCharacterDataByID(player, charid);
            HeadBlend HeadBlend = character.Appearance.GetHeadBlend();
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();
            Dictionary<int, HeadOverlay> overlays = character.Appearance.GetHeadOverlays();
            Decoration[] decor = new Decoration[0];
            player.SetCustomization(character.Appearance.Gender, HeadBlend, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            for (int i = 0; i < 19; i++)
            {
                player.SetFaceFeature(i, FaceFeatures[i]);
            }

            player.SetClothes(2, character.Appearance.HairStyle, 0);
        }

    }
}
