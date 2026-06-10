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
        public decimal TotalGanhos => _jornadaService.JornadaAtual is null?  0 : _jornadaService.JornadaAtual.TotalGanhos;// ?.Ganhos?.Sum(g => g.Valor) ?? 0M;
        public decimal TotalDespesas => _jornadaService.JornadaAtual is null ? 0 : _jornadaService.JornadaAtual.TotalDespesas;// .Despesas?.Sum(d => d.Valor) ?? 0M;
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
            Extrato.CollectionChanged += (s, e) =>
            {
                NotificaUI();
                //OnPropertyChanged(nameof(TemLancamentos));
            };


            //Metas.CollectionChanged += (s, e) =>
            //{
            //    OnPropertyChanged(nameof(TemMetas));
            //};


            if (!WeakReferenceMessenger.Default.IsRegistered<GanhoLancadoMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<GanhoLancadoMessage>(this, (r,m)=> {
                    MontarExtrato();
                });
            }

            if (!WeakReferenceMessenger.Default.IsRegistered<DespesaLancadaMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<DespesaLancadaMessage>(this, (r, m) => {
                    MontarExtrato();
                });
            }

            #endregion
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
            
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
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
                //NotificaUI();
                MontarExtrato();
                CarregarMetas();
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
                _jornadaService?.CriarJornadaAsync();
            }

            //navegar
            await Shell.Current.GoToAsync("//Jornada"); // //Page só funciona se os ShellContent em AppShell.xaml tiverem o atributo Route="SuaPage"
            CarregarDadosJornada();

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
            } 
        }

        private void MontarExtrato()
        {
            Extrato.Clear();

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

                var index = Extrato.TakeWhile(x => x.Data > ganho.CreatedAt).Count();
                Extrato.Insert(index, item);
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

                var index = Extrato.TakeWhile(x => x.Data > despesa.CreatedAt).Count();
                Extrato.Insert(index, item);
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
        }

        public async Task RecuperarAsync()
        {
            await _jornadaService.RecuperarJornadaAtivaAsync();
        }

        [RelayCommand]
        private async Task PausarJornada()
        {
            if (_jornadaService?.JornadaAtual?.Status == EnumStatusJornada.Ativa)
            {
                await _jornadaService.PausarJornadaAsync();
                await RecuperarAsync();
                NotificaUI();
            }
        }

        [RelayCommand]
        private async Task RetomarJornada()
        {
            await _jornadaService.RetomarJornadaPausadaAsync();
            await RecuperarAsync();
            NotificaUI();
        }

        private void NotificaUI()
        {
            OnPropertyChanged(nameof(DuracaoFormatada));
            OnPropertyChanged(nameof(Ativa));
            OnPropertyChanged(nameof(Inativa));
            OnPropertyChanged(nameof(Pausada));
            OnPropertyChanged(nameof(TotalGanhos));
            OnPropertyChanged(nameof(TotalDespesas));
            OnPropertyChanged(nameof(Lucro));
            OnPropertyChanged(nameof(HoraInicio));
            OnPropertyChanged(nameof(TemLancamentos));
        }
    }
}
