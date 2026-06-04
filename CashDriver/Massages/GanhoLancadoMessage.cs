using CashDriver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Massages
{
    public class GanhoLancadoMessage
    {
        public Ganho Ganho { get; }

        public GanhoLancadoMessage(Ganho ganho)
        {
            Ganho = ganho;
        }
    }
}
