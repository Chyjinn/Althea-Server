using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server.Characters;
using Server.Vehicles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.AnimSelector
{
    internal class AnimSelector : Script
    {
        [Command("animselector")]
        public void animSelector(GTANetworkAPI.Player player)
        {
            GTANetworkAPI.Player p = player;
            player.TriggerEvent("toggleSelectorWindow");            
            
        }
        [RemoteEvent("server:playAnimation")]
        public void playAnimation(GTANetworkAPI.Player player, string animDict, string animName, int flag, bool playPause)
        {
            if (playPause)
            {
                player.PlayAnimation(animDict, animName, flag);
            }
            else
            {
                player.StopAnimation();
            }
        }

        [RemoteEvent("server:AddAnimToDB")]
        public async void AddAnimToDB(Player player, string cmd, string dict, string anim, int flag, string category)
        {
            if (await IsAnimInDatabase(dict,anim))//ha már mentettük
            {
                NAPI.Task.Run(() =>
                {
                    player.SendChatMessage("Az anim már mentve van!");

                });
                
            }
            else//nem mentettük
            {
                if (await InsertAnimToDatabase(cmd,dict,anim,flag,category))//megpróbáljuk hozzáadni
                {
                    //sikerült hozzáadni                    
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Anim mentve (" + dict + ";" + anim + ")");
                    });
                }
                else
                {
                    //nem sikerült hozzáadni
                    NAPI.Task.Run(() =>
                    {
                        player.SendChatMessage("Adatbázis hiba - nem sikerült menteni.");

                    });
                    
                }
            }
        }


        public static async Task<bool> IsAnimInDatabase(string dictionary, string anim)
        {
            bool state = false;
            string query = $"SELECT COUNT(id) FROM `anims` WHERE `dictionary` = @Dict AND `animation` = @Anim";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Dict", dictionary);
                    cmd.Parameters.AddWithValue("@Anim", anim);
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
                        //Log.Log_Server(ex.ToString());
                    }
                }
                con.CloseAsync();
            }
            return state;
        }

        public async static Task<bool> InsertAnimToDatabase(string cmd, string dict, string anim, int flag, string category)//létrehozunk egy új animot az adatbázisban
        {
            bool inserted = false;
            //INSERT INTO `appearances` (`id`, `gender`, `eyeColor`, `hairStyle`, `hairColor`, `hairHighlight`, `parent1face`, `parent2face`, `parent3face`, `parent1skin`, `parent2skin`, `parent3skin`, `faceMix`, `skinMix`, `thirdMix`, `noseWidth`, `noseHeight`, `noseLength`, `noseBridge`, `noseTip`, `noseBroken`, `browHeight`, `browWidth`, `cheekboneHeight`, `cheekboneWidth`, `cheekWidth`, `eyes`, `lips`, `jawWidth`, `jawHeight`, `chinLength`, `chinPosition`, `chinWidth`, `chinShape`, `neckWidth`, `blemishId`, `blemishOpacity`, `facialhairId`, `facialhairColor`, `facialhairOpacity`, `eyebrowId`, `eyebrowColor`, `eyebrowOpacity`, `ageId`, `ageOpacity`, `makeupId`, `makeupOpacity`, `blushId`, `blushColor`, `blushOpacity`, `complexionId`, `complexionOpacity`, `sundamageId`, `sundamageOpacity`, `lipstickId`, `lipstickColor`, `lipstickOpacity`, `frecklesId`, `frecklesOpacity`, `chesthairId`, `chesthairColor`, `chesthairOpacity`, `bodyblemishId`, `bodyblemishOpacity`, `bodyblemish2Id`, `bodyblemish2Opacity`, `tattoos`) VALUES (NULL, '0', '', '0', '', '', '', '', '', '', '', '', '', '', '', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '255', '0', '255', '0', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', '0', '255', '0', '255', '0', NULL);
            //SELECT LAST_INSERT_ID();
            //INSERT INTO `vehicles` (`id`, `ownerType`, `ownerID`, `model`, `posX`, `posY`, `posZ`, `rotX`, `rotY`, `rotZ`, `red1`, `green1`, `blue1`, `red2`, `green2`, `blue2`, `pearlescent`, `locked`, `engine`, `numberPlateText`, `numberPlateType`, `dimension`, `createdBy`, `creationDate`) VALUES (NULL, '0', '11', 'elegy', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '1', 'SZEP', '0', '0', 'Chy', CURRENT_TIMESTAMP);

            string query = $"INSERT INTO `anims` (`command`, `dictionary`, `animation`, `flag`, `category`) "+
                $" VALUES "+
                $" (@Cmd, @Dict, @Anim, @Flag, @Cat);";
            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = await Database.DBCon.GetConString();
                await con.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Cmd", cmd);
                    command.Parameters.AddWithValue("@Dict", dict);
                    command.Parameters.AddWithValue("@Anim",anim);
                    command.Parameters.AddWithValue("@Flag", flag);
                    command.Parameters.AddWithValue("@Cat", category);
                    
                    command.Prepare();
                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        if (rows > 0)
                        {
                            inserted = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Database.Log.Log_Server(ex.ToString());
                    }

                }
            }
            return inserted;
        }

    }
}
