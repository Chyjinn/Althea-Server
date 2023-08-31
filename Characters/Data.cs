using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using Server.Auth;
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
                                Appearance app = new Appearance(Convert.ToInt32(reader["id"]), gender, Convert.ToByte(reader["eyeColor"]), Convert.ToInt32(reader["hairStyle"]), Convert.ToByte(reader["hairColor"]), Convert.ToByte(reader["hairHighlight"]), Convert.ToByte(reader["parent1face"]), Convert.ToByte(reader["parent2face"]), Convert.ToByte(reader["parent3face"]),
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
                                Character c = new Character(Convert.ToUInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToDateTime(reader["dob"]), reader["pob"].ToString(), Convert.ToUInt32(reader["appearanceId"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]), Convert.ToSingle(reader["rot"]));
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
                                Character c = new Character(Convert.ToUInt32(reader["id"]), reader["characterName"].ToString(), Convert.ToDateTime(reader["dob"]), reader["pob"].ToString(), Convert.ToUInt32(reader["appearanceId"]), Convert.ToSingle(reader["posX"]), Convert.ToSingle(reader["posY"]), Convert.ToSingle(reader["posZ"]), Convert.ToSingle(reader["rot"]));
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

        public async static Task<bool> AddNewCharacterToDatabase(Player player, uint accID)//létrehozunk egy új karaktert az adatbázisban, visszaadjuk az ID-jét.
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
                $"(NULL, @Gender, @EyeColor, @HairStyle, @HairColor, @HairHighlight," +
                $" @P1F, @P2F, @P3F, @P1S, @P2S, @P3S, @FaceMix, @SkinMix, @ThirdMix," +
                $" @NoseWidth, @NoseHeight, @NoseLength, @NoseBridge, @NoseTip, @NoseBroken, @BrowHeight, @BrowWidth, @CheekboneHeight, @CheekboneWidth, @CheekWidth," +
                $" @Eyes, @Lips, @JawWidth, @JawHeight, @ChinLength, @ChinPosition, @ChinWidth, @ChinShape, @NeckWidth," +
                $" @BlemishId, @BlemishOpacity, @FacialhairId, @FacialhairColor, @FacialhairOpacity, @EyebrowId, @EyebrowColor, @EyebrowOpacity," +
                $" @AgeId, @AgeOpacity, @MakeupId, @MakeupOpacity, @BlushId, @BlushColor, @BlushOpacity, @ComplexionId, @ComplexionOpacity," +
                $" @SundamageId, @SundamageOpacity, @LipstickId, @LipstickColor, @LipstickOpacity, @FrecklesId, @FrecklesOpacity, @ChesthairId, @ChesthairColor, @ChesthairOpacity," +
                $" @Bodyblemish1Id, @Bodyblemish1Opacity, @Bodyblemish2Id, @Bodyblemish2Opacity, NULL);";
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = Database.DBCon.GetConString();
                    await con.OpenAsync();

                    using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        
                        command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);
                        command.Parameters.AddWithValue("@EyeColor", chardata.Appearance.EyeColor);
                        command.Parameters.AddWithValue("@HairStyle", chardata.Appearance.HairStyle);
                        command.Parameters.AddWithValue("@HairColor", chardata.Appearance.HairColor);
                        command.Parameters.AddWithValue("@HairHighlight", chardata.Appearance.HairHighlight);
                        command.Parameters.AddWithValue("@P1F", chardata.Appearance.Parent1Face);
                        command.Parameters.AddWithValue("@P2F", chardata.Appearance.Parent2Face);
                        command.Parameters.AddWithValue("@P3F", chardata.Appearance.Parent3Face);
                        command.Parameters.AddWithValue("@P1S", chardata.Appearance.Parent1Skin);
                        command.Parameters.AddWithValue("@P2S", chardata.Appearance.Parent2Skin);
                        command.Parameters.AddWithValue("@P3S", chardata.Appearance.Parent3Skin);
                        command.Parameters.AddWithValue("@FaceMix", chardata.Appearance.FaceMix);
                        command.Parameters.AddWithValue("@SkinMix", chardata.Appearance.SkinMix);
                        command.Parameters.AddWithValue("@ThirdMix", chardata.Appearance.OverrideMix);
                        command.Parameters.AddWithValue("@NoseWidth", chardata.Appearance.NoseWidth);
                        command.Parameters.AddWithValue("@NoseHeight", chardata.Appearance.NoseHeight);
                        command.Parameters.AddWithValue("@NoseLength", chardata.Appearance.NoseLength);
                        command.Parameters.AddWithValue("@NoseBridge", chardata.Appearance.NoseBridge);
                        command.Parameters.AddWithValue("@NoseTip", chardata.Appearance.NoseTip);
                        command.Parameters.AddWithValue("@NoseBroken", chardata.Appearance.NoseBroken);
                        command.Parameters.AddWithValue("@BrowHeight", chardata.Appearance.BrowHeight);
                        command.Parameters.AddWithValue("@BrowWidth", chardata.Appearance.BrowWidth);
                        command.Parameters.AddWithValue("@CheekboneHeight", chardata.Appearance.CheekboneHeight);
                        command.Parameters.AddWithValue("@CheekboneWidth", chardata.Appearance.CheekboneWidth);
                        command.Parameters.AddWithValue("@CheekWidth", chardata.Appearance.CheekWidth);
                        command.Parameters.AddWithValue("@Eyes", chardata.Appearance.Eyes);
                        command.Parameters.AddWithValue("@Lips", chardata.Appearance.Lips);
                        command.Parameters.AddWithValue("@JawWidth", chardata.Appearance.JawWidth);
                        command.Parameters.AddWithValue("@JawHeight", chardata.Appearance.JawHeight);
                        command.Parameters.AddWithValue("@ChinLength", chardata.Appearance.ChinLength);
                        command.Parameters.AddWithValue("@ChinPosition", chardata.Appearance.ChinPosition);
                        command.Parameters.AddWithValue("@ChinWidth", chardata.Appearance.ChinWidth);
                        command.Parameters.AddWithValue("@ChinShape", chardata.Appearance.ChinShape);
                        command.Parameters.AddWithValue("@NeckWidth", chardata.Appearance.NeckWidth);
                        //OVERLAYS
                        command.Parameters.AddWithValue("@BlemishId", chardata.Appearance.BlemishId);
                        command.Parameters.AddWithValue("@BlemishOpacity", chardata.Appearance.BlemishOpacity);
                        command.Parameters.AddWithValue("@FacialhairId", chardata.Appearance.FacialHairId);
                        command.Parameters.AddWithValue("@FacialhairColor", chardata.Appearance.FacialHairColor);
                        command.Parameters.AddWithValue("@FacialhairOpacity", chardata.Appearance.FacialHairOpacity);
                        command.Parameters.AddWithValue("@EyebrowId", chardata.Appearance.EyeBrowId);
                        command.Parameters.AddWithValue("@EyebrowColor", chardata.Appearance.EyeBrowColor);
                        command.Parameters.AddWithValue("@EyebrowOpacity", chardata.Appearance.EyeBrowOpacity);
                        command.Parameters.AddWithValue("@AgeId", chardata.Appearance.AgeId);
                        command.Parameters.AddWithValue("@AgeOpacity", chardata.Appearance.AgeOpacity);
                        command.Parameters.AddWithValue("@MakeupId", chardata.Appearance.MakeupId);
                        command.Parameters.AddWithValue("@MakeupOpacity", chardata.Appearance.MakeupOpacity);
                        command.Parameters.AddWithValue("@BlushId", chardata.Appearance.BlushId);
                        command.Parameters.AddWithValue("@BlushColor", chardata.Appearance.BlushColor);
                        command.Parameters.AddWithValue("@BlushOpacity", chardata.Appearance.BlushOpacity);
                        command.Parameters.AddWithValue("@ComplexionId", chardata.Appearance.ComplexionId);
                        command.Parameters.AddWithValue("@ComplexionOpacity", chardata.Appearance.ComplexionOpacity);
                        command.Parameters.AddWithValue("@SundamageId", chardata.Appearance.SundamageId);
                        command.Parameters.AddWithValue("@SundamageOpacity", chardata.Appearance.SundamageOpacity);
                        command.Parameters.AddWithValue("@LipstickId", chardata.Appearance.LipstickId);
                        command.Parameters.AddWithValue("@LipstickColor", chardata.Appearance.LipstickColor);
                        command.Parameters.AddWithValue("@LipstickOpacity", chardata.Appearance.LipstickOpacity);
                        command.Parameters.AddWithValue("@FrecklesId", chardata.Appearance.FrecklesId);
                        command.Parameters.AddWithValue("@FrecklesOpacity", chardata.Appearance.FrecklesOpacity);
                        command.Parameters.AddWithValue("@ChesthairId", chardata.Appearance.ChestHairId);
                        command.Parameters.AddWithValue("@ChesthairColor", chardata.Appearance.ChestHairColor);
                        command.Parameters.AddWithValue("@ChesthairOpacity", chardata.Appearance.ChestHairOpacity);
                        command.Parameters.AddWithValue("@Bodyblemish1Id", chardata.Appearance.BodyBlemish1Id);
                        command.Parameters.AddWithValue("@Bodyblemish1Opacity", chardata.Appearance.BodyBlemish1Opacity);
                        command.Parameters.AddWithValue("@Bodyblemish2Id", chardata.Appearance.BodyBlemish2Id);
                        command.Parameters.AddWithValue("@Bodyblemish2Opacity", chardata.Appearance.BodyBlemish2Opacity);

                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            long appearanceid = command.LastInsertedId;
                            string query2 = $"INSERT INTO `characters` (`id`, `accountId`, `characterName`, `creationDate`, `dob`, `pob`, `appearanceId`, `posX`, `posY`, `posZ`, `rot`) VALUES" +
                                $" (NULL, @AccountID, @CharacterName, CURRENT_TIMESTAMP, @DOB, @POB, @AppearanceID, '-1037', '-2738', '21', '-30');";

                            using (MySqlConnection con2 = new MySqlConnection())
                            {
                                con2.ConnectionString = Database.DBCon.GetConString();
                                await con2.OpenAsync();
                                //executereader kell majd mert insert + select, kell az utolsó id

                                using (MySqlCommand command2 = new MySqlCommand(query2, con2))
                                {
                                    command2.Parameters.AddWithValue("@AccountID", accID);
                                    command2.Parameters.AddWithValue("@CharacterName", chardata.Name);
                                    command2.Parameters.AddWithValue("@DOB", chardata.DOB);
                                    command2.Parameters.AddWithValue("@POB", chardata.POB);
                                    command2.Parameters.AddWithValue("@AppearanceID", appearanceid);

                                    try
                                    {
                                        int rows2 = await command2.ExecuteNonQueryAsync();
                                        if (rows2 > 0)
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

        public async static Task<bool> EditExistingCharacterInDatabase(Player player, uint appearanceID)//létrehozunk egy új karaktert az adatbázisban, visszaadjuk az ID-jét.
        {
            Character chardata = await GetCharacterData(player);
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            string query = $"UPDATE `appearances` SET " +
                $"`gender` = @Gender, `eyeColor` = @EyeColor, `hairStyle` = @HairStyle, `hairColor` = @HairColor, `hairHighlight` = @HairHighlight, " +
                $"`parent1face` = @P1F, `parent2face` = @P2F, `parent3face` = @P3F, `parent1skin` = @P1S, `parent2skin` = @P2S, `parent3skin` = @P3S, `faceMix` = @FaceMix, `skinMix` = @SkinMix, `thirdMix` = @ThirdMix, " +
                $"`noseWidth` = @NoseWidth, `noseHeight` = @NoseHeight, `noseLength` = @NoseLength, `noseBridge` = @NoseBridge, `noseTip` = @NoseTip, `noseBroken` = @NoseBroken, " +
                $"`browHeight` = @BrowHeight, `browWidth` = @BrowWidth, `cheekboneHeight` = @CheekboneHeight, `cheekboneWidth` = @CheekboneWidth, `cheekWidth` = @CheekWidth, " +
                $"`eyes` = @Eyes, `lips` = @Lips, `jawWidth` = @JawWidth, `jawHeight` = @JawHeight, `chinLength` = @ChinLength, `chinPosition` = @ChinPosition, `chinWidth` = @ChinWidth, `chinShape` = @ChinShape, `neckWidth` = @NeckWidth, " +
                $"`blemishId` = @BlemishId, `blemishOpacity` = @BlemishOpacity, `facialhairId` = @FacialhairId, `facialhairColor` = @FacialhairColor, `facialhairOpacity` = @FacialhairOpacity, `eyebrowId` = @EyebrowId, `eyebrowColor` = @EyebrowColor, `eyebrowOpacity` = @EyebrowOpacity, " +
                $"`ageId` = @AgeId, `ageOpacity` = @AgeOpacity, `makeupId` = @MakeupId, `makeupOpacity` = @MakeupOpacity, `blushId` = @BlushId, `blushColor` = @BlushColor, `blushOpacity` = @BlushOpacity, `complexionId` = @ComplexionId, `complexionOpacity` = @ComplexionOpacity, " +
                $"`sundamageId` = @SundamageId, `sundamageOpacity` = @SundamageOpacity, `lipstickId` = @LipstickId, `lipstickColor` = @LipstickColor, `lipstickOpacity` = @LipstickOpacity, `frecklesId` = @FrecklesId, `frecklesOpacity` = @FrecklesOpacity, `chesthairId` = @ChesthairId, `chesthairColor` = @ChesthairColor, `chesthairOpacity` = @ChesthairOpacity," +
                $" `bodyblemishId` = @Bodyblemish1Id, `bodyblemishOpacity`= @Bodyblemish1Opacity, `bodyblemish2Id`= @Bodyblemish2Id, `bodyblemish2Opacity`= @Bodyblemish2Opacity WHERE `appearances`.`id` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {

                    command.Parameters.AddWithValue("@Gender", chardata.Appearance.Gender);
                    command.Parameters.AddWithValue("@EyeColor", chardata.Appearance.EyeColor);
                    command.Parameters.AddWithValue("@HairStyle", chardata.Appearance.HairStyle);
                    command.Parameters.AddWithValue("@HairColor", chardata.Appearance.HairColor);
                    command.Parameters.AddWithValue("@HairHighlight", chardata.Appearance.HairHighlight);
                    command.Parameters.AddWithValue("@P1F", chardata.Appearance.Parent1Face);
                    command.Parameters.AddWithValue("@P2F", chardata.Appearance.Parent2Face);
                    command.Parameters.AddWithValue("@P3F", chardata.Appearance.Parent3Face);
                    command.Parameters.AddWithValue("@P1S", chardata.Appearance.Parent1Skin);
                    command.Parameters.AddWithValue("@P2S", chardata.Appearance.Parent2Skin);
                    command.Parameters.AddWithValue("@P3S", chardata.Appearance.Parent3Skin);
                    command.Parameters.AddWithValue("@FaceMix", chardata.Appearance.FaceMix);
                    command.Parameters.AddWithValue("@SkinMix", chardata.Appearance.SkinMix);
                    command.Parameters.AddWithValue("@ThirdMix", chardata.Appearance.OverrideMix);
                    command.Parameters.AddWithValue("@NoseWidth", chardata.Appearance.NoseWidth);
                    command.Parameters.AddWithValue("@NoseHeight", chardata.Appearance.NoseHeight);
                    command.Parameters.AddWithValue("@NoseLength", chardata.Appearance.NoseLength);
                    command.Parameters.AddWithValue("@NoseBridge", chardata.Appearance.NoseBridge);
                    command.Parameters.AddWithValue("@NoseTip", chardata.Appearance.NoseTip);
                    command.Parameters.AddWithValue("@NoseBroken", chardata.Appearance.NoseBroken);
                    command.Parameters.AddWithValue("@BrowHeight", chardata.Appearance.BrowHeight);
                    command.Parameters.AddWithValue("@BrowWidth", chardata.Appearance.BrowWidth);
                    command.Parameters.AddWithValue("@CheekboneHeight", chardata.Appearance.CheekboneHeight);
                    command.Parameters.AddWithValue("@CheekboneWidth", chardata.Appearance.CheekboneWidth);
                    command.Parameters.AddWithValue("@CheekWidth", chardata.Appearance.CheekWidth);
                    command.Parameters.AddWithValue("@Eyes", chardata.Appearance.Eyes);
                    command.Parameters.AddWithValue("@Lips", chardata.Appearance.Lips);
                    command.Parameters.AddWithValue("@JawWidth", chardata.Appearance.JawWidth);
                    command.Parameters.AddWithValue("@JawHeight", chardata.Appearance.JawHeight);
                    command.Parameters.AddWithValue("@ChinLength", chardata.Appearance.ChinLength);
                    command.Parameters.AddWithValue("@ChinPosition", chardata.Appearance.ChinPosition);
                    command.Parameters.AddWithValue("@ChinWidth", chardata.Appearance.ChinWidth);
                    command.Parameters.AddWithValue("@ChinShape", chardata.Appearance.ChinShape);
                    command.Parameters.AddWithValue("@NeckWidth", chardata.Appearance.NeckWidth);
                    //OVERLAYS
                    command.Parameters.AddWithValue("@BlemishId", chardata.Appearance.BlemishId);
                    command.Parameters.AddWithValue("@BlemishOpacity", chardata.Appearance.BlemishOpacity);
                    command.Parameters.AddWithValue("@FacialhairId", chardata.Appearance.FacialHairId);
                    command.Parameters.AddWithValue("@FacialhairColor", chardata.Appearance.FacialHairColor);
                    command.Parameters.AddWithValue("@FacialhairOpacity", chardata.Appearance.FacialHairOpacity);
                    command.Parameters.AddWithValue("@EyebrowId", chardata.Appearance.EyeBrowId);
                    command.Parameters.AddWithValue("@EyebrowColor", chardata.Appearance.EyeBrowColor);
                    command.Parameters.AddWithValue("@EyebrowOpacity", chardata.Appearance.EyeBrowOpacity);
                    command.Parameters.AddWithValue("@AgeId", chardata.Appearance.AgeId);
                    command.Parameters.AddWithValue("@AgeOpacity", chardata.Appearance.AgeOpacity);
                    command.Parameters.AddWithValue("@MakeupId", chardata.Appearance.MakeupId);
                    command.Parameters.AddWithValue("@MakeupOpacity", chardata.Appearance.MakeupOpacity);
                    command.Parameters.AddWithValue("@BlushId", chardata.Appearance.BlushId);
                    command.Parameters.AddWithValue("@BlushColor", chardata.Appearance.BlushColor);
                    command.Parameters.AddWithValue("@BlushOpacity", chardata.Appearance.BlushOpacity);
                    command.Parameters.AddWithValue("@ComplexionId", chardata.Appearance.ComplexionId);
                    command.Parameters.AddWithValue("@ComplexionOpacity", chardata.Appearance.ComplexionOpacity);
                    command.Parameters.AddWithValue("@SundamageId", chardata.Appearance.SundamageId);
                    command.Parameters.AddWithValue("@SundamageOpacity", chardata.Appearance.SundamageOpacity);
                    command.Parameters.AddWithValue("@LipstickId", chardata.Appearance.LipstickId);
                    command.Parameters.AddWithValue("@LipstickColor", chardata.Appearance.LipstickColor);
                    command.Parameters.AddWithValue("@LipstickOpacity", chardata.Appearance.LipstickOpacity);
                    command.Parameters.AddWithValue("@FrecklesId", chardata.Appearance.FrecklesId);
                    command.Parameters.AddWithValue("@FrecklesOpacity", chardata.Appearance.FrecklesOpacity);
                    command.Parameters.AddWithValue("@ChesthairId", chardata.Appearance.ChestHairId);
                    command.Parameters.AddWithValue("@ChesthairColor", chardata.Appearance.ChestHairColor);
                    command.Parameters.AddWithValue("@ChesthairOpacity", chardata.Appearance.ChestHairOpacity);
                    command.Parameters.AddWithValue("@Bodyblemish1Id", chardata.Appearance.BodyBlemish1Id);
                    command.Parameters.AddWithValue("@Bodyblemish1Opacity", chardata.Appearance.BodyBlemish1Opacity);
                    command.Parameters.AddWithValue("@Bodyblemish2Id", chardata.Appearance.BodyBlemish2Id);
                    command.Parameters.AddWithValue("@Bodyblemish2Opacity", chardata.Appearance.BodyBlemish2Opacity);
                    command.Parameters.AddWithValue("@AppearanceID", appearanceID);
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
