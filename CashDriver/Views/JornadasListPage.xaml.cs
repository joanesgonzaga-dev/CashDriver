using CashDriver.ViewModels;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;

namespace CashDriver.Views;

public partial class JornadasListPage : ContentPage
{
    private bool _isInitialized;
	public JornadasListPage(JornadasListViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        if (BindingContext is JornadasListViewModel vm)
        {
            //if (!_isInitialized)
            //{
            //    await vm.MontaListaAsync();
            //    _isInitialized = true;
            //}
            await vm.MontaListaAsync();
        }
    }
}