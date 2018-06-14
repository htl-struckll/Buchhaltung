using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using MySql.Data.MySqlClient;

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
        #region var´s

        private List<Product> allProducts;
        private static MySqlConnection _connection; //sql connection
        #endregion

        public NewProduct()
        {
            InitializeComponent();
            try
            {
                IdOfProduct.Text += Buchhaltung.Products.Count.Equals(0) ? 1 : Buchhaltung.Products[Buchhaltung.Products.Count - 1].Id + 1;

                LoadProducts();
                LoadTaxDropDown();
                LoadKindOfAmounDropBox();
                LoadGroupDropBox();
                Topmost = true;
            }
            catch (Exception e)
            {
                Buchhaltung.SaveErrorMsg(e);
                Buchhaltung.Log(e.Message);
            }
        }

        /// <summary>
        /// Calls SaveProductFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProduct_CLICK(object sender, RoutedEventArgs e) => ProgressProductAndCallSave();


        /// <summary>
        /// Checks the input and saves it into Buchhaltung
        /// </summary>
        public void ProgressProductAndCallSave()
        {
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
                {
                    Product p = Buchhaltung.Products.Count.Equals(0) ? new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax),
                        Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, 1, Group.Text) : new Product(NameOfTheProdukt.Text, Convert.ToDouble(price), Convert.ToInt32(tax),
                        Convert.ToDouble(AmountOfProduct.Text), KindOfAmount.Text, Buchhaltung.Products.Max().Id+1,
                        Group.Text);

                    if (CheckIfProductExists(p))
                    {
                        if (Buchhaltung.ShowYesNoMessageBox(
                            "Das Produkt existiert bereits! Bist du sicher das du es trotzdem hinzufügen möchtest?",
                            "Produkt existiert schon"))
                        {
                            Buchhaltung.Products.Add(p);
                            SaveProductIntoSql(p);
                        }
                    }
                    else
                    { 
                       Buchhaltung.Products.Add(p);
                       SaveProductIntoSql(p);
                    }
                    Close();
                }else
                    Buchhaltung.Log("Steuer ist nicht im 0 - 100% Bereich!");
            }
            catch (Exception e)
            {
                Buchhaltung.SaveErrorMsg(e);
                Buchhaltung.Log(e.Message);

            }
        }


        /// <summary>
        /// Saves all Product
        /// </summary>
        public static void SaveProductIntoSql(Product p)
        {
            try
            {
                string query =
                    "INSERT INTO `Product`(`Id`, `Name`, `Price`, `Tax`, `Amount`, `KindOfAmount`, `GroupOfProduct`) VALUES(@id,@name,@price,@tax,@amount,@kindofamount,@groupofproduct)";

                CreateConnection();
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@id", p.Id);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@price", p.Price);
                cmd.Parameters.AddWithValue("@tax", p.Tax);
                cmd.Parameters.AddWithValue("@amount", p.Amount);
                cmd.Parameters.AddWithValue("@kindofamount", p.KindOfAmount);
                cmd.Parameters.AddWithValue("@groupofproduct", p.Group);
                cmd.Prepare();

                cmd.ExecuteScalar();

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }

        #region SQL

        /// <summary>
        /// Creates a new Connection
        /// </summary>
        protected static void CreateConnection() => _connection = new MySqlConnection(Buchhaltung.ConnectionString);

        /// <summary>
        /// Closes the connection
        /// </summary>
        protected static void CloseConnection() => _connection.Close();

        #endregion


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
        private void LoadProducts() => allProducts = Buchhaltung.Products;

        /// <summary>
        /// Loads tax to drop down
        /// </summary>
        private void LoadTaxDropDown()
        {
            foreach (Product product in allProducts)
            {
                if (!TaxDropDown.Items.Contains(product.Id + "%"))
                    TaxDropDown.Items.Add(product.Id + "%");
            }
        }

        /// <summary>
        /// Load kind of amount into dropdown
        /// </summary>
        private void LoadKindOfAmounDropBox()
        {
            foreach (Product product in allProducts)
            {
                if (!KindOfAmoutDropDown.Items.Contains(product.KindOfAmount))
                    KindOfAmoutDropDown.Items.Add(product.KindOfAmount);
            }
        }

        /// <summary>
        /// Loads groups into dropdown
        /// </summary>
        private void LoadGroupDropBox()
        {
            foreach (Product product in allProducts)
            {
                if (!GroupDropDown.Items.Contains(product.Group))
                    GroupDropDown.Items.Add(product.Group);
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
                ProgressProductAndCallSave();
        }
    }
}