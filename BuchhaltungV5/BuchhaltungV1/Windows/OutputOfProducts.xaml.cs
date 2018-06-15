using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Threading;
//todo write update if row was edited
namespace BuchhaltungV4
{
    /// <summary>
    /// Interaktionslogik für OutputOfProducts.xaml
    /// </summary>
    public partial class OutputOfProducts
    {
        private static MySqlConnection _connection;

        public OutputOfProducts()
        {
            try
            {
                InitializeComponent();
                LoadDataInWpfWindow();
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }

            Topmost = true;
        }

        /// <summary>
        /// Loads data into WpfWindow
        /// </summary>
        public void LoadDataInWpfWindow() => ProductTable.ItemsSource = Buchhaltung.Products;

        /// <summary>
        /// Seaarches the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchFunc(object sender, TextChangedEventArgs e)
        {
            try
            {
                ProductTable.Items.Filter = o =>
                {
                    if (SearchBox.Text == "")
                    {
                        return true;
                    }

                    using (Product p = o as Product)
                    {
                        if (p != null && p.Id.ToString() == SearchBox.Text.Trim()
                            | (SearchBox.Text.ToLower().Contains((p.Name).ToLower()))
                            | (SearchBox.Text.ToLower()
                                .Contains(Convert.ToString(p.Price, CultureInfo.CurrentCulture).ToLower()))
                            | (SearchBox.Text.ToLower().Contains(Convert.ToString(p.Tax).ToLower()))
                            | (SearchBox.Text.ToLower().Contains(Convert.ToString(p.Group).ToLower()))
                            | (p.Name.ToLower().Contains(SearchBox.Text.ToLower()))
                            | (Convert.ToString(p.Price, CultureInfo.CurrentCulture).ToLower()
                                .Contains(SearchBox.Text.ToLower()))
                            | (Convert.ToString(p.Tax).ToLower().Contains(SearchBox.Text.ToLower()))
                            | (Convert.ToString(p.Group).ToLower().Contains(SearchBox.Text.ToLower()))
                            | (Convert.ToString(p.KindOfAmount).ToLower().Contains(SearchBox.Text.ToLower())))
                            return true;
                    }

                    return false;
                };
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Edits the Grid to the correct 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductGridLoaded(object sender, RoutedEventArgs e)
        {
            ProductTable.Columns[0].IsReadOnly = true;

            ProductTable.Columns[2].Header = "Preis (€)";
            ProductTable.Columns[3].Header = "Steuer (%)";
            ProductTable.Columns[4].Header = "Menge";
            ProductTable.Columns[5].Header = "Art der Menge";
            ProductTable.Columns[6].Header = "Kat.";
        }

        /// <summary>
        /// Deletes the product
        /// </summary>
        /// <param name="p"></param>
        private void Delete(Product p)
        {
            //if product is null it returns
            if (p.Equals(null)) return;

            if (MessageBox.Show("'" + p.Name + "' löschen?", "Sicher löschen", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Buchhaltung.Products.Remove(p);
                RemoveProductSQL(p.Id);
                ProductTable.Items.Refresh();
            }
        }

        #region Event´s
        /// <summary>
        /// Opens the new Product window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewProduct np = new NewProduct();
            np.Show();

            np.Closed += (x, y) =>
            {
                ProductTable.ItemsSource = null;
                LoadDataInWpfWindow();
            };
        }

        /// <summary>
        /// Calls the delete function for the product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e) => Delete(ProductTable.SelectedItem is Product ut ? ut : null);

        /// <summary>
        /// Closes Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) => Close();
        #endregion

        #region sql

        /// <summary>
        /// Removes the product from the sql table
        /// </summary>
        /// <param name="id"></param>
        private void RemoveProductSQL(int id)
        {
            try
            {
                string query = "DELETE FROM `Product` WHERE id LIKE " + id;
                CreateConnection();

                MySqlCommand commandDatabase = new MySqlCommand(query, _connection) { CommandTimeout = 60 };
                _connection.Open();

                commandDatabase.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        private void UpdateCell()
        {
          
            Product p = _itemFromProductTable;
            
            try
            {
                string query = "UPDATE `Product` SET `Name`=@name,`Price`=@price,`Tax`=@tax,`Amount`=@amount,`KindOfAmount`=@kindofamount,`GroupOfProduct`=@groupofproduct WHERE id LIKE " + p.Id;
                CreateConnection();

                MySqlCommand cmd = new MySqlCommand(query, _connection) { CommandTimeout = 60 };
                _connection.Open();

                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@price", p.Price);
                cmd.Parameters.AddWithValue("@tax", p.Tax);
                cmd.Parameters.AddWithValue("@amount", p.Amount);
                cmd.Parameters.AddWithValue("@kindofamount", p.KindOfAmount);
                cmd.Parameters.AddWithValue("@groupofproduct", p.Group);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }



        /// <summary>
        /// Creates a new Connection
        /// </summary>
        protected static void CreateConnection() => _connection = new MySqlConnection(Buchhaltung.ConnectionString);

        /// <summary>
        /// Closes the connection
        /// </summary>
        protected static void CloseConnection() => _connection.Close();
        #endregion

        private Product _itemFromProductTable = null;
        private void ProductTable_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!(e.Row.Item is Product))
                Buchhaltung.Log("ERROR: With getting item");
            else
            {
                _itemFromProductTable = (Product)e.Row.Item;
                Thread thread = new Thread(UpdateCell);
                thread.Start();
            }
        }

    }
}
