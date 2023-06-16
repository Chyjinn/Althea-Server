using System;
using GTANetworkAPI;

namespace Server.Login
{
    internal class Authentication : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player) {
            //NAPI.ClientEvent.TriggerClientEvent(player, "ShowLoginForm", true);
        }

        [RemoteEvent("LoginInfoFromClient")]
        public void LoginInfoFromClient(Player player, string username, string password)
        {
            if (true)
            {

            }
            else
            {

            }


        }
    }
}
