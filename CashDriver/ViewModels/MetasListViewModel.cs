using CashDriver.Models;
using CashDriver.Services;
using CashDriver.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashDriver.ViewModels
{
    public partial class MetasListViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;
        public ObservableCollection<Meta> Metas => _jornadaService.CadastroDeMetas;
        public MetasListViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        private async Task CriarMetaRapido()
        {
            await Shell.Current.GoToAsync(nameof(CriarMetaPage));
        }

        [RelayCommand]
        private async Task EditarMeta(Meta meta)
        {
            var id = meta.Id;
            await Shell.Current.GoToAsync(nameof(CriarMetaPage), new Dictionary<string, object>
            {
                ["Meta"] = meta
            });
        }


        [RelayCommand]
        private async Task DeletarMeta(Meta meta)
        {
            if (_jornadaService.CadastroDeMetas.Contains(meta))
            {
                bool b = await Application.Current.MainPage.DisplayAlert("Excluir", $"Confirma a exclusão da meta {meta.Nome} ?", "SIM", "NÃO");

                if (b)
                {
                     b =_jornadaService.CadastroDeMetas.Remove(meta);

                    if (b)
                    {
                        await Application.Current.MainPage.DisplayAlert("Excluir", $"Meta excluída com sucesso!", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Excluir", $"Erros impediram a Meta de ser excluída!", "OK");
                    }
                }
            }
        }
    }
}
