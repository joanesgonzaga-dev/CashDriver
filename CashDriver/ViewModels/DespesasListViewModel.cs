using CashDriver.Models;
using CashDriver.Services;
using CashDriver.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class DespesasListViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
        public List<TipoDespesa> Despesas => _jornadaService.CadastroDeDespesas;

        public DespesasListViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        private async Task CriarDespesaRapido()
        {
            await Shell.Current.GoToAsync(nameof(CriarDespesaPage));
        }
    }
}
