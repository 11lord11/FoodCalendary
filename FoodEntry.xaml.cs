using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using static FoodCalendar.CalendarView;


namespace FoodCalendar
{
    public partial class FoodEntryWindow : Window
    {
        [XmlRoot("Day")]
        public class DayViewModel
        {
            public DayViewModel()
            {
                // Конструктор без параметров для XmlSerializer
            }

            public DayViewModel(DateTime day)
            {
                Day = day;
                FoodIcon = "/Icons/food.png";
                WaterIcon = "/Icons/water.png";
                
                SelectedActionIcon = "/Icons/exercise.png";
            }

            [XmlElement("Day")]
            public DateTime Day { get; set; }

            [XmlElement("FoodIcon")]
            public string FoodIcon { get; set; }

            [XmlElement("WaterIcon")]
            public string WaterIcon { get; set; }

            [XmlElement("SelectedAction")]
            public ActionType SelectedAction { get; set; }

            [XmlElement("SelectedActionIcon")]
            public string SelectedActionIcon { get; set; }
        }

        private readonly DayViewModel day;


        public FoodEntryWindow(DayViewModel day)
        {
            InitializeComponent();
            this.day = day;
           
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (eatRadioButton.IsChecked == true)
            {
                day.SelectedAction = ActionType.Eat;
            }
            else if (drinkRadioButton.IsChecked == true)
            {
                day.SelectedAction = ActionType.Drink;
            }
            else if (exerciseRadioButton.IsChecked == true)
            {
                day.SelectedAction = ActionType.Exercise;
            }
            else
            {
                // Handle the case where none of the radio buttons are checked.
                MessageBox.Show("Пожалуйста, выберите действие.");
                return;
            }
            this.DialogResult = true;
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public enum ActionType
        {
           
            Eat,
            Drink,
            Exercise
        }

        public class UserActions
        {
            [XmlElement("Date")]
            public string Date { get; set; }

            [XmlArray("Actions")]
            [XmlArrayItem("Action")]
            public List<string> Actions { get; set; }
        }

        // Method to save user actions to XML file
        public void SaveActionsToXml(string filePath, string date, List<string> actions)
        {
            UserActions userActions = new UserActions
            {
                Date = date,
                Actions = actions
            };

            XmlSerializer serializer = new XmlSerializer(typeof(UserActions));
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, userActions);
            }
        }

        // Method to save selected actions for the current day to XML file
        private void SaveSelectedActions()
        {
            string filePath = @"path\to\file.xml"; // Replace with actual file path
            List<DayViewModel> daysData = new List<DayViewModel>();
            if (File.Exists(filePath))
            {
                // Deserialize the XML file into a List<DayViewModel> object.
                XmlSerializer serializer = new XmlSerializer(typeof(List<DayViewModel>));
                using (StreamReader streamReader = new StreamReader(filePath))

                
                {
                    List<DayViewModel> days = (List<DayViewModel>)serializer.Deserialize(streamReader);
                    foreach (DayViewModel day in days)
                    {
                        Console.WriteLine("Day {0}", day.Day);
                        Console.WriteLine("Tasks:");
                       
                    }

                }
            }
        }

    }

}




