using CashDriver.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace CashDriver.Models
{
    public partial class Jornada : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Inicio { get; set; } = DateTime.Now;
        public DateTime? InicioPeriodoAtual { get; set; } = null;
        public DateTime? Termino { get; set; }
        public TimeSpan? TempoAcumulado { get; set; } = TimeSpan.Zero;
        public decimal TotalGanhos => Ganhos?.Sum(g => g.Valor) ?? 0;
        public decimal TotalDespesas => Despesas?.Sum(d => d.Valor) ?? 0;
        public decimal Lucro => TotalGanhos - TotalDespesas;
        
        [ObservableProperty]
        public EnumStatusJornada status = EnumStatusJornada.Inativa;
        public ObservableCollection<Meta> Metas { get; set; } = new();
        public ObservableCollection<Despesa> Despesas { get; set; } = new();
        public ObservableCollection<Ganho> Ganhos { get; set; } = new();

        //public ObservableCollection<Plataforma> Plataformas { get; set; } = new();

        #region NotMapped Properties and Fields

        [NotMapped]
        public string DiaSemanaAbreviado => Inicio.ToString("ddd", new CultureInfo("pt-BR")).Replace(".", "").ToUpper();

        [NotMapped]
        public string DiaDoMesEmNumero => Inicio.ToString("dd", new CultureInfo("pt-BR"));

        [NotMapped]
        public string NomeMesAbreviado => Inicio.ToString("MMM", new CultureInfo("pt-BR")).Replace(".", "").ToUpper();

        [NotMapped]
        public string Intervalo => Inicio.ToString(@"HH\:mm - ") + (Termino == null ? "" : Termino?.ToString(@"HH\:mm"));

        [NotMapped]
        public Color StatusLabelColor =>
            Status switch
            {
                EnumStatusJornada.Ativa => Color.FromArgb("#19C37D"),
                EnumStatusJornada.Pausa => Color.FromArgb("#FFC107"),
                EnumStatusJornada.Encerrada => Color.FromArgb("#5985E1"), 
                _ => Colors.Gray
            };

        [NotMapped]
        public Color StatusBackgroundColor =>
            Status switch
            {
                EnumStatusJornada.Ativa => Color.FromArgb("#DDF5EA"),
                EnumStatusJornada.Pausa => Color.FromArgb("#FFF3CD"),
                EnumStatusJornada.Encerrada => Color.FromArgb("#D9ECFF"),
                _ => Colors.LightGray
            };

        #endregion


        //Para exibição. Calculo da duração atual da Jornada Ativa, considerando 
        //as pausas da Jornada
        [NotMapped]
        public TimeSpan? DuracaoTotalJornada {
            get
            {
                //InicioDoPeriodoAtual será nulo, caso JornadaAtual não tenha sido iniciada
                if (InicioPeriodoAtual == null)
                {
                    return TempoAcumulado; // TimeSpan.Zero
                }

                return TempoAcumulado + (DateTime.Now - InicioPeriodoAtual.Value);
            }
        } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EnumSyncStatus SyncStatus { get; set; }

    }
}
