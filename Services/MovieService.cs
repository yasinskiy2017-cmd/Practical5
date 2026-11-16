using System.Text.Json;
using MovieCatalogApp.Models;

namespace MovieCatalogApp.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private string? _nextPageToken;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.imdbapi.dev/");
        }

        public async Task<List<Movie>> GetMoviesAsync(bool reset = false)
        {
            if (reset)
            {
                _nextPageToken = null;
            }

            try
            {
                string endpoint = "titles";
                if (!string.IsNullOrEmpty(_nextPageToken))
                {
                    endpoint += $"?pageToken={Uri.EscapeDataString(_nextPageToken)}";
                }

                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("nextPageToken", out var tokenProp))
                    {
                        _nextPageToken = tokenProp.GetString();
                    }
                    else
                    {
                        _nextPageToken = null;
                    }

                    if (root.TryGetProperty("titles", out var titlesArray))
                    {
                        var result = new List<Movie>();
                        foreach (var item in titlesArray.EnumerateArray())
                        {
                            string id = item.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
                            string title = item.TryGetProperty("primaryTitle", out var titleProp) ? titleProp.GetString() ?? "Без назви" : "Без назви";
                            string year = item.TryGetProperty("startYear", out var yearProp) && yearProp.ValueKind == JsonValueKind.Number
                                ? yearProp.GetInt32().ToString()
                                : "Невідомо";

                            string poster = "https://via.placeholder.com/150x220.png?text=No+Poster";
                            if (item.TryGetProperty("primaryImage", out var imgProp) && imgProp.ValueKind == JsonValueKind.Object)
                            {
                                if (imgProp.TryGetProperty("url", out var urlProp))
                                {
                                    poster = urlProp.GetString() ?? poster;
                                }
                            }

                            string plot = item.TryGetProperty("plot", out var plotProp) ? plotProp.GetString() ?? "Опис відсутній." : "Опис відсутній.";
                            string rating = "N/A";
                            if (item.TryGetProperty("rating", out var ratingProp) && ratingProp.ValueKind == JsonValueKind.Object)
                            {
                                if (ratingProp.TryGetProperty("aggregateRating", out var aggProp) && aggProp.ValueKind == JsonValueKind.Number)
                                {
                                    rating = $"{aggProp.GetDouble():0.0}/10";
                                }
                            }

                            result.Add(new Movie
                            {
                                Id = id,
                                Title = title,
                                Year = $"Рік випуску: {year}",
                                Poster = poster,
                                Description = plot,
                                Rating = $"Рейтинг IMDb: {rating}"
                            });
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }

            return new List<Movie>();
        }
    }
}
