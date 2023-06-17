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

        public static async Task<string> GetPasswordSalt(string username)
        {
            string query = $"SELECT passwordSalt AS pwSalt FROM `accounts` WHERE `name` = @Username LIMIT 1";


            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["pwSalt"].ToString();
                    }

                }
            }
            return "";
        }

        public static async Task<string> GetPasswordHash(string username)
        {
            string query = $"SELECT passwordHash AS pwHash FROM `accounts` WHERE `name` = @Username LIMIT 1";


            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["pwHash"].ToString();
                    }

                }
            }
            return "";
        }

        public static async Task<bool> AccountExists(string username)
        {
            string query = $"SELECT name AS Username FROM `accounts` WHERE `name` = @Username LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if ((string)reader["Username"] == username)
                        {
                            return true;
                        }
                    }
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
            return false;
        }

        public static async Task<bool> SocialClubInUse(ulong socialclubid)
        {
            string query = $"SELECT scId AS SocialClubId FROM `accounts` WHERE `scId` = @ScId LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@ScId", socialclubid);

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
            return false;
        }

        public static async Task<bool> SerialInUse(string serial)
        {
            string query = $"SELECT serial AS Serial FROM `accounts` WHERE `serial` = @SerialNumber LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@SerialNumber", serial);

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
            return false;
        }

        public static async void RegisterPlayer(Player player, string username, string email, string passwordHash, string salt)
        {
            string query = $"INSERT INTO `accounts` (name,email,passwordHash,passwordSalt,serial,scId,sc) VALUES (@Username,@Email,@pwHash,@Salt,@Serial,@SCID,@SCNAME)";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Data.MySQL.con))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@pwHash", passwordHash);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@Serial", player.Serial);
                    command.Parameters.AddWithValue("@SCID", player.SocialClubId);
                    command.Parameters.AddWithValue("@SCNAME", player.SocialClubName);
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
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
        }


        public static async Task<bool> LoginPlayer(Player player, string username, string password)
        {
            string query = $"SELECT name,passwordHash,passwordSalt,token,serial,scId,sc FROM `accounts` WHERE `name` = @Username LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if ((string)reader["name"] == username)
                        {
                            if ((string)reader["serial"] == player.Serial)
                            {
                                if (Convert.ToUInt32(reader["scId"]) == player.SocialClubId)
                                {
                                    if ((string)reader["sc"] == player.SocialClubName)
                                    {
                                        string salt = (string)reader["passwordSalt"];
                                        string hash = (string)reader["passwordHash"];

                                        if (verifyPassword(password, hash, salt))
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            player.SendChatMessage("Helytelen jelszó");
                                        }
                                    }
                                    else
                                    {
                                        player.SendChatMessage("Helytelen Social Club név");
                                    }
                                }
                                else
                                {
                                    player.SendChatMessage("Helytelen Social Club ID");
                                }
                            }
                            else
                            {
                                player.SendChatMessage("Nem ehhez a géphez van társítva");
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void TimeoutPlayer(Player player)
        {
            player.SetSharedData("server:LoginTimeout", true);
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("server:LoginTimeout", false);
            }, 5000);
        }
    }
}
