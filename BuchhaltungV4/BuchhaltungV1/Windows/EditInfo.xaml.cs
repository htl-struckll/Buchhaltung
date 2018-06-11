using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for EditInfo.xaml
    /// </summary>
    public partial class EditInfo
    {
        public EditInfo()
        {
            InitializeComponent();
            Topmost = true;
            FillFields();
        }

        /// <summary>
        /// Gets the Date from the DatePicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActDate_Click(object sender, RoutedEventArgs e) => DateTextBox.Text =
            Convert.ToString(Convert.ToDateTime(DatePicker.SelectedDate).ToShortDateString());

        /// <summary>
        /// Closes the Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Checks if everything is ok and saves it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "" &&
                (WeekNrTextBox.Text != "" && Convert.ToInt32(WeekNrTextBox.Text) > 0 &&
                 Convert.ToInt32(WeekNrTextBox.Text) < 53) && DateTextBox.Text != "" &&
                OldCashTextBox.Text != "")
            {
                UpdateWeek();
                Close();
            }
            else
                Buchhaltung.Log("Fehlerhafte Eingabe!");
        }

        /// <summary>
        /// Update Data
        /// </summary>
        private void UpdateWeek()
        {
            try
            {
                string oldCashDeskStr = (OldCashTextBox.Text);

                if (oldCashDeskStr.Contains("€"))
                    oldCashDeskStr = oldCashDeskStr.Replace("€", "");

                Buchhaltung.CurrWeek.WeekNr = WeekNrTextBox.Text;
                Buchhaltung.CurrWeek.EditName = NameTextBox.Text;
                Buchhaltung.CurrWeek.LastChashDesk = Convert.ToDouble(oldCashDeskStr);
                Buchhaltung.CurrWeek.ExactTime = Convert.ToDateTime(DateTextBox.Text);
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        /// <summary>
        /// Fills Fields with current Info
        /// </summary>
        private void FillFields()
        {
            NameTextBox.Text = Buchhaltung.CurrWeek.EditName;
            WeekNrTextBox.Text = Buchhaltung.CurrWeek.WeekNr;
            DateTextBox.Text = Buchhaltung.CurrWeek.ExactTime.ToShortDateString();
            OldCashTextBox.Text = Convert.ToString(Buchhaltung.CurrWeek.LastChashDesk, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Expands DatePicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandWeek(object sender, MouseEventArgs e) =>
            DatePicker.IsDropDownOpen = true;

        /// <summary>
        /// Checks if input is a number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewWeekNr(object sender, TextCompositionEventArgs e) =>
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
}