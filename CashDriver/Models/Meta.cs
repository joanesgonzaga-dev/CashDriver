using CashDriver.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Models
{
    public partial class Meta : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; }
        public decimal ValorAtual { get; set; }
        public decimal ValorMeta { get; set; } 
        public EnumTipoMeta Tipo { get; set; }
        public DateTime DataFim { get; set; }
        public double Progresso => ValorAtual == 0 ? 0 : (double)(ValorAtual / ValorMeta);
        [ObservableProperty]
        public bool recorrente;
        [ObservableProperty]
        private bool selecionada;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EnumSyncStatus SyncStatus { get; set; }
    }
}
