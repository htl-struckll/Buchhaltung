using System;
using System.Windows;
using System.Windows.Input;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for NewEntry.xaml
    /// </summary>
    public partial class NewEntry : Window
    {
        private double priceInUse;
        private int actualAmount;

        public NewEntry()
        {
            InitializeComponent();
        }

        #region Events
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (inputAmount.Text.Length > 0 && inputID.Text.Length > 0)
            {
                SaveEntry();
                this.Close();
            }
            else
                MessageBox.Show("Nicht alle Felder wurden ausgefüllt!");
        }

        private void OnTextInput_ID(object sender, KeyEventArgs e)
        {
            SearchForProduct();
        }

        private void InputPrice_keyDown(object sender, KeyEventArgs e)
        {
            CalcPrice();
        }
        #endregion

        private void SaveEntry()
        {
            try
            {
                foreach (Product p in Buchhaltung.products)
                {
                    if (p.ID != Convert.ToInt32(inputID.Text)) continue;
                    Entry e;
                    if (inputAmountOnTheHouse.Text == "")
                    {
                        // Convert.ToInt32(p.Amount *     Falls Probleme auftreten wieder probieren
                        actualAmount = Convert.ToInt32(inputAmount.Text);
                        e = new Entry(p, actualAmount, 0, priceInUse);
                    }
                    else
                    {
                        actualAmount = Convert.ToInt32(p.Amount * Convert.ToInt32(inputAmount.Text) + Convert.ToInt32(inputAmountOnTheHouse.Text));
                        e = new Entry(p, actualAmount, Convert.ToInt32(inputAmountOnTheHouse.Text),priceInUse);
                    }
                    //Adds at the currentWeek and the current Day the entry
                    Buchhaltung.currWeek.GetCurrentDayAndAddEntry(e);
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
            if (inputID.Text.Length <= 0 || inputAmount.Text.Length <= 0) return;

            priceOutput.Text = "Preis: ";
            try
            {
                foreach (Product p in Buchhaltung.products)
                {
                    if (Convert.ToInt32(inputID.Text) == p.ID)
                    {
                        priceOutput.Text += "   " + (p.Price * Convert.ToInt32(inputAmount.Text)) + "€";
                        priceInUse = p.Price * Convert.ToDouble(inputAmount.Text);
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
            if (inputID.Text.Length <= 0) return;

            productOutput.Text = "Produkt:";
            try
            {
                foreach (Product p in Buchhaltung.products)
                {
                    if (p.ID ==Convert.ToInt32(inputID.Text))
                    {
                        productOutput.Text += "       " + p.Name;
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
