using CashDriver.Services;
using CashDriver.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CashDriver.Views.Popups;

public partial class LancarDespesaPopup : Popup
{
	public LancarDespesaPopup(LancarDespesaPopupViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    private void FecharPopup_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}