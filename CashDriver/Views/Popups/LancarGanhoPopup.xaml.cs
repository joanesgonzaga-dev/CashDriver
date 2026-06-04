using CashDriver.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CashDriver.Views.Popups;

public partial class LancarGanhoPopup : Popup
{
	public LancarGanhoPopup(LancarGanhoPopupViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    private void FecharPopup_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}