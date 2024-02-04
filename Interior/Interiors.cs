using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Vehicles;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Interior
{
    public class Interior
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public byte Category { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public string IPL { get; set; }

        public Interior(uint id, string name, byte cat, Vector3 position, float heading, string ipl)
        {
            NAPI.Task.Run(() =>
            {
                ID = id;
                Name = name;
                Category = cat;
                Position = position;
                Heading = heading;
                IPL = ipl;
            });

        }

    }

    public class Property
    {
        public uint ID { get; set; }
        public byte PropertyType { get; set; }
        public string Name { get; set; }
        public uint OwnerType { get; set; }
        public uint OwnerID { get; set; }
        public string OwnerName { get; set; }
        public uint Postal { get; set; }
        public string StreetName { get; set; }
        public uint StreetNumber { get; set; }
        public Vector3 EntrancePos { get; set; }
        public float EntranceHeading { get; set; }
        public uint EntranceDimension { get; set; }
        public Vector3 ExitPos { get; set; }
        public float ExitHeading { get; set; }
        public uint ExitDimension { get; set; }
        public string IPL { get; set; }
        public bool Locked { get; set; }
        public int Price { get; set; }
        public Property(uint id, byte proptype, string name, uint ownertype, uint ownerid, Vector3 entrancepos, float entranceheading, uint entrancedim, Vector3 exitpos, float exitheading, uint exitdim, string ipl, bool locked, int price, uint postal, string streetname, uint streetnumber) 
        {
            NAPI.Task.Run(() =>
            {
                ID = id;
                PropertyType = proptype;
                Name = name;
                OwnerType = ownertype;
                OwnerID = ownerid;
                EntrancePos = entrancepos;
                EntranceHeading = entranceheading;
                EntranceDimension = entrancedim;
                ExitPos = exitpos;
                ExitHeading = exitheading;
                ExitDimension = exitdim;
                IPL = ipl;
                Locked = locked;
                Price = price;
                Postal = postal;
                StreetName = streetname;
                StreetNumber = streetnumber;
            });

        }
    }


    internal class Interiors : Script
    {
        static List<Property> Properties = new List<Property>();


        public async Task<Property> GetPropertyByID(uint id)
        {
            foreach (var item in Properties)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }


        [Command("getground")]
        public void GetGroundZ(Player player)
        {
            player.TriggerEvent("client:GetGroundZ");
            ColShape cs = NAPI.ColShape.CreateCylinderColShape(new Vector3(0f, 0f, 0f), 2f, 4f, 0);

        }


        [Command("createinterior", Alias = "makeinterior")]
        public async void CreateInterior(Player player, byte propType, uint interiorID, string name, int price)
        {
            //tehát: kell a groundZ pozíciója a adott pontra tehát át kell majd küldeni elsőként kliensre ezt megszerezni
            //aztán: visszajön a koordináta, illetve az utcanév esetleg postalcode is
            //megvizsgáljuk hogy az adott utcában hány darab ház van már, a házszám mindig a szám +1 lesz
            //nevet bármit adhatunk neki
            //az entrance a játékos pozíciója lesz (plusz groundZ) hogy a földre tegye a marker-t, colshape-t, stb
            //az exit pedig a megadott interiorID alapján kerül kinyerésre, ahogy az IPL is, az exitDIM az interior ID-je lesz
            Vector3 pos = player.Position;

            player.TriggerEvent("client:CreateInterior", name, pos.X, pos.Y, pos.Z, interiorID, propType, price);
        }

        public async static Task<long> AddPropertyToDatabase(byte PropertyType, string Name, string StreetName, uint StreetNumber, float EntrancePosX, float EntrancePosY, float EntrancePosZ, float EntranceHeading, uint EntranceDimension, float ExitPosX, float ExitPosY, float ExitPosZ, float ExitHeading, string IPL, int Price, string creator)//létrehozunk egy új itemet az adatbázisban
        {
            long number = 0;
            string query = $"INSERT INTO `properties` " +
                $"(`propType`, `name`, `postal`, `streetName`, `streetNumber`, `entranceX`, `entranceY`, `entranceZ`, `entranceHeading`, `entranceDimension`, `exitX`, `exitY`, `exitZ`, `exitHeading`, `IPL`, `createdBy`, `price`)" +
                $" VALUES " +
                $"(@PropType, @Name, @Postal, @StreetName, @StreetNumber, @EntranceX, @EntranceY, @EntranceZ, @EntranceHeading, @EntranceDim, @ExitX, @ExitY, @ExitZ, @ExitHeading, @IPL, @CreatedBy, @Price)";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.CommandTimeout = 10;
                    
                    command.Parameters.AddWithValue("@PropType", PropertyType);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Postal", 1);
                    command.Parameters.AddWithValue("@StreetName", StreetName);
                    command.Parameters.AddWithValue("@StreetNumber", StreetNumber);
                    command.Parameters.AddWithValue("@EntranceX", EntrancePosX);
                    command.Parameters.AddWithValue("@EntranceY", EntrancePosY);
                    command.Parameters.AddWithValue("@EntranceZ", EntrancePosZ);
                    command.Parameters.AddWithValue("@EntranceHeading", EntranceHeading);
                    command.Parameters.AddWithValue("@EntranceDim", EntranceDimension);
                    command.Parameters.AddWithValue("@ExitX", ExitPosX);
                    command.Parameters.AddWithValue("@ExitY", ExitPosY);
                    command.Parameters.AddWithValue("@ExitZ", ExitPosZ);
                    command.Parameters.AddWithValue("@ExitHeading", ExitHeading);
                    command.Parameters.AddWithValue("@IPL", IPL);
                    command.Parameters.AddWithValue("@CreatedBy", creator);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();

                        if (rows > 0)
                        {
                            number = command.LastInsertedId;
                        }

                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                await con.CloseAsync();
            }
            return number;
        }

        public async static Task UpdatePropertyLock(uint id, bool locked)
        {
            string query = $"UPDATE `properties` SET `locked` = @Locked WHERE `properties`.`id` = @ID";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Locked", locked);
                    command.Prepare();
                    try
                    {
                        command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
        }

        public async static Task UpdateEntrance(uint id, float posX, float posY, float posZ, float heading, uint dim)
        {
            string query = $"UPDATE `properties` SET `entranceX` = @PosX, `entranceY` = @PosY, `entranceZ` = @PosZ, `entranceHeading` = @Heading, `entranceDimension` = @Dim WHERE `properties`.`id` = @ID";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Dim", dim);
                    command.Parameters.AddWithValue("@PosX", posX);
                    command.Parameters.AddWithValue("@PosY", posY);
                    command.Parameters.AddWithValue("@PosZ", posZ);
                    command.Parameters.AddWithValue("@Heading", heading);
                    command.Prepare();
                    try
                    {
                        command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
        }

        public async static Task UpdateExit(uint id, float posX, float posY, float posZ, float heading, uint dim)
        {
            string query = $"UPDATE `properties` SET `exitX` = @PosX, `exitY` = @PosY, `exitZ` = @PosZ, `exitHeading` = @Heading, `exitDimension` = @Dim WHERE `properties`.`id` = @ID";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Dim", dim);
                    command.Parameters.AddWithValue("@PosX", posX);
                    command.Parameters.AddWithValue("@PosY", posY);
                    command.Parameters.AddWithValue("@PosZ", posZ);
                    command.Parameters.AddWithValue("@Heading", heading);
                    command.Prepare();
                    try
                    {
                        command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
        }

        public async static Task UpdateExitDimension(long id)
        {
            string query = $"UPDATE `properties` SET `exitDimension` = @ExitDim WHERE `properties`.`id` = @ID";

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();
                //executereader kell majd mert insert + select, kell az utolsó id

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ExitDim", id);
                    command.Parameters.AddWithValue("@ID", id);
                    command.Prepare();
                    try
                    {
                        command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
                con.CloseAsync();
            }
        }

        [Command("checkint")]
        public async void CheckInterior(Player player, uint interiorID)
        {
            player.TriggerEvent("client:ClearIPLs");
            Interior i = await GetInteriorByID(interiorID);
            if (i != null)
            {
                NAPI.Task.Run(() =>
                {
                    if (i.IPL != "")
                    {
                        player.TriggerEvent("client:RequestIPL", i.IPL);
                    }
                    player.Position = i.Position;
                    player.Heading = i.Heading;
                }, 250);
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Nem létezik interior " + interiorID + " ID-vel.");
                }, 250);
            }
        }

        [RemoteEvent("server:CreateInterior")]
        public async void ProcessInteriorCreation(Player player, uint interiorID, byte propType, string name, float posZ, string streetName, int price)
        {
            DateTime start = DateTime.Now;
            uint dim = player.Dimension;
            string playerName = player.Name;
            Vector3 pos = player.Position;
            Vector3 entrance = new Vector3(player.Position.X, player.Position.Y, posZ);
            float heading = player.Rotation.Z;
            DateTime intistart = DateTime.Now;
            Interior i = await GetInteriorByID(interiorID);
            DateTime intistop = DateTime.Now;

            if (i != null)//érvényes interior ID-t adott meg
            {
                DateTime numberstart = DateTime.Now;
                uint currentNumber = await GetLastStreetNumber(streetName);
                DateTime numberstop = DateTime.Now;
                long id = await AddPropertyToDatabase(propType, name, streetName, currentNumber + 1, entrance.X, entrance.Y, entrance.Z, heading, dim, i.Position.X, i.Position.Y, i.Position.Z, i.Heading, i.IPL, price, playerName);
                DateTime insertstop = DateTime.Now;

                if (id != 0)//ha sikerült beilleszteni
                {
                    UpdateExitDimension(id);
                    DateTime create = DateTime.Now;
                    Property p = new Property(Convert.ToUInt32(id), propType, name, 0, 0, entrance, heading, dim, i.Position, i.Heading, Convert.ToUInt32(id), i.IPL, true, price, 1, streetName, currentNumber+1);
                    p.OwnerName = "Nincs";
                    
                    Properties.Add(p);

                    DateTime created = DateTime.Now;
                    DateTime end = DateTime.Now;
                    TimeSpan time = end - start;
                    TimeSpan inti = intistop - intistart;
                    TimeSpan numbertime = numberstop - numberstart;
                    TimeSpan inserttime = insertstop - numberstop;
                    TimeSpan creation = created - create;
                    Database.Log.Log_Server("Interior létrehozás " + time.Milliseconds + " ms részletezés: Interior lekérdezés: " +inti.Milliseconds + " ms; Házszám lekérdezés: " + numbertime.Milliseconds + " ms; Adatbázis insert: " + inserttime.Milliseconds + " ms; Létrehozás: " + creation.Milliseconds + " ms");
                    foreach (var item in NAPI.Pools.GetAllPlayers())
                    {
                        //minden játékosnak újraküldjük az interiorokat
                        SendPropertiesToPlayer(item);
                    }
                    
                }



                //az interior alapján állítjuk be a property exit-et





            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Érvénytelen Interior ID!");
                });
                
            }
            //megvan a bejárat pozíciója, meg akarjuk szerezni az interior ID alapján a kijárat adatait
        }

        public async static void SendPropertiesToPlayer(Player player)
        {
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("client:ReloadProperties", NAPI.Util.ToJson(Properties));
                player.SendChatMessage(NAPI.Util.ToJson(Properties));
            });
        }

        public async static Task<string> GetOwnerName(Property p)
        {
            string query = $"SELECT `characterName` FROM `characters` WHERE `id` LIKE @ID LIMIT 1";
            string res;
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", p.OwnerID);
                        cmd.Prepare();
                        res = Convert.ToString(await cmd.ExecuteScalarAsync());
                    }
                    con.CloseAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

            }
            return "Nincs";
        }



        public async static void InitiateInteriors()
        {
            DateTime timestamp1 = DateTime.Now;

            await LoadInteriors();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput(Properties.Count + " db interior betöltve " + LoadTime.Milliseconds + " ms alatt.");
            //Database.Log.Log_Server(Properties.Count + " db interior betöltve " + LoadTime.Milliseconds + " ms alatt.");
        }

        //

        public async Task<uint> GetLastStreetNumber(string StreetName)
        {
            uint current = 0;
            string query = $"SELECT COUNT(id) AS `number` FROM `properties` WHERE `streetName` LIKE @StreetName GROUP BY `streetName` LIMIT 1";
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@StreetName", StreetName);
                        cmd.Prepare();
                        current = Convert.ToUInt32(await cmd.ExecuteScalarAsync());
                    }
                    con.CloseAsync();
                    return current;
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }
            }
            return current;
        }

        public async static Task<bool> LoadInteriors()
        {
            string query = $"SELECT * FROM `properties` WHERE `deleteDate` IS NULL";
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
                                Property p = new Property(Convert.ToUInt32(reader["id"]), Convert.ToByte(reader["propType"]), reader["name"].ToString(), Convert.ToUInt32(reader["ownerType"]), Convert.ToUInt32(reader["ownerID"]), new Vector3(Convert.ToSingle(reader["entranceX"]), Convert.ToSingle(reader["entranceY"]), Convert.ToSingle(reader["entranceZ"])), Convert.ToSingle(reader["entranceHeading"]), Convert.ToUInt32(reader["entranceDimension"]), new Vector3(Convert.ToSingle(reader["exitX"]), Convert.ToSingle(reader["exitY"]), Convert.ToSingle(reader["exitZ"])), Convert.ToSingle(reader["exitHeading"]), Convert.ToUInt32(reader["exitDimension"]), reader["ipl"].ToString(), Convert.ToBoolean(reader["locked"]), Convert.ToInt32(reader["price"]), Convert.ToUInt32(reader["postal"]), Convert.ToString(reader["streetName"]), Convert.ToUInt32(reader["streetNumber"]));
                                
                                p.OwnerName = await GetOwnerName(p);
                                Properties.Add(p);
                            }
                        }
                    }
                    con.CloseAsync();
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

                

            }
            return true;
        }
        
        
        public async Task<Interior> GetInteriorByID(uint id)
        {
            Interior i = null;
            string query = $"SELECT * FROM `interiors` WHERE `id` LIKE @ID LIMIT 1";
            using (MySqlConnection con = new MySqlConnection())
            {
                try
                {
                    con.ConnectionString = await Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Prepare();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                i = new Interior(Convert.ToUInt32(reader["id"]), Convert.ToString(reader["name"]), Convert.ToByte(reader["category"]), new Vector3(Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"])), Convert.ToSingle(reader["heading"]), Convert.ToString(reader["ipl"]));
                            }

                        }
                    }
                    con.CloseAsync();
                }
                catch (Exception ex)
                {
                    Database.Log.Log_Server(ex.ToString());
                }

            }
            return i;
        }



        [Command("marker")]
        public void CreateMarker(Player player, int type, float dirX, float dirY, float dirZ, float rotX, float rotY, float rotZ, float scale, bool move)
        {
            //ROTX = 180, scale 0.4f kültéren, beltéren 0.2f
            NAPI.Marker.CreateMarker(type, player.Position, new Vector3(dirX, dirY, dirZ), new Vector3(rotX, rotY, rotZ), scale, new Color(255, 255, 255, 255), move, player.Dimension);
        }

        [Command("markertype")]
        public void SetMarkerType(Player player, int type)
        {
            //ROTX = 180, scale 0.4f kültéren, beltéren 0.2f
            player.TriggerEvent("client:SetMarkerType", type);
        }

        [Command("gotoprop", Alias = "gotoint")]
        public async void GotoVehicle(Player player, uint property_id)
        {
            Property p = await GetPropertyByID(property_id);
            if (p != null)
            {
                NAPI.Task.Run(() =>
                {
                    player.Position = p.EntrancePos;
                    player.Dimension = p.EntranceDimension;
                    player.SendChatMessage("Ingatlan goto: " + property_id);
                });

            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Ingatlan nem létezik!");
                });
                
            }
        }

        [Command("setpropentrance", Alias = "setinteriorentrance")]
        public async void SetPropertyEntrance(Player player, uint property_id)
        {
            player.TriggerEvent("client:SetEntrance", property_id);
        }

        [RemoteEvent("server:SetPropEntrance")]
        public async void SetPropEntranceProcess(Player player, uint property_id, float groundZ)
        {
            player.SendChatMessage("szerver hívás");
            Vector3 pos = player.Position;
            float heading = player.Heading;
            uint dim = player.Dimension;
            pos.Z = groundZ;
            Property p = await GetPropertyByID(property_id);
            if (p != null)
            {
                p.EntrancePos = pos;
                p.EntranceHeading = heading;
                p.EntranceDimension = dim;
                UpdateEntrance(property_id, pos.X, pos.Y, groundZ, heading, dim);
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Ingatlan bejárata szerkesztve. [" + property_id + "]");
                    foreach (var item in NAPI.Pools.GetAllPlayers())
                    {
                        SendPropertiesToPlayer(item);
                    }
                });
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Ingatlan nem létezik!");
                });

            }
        }

        [Command("setpropexit", Alias = "setinteriorexit")]
        public async void SetPropertyExit(Player player, uint property_id)
        {
            player.TriggerEvent("client:SetExit", property_id);
        }

        [RemoteEvent("server:SetPropExit")]
        public async void SetPropExitProcess(Player player, uint property_id, float groundZ)
        {
            Vector3 pos = player.Position;
            float heading = player.Heading;
            uint dim = player.Dimension;
            pos.Z = groundZ;
            Property p = await GetPropertyByID(property_id);
            if (p != null)
            {
                p.ExitPos = pos;
                p.ExitHeading = heading;
                p.ExitDimension = dim;
                UpdateExit(property_id, pos.X, pos.Y, groundZ, heading, dim);
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Ingatlan kijárata szerkesztve. [" + property_id + "]");
                    foreach (var item in NAPI.Pools.GetAllPlayers())
                    {
                        SendPropertiesToPlayer(item);
                    }
                });
            }
            else
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Ingatlan nem létezik!");
                });

            }
        }



        [RemoteEvent("server:SetCPHeight")]
        public void SetCheckpointHeight(Player player, float height)
        {
            player.SetData("interior:CPheight", height);
        }

        [RemoteEvent("server:TogglePropertyLock")]
        public async void TogglePropertyLock(Player player, uint propid)
        {
            Property p = await GetPropertyByID(propid);
            if (p != null)//ha létezik az interior
            {
                //meg kell nézni hogy van-e kulcs/joga hozzá majd
                p.Locked = !p.Locked;
                if (p.Locked == true)
                {
                    Chat.Commands.ChatEmoteME(player, "bezárta egy ingatlan ajtaját. ((" + propid + "))");
                }
                else
                {
                    Chat.Commands.ChatEmoteME(player, "kinyitotta egy ingatlan ajtaját. ((" + propid + "))");
                }
                //adatbázisban is frissítsük le
            }
        }


        [RemoteEvent("server:EnterProperty")]
        public async void EnterProperty(Player player, uint propid)
        {
            Property p = await GetPropertyByID(propid);
            if (p != null)//ha létezik az interior
            {
                if (p.Locked == false)//elsőként ellenőrizzük hogy zárva van-e, hiszen akkor nem kell dimenziót és távolságot néznünk amúgy sem
                {
                    if (player.Dimension == p.EntranceDimension)//megfelelő dimenziókban vannak
                    {
                        if (Vector3.Distance(player.Position, p.EntrancePos) < 2f)//elég közel van a belépéshez a player
                        {
                            if (p.IPL != "")
                            {
                                player.TriggerEvent("client:RequestIPL", p.IPL);
                            }
                            
                            //betöltjük az IPL-t a játékosnak (ha van) és beléptetjük őt
                            player.Dimension = p.ExitDimension;
                            player.Position = p.ExitPos;
                            player.Heading = p.ExitHeading;
                        }
                    }
                }
                else
                {
                    player.SendChatMessage("Zárva!");
                }
            }
        }
        

        [RemoteEvent("server:ExitProperty")]
        public async void ExitProperty(Player player, uint propid)
        {
            Property p = await GetPropertyByID(propid);
            if (p != null)//ha létezik az interior
            {
                if (p.Locked == false)
                {
                    if (player.Dimension == p.ExitDimension)//megfelelő dimenziókban vannak
                    {
                        if (Vector3.Distance(player.Position, p.ExitPos) < 2f)//elég közel van a belépéshez a player
                        {
                            //betöltjük az IPL-t a játékosnak (ha van) és beléptetjük őt
                            player.Dimension = p.EntranceDimension;
                            player.Position = p.EntrancePos;
                            player.Heading = p.EntranceHeading;
                            if (p.IPL != "")
                            {
                                player.TriggerEvent("client:RemoveIPL", p.IPL);
                            }
                            
                        }
                    }
                }
                else
                {
                    player.SendChatMessage("Zárva!");
                }
            }
        }

        List<GTANetworkAPI.Object> InteriorObjects = new List<GTANetworkAPI.Object>();

        [RemoteEvent("server:PlaceObject")]
        public void PlaceObject(Player player, string objName, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            GTANetworkAPI.Object obj = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(objName), new Vector3(posX,posY,posZ), new Vector3(rotX,rotY,rotZ), 255, player.Dimension);
            InteriorObjects.Add(obj);
        }



    }


}
