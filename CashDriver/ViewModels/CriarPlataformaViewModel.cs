using CashDriver.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.ViewModels
{
    public partial class CriarPlataformaViewModel : ObservableObject
    {
        private readonly JornadaService _jornadaService;

        [ObservableProperty]
        private string? nome;

        public CriarPlataformaViewModel(JornadaService jornadaService)
        {
            _jornadaService = jornadaService;
        }

        [RelayCommand]
        public async Task Salvar()
        {
            _jornadaService.CadastroDePlataformas.Add(new Models.Plataforma()
            {
                Name = Nome,
            });
        }
    }
}
