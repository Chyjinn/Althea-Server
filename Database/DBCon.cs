using System;
using System.Drawing;
using System.Security.Cryptography;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Server;

namespace Database
{
    static class DBCon
    {
        public static string GetConString()
        {
            string host = "localhost";
            string username = "root";
            string password = "";
            string database = "althea";
            return $"SERVER={host};PASSWORD={password};UID={username};DATABASE={database};";
        }

    }
}
