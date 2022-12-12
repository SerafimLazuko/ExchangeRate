using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Helpers
{
    public class RateItem
    {
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int Amount { get; set; }
    }
}
