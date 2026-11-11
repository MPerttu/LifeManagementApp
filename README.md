# LifeManagementApp

.NET MAUI (C# / .NET 9) -mobiilisovellus arjen hallintaan, tehtävien seurantaan ja sääolosuhteiden tarkistamiseen. Sovellus hyödyntää MVVM-arkkitehtuuria, SQLite-paikallistietokantaa ja ulkoista REST-säärajapintaa.

---

## 📱 Toiminnallisuudet & Näkymät

* **Koti (HomePage):**
  * Tämänhetkisen säätilan haku valitulle paikkakunnalle Open-Meteo REST API:sta (lämpötila, tuulen nopeus, WMO-sääkuvaus emojein).
  * **Kitesurf-indikaattori:** Reagoi dynaamisesti käyttäjän asettamaan tuulirajaan ja ilmoittaa suotuisasta leijalautailukelistä.
  * **Päivän paras tuuli-ikkuna (LINQ):** Tuntikohtaisen ennusteen perusteella laskettu optimaalinen harrastusaika ja maksimituuli.
  * **Top 3 tärkeintä tehtävää:** Suodattaa automaattisesti tekemättömät tehtävät prioriteetin (laskeva) ja eräpäivän (nouseva) mukaan.
* **Tehtävät (TodoListPage):**
  * Kaikkien tehtävien listaus (`CollectionView`).
  * Tehtävän kuittaaminen tehdyksi lennosta.
  * Tehtävän poisto vahvistusdialogilla (`DisplayAlert`), joka ehkäisee vahinkopoistot.
  * Tehtävien lajittelu napin painalluksella (Prioriteetin tai Eräpäivän mukaan).
  * Siirtyminen uuden tehtävän lisäykseen (`AddTodoPage`) tai olemassa olevan muokkaukseen (`EditTodoPage`).
* **Asetukset (SettingsPage):**
  * Valittavissa oleva sijainti (Vaasa, Seinäjoki, Oulu, Helsinki).
  * Kitesurf-tuulirajan säätö (liukusäädin / Slider).
  * Tallennus laitteen pysyvään muistiin (`Preferences`).

---

## 🏗️ Tekninen arkkitehtuuri

* **Kehitysalusta:** .NET 9.0 (.NET MAUI)
* **Arkkitehtuurimalli:** MVVM (Model-View-ViewModel) hyödyntäen `CommunityToolkit.Mvvm`-kirjastoa (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`).
* **Tietokanta & ORM:** SQLite ja Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`).
  * 1–N-relaatio: `Category` (1) $\rightarrow$ `TodoItem` (N).
* **Ulkoinen rajapinta:** Open-Meteo REST API (`HttpClient` + `System.Text.Json`).
* **Käyttöliittymä:** XAML, MAUI Shell -navigointi, `IValueConverter` (prioriteettien dynaaminen värikoodaus).
* **Riippuvuuksien hallinta:** Sisäänrakennettu Dependency Injection (`MauiProgram.cs`).

---

## 🧪 Yksikkötestaus (xUnit)

Ratkaisu sisältää erillisen testiprojektin **`LifeManagementApp.Tests`**, joka testaa kriittistä liiketoimintalogiikkaa (AAA-malli):
1. **Kitesurf-raja-arvotestaus:** Varmistaa `WeatherInfo`-mallin toiminnan tuulen ollessa kriittisellä rajalla ($5.0$ m/s vs $> 5.0$ m/s) hyödyntäen `[Fact]`- ja `[Theory]`-annotaatioita.
2. **Top 3 -järjestys ja suodatus:** Varmistaa LINQ-logiikan toiminnan (tehdyt rajataan pois, järjestys prioriteetin ja eräpäivän mukaan, maksimissaan kolme alkiota).

---

## 🚀 Käyttöönotto & Ajo

1. Kloonaa repositorio:
   ```bash
   git clone [https://github.com/](https://github.com/)<sinun-kayttajatunnus>/LifeManagementApp.git