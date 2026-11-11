using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views
{
    public partial class AddTodoPage : ContentPage
    {
        private readonly AddTodoViewModel _viewModel;

        public AddTodoPage(AddTodoViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCategoriesAsync();
        }
    }
}