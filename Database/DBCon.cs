using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server;

namespace Server.Database
{
    static class DBCon
    {
        public async static Task<string> GetConString()
        {
            string host = "localhost";
            string username = "root";
            string password = "";
            string database = "althea";
            return $"SERVER={host};PASSWORD={password};UID={username};DATABASE={database};default command timeout=20;";
        }

    }
}
