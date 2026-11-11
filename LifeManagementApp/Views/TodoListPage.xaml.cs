using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views
{
    public partial class TodoListPage : ContentPage
    {
        private readonly TodoListViewModel _viewModel;

        public TodoListPage(TodoListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Tämä metodi suoritetaan JOKA KERTA kun sivulle saavutaan (myös muokkaussivulta palatessa)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTodosAsync();
        }
    }
}