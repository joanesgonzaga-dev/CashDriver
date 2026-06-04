using CashDriver.Models;
using CashDriver.Models.Enums;
#if ANDROID
using Java.Lang.Annotation;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Services
{
    public class JornadaService
    {
        public Jornada? JornadaAtual { get; set; }
        public List<TipoDespesa> CadastroDeDespesas { get; set; } = new();
        public ObservableCollection<Meta> CadastroDeMetas { get; } = new();
        public ObservableCollection<Plataforma> CadastroDePlataformas { get; } = new();
        public PersistenceService _persistenceService;

        public JornadaService(PersistenceService persistenceService)
        {
            _persistenceService = persistenceService;

            // Inicializa uma única vez

            //Plataformas
            //CadastroDePlataformas.Add(new Plataforma { Name = "Uber"});
            //CadastroDePlataformas.Add(new Plataforma { Name = "99 pop" });
            //CadastroDePlataformas.Add(new Plataforma { Name = "inDrive" });
            //CadastroDePlataformas.Add(new Plataforma { Name = "Maxim" });
            //CadastroDePlataformas.Add(new Plataforma { Name = "V1" });
            //CadastroDePlataformas.Add(new Plataforma { Name = "XCarro" });
            
            //Metas
            //CadastroDeMetas.Add(new Meta { Nome = "Aluguel", ValorMeta = 2287, ValorAtual = 1200 });
            //CadastroDeMetas.Add(new Meta { Nome = "Meta diária", ValorMeta = 200, ValorAtual = 75.80M });
            //CadastroDeMetas.Add(new Meta { Nome = "Combustível", ValorMeta = 100, ValorAtual = 60 });

            //Despesas
            /*
            CadastroDeDespesas.Add(new TipoDespesa { DescricaoTipo = "Combustível", Tipo = EnumTipoDespesa.Combustivel, NomeIcone = string.Empty });
            CadastroDeDespesas.Add(new TipoDespesa { DescricaoTipo = "Alimentação", Tipo = EnumTipoDespesa.Alimentacao, NomeIcone = string.Empty });
            CadastroDeDespesas.Add(new TipoDespesa { DescricaoTipo = "Manutenção", Tipo = EnumTipoDespesa.Manutencao, NomeIcone = string.Empty });
            CadastroDeDespesas.Add(new TipoDespesa { DescricaoTipo = "Limpeza/Higienização" , Tipo = EnumTipoDespesa.Limpeza, NomeIcone = string.Empty });
            CadastroDeDespesas.Add(new TipoDespesa { DescricaoTipo = "Diversos" , Tipo = EnumTipoDespesa.Diversos, NomeIcone = string.Empty });
            */

            
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
            JornadaAtual = await _persistenceService.ObterJornadaAtivaAsync();
        }

        private async Task CarregaCadastroDeDespesas()
        {
            CadastroDeDespesas.Clear();
            CadastroDeDespesas = await _persistenceService.ObterTiposDespesaAsync();
        }

        private async Task CarregaCadastroDePlataformas()
        {
            CadastroDePlataformas.Clear();
            CadastroDePlataformas = await _persistenceService.ObterTiposDespesaAsync();
        }

        public async Task RemoverDespesaAsync(Guid despesaId)
        {
            try
            {
                var despesa = JornadaAtual.Despesas.FirstOrDefault(x => x.Id == despesaId);

                if (despesa == null)
                {
                    return;
                }
                JornadaAtual.Despesas.Remove(despesa);
                await _persistenceService.RemoverDespesa(despesa);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RemoverGanhoAsync(Guid lancamentoId)
        {
            var ganho = JornadaAtual?.Ganhos.First(g => g.Id == lancamentoId);

            if (ganho == null)
            {
                return;
            }

            JornadaAtual?.Ganhos.Remove(ganho);
            await _persistenceService.RemoverGanho(ganho);
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
                JornadaAtual?.Ganhos.Add(ganho);
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
    }
}
