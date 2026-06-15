#if ANDROID
using AndroidX.ConstraintLayout.Core.State.Helpers;
#endif
using CashDriver.Data;
using CashDriver.Massages;
using CashDriver.Models;
using CashDriver.Models.Enums;
using CashDriver.Services;
using CashDriver.Views;
using CashDriver.Views.Popups;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace CashDriver.ViewModels
{
    public partial class JornadaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
        public bool Ativa => _jornadaService.JornadaAtual != null && (_jornadaService.JornadaAtual?.Status == EnumStatusJornada.Ativa);
        public bool Pausada => _jornadaService.JornadaAtual != null && (_jornadaService.JornadaAtual?.Status ==  EnumStatusJornada.Pausa);
        public bool Inativa => _jornadaService.JornadaAtual == null || _jornadaService.JornadaAtual?.Status == EnumStatusJornada.Encerrada;
        private System.Timers.Timer _timer;
        public decimal TotalGanhos => _jornadaService.JornadaAtual is null?  0 : _jornadaService.JornadaAtual.TotalGanhos;
        public decimal TotalDespesas => _jornadaService.JornadaAtual is null ? 0 : _jornadaService.JornadaAtual.TotalDespesas;
        public decimal Lucro => TotalGanhos - TotalDespesas;
        
        [ObservableProperty]
        private ObservableCollection<Meta> metas = new();
        
        public ObservableCollection<ExtratoItem> Extrato { get; } = new();
        
        public string HoraInicio => "Iniciada às " + _jornadaService?.JornadaAtual?.Inicio.ToShortTimeString();
        
        public bool TemLancamentos =>  Extrato.Any() == true;
        
        public JornadaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;

            #region contabiliza e atualiza tempo na tela
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnPropertyChanged(nameof(DuracaoFormatada));
                });
            };

            _timer.Start();
            #endregion

            #region registro de eventos
            // Notifica a UI que TemLancamentos mudou sempre que a coleção Extrato for alterada
            // (Add, Remove, Clear, etc. disparam CollectionChanged)

            // Este evento é registrado apenas uma vez no construtor,
            // evitando múltiplas inscrições e execução duplicada

            // Mantém a lógica centralizada:
            // a atualização de TemLancamentos não precisa ser chamada manualmente em vários pontos do código
            //Extrato.CollectionChanged += (s, e) =>
            //{
            //    OnPropertyChanged(nameof(TemLancamentos));
            //};


            //Metas.CollectionChanged += (s, e) =>
            //{
            //    OnPropertyChanged(nameof(TemMetas));
            //};


            if (!WeakReferenceMessenger.Default.IsRegistered<GanhoLancadoMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<GanhoLancadoMessage>(this, (r,m)=> {
                    AdicionaGanhoAoExtrato(m.Ganho);
                    NotificaUIValores();
                });
            }

            if (!WeakReferenceMessenger.Default.IsRegistered<DespesaLancadaMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<DespesaLancadaMessage>(this, (r, m) => {
                    AdicionaDespesaAoExtrato(m.Despesa);
                    NotificaUIValores();
                });
            }

            #endregion
        }

        private void AdicionaGanhoAoExtrato(Ganho ganho)
        {
            var item = new ExtratoItem
            {
                Id = Guid.NewGuid(),
                LancamentoId = ganho.Id,
                Tipo = Models.Enums.EnumTipoExtrato.Ganho,
                Descricao = ganho.PlataformaNome,
                Valor = ganho.Valor,
                ValorTexto = $"+ {ganho.Valor.ToString("c", new CultureInfo("pt-br"))}",
                ValorColor = Color.FromArgb("#19c37d"),
                Data = ganho.CreatedAt
            };
            Extrato.Insert(0, item);
        }

        [RelayCommand]
        private async Task AdicionarDespesa()
        {
            if (_jornadaService.JornadaAtual is null || _jornadaService.JornadaAtual.Status != EnumStatusJornada.Ativa)
            {
                await Shell.Current.DisplayAlert("Alerta!", "Não existe jornada Ativa para lançar despesas!", "OK");
                return;
            }

            LancarDespesaPopup? popup = null;

            var vm = new LancarDespesaPopupViewModel(
                _jornadaService,
                () => popup?.Close());

            popup = new LancarDespesaPopup(vm);
            
            var obj = await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        private void AdicionaDespesaAoExtrato(Despesa despesa)
        {
            var tipo = _jornadaService?.CadastroDeDespesas.FirstOrDefault(t => t.Id == despesa.TipoDespesaId);
            var item = new ExtratoItem()
            {
                Id = Guid.NewGuid(),
                LancamentoId = despesa.Id,
                Tipo = Models.Enums.EnumTipoExtrato.Despesa,
                Descricao = tipo.DescricaoTipo,//despesa.Tipo?.DescricaoTipo ?? string.Empty; //tá disparando exception
                Valor = despesa.Valor,
                ValorTexto = $"- {despesa.Valor.ToString("C", new CultureInfo("pt-BR"))}",
                ValorColor = Color.FromArgb("#FF4500"),
                Data = despesa.CreatedAt
            };
            
            Extrato.Insert(0, item);
        }

        [RelayCommand]
        private async Task AdicionarGanho()
        {

            if (_jornadaService.JornadaAtual == null || _jornadaService.JornadaAtual.Status != EnumStatusJornada.Ativa)
            {
                await Shell.Current.DisplayAlert("Alerta!", "Não existe jornada Ativa para lançar ganhos!", "OK");
                return;
            }

            LancarGanhoPopup? popup = null;
            var vm = new LancarGanhoPopupViewModel(
                _jornadaService,
                () => popup?.Close());

            popup = new LancarGanhoPopup(vm);

            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            NotificaUIValores();
        }

        [RelayCommand]
        private async void EncerrarJornada()
        {
            // Simular Encerrar Jornada
            var resultado =  await Application.Current.MainPage.DisplayAlert(
                "Jornada",
                "Confirma o encerramento da jornada?",
                "SIM", "NÃO");

            if (resultado)
            {
                //Persistir neste momento
                await _jornadaService.EncerrarJornadaAsync();
                _jornadaService.JornadaAtual = null;
                Extrato.Clear();
                NotificaUIStatus();
                NotificaUIValores();
            }
        }

        [RelayCommand]
        private async Task CriarMetaRapido()
        {
            await Shell.Current.GoToAsync(nameof(CriarMetaPage));
        }


        [RelayCommand]
        private async Task IniciarJornada()
        {
            if (_jornadaService.JornadaAtual != null && _jornadaService.JornadaAtual.Status == EnumStatusJornada.Ativa)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Jornada",
                    $"Já existe uma Jornada iniciada!",
                    "OK"
                );
            }

            else
            {
                await _jornadaService.CriarJornadaAsync();
            }

            //navegar
            await Shell.Current.GoToAsync("//Jornada"); // //Page só funciona se os ShellContent em AppShell.xaml tiverem o atributo Route="SuaPage"
            CarregarDadosJornada();
            NotificaUIValores();
            NotificaUIStatus();
        }

        public string DuracaoFormatada
        {
            get
            {
                var jornada = _jornadaService.JornadaAtual;

                if (jornada == null)
                    return "00:00:00";
                return jornada?.DuracaoTotalJornada?.ToString(@"hh\:mm\:ss") ?? "00:00:00"; 
            }
        }

        public void CarregarDadosJornada()
        {
            if (_jornadaService.JornadaAtual != null)
            {
                MontarExtrato();
                NotificaUIStatus();
                NotificaUIValores();
            } 
        }

        private void MontarExtrato()
        {

            List<ExtratoItem> itens = new List<ExtratoItem>();

            ////Ganhos
            foreach (var ganho in _jornadaService.JornadaAtual?.Ganhos ?? Enumerable.Empty<Ganho>())
            {
                var item = new ExtratoItem
                {
                    Id = Guid.NewGuid(),
                    LancamentoId = ganho.Id,
                    Tipo = Models.Enums.EnumTipoExtrato.Ganho,
                    Descricao = ganho.PlataformaNome,
                    Valor = ganho.Valor,
                    ValorTexto = $"+ {ganho.Valor.ToString("c", new CultureInfo("pt-br"))}",
                    ValorColor = Color.FromArgb("#19c37d"),
                    Data = ganho.CreatedAt
                };

                //var index = Extrato.TakeWhile(x => x.Data > ganho.CreatedAt).Count();
                itens.Add(item); //Insert(index, item);

            }

            //Despesas
            foreach (var despesa in _jornadaService?.JornadaAtual?.Despesas ?? Enumerable.Empty<Despesa>())
            {
                var tipo = _jornadaService?.CadastroDeDespesas.FirstOrDefault(t => t.Id == despesa.TipoDespesaId);
                var item = new ExtratoItem()
                {
                    Id = Guid.NewGuid(),
                    LancamentoId = despesa.Id,
                    Tipo = Models.Enums.EnumTipoExtrato.Despesa,
                    Descricao = tipo.DescricaoTipo,//despesa.Tipo?.DescricaoTipo ?? string.Empty; //tá disparando exception
                    Valor = despesa.Valor,
                    ValorTexto = $"- {despesa.Valor.ToString("C", new CultureInfo("pt-BR"))}",
                    ValorColor = Color.FromArgb("#FF4500"),
                    Data = despesa.CreatedAt
                };

                //var index = Extrato.TakeWhile(x => x.Data > despesa.CreatedAt).Count();
                itens.Add(item); //Insert(index, item);
            }

            Extrato.Clear();
            var itensOrdenados = itens.OrderByDescending(i => i.Data);

            foreach (var item in itensOrdenados)
            {
                Extrato.Add(item);
            }
        }

        public void CarregarMetas()
        {
            //Metas.Clear();
            //foreach (var meta in _jornadaService.JornadaAtual?.Metas.Where(m => m.Selecionada) ?? Enumerable.Empty<Meta>())
            //{
            //    Metas.Add(meta);
            //}
        }

        [RelayCommand]
        public async Task DeletarLancamentoExtrato(ExtratoItem item)
        {
            var result = await Application.Current.MainPage.DisplayAlert(
            "Exxcluir",
            "Confirma a exclusão do lançamento?",
            "SIM",
            "NÃO");

            if (!result)
            {
                return;
            }

            //Não confundir com item.Id, que é o Id do ExtratoItem (apenas para controle da UI), o item.LancamentoId é o Id do ganho ou despesa na jornada
            if (item.Tipo == EnumTipoExtrato.Ganho)
            {
                await _jornadaService.RemoverGanhoAsync(item.LancamentoId);
            }

            else if (item.Tipo == EnumTipoExtrato.Despesa)
            {
                await _jornadaService.RemoverDespesaAsync(item.LancamentoId);
            }

            var itemToRemove = Extrato.FirstOrDefault(i => i.LancamentoId == item.LancamentoId);
            if (itemToRemove != null)
            {
                Extrato.Remove(itemToRemove);
            }

            NotificaUIValores();
        }

        public async Task RecuperarJornadaAtivaAsync()
        {
            await _jornadaService.RecuperarJornadaAtivaAsync();
        }

        [RelayCommand]
        private async Task PausarJornada()
        {
            if (_jornadaService?.JornadaAtual?.Status == EnumStatusJornada.Ativa)
            {
                await _jornadaService.PausarJornadaAsync();
                await RecuperarJornadaAtivaAsync();
                NotificaUIStatus();
            }
        }

        [RelayCommand]
        private async Task RetomarJornada()
        {
            await _jornadaService.RetomarJornadaPausadaAsync();
            await RecuperarJornadaAtivaAsync();
            NotificaUIStatus();
            NotificaUIValores();
        }

        private void NotificaUIStatus()
        {
            OnPropertyChanged(nameof(DuracaoFormatada));
            OnPropertyChanged(nameof(Ativa));
            OnPropertyChanged(nameof(Inativa));
            OnPropertyChanged(nameof(Pausada));
            OnPropertyChanged(nameof(HoraInicio));
           
        }

        private void NotificaUIValores()
        {
            OnPropertyChanged(nameof(TotalGanhos));
            OnPropertyChanged(nameof(TotalDespesas));
            OnPropertyChanged(nameof(Lucro));
            OnPropertyChanged(nameof(TemLancamentos));
        }
    }
}
