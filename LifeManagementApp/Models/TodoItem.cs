using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeManagementApp.Models
{
    /// <summary>
    /// TodoItem-luokka edustaa sovelluksen yksittäistä tehtävää.
    /// Perii ObservableObject-luokan, jotta sen kenttien muutokset
    /// päivittyvät automaattisesti käyttöliittymään (Data Binding).
    /// </summary>
    public partial class TodoItem : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private int _priority = 3;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Today;

        [ObservableProperty]
        private bool _isDone = false;

        // --- RELAATIO (1-N) ---
        // Vierasavain (Foreign Key), joka osoittaa Category-taulun pääavaimeen
        [ObservableProperty]
        private int? _categoryId;

        // Navigaatio-ominaisuus Category-olioon
        [ObservableProperty]
        private Category? _category;
    }
}