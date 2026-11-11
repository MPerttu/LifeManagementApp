using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;
using Microsoft.Maui.Storage;

namespace LifeManagementApp.ViewModels
{
    /// <summary>
    /// HomeViewModel hallitsee päänäkymän (HomePage) tilaa ja logiikkaa.
    /// Perii ObservableObject-luokan, joka tarjoaa INotifyPropertyChanged-toiminnallisuuden.
    /// Huom: Luokan on oltava partial, jotta CommunityToolkitin generaattori voi luoda koodia sen taustalle.
    /// </summary>
    public partial class HomeViewModel : ObservableObject
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IWeatherService _weatherService;

        // ObservableCollection ilmoittaa XAML:n CollectionViewlle automaattisesti, kun rivejä tulee lisää tai poistuu.
        public ObservableCollection<TodoItem> Top3Todos { get; } = new();

        // [ObservableProperty] generoi automaattisesti taustalle julkisen IsLoading-ominaisuuden.
        [ObservableProperty]
        private bool _isLoading;

        // Lämpötilan näyttöteksti käyttöliittymälle
        [ObservableProperty]
        private string _temperatureText = "-- °C";

        // Tuulen nopeuden näyttöteksti
        [ObservableProperty]
        private string _windSpeedText = "-- m/s";

        // Kitesurf-huomio (tuulen ylittäessä asetetun rajan näytetään ilmoitus)
        [ObservableProperty]
        private string _kiteSurfStatus = string.Empty;

        // Kertoo XAMLille, näytetäänkö Kitesurf-laatikko ollenkaan ruudulla
        [ObservableProperty]
        private bool _isKiteSurfVisible;

        // Sijainnin nimi käyttöliittymän otsikkoa varten
        [ObservableProperty]
        private string _locationTitle = "Vaasa";

        // B2 Bonus: Paras kite-ikkuna tänään (kellonaikaväli ja maksimituuli)
        [ObservableProperty]
        private string _bestKiteWindowText = string.Empty;

        // Sääsymboli ja kuvaus WMO-koodin perusteella käyttöliittymälle (esim. Puolipilvistä)
        [ObservableProperty]
        private string _weatherDescriptionText = string.Empty;

        /// <summary>
        /// Konstruktori-injektio: DI tuo tarvittavat palvelut rajapintojen kautta.
        /// </summary>
        public HomeViewModel(ITodoRepository todoRepository, IWeatherService weatherService)
        {
            _todoRepository = todoRepository;
            _weatherService = weatherService;
        }

        /// <summary>
        /// RelayCommand generoi LoadDataCommand-komennon XAML-sidontoja varten.
        /// </summary>
        [RelayCommand]
        public async Task LoadDataAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;

                // 1. Luetaan valittu sijainti ja tuuliraja pysyvästä Preferences-muistista (Bonus B2)
                string cityName = Preferences.Default.Get(SettingsViewModel.KeyCity, "Vaasa");
                double lat = Preferences.Default.Get(SettingsViewModel.KeyLat, 63.10);
                double lon = Preferences.Default.Get(SettingsViewModel.KeyLon, 21.62);
                double threshold = Preferences.Default.Get(SettingsViewModel.KeyWindThreshold, 5.0);

                LocationTitle = cityName;

                // 2. Haetaan säätiedot asynkronisesti valitun kaupungin koordinaateilla
                var weather = await _weatherService.GetCurrentWeatherAsync(lat, lon);

                if (weather != null)
                {
                    TemperatureText = $"{weather.Temperature:F1} °C";
                    WindSpeedText = $"{weather.WindSpeed:F1} m/s";
                    WeatherDescriptionText = weather.WeatherDescription;

                    // Verrataan tuulta dynaamiseen käyttäjän asettamaan rajaan
                    bool isKite = weather.WindSpeed >= threshold;
                    IsKiteSurfVisible = isKite;

                    if (isKite)
                    {
                        KiteSurfStatus = $"🪁 Kitesurf-keli kohteessa {cityName}! Tuulta yli {threshold:F1} m/s.";
                    }
                    else
                    {
                        KiteSurfStatus = $"Ei riittävästi tuulta kohteessa {cityName} (raja {threshold:F1} m/s).";
                    }

                    BestKiteWindowText = weather.BestKiteWindow;
                }
                else
                {
                    TemperatureText = "Ei saatavilla";
                    WindSpeedText = "Ei saatavilla";
                    WeatherDescriptionText = "Sääkuvaus ei saatavilla";
                    KiteSurfStatus = "Säätietoja ei voitu hakea (offline).";
                    IsKiteSurfVisible = false;
                    BestKiteWindowText = string.Empty;
                }

                // 3. Haetaan tietokannasta Top 3 tärkeintä keskeneräistä tehtävää
                var topTodos = await _todoRepository.GetTop3TodosAsync();

                Top3Todos.Clear();
                foreach (var item in topTodos)
                {
                    Top3Todos.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}