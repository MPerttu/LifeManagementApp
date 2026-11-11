using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LifeManagementApp.Data;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services
{
    /// <summary>
    /// TodoRepository toteuttaa ITodoRepository-rajapinnan käyttäen EF Coren AppDbContextiä.
    /// Kaikki tietokantakyselyt suoritetaan asynkronisesti (async/await), jotta käyttöliittymä ei jäädy.
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        // Konstruktori-injektio: Dependency Injection tuo AppDbContext-olion tälle luokalle automaattisesti
        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hakee kaikki tehtävät tietokannasta listaksi.
        /// .Include(t => t.Category) suorittaa SQL JOIN -kyselyn ja tuo Category-tiedot mukaan
        /// </summary>
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            return await _context.TodoItems
                .Include(t => t.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Etusivun vaatimus (Toiminnallisuus 2p):
        /// 1. Suodatetaan vain keskeneräiset tehtävät (!t.IsDone).
        /// 2. Järjestetään ensin prioriteetin mukaan laskevasti (OrderByDescending: 5 on tärkein).
        /// 3. Järjestetään toissijaisesti eräpäivän mukaan nousevasti (ThenBy: lähin päivämäärä ensin).
        /// 4. Otetaan vain 3 ensimmäistä (Take(3)).
        /// </summary>
        public async Task<List<TodoItem>> GetTop3TodosAsync()
        {
            return await _context.TodoItems
                .Include(t => t.Category)
                .Where(t => !t.IsDone)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .Take(3)
                .ToListAsync();
        }

        /// <summary>
        /// Hakee kaikki ennalta määritellyt kategoriat (Category 1-N relaatio).
        /// </summary>
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        /// <summary>
        /// Lisää uuden Todo-olion tietokantaan ja tallentaa muutokset pysyvästi levylle.
        /// </summary>
        public async Task AddTodoAsync(TodoItem item)
        {
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Päivittää muuttuneen tehtävän tilan tietokantaan.
        /// </summary>
        public async Task UpdateTodoAsync(TodoItem item)
        {
            _context.TodoItems.Update(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Poistaa tehtävän tietokannasta.
        /// </summary>
        public async Task DeleteTodoAsync(TodoItem item)
        {
            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}