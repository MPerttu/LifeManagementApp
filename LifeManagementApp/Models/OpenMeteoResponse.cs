using System.Text.Json.Serialization;

namespace LifeManagementApp.Models
{
    /// <summary>
    /// Nämä luokat mallintavat Open-Meteo REST-rajapinnan palauttaman JSON-vastauksen rakenteen.
    /// System.Text.Json käyttää [JsonPropertyName]-attribuutteja mäppäämään JSON-avaimet C#-ominaisuuksiksi.
    /// </summary>
    public class OpenMeteoResponse
    {
        // JSON:in juuritason "current"-objekti
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    public class CurrentWeather
    {
        // JSON:in avain "temperature_2m"
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        // JSON:in avain "wind_speed_10m"
        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }
    }
}