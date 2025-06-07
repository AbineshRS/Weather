using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class OpenWeatherMapService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
    }

    public async Task<OpenWeatherResponse?> GetCurrentWeatherAsync(string city)
    {
        // Example API call:
        // http://api.openweathermap.org/data/2.5/weather?q=Delhi&appid=APIKEY&units=metric
        string _apiKey = "964c81db15ef601093f46fa41188be99";
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);
            return response;
        }
        catch
        {
            return null;
        }
    }
}

// Model for deserializing OpenWeatherMap response (only needed fields)
public class OpenWeatherResponse
{
    public Main? Main { get; set; }
    public WeatherResponse[]? Weather { get; set; }
    public long Dt { get; set; } // unix timestamp
}

public class Main
{
    public float Temp { get; set; }
}

public class WeatherResponse
{
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
