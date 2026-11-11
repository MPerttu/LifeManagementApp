using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels
{
    /// <summary>
    /// EditTodoViewModel vastaa valitun tehtävän tietojen muokkaamisesta.
    /// [QueryProperty] ottaa vastaan Shell-navigoinnin mukana tulleen TodoItem-olion.
    /// </summary>
    [QueryProperty(nameof(Item), "TodoItem")]
    public partial class EditTodoViewModel : ObservableObject
    {
        private readonly ITodoRepository _todoRepository;

        // Vastaanotettu TodoItem
        [ObservableProperty]
        private TodoItem? _item;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private int _priority = 3;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Today;

        // Kategoriat pudotusvalikkoa varten (1-N relaatio)
        public ObservableCollection<Category> Categories { get; } = new();

        // Käyttäjän valitsema kategoria
        [ObservableProperty]
        private Category? _selectedCategory;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public EditTodoViewModel(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// Ladataan tietokannasta kaikki kategoriat ja esivalitaan tehtävän nykyinen kategoria.
        /// </summary>
        public async Task LoadCategoriesAsync()
        {
            var cats = await _todoRepository.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats)
            {
                Categories.Add(c);
            }

            // Asetetaan nykyinen kategoria valituksi pudotusvalikossa
            if (Item != null && Item.CategoryId.HasValue)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == Item.CategoryId.Value);
            }
        }

        // Laukeaa automaattisesti, kun Shell asettaa "Item"-arvon navigoinnissa
        partial void OnItemChanged(TodoItem? value)
        {
            if (value != null)
            {
                Title = value.Title;
                Priority = value.Priority;
                DueDate = value.DueDate;

                // Jos kategoriat on jo ladattu, kohdistetaan valinta
                if (Categories.Count > 0 && value.CategoryId.HasValue)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId.Value);
                }
            }
        }

        /// <summary>
        /// Tallentaa muokatun tehtävän tiedot ja päivitetyn kategorian tietokantaan.
        /// </summary>
        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Otsikko ei saa olla tyhjä!";
                return;
            }

            if (Item != null)
            {
                Item.Title = Title.Trim();
                Item.Priority = Priority;
                Item.DueDate = DueDate;

                // Päivitetään kategoriaviittaus (vierasavain ja navigaatio-olio)
                Item.CategoryId = SelectedCategory?.Id;
                Item.Category = SelectedCategory;

                // Päivitetään tietokantaan repositoryn kautta
                await _todoRepository.UpdateTodoAsync(Item);
            }

            // Palataan takaisin listanäkymään
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            // Perutaan muutokset ja palataan takaisin
            await Shell.Current.GoToAsync("..");
        }
    }
}