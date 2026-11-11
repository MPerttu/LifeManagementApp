using System.Collections.Generic;
using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services
{
    /// <summary>
    /// ITodoRepository määrittelee sopimuksen (rajapinnan) tietokantaoperaatioille.
    /// Tämän ansiosta ViewModelit ovat riippumattomia suorasta tietokantatoteutuksesta.
    /// </summary>
    public interface ITodoRepository
    {
        // Hakee kaikki tehtävät tietokannasta asynkronisesti
        Task<List<TodoItem>> GetAllTodosAsync();

        // Hakee etusivulle vaaditut 3 tärkeintä keskeneräistä tehtävää:
        // Järjestys: ensin korkein prioriteetti (5 -> 1), sitten aikaisin eräpäivä
        Task<List<TodoItem>> GetTop3TodosAsync();

        // Tallentaa uuden tehtävän SQLite-tietokantaan
        Task AddTodoAsync(TodoItem item);

        // Päivittää olemassa olevan tehtävän tilan (esim. kun IsDone muuttuu)
        Task UpdateTodoAsync(TodoItem item);

        // Poistaa tehtävän tietokannasta
        Task DeleteTodoAsync(TodoItem item);

        Task<List<Category>> GetAllCategoriesAsync();
    }
}