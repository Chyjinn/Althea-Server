using System;
using System.Collections.Generic;
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
            /*
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
            */
        }


        [ServerEvent(Event.ChatMessage)]
        public void ChatMessage(Player player, string message)//sima szöveg
        {
            NAPI.Task.Run(() =>
            {
                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
                foreach (Player item in nearbyPlayers)
                {
                    item.SendChatMessage(player.Name + " mondja: " + message);
                    item.TriggerEvent("client:LogMessage", player.Name + " mondja: " + message);
                }
            });
        }

        //c2a2da
        [Command("me", "HASZNÁLAT: /me [cselekvés]", GreedyArg = true)]
        public static void ChatEmoteME(Player player, string message)
        {
            NAPI.Task.Run(() =>
            {
                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
                foreach (Player item in nearbyPlayers)
                {
                    item.SendChatMessage("!{#c2a2da}*** " + player.Name + " " + message);
                    item.TriggerEvent("client:LogMessage", "*** " + player.Name + " " + message);
                }
            });
        }

        [Command("do", "HASZNÁLAT: /do [történés]", GreedyArg = true)]
        public void ChatEmoteDO(Player player, string message)
        {
            NAPI.Task.Run(() =>
            {
                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
                foreach (Player item in nearbyPlayers)
                {
                    item.SendChatMessage("!{#ff2850}* " + message + " ((" + player.Name + "))");
                    item.TriggerEvent("client:LogMessage", "* " + message + " ((" + player.Name + "))");
                }
            });
        }

        [RemoteEvent("server:BeanBagHit")]
        public void HitByBeanbag(Player player)
        {
            NAPI.Task.Run(() =>
            {
                player.SendChatMessage("Eltaláltak egy babzsák sörétes puskával! Kérlek ügyelj a karakteredhez illő szerepjátékra.");
                player.TriggerEvent("client:LogMessage", "Eltaláltak egy babzsák sörétes puskával! Kérlek ügyelj a karakteredhez illő szerepjátékra.");
                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(10.0, player);
                foreach (Player item in nearbyPlayers)
                {
                    item.SendChatMessage("!{#ff2850}* A földre esik. ((" + player.Name + "))");
                    item.TriggerEvent("client:LogMessage", "* A földre esik. ((" + player.Name + "))");
                }
            });
        }

        [Command("ame", "HASZNÁLAT: /ame [leírás]", GreedyArg = true)]
        public void ChatEmoteAME(Player player, string message)
        {
            NAPI.Task.Run(() =>
            {
                var nearbyPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(5.0, player);
                foreach (Player item in nearbyPlayers)
                {
                    item.SendChatMessage("!{#8362A2}> " + player.Name + " " + message);
                    item.TriggerEvent("client:LogMessage", "> " + player.Name + " " + message);
                }
            });

        }


        [Command("placedo", "HASZNÁLAT: /placedo [leírás]", GreedyArg = true)]
        public void PlaceDo(Player player, string message)
        {
            NAPI.Task.Run(() =>
            {
                TextLabel tl = NAPI.TextLabel.CreateTextLabel(message + " ((" + player.Name + "))", player.Position, 5f, 1f, 4, new Color(255, 40, 80, 255), true, player.Dimension);
                placedos.Add(tl);
            });

        }




        [Command("pos")]
        public void PlayerPos(Player player)
        {
            player.SendChatMessage($"X:{player.Position.X}, Y:{player.Position.Y}, Z: {player.Position.Z}, rX: {player.Rotation.X}, rY: {player.Rotation.Y}, rZ: {player.Rotation.Z}");
        }

    }
}
