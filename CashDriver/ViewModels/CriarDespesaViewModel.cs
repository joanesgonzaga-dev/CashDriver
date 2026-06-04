using CashDriver.Models;
using CashDriver.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class CriarDespesaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;

        [ObservableProperty]
        private string? descricao;

        [ObservableProperty]
        private decimal valor;

        public CriarDespesaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        private async Task Salvar()
        {
            _jornadaService?.JornadaAtual?.Despesas.Add(new()
            {
                JornadaId = _jornadaService.JornadaAtual.Id,
                
                Tipo      = null,
                Valor = Valor,
            });
           
        }
    }
}
