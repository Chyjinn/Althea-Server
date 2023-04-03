using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace Server.Chat
{
    class Commands : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            NAPI.Util.ConsoleOutput("Elindult a chat is!");
        }
        //c2a2da
        [Command("me", "HASZNÁLAT: /me cselekvés", GreedyArg = true)]
        public void emote_ME(Player player, String message)
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
            foreach(Player item in nearbyPlayers)
                {
                item.SendChatMessage("!{#c2a2da}** " + player.Name + " " + message);
            }
        }

        [Command("pos")]
        public void PlayerPos(Player player)
        {
            player.SendChatMessage($"X:{player.Position.X}, Y:{player.Position.Y}, Z: {player.Position.Z}, rX: {player.Rotation.X}, rY: {player.Rotation.Y}, rZ: {player.Rotation.Z}");
        }

    }
}
