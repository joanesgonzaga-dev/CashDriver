using CashDriver.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Models
{
    public class ExtratoItem
    {
        public Guid Id{ get; set; }
        public Guid LancamentoId { get; set; }
        public EnumTipoExtrato Tipo { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public string? ValorTexto { get; set; }
        public Microsoft.Maui.Graphics.Color? ValorColor { get; set; }
        public DateTime Data { get; set; }
    }
}
