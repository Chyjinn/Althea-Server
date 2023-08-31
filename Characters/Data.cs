using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Characters
{
    internal class Data
    {
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
                                    Convert.ToByte(reader["faceMix"]), Convert.ToByte(reader["skinMix"]), Convert.ToByte(reader["thirdMix"]), Convert.ToInt32(reader["hairStyle"]),
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

        public static async Task<Character[]> LoadAllCharacterData(uint accID)
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

        public static async Task<Character> LoadCharacterData(uint accID, uint charID)
        {
            string query = $"SELECT id,characterName,dob,pob,appearanceId,posX,posY,posZ,rot FROM `characters` WHERE `accountId` = @accountID AND `id` = @characterID LIMIT 1";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@accountID", accID);
                    cmd.Parameters.AddWithValue("@characterID", charID);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Character c = new Character(Convert.ToUInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToDateTime(reader["dob"]), reader["pob"].ToString(), Convert.ToInt32(reader["appearanceId"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]), Convert.ToSingle(reader["rot"]));
                                return c;
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

        public async static Task<bool> CreateNewCharacter(Player player, uint accID)//létrehozunk egy új karaktert az adatbázisban, visszaadjuk az ID-jét.
        {
            Character chardata = await GetCharacterData(player);
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            string query = $"INSERT INTO `appearances` " +
                $"(`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, " +
                $"`parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, " +
                $"`noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, " +
                $"`eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, " +
                $"`blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, " +
                $"`ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, " +
                $"`sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`," +
                $" `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`)" +
                $" VALUES " +
                $"(NULL, '@Gender', '@EyeColor', '@HairStyle', '@HairColor', '@HairHightlight'," +
                $" '@P1F', '@P2F', 'P3F', '@P1S', '@P2S', '@P3S', '@FaceMix', '@SkinMix', '@ThirdMix'," +
                $" '@NoseWidth', '@NoseHeight', '@NoseLength', '@NoseBridge', '@NoseTip', '@NoseBroken', '@BrowHeight', '@BrowWidth', '@CheekboneHeight', '@CheekboneWidth', '@CheekWidth'," +
                $" '@Eyes', '@Lips', '@JawWidth', '@JawHeight', '@ChinLength', '@ChinPosition', '@ChinWidth', '@ChinShape', '@NeckWidth'," +
                $" '@BlemishId', '@BlemishOpacity', '@FacialhairId', '@FacialhairColor', '@FacialhairOpacity', '@EyebrowId', '@EyebrowColor', '@EyebrowOpacity'," +
                $" '@AgeId', '@AgeOpacity', '@MakeupId', '@MakeupOpacity', '@BlushId', '@BlushColor', '@BlushOpacity', '@ComplexionId', '@ComplexionOpacity'," +
                $" '@SundamageId', '@SundamageOpacity', '@LipstickId', '@LipstickColor', '@LipstickOpacity', '@FrecklesId', '@FrecklesOpacity', '@ChesthairId', '@ChesthairColor', '@ChesthairOpacity'," +
                $" '@Bodyblemish1Id', '@Bodyblemish1Opacity', '@Bodyblemish2Id', '@Bodyblemish2Opacity', NULL);" +
                $"SELECT LAST_INSERT_ID() AS 'lastid';";
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = Database.DBCon.GetConString();
                    await con.OpenAsync();
                    //executereader kell majd mert insert + select, kell az utolsó id

                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);
                        command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);
                    command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);
                    command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);

                    command.Prepare();
                        try
                        {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())//ha végbement a művelet
                            {
                                //itt frissíteni kell szúrni a karakter adatait, szóval chardata-t...
                                uint appearanceid = Convert.ToUInt32(reader["lastid"]);
                                string query2 = $"INSERT INTO `characters` (`id`, `accountId`, `characterName`, `creationDate`, `dob`, `pob`, `appearanceId`, `posX`, `posY`, `posZ`, `rot`) VALUES (NULL, NULL, '', CURRENT_TIMESTAMP, '', '', '', '', '', '', '');";
                                



                            }
                        }

                        }
                        catch (Exception ex)
                        {
                            Database.Log.Log_Server(ex.ToString());
                        }
                    }
                }
                return false;
            }

        

        public static async Task<bool> CreateNewCharacter(Player player)//létrehozunk egy új karaktert az adatbázisban, visszaadjuk az ID-jét.
        {
            uint accID = 0;
            Character chardata = null;
            chardata = await GetCharacterData(player);
            NAPI.Task.Run(() =>
            {
                accID = player.GetData<uint>("player:accID");
            });

            if (accID != 0 && chardata != null)//ha 0 akkor nincs még belépve a player
            {
                //todo: insert into appearances
                //appearance id alapján beszúrni a karaktert
            
            string query = $"INSERT INTO `appearances` (gender) VALUES (@accID);" +
                    $"SELECT LAST_INSERT_ID();";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();
                    //executereader kell majd mert insert + select, kell az utolsó id

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    //command.Parameters.AddWithValue("@Username", username);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
            }
                return false;
        }
            else
        {
            return false;
        }
            
        }

        public static async Task<Character> GetCharacterData(Player player)//karakter ID alapján egy karaktert ad vissza
        {
            Character character = NAPI.Util.FromJson<Character>(player.GetData<string>("player:CharacterEditor"));
            return character;
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
    }
}
