using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Data;

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

            public static string HashPassword(string password, string salt)
            {
                int nIterations = 9856;
                int nHash = 70;
                var saltBytes = Convert.FromBase64String(salt);

                using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations))
                {
                    return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
                }
            }

        public static bool verifyPassword(string password, string hashed_password, string salt)
        {
            string new_hashed = HashPassword(password, salt);
            return new_hashed.Equals(hashed_password);
        }

        public static async Task<string> GenerateNewToken(uint AccountID)
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


        
        public static async Task<bool> SaveGeneratedToken(uint AccountID, string token, DateTime expiration)
        {
                string query = $"INSERT INTO `tokens` (accountId,token,expiration) VALUES (@accID,@Token,@Expiration)";
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Log_Server(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            return false;
        }

        public static async Task<bool> DeleteUsedToken(string token)
        {
            string query = $"DELETE FROM `tokens` WHERE `tokens`.`token` LIKE @Token";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Data.MySQL.con))
                {
                    command.Parameters.AddWithValue("@Token", token);
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
                        Log.Log_Server(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Log_Server(ex.ToString());
            }
            return false;
        }


        public static async Task<bool> VerifyToken(uint AccountID, string token)
        {
            string query = $"SELECT accountId,token,expiration FROM `tokens` WHERE `token` = @Token LIMIT 1";


            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                            if (expiration > DateTime.Now && acc == AccountID && dbToken == token)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return false;
        }



                


            public static async Task<bool> TokenInUse(string token)
        {
            string query = $"SELECT COUNT(id) FROM `tokens` WHERE `token` = @TokenString";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@TokenString", token);
                try
                {
                    var count = await cmd.ExecuteScalarAsync();
                    if (Convert.ToInt32(count) > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return false;
        }

        public static async Task<string> GetPasswordSalt(string username)
        {
            string query = $"SELECT passwordSalt AS pwSalt FROM `accounts` WHERE `userName` = @Username LIMIT 1";


            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                try
                { 
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader["pwSalt"].ToString();
                        }
                    }
                }
                catch(Exception ex)
                {
                Log.Log_Server(ex.ToString());
            }
        }
            return "";
        }

        public static async Task<string> GetPasswordHash(string username)
        {
            string query = $"SELECT passwordHash AS pwHash FROM `accounts` WHERE `userName` = @Username LIMIT 1";


            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                try
                { 
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader["pwHash"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return "";
        }

        public static async Task<bool> AccountExists(string username)
        {
            string query = $"SELECT userName FROM `accounts` WHERE `userName` = @Username LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return false;
        }

        public static async Task<bool> AccountExists(uint accountID)
        {
            string query = $"SELECT id FROM `accounts` WHERE `id` = @AccID LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return false;
        }

        public static async Task<bool> EmailInUse(string email)
        {
            string query = $"SELECT email AS Email FROM `accounts` WHERE `email` = @Email LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                Log.Log_Server(ex.ToString());
                }
        }
            return false;
        }

        public static async Task<bool> SocialClubInUse(ulong socialclubid)
        {
            string query = $"SELECT scId AS SocialClubId FROM `accounts` WHERE `scId` = @ScId LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                Log.Log_Server(ex.ToString());
                }
        }
            return false;
        }

        public static async Task<bool> SerialInUse(string serial)
        {
            string query = $"SELECT serial AS Serial FROM `accounts` WHERE `serial` = @SerialNumber LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
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
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            return false;
        }

        public static async Task<bool> RegisterPlayer(Player player, string username, string email, string passwordHash, string salt, string serial, ulong scID, string scName)
        {
            string query = $"INSERT INTO `accounts` (userName,email,passwordHash,passwordSalt,serial,scId,sc) VALUES (@Username,@Email,@pwHash,@Salt,@Serial,@SCID,@SCNAME)";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Data.MySQL.con))
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
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Log_Server(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Log_Server(ex.ToString());
            }
            return false;
        }

        public static async Task<string[]> GetLoginData(string username)
        {
            string query = $"SELECT id,userName,passwordHash,passwordSalt,serial,scId,sc FROM `accounts` WHERE `userName` = @Username LIMIT 1";
            string[] res;
            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Prepare();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            res = new string[7] { reader["id"].ToString(), reader["userName"].ToString(), reader["passwordHash"].ToString(), reader["passwordSalt"].ToString(),reader["serial"].ToString(), reader["scId"].ToString(), reader["sc"].ToString() };
                            return res;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            res = new string[0];
            return res;
        }

        public static async Task<string[]> GetLoginData(uint accountID)
        {
            string query = $"SELECT id,userName,passwordHash,passwordSalt,serial,scId,sc FROM `accounts` WHERE `id` = @AccID LIMIT 1";
            string[] res;
            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@AccID", accountID);
                cmd.Prepare();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            res = new string[7] { reader["id"].ToString(), reader["userName"].ToString(), reader["passwordHash"].ToString(), reader["passwordSalt"].ToString(), reader["serial"].ToString(), reader["scId"].ToString(), reader["sc"].ToString() };
                            return res;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Log_Server(ex.ToString());
                }
            }
            res = new string[0];
            return res;
        }

        public static void TimeoutPlayer(Player player)
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
