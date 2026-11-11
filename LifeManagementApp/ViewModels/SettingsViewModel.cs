using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace LifeManagementApp.ViewModels
{
    /// <summary>
    /// Edustaa esivalittua kaupunkia asetussivulla.
    /// </summary>
    public class CityLocation
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    /// <summary>
    /// SettingsViewModel vastaa käyttäjän valitsemien asetusten tallentamisesta ja lataamisesta.
    /// Käyttää .NET MAUI:n Preferences-rajapintaa pysyvään avain-arvo-tallennukseen.
    /// Täyttää Bonus B2 vaatimuksen: "asetussivu, jossa sijainnin ja tuulirajan voi vaihtaa (Preferences)".
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        // Preferences-avaimet vakioina (estää kirjoitusvirheet)
        public const string KeyCity = "pref_city_name";
        public const string KeyLat = "pref_latitude";
        public const string KeyLon = "pref_longitude";
        public const string KeyWindThreshold = "pref_wind_threshold";

        public ObservableCollection<CityLocation> AvailableCities { get; } = new()
        {
            new CityLocation { Name = "Vaasa", Latitude = 63.10, Longitude = 21.62 },
            new CityLocation { Name = "Kauhava", Latitude = 63.10, Longitude = 23.06 },
            new CityLocation { Name = "Seinäjoki", Latitude = 62.79, Longitude = 22.84 }
        };

        [ObservableProperty]
        private CityLocation _selectedCity;

        [ObservableProperty]
        private double _windThreshold = 5.0;

        // Vahvistusviestin teksti
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        // Ohjaa tekstin näkyvyyttä XAMLissa (oletuksena false, eli piilossa)
        [ObservableProperty]
        private bool _isStatusMessageVisible;

        public SettingsViewModel()
        {
            // Ladataan tallennetut arvot tai käytetään oletuksia
            string savedCity = Preferences.Default.Get(KeyCity, "Vaasa");
            WindThreshold = Preferences.Default.Get(KeyWindThreshold, 5.0);

            // Kohdistetaan pudotusvalikon valinta
            var found = AvailableCities.FirstOrDefault(c => c.Name == savedCity);
            _selectedCity = found ?? AvailableCities[0];
        }

        /// <summary>
        /// Tallentaa käyttäjän tekemät muutokset pysyvästi laitteen Preferences-muistiin.
        /// Näyttää vahvistusviestin 3 sekunnin ajan ja piilottaa sen automaattisesti.
        /// </summary>
        [RelayCommand]
        public async Task SaveSettingsAsync()
        {
            if (SelectedCity != null)
            {
                Preferences.Default.Set(KeyCity, SelectedCity.Name);
                Preferences.Default.Set(KeyLat, SelectedCity.Latitude);
                Preferences.Default.Set(KeyLon, SelectedCity.Longitude);
            }

            Preferences.Default.Set(KeyWindThreshold, WindThreshold);

            // Näytetään viesti
            StatusMessage = "Asetukset tallennettu onnistuneesti!";
            IsStatusMessageVisible = true;

            // Odotetaan 3 sekuntia käyttöliittymää jäädyttämättä
            await Task.Delay(3000);

            // Piilotetaan viesti
            IsStatusMessageVisible = false;
            StatusMessage = string.Empty;
        }
    }
}