using CashDriver.Services;
using CashDriver.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CashDriver.Views.Popups;

public partial class FiltrarJornadasPopup : Popup
{
	public FiltrarJornadasPopup(FiltrarJornadasPopupViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private void FecharPopup_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void DataInicialTapped(object sender, TappedEventArgs e)
    {
        DataInicialPicker?.Focus();
    }
}