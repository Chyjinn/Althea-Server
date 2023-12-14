using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Server.Database
{
    public class Discord : Script
    {

        private static readonly Dictionary<string, int> Colors = new Dictionary<string, int>
    {
        { "RED", 13632027 },
        { "GREEN", 4289797 },
        { "BLUE", 4886754 },
        { "ORANGE", 16098851 },
        { "BLACK", 1 },
        { "WHITE", 16777215 },
        { "GREY", 10197915 },
        { "YELLOW", 16312092 },
        { "BROWN", 9131818 },
        { "CYAN", 5301186 }
    };
        
        public static void SendMessage(string webhook, string avatarurl, string imageurl, string title, string msg, List<object> fields = null, string color = "grey")
        {
            color = color.ToUpper();
            var embed = NAPI.Util.ToJson(new
            {
                embeds = new[]
                {
                new
                {
                    author = new
                    {
                        name = "Teszt1", //a posztoló neve
                        icon_url = avatarurl //szerver logó avagy bármilyen releváns logó
                    },
                    title = title,//nagy alcím, üresen is lehet hagyni
                    description = msg,//leírás, üresen is lehet hagyni
                    thumbnail = new
                    {
                        url = imageurl//ez pedig a nagyobb ábrázoló kép
                    },
                    fields = fields,
                    color = Colors[color]//kéne több szín, meg a szerver színei, stb
                }
            }
            });

            var request = WebRequest.Create(webhook);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 1000;

            var postData = Encoding.UTF8.GetBytes(embed);
            request.ContentLength = postData.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.BadRequest)
                    {
                        Console.WriteLine($"[ERROR] Unable to process request: {response.StatusCode}\nReason: {response.StatusDescription}");
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}


