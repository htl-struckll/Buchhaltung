using System.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for RecapOfWeek.xaml
    /// </summary>
    public partial class RecapOfWeek
    {

        private readonly List<DataForWeekRecapTable> _data = new List<DataForWeekRecapTable>();
            
        public RecapOfWeek() { 

            InitializeComponent();
            FillWindow();
            FillTable();
            Topmost = true;
        }

        private void FillWindow()
        {
            WeekNrOutput.Text = "Kw: " + Buchhaltung.CurrWeek.WeekNr;
            NameOutput.Text = "Name: " + Buchhaltung.CurrWeek.EditName;
            DateOutput.Text = "Datum: " + Buchhaltung.CurrWeek.ExactTime.ToShortDateString();
            PlaceKeeperOutput.Text = "Platzwart: 230€";
            CantinaOutput.Text = "Kantine: " + Convert.ToString(240 + GetSales() * 0.10, CultureInfo.InvariantCulture) + "€";
            TwenTaxOutput.Text = Convert.ToString(GetSales() * 0.20, CultureInfo.InvariantCulture) + "€";
            TenTaxOutput.Text = Convert.ToString(GetSales() * 0.10, CultureInfo.InvariantCulture) + "€";
            LastCashOutOutput.Text = Convert.ToString(Buchhaltung.CurrWeek.LastChashDesk, CultureInfo.InvariantCulture) + "€";
            SummeOutput.Text = Convert.ToString(GetSales()) + "€";
            ToBankOutput.Text = Convert.ToString(GetTotalBank(), CultureInfo.InvariantCulture) + "€";
            AllSalesOutput.Text ="Gesamt: " +  Convert.ToString( GetSales() - GetSales() * 0.10  - 230- 240 - GetTotalBank() + Buchhaltung.CurrWeek.LastChashDesk, CultureInfo.InvariantCulture) + "€";
        }

        private static double GetSales()
        {
            try
            {
                double sales = 0;
                foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
                {
                    foreach (Entry e in d.Entrys)
                    {
                        sales += e.Price;
                    }
                }

                return sales;
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }

            return 0; //error
        }

        /// <summary>
        /// Gets the total bank amount
        /// </summary>
        /// <returns>totalBank</returns>
        private double GetTotalBank()
        {
            double totalBank = 0;

            foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
            {
                totalBank += d.ToBank;
            }

            return totalBank;
        }

        /// <summary>
        /// Looks if the entrys are already there and when not it adds them
        /// </summary>
        private void FillTable()
        {
            RecapTable.Items.Clear();

            foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
            {
                foreach (Entry e in d.Entrys)
                {
                    bool success = false;
                    foreach (DataForWeekRecapTable existignData in _data)
                    {
                        if (existignData.Name == e.ProductForEntry.Name)
                        {
                            existignData.Amount += e.Amount;
                            existignData.AmountOfKindOfProduct += e.ProductForEntry.Amount * e.Amount;
                            success = true;
                            break;
                        }
                    }

                    if (!success)
                    {
                        DataForWeekRecapTable dataToAdd = new DataForWeekRecapTable(e.Amount, e.ProductForEntry.Name, e.ProductForEntry.Amount * e.Amount,e.ProductForEntry.KindOfAmount);
                        _data.Add(dataToAdd);
                    }
                }                    
            }
            RecapTable.ItemsSource = _data;
        }

        private void ProductGridLoaded(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn recapTableColumn in RecapTable.Columns)
            {
                recapTableColumn.IsReadOnly = true;
            }
        }
    }
}
