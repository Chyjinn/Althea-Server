using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GTANetworkAPI;

namespace Server.Chat
{
    class Commands : Script
    {

        List<TextLabel> placedos = new List<TextLabel>();

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            
        }


        [ServerEvent(Event.PlayerWeaponSwitch)]
        public void PlayerWeaponSwitch(Player player, WeaponHash oldWeapon, WeaponHash newWeapon)
        {
            if (oldWeapon == WeaponHash.Unarmed)
            {
                ChatEmoteME(player, "elővesz egy fegyvert.");
            }
            else if(newWeapon == WeaponHash.Unarmed)
            {
                ChatEmoteME(player, "eltesz egy fegyvert.");
            }
            else
            {
                ChatEmoteME(player, $"lecseréli "+ oldWeapon.ToString() + " fegyverét egy "+ newWeapon.ToString() + "-ra.");
            }    
            
        }


        [ServerEvent(Event.ChatMessage)]
        public void ChatMessage(Player player, string message)//sima szöveg
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
            foreach (Player item in nearbyPlayers)
            {
                item.SendChatMessage(player.Name + " mondja: " + message);
            }
        }

        //c2a2da
        [Command("me", "HASZNÁLAT: /me [cselekvés]", GreedyArg = true)]
        public void ChatEmoteME(Player player, string message)
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
            foreach(Player item in nearbyPlayers)
                {
                item.SendChatMessage("!{#c2a2da}*** " + player.Name + " " + message);
            }
        }

        [Command("do", "HASZNÁLAT: /do [történés]", GreedyArg = true)]
        public void ChatEmoteDO(Player player, string message)
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
            foreach (Player item in nearbyPlayers)
            {
                item.SendChatMessage("!{#ff2850}* " + message + " ((" + player.Name + "))");
            }
        }

        [RemoteEvent("server:BeanBagHit")]
        public void HitByBeanbag(Player player)
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(10.0, player);
            foreach (Player item in nearbyPlayers)
            {
                item.SendChatMessage("!{#ff2850}* Eltalálta egy beanbag shotgun. ((" + player.Name + "))");
            }
        }

        [Command("ame", "HASZNÁLAT: /ame [leírás]", GreedyArg = true)]
        public void ChatEmoteAME(Player player, string message)
        {
            var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
            foreach (Player item in nearbyPlayers)
            {
                item.SendChatMessage("!{#8362A2}> " + player.Name +" " + message);
            }
        }


        [Command("placedo", "HASZNÁLAT: /placedo [leírás]", GreedyArg = true)]
        public void PlaceDo(Player player, string message)
        {
            TextLabel tl = NAPI.TextLabel.CreateTextLabel(message+" (("+player.Name+"))", player.Position, 5f, 1f, 4, new Color(255, 40, 80, 255), true, player.Dimension);
            placedos.Add(tl);
        }




        [Command("pos")]
        public void PlayerPos(Player player)
        {
            player.SendChatMessage($"X:{player.Position.X}, Y:{player.Position.Y}, Z: {player.Position.Z}, rX: {player.Rotation.X}, rY: {player.Rotation.Y}, rZ: {player.Rotation.Z}");
        }

    }
}
