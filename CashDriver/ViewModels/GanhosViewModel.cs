using CashDriver.Models;
using CashDriver.Services;
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
    public partial class GanhosViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
        public ObservableCollection<Ganho> Ganhos => _jornadaService.JornadaAtual.Ganhos;

        [ObservableProperty]
        private string descricao;

        [ObservableProperty]
        private decimal valor;

        public GanhosViewModel(JornadaService service)
        {
            _jornadaService = service;
        }

        [RelayCommand]
        private void AdicionarGanho()
        {
            if (Valor <= 0) return;

            _jornadaService.JornadaAtual.Ganhos.Add(new Ganho
            {
                Descricao = Descricao,
                Valor = Valor
            });

            // limpar campos
            Descricao = string.Empty;
            Valor = 0;
        }
    }
}
