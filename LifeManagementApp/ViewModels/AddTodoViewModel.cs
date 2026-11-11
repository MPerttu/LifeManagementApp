using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels
{
    public partial class AddTodoViewModel : ObservableObject
    {
        private readonly ITodoRepository _todoRepository;

        // Käyttäjän lomakkeelle syöttämä otsikko
        [ObservableProperty]
        private string _title = string.Empty;

        // Prioriteetti (1-5), oletuksena 3
        [ObservableProperty]
        private int _priority = 3;

        // Eräpäivä, oletuksena huominen
        [ObservableProperty]
        private DateTime _dueDate = DateTime.Today.AddDays(1);

        // Kategoriat pudotusvalikkoa varten (1-N relaatio)
        public ObservableCollection<Category> Categories { get; } = new();

        // Käyttäjän valitsema kategoria
        [ObservableProperty]
        private Category? _selectedCategory;

        // Virheviesti, jos validointi pettää
        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public AddTodoViewModel(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// Ladataan tietokannasta kaikki ennalta määritellyt kategoriat pudotusvalikkoon.
        /// </summary>
        public async Task LoadCategoriesAsync()
        {
            var cats = await _todoRepository.GetAllCategoriesAsync();
            Categories.Clear();
            foreach (var c in cats)
            {
                Categories.Add(c);
            }

            // Oletusvalintana listan ensimmäinen kategoria
            if (Categories.Count > 0 && SelectedCategory == null)
            {
                SelectedCategory = Categories[0];
            }
        }

        /// <summary>
        /// Tallentaa uuden tehtävän. Sisältää kurssivaatimuksen mukaisen validoinnin:
        /// "Tyhjää otsikkoa ei hyväksytä."
        /// </summary>
        [RelayCommand]
        public async Task SaveTodoAsync()
        {
            // Validointi: string.IsNullOrWhiteSpace tarkistaa sekä tyhjän että pelkät välilyönnit
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Otsikko ei saa olla tyhjä!";
                return;
            }

            ErrorMessage = string.Empty;

            var newTodo = new TodoItem
            {
                Title = Title.Trim(),
                Priority = Priority,
                DueDate = DueDate,
                IsDone = false,
                CategoryId = SelectedCategory?.Id // Tallennetaan valitun kategorian ID relaatiota varten
            };

            // Tallennus tietokantaan repositoryn kautta
            await _todoRepository.AddTodoAsync(newTodo);

            // Nollataan kentät seuraavaa lisäystä varten
            Title = string.Empty;
            Priority = 3;
            DueDate = DateTime.Today.AddDays(1);

            // Shell-navigointi taaksepäin edelliselle sivulle ("..")
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            // Perutaan ja palataan takaisin
            await Shell.Current.GoToAsync("..");
        }
    }
}