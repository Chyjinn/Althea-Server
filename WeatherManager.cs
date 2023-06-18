using GTANetworkAPI;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    internal class WeatherManager: Script
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [Command("getweather")]
        public void GetWeatherCommand(Player player)
        {
            Task.Run(async () =>
            {
                try
                {
                    string apiKey = "KLKCY9S8XAYVYXLCXXYB3Y6J9";
                    string city = "Los Angeles";

                    DateTime targetTime = DateTime.UtcNow.AddHours(-9);

                    string apiUrl = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}/{targetTime:yyyy-MM-dd}?unitGroup=metric&key={apiKey}";

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        WeatherData weatherData = NAPI.Util.FromJson<WeatherData>(json);
                        WeatherHour closestHour = FindClosestHour(weatherData.Days[0].Hours, targetTime);
                        player.SendChatMessage($"Time: {closestHour.Time}");
                        player.SendChatMessage($"Degrees: {closestHour.Temp}");
                        player.SendChatMessage($"Weather Type: {closestHour.Conditions}");
                        player.SendChatMessage($"Wind Direction: {closestHour.Winddir}");
                        player.SendChatMessage($"Wind Strength: {closestHour.Windspeed}");
                    }
                    else
                    {
                        player.SendChatMessage("Failed to retrieve weather data. Status code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    player.SendChatMessage("An error occurred while retrieving weather data: " + ex.Message);
                }
            });
            }


        private WeatherHour FindClosestHour(WeatherHour[] hours, DateTime targetTime)
        {
            TimeSpan minDifference = TimeSpan.MaxValue;
            WeatherHour closestHour = null;

            foreach (WeatherHour hour in hours)
            {
                DateTime hourTime = targetTime.Date.AddHours(hour.Time / 100);
                TimeSpan difference = targetTime - hourTime;

                if (difference >= TimeSpan.Zero && difference < minDifference)
                {
                    minDifference = difference;
                    closestHour = hour;
                }
            }

            return closestHour;
        }
    }



    public class WeatherData
    {
        public WeatherDay[] Days { get; set; }
    }

    public class WeatherDay
    {
        public WeatherHour[] Hours { get; set; }
    }

    public class WeatherHour
    {
        public int Time { get; set; }
        public string Temp { get; set; }
        public string Conditions { get; set; }
        public string Winddir { get; set; }
        public string Windspeed { get; set; }
    }
}
