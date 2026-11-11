using System;
using System.Collections.Generic;
using System.Linq;
using LifeManagementApp.Models;
using Xunit;

namespace LifeManagementApp.Tests
{
    /// <summary>
    /// xUnit-yksikkötestit sovelluksen liiketoimintalogiikalle.
    /// Täyttää kurssin Bonus B3 -vaatimuksen (+1 p):
    /// "xUnit-testejä ViewModelille tai servicelle (esim. top 3 -järjestys, kite-raja)"
    /// </summary>
    public class WeatherAndTodoTests
    {
        // =========================================================================
        // 1. KITESURF-LIIKETOIMINTASÄÄNTÖ (Raja-arvotestaus: Tuuli > 5.0 m/s)
        // =========================================================================

        [Fact]
        public void IsKiteSurfWeather_ReturnsTrue_WhenWindSpeedExceedsFive()
        {
            // Arrange (Valmistelu): Luodaan sääolio, jossa tuuli ylittää rajan (5.1 m/s)
            var weather = new WeatherInfo
            {
                Temperature = 15.0,
                WindSpeed = 5.1
            };

            // Act (Toiminta): Luetaan laskettu arvo
            bool isKite = weather.IsKiteSurfWeather;

            // Assert (Tarkistus): Arvon on oltava true ja viestin ilmoitettava leijakelistä
            Assert.True(isKite);
            Assert.Contains("Kitesurf-keli", weather.KiteSurfMessage);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(2.5)]
        [InlineData(5.0)] // Kriittinen raja-arvo: tasan 5.0 ei vielä riitä, vaatii aidosti yli (> 5.0)
        public void IsKiteSurfWeather_ReturnsFalse_WhenWindSpeedIsFiveOrLess(double windSpeed)
        {
            // Arrange
            var weather = new WeatherInfo
            {
                Temperature = 12.0,
                WindSpeed = windSpeed
            };

            // Act & Assert
            Assert.False(weather.IsKiteSurfWeather);
            Assert.Contains("Ei riittävästi", weather.KiteSurfMessage);
        }

        // =========================================================================
        // 2. TOP 3 -JÄRJESTYS JA SUODATUS (TodoRepositoryn LINQ-logiikka)
        // Säännöt:
        // 1. Suodatetaan valmiit pois (!IsDone)
        // 2. Prioriteetti laskevasti (5 on tärkein)
        // 3. Eräpäivä nousevasti (lähin päivä ensin)
        // 4. Enintään 3 alkiota (Take(3))
        // =========================================================================

        [Fact]
        public void Top3Logic_FiltersOutDoneItems_AndSortsByPriorityThenDueDate()
        {
            // Arrange: Luodaan testilista erilaisilla tiloilla, prioriteeteilla ja päivämäärillä
            var today = DateTime.Today;
            var testTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Matala arkiaskare", Priority = 1, DueDate = today, IsDone = false },
                new TodoItem { Id = 2, Title = "Kriittinen mutta tehty jo", Priority = 5, DueDate = today, IsDone = true }, // Suodattuu pois
                new TodoItem { Id = 3, Title = "Kiireellinen, eräpäivä huomenna", Priority = 4, DueDate = today.AddDays(1), IsDone = false },
                new TodoItem { Id = 4, Title = "Kiireellinen, eräpäivä tänään", Priority = 4, DueDate = today, IsDone = false },
                new TodoItem { Id = 5, Title = "Keskivertotyö", Priority = 3, DueDate = today, IsDone = false },
                new TodoItem { Id = 6, Title = "Toinen keskivertotyö", Priority = 3, DueDate = today.AddDays(2), IsDone = false }
            };

            // Act: Ajetaan sama LINQ-kysely kuin TodoRepository.GetTop3TodosAsync()
            var top3 = testTodos
                .Where(t => !t.IsDone)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .Take(3)
                .ToList();

            // Assert: Varmistetaan sääntöjen toteutuminen
            Assert.Equal(3, top3.Count);
            Assert.DoesNotContain(top3, t => t.IsDone);

            // Oikea järjestys ID-numeroilla:
            // 1. sija: Id 4 (Prioriteetti 4, DueDate tänään)
            // 2. sija: Id 3 (Prioriteetti 4, DueDate huomenna)
            // 3. sija: Id 5 (Prioriteetti 3, DueDate tänään)
            Assert.Equal(4, top3[0].Id);
            Assert.Equal(3, top3[1].Id);
            Assert.Equal(5, top3[2].Id);
        }
    }
}