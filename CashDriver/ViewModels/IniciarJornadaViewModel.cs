using CashDriver.Models;
using CashDriver.Services;
using CashDriver.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class IniciarJornadaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
        public ObservableCollection<Meta> Metas => _jornadaService.CadastroDeMetas;

        public IniciarJornadaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        private async Task IniciarJornada()
        {
            if (_jornadaService.JornadaAtual != null && _jornadaService.JornadaAtual.Status == Models.Enums.EnumStatusJornada.Ativa)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Jornada",
                    $"Já existe uma Jornada iniciada!",
                    "OK"
                    );
            }

            else
            {
                _jornadaService.CriarJornadaAsync();
            }

            //navegar
            await Shell.Current.GoToAsync("//Jornada"); // //Page só funciona se os ShellContent em AppShell.xaml tiverem o atributo Route="SuaPage"
        }


       
        [RelayCommand]
        private async Task CriarMetaRapido()
        {
            await Shell.Current.GoToAsync(nameof(CriarMetaPage));
        }
    }
}
