using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;

namespace Server.Inventory
{
    public class Items : Script
    {
        static Dictionary<int, int> Bags = new Dictionary<int, int>();

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

        public static List<Item> GetInventory(int OwnerType, uint OwnerID)
        {
            return Inventories[new Tuple<int, uint>(OwnerType, OwnerID)];
        }

        public static void SetInventory(int OwnerType, uint OwnerID, Item[] items)
        {
            Inventories[new Tuple<int, uint>(OwnerType, OwnerID)] = items.ToList();
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
        public async static void GiveItem(Player player,int targetid, uint itemid, string itemvalue, int amount)
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
                    target.SendChatMessage("Kaptál "+amount +" db " + ItemList.GetItemName(i.ItemID) + " tárgyat "+ player.Name+" -tól!");
                }, 500);
            }

            //Item newitem = new Item(0, charid, 0, itemid, itemvalue, amount, false, -1);

        }

        public async static Task<uint> AddItemToDatabase(uint charid,Item itemdata)//létrehozunk egy új itemet az adatbázisban
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

        public static void LoadInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            RefreshInventory(player, charid);
        }


        public async static void RefreshInventory(Player player,uint charid)
        {
            Item[] playerItems = await GetPlayerInventory(charid);
            SetInventory(0, charid, playerItems);
            await SortPlayerInventory(player);
            Inventory.ItemList.SendItemListToPlayer(player);
            SendInventoryToPlayer(player, playerItems);
        }
        

        public async static void SendInventoryToPlayer(Player player, Item[] items)
        {
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(items);
                player.TriggerEvent("client:InventoryFromServer", json);
            }, 500);
        }
        

        public static async Task<Item[]> GetPlayerInventory(uint charid)//felhasználónév alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
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
                                    Item loadedItem = new Item(Convert.ToUInt32(reader["DbID"]), Convert.ToUInt32(reader["ownerID"]), Convert.ToInt32(reader["ownerType"]), Convert.ToUInt32(reader["itemID"]),reader["itemValue"].ToString(),Convert.ToInt32(reader["itemAmount"]),Convert.ToBoolean(reader["inUse"]),Convert.ToBoolean(reader["duty"]),Convert.ToInt32(reader["priority"]));
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

        public Item GetItemByDbId(Player player, uint dbid)
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

        public Item GetItemByDbId(int ownertype, uint ownerid, uint dbid)
        {
            foreach (var item in GetInventory(ownertype,ownerid))
            {
                if (item.DBID == dbid)
                {
                    return item;
                }
            }
            return null;
        }


        public Item GetItemByDbId(uint dbid)//az összes itemen végig megyünk, hosszú futási idő - el kéne kerülni a használatát
        {
            foreach (var item in Inventories)
            {
                foreach (var items in item.Value)
                {
                    if (items.DBID == dbid)
                    {
                        return items;
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

        
        [RemoteEvent("server:UseItem")]
        public void ItemUse(Player player, uint item_dbid)
        {
            Item i = GetItemByDbId(player, item_dbid);
            switch (i.ItemID)
            {
                case 18:
                    if (i.InUse)
                    {
                        Item[] tarak = GetItemsByItemID(player, 19);
                        NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey("weapon_unarmed"));
                        int remaining_ammo = NAPI.Player.GetPlayerWeaponAmmo(player, NAPI.Util.GetHashKey("weapon_pistol"));

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
                        Server.Chat.Commands.ChatEmoteME(player, "eltesz egy fegyver. ("+ItemList.GetItemName(i.ItemID)+")");
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


                        NAPI.Player.GivePlayerWeapon(player, NAPI.Util.GetHashKey("weapon_pistol"), loszer);
                        NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey("weapon_pistol"));
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
                    break;

                case 20:
                    if (i.InUse)
                    {
                        Item[] tarak = GetItemsByItemID(player, 19);
                        
                        int remaining_ammo = NAPI.Player.GetPlayerWeaponAmmo(player, NAPI.Util.GetHashKey("weapon_combatpistol"));

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


                        NAPI.Player.GivePlayerWeapon(player, NAPI.Util.GetHashKey("weapon_combatpistol"), loszer);
                        NAPI.Player.SetPlayerCurrentWeapon(player, NAPI.Util.GetHashKey("weapon_pistol"));
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
                    break;
                default:
                    break;
            }


            if (i.ItemID == 18 && i.InUse == false)//pisztoly és nincs elővéve
            {

            }
            else if (i.ItemID == 18 && i.InUse == true)//pisztoly és elő van véve
            {

            }

        }
        
        private async Task<bool> SortInventory(int ownertype, uint ownerid)//sorba rendezzük prioritás alapján az itemeket és új számokat adunk nekik növekvő sorrendben
        {
            List<Item> items = GetInventory(ownertype, ownerid);
            List<Item> ordered = items.OrderBy(o => o.Priority).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Priority = i;
            }
            SetInventory(ownertype, ownerid, ordered.ToArray());
            return true;
        }

        private async static Task<bool> SortPlayerInventory(Player player)//sorba rendezzük prioritás alapján az itemeket és új számokat adunk nekik növekvő sorrendben
        {
            List<Item> items = GetPlayerInventory(player);
            List<Item> ordered = items.OrderBy(o => o.Priority).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Priority = i;
                if (await UpdateItem(ordered[i]))
                {
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage(ordered[i].DBID + " rendezve.");
                    }, 500);
                }
            }
            SetPlayerInventory(player, ordered);
            return true;
        }


        private async void OrderInventory(Player player)
        {
            uint charid = player.GetData<UInt32>("player:charID");
            if (await SortPlayerInventory(player))//megvárjuk amíg elrendezte az inventoryt a playernek
            {
                //RefreshInventory(player, charid);
                //leküldjük az itemeket sorrendjét egy listában amit majd kliens kibont -> átálligatja a priority-t és elrendezi
                /*Dictionary<uint, uint> priorities = new Dictionary<uint, uint>();
                List<Item> items = GetPlayerInventory(player);
                player.TriggerEvent("client:GetItemPriorities", priorities);*/
            }
        }


        
        [RemoteEvent("server:MoveItem")]
        public async void MoveItem(Player player, uint item1_dbid,uint owner_type, uint owner_id)
        {
            if (owner_type != null && owner_id != null)//nincs nyitva tároló tehát a játékoson belülre mozgatjuk
            {
                Item i1 = GetItemByDbId(player, item1_dbid);
                player.TriggerEvent("client:RemoveItem", i1.DBID);

                string json = NAPI.Util.ToJson(i1);
                player.TriggerEvent("client:AddItemToInventory", json);
                
                if (i1.InUse)//használatban van (viseli) + ruha itemid-nek megfelel -> le kell venni róla
                {
                    Tuple<bool, int> slot = GetClothingSlotFromItemId(i1.ItemID);
                    bool gender = player.GetData<bool>("player:gender");
                    int[] clothing = GetDefaultClothes(gender, i1.ItemID);

                    if (slot.Item1)//ruha
                    {
                        if (clothing.Length == 2)//sima ruha
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SetClothes(slot.Item2, clothing[0], clothing[1]);
                            }, 50);
                            
                        }
                        else
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.SetClothes(11, clothing[0], clothing[1]);
                                player.SetClothes(3, clothing[2], 0);
                                player.SetClothes(8, clothing[3], clothing[4]);
                            }, 50);
                        }
                        //player.SetClothes(slot.Item2,)
                    }
                    else//prop
                    {
                        NAPI.Task.Run(() =>
                        {
                            if (clothing.Length == 2)
                            {
                                player.SetAccessories(slot.Item2, clothing[0], clothing[1]);
                            }
                            
                        }, 50);
                    }
                }
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }, 100);
                
                i1.InUse = false;
                if (await UpdateItem(i1))
                {
                    
                }
                else
                {
                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i1.DBID + ")");
                }
            }
            else//van nyitott tároló, tehát a tárolóhoz adjuk
            {
                
            }
            OrderInventory(player);
        }

        public void SortInventory(Entity e)
        {
            //Inventories[e] = Inventories[e].OrderBy(x => x.Priority).ToDictionary(pair => pair, x => x.Value);)
            //dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            //Inventories[e].OrderBy<>
        }

        
        [RemoteEvent("server:SwapItem")]
        public async void SwapItem(Player player, uint item1_dbid, uint item2_dbid)
        {
            Item i1 = GetItemByDbId(player, item1_dbid);
            Item i2 = GetItemByDbId(player, item2_dbid);

            if (1 <= i1.ItemID && i1.ItemID <= 14 && i1.ItemID == i2.ItemID && i1.InUse)//használatban lévő ruhát húzott egy másik itemre, meg szeretné cserélni
            {
                MoveItemToClothing(player, item2_dbid, -1);
            }
            else//nem ruha item
            {
                if (i1.InUse == false && i2.InUse == false)
                {
                    int ownertype = i1.OwnerType;
                    uint ownerid = i1.OwnerID;
                    int priority = i1.Priority;
                    i1.OwnerType = i2.OwnerType;
                    i1.OwnerID = i2.OwnerID;
                    i1.Priority = i2.Priority;
                    i2.OwnerType = ownertype;
                    i2.OwnerID = ownerid;
                    i2.Priority = priority;

                    if (i1.OwnerType != 0)//nem játékosnál van, tehát container-hez adjuk
                    {
                        try
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("client:RemoveItem", i1.DBID);
                                string json = NAPI.Util.ToJson(i1);
                                player.TriggerEvent("client:AddItemToContainer", json);
                            }, 50);
                            if (await UpdateItem(i1) && await UpdateItem(i2))
                            {
                                NAPI.Task.Run(() =>
                                {
                                    //player.SendChatMessage("ItemUpdate: " + i1.DBID +" -> " + i2.DBID);
                                }, 500);
                            }
                            else
                            {
                                Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i1.DBID + " & " + i2.DBID + ")");
                            }
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
                                player.TriggerEvent("client:RemoveItem", i1.DBID);
                                string json = NAPI.Util.ToJson(i1);
                                player.TriggerEvent("client:AddItemToInventory", json);
                            }, 50);
                            if (await UpdateItem(i1) && await UpdateItem(i2))
                            {
                                NAPI.Task.Run(() =>
                                {
                                    //player.SendChatMessage("ItemUpdate: " + i1.DBID +" -> " + i2.DBID);
                                }, 500);
                            }
                            else
                            {
                                Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i1.DBID + " & " + i2.DBID + ")");
                            }
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
                                player.TriggerEvent("client:RemoveItem", i2.DBID);
                                string json = NAPI.Util.ToJson(i2);
                                player.TriggerEvent("client:AddItemToContainer", json);
                            }, 50);
                            if (await UpdateItem(i2))
                            {
                                NAPI.Task.Run(() =>
                                {
                                    //player.SendChatMessage("ItemUpdate: " + i1.DBID +" -> " + i2.DBID);
                                }, 500);

                            }
                            else
                            {
                                Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i2.DBID + ")");
                            }
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
                                player.TriggerEvent("client:RemoveItem", i2.DBID);
                                string json = NAPI.Util.ToJson(i2);
                                player.TriggerEvent("client:AddItemToInventory", json);
                            }, 50);
                            if (await UpdateItem(i2))
                            {
                                NAPI.Task.Run(() =>
                                {
                                    //player.SendChatMessage("ItemUpdate: " + i1.DBID +" -> " + i2.DBID);
                                }, 500);

                            }
                            else
                            {
                                Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i2.DBID + ")");
                            }
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
            for (uint i = 1; i <= 12; i++)
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
                                    player.TriggerEvent("client:RemoveItem", worn.DBID);
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
                        res = new int[2] { 0, 0 };
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
                        res = new int[2] { 0, 0 };
                        break;
                    case 14://decal
                        res = new int[2] { 0, 0 };
                        break;
                }
            }
            return res;
        }

        public static Tuple<bool,int> GetClothingSlotFromItemId(uint itemid)
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
                    res = Tuple.Create(false,1);
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
                case 13:
                    res = Tuple.Create(true, 3);
                    break;
                case 14:
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
            Item i = GetItemByDbId(player, db_id);
            if (ItemList.GetItemType(i.ItemID) == 1 && i.InUse == false)//1-es típus: ruha
            {
                int clothing_id = -5;
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
                                ItemValueToAccessorySwap(player, i, toSwap, clothing_id);
                            }
                        }
                        else
                        {
                            ItemValueToAccessory(player, i, clothing_id);
                        }


                        
                        //ha igen, akkor: ruhát ráadni, elküldeni kliensnek a törlést és a hozzáadást a slothoz
                    }
                    else if((i.ItemID == 2 && target_slot == 6) || (i.ItemID == 2 && target_slot == -1))//maszk
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
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
                    else if ((i.ItemID == 3 && target_slot == 1) ||((i.ItemID == 3 && target_slot == -1)))//nyaklánc - accessories
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
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
                    else if ((i.ItemID == 4 && target_slot == 7) || (i.ItemID == 4 && target_slot == -1))//szemüveg - prop
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
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
                    else if ((i.ItemID == 5 && target_slot == 2) || (i.ItemID == 5 && target_slot == -1))//póló + undershirt + torso
                    {
                        Item toSwap = GetClothingOnSlot(player, i.ItemID);
                        if (toSwap != null)//megszereztük az itemet ami rajta van, meg akarjuk cserélni. TODO: törölni mind a kettőt és hozzáadni a megfelelő helyekre
                        {
                            if (toSwap.ItemID == i.ItemID)
                            {
                                try
                                {
                                    i.InUse = true;
                                    toSwap.InUse = false;
                                    NAPI.Task.Run(() =>
                                    {
                                        player.TriggerEvent("client:RemoveItem", i.DBID);
                                        player.TriggerEvent("client:RemoveItem", toSwap.DBID);
                                        Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                                        player.SetClothes(clothing_id, t.Drawable, t.Texture);
                                        player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);
                                        player.SetClothes(3, t.Torso, 0);

                                        string json = NAPI.Util.ToJson(i);
                                        player.TriggerEvent("client:AddItemToClothing", json);
                                        string json2 = NAPI.Util.ToJson(toSwap);
                                        player.TriggerEvent("client:AddItemToInventory", json2);
                                        player.TriggerEvent("client:RefreshInventoryPreview");
                                    }, 50);
                                    if (await UpdateItem(i) && await UpdateItem(toSwap))
                                    {
                                        NAPI.Task.Run(() =>
                                        {
                                            //player.SendChatMessage("ItemUpdate: " + i.DBID + " -> " + toSwap.DBID);
                                        }, 500);

                                    }
                                    else
                                    {
                                        Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i.DBID + " & " + toSwap.DBID + ")");
                                    }
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
                                    player.TriggerEvent("client:RemoveItem", i.DBID);
                                    Top t = NAPI.Util.FromJson<Top>(i.ItemValue);
                                    player.SetClothes(clothing_id, t.Drawable, t.Texture);
                                    player.SetClothes(8, t.UndershirtDrawable, t.UndershirtTexture);
                                    player.SetClothes(3, t.Torso, 0);

                                    string json = NAPI.Util.ToJson(i);
                                    player.TriggerEvent("client:AddItemToClothing", json);
                                    player.TriggerEvent("client:RefreshInventoryPreview");

                                }, 50);
                                if (await UpdateItem(i))
                                {
                                    NAPI.Task.Run(() =>
                                    {
                                        //player.SendChatMessage("ItemUpdate: " + i.DBID);
                                    }, 500);

                                }
                                else
                                {
                                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i.DBID + ")");
                                }

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
            OrderInventory(player);
        }

        public async void ItemValueToClothingSwap(Player player, Item i1, Item i2, int clothing_id)
        {
            try
            {
                i1.InUse = true;
                i2.InUse = false;
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:RemoveItem", i1.DBID);
                    player.TriggerEvent("client:RemoveItem", i2.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);
                    player.SetClothes(clothing_id, c.Drawable, c.Texture);
                    
                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    string json2 = NAPI.Util.ToJson(i2);
                    player.TriggerEvent("client:AddItemToInventory", json2);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }, 50);
                if (await UpdateItem(i1) && await UpdateItem(i2))
                {
                    NAPI.Task.Run(() =>
                    {
                        //player.SendChatMessage("ItemUpdate: " + i1.DBID +" -> " + i2.DBID);
                    }, 500);

                }
                else
                {
                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i1.DBID + " & "+i2.DBID+")");
                }
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
                    player.TriggerEvent("client:RemoveItem", i.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                    player.SetClothes(clothing_id, c.Drawable, c.Texture);
                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");

                }, 50);
                if (await UpdateItem(i))
                {
                    NAPI.Task.Run(() =>
                    {
                        //player.SendChatMessage("ItemUpdate: " + i.DBID);
                    }, 500);

                }
                else
                {
                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i.DBID + ")");
                }

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
                NAPI.Task.Run(() =>
                {
                    player.TriggerEvent("client:RemoveItem", i1.DBID);
                    player.TriggerEvent("client:RemoveItem", i2.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i1.ItemValue);
                    player.SetAccessories(clothing_id, c.Drawable, c.Texture);

                    string json = NAPI.Util.ToJson(i1);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    string json2 = NAPI.Util.ToJson(i2);
                    player.TriggerEvent("client:AddItemToInventory", json2);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }, 50);
                if (await UpdateItem(i1) && await UpdateItem(i2))
                {
                    NAPI.Task.Run(() =>
                    {
                        //player.SendChatMessage("ItemUpdate: " + i1.DBID + " -> "+ i2.DBID);
                    }, 500);

                }
                else
                {
                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i1.DBID + " & " + i2.DBID + ")");
                }
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
                    player.TriggerEvent("client:RemoveItem", i.DBID);
                    Clothing c = NAPI.Util.FromJson<Clothing>(i.ItemValue);
                    player.SetAccessories(clothing_id, c.Drawable, c.Texture);
                    
                    string json = NAPI.Util.ToJson(i);
                    player.TriggerEvent("client:AddItemToClothing", json);
                    player.TriggerEvent("client:RefreshInventoryPreview");
                }, 50);
                if (await UpdateItem(i))
                {
                    NAPI.Task.Run(() =>
                    {
                        //player.SendChatMessage("ItemUpdate: " + i.DBID);
                    }, 500);

                }
                else
                {
                    Database.Log.Log_Server("Adatbázis mentési hiba. (ITEM-DB-ID: " + i.DBID + ")");
                }
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






        [RemoteEvent("server:MoveItemInInventory")]
        public void MoveItemInInventory(Player player, uint source_dbid, uint target_dbid)
        {
            uint charid = player.GetData<UInt32>("player:charID");

            Item i1 = GetItemByDbId(player, source_dbid);
            Item i2 = GetItemByDbId(player, target_dbid);

            if (i1 != null && i2 != null)
            {
                if (i1.ItemID == i2.ItemID)//ugyan az a két item
                {

                }
                else
                {

                }


            }
            else//hiba, az egész inventory-t újratöltjük a playernek
            {
                player.SendChatMessage("refresh inv");
                RefreshInventory(player, charid);
            }


            /*


            if (i != null)
            {
                player.SendChatMessage("item not null");
                if (TryItemMove(player,section,startslot,endslot))
                {
                    //adatbázis kezelés
                    player.SendChatMessage(ItemList.GetItemName(i.ItemID) + " áthelyezve, kezdő slot: " + startslot + " cél slot: " + endslot);
                }
                else
                {

                    player.SendChatMessage("item destination not empty");
                }
            }
            else//hiba van, frissítjük a player inventoryját
            {

            */
        }

        /*
        public bool TryItemMove(Player player,int section, int startslot, int endslot)
        {
            uint charid = player.GetData<UInt32>("player:charID");

            if (GetItemByData(charid, section, endslot) == null)
            {
                Item i = GetItemByData(charid, section, startslot);
                i.ItemSlot = endslot;
                return true;
            }

            return false;

        }


        
        public void HandleItemUse(Player player, Item i)
        {
            if (i.InUse)//használatban van, el akarjuk tenni
            {
                switch (ItemList.GetItemType(i.ItemID))
                {
                    case 1://fegyver
                        player.RemoveWeapon(WeaponHash.Pistol);
                        i.InUse = false;
                        player.TriggerEvent("client:ItemUseToCEF", ItemList.GetItemSection(i.ItemID), i.ItemSlot, 0);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (ItemList.GetItemType(i.ItemID))
                {
                    case 1://fegyver
                        player.GiveWeapon(WeaponHash.Pistol, 50);
                        i.InUse = true;
                        player.TriggerEvent("client:ItemUseToCEF",ItemList.GetItemSection(i.ItemID),i.ItemSlot, 1);
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }*/
    }




    
}
