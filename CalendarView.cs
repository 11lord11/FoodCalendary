using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace FoodCalendar
{
    public partial class CalendarView :  INotifyPropertyChanged
    {
        public ObservableCollection<DayViewModel> Days { get; private set; }

        public CalendarView()
        {
            Days = new ObservableCollection<DayViewModel>();
            UpdateDays();
            LoadDataFromXml();
        }

        private void OpenCalendar_Click(object sender, RoutedEventArgs e)
        {
            Window calendarWindow = new Window();
            calendarWindow.Title = "Calendar";

            CalendarView calendarView = new CalendarView();
            calendarView.LoadDataFromXml();

            calendarWindow.Content = calendarView;
            calendarWindow.ShowDialog();
        }


        public class DayViewModel : INotifyPropertyChanged
        {
            private int day;
            public int Day
            {
                get { return day; }
                set
                {
                    if (day != value)
                    {
                        day = value;
                        OnPropertyChanged("Day");
                    }
                }
            }

            private string icon;
            public string Icon
            {
                get { return icon; }
                set
                {
                    if (icon != value)
                    {
                        icon = value;
                        OnPropertyChanged("Icon");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime CurrentMonth
        {
            get { return _currentMonth; }
            set
            {
                if (_currentMonth != value)
                {
                    _currentMonth = value;
                    OnPropertyChanged(nameof(CurrentMonth));
                    LoadDataFromXml();
                }
            }
        }

        private DateTime _currentMonth = DateTime.Now;

        public int RowCount
        {
            get { return (Days.Count + 6) / 7; }
        }

        private void UpdateDays()
        {
            Days.Clear();
            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                Days.Add(new DayViewModel { Day = i });
            }
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
            UpdateDays();
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
            UpdateDays();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadDataFromXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("data.xml"); 

            XmlNodeList dayNodes = doc.SelectNodes("/days/day");

            foreach (XmlNode dayNode in dayNodes)
            {
                int day = int.Parse(dayNode.Attributes["number"].Value);
                string icon = dayNode.Attributes["icon"].Value;
                DayViewModel dayViewModel = new DayViewModel { Day = day, Icon = icon };

                DayViewModel existingDay = Days.FirstOrDefault(d => d.Day == day);
                if (existingDay != null)
                {
                    existingDay.Icon = icon;
                }
            }
        }

        private void OnActionSelected(object sender, string icon)
        {
            DayViewModel day = (sender as FrameworkElement).DataContext as DayViewModel;
            day.Icon = icon;

            SaveDataToXml();
        }

        private void SaveDataToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("days");
            doc.AppendChild(root);

            foreach (DayViewModel day in Days)
            {
                int index = Days.IndexOf(day);
                DayViewModel dayViewModel = new DayViewModel { Day = day.Day, Icon = day.Icon };
                XmlElement dayElement = doc.CreateElement("day");
                dayElement.SetAttribute("number", dayViewModel.Day.ToString());
                dayElement.SetAttribute("icon", dayViewModel.Icon);
                root.AppendChild(dayElement);
            }

            doc.Save("data.xml"); // Replace with the correct XML file path
        }

        public  bool Equalss(object obj)
        {
            return obj is CalendarView view &&
                   _contentLoaded == view._contentLoaded &&
                   EqualityComparer<ObservableCollection<DayViewModel>>.Default.Equals(Days, view.Days) &&
                   CurrentMonth == view.CurrentMonth &&
                   _currentMonth == view._currentMonth &&
                   RowCount == view.RowCount;
        }

        public  int GetHashCodee()
        {
            int hashCode = -830164607;
            hashCode = hashCode * -1521134295 + _contentLoaded.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ObservableCollection<DayViewModel>>.Default.GetHashCode(Days);
            hashCode = hashCode * -1521134295 + CurrentMonth.GetHashCode();
            hashCode = hashCode * -1521134295 + _currentMonth.GetHashCode();
            hashCode = hashCode * -1521134295 + RowCount.GetHashCode();
            return hashCode;
        }
    }
}
