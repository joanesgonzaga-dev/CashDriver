using CashDriver.ViewModels;

namespace CashDriver.Views;

public partial class IniciarJornadaPage : ContentPage
{
	public IniciarJornadaPage(IniciarJornadaViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}