using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Characters
{
    class ClothingShop
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public ColShape CollisionShape { get; set; }
        public Blip ActualBlip { get; set; }
        public uint Dimension { get; set; }
        public int Blip { get; set; }
        public byte BlipColor { get; set; }
        public bool HatLow { get; set; }
        public bool HatMed { get; set; }
        public bool HatHigh { get; set; }
        public bool MaskLow { get; set; }
        public bool MaskMed { get; set; }
        public bool MaskHigh { get; set; }
        public bool AccLow { get; set; }
        public bool AccMed { get; set; }
        public bool AccHigh { get; set; }
        public bool GlassesLow { get; set; }
        public bool GlassesMed { get; set; }
        public bool GlassesHigh { get; set; }
        public bool TopLow { get; set; }
        public bool TopMed { get; set; }
        public bool TopHigh { get; set; }
        public bool EarLow { get; set; }
        public bool EarMed { get; set; }
        public bool EarHigh { get; set; }
        public bool PantsLow { get; set; }
        public bool PantsMed { get; set; }
        public bool PantsHigh { get; set; }
        public bool BraceletLow { get; set; }
        public bool BraceletMed { get; set; }
        public bool BraceletHigh { get; set; }
        public bool ShoesLow { get; set; }
        public bool ShoesMed { get; set; }
        public bool ShoesHigh { get; set; }
        public bool WatchLow { get; set; }
        public bool WatchMed { get; set; }
        public bool WatchHigh { get; set; }
        public bool BagLow { get; set; }
        public bool BagMed { get; set; }
        public bool BagHigh { get; set; }
        public bool ArmorLow { get; set; }
        public bool ArmorMed { get; set; }
        public bool ArmorHigh { get; set; }
        public bool DecalLow { get; set; }
        public bool DecalMed { get; set; }
        public bool DecalHigh { get; set; }
        public ClothingShop(uint id, string name, float posX, float posY, float posZ, uint dim, int blip, byte blipcolor, bool hatlow, bool hatmed, bool hathigh, bool masklow, bool maskmed, bool maskhigh, bool acclow, bool accmed, bool acchigh, bool glasslow, bool glassmed, bool glasshigh, bool toplow, bool topmed, bool tophigh, bool earlow, bool earmed, bool earhigh, bool pantslow, bool pantsmed, bool pantshigh, bool braceletlow, bool braceletmed, bool bracelethigh, bool shoeslow, bool shoesmed, bool shoeshigh, bool watchlow, bool watchmed, bool watchhigh, bool baglow, bool bagmed, bool baghigh, bool armorlow, bool armormed, bool armorhigh, bool decallow, bool decalmed, bool decalhigh)
        {
            ID = id;
            Name = name;
            Position = new Vector3(posX, posY, posZ);
            Dimension = dim;
            Blip = blip;
            BlipColor = blipcolor;
            HatLow = hatlow;
            HatMed = hatmed;
            HatHigh = hathigh;
            MaskLow = masklow;
            MaskMed = maskmed;
            MaskHigh = maskhigh;
            AccLow = acclow;
            AccMed = accmed;
            AccHigh = acchigh;
            GlassesLow = glasslow;
            GlassesMed = glassmed;
            GlassesHigh = glasshigh;
            TopLow = toplow;
            TopMed = topmed;
            TopHigh = tophigh;
            EarLow = earlow;
            EarMed = earmed;
            EarHigh = earhigh;
            PantsLow = pantslow;
            PantsMed = pantsmed;
            PantsHigh = pantshigh;
            BraceletLow = braceletlow;
            BraceletMed = braceletmed;
            BraceletHigh = bracelethigh;
            ShoesLow = shoeslow;
            ShoesMed = shoesmed;
            ShoesHigh = shoeshigh;
            WatchLow = watchlow;
            WatchMed = watchmed;
            WatchHigh = watchhigh;
            BagLow = baglow;
            BagMed = bagmed;
            BagHigh = baghigh;
            ArmorLow = armorlow;
            ArmorMed = armormed;
            ArmorHigh = armorhigh;
            DecalLow = decallow;
            DecalMed = decalmed;
            DecalHigh = decalhigh;
        }

    }

    class ClothingItem
    {
        public uint ID { get; set; }
        public bool Gender { get; set; }
        public string Name { get; set; }
        public uint Component { get; set; }
        public uint Category { get; set; }
        public string ItemValue { get; set; }
        public uint Price { get; set; }
        public string Image { get; set; }
        public ClothingItem(uint id, bool gender, string name, uint component, uint category, string itemValue, uint price, string image)
        {
            ID = id;
            Gender = gender;
            Name = name;
            Component = component;
            Category = category;
            ItemValue = itemValue;
            Price = price;
            Image = image;  
        }
    }

    internal class Clothing : Script
    {
        static List<ClothingShop> ClothingShops = new List<ClothingShop>();

        public static async Task LoadClothingShops()
        {
            string query = $"SELECT * FROM `clothingshops`";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    ClothingShop shop = new ClothingShop(Convert.ToUInt32(reader["id"]), Convert.ToString(reader["name"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]), Convert.ToUInt32(reader["dim"]), Convert.ToInt32(reader["blip"]), Convert.ToByte(reader["blipColor"]),
                                        Convert.ToBoolean(reader["hatLow"]), Convert.ToBoolean(reader["hatMed"]), Convert.ToBoolean(reader["hatHigh"]),
                                       Convert.ToBoolean(reader["maskLow"]), Convert.ToBoolean(reader["maskMed"]), Convert.ToBoolean(reader["maskHigh"]),
                                       Convert.ToBoolean(reader["accLow"]), Convert.ToBoolean(reader["accMed"]), Convert.ToBoolean(reader["accHigh"]),
                                       Convert.ToBoolean(reader["glassLow"]), Convert.ToBoolean(reader["glassMed"]), Convert.ToBoolean(reader["glassHigh"]),
                                       Convert.ToBoolean(reader["topLow"]), Convert.ToBoolean(reader["topMed"]), Convert.ToBoolean(reader["topHigh"]),
                                       Convert.ToBoolean(reader["earLow"]), Convert.ToBoolean(reader["earMed"]), Convert.ToBoolean(reader["earHigh"]),
                                       Convert.ToBoolean(reader["pantsLow"]), Convert.ToBoolean(reader["pantsMed"]), Convert.ToBoolean(reader["pantsHigh"]),
                                       Convert.ToBoolean(reader["braceletLow"]), Convert.ToBoolean(reader["braceletMed"]), Convert.ToBoolean(reader["braceletHigh"]),
                                       Convert.ToBoolean(reader["shoesLow"]), Convert.ToBoolean(reader["shoesMed"]), Convert.ToBoolean(reader["shoesHigh"]),
                                       Convert.ToBoolean(reader["watchLow"]), Convert.ToBoolean(reader["watchMed"]), Convert.ToBoolean(reader["watchHigh"]),
                                       Convert.ToBoolean(reader["bagLow"]), Convert.ToBoolean(reader["bagMed"]), Convert.ToBoolean(reader["bagHigh"]),
                                       Convert.ToBoolean(reader["armorLow"]), Convert.ToBoolean(reader["armorMed"]), Convert.ToBoolean(reader["armorHigh"]),
                                       Convert.ToBoolean(reader["decalLow"]), Convert.ToBoolean(reader["decalMed"]), Convert.ToBoolean(reader["decalHigh"]));

                                    NAPI.Task.Run(() =>
                                    {
                                        if (shop.Blip != -1)
                                        {
                                            shop.ActualBlip = NAPI.Blip.CreateBlip(shop.Blip, shop.Position, 0.75f, shop.BlipColor, shop.Name, 255, 0, true, 0, shop.Dimension);
                                        }
                                        shop.CollisionShape = NAPI.ColShape.CreateCylinderColShape(shop.Position, 5f, 10f, shop.Dimension);
                                        shop.CollisionShape.SetData("ClothingShop:ID", shop.ID);
                                        ClothingShops.Add(shop);
                                    });
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
            }
        }

        public async Task<List<ClothingItem>> GetClothingShopItems(ClothingShop shop, bool gender)
        {
            List<ClothingItem> items = new List<ClothingItem>();
            //clothes + props
            if (shop.MaskLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(1, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }
            /*
             PROP-ok a sima ruhák után beszúrva
             sapka 12
            szemcsi 13
            füles 14
            óra 15
            karkötő 16

            maszk 1
            láb 4
            táska 5
            cipő 6
            kieg 7
            armor 9
            decal 10
            top 11
             */

            if (shop.MaskMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(1, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.MaskHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(1, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.PantsLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(4, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.PantsMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(4, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }
            
            if (shop.PantsHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(4, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BagLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(5, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BagMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(5, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BagHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(5, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ShoesLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(6, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ShoesMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(6, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ShoesHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(6, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.AccLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(7, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.AccMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(7, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.AccHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(7, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ArmorLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(9, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ArmorMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(9, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.ArmorHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(9, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.DecalLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(10, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.DecalMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(10, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.DecalHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(10, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.TopLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(11, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.TopMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(11, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.TopHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(11, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.HatLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(12, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.HatMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(12, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.HatHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(12, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.GlassesLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(13, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.GlassesMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(13, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.GlassesHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(13, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.EarLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(14, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.EarMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(14, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.EarHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(14, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }
            if (shop.WatchLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(15, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.WatchMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(15, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.WatchHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(15, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BraceletLow)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(16, 1, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BraceletMed)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(16, 2, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            if (shop.BraceletHigh)
            {
                List<ClothingItem> tempItems = await GetClothingItemsByComponentAndType(16, 3, gender);
                foreach (var item in tempItems)
                {
                    items.Add(item);
                }
            }

            //items -> megfelelő gender és kategóriák amiket a bolt tartalmaz
            //átküldjük kliensnek
            //kliens kezeli, megvételkor ellenőrizzük hogy az adott boltban elérhető-e az a ruha
            return items;
        }

        public async Task<List<ClothingItem>> GetClothingItemsByComponentAndType(uint component, uint category, bool gender)
        {
            List<ClothingItem> items = new List<ClothingItem>();
            
            string query = $"SELECT * FROM `clothingshop_items` WHERE `component` LIKE @Comp AND `category` LIKE @Cat AND `gender` LIKE @Gender";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Comp", component);
                        cmd.Parameters.AddWithValue("@Cat", category);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    ClothingItem item = new ClothingItem(Convert.ToUInt32(reader["id"]), Convert.ToBoolean(reader["gender"]), Convert.ToString(reader["name"]), Convert.ToUInt32(reader["component"]), Convert.ToUInt32(reader["category"]), Convert.ToString(reader["itemValue"]), Convert.ToUInt32(reader["price"]), Convert.ToString(reader["image"]));
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
            }
            return items;
        }


        [ServerEvent(Event.ResourceStart)]
        public void CreateClothingCP()
        {
            //Checkpoint cp = NAPI.Checkpoint.CreateCheckpoint(CheckpointType.Cyclinder3, new Vector3(428f, -800f, 28.5f), new Vector3(0, 1f, 0), 1f, new Color(255, 0, 0, 100), 0);
            //NAPI.Blip.CreateBlip(149, new Vector3(103.7f, -1939.2f, 50f), 0.5f, 85, "N?", 255, 0, true, 0, 0);
        }


        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColShape(ColShape colshape, Player player)
        {
            if (colshape.HasData("ClothingShop:ID"))
            {
                uint shopid = colshape.GetData<uint>("ClothingShop:ID");
                player.SendChatMessage("Beléptél egy ruhaboltba. Használ a /ruhabolt parancsot a választék megtekintéséhez!");
                player.SetData("ClothingShop:ID", shopid);
            }
            /*
            foreach (var item in ClothingShops)
            {
                if (checkpoint == item.Checkpoint)
                {
                    bool gender = player.GetData<bool>("Player:Gender");
                    List<ClothingItem> clothes = await GetClothingShopItems(item, gender);
                    NAPI.Task.Run(() =>
                    {
                        string json = NAPI.Util.ToJson(clothes);
                        player.TriggerEvent("client:LoadClothingShop", json);
                        player.SendChatMessage(json);
                    });
                    break;
                }
            }*/
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitCheckpoint(ColShape colshape, Player player)
        {
            if (colshape.HasData("ClothingShop:ID"))
            {
                
                if (player.HasData("Player:InClothingShop"))
                {
                    bool state = player.GetData<bool>("Player:InClothingShop");
                    if (state)//ha benne van a boltban akkor nem töröljük a dolgait
                    {
                        //benne van a boltban tehát nem kell töröljük, mert majd ez alapján dobjuk őt vissza a boltba
                    }
                    else
                    {
                        player.ResetData("ClothingShop:ID");
                        player.ResetData("Player:InClothingShop");
                        player.SendChatMessage("Elhagytad a ruhaboltot.");
                    }
                }
            }
        }

        [Command("ruhabolt")]
        public async void OpenClothingShop(Player player)
        {
            if (player.HasData("Player:InClothingShop"))
            {
                bool state = player.GetData<bool>("Player:InClothingShop");
                if (state)//be akarja zárni a boltot
                {
                    uint shopid = player.GetData<uint>("ClothingShop:ID");
                    foreach (var item in ClothingShops)
                    {
                        if (item.ID == shopid)
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("client:SkyCam", true);
                                player.TriggerEvent("client:CloseClothingShop");
                                NAPI.Task.Run(() =>
                                {
                                    
                                    player.SetSharedData("player:Frozen", false);
                                    player.TriggerEvent("client:DeleteCamera", true);
                                    player.SetData("Player:InClothingShop", false);


                                    player.Dimension = item.Dimension;
                                    player.Position = item.Position;

                                    player.TriggerEvent("client:SkyCam", false);
                                    player.StopAnimation();
                                }, 2000);
                            });

                            

                            Items.SetWornClothing(player);
                            break;
                        }
                    }
                }
                else if(player.HasData("ClothingShop:ID"))//nincs boltban de cp-ben van
                {
                    uint shopid = player.GetData<uint>("ClothingShop:ID");
                    foreach (var item in ClothingShops)
                    {
                        if (item.ID == shopid)
                        {
                            bool gender = player.GetData<bool>("Player:Gender");
                            List<ClothingItem> clothes = await GetClothingShopItems(item, gender);
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("client:SkyCam", true);
                                NAPI.Task.Run(() =>
                                {
                                    player.SetSharedData("player:Frozen", true);
                                    player.SetData("Player:InClothingShop", true);

                                    player.Dimension = Convert.ToUInt32(player.Id + 1);
                                    player.Position = new Vector3(-812.2f, 175f, 76.75f);
                                    player.Rotation = new Vector3(0f, 0f, 110f);

                                    player.TriggerEvent("client:SkyCam", false);
                                    player.TriggerEvent("client:EditorCamera");


                                    string json = NAPI.Util.ToJson(clothes);
                                    player.TriggerEvent("client:LoadClothingShop", json);
                                    player.PlayAnimation("nm@hands", "hands_up", 1);
                                }, 2000);
                            });
                            break;
                        }
                    }
                }
            }
            else if (player.HasData("ClothingShop:ID"))
            {
                uint shopid = player.GetData<uint>("ClothingShop:ID");
                foreach (var item in ClothingShops)
                {
                    if (item.ID == shopid)
                    {
                        bool gender = player.GetData<bool>("Player:Gender");
                        List<ClothingItem> clothes = await GetClothingShopItems(item, gender);
                        
                        NAPI.Task.Run(() =>
                        {
                            player.TriggerEvent("client:SkyCam", true);
                            NAPI.Task.Run(() =>
                            {
                                player.SetSharedData("player:Frozen", true);
                                player.SetData("Player:InClothingShop", true);

                                player.Dimension = Convert.ToUInt32(player.Id + 1);
                                player.Position = new Vector3(-812.2f, 175f, 76.75f);
                                player.Rotation = new Vector3(0f, 0f, 110f);

                                player.TriggerEvent("client:SkyCam", false);
                                player.TriggerEvent("client:EditorCamera");


                                string json = NAPI.Util.ToJson(clothes);
                                player.TriggerEvent("client:LoadClothingShop", json);
                                player.PlayAnimation("nm@hands", "hands_up", 1);
                            }, 2000);
                        });



                        break;
                    }
                }
            }

        }





        [Command("clothingshop", Alias = "clothes")]
        public async void Clothes(Player player, uint shopid)
        {
            bool gender = player.GetData<bool>("Player:Gender");
            ClothingShop shop = ClothingShops.Where((i) => i.ID == shopid).FirstOrDefault();
            
            List<ClothingItem> clothes = await GetClothingShopItems(shop, gender);
            NAPI.Task.Run(() =>
            {
                string json = NAPI.Util.ToJson(clothes);
                player.TriggerEvent("client:LoadClothingShop", json);
                player.SendChatMessage(json);
            });

            /*
            if (player.HasData("player:ClothingShop"))
            {
                
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("ruhaboltban van");
                });
                uint shopid = player.GetData<uint>("player:ClothingShop");
                if (shopid != 0)
                {
                    

                }
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("nincs ruhaboltban");
                });
            }*/
        }


        [RemoteEvent("server:CloseClothingShop")]
        public void CloseClothingShop(Player player)
        {
            player.Position = new Vector3(430f, -811.3f, 29.5f);
            player.Dimension = 0;
            player.SetSharedData("player:Frozen", false);
            player.TriggerEvent("client:DeleteCamera", true);
        }



        [RemoteEvent("server:TextureFromClient")]
        public void SetTexture(Player player, int slot, int texture)
        {
            ComponentVariation cn = new ComponentVariation();
            cn.Drawable = NAPI.Player.GetPlayerClothes(player, slot).Drawable;
            cn.Texture = texture;
            player.SetClothes(slot, cn.Drawable, cn.Texture);
        }

        [RemoteEvent("server:DrawableFromClient")]
        public void SetDrawable(Player player, int slot, int drawable)
        {
            ComponentVariation cn = NAPI.Player.GetPlayerClothes(player, slot);
            cn.Drawable = drawable;
            cn.Texture = NAPI.Player.GetPlayerClothes(player, slot).Texture;
            player.SetClothes(slot, drawable, 0);
        }
    }
}
