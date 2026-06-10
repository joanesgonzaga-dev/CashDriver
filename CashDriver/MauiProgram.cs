using CashDriver.Data;
using CashDriver.Services;
using CashDriver.ViewModels;
using CashDriver.Views;
using CommunityToolkit.Maui;
#if ANDROID
using Javax.Xml.Validation;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;


namespace CashDriver
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("Inter_18pt-Regular.ttf", "Inter18ptRegular");
                    fonts.AddFont("Inter_18pt-Bold.ttf", "Inter18ptBold");
                    fonts.AddFont("Inter_18pt-SemiBold.ttf", "Inter18ptSemiBold");
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Filename={DatabaseConstants.DatabasePath}");

                # if DEBUG
                    options.EnableSensitiveDataLogging();
                #endif
            });

            builder.Services.AddSingleton<JornadaService>();
            builder.Services.AddTransient<JornadaViewModel>();
            builder.Services.AddTransient<JornadaPage>();
            
            builder.Services.AddTransient<CriarMetaViewModel>();
            builder.Services.AddTransient<CriarMetaPage>();
            builder.Services.AddTransient<MetasListPage>();
            builder.Services.AddTransient<MetasListViewModel>();

            builder.Services.AddTransient<GanhosViewModel>();
            builder.Services.AddTransient<GanhosPage>();

            builder.Services.AddTransient<DespesasListPage>();
            builder.Services.AddTransient<DespesasListViewModel>();

            builder.Services.AddTransient<JornadasListPage>();
            builder.Services.AddTransient<JornadasListViewModel>();

            builder.Services.AddScoped<PersistenceService>();

            var culture = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            SQLitePCL.Batteries.Init();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                //EnsureCreated(): Alterações em entidades NÃO atualizam schema automaticamente


                //if (File.Exists(DatabaseConstants.DatabasePath))
                //{
                //    File.Delete(DatabaseConstants.DatabasePath);
                //}
                db.Database.EnsureCreated();

            }

            return app;
        }
    }
}
