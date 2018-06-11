using System;
using System.Windows;
using System.Windows.Controls;

namespace BuchhaltungV4
{
    /// <summary>
    /// Interaction logic for EditDataGrid.xaml
    /// </summary>
    public partial class EditDataGrid
    {
        private Entry
            _originalEntry; //try not to override but can´t be a const for obvious reasons /gets changed when changing the num/

        private Entry _newEntry; //new entry to override the originalOne

        public EditDataGrid()
        {
            InitializeComponent();
            Activate();
        }

        #region ClickEvents

        //Cancels the programm
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Calls the edit function
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditDataAndSave();
        }

        //Calls delete function
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Buchhaltung.ShowYesNoMessageBox("Bist du sicher das du diesen Eintrag löschen willst?", "Löschen"))
                DeleteCurrEntry();
        }

        #endregion

        /// <summary>
        /// Deletes CurrEntry in Table and Data
        /// </summary>
        private void DeleteCurrEntry()
        {
            GetCurrEntry();

            if (IdInput.Text != "" && AmountInput.Text != "" && OnTheHouseInput.Text != "" && NumInput.Text != "")
            {
                bool suc = false; //to stop the loop
                foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
                {
                    if (d.Name == Buchhaltung.CurrDay)
                    {
                        for (int i = 0; i < d.Entrys.Count; i++)
                        {
                            //compares the original entry with the entrys in the mainProgramm
                            if (_originalEntry.ProductForEntry.Id == (d.Entrys[i].ProductForEntry.Id) &&
                                _originalEntry.Amount == (d.Entrys[i].Amount))
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
                Close();
            }
        }

        /// <summary>
        /// Edits data In Table and Data
        /// </summary>
        private void EditDataAndSave()
        {
            if (IdInput.Text != "" && AmountInput.Text != "" && OnTheHouseInput.Text != "" && NumInput.Text != "")
            {
                bool stopIt = false; //to stop the loop
                try
                {
                    foreach (Day dummy in Buchhaltung.CurrWeek.DaysInWeek)
                    {
                        foreach (Product t in Buchhaltung.Products)
                        {
                            if (IdInput.Text == Convert.ToString(t.Id)) //Checks the id of the product
                            {
                                //Gives the newEntry the correct information
                                Product pNew = t;
                                _newEntry.ProductForEntry = pNew;
                                _newEntry.Amount = Convert.ToInt32(AmountInput.Text);
                                _newEntry.AmountOnTheHouse = Convert.ToInt32(OnTheHouseInput.Text);
                                _newEntry.Price = _originalEntry.Amount * _originalEntry.ProductForEntry.Price;
                                stopIt = true;
                                break;
                            }
                        }

                        if (stopIt) //stopsLoop
                            break;
                    }

                    OverridesEntryInMainProgramm();
                    Buchhaltung.SaveEntrys();
                    Close();
                }
                catch (Exception e)
                {
                    Buchhaltung.Log(e.Message);
                    Buchhaltung.SaveErrorMsg(e);
                }
            }
            else
                Buchhaltung.Log("Es wurden Felder leer gelassen!");
        }

        /// <summary>
        /// Override the MainEntry with the new Entry
        /// </summary>
        private void OverridesEntryInMainProgramm()
        {
            bool suc = false; //to stop the loop
            foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
            {
                if (d.Name == Buchhaltung.CurrDay)
                {
                    for (int i = 0; i < d.Entrys.Count; i++)
                    {
                        //compares the original entry with the entrys in the mainProgramm
                        if (_originalEntry.ProductForEntry.Id == (d.Entrys[i].ProductForEntry.Id) &&
                            _originalEntry.Amount == (d.Entrys[i].Amount))
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

        /// <summary>
        /// Fills the TextField when the Num_input is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool suc = false;
            try
            {
                foreach (DataBoxForGrid d in Buchhaltung.Db)
                {
                    if (NumInput.Text == Convert.ToString(d.C))
                    {
                        IdInput.Text = Convert.ToString(d.Id);
                        OutputName.Text = d.Name;
                        AmountInput.Text = Convert.ToString(d.Amount);
                        OnTheHouseInput.Text = Convert.ToString(d.OnTheHouse);
                        suc = true;
                        break;
                    }

                    IdInput.Text = "";
                    OutputName.Text = "";
                    AmountInput.Text = "";
                    OnTheHouseInput.Text = "";
                }

                GetCurrEntry();

                if (!suc)
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

        /// <summary>
        /// Gets the new Entry
        /// </summary>
        private void GetCurrEntry()
        {
            if (NumInput.Text != "" && IdInput.Text != "" && AmountInput.Text != "" && OnTheHouseInput.Text != "")
            {
                foreach (Day d in Buchhaltung.CurrWeek.DaysInWeek)
                {
                    if (d.Name == Buchhaltung.CurrDay)
                    {
                        foreach (Entry e in d.Entrys)
                        {
                            if (IdInput.Text == Convert.ToString(e.ProductForEntry.Id) &&
                                AmountInput.Text == Convert.ToString(e.Amount))
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

        /// <summary>
        /// updated the changed id to the correct product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdChange(object sender, TextChangedEventArgs e)
        {
            if (IdInput.Text != "")
            {
                foreach (Product p in Buchhaltung.Products)
                {
                    if (Convert.ToString(p.Id) == IdInput.Text)
                    {
                        OutputName.Text = p.Name;
                        break;
                    }
                }
            }
        }
    }
}