using System;
using System.Windows;
using System.Windows.Input;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for NewEntry.xaml
    /// </summary>
    public partial class NewEntry
    {
        private double _priceInUse;

        private bool _idIsOk = false; //needed for checking if input is ok
        private bool _amountIsOk = false;
        private bool _onTheHouseIsOk = true;

        public NewEntry()
        {
            InitializeComponent();
        }

        #region Events

        /// <summary>
        /// Closes the programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// When the input is ok it closes it and saves everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e) => SaveAndEnd();


        /// <summary>
        /// Checks if id is ok 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextInput_ID(object sender, KeyEventArgs e)
        {
            string input = InputId.Text;

            try
            {
                int dummy = Convert.ToInt32(input);

                _idIsOk = SearchForProduct();
            }
            catch (Exception ex)
            {
                _idIsOk = false;
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        /// <summary>
        /// Checks if amount is ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputAmount_keyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string t = InputAmount.Text;
                double dummy = Convert.ToDouble(t);

                _amountIsOk = true;

                CalcPrice();
            }
            catch (Exception ex)
            {
                _amountIsOk = false;
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        /// <summary>
        /// Checks if OnTheHouse is ok 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputOnTheHouse_keyDown(object sender, KeyEventArgs e)
        {
            try
            {
                double dummy;

                string t = InputAmountOnTheHouse.Text;
                if (t.Length > 0)
                    dummy = Convert.ToDouble(t);

                _onTheHouseIsOk = true;

            }
            catch (Exception ex)
            {
                _onTheHouseIsOk = false;
                Buchhaltung.SaveErrorMsg(ex);
            }
        }


        /// <summary>
        /// Checks if input is enter and handels it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveAndEnd();
        }

        #endregion

        /// <summary>
        /// Saves Entry
        /// </summary>
        private void SaveEntry()
        {
            try
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (p.Id != Convert.ToInt32(InputId.Text)) continue;
                    Entry e;
                    int actualAmount = 0;
                    if (InputAmountOnTheHouse.Text == "")
                    {
                        actualAmount = Convert.ToInt32(InputAmount.Text);
                        e = new Entry(p, actualAmount, 0, _priceInUse);
                    }
                    else
                    {
                        actualAmount =
                            Convert.ToInt32(Convert.ToInt32(InputAmount.Text) +
                                            Convert.ToInt32(InputAmountOnTheHouse.Text));
                        e = new Entry(p, actualAmount, Convert.ToInt32(InputAmountOnTheHouse.Text), _priceInUse);
                    }

                    //Adds at the currentWeek and the current Day the entry
                    Buchhaltung.CurrWeek.GetCurrentDayAndAddEntry(e);
                    break;
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
            finally
            {
                Buchhaltung.SaveEntrys();
            }
        }

        /// <summary>
        /// Calculates the Price
        /// </summary>
        private void CalcPrice()
        {
            if (InputId.Text.Length <= 0 || InputAmount.Text.Length <= 0) return;

            PriceOutput.Text = "Preis: ";
            try
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (Convert.ToInt32(InputId.Text) == p.Id)
                    {
                        _priceInUse = p.Price * Convert.ToInt32(InputAmount.Text);
                        PriceOutput.Text += "   " + (_priceInUse) + "€";
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        /// <summary>
        /// Searches the product with the given id
        /// </summary>
        /// <returns>true if prouduct was found</returns>
        private bool SearchForProduct()
        {
            if (InputId.Text.Length <= 0) return false;

            ProductOutput.Text = "Produkt:";
            try
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (p.Id == Convert.ToInt32(InputId.Text))
                    {
                        ProductOutput.Text += "       " + p.Name;
                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
                return false;
            }
        }


        private void SaveAndEnd()
        {
            if (_idIsOk && _amountIsOk && _onTheHouseIsOk)
            {
                SaveEntry();
                Close();
            }
            else
                MessageBox.Show("Inkorrekte Eingabe!");
        }
    }
}