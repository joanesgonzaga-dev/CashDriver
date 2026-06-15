using CashDriver.Services;
using CashDriver.Views.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class FiltrarJornadasPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime dataInicial = DateTime.Now;
    
        [ObservableProperty]
        private DateTime dataFinal = DateTime.Now;

        private readonly JornadaService _jornadaService;

        private readonly Action _fechar;
    
            public FiltrarJornadasPopupViewModel(JornadaService jornadaService, Action fechar)
            {
                _jornadaService = jornadaService;
            _fechar = fechar;
            }
    
            [RelayCommand]
            private async Task Filtrar()
            {
            //if (DataFinal < DataInicial)
            //{
            //    Shell.Current.DisplayAlert("Atenção", "A data final deve ser maior ou igual à data inicial!", "OK");
            //    return;
            //}

            FiltrarJornadasPopup? popup = null;
            var vm = new FiltrarJornadasPopupViewModel(
                _jornadaService,
                () => popup?.Close());

            popup = new FiltrarJornadasPopup(vm);

            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

            _fechar();
        }
    }
}
