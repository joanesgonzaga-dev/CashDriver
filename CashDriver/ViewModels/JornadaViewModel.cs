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
using System.Globalization;

namespace CashDriver.ViewModels
{
    public partial class JornadaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
       
        [ObservableProperty]
        private Jornada? jornadaAtual;

        public bool Ativa => JornadaAtual != null && (JornadaAtual?.Status == EnumStatusJornada.Ativa);

        public bool Pausada => JornadaAtual != null && (JornadaAtual?.Status ==  EnumStatusJornada.Pausa);
        public bool Inativa => JornadaAtual == null || JornadaAtual?.Status == EnumStatusJornada.Encerrada;

        private System.Timers.Timer _timer;

        public decimal TotalGanhos => _jornadaService.JornadaAtual is null?  0 : _jornadaService.JornadaAtual.TotalGanhos;// ?.Ganhos?.Sum(g => g.Valor) ?? 0M;

        public decimal TotalDespesas => _jornadaService.JornadaAtual is null ? 0 : _jornadaService.JornadaAtual.TotalDespesas;// .Despesas?.Sum(d => d.Valor) ?? 0M;

        public decimal Lucro => TotalGanhos - TotalDespesas;

        [ObservableProperty]
        private ObservableCollection<Meta> metas = new();

        public ObservableCollection<ExtratoItem> Extrato { get; } = new();

        public bool TemLancamentos => Extrato?.Any() == true;

        public string HoraInicio => "Iniciada às " + _jornadaService?.JornadaAtual?.Inicio.ToShortTimeString();

        public bool TemMetas => Metas?.Any() == true;

        public JornadaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
            JornadaAtual = _jornadaService.JornadaAtual;

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
                OnPropertyChanged(nameof(TemLancamentos));
            };

            
            Metas.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TemMetas));
            };


            if (!WeakReferenceMessenger.Default.IsRegistered<GanhoLancadoMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<GanhoLancadoMessage>(this, (r,m)=> {
                    AdicionarGanhoAoExtrato(m.Ganho);
                });
            }

            if (!WeakReferenceMessenger.Default.IsRegistered<DespesaLancadaMessage>(this))
            {
                WeakReferenceMessenger.Default.Register<DespesaLancadaMessage>(this, (r, m) => {
                    AdicionarDespesaAoExtrato(m.Despesa);
                });
            }

            #endregion
        }

        partial void OnJornadaAtualChanged(Jornada? value)
        {
            if (value is not null)
            {
                value.PropertyChanged += JornadaAtual_PropertyChanged;
            }

            NotificaUI();
        }

        private void JornadaAtual_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Jornada.Status))
            {
                NotificaUI();
                // ou qualquer outra propriedade dependente
            }
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

        private void AdicionarDespesaAoExtrato(Despesa despesa)
        {
            //O callback do Messenger parecia estar executando fora da thread principal da UI.
            //Então forcei a execução na Thread principal
            MainThread.BeginInvokeOnMainThread(()=>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Extrato.Insert(0, new ExtratoItem
                    {
                        Id = Guid.NewGuid(),
                        LancamentoId = despesa.Id,
                        Tipo = Models.Enums.EnumTipoExtrato.Despesa,
                        Descricao = despesa?.Tipo?.DescricaoTipo,
                        Valor = despesa.Valor,
                        ValorTexto = $"- {despesa.Valor.ToString("C", new CultureInfo("pt-BR"))}",
                        ValorColor = Color.FromArgb("#FF4500"),
                        Data = despesa.CreatedAt
                    });

                    NotificaUI();
                });
            });
        }

        [RelayCommand]
        private async Task AdicionarGanho()
        {

            if (_jornadaService.JornadaAtual.Status != EnumStatusJornada.Ativa)
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
        private void AdicionarGanhoAoExtrato(Ganho ganho)
        {
            //O callback do Messenger parecia estar executando fora da thread principal da UI.
            //Então forcei a execução na Thread principal
            MainThread.BeginInvokeOnMainThread(()=>
            {
                Extrato.Insert(0, new ExtratoItem
                {
                    Id = Guid.NewGuid(),
                    LancamentoId = ganho.Id,
                    Tipo = Models.Enums.EnumTipoExtrato.Ganho,
                    Descricao = ganho.PlataformaNome,
                    Valor = ganho.Valor,
                    ValorTexto = $"+ {ganho.Valor.ToString("C", new CultureInfo("pt-BR"))}",
                    ValorColor = Color.FromArgb("#19C37D"),
                    Data = ganho.CreatedAt
                });

                NotificaUI();
            });
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
                JornadaAtual = null;
                NotificaUI();
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
                JornadaAtual = _jornadaService?.JornadaAtual;
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
            JornadaAtual = _jornadaService.JornadaAtual;

            if (JornadaAtual != null)
            {
                //TotalGanhos = JornadaAtual.TotalGanhos;
                //ESSA LINHA É A CHAVE
                //OnPropertyChanged(nameof(Metas));
                NotificaUI();
                MontarExtrato();
                CarregarMetas();
            }
        }

        private void MontarExtrato()
        {
            try
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

                NotificaUI();
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public void CarregarMetas()
        {
            Metas.Clear();
            foreach (var meta in _jornadaService.JornadaAtual?.Metas.Where(m => m.Selecionada) ?? Enumerable.Empty<Meta>())
            {
                Metas.Add(meta);
            }
        }

        [RelayCommand]
        public async Task Mostrar()
        {
            await Application.Current.MainPage.DisplayAlert("OK", $"{Ativa}", "OK");
        }

        [RelayCommand]
        public async Task DeletarLancamentoExtrato(ExtratoItem item)
        {
            try
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

                if (item.Tipo == EnumTipoExtrato.Ganho)
                {
                    await _jornadaService.RemoverGanhoAsync(item.LancamentoId);
                }

                else if (item.Tipo == EnumTipoExtrato.Despesa)
                {
                    await _jornadaService.RemoverDespesaAsync(item.Id);
                }

                Extrato.Remove(item);
                NotificaUI();
            }
            catch (Exception ex)
            {
                throw ex;
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
            try
            {
                await _jornadaService.RetomarJornadaPausadaAsync();
                await RecuperarAsync();
                NotificaUI();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void NotificaUI()
        {
            OnPropertyChanged(DuracaoFormatada);
            OnPropertyChanged(nameof(Ativa));
            OnPropertyChanged(nameof(Inativa));
            OnPropertyChanged(nameof(Pausada));
            OnPropertyChanged(nameof(TotalGanhos));
            OnPropertyChanged(nameof(TotalDespesas));
            OnPropertyChanged(nameof(Lucro));
            OnPropertyChanged(nameof(HoraInicio));
        }
    }
}
