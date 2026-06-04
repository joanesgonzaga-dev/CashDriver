using CashDriver.ViewModels;

namespace CashDriver.Views;

public partial class GanhosPage : ContentPage
{
	public GanhosPage(GanhosViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}