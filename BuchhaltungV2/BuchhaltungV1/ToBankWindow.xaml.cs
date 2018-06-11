using System;
using System.Windows;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for ToBankWindow.xaml
    /// </summary>
    public partial class ToBankWindow
    {
        public ToBankWindow()
        {
            InitializeComponent();
            Topmost = true;
        }

        //Updates toBank value currDay
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ToBankInput.Text != "")
                {
                    string toBank = ToBankInput.Text;

                    if (toBank.Contains("€"))
                        toBank = toBank.Replace("€","");

                    Buchhaltung.CurrWeek.GetCurrentDay().AddBank(Convert.ToDouble(toBank));

                    Close();
                }
                else
                {
                    Buchhaltung.Log("Felder sind leer!");
                }
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        //closes the Programm
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
