using ExchangeRate.API;
using ExchangeRate.Commands;
using ExchangeRate.Helpers;
using ExchangeRate.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;
using System.Net.Http;

namespace ExchangeRate.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRequestRateService requestRateService;

        private string startDate;
        private string endDate;

        private bool isCorrectStartDate = false;
        private bool isCorrectEndDate = false;

        private const string startHint = "Начальная дата";
        private const string endHint = "Конечная дата";

        public string StartDate
        {
            get { return startDate; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    startDate = startHint;
                    return;
                }

                if (IsValidDate(value))
                {
                    startDate = DateTime.Parse(value).Date.ToString("yyyy-MM-dd");
                    isCorrectStartDate = true;
                    Properties.Settings.Default.LastStartDatePicked = value;
                }
                else
                {
                    startDate = value;
                    isCorrectStartDate = false;
                }

                if (isCorrectStartDate && isCorrectEndDate)
                {
                    Information = string.Empty;
                }

                OnPropertyChanged("StartDate");
            }
        }

        public string EndDate
        {
            get { return endDate; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    endDate = endHint;
                    return;
                }

                if (IsValidDate(value))
                {
                    endDate = DateTime.Parse(value).Date.ToString("yyyy-MM-dd");
                    isCorrectEndDate = true;
                    Properties.Settings.Default.LastEndDatePicked = value;
                }
                else
                {
                    endDate = value;
                    isCorrectEndDate = false;
                }

                if (isCorrectStartDate && isCorrectEndDate)
                {
                    Information = string.Empty;
                }

                OnPropertyChanged("EndDate");
            }
        }

        private string information;
        public string Information
        {
            get { return information; }
            set
            {
                information = value;
                OnPropertyChanged("Information");
            }
        }

        public ObservableCollection<string> Currencies { get; set; }

        public string SelectedCurrency { get; set; }

        private bool isBusy;

        public bool IsBusy 
        { 
            get => isBusy; 
            set 
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private ChartViewModel chartViewModel;

        public ChartViewModel ChartViewModel
        {
            get => chartViewModel;
            set
            {
                chartViewModel = value;
                OnPropertyChanged("ChartView");
            }
        }

        public UserControl ChartView
        {
            get
            {
                if (chartViewModel == null)
                {
                    return null;
                }

                return chartViewModel.ChartView;
            }
        }

        public int WindowHeight { get; set; }

        public int WindowWidth { get; set; }

        public ApplicationViewModel(IRequestRateService requestRateService)
        {
            this.requestRateService = requestRateService;

            EndDate = Properties.Settings.Default.LastEndDatePicked;
            StartDate = Properties.Settings.Default.LastStartDatePicked;
        }

        private bool IsValidDate(string dateString)
        {
            if (!DateTime.TryParse(dateString, out DateTime parsed))
            {
                if (dateString != startHint && dateString != endHint)
                    Information = $"Некорректный формат даты: {dateString}";

                return false;
            }

            if (parsed < DateTime.Now.AddYears(-5) || parsed > DateTime.Now)
            {
                Information = $"Некорректная дата: {parsed.ToShortDateString()}. Выберите дату в диапазоне последних 5 лет, начиная с : {DateTime.Now.AddYears(-5).ToShortDateString()}!";
                return false;
            }

            if (dateString == startHint || dateString == endHint)
            {
                return false;
            }

            return true;
        }

        #region Commands

        private RequestRatesCommand requestRatesCommand;

        public RequestRatesCommand RequestRateCommand
        {
            get
            {
                return requestRatesCommand ??= new RequestRatesCommand(
                    execute => { RequestRate($"{Properties.Settings.Default["ServerUrl"]}?startDate={StartDate}&endDate={EndDate}&currency={SelectedCurrency}"); },
                    canExecute => isCorrectStartDate && isCorrectEndDate);
            }
        }

        private async void RequestRate(string url)
        {

            IsBusy = true;

            Task<string> resultTask = requestRateService.GetRateAsync(url);

            var result = string.Empty;

            try
            {
                result = await resultTask;
            }
            catch(Exception e)
            {
                Information = e.Message;
                log.Error(e);
            }

            if (resultTask.IsCompletedSuccessfully)
            {
                var items = JsonSerializer.Deserialize<List<RateItem>>(result);

                this.ChartViewModel = new ChartViewModel(items, new CultureInfo("ru-RU"));
            }

            IsBusy = false;
        }

        #endregion

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