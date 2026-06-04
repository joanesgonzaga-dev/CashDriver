using CashDriver.Models;
using CashDriver.Services;
using CashDriver.ViewModels;
using CashDriver.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace CashDriver.Views;

public partial class DespesasListPage : ContentPage
{
	private readonly JornadaService _jornadaService;
	public DespesasListPage(DespesasListViewModel vm, JornadaService jornadaService)
	{
		InitializeComponent();
		_jornadaService = jornadaService;
		BindingContext = vm;
	}

	//private async Task LancarDespesaRapido(TipoDespesa tipoDespesa)
	//{
	//	LancarDespesaPopup? popup = null;

	//	var vm = new LancarDespesaPopupViewModel(
	//		_jornadaService,
			
	//		() => popup?.Close());

	//	popup = new LancarDespesaPopup(vm);

	//	await this.ShowPopupAsync(popup);
	//}

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TipoDespesa tipo)
        {
            ((CollectionView)sender).SelectedItem = null; //anula a seleçao anterior
            //await LancarDespesaRapido(tipo);
        }
    }
}