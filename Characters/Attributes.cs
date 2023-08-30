using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

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
            this.Appearance = null;
        }
    }

    public class Appearance
    {
        public int Id { get; set; }
        public bool Gender { get;  set; }
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

            HeadOverlay Blemishes = new HeadOverlay();
            Blemishes.Index = BlemishId;
            Blemishes.Opacity = BlemishOpacity;

            HeadOverlay FacialHair = new HeadOverlay();
            FacialHair.Index = FacialHairId;
            FacialHair.Color = FacialHairColor;
            FacialHair.Opacity = FacialHairOpacity;

            HeadOverlay EyeBrows = new HeadOverlay();
            EyeBrows.Index = EyeBrowId;
            EyeBrows.Color = EyeBrowColor;
            EyeBrows.Opacity = EyeBrowOpacity;

            HeadOverlay Ageing = new HeadOverlay();
            Ageing.Index = AgeId;
            Ageing.Opacity = AgeOpacity;

            HeadOverlay Makeup = new HeadOverlay();
            Makeup.Index = MakeupId;
            Makeup.Opacity = MakeupOpacity;

            HeadOverlay Blush = new HeadOverlay();
            Blush.Index = BlushId;
            Blush.Color = BlushColor;
            Blush.Opacity = BlushOpacity;

            HeadOverlay Complexion = new HeadOverlay();
            Complexion.Index = ComplexionId;
            Makeup.Opacity = ComplexionOpacity;

            HeadOverlay SunDamage = new HeadOverlay();
            SunDamage.Index = SundamageId;
            SunDamage.Opacity = SundamageOpacity;

            HeadOverlay Lipstick = new HeadOverlay();
            Lipstick.Index = LipstickId;
            Lipstick.Color = LipstickColor;
            Lipstick.Opacity = LipstickOpacity;

            HeadOverlay Freckles = new HeadOverlay();
            Freckles.Index = FrecklesId;
            Freckles.Opacity = FrecklesOpacity;

            HeadOverlay ChestHair = new HeadOverlay();
            ChestHair.Index = ChestHairId;
            ChestHair.Color = ChestHairColor;
            ChestHair.Opacity = ChestHairOpacity;

            HeadOverlay BodyBlemishes1 = new HeadOverlay();
            BodyBlemishes1.Index = BodyBlemish1Id;
            BodyBlemishes1.Opacity = BodyBlemish1Opacity;

            HeadOverlay BodyBlemishes2 = new HeadOverlay();
            BodyBlemishes2.Index = BodyBlemish2Id;
            BodyBlemishes2.Opacity = BodyBlemish2Opacity;

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

        public static async void HandleCharacterAppearance(Player player)
        {
            Character character = await Data.GetCharacterData(player);
            HeadBlend h = new HeadBlend();
            h.ShapeFirst = character.Appearance.Parent1Face;
            h.ShapeSecond = character.Appearance.Parent2Face;
            h.ShapeThird = character.Appearance.Parent3Face;
            h.SkinFirst = character.Appearance.Parent1Skin;
            h.SkinSecond = character.Appearance.Parent2Skin;
            h.SkinThird = character.Appearance.Parent3Skin;
            h.ShapeMix = character.Appearance.FaceMix;
            h.SkinMix = character.Appearance.SkinMix;
            h.ThirdMix = character.Appearance.OverrideMix;
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();
            Dictionary<int, HeadOverlay> overlays = character.Appearance.GetHeadOverlays();
            NAPI.Task.Run(() =>
            {
                Decoration[] decor = new Decoration[0];
                player.SetCustomization(character.Appearance.Gender, h, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            });
        }

        public static async void HandleCharacterAppearanceById(Player player, uint charid)
        {
            Character character = await Data.GetCharacterDataByID(player, charid);
            HeadBlend h = new HeadBlend();
            h.ShapeFirst = character.Appearance.Parent1Face;
            h.ShapeSecond = character.Appearance.Parent2Face;
            h.ShapeThird = character.Appearance.Parent3Face;
            h.SkinFirst = character.Appearance.Parent1Skin;
            h.SkinSecond = character.Appearance.Parent2Skin;
            h.SkinThird = character.Appearance.Parent3Skin;
            h.ShapeMix = character.Appearance.FaceMix;
            h.SkinMix = character.Appearance.SkinMix;
            h.ThirdMix = character.Appearance.OverrideMix;
            float[] FaceFeatures = character.Appearance.GetFaceFeatures();
            Dictionary<int, HeadOverlay> overlays = character.Appearance.GetHeadOverlays();
            NAPI.Task.Run(() =>
            {
                Decoration[] decor = new Decoration[0];
                player.SetCustomization(character.Appearance.Gender, h, character.Appearance.EyeColor, character.Appearance.HairColor, character.Appearance.HairHighlight, FaceFeatures, overlays, decor);
            });
        }

    }
}
