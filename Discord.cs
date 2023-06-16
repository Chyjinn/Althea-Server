using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Discord
    {

        public static Task Main() => new Discord().MainAsync();
        
        
        //MTExOTM5MDA5MzU0MzczNTMyNg.GQfQ-G.jQr3VZdSEPTyqW8zsc6K-9-To9l9d4iZj6LM-Y
        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            string token = "MTExOTM5MDA5MzU0MzczNTMyNg.GQfQ-G.jQr3VZdSEPTyqW8zsc6K-9-To9l9d4iZj6LM-Y";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Server.Main.Log_Server(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
