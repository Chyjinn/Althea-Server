using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace Database
{
    class Log : Script
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
            NAPI.Util.ConsoleOutput($"[{now.Hour}:{now.Minute}:{now.Second}] {message}");
            File.AppendAllText(ServerLogPath, $"[{now.Hour}:{now.Minute}:{now.Second}] {message}");
            return;
        }
    }
}
