using System;
using System.Windows;
using System.Windows.Controls;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für OutputOfProducts.xaml
    /// </summary>
    public partial class OutputOfProducts : Window
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
            this.Topmost = true;
        }

        public void LoadDataInWpfWindow()
        {
            ProductTable.ItemsSource = Buchhaltung.products;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchFunc(object sender, TextChangedEventArgs e)
        {
            try
            {
                ProductTable.Items.Clear();
                if (SearchBox.Text.Trim() != "")
                {
                    //items
                    foreach (Product p in Buchhaltung.products.FindAll(p => (p.ID.ToString() == SearchBox.Text.Trim())
                                                                            | (SearchBox.Text.ToLower()
                                                                                .Contains((p.Name).ToLower()))
                                                                            | (SearchBox.Text.ToLower()
                                                                                .Contains(Convert.ToString(p.Price)
                                                                                    .ToLower()))
                                                                            | (SearchBox.Text.ToLower()
                                                                                .Contains(Convert.ToString(p.Tax)
                                                                                    .ToLower()))
                                                                            | (p.Name.ToLower()
                                                                                .Contains(SearchBox.Text.ToLower()))
                                                                            | (Convert.ToString(p.Price).ToLower()
                                                                                .Contains(SearchBox.Text.ToLower()))
                                                                            | (Convert.ToString(p.Tax).ToLower()
                                                                                .Contains(SearchBox.Text.ToLower()))
                                                                            | (Convert.ToString(p.KindOfAmount)
                                                                                .ToLower().Contains(
                                                                                    SearchBox.Text.ToLower()))
                        )
                    )
                    {
                        var data = p;
                        ProductTable.Items.Add(data);
                    }
                }
                else
                    LoadDataInWpfWindow();
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message + ex.StackTrace);
            }
        }

        private void UpdateIDsOfProduct()
        {
            for (int i = 0; i < Buchhaltung.products.Count; i++)
            {
                Buchhaltung.products[i].ID = i + 1;
            }
        }

        private void ProductGridLoaded(object sender, RoutedEventArgs e)
        {
            ProductTable.Columns[0].IsReadOnly = true;
        }
    }
}
