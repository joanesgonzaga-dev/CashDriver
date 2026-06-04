using CashDriver.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Models
{
    public partial class TipoDespesa : ObservableObject
    {
        public int Id { get; set; }
        public string? DescricaoTipo { get; set; }
        public string? NomeIcone { get; set; }
        public EnumTipoDespesa Tipo { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; private set; }
        public EnumSyncStatus SyncStatus { get; set; }
    }
}
