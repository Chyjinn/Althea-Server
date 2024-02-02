using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Inventory;

namespace Server.Auth
{
    public class Auth
    {
            public static string GenerateSalt(int nSalt)
            {
                var saltBytes = new byte[nSalt];

                using (var provider = new RNGCryptoServiceProvider())
                {
                    provider.GetNonZeroBytes(saltBytes);
                }

                return Convert.ToBase64String(saltBytes);
            }

            public static string HashPassword(string password, string salt)//jelszó, GeneratedSalt
            {
                int nIterations = 9856;
                int nHash = 70;
                var saltBytes = Convert.FromBase64String(salt);

                using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations))
                {
                    return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
                }
            }

        public static bool verifyPassword(string password, string hashed_password, string salt)//jelszó, hash, salt - return true/false
        {
            string new_hashed = HashPassword(password, salt);
            return new_hashed.Equals(hashed_password);
        }

        public static async Task<string> GenerateNewToken(uint AccountID)//addig generál tokent amíg olyat kap ami még nem létezik
            {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                string token;
                do
                {
                    byte[] bytes = new byte[64];
                    cryptoProvider.GetBytes(bytes);

                    token = Convert.ToBase64String(bytes);
                } while (await TokenInUse(token));
                return token;
            }

            }


        public static async void DeleteTokens()
        {
            DateTime timestamp1 = DateTime.Now;
            
            int deleted = await DeleteExpiredTokens();

            DateTime timestamp2 = DateTime.Now;

            TimeSpan LoadTime = timestamp2 - timestamp1;

            NAPI.Util.ConsoleOutput(deleted + " db lejárt token törölve " + LoadTime.Milliseconds + " ms alatt.");
        }

        public async static Task<int> DeleteExpiredTokens()
        {
            int rows = 0;
            string query = $"DELETE FROM `tokens` WHERE `tokens`.`expiration` < @Now";
            //string query2 = $"UPDATE `characters` SET `characterName` = @CharacterName, `dob` = @DOB, `pob` = @POB WHERE `appearanceId` = @AppearanceID";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Now", DateTime.Now);
                    command.Prepare();
                    try
                    {
                        rows = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }
            
            return rows;
        }
        

        public static async Task<bool> SaveGeneratedToken(uint AccountID, string token, DateTime expiration)//token mentése amit GenerateNewToken-el szereztünk
        {
            string query = $"INSERT INTO `tokens` (accountId,token,expiration) VALUES (@accID,@Token,@Expiration)";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@accID", AccountID);
                        command.Parameters.AddWithValue("@Token", token);
                        command.Parameters.AddWithValue("@Expiration", expiration);
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
                return state;
            }
            
        }

        public static async Task<bool> DeleteUsedToken(string token)//felhasznált token törlése
        {
            string query = $"DELETE FROM `tokens` WHERE `tokens`.`token` LIKE @Token";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();


                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Token", token);
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
                return state;
            }
            
        }


        public static async Task<bool> VerifyToken(uint AccountID, string token)//account id alapján token ellenőrzés
        {
            string query = $"SELECT accountId,token,expiration FROM `tokens` WHERE `token` = @Token LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                uint acc = Convert.ToUInt32(reader["accountId"]);
                                string dbToken = reader["token"].ToString();
                                DateTime expiration = Convert.ToDateTime(reader["expiration"]);
                                if (expiration > DateTime.Now && acc == AccountID && dbToken == token)//lejárati dátumot ellenőrizzük, illetve hogy a megfelelő account ID és token van-e használva
                                {
                                    
                                    state = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }


        public static async Task<string[]> GetLoginData(string username)//felhasználónév alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
        {
            string query = $"SELECT * FROM `accounts` WHERE `userName` = @Username LIMIT 1";
            string[] res = new string[0];
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                try
                {
                    await con.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Prepare();
                        try
                        {
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    res = new string[8] { reader["id"].ToString(), reader["userName"].ToString(), reader["passwordHash"].ToString(), reader["passwordSalt"].ToString(), reader["serial"].ToString(), reader["scId"].ToString(), reader["sc"].ToString(), reader["characterSlots"].ToString() };
                                    
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
                return res;
            }
        }


        public static async Task<string[]> GetBanData(ulong scId, string serial)//account id alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
        {
            string query = $"SELECT playerId,adminId,reason,timestamp,expires FROM `bans` WHERE `serial` = @SERIAL OR `socialId` = @SCID AND `deactivated` = '0'";

            string[] res = new string[0];
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SERIAL", serial);
                    cmd.Parameters.AddWithValue("@SCID", scId);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = new string[5] { reader["playerId"].ToString(), reader["adminId"].ToString(), reader["reason"].ToString(), reader["timestamp"].ToString(), reader["expires"].ToString() };
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
        }

        public static async Task<string[]> GetBanData(uint accountId)//account id alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
        {
            string query = $"SELECT playerId,adminId,reason,timestamp,expires FROM `bans` WHERE `playerId` = @AccID AND `deactivated` = '0'";

            string[] res = new string[0];
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accountId);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = new string[5] { reader["playerId"].ToString(), reader["adminId"].ToString(), reader["reason"].ToString(), reader["timestamp"].ToString(), reader["expires"].ToString() };
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
        }

        public static async Task<bool> IsBanned(ulong scId, string serial)//ha a serial vagy social club bannolva van
        {
            string query = $"SELECT COUNT(playerId) FROM `bans` WHERE `serial` = @SERIAL OR `socialId` = @SCID AND `deactivated` = '0'";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();//hiba volt itt - object reference not set to an instance of an object

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SERIAL", serial);
                    cmd.Parameters.AddWithValue("@SCID", scId);
                    cmd.Prepare();
                    try
                    {
                            var count = await cmd.ExecuteScalarAsync();
                            if (Convert.ToInt32(count) > 0)
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
                return state;
            }
        }

        public static async Task<bool> IsBanned(uint accountId)//ha az account van bannolva
        {
            string query = $"SELECT COUNT(playerId) FROM `bans` WHERE `playerId` = @AccID AND `deactivated` = '0'";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accountId);
                    cmd.Prepare();
                    try
                    {
                            var count = await cmd.ExecuteScalarAsync();
                            if (Convert.ToInt32(count) > 0)
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
                return state;
            }
        }

        public static async Task<string[]> GetLoginData(uint accountID)//account id alapján adja vissza az adatokat, ha nincs ilyen akkor üres string tömböt
        {
            string query = $"SELECT * FROM `accounts` WHERE `id` = @AccID LIMIT 1";

            string[] res = new string[0];
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accountID);
                    cmd.Prepare();
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = new string[8] { reader["id"].ToString(), reader["userName"].ToString(), reader["passwordHash"].ToString(), reader["passwordSalt"].ToString(), reader["serial"].ToString(), reader["scId"].ToString(), reader["sc"].ToString(), reader["characterSlots"].ToString() };
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
            
            
        }



        public static async Task<bool> TokenInUse(string token)//ha létezik a token akkor return true
        {
            string query = $"SELECT COUNT(id) FROM `tokens` WHERE `token` = @TokenString";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TokenString", token);
                    try
                    {
                        var count = await cmd.ExecuteScalarAsync();
                        if (Convert.ToInt32(count) > 0)
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
                return state;
            }
            
        }

        public static async Task<string> GetPasswordSalt(string username)//return salt
        {
            string query = $"SELECT passwordSalt AS pwSalt FROM `accounts` WHERE `userName` = @Username LIMIT 1";
            string res = "";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = reader["pwSalt"].ToString();
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
        }

        public static async Task<string> GetPasswordHash(string username)//return hash
        {
            string query = $"SELECT passwordHash AS pwHash FROM `accounts` WHERE `userName` = @Username LIMIT 1";
            string res = "";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = reader["pwHash"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
        }

        public static async Task<bool> AccountExists(string username)//felhasználónév alapján
        {
            string query = $"SELECT userName FROM `accounts` WHERE `userName` = @Username LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if ((string)reader["userName"] == username)
                                {
                                    state = true;
                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }

        public static async Task<bool> AccountExists(uint accountID)//account id alapján
        {
            string query = $"SELECT id FROM `accounts` WHERE `id` = @AccID LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accountID);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (Convert.ToUInt16(reader["id"]) == accountID)
                                {
                                    state = true;
                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }

        public static async Task<bool> EmailInUse(string email)//használatban van-e az email
        {
            string query = $"SELECT email AS Email FROM `accounts` WHERE `email` = @Email LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if ((string)reader["Email"] == email)
                                {
                                    state = true;
                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }

        public static async Task<Tuple<int, string>> GetAdminData(uint accID)//admin szint és név
        {
            string query = $"SELECT adminLevel, adminNick FROM `accounts` WHERE `id` = @AccID LIMIT 1";
            Tuple<int, string> res = Tuple.Create(0, "Admin");
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccID", accID);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                res = Tuple.Create(Convert.ToInt32(reader["adminLevel"]), Convert.ToString(reader["adminNick"]));
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return res;
            }
        }

        public static async Task<bool> SocialClubInUse(ulong socialclubid)//social club ellenőrzés
        {
            string query = $"SELECT scId AS SocialClubId FROM `accounts` WHERE `scId` = @ScId LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ScId", socialclubid);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (Convert.ToUInt32(reader["SocialClubId"]) == socialclubid)
                                {
                                    state = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }

        public static async Task<bool> SerialInUse(string serial)//serial ellenőrzés
        {
            string query = $"SELECT serial AS Serial FROM `accounts` WHERE `serial` = @SerialNumber LIMIT 1";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SerialNumber", serial);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if ((string)reader["Serial"] == serial)
                                {

                                    state = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
                return state;
            }
        }

        public static async Task<bool> RegisterPlayer(Player player, string username, string email, string passwordHash, string salt, string serial, ulong scID, string scName)//minden adatot vár ami egy accounthoz tartozik
        {
            string query = $"INSERT INTO `accounts` (userName,email,passwordHash,passwordSalt,serial,scId,sc) VALUES (@Username,@Email,@pwHash,@Salt,@Serial,@SCID,@SCNAME)";
            bool state = false;
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();


                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@pwHash", passwordHash);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@Serial", serial);
                    command.Parameters.AddWithValue("@SCID", scID);
                    command.Parameters.AddWithValue("@SCNAME", scName);
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
                return state;
            }
        }




        //MISC

        public static void TimeoutPlayer(Player player)//timeout a bejelentkezés / regisztráció kérésekre - ne lehessen spamelni
        {
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("server:LoginTimeout", true);
            }, 0);
            
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("server:LoginTimeout", false);
            }, 5000);
        }
    }
}
