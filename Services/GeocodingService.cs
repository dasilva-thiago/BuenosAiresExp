using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace BuenosAiresExp.Services
{
    internal class GeocodingService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<(double Lat, double Lng)?> SearchCoordinatesAsync(string query)
        {
        
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BuenosAiresExp/1.0");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=1";

            
            var response = await _httpClient.GetStringAsync(url);

       
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
                return (lat, lng);
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

