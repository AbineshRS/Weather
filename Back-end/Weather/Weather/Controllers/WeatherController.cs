using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather.Controllers;
using Weather.Model;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly Weather.Model.WeatherDbContext _dbContext;
    private readonly OpenWeatherMapService _weatherService;

    public WeatherController(Weather.Model.WeatherDbContext dbContext, OpenWeatherMapService weatherService)
    {
        _dbContext = dbContext;
        _weatherService = weatherService;
    }

    // Endpoint to fetch current weather for a location, save to DB and return
    [HttpGet("current/{location}")]
    public async Task<IActionResult> GetCurrentWeather(string location)
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(location);
        if (weather == null || weather.Main == null || weather.Weather == null || weather.Weather.Length == 0)
        {
            return BadRequest("Could not retrieve weather data.");
        }

        var weatherRecord = new WeatherRecord
        {
            Location = location,
            Date = DateTimeOffset.FromUnixTimeSeconds(weather.Dt).DateTime,
            TemperatureCelsius = weather.Main.Temp,
            WeatherDescription = weather.Weather[0].Description,
            Icon = weather.Weather[0].Icon
        };

        _dbContext.Weathers.Add(weatherRecord);
        await _dbContext.SaveChangesAsync();

        return Ok(weatherRecord);
    }
    [HttpGet]
    [Route("data")]
    public async Task<IActionResult> getdata()
    {
        var data = await _dbContext.Weathers.OrderByDescending(w => w.Id).FirstOrDefaultAsync();
        return Ok(data);
    }
    [HttpGet]
    [Route("bydate")]
    public async Task<IActionResult> GetWeatherByDateRange(
     [FromQuery] string location,
     [FromQuery] string from,
     [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location is required.");

        if (!DateTime.TryParse(from, out DateTime fromDate) ||
            !DateTime.TryParse(to, out DateTime toDate))
        {
            return BadRequest("Invalid date format.");
        }

        if (fromDate > toDate)
            return BadRequest("From date must be earlier than To date.");

        // 🔥 This line fixes your issue:
        toDate = toDate.Date.AddDays(1).AddTicks(-1);

        var weatherList = await _dbContext.Weathers
            .Where(w => w.Location == location &&
                        w.Date >= fromDate &&
                        w.Date <= toDate)
            .OrderByDescending(w => w.Date)
            .ToListAsync();

        if (weatherList == null || weatherList.Count == 0)
            return NotFound("No data found for the selected range.");

        return Ok(weatherList);
    }




    // Endpoint to get historical weather records with filters
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string location, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        

        // Validate date range max 30 days
        if (to < from || (to - from).TotalDays > 30)
            return BadRequest("Date range must be maximum 30 days.");

        var records = await _dbContext.Weathers
            .Where(w => w.Location == location && w.Date.Date >= from.Date && w.Date.Date <= to.Date)
            .OrderByDescending(w => w.Date)
            .ToListAsync();

        return Ok(records);
    }
}
