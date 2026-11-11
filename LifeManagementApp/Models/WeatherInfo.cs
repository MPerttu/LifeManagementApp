using System;
using System.Collections.Generic;

namespace LifeManagementApp.Models
{
    /// <summary>
    /// Edustaa yksittäisen tunnin säähavaintoa/ennustetta.
    /// </summary>
    public class HourlyForecast
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
    }

    /// <summary>
    /// WeatherInfo kokoaa yhteen Open-Meteosta haetut säätiedot.
    /// Sisältää liiketoimintalogiikan kitesurf-olosuhteiden ja parhaan aikaikkunan arviointiin.
    /// </summary>
    public class WeatherInfo
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }

        /// <summary>
        /// Open-Meteo REST API:n palauttama WMO-sääkoodi (0–99).
        /// </summary>
        public int WeatherCode { get; set; }

        // B2 Bonus: Tuntikohtaiset ennusteet
        public List<HourlyForecast> HourlyForecasts { get; set; } = new();

        // B2 Bonus: Laskettu paras ajankohta leijalautailuun tänään
        public string BestKiteWindow { get; set; } = string.Empty;

        // Kitesurf-ehto kurssivaatimuksen mukaisesti: tuuli yli 5 m/s
        public bool IsKiteSurfWeather => WindSpeed > 5.0;

        public string KiteSurfMessage => IsKiteSurfWeather
            ? "🪁 Kitesurf-keli! Tuulta on yli 5 m/s."
            : "Ei riittävästi tuulta leijalautailuun.";

        /// <summary>
        /// Tulkitsee numeerisen WMO-sääkoodin käyttäjälle selkeäksi suomenkieliseksi tekstiksi ja emoji-symboliksi.
        /// </summary>
        public string WeatherDescription => WeatherCode switch
        {
            0 => "☀️ Selkeää",
            1 => "🌤️ Pääosin selkeää",
            2 => "⛅ Puolipilvistä",
            3 => "☁️ Pilvistä",
            45 or 48 => "🌫️ Sumua",
            51 or 53 or 55 => "🌦️ Tihkusadetta",
            61 or 63 or 65 => "🌧️ Vesisadetta",
            71 or 73 or 75 => "🌨️ Lumisadetta",
            77 => "❄️ Lumijyväsiä",
            80 or 81 or 82 => "🌦️ Kuurosateita",
            85 or 86 => "🌨️ Lumikuuroja",
            95 => "⛈️ Ukkosta",
            96 or 99 => "⛈️ Ukkosta ja rakeita",
            _ => "🌡️ Tuntematon sää"
        };
    }
}