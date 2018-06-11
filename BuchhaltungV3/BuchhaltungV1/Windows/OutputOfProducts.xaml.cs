using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für OutputOfProducts.xaml
    /// </summary>
    public partial class OutputOfProducts
    {
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
        /// Closes Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) => Close();

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
    }
}