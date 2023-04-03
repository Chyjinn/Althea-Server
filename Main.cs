using System;
using System.Numerics;
using System.Threading;
using System.IO;
using GTANetworkAPI;

namespace Server
{
    public class Main : Script
    {
        static DateTime DT = DateTime.Now;
        static string ServerLogPath = $"{DT.Year}{DT.Month}{DT.Day}-{DT.Hour}{DT.Minute}{DT.Second}.log";
        
        public static void Log_Server(string message)
        {
            DateTime now = DateTime.Now;
            NAPI.Util.ConsoleOutput($"[{now.Hour}:{now.Minute}:{now.Second}] {message}");
            File.AppendAllText(ServerLogPath, $"[{now.Hour}:{now.Minute}:{now.Second}] {message}");
            return;
        }


        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            NAPI.World.SetWeather(Weather.EXTRASUNNY);
            NAPI.Server.SetGlobalServerChat(true);
            Connection.Connection.InitCon();
        }


        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Player player)
        {
            PlayerAPI.Data p = new PlayerAPI.Data(player);
            player.SetData(PlayerAPI.Data.DataIdentifier, p);



        }


        [ServerEvent(Event.PlayerSpawn)]
        public void PlayerSpawned(Player player)
        {
            

        }

    }
}