using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BuenosAiresExp.Services
{
    internal class GeocodingService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly SemaphoreSlim _requestLock = new SemaphoreSlim(1, 1);
        private static DateTime _nextAllowedRequestUtc = DateTime.MinValue;

        static GeocodingService()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BuenosAiresExp/1.0");
        }

        public static async Task<(double Lat, double Lng, string Address)?> SearchCoordinatesAsync(string query)
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1";

            string response;
            await _requestLock.WaitAsync();
            try
            {
                var waitTime = _nextAllowedRequestUtc - DateTime.UtcNow;
                if (waitTime > TimeSpan.Zero)
                    await Task.Delay(waitTime);

                _nextAllowedRequestUtc = DateTime.UtcNow.AddSeconds(1);
                response = await _httpClient.GetStringAsync(url);
            }
            finally
            {
                _requestLock.Release();
            }

       
            var results = System.Text.Json.JsonSerializer.Deserialize<List<NominatimResult>>(response);

            if (results == null || results.Count == 0)
                return null;

            // converte as strings de lat/lng para double
            var first = results[0];
            if (double.TryParse(first.lat, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                double.TryParse(first.lon, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double lng))
            {
                return (lat, lng, first.display_name);
            }

            return null;
        }

        internal class NominatimResult
        {
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
        }
    }
 }

