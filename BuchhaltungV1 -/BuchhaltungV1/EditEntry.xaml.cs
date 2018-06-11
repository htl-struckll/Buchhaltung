using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for EditDataGrid.xaml
    /// </summary>
    public partial class EditDataGrid : Window
    {
        private Entry _originalEntry; ////try not to override but can´t be a const for obvius reasons /gets changed when changing the num/
        private Entry _newEntry;

        public EditDataGrid()
        {
            InitializeComponent();
        }

        #region ClickEvents
        //Cancels the programm
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Calls the edit function
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditDataAndSave();
        }

        //Calls delete function
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(Buchhaltung.ShowYesNoMessageBox("Bist du sicher das du diesen Eintrag löschen willst?", "Löschen"))
                DeleteCurrEntry();
        }
        #endregion

        //Deletes CurrentEntry
        private void DeleteCurrEntry()
        {
            GetCurrEntry();

            if (id_input.Text != "" && amount_input.Text != "" && onTheHouse_input.Text != "" && num_input.Text != "")
            {
                bool suc = false; //to stop the loop
                foreach (Day d in Buchhaltung.currWeek.DaysInWeek)
                {
                    if (d.Name == Buchhaltung.currDay)
                    {
                        for (int i = 0; i < d.Entrys.Count; i++)
                        {
                            //compares the original entry with the entrys in the mainProgramm
                            if (_originalEntry.ProductForEntry.ID == (d.Entrys[i].ProductForEntry.ID) && _originalEntry.Amount == (d.Entrys[i].Amount))
                            {
                                d.Entrys.RemoveAt(i);
                                suc = true;
                                break;
                            }
                        }
                    }
                    if (suc)
                        break;
                }
                Buchhaltung.SaveEntrys();
                this.Close();
            }
        }

        //edits the data 
        private void EditDataAndSave()
        {
            if (id_input.Text != "" && amount_input.Text != "" && onTheHouse_input.Text != "" && num_input.Text != "")
            {
                bool stopIt = false; //to stop the loop
                try
                {
                    foreach (Day d in Buchhaltung.currWeek.DaysInWeek)
                    {
                        Product pNew;
                        for (int i = 0; i < Buchhaltung.products.Count; i++)
                        {
                            if (id_input.Text == Convert.ToString(Buchhaltung.products[i].ID)) //Checks the id of the product
                            {
                                //Gives the newEntry the correct information
                                pNew = Buchhaltung.products[i]; //generates new Product for the entry
                                _newEntry.ProductForEntry = pNew;
                                _newEntry.Amount = Convert.ToInt32(amount_input.Text);
                                _newEntry.AmountOnTheHouse = Convert.ToInt32(onTheHouse_input.Text);
                                _newEntry.Price = _originalEntry.Amount * _originalEntry.ProductForEntry.Price; //schon verhaut
                                stopIt = true;
                                break;
                            }
                        }
                        if (stopIt) //stopsLoop
                            break;
                    }
                    OverridesEntryInMainProgramm();
                    Buchhaltung.SaveEntrys();
                    this.Close();
                }
                catch (Exception e)
                {
                    Buchhaltung.Log(e.Message);
                    Buchhaltung.SaveErrorMsg(e);
                }
            }
            else
                Buchhaltung.Log("Es wurden Felder leer gelassen");
        }

        //Override the MainEntry with the new Entry
        private void OverridesEntryInMainProgramm()
        {
            bool suc = false; //to stop the loop
            foreach (Day d in Buchhaltung.currWeek.DaysInWeek)
            {
                if (d.Name == Buchhaltung.currDay)
                {
                    for (int i = 0; i < d.Entrys.Count; i++)
                    {
                        //compares the original entry with the entrys in the mainProgramm
                        if (_originalEntry.ProductForEntry.ID == (d.Entrys[i].ProductForEntry.ID) && _originalEntry.Amount == (d.Entrys[i].Amount)) 
                        {
                            d.Entrys[i] = _newEntry;
                            suc = true;
                            break;
                        }
                    }
                }
                if (suc)
                    break;
            }
        }

        //Changes the OutputFields when the num textBox is changed and gets the correct entry
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool suc = false;
            try
            {
                foreach (DataBoxForGrid d in Buchhaltung.db)
                {
                    if (num_input.Text == Convert.ToString(d.C))
                    {
                        id_input.Text = Convert.ToString(d.ID);
                        outputName.Text = d.Name;
                        amount_input.Text = Convert.ToString(d.Amount);
                        onTheHouse_input.Text = Convert.ToString(d.OnTheHouse);
                        suc = true;
                        break;
                    }
                    else
                    {
                        id_input.Text = "";
                        outputName.Text = "";
                        amount_input.Text = "";
                        onTheHouse_input.Text = "";
                    }
                }
                GetCurrEntry();

                if(!suc)
                {
                    _originalEntry = null;
                    _newEntry = null;
                }
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        //gets the new entry
        private void GetCurrEntry()
        {
            if (num_input.Text != "" && id_input.Text != "" && amount_input.Text != "" && onTheHouse_input.Text != "")
            {
                foreach (Day d in Buchhaltung.currWeek.DaysInWeek)
                {
                    if (d.Name == Buchhaltung.currDay)
                    {
                        foreach (Entry e in d.Entrys)
                        {
                            if (id_input.Text == Convert.ToString(e.ProductForEntry.ID) && amount_input.Text == Convert.ToString(e.Amount))
                            {
                                _originalEntry = new Entry(e.ProductForEntry, e.Amount, e.AmountOnTheHouse, e.Price);
                                _newEntry = new Entry(e.ProductForEntry, e.Amount, e.AmountOnTheHouse, e.Price);
                                break;
                            }
                        }
                    }
                }
            }
        }

        //updated the changed id to the correct product
        private void IdChange(object sender, TextChangedEventArgs e)
        {
            if(id_input.Text != "")
            {
                foreach (Product p in Buchhaltung.products)
                {
                    if(Convert.ToString(p.ID) == id_input.Text)
                    {
                        outputName.Text = p.Name;
                        break;
                    }
                }
            }
        }
    }
}
