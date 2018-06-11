using System;
using System.Globalization;
using System.Windows;

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

        private void actDate_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now;

            switch (Convert.ToString(today.DayOfWeek))
            {
                case "Monday":
                    DateTextBox.Text = today.ToShortDateString();
                    break;
                case "Tuesday":
                    if (today.Day - 1 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 1).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 1 < 10)
                        DateTextBox.Text = "0" + (today.Day - 1).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 1).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 1).ToString() + "." + today.Month + "." + today.Year;
                    break;
                case "Wednesday":
                    if (today.Day - 2 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 2).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 2 < 10)
                        DateTextBox.Text = "0" + (today.Day - 2).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 2).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 2).ToString() + "." + today.Month + "." + today.Year;
                    break;
                case "Thursday":
                    if (today.Day - 3 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 3).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 3 < 10)
                        DateTextBox.Text = "0" + (today.Day - 3).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 3).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 3).ToString() + "." + today.Month + "." + today.Year;
                    break;
                case "Friday":
                    if (today.Day - 4 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 4).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 4 < 10)
                        DateTextBox.Text = "0" + (today.Day - 4).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 4).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 4).ToString() + "." + today.Month + "." + today.Year;
                    break;
                case "Saturday":
                    if (today.Day - 5 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 5).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 5 < 10)
                        DateTextBox.Text = "0" + (today.Day - 5).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 5).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 5).ToString() + "." + today.Month + "." + today.Year;
                    break;
                case "Sunday":
                    if (today.Day - 6 < 10 && today.Month < 10)
                        DateTextBox.Text = "0" + (today.Day - 6).ToString() + ".0" + today.Month + "." + today.Year;
                    else if (today.Day - 6 < 10)
                        DateTextBox.Text = "0" + (today.Day - 6).ToString() + "." + today.Month + "." + today.Year;
                    else if (today.Month < 10)
                        DateTextBox.Text = (today.Day - 6).ToString() + ".0" + today.Month + "." + today.Year;
                    else
                        DateTextBox.Text = (today.Day - 6).ToString() + "." + today.Month + "." + today.Year;
                    break;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e) => Close();
 
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "" && (WeekNrTextBox.Text != "" && Convert.ToInt32(WeekNrTextBox.Text) > 0 && Convert.ToInt32(WeekNrTextBox.Text) < 53)&& DateTextBox.Text != "" &&
                OldCashTextBox.Text != "" )
            {
                UpdateWeek();
                Close();
            }
            else
                Buchhaltung.Log("Fehlerhafte Eingabe!");
        }

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

        private void FillFields()
        {
            NameTextBox.Text = Buchhaltung.CurrWeek.EditName;
            WeekNrTextBox.Text = Buchhaltung.CurrWeek.WeekNr;
            DateTextBox.Text = Buchhaltung.CurrWeek.ExactTime.ToShortDateString();
            OldCashTextBox.Text = Convert.ToString(Buchhaltung.CurrWeek.LastChashDesk, CultureInfo.InvariantCulture);
        }
    }
}
