using CashDriver.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Models
{
    public partial class Ganho : ObservableObject
    {
        public Guid Id { get; set; }
        public Guid PlataformaId { get; set; }
        public Guid JornadaId { get; set; }
        public Jornada? Jornada { get; set; }
        public string? PlataformaNome { get; set; }
        public string? Descricao { get; set; }
        [ObservableProperty]
        private decimal valor;
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EnumSyncStatus SyncStatus { get; set; }
    }
}
