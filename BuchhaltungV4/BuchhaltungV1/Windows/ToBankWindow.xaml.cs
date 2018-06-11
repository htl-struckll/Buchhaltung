using System;
using System.Windows;
using System.Windows.Input;

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

        /// <summary>
        /// Calls save function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, RoutedEventArgs e) => Save();



        /// <summary>
        /// Checks input and adds to the Bank in the current day
        /// </summary>
        private void Save()
        {
            try
            {
                if (ToBankInput.Text != "")
                {
                    string toBank = ToBankInput.Text;

                    if (toBank.Contains("€"))
                        toBank = toBank.Replace("€", "");

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

        /// <summary>
        /// closes the Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelBtn_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Checks if Key is enter and handels it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChecksEnter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Save();
        }
    }
}