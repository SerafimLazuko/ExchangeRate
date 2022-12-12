using ExchangeRate.ViewModel;
using System.Windows.Controls;

namespace ExchangeRate.View
{
    public partial class ChartView : UserControl
    {
        public ChartView(ChartViewModel chartViewModel)
        {
            InitializeComponent();
            DataContext = chartViewModel;
        }
    }
}
