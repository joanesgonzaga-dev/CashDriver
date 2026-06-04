using CashDriver.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Models
{
    public partial class Despesa : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? JornadaId { get; set; }
        public Jornada? Jornada { get; set; }
        public int TipoDespesaId { get; set; }
        public TipoDespesa? Tipo { get; set; }
        public string? Detalhes { get; set; } = string.Empty;
        public decimal Valor { get; set; } = 0.0M;

        //[ObservableProperty]
        //public bool selecionada;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public EnumSyncStatus SyncStatus { get; set; }

    }
}
