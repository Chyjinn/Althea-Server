using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace Server.Database
{
    public class Log : Script
    {
        static DateTime DT = DateTime.Now;
        static string ServerLogPath = $"[SERVERLOG] {DT.Year}{DT.Month}{DT.Day}-{DT.Hour}{DT.Minute}{DT.Second}.log";
        [RemoteEvent("server:LogChat")]
        public void Log_Server_From_Player(Player p, string msg)
        {
            DateTime now = DateTime.Now;
            NAPI.Util.ConsoleOutput($"[{now.Hour}:{now.Minute}:{now.Second}] {msg}");
            File.AppendAllText(ServerLogPath, $"[{now.Hour}:{now.Minute}:{now.Second}] {msg}");
            return;
        }

        public static void Log_Server(string message)
        {
            DateTime now = DateTime.Now;
            NAPI.Task.Run(() =>
            {
                NAPI.Util.ConsoleOutput($"{now.ToString("yyyy/MM/dd HH:mm:ss")} {message}");
                File.AppendAllText(ServerLogPath, $"{now.ToString("yyyy/MM/dd HH:mm:ss")} {message}");
                return;
            });

            List<object> fields = new List<object>
                {
                    new//mindig ebben a formátumban, name az alcím, value a bővebb szöveg, inline pedig hogy jobbra legyen-e a többi cucctól
                    {
                        name = "Időpont",
                        value = now.ToString("yyyy.MM.dd. HH:mm:ss"),
                        inline = true
                    },
                    new
                    {
                        name = "Resource",
                        value = typeof(Log).Name, 
                        inline = true
                    },
                    new
                    {
                        name = "Üzenet",
                        value = message,
                        inline = false
                    },
                new
                {
                    name = "Kép",
                    value = "https://i.gyazo.com/6d931477b98988b8418d1acdbf8f973e.png",
                    inline = false
                }
                };
            Database.Discord.SendMessage("https://discord.com/api/webhooks/1184745746042470420/t0TFci5kauo7lyhYIncyT8qTGBMDcUSMiSPUqsPfIC0YlCKvL_dmgd5Q9xQNdj38uO-S", "https://i.gyazo.com/6d931477b98988b8418d1acdbf8f973e.png", "https://media.tenor.com/pPKOYQpTO8AAAAAM/monkey-developer.gif", "", "", fields, "red");


        }
    }
}
