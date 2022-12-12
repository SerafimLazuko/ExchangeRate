using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExchangeRate.Helpers
{
    public class ChartItem 
    {
        public IFormatProvider formatProvider;

        public DateTime Date { get; set; }

        public string DateString => this.Date.ToString("MMM dd (yyyy)", formatProvider);

        public double Value { get; set; }

        public string ValueString => this.Value.ToString("C", formatProvider);
    }
}
