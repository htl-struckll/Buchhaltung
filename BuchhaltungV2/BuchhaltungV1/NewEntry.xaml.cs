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
        private int _actualAmount;

        public NewEntry()
        {
            InitializeComponent();
        }

        #region Events
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            int inputAmount;
            try
            {
                inputAmount = Convert.ToInt32(InputAmount.Text);
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
                inputAmount = -1;
            }

            if (inputAmount > -1 && InputId.Text.Length > 0)
            {
                SaveEntry();
                Close();
            }
            else if(inputAmount != -1)
                MessageBox.Show("Nicht alle Felder wurden korrekt ausgefüllt!");
        }

        private void OnTextInput_ID(object sender, KeyEventArgs e)
        {
            string input = InputId.Text;

            try
            {
                int dummy = Convert.ToInt32(input);
                SearchForProduct();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        private void InputPrice_keyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string t = InputAmount.Text;
                double dummy = Convert.ToDouble(t);

                CalcPrice();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
            }
        }
        #endregion

        private void SaveEntry()
        {
            try
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (p.Id != Convert.ToInt32(InputId.Text)) continue;
                    Entry e;
                    if (InputAmountOnTheHouse.Text == "")
                    {
                        _actualAmount = Convert.ToInt32(InputAmount.Text);
                        e = new Entry(p, _actualAmount, 0, _priceInUse);
                    }
                    else
                    {
                        _actualAmount = Convert.ToInt32(p.Amount * Convert.ToInt32(InputAmount.Text) + Convert.ToInt32(InputAmountOnTheHouse.Text));
                        e = new Entry(p, _actualAmount, Convert.ToInt32(InputAmountOnTheHouse.Text),_priceInUse);
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

            Buchhaltung.SaveEntrys();
        }

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
                        PriceOutput.Text += "   " + (p.Price * Convert.ToInt32(InputAmount.Text)) + "€";
                        _priceInUse = p.Price * Convert.ToDouble(InputAmount.Text);
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

        private void SearchForProduct()
        {
            if (InputId.Text.Length <= 0) return;

            ProductOutput.Text = "Produkt:";
            try
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (p.Id ==Convert.ToInt32(InputId.Text))
                    {
                        ProductOutput.Text += "       " + p.Name;
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
    }
}
