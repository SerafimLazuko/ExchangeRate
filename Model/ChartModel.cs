using ExchangeRate.Helpers;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Model
{
    public class ChartModel : INotifyPropertyChanged
    {
        public ChartValues<ChartItem> ChartItems { get; set; }

        public ChartModel(ChartValues<ChartItem> chartItems)
        {
            ChartItems = new ChartValues<ChartItem>(chartItems);
            OnPropertyChanged("ChartItems");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
