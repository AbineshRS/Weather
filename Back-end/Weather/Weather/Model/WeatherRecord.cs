using System.ComponentModel.DataAnnotations;

namespace Weather.Model
{
    public class WeatherRecord
    {
        [Key]
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public float TemperatureCelsius { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
