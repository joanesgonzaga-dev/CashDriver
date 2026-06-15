using CashDriver.Models;
using CashDriver.Models.Enums;
using System.Collections.ObjectModel;

#if ANDROID
using Java.Lang.Annotation;
#endif

namespace CashDriver.Services
{
    public class JornadaService
    {
        public Jornada? JornadaAtual { get; set; }
        public List<TipoDespesa> CadastroDeDespesas { get; set; } = new();
        public ObservableCollection<Meta> CadastroDeMetas { get; } = new();
        
        public List<Plataforma> CadastroDePlataformas = new();
        
        public PersistenceService _persistenceService;

        public JornadaService(PersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
        }

        public async Task CriarJornadaAsync()    
        {
            if (JornadaAtual is null)
            {
                JornadaAtual = new Jornada
                {
                    Inicio = DateTime.Now,
                    InicioPeriodoAtual = DateTime.Now,
                    TempoAcumulado = TimeSpan.Zero,
                    Status = Models.Enums.EnumStatusJornada.Ativa,
                    Ganhos = new(),
                    Metas = CadastroDeMetas,
                    Despesas = new()
                   
                };

                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Nova jornada",
                    "Já existe uma jornada iniciada",
                    "OK");
            }
        }

        public async Task EncerrarJornadaAsync()
        {
            try
            {
                JornadaAtual.Termino = DateTime.Now;
                JornadaAtual.Status = EnumStatusJornada.Encerrada;
                JornadaAtual.TempoAcumulado += (DateTime.Now - JornadaAtual.InicioPeriodoAtual);
                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
                JornadaAtual = null;
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        public async Task RecuperarJornadaAtivaAsync()
        {
            await CarregaCadastroDeDespesas();
            await CarregaCadastroDePlataformas();
            JornadaAtual = await _persistenceService.ObterJornadaAtivaAsync();
        }

        private async Task CarregaCadastroDeDespesas()
        {
            CadastroDeDespesas.Clear();
            CadastroDeDespesas = await _persistenceService.ObterTiposDespesaAsync();
        }

        public async Task CarregaCadastroDePlataformas()
        {
            CadastroDePlataformas.Clear();
            CadastroDePlataformas = await _persistenceService.ObterPlataformasAsync();
        }

        public async Task RemoverDespesaAsync(Guid despesaId)
        {
            var despesa = JornadaAtual.Despesas.FirstOrDefault(x => x.Id == despesaId);
            if (despesa == null)
            {
                return;
            }

            JornadaAtual.TotalDespesas -= despesa.Valor;
            JornadaAtual.Lucro = JornadaAtual.TotalGanhos - JornadaAtual.TotalDespesas;
            await _persistenceService.RemoverDespesa(despesa);
            await _persistenceService.SalvarJornadaAsync(JornadaAtual);
        }

        public async Task RemoverGanhoAsync(Guid ganhoId)
        {
            var ganho = JornadaAtual?.Ganhos.First(g => g.Id == ganhoId);
            if (ganho == null)
            {
                return;
            }

            JornadaAtual.TotalGanhos -= ganho.Valor;
            JornadaAtual.Lucro = JornadaAtual.TotalGanhos - JornadaAtual.TotalDespesas;
            await _persistenceService.RemoverGanho(ganho);
            await _persistenceService.SalvarJornadaAsync(JornadaAtual);
        }

        public async Task LancarDespesaAsync(Despesa despesa)
        {
            try
            {
                if (JornadaAtual == null)
                {
                    throw new InvalidOperationException("Não existe jornada ativa!");
                }

                await _persistenceService.AdicionarDespesaAsync(despesa);
                JornadaAtual.TotalDespesas += despesa.Valor;
                JornadaAtual.Lucro = JornadaAtual.TotalGanhos - JornadaAtual.TotalDespesas;
                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LancarGanhoAsync(Ganho ganho)
        {
            try
            {
                if (JornadaAtual == null)
                {
                    throw new InvalidOperationException("Não existe jornada ativa!");
                }

                await _persistenceService.AdicionarGanhoAsync(ganho);
                JornadaAtual.TotalGanhos += ganho.Valor;
                JornadaAtual.Lucro = JornadaAtual.TotalGanhos - JornadaAtual.TotalDespesas;
                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PausarJornadaAsync()
        {
            try
            {
                JornadaAtual.Status = EnumStatusJornada.Pausa;
                JornadaAtual.TempoAcumulado += (DateTime.Now - JornadaAtual.InicioPeriodoAtual);
                JornadaAtual.InicioPeriodoAtual = null;
                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RetomarJornadaPausadaAsync()
        {
            if (JornadaAtual.Status == EnumStatusJornada.Pausa)
            {
                JornadaAtual.Status = EnumStatusJornada.Ativa;
                JornadaAtual.InicioPeriodoAtual = DateTime.Now;
                await _persistenceService.SalvarJornadaAsync(JornadaAtual);
            }
        }

        public async Task<IEnumerable<Jornada>> RetornarJornadasPeriodo()
        {
            return await _persistenceService.ObterJornadasAsync();
        }
    }
}
