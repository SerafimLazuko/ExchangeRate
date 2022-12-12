using ExchangeRate.Helpers;
using ExchangeRate.Model;
using ExchangeRate.View;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace ExchangeRate.ViewModel
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        private readonly string currency;

        public ChartView ChartView { get; set; }

        public ChartModel ChartModel { get; set; }

        public ChartViewModel(List<RateItem> rateItems, IFormatProvider formatProvider)
        {
            currency = rateItems.First().Currency;
            FormatProvider = formatProvider;
            ChartModel = new ChartModel(CreateChartItemsCollection(rateItems));
            ChartView = new ChartView(this);
            BuildChart();
        }

        public SeriesCollection SeriesCollection { get; private set; }

        public Func<double, string> XLabelFormatter => value => value >= 0 && (int)value < this.ChartModel.ChartItems.Count() ? this.ChartModel.ChartItems.ElementAt((int)value).DateString : string.Empty;

        public Func<double, string> YLabelFormatter => value =>
        {
            if(currency == "BTC")
                return value > 0 ? $"{value:N3} USD" : string.Empty;
            else
                return value > 0 ? $"{value:N3} BYN" : string.Empty;
        };

        public IFormatProvider FormatProvider { get; }

        private ChartValues<ChartItem>  CreateChartItemsCollection(List<RateItem> rateItems)
        {
            ChartValues<ChartItem> itemsCollection = new ChartValues<ChartItem>();

            foreach (var item in rateItems)
            {
                ChartItem newItem = new ChartItem();

                newItem.formatProvider = FormatProvider;
                newItem.Date = item.Date;
                newItem.Value = item.Value;

                if (!itemsCollection.Contains(newItem))
                    itemsCollection.Add(newItem);
            }

            return itemsCollection;
        }

        private void BuildChart()
        {
            var items = this.ChartModel.ChartItems;
            var maxRateIndex = items.IndexOf(items.MaxBy(i => i.Value));

            var fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f8f8d9")); 
            var strokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a35f1b"));

            this.SeriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Values = this.ChartModel.ChartItems,
                    Configuration = new CartesianMapper<ChartItem>()
                    .X(point => this.ChartModel.ChartItems.IndexOf(point))
                    .Y(point => point.Value)
                    .Fill(point => this.ChartModel.ChartItems.IndexOf(point) == maxRateIndex ? Brushes.IndianRed : fillColor)
                    .Stroke(point => this.ChartModel.ChartItems.IndexOf(point) == maxRateIndex ? Brushes.IndianRed : strokeColor),

                    Fill = fillColor,
                    Stroke = strokeColor,
                    Title = string.Empty,
                    PointGeometrySize = 7,
                },
            };

            OnPropertyChanged("SeriesCollection");
        }

        #region OnPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
