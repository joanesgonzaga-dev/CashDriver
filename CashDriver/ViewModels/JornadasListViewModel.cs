using CashDriver.Models;
using CashDriver.Services;
using CashDriver.Views.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashDriver.ViewModels
{
    public partial class JornadasListViewModel : ObservableObject
    {
        JornadaService _jornadaService;

        [ObservableProperty]
        private ObservableCollection<Jornada> jornadas = new();

        public string QtdJornadas => Jornadas is null ? "00" : Jornadas.Count.ToString("D2");

        public decimal TotalGanhosPeriodo => Jornadas is null ? 0M : Jornadas.Sum(j => j.TotalGanhos);
        
        public decimal TotalDespesasPeriodo => Jornadas is null ? 0M : Jornadas.Sum(j => j.TotalDespesas);
        
        public decimal SaldoPeriodo => (TotalGanhosPeriodo - TotalDespesasPeriodo);

        public string TotalDeHoras
        {
            get
            {
                try
                {
                    var total = TimeSpan.FromTicks(Jornadas.Sum(j => j.TempoAcumulado?.Ticks ?? 0));
                    return $"{(int)total.TotalHours:D2}:{total.Minutes:D2}:{total.Seconds:D2}";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string LucroMedio
        {
            get
            {
                decimal media = Jornadas.Count == 0 ? 0 : Jornadas.Sum(j => j.TotalGanhos - j.TotalDespesas) / Jornadas.Count;
                return media.ToString("C2");
            }
        }

        public JornadasListViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        public async Task MontaListaAsync()
        {
            //Isso troca a referência da coleção inteira. A partir daí, o Binding detecta que a propriedade Jornadas mudou (pois é [ObservableProperty] jornadas)
            //e recria o ItemsSource,
            //atualizando corretamente os valores exibidos.
            Jornadas = new ObservableCollection<Jornada>(await _jornadaService.RetornarJornadasPeriodo());
            NotificaUI();
        }

        private void NotificaUI()
        {
            OnPropertyChanged(nameof(QtdJornadas));
            OnPropertyChanged(nameof(TotalDeHoras));
            OnPropertyChanged(nameof(LucroMedio));
            OnPropertyChanged(nameof(TotalGanhosPeriodo));
            OnPropertyChanged(nameof(TotalDespesasPeriodo));
            OnPropertyChanged(nameof(SaldoPeriodo));
        }

        [RelayCommand]
        private async Task Filtrar()
        {
            FiltrarJornadasPopup? popup = null;
            var vm = new FiltrarJornadasPopupViewModel(
                _jornadaService,
                () => popup?.Close());

            popup = new FiltrarJornadasPopup(vm);
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);


        }
    }
}
