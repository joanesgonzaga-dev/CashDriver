using CashDriver.Models;
using CashDriver.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CashDriver.ViewModels
{
    public partial class JornadasListViewModel : ObservableObject
    {
        JornadaService _jornadaService;
        PersistenceService _persistenceService;

        [ObservableProperty]
        private ObservableCollection<Jornada> jornadas = new();

        public string QtdJornadas => Jornadas is null ? "00" : Jornadas.Count.ToString("D2");

        public decimal TotalGanhosPeriodo => Jornadas is null ? 0M : Jornadas.Sum(j => j.TotalGanhos);
        //public string strTotalGanhosPeriodo => Jornadas is null ? "R$ 00,00": Jornadas.Sum(j => j.TotalGanhos).ToString("D2");
        public decimal TotalDespesasPeriodo => Jornadas is null ? 0M : Jornadas.Sum(j => j.TotalDespesas);
        //public string strTotalDespesasPeriodo => Jornadas is null ? "R$ 00,00" : Jornadas.Sum(j => j.TotalDespesas).ToString("D2");
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
                try
                {
                    decimal media = Jornadas.Count == 0 ? 0 : Jornadas.Sum(j => j.TotalGanhos - j.TotalDespesas) / Jornadas.Count;
                    return media.ToString("C2");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }
        }

        public JornadasListViewModel(JornadaService jornadaService, PersistenceService persistenceService)
        {
            _jornadaService = jornadaService;
            _persistenceService = persistenceService;
        }

        public async Task MontaListaAsync()
        {
            try
            {
                //Isso troca a referência da coleção inteira. A partir daí, o Binding detecta que a propriedade Jornadas mudou (pois é [ObservableProperty] jornadas)
                //e recria o ItemsSource,
                //atualizando corretamente os valores exibidos.
                Jornadas = new ObservableCollection<Jornada>(await _persistenceService.ObterJornadasAsync());
                
                OnPropertyChanged(nameof(QtdJornadas));
                OnPropertyChanged(nameof(TotalDeHoras));
                OnPropertyChanged(nameof(LucroMedio));
                OnPropertyChanged(nameof(TotalGanhosPeriodo));
                OnPropertyChanged(nameof(TotalDespesasPeriodo));
                OnPropertyChanged(nameof(SaldoPeriodo));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
