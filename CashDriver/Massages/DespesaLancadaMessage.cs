using CashDriver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Massages
{
    public class DespesaLancadaMessage
    {
        public Despesa Despesa { get;}

        public DespesaLancadaMessage(Despesa despesa)
        {
            Despesa = despesa;
        }
    }
}
