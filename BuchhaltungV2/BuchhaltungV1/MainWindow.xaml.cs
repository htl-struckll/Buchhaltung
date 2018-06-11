using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
           InitializeComponent();
           LookForWeeks();
           Topmost = true;
        }

        //Fills the weekComboBox
        private void LookForWeeks()
        {
            for (int i = 0; i < 53; i++)
            {
                if(File.Exists(@"Data\" + i + @".week"))
                {
                    if (!ComboOfWeeks.Items.Contains(i))
                        ComboOfWeeks.Items.Add(i + "te Woche");
                }
            }
        }

        //Calls the function to start the prorgramm
        private void StartProgramm_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = true;
            StartProgramm();
        }

        //Starts the programm with the data needed
        private void StartProgramm()
        {
            try
            {
                if (Convert.ToInt32(WeekNrTextBox.Text) > 0 && Convert.ToInt32(WeekNrTextBox.Text) < 56) //Week in year
                {
                    if (File.Exists(@"Data\" + WeekNrTextBox.Text + @".week")) //Week does exist
                    {
                        Buchhaltung b = new Buchhaltung(WeekNrTextBox.Text);
                        b.Show();
                    }
                    else
                    {
                        string oldCashDeskStr = OldCashDesk.Text;
                        if (oldCashDeskStr.Contains("€"))
                            oldCashDeskStr = oldCashDeskStr.Replace('€', ' ');
                        Buchhaltung b = new Buchhaltung(WeekNrTextBox.Text, NameInput.Text, Convert.ToDateTime(DateTextBox.Text), Convert.ToDouble(oldCashDeskStr));
                        MessageBox.Show("Die Woche wurde nicht gefunden! Es wird eine neue erstellt.");
                        b.Show();
                    }
                    Close();
                }
                else
                    Buchhaltung.Log("Ungültige Woche");
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        private void CheckEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartProgramm();
        }

        private void UpdateWeekNrTxtBox(object sender, SelectionChangedEventArgs e)
        {
           string fullWeek =  (string)ComboOfWeeks.SelectedValue;
            string[] splittedWeekNr = fullWeek.Split('t');
            WeekNrTextBox.Text = splittedWeekNr[0];
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = false;
            Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = false;
            Close();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //Gets the current MondayDate  todo automatic date does not work
        private void CurrDate_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now;

            if (Convert.ToString(today.DayOfWeek) == "Monday")
                DateTextBox.Text = today.ToShortDateString();
            else if (Convert.ToString(today.DayOfWeek) == "Tuesday")
            {
                if(today.Day-1 <10 && today.Month < 10)
                    DateTextBox.Text = "0"+(today.Day - 1).ToString() + ".0" + today.Month + "." + today.Year;
                else if(today.Day-1 < 10)
                    DateTextBox.Text = "0" + (today.Day - 1).ToString() + "." + today.Month + "." + today.Year;
                else if(today.Month < 10)
                    DateTextBox.Text =(today.Day - 1).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 1).ToString() + "." + today.Month + "." + today.Year;
            }
            else if (Convert.ToString(today.DayOfWeek) == "Wednesday")
            {
                if (today.Day-2 < 10 && today.Month < 10)
                    DateTextBox.Text = "0" + (today.Day - 2).ToString() + ".0" + today.Month + "." + today.Year;
                else if (today.Day-2 < 10)
                    DateTextBox.Text = "0" + (today.Day - 2).ToString() + "." + today.Month + "." + today.Year;
                else if (today.Month < 10)
                    DateTextBox.Text = (today.Day - 2).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 2).ToString() + "." + today.Month + "." + today.Year;
            }
            else if (Convert.ToString(today.DayOfWeek) == "Thursday")
            {
                if (today.Day-3 < 10 && today.Month < 10)
                    DateTextBox.Text = "0" + (today.Day - 3).ToString() + ".0" + today.Month + "." + today.Year;
                else if (today.Day-3 < 10)
                    DateTextBox.Text = "0" + (today.Day - 3).ToString() + "." + today.Month + "." + today.Year;
                else if (today.Month < 10)
                    DateTextBox.Text = (today.Day - 3).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 3).ToString() + "." + today.Month + "." + today.Year;
            }
            else if (Convert.ToString(today.DayOfWeek) == "Friday")
            {
                if (today.AddDays(-4).Day < 10 && today.Month < 10)
                    DateTextBox.Text = "0" + (today.Day - 4).ToString() + ".0" + today.Month + "." + today.Year;
                else if (today.Day-4 < 10)
                    DateTextBox.Text = "0" + (today.Day - 4).ToString() + "." + today.Month + "." + today.Year;
                else if (today.Month < 10)
                    DateTextBox.Text = (today.Day - 4).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 4).ToString() + "." + today.Month + "." + today.Year;
            }
            else if (Convert.ToString(today.DayOfWeek) == "Saturday")
            {
                if (today.Day-5 < 10 && today.Month < 10)
                    DateTextBox.Text = "0" + (today.Day - 5).ToString() + ".0" + today.Month + "." + today.Year;
                else if (today.Day-5 < 10)
                    DateTextBox.Text = "0" + (today.Day - 5).ToString() + "." + today.Month + "." + today.Year;
                else if (today.Month < 10)
                    DateTextBox.Text = (today.Day - 5).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 5).ToString() + "." + today.Month + "." + today.Year;
            }
            else if (Convert.ToString(today.DayOfWeek) == "Sunday")
            {
                if (today.Day-6 < 10 && today.Month < 10)
                    DateTextBox.Text = "0" + (today.Day - 6).ToString() + ".0" + today.Month + "." + today.Year;
                else if (today.Day-6 < 10)
                    DateTextBox.Text = "0" + (today.Day - 6).ToString() + "." + today.Month + "." + today.Year;
                else if (today.Month < 10)
                    DateTextBox.Text = (today.Day - 6).ToString() + ".0" + today.Month + "." + today.Year;
                else
                    DateTextBox.Text = (today.Day - 6).ToString() + "." + today.Month + "." + today.Year;
            }

        }

        /// <summary>
        /// Looks if File Exists When not it expands additional info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckWeek_txtChanged(object sender, TextChangedEventArgs e)
        {
            NewWeekExpander.IsExpanded = !File.Exists(@"Data\" + WeekNrTextBox.Text + @".week") && WeekNrTextBox.Text != "";
        }
    }
}
