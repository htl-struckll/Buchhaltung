using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;

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
            AnimateAndCheck();
            Topmost = true;
        }

        /// <summary>
        /// Fills the comboBox
        /// </summary>
        private void LookForWeeks()
        {
            for (int i = 0; i < 53; i++)
            {
                if (File.Exists(@"Data\" + i + @".week"))
                {
                    if (!ComboOfWeeks.Items.Contains(i))
                        ComboOfWeeks.Items.Add(i + ". Woche");
                }
            }
        }

        /// <summary>
        /// Calls the function to start the prorgramm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartProgramm_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = true; //Only important when called by ChangeWeek from Buchhaltungs
            StartProgramm();
        }

        /// <summary>
        /// Starts the programm with the data needed
        /// </summary>
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
                        Buchhaltung b = new Buchhaltung(WeekNrTextBox.Text, NameInput.Text,
                            Convert.ToDateTime(DateTextBox.Text), Convert.ToDouble(oldCashDeskStr));
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

        /// <summary>
        /// Starts Programm on Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartProgramm();
        }

        /// <summary>
        /// Updates week nr in textBox from selected DropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateWeekNrTxtBox(object sender, SelectionChangedEventArgs e)
        {
            string fullWeek = (string) ComboOfWeeks.SelectedValue;
            string[] splittedWeekNr = fullWeek.Split('.');
            WeekNrTextBox.Text = splittedWeekNr[0];
        }

        /// <summary>
        /// Closes Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = false;
            Close();
        }

        /// <summary>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        /// <summary>
        /// makes the window moveable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Calls AnimateAndCheck function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckWeek_txtChanged(object sender, TextChangedEventArgs e) => AnimateAndCheck();

        /// <summary>
        /// Looks if File Exists When not it expands additional info
        /// </summary>
        private void AnimateAndCheck()
        {
            if (File.Exists(@"Data\" + WeekNrTextBox.Text + @".week") && WeekNrTextBox.Text != "" ||
                WeekNrTextBox.Text == "")
            {
                Expander ex = NewWeekExpander;
                DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));

                ex.BeginAnimation(OpacityProperty, animation);
            }
            else
            {
                Expander ex = NewWeekExpander;
                DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));

                NewWeekExpander.IsExpanded = true;

                ex.BeginAnimation(OpacityProperty, animation);
            }
        }

        /// <summary>
        /// Expands week
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandWeek(object sender, MouseEventArgs e) => DatePicker.IsDropDownOpen = true;

        /// <summary>
        /// Updates Date with the selected Date from the Picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDate(object sender, SelectionChangedEventArgs e) => DateTextBox.Text =
            Convert.ToString(Convert.ToDateTime(DatePicker.SelectedDate).ToShortDateString());

        /// <summary>
        /// Checks if input is a number with RegEx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewWeekNr(object sender, TextCompositionEventArgs e) =>
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
}