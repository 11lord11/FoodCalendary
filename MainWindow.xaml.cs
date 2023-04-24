using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using DocumentFormat.OpenXml.Bibliography;
using FoodCalendar.Properties;

namespace FoodCalendar
{
    public partial class CalendarView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<DayViewModel> Day { get; private set; }
        public CalendarView()
        {
            InitializeComponent();
            DataContext = this;
            Day = new ObservableCollection<DayViewModel>();
            UpdateDays();
            LoadDataFromXml();
        }
        private void OpenCalendar_Click(object sender, RoutedEventArgs e)
        {
            // Create a new window to show the calendar
            Window calendarWindow = new Window();
            calendarWindow.Title = "Calendar";

            // Create a new instance of the CalendarView control
            CalendarView calendarView = new CalendarView();
            calendarView.LoadDataFromXml();

            // Add the CalendarView control to the window
            calendarWindow.Content = calendarView;

            // Show the window
            calendarWindow.ShowDialog();
        }


        private void InitializeComponent()
        {
            throw new NotImplementedException();
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
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    LoadDataFromXml(); // reload data when month changes
                }
            }
    }
        private DateTime _currentMonth = DateTime.Now;

        public int RowCount
        {
            get { return (Day.Count + 6) / 7; }
        }

        



        private void UpdateDays()
        {
            Day.Clear();
            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                Day.Add(new DayViewModel { Day = i });
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
            // load data from xml file
            XmlDocument doc = new XmlDocument();
            doc.Load("data.xml"); // replace with your xml file name/path
            XmlNodeList dayNodes = doc.SelectNodes("/days/day");

            // update view models with data from xml
            foreach (XmlNode dayNode in dayNodes)
            {
                int day = int.Parse(dayNode.Attributes["number"].Value);
                string icon = dayNode.Attributes["icon"].Value;
                DayViewModel dayViewModel = new DayViewModel { Day = day, Icon = icon };
                // find the day view model with the corresponding day and update it
                DayViewModel existingDay = Day.FirstOrDefault(d => d.Day == day);
                if (existingDay != null)
                {
                    existingDay.Icon = icon;
                }
            }
        }

        private void OnActionSelected(object sender, string icon)
        {
            // Get the selected day
            DayViewModel day = (sender as FrameworkElement).DataContext as DayViewModel;

            // Set the icon property
            day.Icon = icon;

            // ... save the selected actions to a JSON or XML file
            SaveDataToXml();
        }

        private void SaveDataToXml()
        {
            // create xml document and root element
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("days");
            doc.AppendChild(root);

            // create element for each day with its icon attribute
            foreach (DayViewModel day in Day)
            {
                int index = Day.IndexOf(day);
                DayViewModel dayViewModel = new DayViewModel { Day = day.Day, Icon = day.Icon };
                XmlElement dayElement = doc.CreateElement("day");
                dayElement.SetAttribute("number", dayViewModel.Day.ToString());
                dayElement.SetAttribute("icon", dayViewModel.Icon);
                root.AppendChild(dayElement);
            }


            // save xml to file
            doc.Save("data.xml");
        }
    }
}

            // create xml document and root















