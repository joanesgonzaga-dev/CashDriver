using CashDriver.ViewModels;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;

namespace CashDriver.Views;

public partial class JornadaPage : ContentPage
{
    private bool _initialized;
	public JornadaPage(JornadaViewModel vm)
	{
		InitializeComponent();
#if ANDROID
        StatusBar.SetColor(Color.FromArgb("#071B34"));
        StatusBar.SetStyle(StatusBarStyle.LightContent);
#endif
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is JornadaViewModel vm)
        {
            if (!_initialized)
            {
                await vm.RecuperarAsync();
                _initialized = true;
            }
           // await vm.RecuperarAsync();
            vm.CarregarDadosJornada();
        }
       
    }
}