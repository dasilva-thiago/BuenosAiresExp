using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuenosAiresExp.Services
{
    public static class WikimediaImageService
    {
        private static readonly HttpClient _httpClient = new();

        private const string WikiApiEn = "https://en.wikipedia.org/w/api.php";
        private const string WikiApiEs = "https://es.wikipedia.org/w/api.php";

        public static async Task<string?> SearchImageUrlAsync(double latitude, double longitude, string locationName)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BuenosAiresExp/1.0");

                //geosearch por coordenadas — mais precisa
                var imageUrl = await SearchByCoordinatesAsync(WikiApiEs, latitude, longitude, locationName)
                            ?? await SearchByCoordinatesAsync(WikiApiEn, latitude, longitude, locationName);

                if (imageUrl != null)
                    return imageUrl;

                //busca por título exato — fallback
                imageUrl = await SearchByTitleAsync(WikiApiEs, locationName)
                        ?? await SearchByTitleAsync(WikiApiEn, locationName);

                return imageUrl;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string?> SearchByCoordinatesAsync(
            string apiBase, double lat, double lng, string locationName)
        {
            try
            {
                var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var geoUrl = $"{apiBase}?action=query&list=geosearch" +
                             $"&gscoord={latStr}|{lngStr}" +
                             $"&gsradius=100&gslimit=3&format=json";

                var geoJson = await _httpClient.GetStringAsync(geoUrl);
                using var geoDoc = JsonDocument.Parse(geoJson);

                var results = geoDoc.RootElement
                    .GetProperty("query")
                    .GetProperty("geosearch");

                if (results.GetArrayLength() == 0)
                    return null;

                // Percorre os candidatos e retorna o primeiro com título relevante
                foreach (var result in results.EnumerateArray())
                {
                    var pageId = result.GetProperty("pageid").GetInt64().ToString();
                    var articleTitle = result.GetProperty("title").GetString() ?? "";

                    // Rejeita se o artigo encontrado não tiver relação com o nome do local
                    if (!IsTitleRelevant(locationName, articleTitle))
                        continue;

                    var imageUrl = await GetPageThumbnailAsync(apiBase, pageId);
                    if (imageUrl != null)
                        return imageUrl;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string?> SearchByTitleAsync(string apiBase, string locationName)
        {
            try
            {
                var searchUrl = $"{apiBase}?action=query" +
                                $"&titles={Uri.EscapeDataString(locationName)}" +
                                $"&prop=pageimages&piprop=thumbnail&pithumbsize=400" +
                                $"&format=json";

                var json = await _httpClient.GetStringAsync(searchUrl);
                using var doc = JsonDocument.Parse(json);

                var pages = doc.RootElement
                    .GetProperty("query")
                    .GetProperty("pages");

                foreach (var page in pages.EnumerateObject())
                {
                    if (page.Name == "-1") continue;

                    if (page.Value.TryGetProperty("thumbnail", out var thumb) &&
                        thumb.TryGetProperty("source", out var source))
                    {
                        return source.GetString();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string?> GetPageThumbnailAsync(string apiBase, string pageId)
        {
            try
            {
                var url = $"{apiBase}?action=query&pageids={pageId}" +
                          $"&prop=pageimages&piprop=thumbnail&pithumbsize=400" +
                          $"&format=json";

                var json = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);

                var pages = doc.RootElement
                    .GetProperty("query")
                    .GetProperty("pages");

                foreach (var page in pages.EnumerateObject())
                {
                    if (page.Value.TryGetProperty("thumbnail", out var thumb) &&
                        thumb.TryGetProperty("source", out var source))
                    {
                        return source.GetString();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Verifica se o título do artigo Wikipedia tem relação com o nome do local.
        // Tokeniza ambos e exige ao menos uma palavra significativa em comum.
        private static bool IsTitleRelevant(string locationName, string articleTitle)
        {
            var locationWords = Tokenize(locationName);
            var titleWords = Tokenize(articleTitle);

            // Palavras genéricas que não devem ser usadas como critério de relevância
            var stopWords = new HashSet<string>
            {
                "de", "del", "la", "las", "los", "el", "the", "of", "and", "e", "y",
                "san", "santa", "cafe", "bar", "hotel", "plaza", "avenida", "calle",
                "parque", "museo", "teatro", "iglesia", "centro", "club"
            };

            var significantLocation = locationWords
                .Where(w => w.Length > 3 && !stopWords.Contains(w))
                .ToHashSet();

            if (significantLocation.Count == 0)
                return false;

            // Exige que pelo menos uma palavra significativa do nome apareça no título
            return significantLocation.Any(w => titleWords.Contains(w));
        }

        // Normaliza string: minúsculas, remove acentos, separa em palavras
        private static HashSet<string> Tokenize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new HashSet<string>();

            var normalized = input
                .ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD);

            // Remove diacríticos (acentos)
            normalized = new string(normalized
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                            != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());

            return Regex.Split(normalized, @"[^a-z0-9]+")
                        .Where(w => w.Length > 0)
                        .ToHashSet();
        }

        public static async Task<System.Drawing.Bitmap?> DownloadImageAsync(string imageUrl)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(imageUrl);
                using var ms = new System.IO.MemoryStream(bytes);
                return new System.Drawing.Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }
    }
}