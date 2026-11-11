using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services
{
    /// <summary>
    /// Sääpalvelun rajapinta (Interface).
    /// Määrittelee sopimuksen sään hakemiselle, jotta ViewModelit eivät riipu suoraan HttpClientistä.
    /// </summary>
    public interface IWeatherService
    {
        // Hakee säätiedot annetuilla koordinaateilla asynkronisesti
        Task<WeatherInfo?> GetCurrentWeatherAsync(double latitude, double longitude);
    }
}