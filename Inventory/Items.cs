using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using Server.Characters;

namespace Server.Inventory
{
    class Container
    {
        public uint ItemID { get; }//ITEMID
        public int Drawable { get; } = -1;//DRAWABLE, ha ruháról beszélünk pl
        public uint Capacity { get; }
        public Container(uint itemid, int drawable, uint capacity) {
            ItemID = itemid;
            Drawable = drawable; 
            Capacity = capacity;
        }

        public Container(uint itemid, uint capacity)
        {
            ItemID = itemid;
            Capacity = capacity;
        }
    }

    public class GroundItem
    {
        public uint ID { get; set; }
        public uint Item_DBID { get;  }
        public GTANetworkAPI.Object? Object { get; set; }
        public TextLabel? Label { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public uint Dimension { get; set; }
        public DateTime ThrownAwayTime { get; set; }
        public uint ThrownAwayBy { get; set; }
        public DateTime PickUpTime { get; set; } = DateTime.Now;
        public uint PickedUpBy { get; set; } = 0;

        public GroundItem(uint id, uint itemid, Vector3 pos, Vector3 rot, uint dim, DateTime thrownaway, uint thrownawayby, DateTime pickedup, uint pickedupby)
        {
            ID = id;
            Item_DBID = itemid;
            Position = pos;
            Rotation = rot;
            Dimension = dim;
            ThrownAwayTime = thrownaway;
            ThrownAwayBy = thrownawayby;
            PickUpTime = pickedup;
            PickedUpBy = pickedupby;
        }

        public GroundItem(uint id, uint itemid, Vector3 pos, Vector3 rot, uint dim, DateTime thrownaway, uint thrownawayby)
        {
            ID = id;
            Item_DBID = itemid;
            Position = pos;
            Rotation = rot;
            Dimension = dim;
            ThrownAwayTime = thrownaway;
            ThrownAwayBy = thrownawayby;
        }

        public async Task CreateObject(string objectname)
        {
            Item i = await Items.LoadItemFromGround(Item_DBID);
            string szoveg = "";



            if (Items.IsItemContainer(i.ItemID))
            {
                szoveg = "Bal klikk: Felvétel | Jobb klikk: Megnyitás";
            }
            else
            {
                szoveg = "Bal klikk: Felvétel";
            }

            NAPI.Task.Run(() =>
            {
                Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objectname), Position, Rotation, 255, Dimension);
                Object.Rotation = Rotation;
                Vector3 labelPos = Position;
                labelPos.Z += 0.2f;

                Label = NAPI.TextLabel.CreateTextLabel(szoveg, Position, 1.5f, 1f, 4, new Color(255, 255, 255, 200), true, Dimension);
                NAPI.Task.Run(() =>
                {
                    
                    Object.SetSharedData("object:ID", ID);
                }, 500);
            }, 250);
        }

        [ServerEvent(Event.Update)]

        public async Task CreateObject(string objectname, string szoveg, uint itemid)
        { 
            NAPI.Task.Run(() =>
            {
                Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objectname), Position, Rotation, 255, Dimension);
                Vector3 labelPos = Position;
                labelPos.Z += 0.2f;

                Label = NAPI.TextLabel.CreateTextLabel(szoveg, Position, 1.5f, 1f, 4, new Color(255, 255, 255, 200), true, Dimension);
                NAPI.Task.Run(() =>
                {

                    Object.SetSharedData("object:ID", ID);
                }, 500);
            }, 250);
        }
    }

    static class Gloves
    {
        //11 féle kesztyű van, férfi - nő verzióban
        //nem (true/false), bemeneti torso, kesztyű típus (0-10, 11db) -> kimenet pedig a megfelelelő torso ha van, vagy -1 ha nincs
        static Dictionary<int, int> FemaleBlackLeatherOpen = new Dictionary<int, int>//0
        {
            { 0, 20 },
            { 1, 21 },
            { 2, 22 },
            { 3, 23 },
            { 4, 24 },
            { 5, 25 },
            { 6, 26 },
            { 7, 27 },
            { 9, 28 },
            { 11, 29 },
            { 12, 30 },
            { 14, 31 },
            { 15, 32 },
            { 129, 132 },
            { 130, 139 },
            { 131, 146 },
            { 153, 154 },
            { 161, 162 },
            { 229, 230 },
        };
        static Dictionary<int, int> FemaleBlackLeatherFull = new Dictionary<int, int>//1
        {
            { 0, 33 },
            { 1, 34 },
            { 2, 35 },
            { 3, 36 },
            { 4, 37 },
            { 5, 38 },
            { 6, 39 },
            { 7, 40 },
            { 9, 41 },
            { 11, 42 },
            { 12, 43 },
            { 14, 44 },
            { 15, 45 },
            { 129, 133 },
            { 130, 140 },
            { 131, 147 },
            { 153, 155 },
            { 161, 163 },
            { 229, 231 },
        };
        static Dictionary<int, int> FemaleBlackFabricFull = new Dictionary<int, int>//1
        {
            { 0, 46 },
            { 1, 47 },
            { 2, 48 },
            { 3, 49 },
            { 4, 50 },
            { 5, 51 },
            { 6, 52 },
            { 7, 53 },
            { 9, 54 },
            { 11, 55 },
            { 12, 56 },
            { 14, 57 },
            { 15, 58 },
            { 129, 134 },
            { 130, 141 },
            { 131, 148 },
            { 153, 156 },
            { 161, 164 },
            { 229, 232 },
        };
        static Dictionary<int, int> FemaleBlackFabricFingerless = new Dictionary<int, int>//1
        {
            { 0, 59 },
            { 1, 60 },
            { 2, 61 },
            { 3, 62 },
            { 4, 63 },
            { 5, 64 },
            { 6, 65 },
            { 7, 66 },
            { 9, 67 },
            { 11, 68 },
            { 12, 69 },
            { 14, 70 },
            { 15, 71 },
            { 129, 135 },
            { 130, 142 },
            { 131, 149 },
            { 153, 157 },
            { 161, 166 },
            { 229, 233 },
        };
        static Dictionary<int, int> FemaleYellowWorkerFull = new Dictionary<int, int>//1
        {
            { 0, 72 },
            { 1, 73 },
            { 2, 74 },
            { 3, 75 },
            { 4, 76 },
            { 5, 77 },
            { 6, 78 },
            { 7, 79 },
            { 9, 80 },
            { 11, 81 },
            { 12, 82 },
            { 14, 83 },
            { 15, 84 },
            { 129, 136 },
            { 130, 143 },
            { 131, 150 },
            { 153, 158 },
            { 161, 166 },
            { 229, 234 },
        };
        static Dictionary<int, int> FemaleWhiteLeatherFull = new Dictionary<int, int>//1
        {
            { 0, 85 },
            { 1, 86 },
            { 2, 87 },
            { 3, 88 },
            { 4, 89 },
            { 5, 90 },
            { 6, 91 },
            { 7, 92 },
            { 9, 93 },
            { 11, 94 },
            { 12, 95 },
            { 14, 96 },
            { 15, 97 },
            { 129, 137 },
            { 130, 144 },
            { 131, 151 },
            { 153, 159 },
            { 161, 167 },
            { 229, 235 },
        };
        static Dictionary<int, int> FemaleBlueLatexFull = new Dictionary<int, int>//1
        {
            { 0, 98 },
            { 1, 99 },
            { 2, 100 },
            { 3, 101 },
            { 4, 102 },
            { 5, 103 },
            { 6, 104 },
            { 7, 105 },
            { 9, 106 },
            { 11, 107 },
            { 12, 108 },
            { 14, 109 },
            { 15, 110 },
            { 129, 138 },
            { 130, 145 },
            { 131, 152 },
            { 153, 160 },
            { 161, 168 },
            { 229, 236 },
        };
        static Dictionary<int, int> FemaleGreenFabricFull = new Dictionary<int, int>//nem támogat minden torsot
        {
            { 0, 114 },
            { 1, 115 },
            { 2, 116 },
            { 3, 117 },
            { 4, 118 },
            { 5, 119 },
            { 6, 120 },
            { 7, 121 },
            { 9, 122 },
            { 11, 123 },
            { 12, 124 },
            { 14, 125 },
            { 15, 126 }
        };
        static Dictionary<int, int> FemaleLightBlueSportFull = new Dictionary<int, int>
        {
            { 0, 187 },
            { 1, 188 },
            { 2, 189 },
            { 3, 190 },
            { 4, 191 },
            { 5, 192 },
            { 6, 193 },
            { 7, 194 },
            { 9, 195 },
            { 11, 196 },
            { 12, 197 },
            { 14, 198 },
            { 15, 170 },
            { 129, 199 },
            { 130, 200 },
            { 131, 201 },
            { 153, 202 },
            { 161, 204 },
            { 229, 238 },
        };
        static Dictionary<int, int> FemaleDarkBlueSportFull = new Dictionary<int, int>
        {
            { 0, 171 },
            { 1, 172 },
            { 2, 173 },
            { 3, 174 },
            { 4, 175 },
            { 5, 176 },
            { 6, 177 },
            { 7, 178 },
            { 9, 179 },
            { 11, 180 },
            { 12, 181 },
            { 14, 182 },
            { 15, 169 },
            { 129, 183 },
            { 130, 184 },
            { 131, 185 },
            { 153, 186 },
            { 161, 203 },
            { 229, 237 },
        };
        static Dictionary<int, int> FemaleBlackSportFull = new Dictionary<int, int>
        {
            { 0, 212 },
            { 1, 213 },
            { 2, 214 },
            { 3, 215 },
            { 4, 216 },
            { 5, 217 },
            { 6, 218 },
            { 7, 219 },
            { 9, 220 },
            { 11, 221 },
            { 12, 222 },
            { 14, 223 },
            { 15, 211 },
            { 129, 224 },
            { 130, 225 },
            { 131, 226 },
            { 153, 227 },
            { 161, 228 },
            { 229, 239 },
        };

        static Dictionary<int, int> MaleBlackLeatherOpen = new Dictionary<int, int>//0
        {
            { 0, 19 },
            { 1, 20 },
            { 2, 21 },
            { 4, 22 },
            { 5, 23 },
            { 6, 24 },
            { 8, 25 },
            { 11, 26 },
            { 12, 27 },
            { 14, 38 },
            { 15, 39 },
            { 112, 115 },
            { 113, 122 },
            { 114, 129 },
            { 184, 185 },
        };
        static Dictionary<int, int> MaleBlackLeatherFull = new Dictionary<int, int>//0
        {
            { 0, 30 },
            { 1, 31 },
            { 2, 32 },
            { 4, 33 },
            { 5, 34 },
            { 6, 35 },
            { 8, 36 },
            { 11, 37 },
            { 12, 38 },
            { 14, 39 },
            { 15, 40 },
            { 112, 116 },
            { 113, 123 },
            { 114, 130 },
            { 184, 186 },
        };
        static Dictionary<int, int> MaleBlackFabricFull = new Dictionary<int, int>//0
        {
            { 0, 41 },
            { 1, 42 },
            { 2, 43 },
            { 4, 44 },
            { 5, 45 },
            { 6, 46 },
            { 8, 47 },
            { 11, 48 },
            { 12, 49 },
            { 14, 50 },
            { 15, 51 },
            { 112, 117 },
            { 113, 124 },
            { 114, 131 },
            { 184, 187 },
        };
        static Dictionary<int, int> MaleBlackFabricFingerless = new Dictionary<int, int>//0
        {
            { 0, 52 },
            { 1, 53 },
            { 2, 54 },
            { 4, 55 },
            { 5, 56 },
            { 6, 57 },
            { 8, 58 },
            { 11, 59 },
            { 12, 60 },
            { 14, 61 },
            { 15, 62 },
            { 112, 118 },
            { 113, 125 },
            { 114, 132 },
            { 184, 188 },
        };
        static Dictionary<int, int> MaleYellowWorkerFull = new Dictionary<int, int>//0
        {
            { 0, 63 },
            { 1, 64 },
            { 2, 65 },
            { 4, 66 },
            { 5, 67 },
            { 6, 68 },
            { 8, 69 },
            { 11, 70 },
            { 12, 71 },
            { 14, 72 },
            { 15, 73 },
            { 112, 119 },
            { 113, 126 },
            { 114, 133 },
            { 184, 189 },
        };
        static Dictionary<int, int> MaleWhiteLeatherFull = new Dictionary<int, int>
        {
            { 0, 74 },
            { 1, 75 },
            { 2, 76 },
            { 4, 77 },
            { 5, 78 },
            { 6, 79 },
            { 8, 80 },
            { 11, 81 },
            { 12, 82 },
            { 14, 83 },
            { 15, 84 },
            { 112, 120 },
            { 113, 127 },
            { 114, 134 },
            { 184, 190 },
        };
        static Dictionary<int, int> MaleBlueLatexFull = new Dictionary<int, int>//0
        {
            { 0, 85 },
            { 1, 86 },
            { 2, 87 },
            { 4, 88 },
            { 5, 89 },
            { 6, 90 },
            { 8, 91 },
            { 11, 92 },
            { 12, 93 },
            { 14, 94 },
            { 15, 95 },
            { 112, 121 },
            { 113, 128 },
            { 114, 135 },
            { 184, 191 },
        };
        static Dictionary<int, int> MaleGreenFabricFull = new Dictionary<int, int>//0
        {
            { 0, 99 },
            { 1, 100 },
            { 2, 101 },
            { 4, 102 },
            { 5, 103 },
            { 6, 104 },
            { 8, 105 },
            { 11, 106 },
            { 12, 107 },
            { 14, 108 },
            { 15, 109 },
        };
        static Dictionary<int, int> MaleLightBlueSportFull = new Dictionary<int, int>//0
        {
            { 0, 151 },
            { 1, 152 },
            { 2, 153 },
            { 4, 154 },
            { 5, 155 },
            { 6, 156 },
            { 8, 157 },
            { 11, 158 },
            { 12, 159 },
            { 14, 160 },
            { 15, 137 },
            { 112, 161 },
            { 113, 162 },
            { 114, 163 },
            { 184, 193 },
        };
        static Dictionary<int, int> MaleDarkBlueSportFull = new Dictionary<int, int>
        {
            { 0, 138 },
            { 1, 139 },
            { 2, 140 },
            { 4, 141 },
            { 5, 142 },
            { 6, 143 },
            { 8, 144 },
            { 11, 145 },
            { 12, 146 },
            { 14, 147 },
            { 15, 136 },
            { 112, 148 },
            { 113, 149 },
            { 114, 150 },
            { 184, 192 },
        };
        static Dictionary<int, int> MaleBlackSportFull = new Dictionary<int, int>//0
        {
            { 0, 171 },
            { 1, 172 },
            { 2, 173 },
            { 4, 174 },
            { 5, 175 },
            { 6, 176 },
            { 8, 177 },
            { 11, 178 },
            { 12, 179 },
            { 14, 180 },
            { 15, 170 },
            { 112, 181 },
            { 113, 182 },
            { 114, 183 },
            { 184, 194 },
        };

        public static int GetCorrectTorsoForGloves(bool gender, int torso, int glove)
        {
            int correctglove = -1;
            if(gender)//true -> férfi
            {
                //meg kell találni a torsohoz tartozó kesztyűt
                switch (glove)
                {
                    case 0:
                        if (MaleBlackLeatherOpen.ContainsKey(torso))
                        {
                            correctglove = MaleBlackLeatherOpen[torso];
                        }
                        break;
                    case 1:
                        if (MaleBlackLeatherFull.ContainsKey(torso))
                        {
                            correctglove = MaleBlackLeatherFull[torso];
                        }
                        break;
                    case 2:
                        if (MaleBlackFabricFull.ContainsKey(torso))
                        {
                            correctglove = MaleBlackFabricFull[torso];
                        }
                        break;
                    case 3:
                        if (MaleBlackFabricFingerless.ContainsKey(torso))
                        {
                            correctglove = MaleBlackFabricFingerless[torso];
                        }
                        break;
                    case 4:
                        if (MaleYellowWorkerFull.ContainsKey(torso))
                        {
                            correctglove = MaleYellowWorkerFull[torso];
                        }
                        break;
                    case 5:
                        if (MaleWhiteLeatherFull.ContainsKey(torso))
                        {
                            correctglove = MaleWhiteLeatherFull[torso];
                        }
                        break;
                    case 6:
                        if (MaleBlueLatexFull.ContainsKey(torso))
                        {
                            correctglove = MaleBlueLatexFull[torso];
                        }
                        break;
                    case 7:
                        if (MaleGreenFabricFull.ContainsKey(torso))
                        {
                            correctglove = MaleGreenFabricFull[torso];
                        }
                        break;
                    case 8:
                        if (MaleLightBlueSportFull.ContainsKey(torso))
                        {
                            correctglove = MaleLightBlueSportFull[torso];
                        }
                        break;
                    case 9:
                        if (MaleDarkBlueSportFull.ContainsKey(torso))
                        {
                            correctglove = MaleDarkBlueSportFull[torso];
                        }
                        break;
                    case 10:
                        if (MaleBlackSportFull.ContainsKey(torso))
                        {
                            correctglove = MaleBlackSportFull[torso];
                        }
                        break;
                    default:
                        break;
                }
            }
            else//nő
            {
                switch (glove)
                {
                    case 0:
                        if (FemaleBlackLeatherOpen.ContainsKey(torso))
                        {
                            correctglove = FemaleBlackLeatherOpen[torso];
                        }
                        break;
                    case 1:
                        if (FemaleBlackLeatherFull.ContainsKey(torso))
                        {
                            correctglove = FemaleBlackLeatherFull[torso];
                        }
                        break;
                    case 2:
                        if (FemaleBlackFabricFull.ContainsKey(torso))
                        {
                            correctglove = FemaleBlackFabricFull[torso];
                        }
                        break;
                    case 3:
                        if (FemaleBlackFabricFingerless.ContainsKey(torso))
                        {
                            correctglove = FemaleBlackFabricFingerless[torso];
                        }
                        break;
                    case 4:
                        if (FemaleYellowWorkerFull.ContainsKey(torso))
                        {
                            correctglove = FemaleYellowWorkerFull[torso];
                        }
                        break;
                    case 5:
                        if (FemaleWhiteLeatherFull.ContainsKey(torso))
                        {
                            correctglove = FemaleWhiteLeatherFull[torso];
                        }
                        break;
                    case 6:
                        if (FemaleBlueLatexFull.ContainsKey(torso))
                        {
                            correctglove = FemaleBlueLatexFull[torso];
                        }
                        break;
                    case 7:
                        if (FemaleGreenFabricFull.ContainsKey(torso))
                        {
                            correctglove = FemaleGreenFabricFull[torso];
                        }
                        break;
                    case 8:
                        if (FemaleLightBlueSportFull.ContainsKey(torso))
                        {
                            correctglove = FemaleLightBlueSportFull[torso];
                        }
                        break;
                    case 9:
                        if (FemaleDarkBlueSportFull.ContainsKey(torso))
                        {
                            correctglove = FemaleDarkBlueSportFull[torso];
                        }
                        break;
                    case 10:
                        if (FemaleBlackSportFull.ContainsKey(torso))
                        {
                            correctglove = FemaleBlackSportFull[torso];
                        }
                        break;
                    default:
                        break;
                }
            }
            return correctglove;
        }
    }

    public class Items : Script
    {
        static List<Container> Containers = new List<Container>//ITEMID, DRAWABLE(ha ruha), KAPACITÁS/SÚLY
        {
            new Container(11,82,15000)
        };
        /*
         * {"model":"mp_f_freemode_01", "dlc":"mp2023_01", "components":[45, 226, 84, 244, 190, 110, 141, 144, 244, 57, 190, 533], "props":[193, 54, 22, null, null, null, 35, 20]},
{"model":"mp_m_freemode_01", "dlc":"mp2023_01", "components":[45, 225, 80, 210, 176, 110, 134, 174, 198, 57, 173, 494], "props":[194, 52, 41, null, null, null, 46, 13]}*/

        static Dictionary<int, int> MaleClothingOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 45 },
            { 1, 225 },
            { 2, 80 },
            { 3, 210 },
            { 4, 176 },
            { 5, 110 },
            { 6, 134 },
            { 7, 174 },
            { 8, 198 },
            { 9, 57 },
            { 10, 173 },
            { 11, 494 }
        };

        static Dictionary<int, int> MaleAccessoryOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 194 },
            { 1, 52 },
            { 2, 41 },
            { 6, 46 },
            { 7, 13 }
        };

        static Dictionary<int, int> FemaleAccessoryOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 193 },
            { 1, 54 },
            { 2, 22 },
            { 6, 35 },
            { 7, 20 }
        };

        static Dictionary<int, int> FemaleClothingOffsets = new Dictionary<int, int>
        {
            //COMPONENET ID - OFFSET
            { 0, 45 },
            { 1, 226 },
            { 2, 84 },
            { 3, 244 },
            { 4, 190 },
            { 5, 110 },
            { 6, 141 },
            { 7, 144 },
            { 8, 244 },
            { 9, 57 },
            { 10, 190 },
            { 11, 533 }
        };

        public void SetClothingOffset(Player player, bool gender, int component, int offset)
        {
            if (gender)//férfi
            {
                MaleClothingOffsets[component] = offset;
            }
            else
            {
                FemaleClothingOffsets[component] = offset;
            }
        }

        public void SetAccessoryOffset(Player player, bool gender, int component, int offset)
        {
            if (gender)//férfi
            {
                MaleAccessoryOffsets[component] = offset;
            }
            else
            {
                FemaleAccessoryOffsets[component] = offset;
            }
        }

        public static int GetCorrectClothing(bool gender, int component, int drawable)//ha negatív szám akkor visszaadja a jó drawablet
        {
            if (gender)//férfi
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return MaleClothingOffsets[component]+Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return FemaleClothingOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
        }
        public static int GetCorrectAccessory(bool gender, int component, int drawable)//ha negatív szám akkor visszaadja a jó drawablet
        {
            if (gender)//férfi
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return MaleAccessoryOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable < 0)//negatív, modolt -> offset + abszolút érték
                {
                    return FemaleAccessoryOffsets[component] + Math.Abs(drawable);
                }
                else
                {
                    return drawable;
                }
            }
        }



        public int DrawableToModdedClothing(bool gender, int component, int drawable)
        {
            if (gender)//férfi
            {
                if (drawable > MaleClothingOffsets[component])
                {
                    return -(drawable - MaleClothingOffsets[component]);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable > FemaleClothingOffsets[component])
                {
                    return -(drawable - FemaleClothingOffsets[component]);
                }
                else
                {
                    return drawable;
                }
            }
        }

        public int DrawableToModdedAccessory(bool gender, int component, int drawable)
        {
            if (gender)//férfi
            {
                if (drawable > MaleAccessoryOffsets[component])
                {
                    return -(drawable - MaleAccessoryOffsets[component]);
                }
                else
                {
                    return drawable;
                }
            }
            else
            {
                if (drawable > FemaleAccessoryOffsets[component])
                {
                    return -(drawable - FemaleAccessoryOffsets[component]);
                }
                else
                {
                    return drawable;
                }
            }
        }


        //MASZK female: 226
        //FELSŐ: 533
        static Dictionary<Tuple<int, uint>, bool> OpenedInventories = new Dictionary<Tuple<int, uint>, bool>();

        static Dictionary<Tuple<int, uint>, List<Item>> Inventories = new Dictionary<Tuple<int, uint>, List<Item>>();

        static List<GroundItem> GroundItems = new List<GroundItem>();
        //OWNERTYPE-ok:
        /*
        0 - JÁTÉKOS
        1 - ITEM TÁROLÓ
        2 - JÁRMŰ CSOMAGTARTÓ
        3 - JÁRMŰ KESZTYŰTARTÓ
        4 - INTERIOR
        5 - OBJECT -> meg lehet nyitni rá kattintva
        6 - LERAKOTT ITEM TÁROLÓ (OBJECT) -> megnyithatod és fel is veheted a földről
          
         */

        /*
        TÁROLÓKNÁL:
        ITEMID-t összehasonlítjuk
        ha itemid megfelel akkor esetleg itemvalue-t -> jó drawable? pl táskánál

        */


        public async static void PopulateGroundItems()
        {
            DateTime timestamp1 = DateTime.Now;

            await LoadGroundItems();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput(GroundItems.Count + " db eldobott item betöltve " + LoadTime.Milliseconds + " ms alatt.");
            //TODO: indításnál ellenőrizze mióta van a földön az adott object és vegye fel ha régóta
        }



        public async static Task LoadGroundItems()
        {
            string query = $"SELECT * FROM `grounditems` WHERE `pickupBy` IS NULL";//csak azokat választjuk ki amik nem lettek még felszedve, tehát aktívak
            GroundItems.Clear();
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Prepare();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                GroundItem gi;
                                if (reader["pickupDate"] == System.DBNull.Value || reader["pickupBy"] == System.DBNull.Value)
                                {
                                    gi = new GroundItem(Convert.ToUInt32(reader["id"]), Convert.ToUInt32(reader["item_DbID"]), new Vector3(Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"])), new Vector3(Convert.ToSingle(reader["rotX"]), Convert.ToSingle(reader["rotY"]), Convert.ToSingle(reader["rotZ"])), Convert.ToUInt32(reader["dim"]), Convert.ToDateTime(reader["thrownDate"]), Convert.ToUInt32(reader["thrownBy"]));
                                }
                                else
                                {
                                    gi = new GroundItem(Convert.ToUInt32(reader["id"]), Convert.ToUInt32(reader["item_DbID"]), new Vector3(Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"])), new Vector3(Convert.ToSingle(reader["rotX"]), Convert.ToSingle(reader["rotY"]), Convert.ToSingle(reader["rotZ"])), Convert.ToUInt32(reader["dim"]), Convert.ToDateTime(reader["thrownDate"]), Convert.ToUInt32(reader["thrownBy"]), Convert.ToDateTime(reader["pickupDate"]), Convert.ToUInt32(reader["pickupBy"]));
                                }
                                
                                //létrehozunk egy új földön lévő itemet
                                uint itemid = await LoadItemIDByDatabaseID(gi.Item_DBID);
                                //megszerezzük az adatbázis id alapján az itemid-t

                                await gi.CreateObject(ItemList.GetItemObject(itemid));
                                //az itemid alapján már tudjuk, hogy milyen objectet kéne leraknunk, azt létre is hozzuk
                                GroundItems.Add(gi);
                                //végül hozzáadjuk a földön lévő tárgyak listájához
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

                con.CloseAsync();

            }
        }


        public async static Task<uint> LoadItemIDByDatabaseID(uint dbid)
        {
            uint loaded_itemid = 0;
            string query = $"SELECT * FROM `items` WHERE `DbID` LIKE @DBID";
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DBID", dbid);
                        cmd.Prepare();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                loaded_itemid = loadedItem.ItemID;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

                con.CloseAsync();

            }
            return loaded_itemid;
        }

        public async static Task<bool> PickupGroundItem(GroundItem item)
        {
            bool state = false;
            string query = $"UPDATE `grounditems` SET `pickupDate` = @PickupDate, `pickupBy` = @PickupPlayer  WHERE `grounditems`.`id` = @ID;";
            //string query2 = $"UPDATE `characters` SET `characterName` = @CharacterName, `dob` = @DOB, `pob` = @POB WHERE `appearanceId` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ID", item.ID);
                    command.Parameters.AddWithValue("@PickupPlayer", item.PickedUpBy);
                    command.Parameters.AddWithValue("@PickupDate", item.PickUpTime);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }
            return state;
        }

        
        public async static Task<uint> AddGroundItemToDatabase(GroundItem itemdata)//létrehozunk egy új itemet az adatbázisban
        {
            uint GroundItemID = 0;
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            string query = $"INSERT INTO `grounditems` " +
                $"(`item_DbID`, `posX`, `posY`, `posZ`, `rotX`, `rotY`, `rotZ`, `dim`, `thrownDate`, `thrownBy`)" +
                $" VALUES " +
                $"(@ItemDBID, @PosX, @PosY, @PosZ, @RotX, @RotY, @RotZ, @Dim, @ThrownDate, @ThrownBy)";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ItemDBID", itemdata.Item_DBID);
                    command.Parameters.AddWithValue("@PosX", itemdata.Position.X);
                    command.Parameters.AddWithValue("@PosY", itemdata.Position.Y);
                    command.Parameters.AddWithValue("@PosZ", itemdata.Position.Z);
                    command.Parameters.AddWithValue("@RotX", itemdata.Rotation.X);
                    command.Parameters.AddWithValue("@RotY", itemdata.Rotation.Y);
                    command.Parameters.AddWithValue("@RotZ", itemdata.Rotation.Z);
                    command.Parameters.AddWithValue("@Dim", itemdata.Dimension);
                    command.Parameters.AddWithValue("@ThrownDate", itemdata.ThrownAwayTime);
                    command.Parameters.AddWithValue("@ThrownBy", itemdata.ThrownAwayBy);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            long lastid = command.LastInsertedId;
                            GroundItemID = Convert.ToUInt32(lastid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
            return GroundItemID;
        }
        

        public static bool IsInventoryInUse(int OwnerType, uint OwnerID)
        {
            if(OpenedInventories.ContainsKey(new Tuple<int, uint>(OwnerType, OwnerID)))
                {
                if (OpenedInventories[new Tuple<int, uint>(OwnerType, OwnerID)] == true)//használatban van az adott inventory
                {
                    return true;
                }
                else//nincs használatban
                {
                    return false;
                }
            }
            else//nincs benne a kulcs szóval nincs megnyitva
            {
                return false;
            }
        }

        public static void SetInventoryInUse(int OwnerType, uint OwnerID, bool state)
        {
            OpenedInventories[new Tuple<int, uint>(OwnerType, OwnerID)] = state;
        }

        public static List<Item> GetInventory(int OwnerType, uint OwnerID)
        {
            return Inventories[new Tuple<int, uint>(OwnerType, OwnerID)];
        }

        public static bool IsItemContainer(uint itemid)
        {
            foreach (var item in Containers)
            {
                if (item.ItemID == itemid)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsItemContainer(uint itemid, int drawable)
        {
            foreach (var item in Containers)
            {
                if (item.ItemID == itemid && item.Drawable == drawable)
                {
                    return true;
                }
            }
            return false;
        }


        public static async Task SetInventory(int OwnerType, uint OwnerID, Item[] items)
        {
            Inventories[new Tuple<int, uint>(OwnerType, OwnerID)] = items.ToList();
        }


        public async static Task AddItemToInventory(Player player, int OwnerType, uint OwnerID, Item item)
        {
            foreach (var inv in Inventories)
            {
                if (inv.Value.Contains(item))
                {
                    inv.Value.Remove(item);
                }
            }
            Inventories[new Tuple<int, uint>(OwnerType, OwnerID)].Add(item);
            await SortInventory(player, OwnerType, OwnerID);
        }

        public static void RemoveItemFromInventory(int OwnerType, uint OwnerID, Item item)
        {
            Inventories[new Tuple<int, uint>(OwnerType, OwnerID)].Remove(item);
        }

        public static void AddItemToPlayerInventory(Player player, Item item)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            Inventories[new Tuple<int, uint>(0, charid)].Add(item);
        }
        public static void RemoveItemFromPlayerInventory(Player player, Item item)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            Inventories[new Tuple<int, uint>(0, charid)].Remove(item);
        }

        public static void SetPlayerInventory(Player player, List<Item> items)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            Inventories[new Tuple<int, uint>(0, charid)] = items;
        }

        public static List<Item> GetPlayerInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            if (charid != 0)
            {
                return Inventories[new Tuple<int, uint>(0, charid)];
            }
            return null;
        }

        public static bool IsInventoryLoaded(int OwnerType, uint OwnerID)
        {
            if (Inventories.ContainsKey(new Tuple<int, uint>(OwnerType, OwnerID)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //játékos 20.000 gramm (20 kg)
        //többi dolgot még át kell gondolni
        public async static Task<uint> GetInventoryWeight(int OwnerType, uint OwnerID)
        {
            if (Inventories.ContainsKey(new Tuple<int, uint>(OwnerType, OwnerID)))
            {
                //létezik az inventory
                uint weight = 0;
                foreach (var item in GetInventory(OwnerType,OwnerID))
                {
                    if (IsItemContainer(item.ItemID))//ha tároló
                    {
                        if (item.InUse)//használatban van (táska felvéve), ne számolja a benne lévő dolgok súlyát, nincs más teendőnk
                        {
                            if (item.ItemID <= 27)
                            {
                                weight += Convert.ToUInt32(ItemList.GetItemWeight(item.ItemID) * item.ItemAmount / 2);
                            }
                            else
                            {
                                weight += Convert.ToUInt32(ItemList.GetItemWeight(item.ItemID) * item.ItemAmount);
                            }
                            
                        }
                        else//nincs felvéve, tehát inventory-ban van táska pl, hozzá akaruk számolni a táska súlyt
                        {
                            if (IsInventoryLoaded(1, item.DBID))//be van töltve, vissza tudjuk adni neki
                            {
                                List<Item> containeritems = GetInventory(1, item.DBID);
                                uint containerweight = 0;
                                foreach (var containeritem in containeritems)
                                {
                                    containerweight += Convert.ToUInt32(ItemList.GetItemWeight(containeritem.ItemID) * containeritem.ItemAmount);
                                }
                                if (item.ItemID <= 27)
                                {
                                    weight += Convert.ToUInt32((ItemList.GetItemWeight(item.ItemID) * item.ItemAmount / 2) + containerweight);
                                }
                                else
                                {
                                    weight += Convert.ToUInt32((ItemList.GetItemWeight(item.ItemID) * item.ItemAmount) + containerweight);
                                } 
                            }
                            else//nincs betöltve a tároló, be kell tölteni és utána visszaadni
                            {
                                Item[] ContainerItems = await LoadContainer(item.DBID);
                                uint containerweight = 0;
                                foreach (var containeritem in ContainerItems)
                                {
                                    if (item.InUse)
                                    {
                                        item.InUse = false;
                                    }
                                    containerweight += Convert.ToUInt32(ItemList.GetItemWeight(containeritem.ItemID) * containeritem.ItemAmount);
                                }
                                await SetInventory(1, item.DBID, ContainerItems);//ha már betöltöttük akkor menthetjük is memóriába
                                if (item.ItemID <= 27)
                                {
                                    weight += Convert.ToUInt32((ItemList.GetItemWeight(item.ItemID) * item.ItemAmount / 2) + containerweight);
                                }
                                else
                                {
                                    weight += Convert.ToUInt32((ItemList.GetItemWeight(item.ItemID) * item.ItemAmount) + containerweight);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item.ItemID <= 27 && item.InUse)
                        {
                            weight += Convert.ToUInt32(ItemList.GetItemWeight(item.ItemID) * item.ItemAmount / 2);
                        }
                        else
                        {
                            weight += Convert.ToUInt32(ItemList.GetItemWeight(item.ItemID) * item.ItemAmount);
                        }
                    }
                }
                return weight;
            }
            else
            {
                return 0;
            }
        }

        public static async Task<uint> GetContainerWeight(uint container_dbid)
        {
            uint weight = 0;
            string query = $"SELECT * FROM `items` WHERE `OwnerID` = @OwnerID AND `ownerType` = 1";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OwnerID", container_dbid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item item = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                    items.Add(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                //kiszámoljuk az inventory súlyát

                foreach (var item in items)
                {
                    weight += Convert.ToUInt32(ItemList.GetItemWeight(item.ItemID) * item.ItemAmount);
                }
                return weight;
            }
        }


        public static uint GetInventoryCapacity(int OwnerType)
        {
            switch (OwnerType)
            {
                case 0://játékos 15.000 g = 15 kg
                    return 15000;
                    break;
                case 1://tároló - külön kezelés
                    return 15000;
                    break;
                case 2://játékos 15.000 g = 15 kg
                    return 30000;
                    break;
                case 3://játékos 15.000 g = 15 kg
                    return 10000;
                    break;
                case 4://játékos 15.000 g = 15 kg
                    return 50000;
                    break;
                case 5://játékos 15.000 g = 15 kg
                    return 30000;
                    break;
                case 6://játékos 15.000 g = 15 kg
                    return 20000;
                    break;
                default:
                    break;
            }
            //TODO: kitalálni hogy mi alapján kapjon kapacitást
            //le lehet küldeni majd a GetInventoryWeight - meg ennek a függvénynek az eredményét grafikus nézet miatt
            return 0;
        }


        public async static Task<bool> HasSpaceForItem(int OwnerType, uint OwnerID, uint ItemWeight)
        {
            if (await GetInventoryWeight(OwnerType, OwnerID) + ItemWeight <= GetInventoryCapacity(OwnerType))
            {
                return true;
            }
            else return false;
        }

        public static bool HasItemWithValue(Player player, uint itemid, string itemvalue)
        {
            foreach (var item in GetPlayerInventory(player))
            {
                if (item.ItemID == itemid && item.ItemValue == itemvalue)
                {
                    return true;
                }
            }
            return false;
        }



        [Command("giveitem")]
        public async static void GiveItem(Player player, int targetid, uint itemid, string itemvalue, int amount)
        {
            Player target = Admin.Commands.GetPlayerById(targetid);
            /*
            Clothing c = new Clothing(0, 0);
            Top t = new Top(0, 0, 0, 0, 0);
            Database.Log.Log_Server(NAPI.Util.ToJson(c));
            Database.Log.Log_Server(NAPI.Util.ToJson(t));
            */
            uint charid = target.GetData<UInt32>("Player:CharID");
            Item i = new Item(0, charid, 0, itemid, itemvalue, amount, false, false, 1000);

            uint dbid = await AddItemToDatabase(charid, i);

            if (dbid != 0)
            {
                i.DBID = dbid;
                GetPlayerInventory(target).Add(i);//hozzáadjuk a szerver itemjeihez
                NAPI.Task.Run(() =>
                {
                    string json = NAPI.Util.ToJson(i);
                    target.TriggerEvent("client:AddItemToInventory", json);
                    player.SendChatMessage("Adtál " + amount + " db " + ItemList.GetItemName(i.ItemID) + " tárgyat " + target.Name + " játékosnak!");
                    target.SendChatMessage("Kaptál " + amount + " db " + ItemList.GetItemName(i.ItemID) + " tárgyat " + player.Name + " -tól!");
                }, 500);
            }
            //Item newitem = new Item(0, charid, 0, itemid, itemvalue, amount, false, -1);
        }


        [Command("takeclothingpictures")]
        public async void SetupItemPictures(Player player, bool gender)
        {
            player.Dimension = 9873;
            player.SetSharedData("Player:Frozen", true);
            player.SetSharedData("Player:Invisible", true);
            player.Position = new Vector3(228.6f, -989.3f, -98.5f);
            player.Rotation = new Vector3(0f, 0f, 0f);
            player.TriggerEvent("client:TakeItemPictures", gender);
        }

        [Command("takepicture")]
        public async void TakeCharacterPicture(Player player)
        {
            player.SetSharedData("Player:Frozen", true);
            player.SetSharedData("Player:Invisible", true);
            player.Dimension = Convert.ToUInt32(player.Id+1);
            player.Position = new Vector3(221.45f, -984.5f, -99f);
            player.Rotation = new Vector3(0f, 0f, -90f);
            player.TriggerEvent("client:TakeIDPicture");

            player.StopAnimation();
        }

        [Command("bra")]
        public async void ToggleBra(Player player)
        {
            bool gender = player.GetSharedData<bool>("Player:Gender");
            if (!gender)
            {
                if (player.GetClothesDrawable(11) == 82)
                {
                    player.SetClothes(11, 15, 3);
                }
                else
                {
                    Item top = GetClothingOnSlot(player, 18);
                    if (top == null)
                    {
                        player.SetClothes(11, 82, 0);
                    }
                }

            }
            
        }

        public static void LoadInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            RefreshInventory(player, charid);
        }

        [Command("refreshinv", Alias = "refreshinventory")]
        public void RefreshFasz(Player player)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            List<Item> itemz = GetPlayerInventory(player);
            string json = NAPI.Util.ToJson(itemz);
            player.SendChatMessage(json);
            RefreshInventory(player, charid);
        }

        public async static void RefreshInventory(Player player, uint charid)
        {
            Item[] playerItems = await LoadPlayerInventory(charid);
            await SetInventory(0, charid, playerItems);
            if (await SortPlayerInventory(player))
            {
                Inventory.ItemList.SendItemListToPlayer(player);
                SendInventoryToPlayer(player, GetPlayerInventory(player).ToArray());
            }
        }

        public async static void SendInventoryToPlayer(Player player, Item[] items)
        {
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(items);
                player.TriggerEvent("client:InventoryFromServer", json);
            }, 500);
        }

        public async Task<Item> GetItemByDbId(Player player, uint dbid)
        {
            foreach (var item in GetPlayerInventory(player))
            {
                if (item.DBID == dbid)
                {
                    return item;
                }
            }
            return null;
        }

        public async Task<Item> GetItemByDbId(int ownertype, uint ownerid, uint dbid)
        {
            foreach (var item in GetInventory(ownertype, ownerid))
            {
                if (item.DBID == dbid)
                {
                    return item;
                }
            }
            return null;
        }

        public static async Task<Item> GetItemByDbId(uint dbid)//az összes tárolt inventory-n végigmegyünk, ez lassú, érdemes kerülni
        {
            foreach (var inv in Inventories)
            {
                foreach (var item in inv.Value)
                {
                    if (item.DBID == dbid)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public Item[] GetItemsByItemID(Player player, uint itemid)
        {
            List<Item> items = new List<Item>();
            foreach (var item in GetPlayerInventory(player))
            {
                if (item.ItemID == itemid)
                {
                    items.Add(item);
                }
            }
            return items.ToArray();
        }

        public Vehicle GetVehicleByID(ushort entityid)
        {
            List<Vehicle> vehs = NAPI.Pools.GetAllVehicles();

            foreach (var item in vehs)
            {
                if (item.Id == entityid)
                {
                    return item;
                }
            }
            return null;
        }

        [RemoteEvent("server:OpenVehicleTrunk")]
        public void OpenVehicleTrunk(Player player, ushort entityid)
        {
            Vehicle v = GetVehicleByID(entityid);
            if (v.HasData("Vehicle:ID") && Vector3.Distance(player.Position, v.Position) < 3f)
            {
                uint vehid = v.GetData<uint>("Vehicle:ID");

                if (vehid > 0)
                {
                    if (!IsInventoryInUse(2, vehid))//más nem használja a tárolót
                    {
                        PlayerCloseContainer(player);
                        Chat.Commands.ChatEmoteME(player, "belenéz egy jármű csomagtartójába. ((" + vehid + "))");
                        if (IsInventoryLoaded(2, vehid))//be van töltve, vissza tudjuk adni neki
                        {
                            SendContainerToPlayer(player, 2, vehid, GetInventory(2, vehid).ToArray());
                        }
                        else//nincs betöltve a tároló, be kell tölteni és utána visszaadni
                        {
                            RefreshVehicleTrunk(player, vehid);
                        }
                    }
                    else
                    {
                        player.SendChatMessage("Valaki más már használja ezt a csomagtartót.");
                    }

                }
            }
        }

        [RemoteEvent("server:OpenGroundItem")]
        public async void OpenGroundITem(Player player, uint grounditem_id)
        {
            PlayerCloseContainer(player);
            GroundItem gi = GroundItems.Where((i) => i.ID == grounditem_id).FirstOrDefault();
            if (Vector3.Distance(player.Position, gi.Object.Position) < 3f)//ha elég közel van hozzánk
            {
                if (!IsInventoryInUse(1, grounditem_id))//más nem használja a tárolót
                {
                    Item i = await LoadItemFromGround(gi.Item_DBID);
                    await OpenContainer(player, i);
                }
                else
                {
                    player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                }
            }
        }



        [RemoteEvent("server:ClosedContainer")]
        public async void PlayerCloseContainer(Player player)//bezárta a tárolót, tehát bezárjuk szerver oldalon is
        {
            if (player.HasData("Player:OpenedContainerOwnerType"))
            {
                int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
                switch (container_ownertype)
                {
                    case 1:
                        Item i = await GetItemByDbId(container_targetid);
                        if (i != null)
                        {
                            Chat.Commands.ChatEmoteME(player, "bezár egy tárolót. ((" + i.DBID + "))");
                        }
                        break;
                    case 2:
                        Chat.Commands.ChatEmoteME(player, "bezár egy csomagtartót. ((" + container_targetid + "))");
                        break;
                    case 3:
                        Chat.Commands.ChatEmoteME(player, "bezár egy kesztyűtartót. ((" + container_targetid + "))");
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
                player.ResetData("Player:OpenedContainerOwnerType");
                player.ResetData("Player:OpenedContainerID");
                SetInventoryInUse(container_ownertype, container_targetid, false);
            }

        }

        public async Task<bool> HasAccessToItem(Player player, Item item)
        {
            int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
            if (GetPlayerInventory(player).Contains(item))
            {
                return true;
            }
            else if(GetInventory(container_ownertype, container_targetid).Contains(item))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [RemoteEvent("server:OpenVehicleGloveBox")]
        public void OpenVehicleGloveBox(Player player)
        {
            Vehicle v = player.Vehicle;
            if (v.HasData("Vehicle:ID"))
            {
                uint vehid = v.GetData<uint>("Vehicle:ID");

                if (vehid > 0)//pozitív a jármű id (nem ideiglenes)
                {
                    if(!IsInventoryInUse(3, vehid))
                    {
                        Chat.Commands.ChatEmoteME(player, "belenéz egy jármű kesztyűtartójába. ((" + vehid + "))");
                        PlayerCloseContainer(player);
                        if (IsInventoryLoaded(3, vehid))//be van töltve, vissza tudjuk adni neki
                        {
                            SendContainerToPlayer(player, 3, vehid, GetInventory(3, vehid).ToArray());
                        }
                        else//nincs betöltve a tároló, be kell tölteni és utána visszaadni
                        {
                            RefreshVehicleGloveBox(player, vehid);
                        }
                    }
                    else
                    {
                        player.SendChatMessage("Valaki más már használja ezt a kesztyűtartót.");
                    }
                }
            }
        }

        public async static void RefreshVehicleGloveBox(Player player, uint vehid)
        {
            Item[] vehicleGloveBox = await LoadVehicleGloveBox(vehid);
            foreach (var item in vehicleGloveBox)
            {
                if (item.InUse)
                {
                    item.InUse = false;
                }
            }
            await SetInventory(3, vehid, vehicleGloveBox);
            if (await SortInventory(player, 3, vehid))//sorba rendezzük a cuccokat
            {
                SendContainerToPlayer(player, 3, vehid, GetInventory(3, vehid).ToArray());
            }
        }
        
        public async static void RefreshVehicleTrunk(Player player, uint vehid)
        {
            Item[] vehicleTrunk = await LoadVehicleTrunk(vehid);
            foreach (var item in vehicleTrunk)
            {
                if (item.InUse)
                {
                    item.InUse = false;
                }
            }
            await SetInventory(2, vehid, vehicleTrunk);
            if (await SortInventory(player, 2, vehid))//sorba rendezzük a cuccokat
            {
                SendContainerToPlayer(player, 2, vehid, GetInventory(2,vehid).ToArray());
            }
        }


        public async static void SendContainerToPlayer(Player player, int ownertype, uint ownerid, Item[] items)
        {
            NAPI.Task.Run(() =>
            {
                player.SetData("Player:OpenedContainerOwnerType", Convert.ToInt32(ownertype));//beállítjuk hogy tudjuk melyik van megnyitva neki
                player.SetData("Player:OpenedContainerID", Convert.ToUInt32(ownerid));//beállítjuk hogy tudjuk melyik van megnyitva neki
                SetInventoryInUse(ownertype, ownerid, true);
                string json = NAPI.Util.ToJson(items);
                player.TriggerEvent("client:ContainerFromServer", json);
                string containername = "";

                switch (ownertype)
                {
                    case 1:
                        containername = "Tároló [" + ownerid + "]";
                        break;
                    case 2:
                        containername = Vehicles.Vehicles.GetVehicleById(Convert.ToInt32(ownerid)).NumberPlate + " | Csomagtér [" + ownerid + "]";
                        break;
                    case 3:
                        containername = Vehicles.Vehicles.GetVehicleById(Convert.ToInt32(ownerid)).NumberPlate + " | Kesztyűtartó [" + ownerid + "]";
                        break;
                    default:
                        break;
                }

                player.TriggerEvent("client:SetContainerName", containername);
            });
        }


        public async static Task RefreshContainer(Player player, uint container_dbid)
        {
            Item[] ContainerItems = await LoadContainer(container_dbid);
            foreach (var item in ContainerItems)
            {
                if (item.InUse)
                {
                    item.InUse = false;
                }
            }
            await SetInventory(1, container_dbid, ContainerItems);
            if (await SortInventory(player, 1, container_dbid))//sorba rendezzük a cuccokat
            {
                SendContainerToPlayer(player, 1, container_dbid, ContainerItems.ToArray());
            }
        }

        public async Task OpenContainer(Player player, Item containeritem)
        {
            if (IsItemContainer(containeritem.ItemID))
            {
                int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
                //bezárjuk ha van megnyitott tároló
                player.ResetData("Player:OpenedContainerOwnerType");
                player.ResetData("Player:OpenedContainerID");

                if (!IsInventoryInUse(1, containeritem.DBID))
                {
                    SetInventoryInUse(container_ownertype, container_targetid, true);

                    Chat.Commands.ChatEmoteME(player, "megnyit egy tárolót. ((" + containeritem.DBID + "))");
                    if (IsInventoryLoaded(1, containeritem.DBID))//be van töltve, vissza tudjuk adni neki
                    {
                        SendContainerToPlayer(player, 1, containeritem.DBID, GetInventory(1, containeritem.DBID).ToArray());
                    }
                    else//nincs betöltve a tároló, be kell tölteni és utána visszaadni
                    {
                        await RefreshContainer(player, containeritem.DBID);
                    }
                }
                else
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                    });
                }
            }
        }

        public void CloseContainer(Player player, Item closedcontainer)
        {
            int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
            Chat.Commands.ChatEmoteME(player, "bezár egy tárolót. ((" + closedcontainer.DBID + "))");
            player.ResetData("Player:OpenedContainerOwnerType");
            player.ResetData("Player:OpenedContainerID");
            SetInventoryInUse(container_ownertype, container_targetid, false);
            player.TriggerEvent("client:CloseContainer");
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void PlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            if(player.HasData("Player:OpenedContainerOwnerType"))
            {
                int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");

                player.ResetData("Player:OpenedContainerOwnerType");
                player.ResetData("Player:OpenedContainerID");
                SetInventoryInUse(container_ownertype, container_targetid, false);
            }
        }

        [RemoteEvent("server:GiveItemToPlayer")]
        public async void GiveItemToPlayer(Player player, uint item_dbid, int target_id, uint amount)
        {
            //ha rajta van ruha akkor ne tudja átadni
            //ha használja akkor szintén ne
            Item i = await GetItemByDbId(item_dbid);
            if (i != null)
            {
                if (await HasAccessToItem(player, i))//ha hozzáfér az itemhez
                {
                    if (i.InUse == false)// nincs használatban(kézben fegyver, felvett ruha, stb)
                        {
                            Player target = Admin.Commands.GetPlayerById(target_id);
                            if (Vector3.Distance(player.Position, target.Position) < 5f)//elég közel vannak egymáshoz
                            {
                                uint charid = target.GetData<UInt32>("Player:CharID");
                                if (await HasSpaceForItem(0,charid,ItemList.GetItemWeight(i.ItemID)))//ha a célpontnak van elég helye
                                {
                                    //átadjuk a másiknak
                                    int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                                    uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
                                    if (container_ownertype == 1 && container_targetid == i.DBID)//ha item tároló van megnyitva és az amit épp átadunk akkor bezárjuk
                                    {
                                        CloseContainer(player, i);
                                    }
                                    if (amount == i.ItemAmount)//teljes itemet adunk át
                                    {

                                        i.OwnerType = 0;
                                        i.OwnerID = charid;
                                        List<Item> playerinv = GetPlayerInventory(player);
                                        playerinv.Remove(i);
                                        SetPlayerInventory(player, playerinv);

                                        List<Item> targetinv = GetPlayerInventory(target);
                                        targetinv.Add(i);
                                        SetPlayerInventory(target, targetinv);

                                        player.TriggerEvent("client:RemoveItem", i.DBID);
                                        
                                        string json = NAPI.Util.ToJson(i);
                                        target.TriggerEvent("client:AddItemToInventory", json);
                                        Chat.Commands.ChatEmoteME(player, "átad " + amount + " db " + ItemList.GetItemName(i.ItemID) + " tárgyat " + target.Name + "-nak.");
                                        if (i.ItemID <= 27)//ha ruha
                                        {

                                        }

                                    }
                                    else//csak egy részét adjuk át
                                    {

                                    }
                                }
                                else
                                {
                                    player.SendChatMessage(target.Name + " nem rendelkezik elég hellyel.");
                                }
                            }
                            else
                            {
                                player.SendChatMessage(target.Name + " túl messze van tőled.");
                            }
                        }
                    else
                    {
                        player.SendChatMessage(ItemList.GetItemName(i.ItemID) + " használatban van, ezért nem tudod átadni!");
                    }
                   

                }
            }
        }

        [RemoteEvent("server:PickUpItem")]
        public async void PickUpItem(Player player, uint grounditem_id)
        {
            GroundItem gi = GroundItems.Where((i) => i.ID == grounditem_id).FirstOrDefault();
            if (gi != null)
            {
                if (Vector3.Distance(player.Position, gi.Object.Position) < 3f)//ha elég közel van hozzánk
                {
                    if (!IsInventoryInUse(1, grounditem_id))//más nem használja a tárolót
                    {
                        Item i = await LoadItemFromGround(gi.Item_DBID);
                        if (i != null)//létezik az eldobott item és tényleg el van dobva (ownertype = 6)
                        {
                            uint charid = player.GetData<UInt32>("Player:CharID");
                            if (await HasSpaceForItem(0, charid, ItemList.GetItemWeight(i.ItemID)))//ha a célpontnak van elég helye
                            {
                                GroundItems.Remove(gi);
                                gi.PickUpTime = DateTime.Now;
                                gi.PickedUpBy = charid;
                                i.Priority = 1000;
                                i.OwnerType = 0;
                                i.OwnerID = charid;
                                NAPI.Task.Run(() =>
                                {
                                    string json = NAPI.Util.ToJson(i);
                                    player.TriggerEvent("client:AddItemToInventory", json);
                                    Chat.Commands.ChatEmoteME(player, "felvesz egy tárgyat a földről. ((" + i.DBID + "))");
                                    gi.Object.Delete();
                                    gi.Label.Delete();
                                });

                                await AddItemToInventory(player, 0, charid, i);//hozzáadjuk a tárolójához és meghívjuk CEF-et is
                                if (!await UpdateItem(i))
                                {
                                    Database.Log.Log_Server("ADATBÁZIS HIBA! ITEM DBID: " + i.DBID + " nem került mentésre.");
                                }
                                if (!await PickupGroundItem(gi))
                                {
                                    Database.Log.Log_Server("ADATBÁZIS HIBA! GROUND ITEM ID: " + gi.ID + " nem került felvételre.");
                                }
                            }
                            else
                            {
                                NAPI.Task.Run(() =>
                                {
                                    player.SendChatMessage("Nincs elég helyed felvenni.");
                                });
                            }
                        }
                        else
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SendChatMessage("Hiba! Ez a tárgy nem létezik. ID:" + gi.ID);
                                Database.Log.Log_Server("HIBA! " + player.Name + " megpróbált felvenni egy tárgyat de az nem létezik. ID:" + gi.ID + " ITEM ADATBÁZIS ID: " + gi.Item_DBID);
                            });
                        }
                    }
                    else
                    {
                        NAPI.Task.Run(() =>
                        {
                            player.SendChatMessage("Valaki más használja ezt a tárolót.");
                        });
                    }
                }
                else
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Túl távol vagy a a tárgy felvételéhez!");
                    });
                }
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Nem létezik ez a tárgy.");
                });
            }
        }

        [RemoteEvent("server:DropItem")]
        public async void DropItem(Player player, uint item_dbid, float groundZ, float objRotX, float objRotY, float objRotZ)
        {
            if (player.Vehicle == null)
            {
                Item i = await GetItemByDbId(item_dbid);

                if (i != null)
                {
                    if (await HasAccessToItem(player, i))
                    {
                        if (i.InUse == false)
                        {
                            string itemObj = ItemList.GetItemObject(i.ItemID);
                            if (itemObj != "-1" && itemObj != "")//létezik mert nem -1 (nem található item) és nem üres string (nincs object -> nem eldobható)
                            {
                                int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                                uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
                                if (container_ownertype == 1 && container_targetid == i.DBID)//ha item tároló van megnyitva és az amit épp eldobunk akkor bezárjuk
                                {
                                    CloseContainer(player, i);
                                }

                                Vector3 pos = player.Position;
                                Vector3 rot = new Vector3(objRotX, objRotY, objRotZ);

                                pos.Z = groundZ;
                                uint charid = player.GetData<UInt32>("Player:CharID");
                                //
                                GroundItem gi = new GroundItem(0, i.DBID, pos, rot, player.Dimension, DateTime.Now, charid);
                                string szoveg = "";
                                if (IsItemContainer(i.ItemID))
                                {
                                    szoveg = "Bal klikk: Felvétel | Jobb klikk: Megnyitás";
                                }
                                else
                                {
                                    szoveg = "Bal klikk: Felvétel";
                                }
                                await gi.CreateObject(itemObj, szoveg, i.ItemID);

                                uint id = await AddGroundItemToDatabase(gi);
                                gi.ID = id;
                                GroundItems.Add(gi);
                                RemoveItemFromInventory(i.OwnerType, i.OwnerID, i);
                                i.OwnerType = 6;
                                i.OwnerID = 0;

                                if (await UpdateItem(i))
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        player.TriggerEvent("client:RemoveItem", i.DBID);
                                        Chat.Commands.ChatEmoteME(player, "eldob egy tárgyat a földre. ((" + i.DBID + "))");
                                    });
                                }
                                else
                                {
                                    Database.Log.Log_Server("ITEM ELDOBÁS HIBA! DBID: " + i.DBID + " ELDOBOTT ID: " + id);
                                }
                            }
                            else
                            {
                                NAPI.Task.Run(() =>
                                {
                                    player.SendChatMessage(ItemList.GetItemName(i.ItemID) + " nem dobható el.");
                                }, 100);

                            }
                        }
                        else
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SendChatMessage("Használatban lévő tárgy nem dobható el.");
                            }, 100);
                        }
                    }
                }
            }
            else
            {
                player.SendChatMessage("Járműben nem dobhatsz el tárgyat!");
            }
        }


        [RemoteEvent("server:UseItem")]
        public async void ItemUse(Player player, uint item_dbid)
        {
            Item i = await GetItemByDbId(player, item_dbid);
            if (i != null)//csak akkor tudja használni ha nála van, hiszen az ő inventory-jából kérdeztük le
            {
                if (await HasAccessToItem(player,i))
                {
                    if (IsItemContainer(i.ItemID))//megnézzük hogy tároló-e
                    {
                        int draw = -1;
                        if (i.ItemID == 5 || i.ItemID == 18)//póló esetén máshogy kell az itemvalue
                        {
                            Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                            draw = t.Drawable;
                        }
                        else if (i.ItemID <= 26)//nem póló de ruha
                        {
                            Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                            draw = c.Drawable;
                        }

                        if (draw != -1)//nem -1, tehát találtunk valami drawable-t az itemvalue-ban, tehát ruha
                        {
                            if (IsItemContainer(i.ItemID, draw))//ha ez a drawable (modell) tároló, akkor fogjuk megnyitni a tároló inventoryt
                            {
                                int target_owner_type = player.GetData<int>("Player:OpenedContainerOwnerType");
                                uint target_owner_id = player.GetData<uint>("Player:OpenedContainerID");
                                if (target_owner_type == 1 && target_owner_id == item_dbid)//már meg van nyitva a tároló, be akarja zárni
                                {
                                    CloseContainer(player, i);
                                }
                                else if (!IsInventoryInUse(1, item_dbid))//nincs használatban a tároló
                                {
                                    PlayerCloseContainer(player);
                                    await OpenContainer(player, i);
                                }
                                else
                                {
                                    player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                                }
                            }
                        }
                        else//nem ruhadarab de tároló
                        {
                            int target_owner_type = player.GetData<int>("Player:OpenedContainerOwnerType");
                            uint target_owner_id = player.GetData<uint>("Player:OpenedContainerID");
                            if (target_owner_type == 1 && target_owner_id == item_dbid)//már meg van nyitva a tároló, be akarja zárni
                            {
                                CloseContainer(player, i);
                            }
                            else if (!IsInventoryInUse(1, item_dbid))//nincs használatban a tároló
                            {
                                PlayerCloseContainer(player);
                                await OpenContainer(player, i);
                            }
                            else
                            {
                                player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                            }
                        }
                    }
                    else if (i.ItemID <= 27)//nem tároló de ruha, ha rajta van le akarjuk venni, ha nincs akkor fel
                    {
                        if (i.InUse)//ha rajta van
                        {
                            MoveItem(player, item_dbid, 0);
                        }
                        else//nincs rajta, fel akarja venni
                        {
                            MoveItemToClothing(player, item_dbid, -1);
                        }
                    }
                    else//nem ruha és nem is tároló, egyedi kezelés jön
                    {
                        switch (i.ItemID)
                        {
                            case 31:
                                UseMeleeWeapon(player, "weapon_dagger", i);
                                break;
                            case 32:
                                UseMeleeWeapon(player, "weapon_bat", i);
                                break;
                            case 33:
                                UseMeleeWeapon(player, "weapon_bottle", i);
                                break;
                            case 34:
                                UseMeleeWeapon(player, "weapon_crowbar", i);
                                break;
                            case 35:
                                UseMeleeWeapon(player, "weapon_flashlight", i);
                                break;
                            case 36:
                                UseMeleeWeapon(player, "weapon_golfclub", i);
                                break;
                            case 37:
                                UseMeleeWeapon(player, "weapon_hammer", i);
                                break;
                            case 38:
                                UseMeleeWeapon(player, "weapon_Hatchet", i);
                                break;
                            case 39:
                                UseMeleeWeapon(player, "weapon_knuckle", i);
                                break;
                            case 40:
                                UseMeleeWeapon(player, "weapon_knife", i);
                                break;
                            case 41:
                                UseMeleeWeapon(player, "weapon_machete", i);
                                break;
                            case 42:
                                UseMeleeWeapon(player, "weapon_switchblade", i);
                                break;
                            case 43:
                                UseMeleeWeapon(player, "weapon_nightstick", i);
                                break;
                            case 44:
                                UseMeleeWeapon(player, "weapon_wrench", i);
                                break;
                            case 45:
                                UseMeleeWeapon(player, "weapon_battleaxe", i);
                                break;
                            case 46:
                                UseMeleeWeapon(player, "weapon_poolcue", i);
                                break;
                            case 47:
                                UseMeleeWeapon(player, "weapon_stone_hatchet", i);
                                break;
                            case 48:
                                UseMeleeWeapon(player, "weapon_candycane", i);
                                break;
                            case 49:
                                UseHandgun(player, "weapon_pistol", i, 0);
                                break;
                            case 51:
                                UseHandgun(player, "weapon_combatpistol", i, 0);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    Database.Log.Log_Server(player.Name + " hozzáférés nélkül próbált használni egy tárgyat: " + item_dbid + "");
                }
            }
            else
            {
                Item i2 = await GetItemByDbId(item_dbid);
                if (i2 != null)
                {
                    if (await HasAccessToItem(player,i2))
                    {
                        int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
                        uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
                        if (i2 != null)
                        {
                            if (i2.OwnerType == container_ownertype && i2.OwnerID == container_targetid)//megegyezik a megnyitott tárolóval, nincs baj
                            {

                            }
                            else
                            {
                                Database.Log.Log_Server("ITEM HIBA! Játékos megpróbált egy tárgyat használni de az nem elérhető számára. " + player.Name + " (DBID: " + item_dbid + ")");
                            }
                        }
                        else
                        {
                            Database.Log.Log_Server("ADATBÁZIS HIBA! Játékos megpróbált egy tárgyat használni de az nem létezik. " + player.Name + " (DBID: " + item_dbid + ")");
                        }
                    }
                    else
                    {
                        Database.Log.Log_Server(player.Name + " hozzáférés nélkül próbált használni egy tárgyat: " + item_dbid + "");
                    }

                }
            }
        }

        private async void UseMeleeWeapon(Player player, string namehash, Item i)
        {
            if (i.InUse)
            {
                player.TriggerEvent("client:ChangeItemInUse", i.DBID, i.InUse);
                Server.Chat.Commands.ChatEmoteME(player, "eltesz egy fegyver. (" + ItemList.GetItemName(i.ItemID) + ")");
                NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey("weapon_unarmed"));
                NAPI.Player.RemoveAllPlayerWeapons(player);
                i.InUse = false;
                player.TriggerEvent("client:ChangeItemInUse", i.DBID, i.InUse);
            }
            else
            {
                NAPI.Player.GivePlayerWeapon(player, NAPI.Util.GetHashKey(namehash), 1);
                NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey(namehash));
                i.InUse = true;
                player.TriggerEvent("client:ChangeItemInUse", i.DBID, i.InUse);
                Server.Chat.Commands.ChatEmoteME(player, "elővesz egy fegyver. (" + ItemList.GetItemName(i.ItemID) + ")");
            }
        }

        private async void UseHandgun(Player player, string namehash, Item i, uint ammo_itemid)
        {
            //ha ammo_itemid = 0 akkor nem kell hozzá ammo
            if (i.InUse)
            {
                Item[] tarak = GetItemsByItemID(player, 19);
                int remaining_ammo = NAPI.Player.GetPlayerWeaponAmmo(player, NAPI.Util.GetHashKey(namehash));

                int loszer = 0;
                for (int j = 0; j < tarak.Length; j++)
                {
                    if (tarak[j].ItemAmount > 0)
                    {
                        loszer = remaining_ammo - loszer;
                    }
                }

                //NAPI.Player.RemovePlayerWeapon(player, NAPI.Util.GetHashKey("weapon_pistol"));
                i.InUse = false;
                player.TriggerEvent("client:ChangeItemInUse", i.DBID, i.InUse);
                Server.Chat.Commands.ChatEmoteME(player, "eltesz egy pisztolyt. (" + ItemList.GetItemName(i.ItemID) + ")");

                player.PlayAnimation("reaction@intimidation@1h", "outro", 49);
                NAPI.Task.Run(() =>
                {
                    player.StopAnimation();

                }, 2500);
                NAPI.Task.Run(() =>
                {
                    NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey("weapon_unarmed"));
                    NAPI.Player.RemoveAllPlayerWeapons(player);
                }, 1500);
            }
            else
            {
                Item[] tarak = GetItemsByItemID(player, 19);
                int loszer = 0;//az első nem üres tárat töltjük majd be
                for (int j = 0; j < tarak.Length; j++)
                {
                    if (tarak[j].ItemAmount > 0)
                    {
                        loszer += tarak[j].ItemAmount;
                        tarak[j].InUse = true;
                    }
                }
                NAPI.Player.GivePlayerWeapon(player, NAPI.Util.GetHashKey(namehash), loszer);
                NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey(namehash));
                i.InUse = true;
                player.TriggerEvent("client:ChangeItemInUse", i.DBID, i.InUse);
                Server.Chat.Commands.ChatEmoteME(player, "elővesz egy pisztolyt. (" + ItemList.GetItemName(i.ItemID) + ")");

                player.PlayAnimation("reaction@intimidation@1h", "intro", 49);
                NAPI.Task.Run(() =>
                {
                    player.StopAnimation();
                }, 2500);
                //keresünk tárat és úgy adunk neki fegyvert
            }
        }

        private async static Task<bool> SortInventory(Player player, int ownertype, uint ownerid)//sorba rendezzük prioritás alapján az itemeket és új számokat adunk nekik növekvő sorrendben
        {
            Dictionary<uint, int> priorities = new Dictionary<uint, int>();
            List<Item> items = GetInventory(ownertype,ownerid);
            List<Item> ordered = items.OrderBy(o => o.Priority).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Priority = i;
                priorities[ordered[i].DBID] = i;
                try
                {
                    if (!await UpdateItem(ordered[i]))
                    {
                        Database.Log.Log_Server("Adatbázis hiba! Item DBID: " + ordered[i].DBID);
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Adatbázis hiba! Item DBID: " + ordered[i].DBID);
                }
            }
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:GetItemPriorities", NAPI.Util.ToJson(priorities));
            });
            
            return true;
        }


        private async static Task<bool> SortPlayerInventory(Player player)//sorba rendezzük prioritás alapján az itemeket és új számokat adunk nekik növekvő sorrendben
        {
            Dictionary<uint, int> priorities = new Dictionary<uint, int>();
            List<Item> items = GetPlayerInventory(player);
            List<Item> ordered = items.OrderBy(o => o.Priority).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Priority = i;
                priorities[ordered[i].DBID] = i;
                try
                {
                    UpdateItem(ordered[i]);
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Adatbázis hiba! Item DBID: " + ordered[i].DBID);
                }

            }
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:GetItemPriorities", NAPI.Util.ToJson(priorities));
            }, 500);

            SetPlayerInventory(player, ordered);
            return true;
        }


       


        [RemoteEvent("server:MoveItem")]//simán egy inventory-ba mozgatás, pl ruha levétel, stb
        public async void MoveItem(Player player, uint item_dbid, uint target_inv)
        {
            if (target_inv == 0)//játékos saját magára húzza
            {
                Item i1 = await GetItemByDbId(item_dbid);

                if (await HasAccessToItem(player, i1))
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    int originalowner = i1.OwnerType;
                    uint originalownerid = i1.OwnerID;
                    if (await HasSpaceForItem(0, charid, Convert.ToUInt32(ItemList.GetItemWeight(i1.ItemID) * i1.ItemAmount)))//ha a célpontnak van elég helye
                    {
                        i1.OwnerType = 0;
                        i1.OwnerID = charid;

                        NAPI.Task.Run(() =>
                        {
                            if (originalowner != 0)//nem játékostól jön az item (pl. inventoryban mozgatom akkor ownertype = 0
                            {
                                string tarolo = "tárolóból.";
                                switch (originalowner)
                                {
                                    case 1:
                                        tarolo = "tárolóból";
                                        break;
                                    case 2:
                                        tarolo = "csomagtartóból.";
                                        break;
                                    case 3:
                                        tarolo = "kesztyűtartóból.";
                                        break;
                                    default:
                                        break;
                                }
                                Chat.Commands.ChatEmoteME(player, "kivesz " + i1.ItemAmount + " db " + ItemList.GetItemName(i1.ItemID) + " tárgyat a " + tarolo + " ((" + originalownerid + ")) ");
                            }

                            //player.TriggerEvent("client:RemoveItem", i1.DBID);
                        });





                        if (i1.InUse && i1.ItemID <= 27)//használatban van (viseli) + ruha itemid-nek megfelel -> le kell venni róla
                        {
                            i1.InUse = false;
                            i1.Priority = 1000;

                            Tuple<bool, int> slot = GetClothingSlotFromItemId(i1.ItemID);
                            int[] clothing = GetDefaultClothes(i1.ItemID);

                            if (slot.Item1)//ruha
                            {
                                if (clothing.Length == 1)//kesztyű
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        bool gender = player.GetSharedData<bool>("Player:Gender");
                                        //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                        //beállítjuk a rajta lévő pólót mivel levette a kesztyűt, csak a torso-t akarjuk az alap értékre állítani
                                        Item Polo;
                                        if (gender)//férfi
                                        {
                                            Polo = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                                        }
                                        else
                                        {
                                            Polo = GetClothingOnSlot(player, 18);//lekérjük a pólóját
                                        }

                                        if (Polo != null)//ha van rajta póló
                                        {
                                            Top t = NAPI.Util.FromJson<Top>(Polo.ItemValue);
                                            player.SetClothes(slot.Item2, t.Torso, 0);
                                        }
                                        else//nincs rajta póló, átrakjuk az alap torso-ra
                                        {
                                            player.SetClothes(slot.Item2, clothing[0], 0);//átrakjuk a torso-ját az alap torso-ra
                                        }

                                        string json = NAPI.Util.ToJson(i1);
                                        player.TriggerEvent("client:AddItemToInventory", json);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    });
                                }
                                else if (clothing.Length == 2)//sima ruha
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                        player.SetClothes(slot.Item2, clothing[0], clothing[1]);

                                        string json = NAPI.Util.ToJson(i1);
                                        player.TriggerEvent("client:AddItemToInventory", json);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    });
                                }
                                else
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                        player.SetClothes(11, clothing[0], clothing[1]);
                                        player.SetClothes(3, clothing[2], 0);
                                        player.SetClothes(8, clothing[3], clothing[4]);

                                        string json = NAPI.Util.ToJson(i1);
                                        player.TriggerEvent("client:AddItemToInventory", json);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    });
                                }
                                //player.SetClothes(slot.Item2,)
                            }
                            else//prop
                            {
                                NAPI.Task.Run(() =>
                                {
                                    if (clothing.Length == 2)
                                    {
                                        //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                        player.SetAccessories(slot.Item2, clothing[0], clothing[1]);

                                        string json = NAPI.Util.ToJson(i1);
                                        player.TriggerEvent("client:AddItemToInventory", json);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    }
                                });
                            }
                        }
                        else if (!i1.InUse)//nincs használatban
                        {
                            i1.Priority = 1000;

                            NAPI.Task.Run(() =>
                            {
                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                string json = NAPI.Util.ToJson(i1);
                                player.TriggerEvent("client:AddItemToInventory", json);
                            });
                        }
                        AddItemToInventory(player, 0, charid, i1);
                    }
                    else
                    {
                        player.SendChatMessage("Nincs elég helyed!");
                    }
                }
                else
                {
                    Database.Log.Log_Server(player.Name + " hozzáférés nélkül próbált használni egy tárgyat: " + item_dbid + "");
                }
            }
            else if(target_inv == 1)//a megnyitott tárolóra húzza
            {
                Item i1 = await GetItemByDbId(item_dbid);
                if (await HasAccessToItem(player,i1))
                {
                    int target_owner_type = player.GetData<int>("Player:OpenedContainerOwnerType");
                    uint target_owner_id = player.GetData<uint>("Player:OpenedContainerID");
                    if (await HasSpaceForItem(target_owner_type, target_owner_id, Convert.ToUInt32(ItemList.GetItemWeight(i1.ItemID)*i1.ItemAmount)))//ha a célpontnak van elég helye
                    {
                        if (target_owner_type == 1 && IsItemContainer(i1.ItemID))//ha a tároló amibe bele szeretnénk tenni egy tároló item, és tárolót szeretnénk beletenni az nem jó
                        {
                            player.SendChatMessage("Tárolóba nem rakhatsz tárolót.");
                        }
                        else//vagy nem tároló itembe rakunk tárolót
                        {
                            i1.OwnerType = target_owner_type;
                            i1.OwnerID = target_owner_id;

                            if (i1.InUse && i1.ItemID <= 27)//használatban van (viseli) + ruha itemid-nek megfelel -> le kell venni róla
                            {
                                i1.InUse = false;
                                i1.Priority = 1000;

                                Tuple<bool, int> slot = GetClothingSlotFromItemId(i1.ItemID);
                                int[] clothing = GetDefaultClothes(i1.ItemID);
                                if (await SortPlayerInventory(player))
                                {
                                    if (slot.Item1)//ruha
                                    {
                                        if (clothing.Length == 1)//kesztyű
                                        {

                                            NAPI.Task.Run(() =>
                                            {
                                                bool gender = player.GetSharedData<bool>("Player:Gender");
                                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                                //beállítjuk a rajta lévő pólót mivel levette a kesztyűt, csak a torso-t akarjuk az alap értékre állítani
                                                Item Polo;
                                                if (gender)//férfi
                                                {
                                                    Polo = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                                                }
                                                else
                                                {
                                                    Polo = GetClothingOnSlot(player, 18);//lekérjük a pólóját
                                                }

                                                if (Polo != null)//ha van rajta póló
                                                {
                                                    Top t = NAPI.Util.FromJson<Top>(Polo.ItemValue);
                                                    player.SetClothes(slot.Item2, t.Torso, 0);
                                                }
                                                else//nincs rajta póló, átrakjuk az alap torso-ra
                                                {
                                                    player.SetClothes(slot.Item2, clothing[0], 0);//átrakjuk a torso-ját az alap torso-ra
                                                }

                                                string json = NAPI.Util.ToJson(i1);
                                                player.TriggerEvent("client:AddItemToContainer", json);
                                                player.TriggerEvent("client:RefreshInventoryPreview");
                                            });
                                        }
                                        else if (clothing.Length == 2)//sima ruha
                                        {
                                            NAPI.Task.Run(() =>
                                            {
                                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                                player.SetClothes(slot.Item2, clothing[0], clothing[1]);
                                                player.TriggerEvent("client:RefreshInventoryPreview");
                                                string json = NAPI.Util.ToJson(i1);
                                                player.TriggerEvent("client:AddItemToContainer", json);
                                            });
                                        }
                                        else
                                        {
                                            NAPI.Task.Run(() =>
                                            {
                                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                                player.SetClothes(11, clothing[0], clothing[1]);
                                                player.SetClothes(3, clothing[2], 0);
                                                player.SetClothes(8, clothing[3], clothing[4]);
                                                player.TriggerEvent("client:RefreshInventoryPreview");
                                                string json = NAPI.Util.ToJson(i1);
                                                player.TriggerEvent("client:AddItemToContainer", json);
                                            });
                                        }
                                        //player.SetClothes(slot.Item2,)
                                    }
                                    else//prop
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            if (clothing.Length == 2)
                                            {
                                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                                player.SetAccessories(slot.Item2, clothing[0], clothing[1]);
                                                player.TriggerEvent("client:RefreshInventoryPreview");
                                                string json = NAPI.Util.ToJson(i1);
                                                player.TriggerEvent("client:AddItemToContainer", json);
                                            }
                                        });
                                    }
                                }
                            }
                            else if (!i1.InUse)//nincs használatban
                            {
                                i1.Priority = 1000;

                                NAPI.Task.Run(() =>
                                {
                                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                    string json = NAPI.Util.ToJson(i1);
                                    player.TriggerEvent("client:AddItemToContainer", json);
                                });
                            }
                            NAPI.Task.Run(() =>
                            {
                                string tarolo = "tárolóba.";
                                switch (target_owner_type)
                                {
                                    case 1:
                                        tarolo = "tárolóba.";
                                        break;
                                    case 2:
                                        tarolo = "csomagtartóba.";
                                        break;
                                    case 3:
                                        tarolo = "kesztyűtartóba.";
                                        break;
                                    default:
                                        break;
                                }
                                Chat.Commands.ChatEmoteME(player, "betesz " + i1.ItemAmount + " db " + ItemList.GetItemName(i1.ItemID) + " tárgyat a " + tarolo + " ((" + target_owner_id + ")) ");
                                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                            });

                            AddItemToInventory(player, target_owner_type, target_owner_id, i1);
                        }

                    }
                    else
                    {
                        player.SendChatMessage("Nincs elég helye a tárolóban!");
                    }
                }
                else
                {
                    Database.Log.Log_Server(player.Name + " hozzáférés nélkül próbált mozgatni egy tárgyat: " + item_dbid + "");
                }
               
            }
        }


        [RemoteEvent("server:SwapItem")]
        public async void SwapItem(Player player, uint item1_dbid, uint item2_dbid)
        {
            Item i1 = await GetItemByDbId(item1_dbid);
            Item i2 = await GetItemByDbId(item2_dbid);

            if (await HasAccessToItem(player, i1) && await HasAccessToItem(player, i2))
            {

                if (1 <= i1.ItemID && i1.ItemID <= 27 && i1.ItemID == i2.ItemID && i1.InUse) //használatban lévő ruhát húzott egy másik itemre, meg szeretné cserélni
                {
                    MoveItemToClothing(player, item2_dbid, -1);
                }
                else//nem ruha item
                {
                    if (i1.InUse == false && i2.InUse == false)
                    {
                        int ownertype1 = i1.OwnerType;
                        int ownertype2 = i2.OwnerType;

                        uint ownerid1 = i1.OwnerID;
                        uint ownerid2 = i2.OwnerID;

                        int prio1 = i1.Priority;
                        int prio2 = i2.Priority;
                        if (await HasSpaceForItem(ownertype1, ownerid1, Convert.ToUInt32(ItemList.GetItemWeight(i2.ItemID) * i2.ItemAmount)) && await HasSpaceForItem(ownertype2, ownerid2, Convert.ToUInt32(ItemList.GetItemWeight(i1.ItemID) * i1.ItemAmount)))//ha a célpontnak van elég helye
                        {
                            if (i1.ItemID == i2.ItemID && ItemList.IsItemStackable(i1.ItemID))//ha megegyezik a két tárgy és stackelhetőek, akkor egybe fogjuk stackelni őket
                            {

                            }
                            else//nem egyik meg a két tárgy vagy nem stackelhető, simán megcseréljük
                            {
                                i1.Priority = prio2;
                                i2.Priority = prio1;

                                i1.OwnerType = ownertype2;
                                i1.OwnerID = ownerid2;

                                i2.OwnerType = ownertype1;
                                i2.OwnerID = ownerid1;

                                if (i1.OwnerID != i2.OwnerID || i1.OwnerType != i2.OwnerType)//nem egy inventoryn belül mozgatjuk
                                {
                                    AddItemToInventory(player, ownertype1, ownerid1, i2);
                                    AddItemToInventory(player, ownertype2, ownerid2, i1);
                                }
                                if (i1.OwnerType != 0)//nem játékosnál van, tehát container-hez adjuk
                                {
                                    try
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            string json = NAPI.Util.ToJson(i1);
                                            //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                            player.TriggerEvent("client:AddItemToContainer", json);
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID);
                                    }
                                }
                                else//játékosnál van
                                {
                                    try
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            string json = NAPI.Util.ToJson(i1);
                                            //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                            player.TriggerEvent("client:AddItemToInventory", json);
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID);
                                    }

                                }


                                if (i2.OwnerType != 0)//nem játékosnál van, tehát container-hez adjuk
                                {
                                    try
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            string json = NAPI.Util.ToJson(i2);
                                            //player.TriggerEvent("client:RemoveItem", i2.DBID);
                                            player.TriggerEvent("client:AddItemToContainer", json);
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Database.Log.Log_Server("Hibás ItemValue! DBID:" + i2.DBID);
                                    }
                                }
                                else//játékosnál van
                                {
                                    try
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            string json = NAPI.Util.ToJson(i2);
                                            //player.TriggerEvent("client:RemoveItem", i2.DBID);
                                            player.TriggerEvent("client:AddItemToInventory", json);
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Database.Log.Log_Server("Hibás ItemValue! DBID:" + i2.DBID);
                                    }

                                }
                            }


                        }
                        else
                        {
                            player.SendChatMessage("Nincs elég hely a tárgyak megcseréléséhez!");
                        }
                    }
                }
            }
            else
            {
                Database.Log.Log_Server(player.Name + " hozzáférés nélkül próbált megcserélni két tárgyat: " + item1_dbid + " ÉS " + item2_dbid);
            }
            
        }

        public Item GetItemInUse(Player player, uint itemID)
        {
            foreach (var item in GetPlayerInventory(player))
            {
                if (item.ItemID == itemID && item.InUse)
                {
                    return item;
                }
            }
            return null;
        }

        [RemoteEvent("server:RequestInventoryWeight")]
        public async void RequestInventoryWeight(Player player)
        {
            uint charid = player.GetData<UInt32>("Player:CharID");
            uint invweight = await GetInventoryWeight(0, charid);
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:SetInventoryWeights", invweight, GetInventoryCapacity(0));
            });
        }

        [RemoteEvent("server:RequestContainerWeight")]
        public async void RequestContainerWeight(Player player)
        {
            int container_ownertype = player.GetData<int>("Player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("Player:OpenedContainerID");
            uint invweight = await GetInventoryWeight(container_ownertype, container_targetid);
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:SetContainerWeights", invweight, GetInventoryCapacity(container_ownertype));
            });
        }

        [RemoteEvent("server:SetWornClothing")]
        public static async void SetWornClothing(Player player)
        {
            bool gender = player.GetSharedData<bool>("Player:Gender");

            if (gender)//férfi
            {
                for (uint i = 1; i <= 13; i++)
                {
                    Tuple<bool, int> slot = GetClothingSlotFromItemId(i);
                    int clothing_id = slot.Item2;
                    Item worn = GetClothingOnSlot(player, i);
                    if (worn != null)
                    {
                        if (i == 5 || i == 18)//póló
                        {
                            ItemValueToTop(player, worn, clothing_id, gender);
                        }
                        else if (i == 27)//kesztyű
                        {
                            ItemValueToGlove(player, worn, clothing_id, gender);
                        }
                        else if (slot.Item1)//RUHA
                        {
                            ItemValueToClothing(player, worn, clothing_id);
                        }
                        else//PROP
                        {
                            ItemValueToAccessory(player, worn, clothing_id);
                        }
                    }
                }
            }
            else//nő
            {
                for (uint i = 4; i <= 26; i++)
                {
                    Tuple<bool, int> slot = GetClothingSlotFromItemId(i);
                    int clothing_id = slot.Item2;
                    Item worn = GetClothingOnSlot(player, i);
                    if (worn != null)
                    {
                        if (i == 5 || i == 18)//póló
                        {
                            ItemValueToTop(player, worn, clothing_id, gender);
                        }
                        else if (i == 27)//kesztyű
                        {
                            ItemValueToGlove(player, worn, clothing_id, gender);
                        }
                        else if (slot.Item1)//RUHA
                        {
                            ItemValueToClothing(player, worn, clothing_id);
                        }
                        else//PROP
                        {
                            ItemValueToAccessory(player, worn, clothing_id);
                        }
                    }
                }
            }

            Tuple<bool, int> gloveslot = GetClothingSlotFromItemId(27);
            int glove_id = gloveslot.Item2;
            Item wornGlove = GetClothingOnSlot(player, 27);
            if (wornGlove != null)
            {
                ItemValueToGlove(player, wornGlove, glove_id, gender);
            }
        }


        public static int[] GetDefaultClothes(uint itemid)
        {
            int[] res = new int[0];
            switch (itemid)//megfeleltetjük a slot-ot (0-11) a RAGEMP ruha slottal (Clothes vagy Prop slot)
            {//true = ruha, false = prop
                case 1://kalap
                    res = new int[2] { -1, 0 };
                    break;
                case 2://maszk
                    res = new int[2] { 0, 0 };
                    break;
                case 3://nyaklánc - accessories
                    res = new int[2] { 0, 0 };
                    break;
                case 4://szemüveg
                    res = new int[2] { -1, 0 };
                    break;
                case 5://póló
                    res = new int[5] { 15, 0, 15, 15, 0 };
                    break;
                case 6://fülbevaló
                    res = new int[2] { -1, 0 };
                    break;
                case 7://nadrág
                    res = new int[2] { 14, 12 };
                    break;
                case 8://karkötő
                    res = new int[2] { -1, 0 };
                    break;
                case 9://cipő
                    res = new int[2] { 34, 0 };
                    break;
                case 10://óra
                    res = new int[2] { -1, 0 };
                    break;
                case 11://táska
                    res = new int[2] { 0, 0 };
                    break;
                case 12://páncél
                    res = new int[2] { 0, 0 };
                    break;
                case 13://decal
                    res = new int[2] { 0, 0 };
                    break;
                case 14://kalap
                    res = new int[2] { -1, 0 };
                    break;
                case 15://maszk
                    res = new int[2] { 0, 0 };
                    break;
                case 16://nyaklánc - accessories
                    res = new int[2] { 0, 0 };
                    break;
                case 17://szemüveg
                    res = new int[2] { -1, 0 };
                    break;
                case 18://póló
                    res = new int[5] { 15, 3, 15, 2, 0 };
                    break;
                case 19://fülbevaló
                    res = new int[2] { -1, 0 };
                    break;
                case 20://nadrág
                    res = new int[2] { 15, 3 };
                    break;
                case 21://karkötő
                    res = new int[2] { -1, 0 };
                    break;
                case 22://cipő
                    res = new int[2] { 35, 0 };
                    break;
                case 23://óra
                    res = new int[2] { -1, 0 };
                    break;
                case 24://táska
                    res = new int[2] { 0, 0 };
                    break;
                case 25://páncél
                    res = new int[2] { 0, 0 };
                    break;
                case 26://decal
                    res = new int[2] { 0, 0 };
                    break;
                case 27://kesztyű
                    res = new int[1] { 15 };
                    break;
            }
            return res;
        }

        public static Tuple<bool, int> GetClothingSlotFromItemId(uint itemid)
        {
            Tuple<bool, int> res = Tuple.Create(false, -1);
            switch (itemid)//megfeleltetjük a slot-ot (0-11) a RAGEMP ruha slottal (Clothes vagy Prop slot)
            {//true = ruha, false = prop
                case 1://kalap
                    res = Tuple.Create(false, 0);
                    break;
                case 2://maszk
                    res = Tuple.Create(true, 1);
                    break;
                case 3://nyaklánc - accessories
                    res = Tuple.Create(true, 7);
                    break;
                case 4://szemüveg
                    res = Tuple.Create(false, 1);
                    break;
                case 5://póló
                    res = Tuple.Create(true, 11);
                    break;
                case 6://fülbevaló
                    res = Tuple.Create(false, 2);
                    break;
                case 7://nadrág
                    res = Tuple.Create(true, 4);
                    break;
                case 8://karkötő
                    res = Tuple.Create(false, 7);
                    break;
                case 9://cipő
                    res = Tuple.Create(true, 6);
                    break;
                case 10://óra
                    res = Tuple.Create(false, 6);
                    break;
                case 11://táska
                    res = Tuple.Create(true, 5);
                    break;
                case 12://páncél
                    res = Tuple.Create(true, 9);
                    break;
                case 13://kitűző
                    res = Tuple.Create(true, 10); 
                    break;
                case 14://kalap
                    res = Tuple.Create(false, 0);
                    break;
                case 15://maszk
                    res = Tuple.Create(true, 1);
                    break;
                case 16://nyaklánc - accessories
                    res = Tuple.Create(true, 7);
                    break;
                case 17://szemüveg
                    res = Tuple.Create(false, 1);
                    break;
                case 18://póló
                    res = Tuple.Create(true, 11);
                    break;
                case 19://fülbevaló
                    res = Tuple.Create(false, 2);
                    break;
                case 20://nadrág
                    res = Tuple.Create(true, 4);
                    break;
                case 21://karkötő
                    res = Tuple.Create(false, 7);
                    break;
                case 22://cipő
                    res = Tuple.Create(true, 6);
                    break;
                case 23://óra
                    res = Tuple.Create(false, 6);
                    break;
                case 24://táska
                    res = Tuple.Create(true, 5);
                    break;
                case 25://páncél
                    res = Tuple.Create(true, 9);
                    break;
                case 26://kitűző
                    res = Tuple.Create(true, 10);
                    break;
                case 27://kesztyű
                    res = Tuple.Create(true, 3);
                    break;
                default:
                    return res;
                    break;
            }
            return res;
        }
        public async Task<bool> IsSlotCorrectForItemID(uint itemid, int slot)
        {
            if (itemid == 1 && slot == 0 ||
                itemid == 2 && slot == 1 ||
                itemid == 3 && slot == 2 ||
                itemid == 4 && slot == 3 ||
                itemid == 5 && slot == 4 ||
                itemid == 6 && slot == 5 ||
                itemid == 7 && slot == 6 ||
                itemid == 8 && slot == 7 ||
                itemid == 9 && slot == 8 ||
                itemid == 10 && slot == 9 ||
                itemid == 11 && slot == 10 ||
                itemid == 12 && slot == 11 ||
                itemid == 13 && slot == 12 ||
                itemid == 14 && slot == 0 ||
                itemid == 15 && slot == 1 ||
                itemid == 16 && slot == 2 ||
                itemid == 17 && slot == 3 ||
                itemid == 18 && slot == 4 ||
                itemid == 19 && slot == 5 ||
                itemid == 20 && slot == 6 ||
                itemid == 21 && slot == 7 ||
                itemid == 22 && slot == 8 ||
                itemid == 23 && slot == 9 ||
                itemid == 24 && slot == 10 ||
                itemid == 25 && slot == 11 ||
                itemid == 26 && slot == 12 ||
                itemid == 27 && slot == 13 ||
                slot == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [RemoteEvent("server:MoveItemToClothing")]
        public async void MoveItemToClothing(Player player, uint db_id, int target_slot)
        {
            //slotokat kezelni, a megfelelő ruhát ráadni a playerre, törölni az inventory-jából vagy container-ből az itemet nála és hozzáadni a slothoz
            bool gender = player.GetSharedData<bool>("Player:Gender");
            Item i = await GetItemByDbId(db_id);
            if (await HasAccessToItem(player,i))
            {
                if (ItemList.GetItemType(i.ItemID) == 1 && i.InUse == false)//1-es típus: ruha és nincs felvéve
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    if (await HasSpaceForItem(0, charid, ItemList.GetItemWeight(i.ItemID)))//ha a célpontnak van elég helye
                    {
                        int clothing_id = -1;
                        Tuple<bool, int> slot = GetClothingSlotFromItemId(i.ItemID);
                        clothing_id = slot.Item2;

                        if (clothing_id != -1)//nem -1, tehát találtunk valamit
                        {
                            if (await IsSlotCorrectForItemID(i.ItemID, target_slot))//ha megfelelő slotra húzta vagy -1
                            {
                                if ((i.ItemID <= 13 && gender) || (i.ItemID > 13 && i.ItemID <= 26 && !gender) || i.ItemID == 27)//férfi item VAGY női item VAGY kesztyű
                                {
                                    Item toSwap = GetClothingOnSlot(player, i.ItemID);
                                    if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                                    {
                                        if (toSwap.ItemID == i.ItemID)
                                        {
                                            if (i.OwnerType != 0)//nem a saját inventory-jából próbálja felvenni, megcseréljük őket
                                            {
                                                int ownertype1 = i.OwnerType;
                                                int ownertype2 = toSwap.OwnerType;

                                                uint ownerid1 = i.OwnerID;
                                                uint ownerid2 = toSwap.OwnerID;

                                                int prio1 = i.Priority;
                                                int prio2 = toSwap.Priority;
                                                i.Priority = prio2;
                                                toSwap.Priority = prio1;

                                                i.OwnerType = ownertype2;
                                                toSwap.OwnerType = ownertype1;

                                                i.OwnerID = ownerid2;
                                                toSwap.OwnerID = ownerid1;
                                            }
                                            i.InUse = true;
                                            toSwap.InUse = false;
                                            toSwap.Priority = 1000;
                                            if (i.ItemID == 5 || i.ItemID == 18)//póló
                                            {
                                                ItemValueToTopSwap(player, i, toSwap, clothing_id, gender);
                                            }
                                            else if (i.ItemID == 27)//kesztyű
                                            {
                                                ItemValueToGloveSwap(player, i, toSwap, clothing_id, gender);
                                            }
                                            else if (slot.Item1)//RUHA
                                            {
                                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                                            }
                                            else//PROP
                                            {
                                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i.OwnerType = 0;
                                        i.OwnerID = charid;
                                        i.InUse = true;

                                        if (i.ItemID == 5 || i.ItemID == 18)//póló
                                        {
                                            ItemValueToTop(player, i, clothing_id, gender);
                                        }
                                        else if (i.ItemID == 27)//kesztyű
                                        {
                                            ItemValueToGlove(player, i, clothing_id, gender);
                                        }
                                        else if (slot.Item1)//RUHA
                                        {
                                            ItemValueToClothing(player, i, clothing_id);
                                        }
                                        else//PROP
                                        {
                                            ItemValueToAccessory(player, i, clothing_id);
                                        }

                                    }
                                }
                            }

                        }
                    }
                }
            }


        }

    public async void ItemValueToTopSwap(Player player, Item i1, Item i2, int clothing_id, bool gender)
    {
        NAPI.Task.Run(() =>
        {
            try
            {
                uint charid = player.GetData<UInt32>("Player:CharID");
                AddItemToInventory(player, i1.OwnerType, i1.OwnerID, i1);
                AddItemToInventory(player, i2.OwnerType, i2.OwnerID, i2);

                //player.TriggerEvent("client:RemoveItem", i1.DBID);
                //player.TriggerEvent("client:RemoveItem", i2.DBID);
                Item Top = GetClothingOnSlot(player, i1.ItemID);

                Top t;
                if (Top != null)//van rajta póló
                {
                    t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                }
                else
                {
                    int[] slots = GetDefaultClothes(i1.ItemID);
                    t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                }
                int correctDrawable = GetCorrectClothing(gender, clothing_id, t.Drawable);

                player.SetClothes(clothing_id, correctDrawable, t.Texture);
                player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);
                //KESZTYŰ KEZELÉSE
                Item GloveItem = GetClothingOnSlot(player, 27);//lekérjük a kesztyűjét
                if (GloveItem != null)//van rajta kesztyű
                {
                    int glovetorso = Convert.ToInt32(GloveItem.ItemValue);//0-10, melyik kesztyű
                    int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, glovetorso);
                    if (correctTorso != -1)//kaptunk lehetséges kesztyűt
                    {
                        player.SetClothes(3, correctTorso, 0);
                    }
                    else//nincs kompatibilis kesztyű ezzel a torsoval, beállítjuk az alapot
                    {
                        player.SetClothes(3, t.Torso, 0);
                        player.SendChatMessage("A kesztyűd nem kompatibilits ezzel a TORSO-val.");
                    }
                }
                else
                {
                    player.SetClothes(3, t.Torso, 0);
                }

                string json = NAPI.Util.ToJson(i1);
                player.TriggerEvent("client:AddItemToClothing", json);
                if (i2.OwnerType == 0 && i2.OwnerID == charid)
                {
                    string json2 = NAPI.Util.ToJson(i2);
                    player.TriggerEvent("client:AddItemToInventory", json2);
                }
                else
                {
                    string json2 = NAPI.Util.ToJson(i2);
                    player.TriggerEvent("client:AddItemToContainer", json2);
                }

                player.TriggerEvent("client:RefreshInventoryPreview");
                player.TriggerEvent("client:SetClothingImage", i1.DBID, gender, clothing_id, correctDrawable, t.Texture);
            }
            catch (Exception ex)
            {
                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID + " & " + i2.DBID);
            }
        });
    }

        public static async void ItemValueToTop(Player player, Item i, int clothing_id, bool gender)
        {
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        uint charid = player.GetData<UInt32>("Player:CharID");
                        AddItemToInventory(player, i.OwnerType, i.OwnerID, i);

                        //player.TriggerEvent("client:RemoveItem", i.DBID);
                        Item Top = GetClothingOnSlot(player, i.ItemID);//lekérjük a pólóját
                        Top t;
                        if (Top != null)//van rajta póló
                        {
                            t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                        }
                        else
                        {
                            if (gender)
                            {
                                int[] slots = GetDefaultClothes(5);
                                t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                            }
                            else
                            {
                                int[] slots = GetDefaultClothes(18);
                                t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                            }
                        }
                        int correctDrawable = GetCorrectClothing(gender, clothing_id, t.Drawable);
                        int correctUndershirtDrawable = GetCorrectClothing(gender, 8, t.UndershirtDrawable);
                        player.SetClothes(clothing_id, correctDrawable, t.Texture);
                        player.SetClothes(8, correctUndershirtDrawable, t.UndershirtTexture);
                        //KESZTYŰ KEZELÉSE
                        Item GloveItem = GetClothingOnSlot(player, 27);//lekérjük a kesztyűjét
                        if (GloveItem != null)//van rajta kesztyű
                        {
                            int glovetorso = Convert.ToInt32(GloveItem.ItemValue);//0-10, melyik kesztyű
                            int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, glovetorso);
                            if (correctTorso != -1)//kaptunk lehetséges kesztyűt
                            {
                                player.SetClothes(3, correctTorso, 0);
                            }
                            else//nincs kompatibilis kesztyű ezzel a torsoval, beállítjuk az alapot
                            {
                                player.SetClothes(3, t.Torso, 0);
                                player.SendChatMessage("A kesztyűd nem kompatibilits ezzel a TORSO-val.");
                            }
                        }
                        else
                        {
                            player.SetClothes(3, t.Torso, 0);
                        }

                        string json = NAPI.Util.ToJson(i);
                        player.TriggerEvent("client:AddItemToClothing", json);
                        player.TriggerEvent("client:RefreshInventoryPreview");
                        player.TriggerEvent("client:SetClothingImage", i.DBID, gender, clothing_id, correctDrawable, t.Texture);
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                    }
                    
                });
        }

        public async void ItemValueToGloveSwap(Player player, Item i1, Item i2, int clothing_id, bool gender)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player, i1.OwnerType, i1.OwnerID, i1);
                    AddItemToInventory(player, i2.OwnerType, i2.OwnerID, i2);
                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                    Item Top;
                    if (gender)//férfi
                    {
                        Top = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                    }
                    else
                    {
                        Top = GetClothingOnSlot(player, 18);//lekérjük a pólóját
                    }
                    
                    Top t;
                    if (Top != null)//van rajta póló
                    {
                        t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                    }
                    else
                    {
                        if (gender)
                        {
                            int[] slots = GetDefaultClothes(5);
                            t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                        }
                        else
                        {
                            int[] slots = GetDefaultClothes(18);
                            t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                        }

                    }

                    //KESZTYŰ KEZELÉS
                    int gloves = Convert.ToInt32(i1.ItemValue);
                    int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, gloves);
                    if (correctTorso != -1)//ha kompatibilis kesztyű van hozzá
                    {
                        player.SetClothes(clothing_id, correctTorso, 0);
                    }
                    else
                    {
                        player.SetClothes(clothing_id, t.Torso, 0);
                    }

                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    if (i2.OwnerType == 0 && i2.OwnerID == charid)
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToInventory", json2);
                    }
                    else
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToContainer", json2);
                    }
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID + " & " + i2.DBID);
                }
            });
        }

        public static async void ItemValueToGlove(Player player, Item i, int clothing_id, bool gender)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player,i.OwnerType, i.OwnerID, i);

                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                    Item Top;
                    if (gender)//férfi
                    {
                        Top = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                    }
                    else
                    {
                        Top = GetClothingOnSlot(player, 18);//lekérjük a pólóját
                    }

                    Top t;
                    if (Top != null)//van rajta póló
                    {
                        t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                    }
                    else
                    {
                        if (gender)
                        {
                            int[] slots = GetDefaultClothes(5);
                            t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                        }
                        else
                        {
                            int[] slots = GetDefaultClothes(18);
                            t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                        }
                    }

                    //KESZTYŰ KEZELÉS
                    int gloves = Convert.ToInt32(i.ItemValue);
                    int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, gloves);
                    if (correctTorso != -1)//ha kompatibilis kesztyű van hozzá
                    {
                        player.SetClothes(clothing_id, correctTorso, 0);
                    }
                    else
                    {
                        player.SetClothes(clothing_id, t.Torso, 0);
                    }

                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                }
            });

        }



        public async void ItemValueToClothingSwap(Player player, Item i1, Item i2, int clothing_id)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player, i1.OwnerType, i1.OwnerID, i1);
                    AddItemToInventory(player, i2.OwnerType, i2.OwnerID, i2);

                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                    //player.TriggerEvent("client:RemoveItem", i2.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);
                    bool gender = player.GetSharedData<bool>("Player:Gender");
                    int correctDrawable = GetCorrectClothing(gender, clothing_id, c.Drawable);
                    player.SetClothes(clothing_id, correctDrawable, c.Texture);

                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    if (i2.OwnerType == 0 && i2.OwnerID == charid)
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToInventory", json2);
                    }
                    else
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToContainer", json2);
                    }
                    player.TriggerEvent("client:RefreshInventoryPreview");
                    player.TriggerEvent("client:SetClothingImage", i1.DBID, gender, clothing_id, correctDrawable, c.Texture);
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID);
                }

            });
        }


        public static async void ItemValueToClothing(Player player, Item i, int clothing_id)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player, i.OwnerType, i.OwnerID, i);

                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);

                    bool gender = player.GetSharedData<bool>("Player:Gender");
                    int correctDrawable = GetCorrectClothing(gender, clothing_id, c.Drawable);
                    player.SetClothes(clothing_id, correctDrawable, c.Texture);

                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                    player.TriggerEvent("client:SetClothingImage", i.DBID, gender, clothing_id, correctDrawable, c.Texture);
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                }
            });
        }

        public async void ItemValueToAccessorySwap(Player player, Item i1, Item i2, int clothing_id)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player, i1.OwnerType, i1.OwnerID, i1);
                    AddItemToInventory(player, i2.OwnerType, i2.OwnerID, i2);
                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                    //player.TriggerEvent("client:RemoveItem", i2.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);

                    bool gender = player.GetSharedData<bool>("Player:Gender");
                    int correctDrawable = GetCorrectAccessory(gender, clothing_id, c.Drawable);
                    player.SetAccessories(clothing_id, correctDrawable, c.Texture);

                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    if (i2.OwnerType == 0 && i2.OwnerID == charid)
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToInventory", json2);
                    }
                    else
                    {
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToContainer", json2);
                    }
                    player.TriggerEvent("client:RefreshInventoryPreview");
                    player.TriggerEvent("client:SetPropImage", i1.DBID, gender, clothing_id, correctDrawable, c.Texture);
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID + " & " + i2.DBID);
                }
            });
        }


        public static async void ItemValueToAccessory(Player player, Item i, int clothing_id)
        {
            i.InUse = true;
            NAPI.Task.Run(() =>
            {
                try
                {
                    uint charid = player.GetData<UInt32>("Player:CharID");
                    AddItemToInventory(player, i.OwnerType, i.OwnerID, i);
                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                    bool gender = player.GetSharedData<bool>("Player:Gender");
                    int correctDrawable = GetCorrectAccessory(gender, clothing_id, c.Drawable);
                    player.SetAccessories(clothing_id, correctDrawable, c.Texture);

                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                    player.TriggerEvent("client:SetPropImage", i.DBID, gender, clothing_id, correctDrawable, c.Texture);
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                }
            });
        }



        public static Item GetClothingOnSlot(Player player, uint itemid)
        {
            List<Item> inv = GetPlayerInventory(player);

            if (inv != null)
            {
                foreach (var item in inv)
                {
                    if (item.ItemID == itemid && item.InUse == true)
                    {
                        return item;
                    }
                }
            }
                return null;
        }

        public static async Task<Item> LoadItemFromGround(uint dbid)
        {
            string query = $"SELECT * FROM `items` WHERE `DbID` = @DatabaseID AND `ownerType` = 6 LIMIT 1";
            Item item = null;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DatabaseID", dbid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    item = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return item;
            }
        }

        public static async Task<Item[]> LoadPlayerInventory(uint charid)
        {
            string query = $"SELECT * FROM `items` WHERE `ownerType` = 0 AND `ownerID` = @CharacterID ORDER BY Priority";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CharacterID", charid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                    items.Add(loadedItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return items.ToArray();
            }
        }

        public static async Task<Item[]> LoadVehicleTrunk(uint vehid)
        {
            string query = $"SELECT * FROM `items` WHERE `ownerType` = 2 AND `ownerID` = @VehicleID ORDER BY Priority";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VehicleID", vehid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                    items.Add(loadedItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return items.ToArray();
            }
        }

        public static async Task<Item[]> LoadVehicleGloveBox(uint vehid)
        {
            string query = $"SELECT * FROM `items` WHERE `ownerType` = 3 AND `ownerID` = @VehicleID ORDER BY Priority";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VehicleID", vehid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                    items.Add(loadedItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return items.ToArray();
            }
        }

        public static async Task<Item[]> LoadContainer(uint container_dbid)
        {
            string query = $"SELECT * FROM `items` WHERE `ownerType` = 1 AND `ownerID` = @ContainerID ORDER BY Priority";
            List<Item> items = new List<Item>();
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ContainerID", container_dbid);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]), reader["itemValue"].ToString(), Convert.ToInt32(reader["itemAmount"]), Convert.ToBoolean(reader["inUse"]), Convert.ToBoolean(reader["duty"]), Convert.ToInt32(reader["priority"]));
                                    items.Add(loadedItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
                con.CloseAsync();
                return items.ToArray();
            }
        }


        public async static Task<bool> UpdateItem(Item item)
        {
            bool state = false;
            string query = $"UPDATE `items` SET `ownerID` = @OwnerID, `ownerType` = @OwnerType, `itemAmount` = @ItemAmount, `inUse` = @InUse, `priority` = @Priority  WHERE `items`.`DbID` = @DBID;";
            //string query2 = $"UPDATE `characters` SET `characterName` = @CharacterName, `dob` = @DOB, `pob` = @POB WHERE `appearanceId` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@DBID", item.DBID);
                    command.Parameters.AddWithValue("@OwnerID", item.OwnerID);
                    command.Parameters.AddWithValue("@OwnerType", item.OwnerType);
                    command.Parameters.AddWithValue("@ItemAmount", item.ItemAmount);
                    command.Parameters.AddWithValue("@InUse", item.InUse);
                    command.Parameters.AddWithValue("@Priority", item.Priority);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            state = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }

            return state;
        }

        public async static Task<uint> AddItemToDatabase(uint charid, Item itemdata)//létrehozunk egy új itemet az adatbázisban
        {
            uint ItemDBID = 0;
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            string query = $"INSERT INTO `items` " +
                $"(`ownerID`, `ownerType`, `itemID`, `itemValue`, `itemAmount`, `inUse`, `duty`, `createdBy`, `priority`)" +
                $" VALUES " +
                $"(@OwnerID,@OwnerType, @ItemID, @ItemValue, @ItemAmount, @InUse, @Duty, @Creator, @Priority)";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {

                    command.Parameters.AddWithValue("@OwnerID", itemdata.OwnerID);
                    command.Parameters.AddWithValue("@OwnerType", itemdata.OwnerType);
                    command.Parameters.AddWithValue("@ItemID", itemdata.ItemID);
                    command.Parameters.AddWithValue("@ItemValue", itemdata.ItemValue);
                    command.Parameters.AddWithValue("@ItemAmount", itemdata.ItemAmount);
                    command.Parameters.AddWithValue("@InUse", itemdata.InUse);
                    command.Parameters.AddWithValue("@Duty", itemdata.Duty);
                    command.Parameters.AddWithValue("@Creator", charid);
                    command.Parameters.AddWithValue("@Priority", itemdata.Priority);

                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            long lastid = command.LastInsertedId;
                            ItemDBID = Convert.ToUInt32(lastid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
            }
            return ItemDBID;
        }
    }
}
