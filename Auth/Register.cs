using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Data;
using System;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal class Register : Script
    {

        [RemoteEvent("server:RegisterAttempt")]
        public async void RegisterAttempt(Player player, string username, string email, string password)
        {
            //player.Serial
              //  player.SocialClubId
                //player.SocialClubName
            if (await AccountExists(username))
            {
                player.SendChatMessage("Van ilyen felhasználónév!");
            }
            else if (await EmailInUse(email))
            {
                player.SendChatMessage("Van már ilyen email cím!");
            }
            else if(await SocialClubInUse(player.SocialClubId))
            {
                player.SendChatMessage("Ehhez a Social Club fiókhoz van már felhasználó társítva!");
            }
            else if(await SerialInUse(player.Serial))
            {
                player.SendChatMessage("Ehhez a géphez van már felhasználó társítva!");
            }
            else
            {
                string salt = Password.GenerateSalt(100);
                string pwdHashed = Password.HashPassword(password, salt);
                RegisterPlayer(player, username, email, pwdHashed, salt);
            }
            
        }

        public async Task<bool> AccountExists(string username)
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

        public async Task<bool> EmailInUse(string email)
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

        public async Task<bool> SocialClubInUse(ulong socialclubid)
        {
            string query = $"SELECT scId AS SocialClubId FROM `accounts` WHERE `scId` = @ScId LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, Data.MySQL.con))
            {
                cmd.Parameters.AddWithValue("@ScId", socialclubid);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if ((ulong)reader["SocialClubId"] == socialclubid)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> SerialInUse(string serial)
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


        public async void RegisterPlayer(Player player,string username, string email, string passwordHash, string salt)
        {
            string query = $"INSERT INTO `accounts` (name,email,passwordHash,passwordSalt,serial,scId,sc) VALUES (@Username,@Email,@pwHash,@Salt,@Serial,@SCID,@SCNAME)";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query,Data.MySQL.con))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@pwHash", passwordHash);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@Serial", player.Serial);
                    command.Parameters.AddWithValue("@SCID", player.SocialClubId);
                    command.Parameters.AddWithValue("@SCNAME", player.SocialClubName);

                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Log_Server(ex.ToString());
                    }
                    command.Parameters.AddWithValue("@username", username);
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //
            }
}
    }
}
