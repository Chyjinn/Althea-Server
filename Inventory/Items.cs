using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Server.Characters;

namespace Server.Inventory
{
    class Container
    {
        public uint ItemID { get; }//ITEMID
        public int Drawable { get; } = -1;//DRAWABLE, ha ruháról beszélünk pl
        public float Capacity { get; }
        public Container(uint itemid, int drawable, float capacity) {
            ItemID = itemid;
            Drawable = drawable; 
            Capacity = capacity;
        }

        public Container(uint itemid, float capacity)
        {
            ItemID = itemid;
            Capacity = capacity;
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
            new Container(11,82,15f)
        };

        static Dictionary<Tuple<int, uint>, bool> OpenedInventories = new Dictionary<Tuple<int, uint>, bool>();

        static Dictionary<Tuple<int, uint>, List<Item>> Inventories = new Dictionary<Tuple<int, uint>, List<Item>>();
        //OWNERTYPE-ok:
        /*
        0 - JÁTÉKOS
        1 - ITEM TÁROLÓ
        2 - JÁRMŰ CSOMAGTARTÓ
        3 - JÁRMŰ KESZTYŰTARTÓ
        4 - INTERIOR
        5 - OBJECT
          
         */

        /*
        TÁROLÓKNÁL:
        ITEMID-t összehasonlítjuk
        ha itemid megfelel akkor esetleg itemvalue-t -> jó drawable? pl táskánál

        */

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


        public static void SetInventory(int OwnerType, uint OwnerID, Item[] items)
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
            uint charid = player.GetData<UInt32>("player:charID");
            Inventories[new Tuple<int, uint>(0, charid)].Add(item);
        }
        public static void RemoveItemFromPlayerInventory(Player player, Item item)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            Inventories[new Tuple<int, uint>(0, charid)].Remove(item);
        }



        public static void SetPlayerInventory(Player player, List<Item> items)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            Inventories[new Tuple<int, uint>(0, charid)] = items;
        }

        public static List<Item> GetPlayerInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            return Inventories[new Tuple<int, uint>(0, charid)];
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
            uint charid = target.GetData<UInt32>("player:charID");
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



        public static void LoadInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            RefreshInventory(player, charid);
        }

        [Command("refreshinv", Alias = "refreshinventory")]
        public void RefreshFasz(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            List<Item> itemz = GetPlayerInventory(player);
            string json = NAPI.Util.ToJson(itemz);
            player.SendChatMessage(json);
            RefreshInventory(player, charid);
        }

        public async static void RefreshInventory(Player player, uint charid)
        {
            Item[] playerItems = await LoadPlayerInventory(charid);
            SetInventory(0, charid, playerItems);
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

        public async Task<Item> GetItemByDbId(uint dbid)//az összes tárolt inventory-n végigmegyünk, ez lassú, érdemes kerülni
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
            if (v.HasData("vehicle:ID") && Vector3.Distance(player.Position, v.Position) < 3f)
            {
                uint vehid = v.GetData<uint>("vehicle:ID");

                if (vehid > 0)
                {
                    if (!IsInventoryInUse(2, vehid))//más nem használja a tárolót
                    {
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

        [RemoteEvent("server:ClosedContainer")]
        public void PlayerCloseContainer(Player player)//bezárta a tárolót, tehát bezárjuk szerver oldalon is
        {
            int container_ownertype = player.GetData<int>("player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("player:OpenedContainerID");

            player.ResetData("player:OpenedContainerOwnerType");
            player.ResetData("player:OpenedContainerID");
            SetInventoryInUse(container_ownertype, container_targetid, false);
        }

        [RemoteEvent("server:OpenVehicleGloveBox")]
        public void OpenVehicleGloveBox(Player player)
        {
                Vehicle v = player.Vehicle;
            if (v.HasData("vehicle:ID"))
            {
                uint vehid = v.GetData<uint>("vehicle:ID");

                if (vehid > 0)//pozitív a jármű id (nem ideiglenes)
                {
                    if(!IsInventoryInUse(3, vehid))
                    {
                        Chat.Commands.ChatEmoteME(player, "belenéz egy jármű kesztyűtartójába. ((" + vehid + "))");
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
            SetInventory(3, vehid, vehicleGloveBox);
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
            SetInventory(2, vehid, vehicleTrunk);
            if (await SortInventory(player, 2, vehid))//sorba rendezzük a cuccokat
            {
                SendContainerToPlayer(player, 2, vehid, GetInventory(2,vehid).ToArray());
            }
        }


        public async static void SendContainerToPlayer(Player player, int ownertype, uint ownerid, Item[] items)
        {
            NAPI.Task.Run(() =>
            {
                player.SetData("player:OpenedContainerOwnerType", Convert.ToInt32(ownertype));//beállítjuk hogy tudjuk melyik van megnyitva neki
                player.SetData("player:OpenedContainerID", Convert.ToUInt32(ownerid));//beállítjuk hogy tudjuk melyik van megnyitva neki
                SetInventoryInUse(ownertype, ownerid, true);
                string json = NAPI.Util.ToJson(items);
                player.TriggerEvent("client:ContainerFromServer", json);
            });
        }

        public async static void RefreshContainer(Player player, uint container_dbid)
        {
            Item[] ContainerItems = await LoadContainer(container_dbid);
            foreach (var item in ContainerItems)
            {
                if (item.InUse)
                {
                    item.InUse = false;
                }
            }
            SetInventory(1, container_dbid, ContainerItems);
            if (await SortInventory(player, 1, container_dbid))//sorba rendezzük a cuccokat
            {
                SendContainerToPlayer(player, 1, container_dbid, GetInventory(1, container_dbid).ToArray());
            }
        }

        public void OpenContainer(Player player, Item containeritem)
        {
            int container_ownertype = player.GetData<int>("player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("player:OpenedContainerID");
            //bezárjuk ha van megnyitott tároló
            player.ResetData("player:OpenedContainerOwnerType");
            player.ResetData("player:OpenedContainerID");
            SetInventoryInUse(container_ownertype, container_targetid, false);

            Chat.Commands.ChatEmoteME(player, "megnyit egy tárolót. ((" + containeritem.DBID + "))");
            if (IsInventoryLoaded(1, containeritem.DBID))//be van töltve, vissza tudjuk adni neki
            {
                SendContainerToPlayer(player, 1, containeritem.DBID, GetInventory(1, containeritem.DBID).ToArray());
            }
            else//nincs betöltve a tároló, be kell tölteni és utána visszaadni
            {
                RefreshContainer(player, containeritem.DBID);
            }
        }

        public void CloseContainer(Player player)
        {
            int container_ownertype = player.GetData<int>("player:OpenedContainerOwnerType");
            uint container_targetid = player.GetData<uint>("player:OpenedContainerID");

            player.ResetData("player:OpenedContainerOwnerType");
            player.ResetData("player:OpenedContainerID");
            SetInventoryInUse(container_ownertype, container_targetid, false);
            player.TriggerEvent("client:CloseContainer");
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void PlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            if(player.HasData("player:OpenedContainerOwnerType"))
            {
                int container_ownertype = player.GetData<int>("player:OpenedContainerOwnerType");
                uint container_targetid = player.GetData<uint>("player:OpenedContainerID");

                player.ResetData("player:OpenedContainerOwnerType");
                player.ResetData("player:OpenedContainerID");
                SetInventoryInUse(container_ownertype, container_targetid, false);
            }
        }

        [RemoteEvent("server:GiveItemToPlayer")]
        public async void GiveItemToPlayer(Player player, uint item_dbid)
        {
            //le kell ellenőrizni, hogy az item a játékosnál van-e, vagy a megnyitott tárolójában. HA egyikben sem akkor baj van
        }



        [RemoteEvent("server:UseItem")]
        public async void ItemUse(Player player, uint item_dbid)
        {
            Item i = await GetItemByDbId(player, item_dbid);
            if (i != null)//csak akkor tudja használni ha nála van, hiszen az ő inventory-jából kérdeztük le
            {
                if (IsItemContainer(i.ItemID))//megnézzük hogy tároló-e
                {
                    int draw = -1;
                    if(i.ItemID == 5)//póló esetén máshogy kell az itemvalue
                    {
                        Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                        draw = t.Drawable;
                    }
                    else if(i.ItemID <= 14)//nem póló de ruha
                    {
                        Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                        draw = c.Drawable;
                    }

                    if (draw != -1)//nem -1, tehát találtunk valami drawable-t az itemvalue-ban, tehát ruha
                    {
                        if (IsItemContainer(i.ItemID, draw))//ha ez a drawable (modell) tároló, akkor fogjuk megnyitni a tároló inventoryt
                        {
                            int target_owner_type = player.GetData<int>("player:OpenedContainerOwnerType");
                            uint target_owner_id = player.GetData<uint>("player:OpenedContainerID");
                            if (target_owner_type == 1 && target_owner_id == item_dbid)//már meg van nyitva a tároló, be akarja zárni
                            {
                                CloseContainer(player);
                            }
                            else if (!IsInventoryInUse(1, item_dbid))//nincs használatban a tároló
                            {
                                OpenContainer(player, i);
                            }
                            else
                            {
                                player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                            }
                        }
                    }
                    else//nem ruhadarab de tároló
                    {
                        int target_owner_type = player.GetData<int>("player:OpenedContainerOwnerType");
                        uint target_owner_id = player.GetData<uint>("player:OpenedContainerID");
                        if (target_owner_type == 1 && target_owner_id == item_dbid)//már meg van nyitva a tároló, be akarja zárni
                        {
                            CloseContainer(player);
                        }
                        else if (!IsInventoryInUse(1, item_dbid))//nincs használatban a tároló
                        {
                            OpenContainer(player, i);
                        }
                        else
                        {
                            player.SendChatMessage("Valaki más már használja ezt a tárolót.");
                        }
                    }
                }
                else if(i.ItemID <= 14)//nem tároló de ruha, ha rajta van le akarjuk venni, ha nincs akkor fel
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
                        case 18:
                            UseHandgun(player, "weapon_pistol", i);
                            break;
                        case 20:
                            UseHandgun(player, "weapon_combatpistol", i);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Item i2 = await GetItemByDbId(item_dbid);
                int container_ownertype = player.GetData<int>("player:OpenedContainerOwnerType");
                uint container_targetid = player.GetData<uint>("player:OpenedContainerID");
                if (i2.OwnerType == container_ownertype && i2.OwnerID == container_targetid)//megegyezik a megnyitott tárolóval, nincs baj
                {

                }
                else
                {
                    Database.Log.Log_Server("ITEM HIBA! Játékos megpróbált egy tárgyat használni de az nem elérhető számára. " + player.Name + " (DBID: " + item_dbid + ")");
                }
            }
        }

        private async void UseHandgun(Player player, string namehash, Item i)
        {
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
                Server.Chat.Commands.ChatEmoteME(player, "eltesz egy fegyver. (" + ItemList.GetItemName(i.ItemID) + ")");

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
                Server.Chat.Commands.ChatEmoteME(player, "elővesz egy fegyver. (" + ItemList.GetItemName(i.ItemID) + ")");

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
                        player.SendChatMessage("ADATBÁZIS MENTÉSI HIBA! DBID: " + ordered[i].DBID);
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
                uint charid = player.GetData<UInt32>("player:charID");
                int originalowner = i1.OwnerType;
                uint originalownerid = i1.OwnerID;

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






                if (i1.InUse && i1.ItemID <= 14)//használatban van (viseli) + ruha itemid-nek megfelel -> le kell venni róla
                {
                    i1.InUse = false;
                    i1.Priority = 1000;

                    Tuple<bool, int> slot = GetClothingSlotFromItemId(i1.ItemID);
                    bool gender = player.GetData<bool>("player:gender");
                    int[] clothing = GetDefaultClothes(gender, i1.ItemID);


                        if (slot.Item1)//ruha
                        {
                            if (clothing.Length == 1)//kesztyű
                            {
                                NAPI.Task.Run(() =>
                                {
                                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                                    //beállítjuk a rajta lévő pólót mivel levette a kesztyűt, csak a torso-t akarjuk az alap értékre állítani
                                    Item Polo = GetClothingOnSlot(player, 5);
                                    if (Polo != null)//ha van rajta póló
                                    {
                                        Top t = NAPI.Util.FromJson<Top>(Polo.ItemValue);
                                        player.SetClothes(slot.Item2, t.Torso, 0);
                                    }
                                    else//nincs rajta póló, átrakjuk az alap torso-ra
                                    {
                                        player.SetClothes(slot.Item2, clothing[0], 0);//átrakjuk a torso-ját az alap torso-ra
                                    }
                                    player.TriggerEvent("client:RefreshInventoryPreview");
                                    string json = NAPI.Util.ToJson(i1);
                                    player.TriggerEvent("client:AddItemToInventory", json);
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
                                    player.TriggerEvent("client:AddItemToInventory", json);
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
                                    player.TriggerEvent("client:AddItemToInventory", json);
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
                                    player.TriggerEvent("client:AddItemToInventory", json);
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
            else if(target_inv == 1)//a megnyitott tárolóra húzza
            {
                Item i1 = await GetItemByDbId(item_dbid);
                
                int target_owner_type = player.GetData<int>("player:OpenedContainerOwnerType");
                uint target_owner_id = player.GetData<uint>("player:OpenedContainerID");
                i1.OwnerType = target_owner_type;
                i1.OwnerID = target_owner_id;
                
                if (i1.InUse && i1.ItemID <= 14)//használatban van (viseli) + ruha itemid-nek megfelel -> le kell venni róla
                {
                    i1.InUse = false;
                    i1.Priority = 1000;

                    Tuple<bool, int> slot = GetClothingSlotFromItemId(i1.ItemID);
                    bool gender = player.GetData<bool>("player:gender");
                    int[] clothing = GetDefaultClothes(gender, i1.ItemID);
                    if (await SortPlayerInventory(player))
                    {
                        if (slot.Item1)//ruha
                        {
                            if (clothing.Length == 2)//sima ruha
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
                    Chat.Commands.ChatEmoteME(player, "betesz " + i1.ItemAmount+ " db " + ItemList.GetItemName(i1.ItemID) + " tárgyat a " + tarolo + " (("+ target_owner_id+")) ");
                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                });
                
                AddItemToInventory(player, target_owner_type, target_owner_id, i1);
            }





            
            //OrderInventory(player);
        }


        [RemoteEvent("server:SwapItem")]
        public async void SwapItem(Player player, uint item1_dbid, uint item2_dbid)
        {
            Item i1 = await GetItemByDbId(item1_dbid);
            Item i2 = await GetItemByDbId(item2_dbid);

            if (1 <= i1.ItemID && i1.ItemID <= 14 && i1.ItemID == i2.ItemID && i1.InUse) //használatban lévő ruhát húzott egy másik itemre, meg szeretné cserélni
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


                /*
                Item temp = new Item(i1.DBID, i1.OwnerID, i1.OwnerType, i1.ItemID, i1.ItemValue, i1.ItemAmount, i1.InUse,i1.Duty, i1.Priority);
                player.TriggerEvent("client:RemoveItem", i1.DBID);
                player.TriggerEvent("client:RemoveItem", i2.DBID);

                    i1.InUse = false;
                    i2.InUse = false;

                    string json = NAPI.Util.ToJson(i2);
                    //player.TriggerEvent("client:AddItemToInventory", json2);
                */
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

        [RemoteEvent("server:SetWornClothing")]
        public async void SetWornClothing(Player player)
        {
            for (uint i = 1; i <= 14; i++)
            {
                Tuple<bool, int> slot = GetClothingSlotFromItemId(i);
                int clothing_id = slot.Item2;
                Item worn = GetClothingOnSlot(player, i);

                if (worn != null)
                {
                    switch (worn.ItemID)
                    {
                        case 1:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 2:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        case 3:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 4:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 5:
                            try
                            {
                                worn.InUse = true;
                                NAPI.Task.Run(() =>
                                {
                                    //player.TriggerEvent("client:RemoveItem", worn.DBID);
                                    Top t = NAPI.Util.FromJson<Top>(worn.ItemValue);
                                    player.SetClothes(clothing_id, t.Drawable, t.Texture);
                                    player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);
                                    player.SetClothes(3, t.Torso, 0);
                                    string json = NAPI.Util.ToJson(worn);
                                    player.TriggerEvent("client:AddItemToClothing", json);
                                    player.TriggerEvent("client:RefreshInventoryPreview");

                                }, 50);
                                if (await UpdateItem(worn))
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        //player.SendChatMessage("ItemUpdate: " + worn.DBID);
                                    }, 500);

                                }
                                else
                                {
                                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + worn.DBID + ")");
                                }

                            }
                            catch (Exception ex)
                            {
                                Database.Log.Log_Server("Hibás ItemValue! DBID:" + worn.DBID);
                            }
                            break;
                        case 6:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 7:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        case 8:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 9:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        case 10:
                            ItemValueToAccessory(player, worn, clothing_id);
                            break;
                        case 11:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        case 12:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        case 13:
                                worn.InUse = true;
                                NAPI.Task.Run(() =>
                                {
                                    bool gender = player.GetData<bool>("player:gender");

                                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                                    Item Top = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                                    Top t;
                                    if (Top != null)//van rajta póló
                                    {
                                        t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                                    }
                                    else
                                    {
                                        int[] slots = GetDefaultClothes(gender, 5);
                                        t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                                    }

                                    int gloves = Convert.ToInt32(worn.ItemValue);
                                    int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, gloves);
                                    if (correctTorso != -1)//ha kompatibilis kesztyű van hozzá
                                    {
                                        player.SetClothes(3, correctTorso, 0);
                                    }
                                    else
                                    {
                                        player.SetClothes(3, t.Torso, 0);
                                    }
                                    string json = NAPI.Util.ToJson(worn);
                                    player.TriggerEvent("client:AddItemToClothing", json);
                                    player.TriggerEvent("client:RefreshInventoryPreview");
                                });
                            break;
                        case 14:
                            ItemValueToClothing(player, worn, clothing_id);
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        public static int[] GetDefaultClothes(bool gender, uint itemid)
        {
            int[] res = new int[0];
            if (!gender)//nő
            {
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
                        res = new int[5] { 15, 0, 15, 2, 0 };
                        break;
                    case 6://fülbevaló
                        res = new int[2] { -1, 0 };
                        break;
                    case 7://nadrág
                        res = new int[2] { 15, 0 };
                        break;
                    case 8://karkötő
                        res = new int[2] { -1, 0 };
                        break;
                    case 9://cipő
                        res = new int[2] { 35, 0 };
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
                    case 13://kesztyű
                        res = new int[1] { 15 };
                        break;
                    case 14://decal
                        res = new int[2] { 0, 0 };
                        break;
                }
            }
            else
            {
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
                    case 13://kesztyű
                        res = new int[1] { 15 };
                        break;
                    case 14://decal
                        res = new int[2] { 0, 0 };
                        break;
                }
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
                case 13://kesztyű
                    res = Tuple.Create(true, 3);
                    break;
                case 14://kitűző
                    res = Tuple.Create(true, 10);
                    break;
                default:
                    return res;
                    break;
            }
            return res;
        }

        [RemoteEvent("server:MoveItemToClothing")]
        public async void MoveItemToClothing(Player player, uint db_id, int target_slot)
        {
            //slotokat kezelni, a megfelelő ruhát ráadni a playerre, törölni az inventory-jából vagy container-ből az itemet nála és hozzáadni a slothoz
            Item i = await GetItemByDbId(db_id);
            
            if (ItemList.GetItemType(i.ItemID) == 1 && i.InUse == false)//1-es típus: ruha és nincs felvéve
            {
                int clothing_id = -1;
                Tuple<bool, int> slot = GetClothingSlotFromItemId(i.ItemID);
                clothing_id = slot.Item2;

                if (clothing_id != -1)//nem -1, tehát találtunk valamit
                {
                    //az ItemID-től és a slot-tól függően ha jó itemet húzott a player a jó slotra
                    //ruha vagy prop beállítása, itemvalue konvertálásával


                    if ((i.ItemID == 1 && target_slot == 0) || (i.ItemID == 1 && target_slot == -1))//kalap itemid és kalap target_slot
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            if (toSwap.ItemID == i.ItemID)
                            {
                                toSwap.Priority = 1000;
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);
                        }
                        //ha igen, akkor: ruhát ráadni, elküldeni kliensnek a törlést és a hozzáadást a slothoz
                    }
                    else if ((i.ItemID == 2 && target_slot == 6) || (i.ItemID == 2 && target_slot == -1))//maszk
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);

                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 3 && target_slot == 1) || ((i.ItemID == 3 && target_slot == -1)))//nyaklánc - accessories
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            if (toSwap.ItemID == i.ItemID)
                            {
                                toSwap.Priority = 1000;
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);

                        }
                    }
                    else if ((i.ItemID == 4 && target_slot == 7) || (i.ItemID == 4 && target_slot == -1))//szemüveg - prop
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);

                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {

                            if (toSwap.ItemID == i.ItemID)
                            {
                                toSwap.Priority = 1000;
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 5 && target_slot == 2) || (i.ItemID == 5 && target_slot == -1))//póló + undershirt + torso
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        Item GloveItem = GetClothingOnSlot(player, 13);//lekérjük a kesztyűjét


                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            if (toSwap.ItemID == i.ItemID)
                            {
                                try
                                {
                                    i.InUse = true;
                                    toSwap.InUse = false;
                                    toSwap.Priority = 1000;
                                    NAPI.Task.Run(() =>
                                    {
                                        //player.TriggerEvent("client:RemoveItem", i.DBID);
                                        //player.TriggerEvent("client:RemoveItem", toSwap.DBID);
                                        uint charid = player.GetData<UInt32>("player:charID");
                                        AddItemToInventory(player, 0, charid, i);
                                        AddItemToInventory(player, 0, charid, toSwap);

                                        Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                                        player.SetClothes(clothing_id, t.Drawable, t.Texture);
                                        player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);

                                        if (GloveItem != null)//van rajta kesztyű
                                        {
                                            bool gender = player.GetData<bool>("player:gender");
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
                                        string json2 = NAPI.Util.ToJson(toSwap);
                                        player.TriggerEvent("client:AddItemToInventory", json2);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    });
                                    
                                }
                                catch (Exception ex)
                                {
                                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                                }

                            }
                        }
                        else
                        {
                            try
                            {
                                i.InUse = true;
                                NAPI.Task.Run(() =>
                                {
                                    uint charid = player.GetData<UInt32>("player:charID");
                                    AddItemToInventory(player, 0, charid, i);

                                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                                    Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                                    player.SetClothes(clothing_id, t.Drawable, t.Texture);
                                    player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);

                                    if (GloveItem != null)//van rajta kesztyű
                                    {
                                        bool gender = player.GetData<bool>("player:gender");
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
                                });
                                
                            }
                            catch (Exception ex)
                            {
                                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                            }

                        }
                    }
                    else if ((i.ItemID == 6 && target_slot == 8) || (i.ItemID == 6 && target_slot == -1))//fülbevaló
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);

                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);

                        }
                    }
                    else if ((i.ItemID == 7 && target_slot == 3) || (i.ItemID == 7 && target_slot == -1))//nadrág
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 8 && target_slot == 9) || (i.ItemID == 8 && target_slot == -1))//karkötő
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);

                        }
                    }
                    else if ((i.ItemID == 9 && target_slot == 4) || (i.ItemID == 9 && target_slot == -1))//cipő
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 10 && target_slot == 10) || (i.ItemID == 10 && target_slot == -1))//óra
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 11 && target_slot == 5) || (i.ItemID == 11 && target_slot == -1))//táska
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 12 && target_slot == 11) || (i.ItemID == 12 && target_slot == -1))//armor
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {

                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                    else if ((i.ItemID == 13 && target_slot == 12) || (i.ItemID == 13 && target_slot == -1))//kesztyű
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;

                            if (toSwap.ItemID == i.ItemID)
                            {
                                try
                                {
                                    bool gender = player.GetData<bool>("player:gender");
                                    i.InUse = true;
                                    toSwap.InUse = false;
                                    toSwap.Priority = 1000;
                                    NAPI.Task.Run(() =>
                                    {
                                        uint charid = player.GetData<UInt32>("player:charID");
                                        AddItemToInventory(player, 0, charid, i);
                                        AddItemToInventory(player, 0, charid, toSwap);

                                        Item Top = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                                        Top t;
                                        if (Top != null)//van rajta póló
                                        {
                                            t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                                        }
                                        else
                                        {
                                            int[] slots = GetDefaultClothes(gender, 5);
                                            t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                                        }
                                        
                                        int gloves = Convert.ToInt32(i.ItemValue);
                                        int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, gloves);
                                        if (correctTorso != -1)//ha kompatibilis kesztyű van hozzá
                                        {
                                            player.SetClothes(3, correctTorso, 0);
                                        }
                                        else
                                        {
                                            player.SetClothes(3, t.Torso, 0);
                                        }
                                        

                                        string json = NAPI.Util.ToJson(i);
                                        player.TriggerEvent("client:AddItemToClothing", json);
                                        string json2 = NAPI.Util.ToJson(toSwap);
                                        player.TriggerEvent("client:AddItemToInventory", json2);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    });

                                }
                                catch (Exception ex)
                                {
                                    Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
                                }
                            }
                        }
                        else
                        {
                            i.InUse = true;
                            NAPI.Task.Run(() =>
                            {
                                bool gender = player.GetData<bool>("player:gender");
                                uint charid = player.GetData<UInt32>("player:charID");
                                AddItemToInventory(player, 0, charid, i);

                                //player.TriggerEvent("client:RemoveItem", i.DBID);
                                Item Top = GetClothingOnSlot(player, 5);//lekérjük a pólóját
                                Top t;
                                if (Top != null)//van rajta póló
                                {
                                    t = NAPI.Util.FromJson<Top>(Top.ItemValue);//itemvalue 0-10 közötti érték, melyik kesztyű
                                }
                                else
                                {
                                    int[] slots = GetDefaultClothes(gender, 5);
                                    t = new Top(slots[0], slots[1], slots[3], slots[4], slots[2]);//beállítjuk az alapértékekre a felsőjét
                                }

                                int gloves = Convert.ToInt32(i.ItemValue);
                                int correctTorso = Gloves.GetCorrectTorsoForGloves(gender, t.Torso, gloves);
                                if (correctTorso != -1)//ha kompatibilis kesztyű van hozzá
                                {
                                    player.SetClothes(3, correctTorso, 0);
                                }
                                else
                                {
                                    player.SetClothes(3, t.Torso, 0);
                                }

                                string json = NAPI.Util.ToJson(i);
                                player.TriggerEvent("client:AddItemToClothing", json);
                                player.TriggerEvent("client:RefreshInventoryPreview");
                            });
                        }
                    }
                    else if ((i.ItemID == 14 && target_slot == 13) || (i.ItemID == 14 && target_slot == -1))//kitűző
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            toSwap.Priority = 1000;
                            if (toSwap.ItemID == i.ItemID)
                            {
                                ItemValueToClothingSwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToClothing(player, i, clothing_id);
                        }
                    }
                }

            }
            //OrderInventory(player);
        }

        public async void ItemValueToClothingSwap(Player player, Item i1, Item i2, int clothing_id)
        {
            try
            {
                i1.InUse = true;
                i2.InUse = false;
                i2.Priority = 1000;
                    NAPI.Task.Run(() =>
                    {
                        uint charid = player.GetData<UInt32>("player:charID");
                        AddItemToInventory(player, 0, charid, i1);
                        AddItemToInventory(player, 0, charid, i2);

                        //player.TriggerEvent("client:RemoveItem", i1.DBID);
                        //player.TriggerEvent("client:RemoveItem", i2.DBID);
                        Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);
                        player.SetClothes(clothing_id, c.Drawable, c.Texture);

                        string json = NAPI.Util.ToJson(i1);
                        player.TriggerEvent("client:AddItemToClothing", json);
                        string json2 = NAPI.Util.ToJson(i2);
                        player.TriggerEvent("client:AddItemToInventory", json2);
                        player.TriggerEvent("client:RefreshInventoryPreview");
                    });
                
            }
            catch (Exception ex)
            {
                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID);
            }
        }


        public async void ItemValueToClothing(Player player, Item i, int clothing_id)
        {
            try
            {
                i.InUse = true;
                    NAPI.Task.Run(() =>
                    {
                        uint charid = player.GetData<UInt32>("player:charID");
                        AddItemToInventory(player, 0, charid, i);

                        //player.TriggerEvent("client:RemoveItem", i.DBID);
                        Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                        player.SetClothes(clothing_id, c.Drawable, c.Texture);
                        string json = NAPI.Util.ToJson(i);
                        player.TriggerEvent("client:AddItemToClothing", json);
                        player.TriggerEvent("client:RefreshInventoryPreview");
                    });
                

            }
            catch (Exception ex)
            {
                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
            }

        }

        public async void ItemValueToAccessorySwap(Player player, Item i1, Item i2, int clothing_id)
        {
            try
            {
                i1.InUse = true;
                i2.InUse = false;
                i2.Priority = 1000;

                NAPI.Task.Run(() =>
                {
                    uint charid = player.GetData<UInt32>("player:charID");
                    AddItemToInventory(player, 0, charid, i1);
                    AddItemToInventory(player, 0, charid, i2);
                    //player.TriggerEvent("client:RemoveItem", i1.DBID);
                    //player.TriggerEvent("client:RemoveItem", i2.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);
                    player.SetAccessories(clothing_id, c.Drawable, c.Texture);

                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    string json2 = NAPI.Util.ToJson(i2);
                    player.TriggerEvent("client:AddItemToInventory", json2);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                });
                
            }
            catch (Exception ex)
            {
                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i1.DBID);
            }

        }


        public async void ItemValueToAccessory(Player player, Item i, int clothing_id)
        {
            try
            {
                i.InUse = true;
                NAPI.Task.Run(() =>
                {
                    uint charid = player.GetData<UInt32>("player:charID");
                    AddItemToInventory(player, 0, charid, i);
                    //player.TriggerEvent("client:RemoveItem", i.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                    player.SetAccessories(clothing_id, c.Drawable, c.Texture);

                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                });
                
            }
            catch (Exception ex)
            {
                Database.Log.Log_Server("Hibás ItemValue! DBID:" + i.DBID);
            }

        }



        public static Item GetClothingOnSlot(Player player, uint itemid)
        {
            foreach (var item in GetPlayerInventory(player))
            {
                if (item.ItemID == itemid && item.InUse == true)
                {
                    return item;
                }
            }
            return null;
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
                await con.CloseAsync();
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
                await con.CloseAsync();
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
                await con.CloseAsync();
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
                await con.CloseAsync();
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
