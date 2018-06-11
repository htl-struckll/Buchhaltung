using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

/*
 *
 */
namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für NewProduct.xaml
    /// </summary>
    public partial class NewProduct
    {
        private string[,] _allProductsSplitted;

        public NewProduct()
        {
            InitializeComponent();
            try
            {
                IdOfProduct.Text += Buchhaltung.Products[Buchhaltung.Products.Count - 1].Id + 1;
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }

            LoadProducts();
            LoadTaxDropDown();
            LoadChangeOfAmount();
            Topmost = true;
        }

        //Calls SaveProductFunction
        private void SaveProduct_CLICK(object sender, RoutedEventArgs e) => SaveProduct();


        //Saves the product to Buchhaltung
        public void SaveProduct()
        {
            bool doIt = false;
            try
            {
                string tax = TaxOfProduct.Text, price = PriceOfProduct.Text;
                if (tax.Contains("%"))
                    tax = tax.Replace("%", "");
                if (price.Contains("€"))
                    price = price.Replace("€", "");

                tax = tax.Trim();
                price = price.Trim();

                double dTax = Convert.ToDouble(tax);

                if (dTax > 0 && dTax < 101)
                    doIt = true;
                
                //adds the product
                if (Buchhaltung.Products.Max() != null && doIt)
                {
                    Product p = new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax), Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, Buchhaltung.Products.Max().Id);
                    Buchhaltung.Products.Add(p);
                }
                else if(doIt)
                {
                    Product p = new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax), Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, 0);
                    Buchhaltung.Products.Add(p);
                }

                if (doIt)
                {
                    Buchhaltung.SaveProducts();
                    Close();
                }
                else
                {
                    Buchhaltung.Log("Prozente überschritten!");
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }


        //Loads products from Textfile and splits them
        private void LoadProducts()
        {
            string allProducts ="";
            try
            {
                using (StreamReader reader = new StreamReader(@"Data\products.p"))
                {
                    allProducts = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            try
            {
                string[] firstSplit = allProducts.Split('\n');
                string[,] secondSplit = new string[firstSplit.Length+2, firstSplit[0].Length+2];  //Collum Row
                for (int i = 0; i < firstSplit.Length; i++)
                {
                    string[] tmpString = firstSplit[i].Split(';');
                    for (int x = 0; x < tmpString.Length; x++)
                    {
                        secondSplit[i, x] = tmpString[x];
                    }
                }
                _allProductsSplitted = secondSplit;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        private void LoadTaxDropDown()
        {
            try
            {
                for (int i = 0; i < _allProductsSplitted.GetLength(0) - 1; i++)
                {
                    if (!TaxDropDown.Items.Contains(_allProductsSplitted[i, 3] + "%"))
                        TaxDropDown.Items.Add(_allProductsSplitted[i, 3] + "%");
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        private void LoadChangeOfAmount()
        {
            try
            {
                for (int i = 0; i < _allProductsSplitted.GetLength(0) - 1; i++)
                {
                    if (!KindOfAmoutDropDown.Items.Contains(_allProductsSplitted[i, 5]))
                        KindOfAmoutDropDown.Items.Add(_allProductsSplitted[i, 5]);
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        //Updates the AmountOfKindOfProduct with the selected value
        private void UpdateKindOfAmount(object sender, SelectionChangedEventArgs e)
        {
            KindOfAmount.Text = (string) KindOfAmoutDropDown.SelectedValue;
            KindOfAmountDropDownOutput.Text = (string) KindOfAmoutDropDown.SelectedValue;
        }

        //Updates the TextField with the selected value
        private void UpdateTax(object sender, SelectionChangedEventArgs e)
        {
            TaxOfProduct.Text = (string) TaxDropDown.SelectedValue;
            TaxDropDownOutput.Text = (string) TaxDropDown.SelectedValue;
        }

        //Cancel
        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}