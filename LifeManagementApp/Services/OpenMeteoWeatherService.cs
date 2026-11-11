using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services
{
    /// <summary>
    /// Hakee reaaliaikaiset säätiedot Open-Meteo REST-rajapinnasta.
    /// Sisältää virheenkäsittelyn verkkokatkojen varalle, jotta käyttöliittymä ei kaadu.
    /// </summary>
    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        // HttpClient saadaan Dependency Injectionin (DI) kautta suositellulla tavalla
        public OpenMeteoWeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherInfo?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                // Muotoillaan koordinaatit piste-desimaalilla riippumatta koneen kieliasetuksesta (CultureInfo.InvariantCulture)
                string lat = latitude.ToString(CultureInfo.InvariantCulture);
                string lon = longitude.ToString(CultureInfo.InvariantCulture);

                // HUOMIO: &wind_speed_unit=ms estää sen, että km/h laukaisisi 5 m/s tuulirajan liian aikaisin!
                string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,wind_speed_10m&wind_speed_unit=ms";

                // Suoritetaan HTTP GET -pyyntö asynkronisesti
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Jos palvelin palauttaa virheen (esim. 500 tai 404), palautetaan null kaatumisen sijaan
                    return null;
                }

                // Luetaan vastaus merkkijonona
                string jsonString = await response.Content.ReadAsStringAsync();

                // Muunnetaan (deserialisoidaan) JSON-merkkijono C#-olioksi
                var weatherData = JsonSerializer.Deserialize<OpenMeteoResponse>(jsonString);

                if (weatherData?.Current != null)
                {
                    // Muunnetaan REST-vastaus sovelluksemme omaan WeatherInfo-malliin
                    return new WeatherInfo
                    {
                        Temperature = weatherData.Current.Temperature,
                        WindSpeed = weatherData.Current.WindSpeed
                    };
                }

                return null;
            }
            catch (Exception)
            {
                // Jos laitteella ei ole verkkoyhteyttä (esim. lentokonetilassa),
                // HttpRequestException napataan kiinni tässä eikä sovellus kaadu.
                return null;
            }
        }
    }
}