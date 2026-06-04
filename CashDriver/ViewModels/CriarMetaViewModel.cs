using CashDriver.Models;
using CashDriver.Models.Enums;
using CashDriver.Services;
using CashDriver.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace CashDriver.ViewModels
{
    [QueryProperty(nameof(Meta), "Meta")]
    public partial class CriarMetaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;

        [ObservableProperty]
        private Meta meta;
        
        [ObservableProperty]
        private string nome;

        [ObservableProperty]
        private EnumTipoMeta tipo;
        
        [ObservableProperty]
        private decimal valorMeta;

        [ObservableProperty]
        private DateTime dataFim;

        [ObservableProperty]
        private TimeSpan horaFim;


        [ObservableProperty]
        private bool recorrente;

        public bool ExibirDataHora => Tipo == EnumTipoMeta.PorPrazo;

        public CriarMetaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        private async Task Salvar()
        {

            if (string.IsNullOrWhiteSpace(Nome))
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Preencha o campo NOME", "OK");
                return;
            }

            if (ValorMeta <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "O VALOR da meta não pode ser zero", "OK");
                return;
            }

            if (Meta is null)
            {
                Meta novaMeta = new()
                {
                    Nome = Nome,
                    ValorMeta = ValorMeta,
                    Tipo = Tipo,
                    DataFim = Tipo == EnumTipoMeta.PorPrazo ? MontaDataHoraFim(DataFim, HoraFim) : default,
                    Recorrente = Recorrente
                };

                _jornadaService.CadastroDeMetas.Add(novaMeta);
            }

            else
            {
                Meta.Nome = Nome;
                Meta.ValorMeta = ValorMeta;
                Meta.Tipo = Tipo;
                Meta.DataFim = Tipo == EnumTipoMeta.PorPrazo ? MontaDataHoraFim(DataFim, HoraFim) : default;
                Meta.Recorrente = Recorrente;
            }

            await Shell.Current.GoToAsync("//Metas");
        }

        private async Task Cancelar()
        {
            await Shell.Current.GoToAsync("..");
        }

        private DateTime MontaDataHoraFim(DateTime dataFim, TimeSpan horaFim)
        {
            DateTime data = dataFim.Date;
            TimeSpan hora = horaFim;

            return data.Date + hora;
        }

        partial void OnTipoChanged(EnumTipoMeta value)
        {
            OnPropertyChanged(nameof(ExibirDataHora));
        }

        partial void OnMetaChanged(Meta value)
        {
            if (value == null)
                return;

            Nome = value.Nome;
            Tipo = value.Tipo;
            ValorMeta = value.ValorMeta;
            DataFim = value.DataFim;
            HoraFim = value.DataFim.TimeOfDay;
            Recorrente = value.Recorrente;
        }
    }
}
