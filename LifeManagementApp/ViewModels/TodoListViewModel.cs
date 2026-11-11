using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;
using Microsoft.Maui.Controls;

namespace LifeManagementApp.ViewModels
{
    public partial class TodoListViewModel : ObservableObject
    {
        private readonly ITodoRepository _todoRepository;

        public ObservableCollection<TodoItem> Todos { get; } = new();

        [ObservableProperty]
        private bool _isLoading;

        public TodoListViewModel(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [RelayCommand]
        public async Task LoadTodosAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var list = await _todoRepository.GetAllTodosAsync();

                Todos.Clear();
                foreach (var item in list)
                {
                    Todos.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Lajittelee nykyisen listan prioriteetin mukaan (korkein 5 ensin).
        /// Kurssivaatimus: vähintään kaksi eri lajittelutapaa.
        /// </summary>
        [RelayCommand]
        public void SortByPriority()
        {
            var sorted = Todos.OrderByDescending(t => t.Priority).ToList();
            Todos.Clear();
            foreach (var item in sorted)
            {
                Todos.Add(item);
            }
        }

        /// <summary>
        /// Lajittelee nykyisen listan eräpäivän mukaan (lähin päivämäärä ensin).
        /// </summary>
        [RelayCommand]
        public void SortByDate()
        {
            var sorted = Todos.OrderBy(t => t.DueDate).ToList();
            Todos.Clear();
            foreach (var item in sorted)
            {
                Todos.Add(item);
            }
        }

        /// <summary>
        /// Päivittää tehtävän valmius-tilan (IsDone) tietokantaan, kun käyttäjä painaa kuitattua.
        /// </summary>
        [RelayCommand]
        public async Task ToggleDoneAsync(TodoItem item)
        {
            if (item == null) return;
            await _todoRepository.UpdateTodoAsync(item);
        }

        /// <summary>
        /// Poistaa tehtävän tietokannasta ja listalta.
        /// Pyytää ensin käyttäjältä vahvistuksen DisplayAlert-dialogilla.
        /// </summary>
        [RelayCommand]
        public async Task DeleteTodoAsync(TodoItem? item)
        {
            if (item == null) return;

            // Varmistusdialogi käyttäjälle
            bool confirm = await Shell.Current.DisplayAlert(
                "Vahvista poisto",
                $"Haluatko varmasti poistaa tehtävän \"{item.Title}\"?",
                "Poista",
                "Peruuta");

            if (!confirm) return;

            // Poistetaan tietokannasta repositoryn kautta (välitetään item-olio)
            await _todoRepository.DeleteTodoAsync(item);

            // Poistetaan UI-listasta
            Todos.Remove(item);
        }

        /// <summary>
        /// Navigoi muokkaussivulle ja välittää valitun TodoItem-olion parametrina sanakirjassa.
        /// </summary>
        [RelayCommand]
        public async Task GoToEditTodoPageAsync(TodoItem item)
        {
            if (item == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "TodoItem", item }
            };

            await Shell.Current.GoToAsync("EditTodoPage", navigationParameter);
        }

        /// <summary>
        /// MAUI Shell -navigointi uuden tehtävän lisäyssivulle (AddTodoPage).
        /// </summary>
        [RelayCommand]
        public async Task GoToAddTodoPageAsync()
        {
            await Shell.Current.GoToAsync("AddTodoPage");
        }
    }
}