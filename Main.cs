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
        //PLAYER -811.6, 175.1, 76.7, 110
        //CAM -814 174.1 76.7, -70

        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.World.SetWeather(Weather.EXTRASUNNY);
            NAPI.Server.SetGlobalServerChat(true);
            Data.MySQL.InitConnection();
        }


        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Player player)
        {
            
        }


        [ServerEvent(Event.PlayerSpawn)]
        public void PlayerSpawned(Player player)
        {
            //player.TriggerEvent("CreateAuthForms");
            //player.TriggerEvent("SetLoginCamera", -426.0f, 1117.0f, 350.0f, 0.0f, 0.0f, -163.0f, 70.0f);
        }

        [RemoteEvent("server:Fly")]
        public void Fly(Player player, int x, int y, int z)
        {
            GTANetworkAPI.Vector3 v = new GTANetworkAPI.Vector3(x, y, z);
            player.Position = v;
         
        }
    }
}