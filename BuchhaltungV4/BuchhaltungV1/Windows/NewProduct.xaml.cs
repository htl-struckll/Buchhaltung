using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;

/*
 *
 */
namespace BuchhaltungV4
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
            LoadGroupDropBox();
            Topmost = true;
        }

        /// <summary>
        /// Calls SaveProductFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProduct_CLICK(object sender, RoutedEventArgs e) => SaveProduct();


        /// <summary>
        /// Checks the input and saves it into Buchhaltung
        /// </summary>
        public void SaveProduct()
        {
            bool taxCheck = false;
            bool closeCheck = false;

            try
            {
                //removes the % and € symbol from the input
                string tax = TaxOfProduct.Text, price = PriceOfProduct.Text;
                if (tax.Contains("%"))
                    tax = tax.Replace("%", "");
                if (price.Contains("€"))
                    price = price.Replace("€", "");
                if (tax.Contains("."))
                    tax = tax.Replace(".", ",");
                if (price.Contains("."))
                    price = price.Replace(".", ",");

                //trims them down
                tax = tax.Trim();
                price = price.Trim();

                double dTax = Convert.ToDouble(tax);

                if (dTax > 0 && dTax < 101)
                    taxCheck = true;

                //adds the product (Checks the product id)
                if (Buchhaltung.Products.Max() != null && taxCheck)
                {
                    Product p = new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax),
                        Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, Buchhaltung.Products.Max().Id,
                        Group.Text);

                    if (CheckIfProductExists(p))
                    {
                        if (Buchhaltung.ShowYesNoMessageBox(
                            "Das Produkt existiert bereits! Bist du sicher das du es trotzdem hinzufügen möchtest?",
                            "Produkt existiert schon"))
                        {
                            Buchhaltung.Products.Add(p);
                            closeCheck = true;
                        }
                    }
                    else
                    {
                        Buchhaltung.Products.Add(p);
                        closeCheck = true;
                    }
                }
                else if (taxCheck)
                {
                    Product p = new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax),
                        Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, 0, Group.Text);

                    if (CheckIfProductExists(p))
                    {
                        if (Buchhaltung.ShowYesNoMessageBox(
                            "Das Produkt existiert bereits! Bist du sicher das du es trotzdem hinzufügen möchtest?",
                            "Produkt existiert schon"))
                        {
                            Buchhaltung.Products.Add(p);
                            closeCheck = true;
                        }
                    }
                    else
                    {
                        Buchhaltung.Products.Add(p);
                        closeCheck = true;
                    }
                }

                if (taxCheck && closeCheck)
                {
                    Buchhaltung.SaveProducts();
                    Close();
                }
                else
                {
                    Buchhaltung.Log("Ungültige Prozente!");
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        /// <summary>
        /// Checks if the product you want to add already eyists
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns>true if exists || false if does not exist</returns>
        private bool CheckIfProductExists(Product toAdd)
        {
            foreach (Product p in Buchhaltung.Products)
            {
                if (p.Name.ToLower() == toAdd.Name.ToLower() && p.Price == toAdd.Price && p.Tax == toAdd.Tax &&
                    p.Amount == toAdd.Amount && p.KindOfAmount.ToLower() == toAdd.KindOfAmount.ToLower() &&
                    p.Group.ToLower() == toAdd.Group.ToLower())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Loads products from Textfile and splits them into a 2d array
        /// </summary>
        private void LoadProducts()
        {
            string allProducts = "";
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
                string[,] secondSplit = new string[firstSplit.Length + 2, firstSplit[0].Length + 2]; //Collum Row
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

        /// <summary>
        /// Loads tax to drop down
        /// </summary>
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

        /// <summary>
        /// Load kind of amount into dropdown
        /// </summary>
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

        /// <summary>
        /// Loads groups into dropdown
        /// </summary>
        private void LoadGroupDropBox()
        {
            try
            {
                for (int i = 0; i < _allProductsSplitted.GetLength(0) - 1; i++)
                {
                    if (!GroupDropDown.Items.Contains(_allProductsSplitted[i, 6]))
                        GroupDropDown.Items.Add(_allProductsSplitted[i, 6]);
                }
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        /// <summary>
        /// Updates the AmountOfKindOfProduct with the selected value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateKindOfAmount(object sender, SelectionChangedEventArgs e)
        {
            KindOfAmount.Text = (string) KindOfAmoutDropDown.SelectedValue;
            KindOfAmountDropDownOutput.Text = (string) KindOfAmoutDropDown.SelectedValue;
        }

        /// <summary>
        /// Updates the TextField with the selected value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTax(object sender, SelectionChangedEventArgs e)
        {
            TaxOfProduct.Text = (string) TaxDropDown.SelectedValue;
            TaxDropDownOutput.Text = (string) TaxDropDown.SelectedValue;
        }

        /// <summary>
        /// Update the TextField with the selected value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateGroup(object sender, SelectionChangedEventArgs e)
        {
            Group.Text = (string) GroupDropDown.SelectedValue;
            GroupDropDownOutput.Text = (string) GroupDropDown.SelectedValue;
        }

        /// <summary>
        /// Cancels and cloese the product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Cheks if button is enter and handels it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEnter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                SaveProduct();
        }
    }
}