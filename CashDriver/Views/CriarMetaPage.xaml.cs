using CashDriver.Models;
using CashDriver.ViewModels;

namespace CashDriver.Views;

public partial class CriarMetaPage : ContentPage
{
    public CriarMetaPage(CriarMetaViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {

    }

    private void Entry_Unfocused_1(object sender, FocusEventArgs e)
    {

    }

    private void Entry_Unfocused_2(object sender, FocusEventArgs e)
    {

    }
}