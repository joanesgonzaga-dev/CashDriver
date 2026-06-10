using CashDriver.Massages;
using CashDriver.Models;
using CashDriver.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class LancarGanhoPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal valor;

        [ObservableProperty]
        private string observacoes;

        [ObservableProperty]
        private ObservableCollection<Plataforma> plataformas = new();

        [ObservableProperty]
        private Plataforma plataformaSelecionada;

        private readonly JornadaService _jornadaService;
        private readonly Action _fechar;

        public LancarGanhoPopupViewModel(JornadaService jornadaService, Action fechar)
        {
            _jornadaService = jornadaService;
            _fechar = fechar;
           CarregarPlataformas();
        }

        public void CarregarPlataformas()
        {
            Plataformas.Clear();
            foreach (var plataforma in _jornadaService.CadastroDePlataformas ?? Enumerable.Empty<Plataforma>())
            {
                Plataformas.Add(plataforma);
            }
        }

        [RelayCommand]
        private async Task Lancar()
        {
            if (Valor <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe um valor válido!", "OK");
                return;
            }

            if (PlataformaSelecionada is null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma plataforma!", "OK");
                return;
            }

            var ganho = new Ganho
            {
                PlataformaId = PlataformaSelecionada.Id,
                JornadaId = _jornadaService.JornadaAtual.Id,
                Descricao = Observacoes,
                Valor = Valor,
                PlataformaNome = PlataformaSelecionada.Name,
                CreatedAt = DateTime.Now
            };

            await _jornadaService.LancarGanhoAsync(ganho);
            WeakReferenceMessenger.Default.Send(new GanhoLancadoMessage(ganho));
            _fechar.Invoke();
        }
    }
}
