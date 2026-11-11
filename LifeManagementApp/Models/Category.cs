using System.Collections.Generic;

namespace LifeManagementApp.Models
{
    /// <summary>
    /// Category edustaa tehtäväkategoriaa (esim. Työ, Koulu, Koti).
    /// Muodostaa 1–N-relaation TodoItem-taulun kanssa.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigaatio-ominaisuus: Yhdellä kategorialla voi olla useita tehtäviä (1-N)
        public List<TodoItem> TodoItems { get; set; } = new();
    }
}