using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views
{
    public partial class EditTodoPage : ContentPage
    {
        private readonly EditTodoViewModel _viewModel;

        public EditTodoPage(EditTodoViewModel viewModel)
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