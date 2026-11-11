using System.Net.Http;
using Microsoft.Extensions.Logging;
using LifeManagementApp.Data;
using LifeManagementApp.Services;
using LifeManagementApp.ViewModels;
using LifeManagementApp.Views;

namespace LifeManagementApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ==========================================
            // DEPENDENCY INJECTION (DI) REKISTERÖINNIT
            // ==========================================

            // 1. Data & Palvelut (Services)
            // AppDbContext luodaan Singletonina sovelluksen paikallista kantaa varten
            builder.Services.AddSingleton<AppDbContext>();

            // HttpClient sääpyyntöjä varten
            builder.Services.AddSingleton<HttpClient>();

            // Rajapinnat ja niiden toteutukset (Kytketään rajapinta toteutusluokkaan)
            builder.Services.AddSingleton<ITodoRepository, TodoRepository>();
            builder.Services.AddSingleton<IWeatherService, OpenMeteoWeatherService>();

            // 2. ViewModelit
            // Transient = luodaan uusi instanssi tarvittaessa (tai pidetään näkymäkohtaisena)
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<TodoListViewModel>();
            builder.Services.AddTransient<AddTodoViewModel>();

            // 3. Sivut (Views)
            // Rekisteröidään sivut, jotta DI osaa syöttää niille ViewModelit konstruktorin kautta
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<TodoListPage>();
            builder.Services.AddTransient<AddTodoPage>();

            builder.Services.AddTransient<EditTodoViewModel>();
            builder.Services.AddTransient<EditTodoPage>();

            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

            return builder.Build();
        }
    }
}