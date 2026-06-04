using CashDriver.Massages;
using CashDriver.Models;
using CashDriver.Models.Enums;
using CashDriver.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class LancarDespesaPopupViewModel : ObservableObject
    {

        [ObservableProperty]
        private string valor = "";

        [ObservableProperty]
        private string nome;

        [ObservableProperty]
        private string observacoes;

        [ObservableProperty]
        private ObservableCollection<TipoDespesa> tiposDeDespesas = new();

        [ObservableProperty]
        private TipoDespesa tipoDespesaSelecionada;

        private readonly JornadaService _jornadaService;

        private readonly Action _fechar;

        public LancarDespesaPopupViewModel(JornadaService jornadaService, Action fechar)
        {
            _jornadaService = jornadaService;
            _fechar = fechar;

            CarregarTiposDeDespesas();
        }

        [RelayCommand]
        private async Task Lancar()
        {

            decimal _valorDecimal = 0L;

            if (TipoDespesaSelecionada is null || TipoDespesaSelecionada.Id <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "selecione o tipo de Despesa!", "OK");
                return;
            }

            var texto = Valor.Replace(".",",");
            if (decimal.TryParse(texto, out var _valor))
            {
                _valorDecimal = _valor;
            }

            if (_valorDecimal <= 0M)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe um valor válido!", "OK");
                return;
            }

            var despesa = new Despesa
            {
                JornadaId = _jornadaService?.JornadaAtual?.Id,
                Detalhes = Observacoes,
                TipoDespesaId = TipoDespesaSelecionada.Id,
                Valor = _valorDecimal,
                CreatedAt = DateTime.Now
            };

            await _jornadaService?.LancarDespesaAsync(despesa);
            despesa.Tipo = TipoDespesaSelecionada;
            WeakReferenceMessenger.Default.Send(new DespesaLancadaMessage(despesa));
            _fechar.Invoke();
        }

        public void CarregarTiposDeDespesas()
        {
            TiposDeDespesas.Clear();
            foreach (var tipoDespesa in _jornadaService.CadastroDeDespesas ?? Enumerable.Empty<TipoDespesa>())
            {
                TiposDeDespesas.Add(tipoDespesa);
            }
        }
    }
}
