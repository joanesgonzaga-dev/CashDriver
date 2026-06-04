using CashDriver.ViewModels;

namespace CashDriver.Views;

public partial class MetasListPage : ContentPage
{
	public MetasListPage(MetasListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}