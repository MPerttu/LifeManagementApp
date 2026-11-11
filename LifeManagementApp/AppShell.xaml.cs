using LifeManagementApp.Views;

namespace LifeManagementApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rekisteröidään erillinen lisäyssivu reititykseen,
            // jotta sinne voidaan siirtyä GoToAsync("AddTodoPage") -kutsulla
            Routing.RegisterRoute(nameof(AddTodoPage), typeof(AddTodoPage));
            Routing.RegisterRoute(nameof(EditTodoPage), typeof(EditTodoPage));
        }
    }
}