using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.IO;

/*
 * AutomaticDate does not work (When less then 6 days into the month) 
 * Maybey instead of MessageBoxes a TextBlockSomewhere (Except for error)
 * find a better method instead of  this.Topmost = true;
 * KommaKürzen (komma stellen auf 2 reduzieren)
 * Aufs haus geht nicht richtig (Zusammenfassung und tabelle)
 */
namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für Buchhaltung.xaml 
    /// MainWindow of Programm
    /// Made by Struckl Lukas
    /// </summary>
    public partial class Buchhaltung
    {
        public static List<Product> Products = new List<Product>(); //list of all products
        public static Week CurrWeek; //the current week gets saved
        public static DayOfTheWeek CurrDay; //the current day for adding products
        public static List<DataBoxForGrid> Db = new List<DataBoxForGrid>(); //For editing to have references
        public static bool CloseWindow; //to close window

        /// <summary>
        /// Constructor if Week doesn´t exist
        /// </summary>
        /// <param name="weekNr"></param>
        /// <param name="name"></param>
        /// <param name="exTime"></param>
        /// <param name="lastCashDesk"></param>
        public Buchhaltung(string weekNr, string name, DateTime exTime, double lastCashDesk)
        {
            InitializeComponent();

            LoadProducts();

            CurrWeek = new Week(weekNr,exTime,name,lastCashDesk); //Selects the current week

            GenerateDaysOfThWeekAndAddToWeek(); 

            SaveEntrys();

            DayOutput.Text = "Montag";

            FillWeekInfo();
        }

        /// <summary>
        /// Constructor if week does exist
        /// </summary>
        /// <param name="weekNr"></param>
        public Buchhaltung(string weekNr)
        {
            InitializeComponent();
            LoadProducts();

            CurrWeek = new Week(weekNr); //Selects the current week

            GenerateDaysOfThWeekAndAddToWeek();

            LoadWeek(weekNr); //Needs to be loaded after Products 

            ReCalcAndUpdateInfoLine();

            DayOutput.Text = "Montag";

            FillWeekInfo();
        }

        /// <summary>
        /// Fills Info Box with standart information
        /// </summary>
        private void FillWeekInfo()
        {
            WeekNrOutput.Text = "WochenNr:  " +  CurrWeek.WeekNr;
            DateOutput.Text = "Datum:  "  + CurrWeek.ExactTime.Day + "." + CurrWeek.ExactTime.Month + "."+ CurrWeek.ExactTime.Year;
            PrevCashDeskOutput.Text = "Alter Kassenstand  " + CurrWeek.LastChashDesk + "€";
            NameOutput.Text = "Name:  " + CurrWeek.EditName;
            UpdateToBankField();
        }

        /// <summary>
        /// Seperatly updates the Money of the current day to the bank (For just changing the Day)
        /// </summary>
        private void UpdateToBankField() => ToBankOutput.Text = "An Bank: " + CurrWeek.GetCurrentDay().ToBank + "€";

        /// <summary>
        /// Opens a MessageBox with the Choice between Yes and No
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <returns>true or false (depends on answer)</returns>
        public static bool ShowYesNoMessageBox(string text, string caption)
        {
            MessageBoxResult result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Fills the entry list with entrys
        /// </summary>
        public void FillEntrys()
        {
            TableDay.Items.Clear();
            TableDay.Items.Refresh();
            Db.Clear();
            foreach (Day d in CurrWeek.DaysInWeek)
            {
                int counter = 0;
                if (d.Name == CurrDay)
                {
                    foreach (Entry e in d.Entrys)
                    {
                        counter++;
                        DataBoxForGrid data = new DataBoxForGrid(e.ProductForEntry.Id, e.ProductForEntry.Name, e.Amount, e.AmountOnTheHouse, e.ProductForEntry.Price, e.Price, counter, CurrDay,e.ProductForEntry.Amount,e.ProductForEntry.KindOfAmount); //Makes a new Object to add to the table
                        TableDay.Items.Add(data);
                        Db.Add(data);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Generates the days into the week
        /// </summary>
        private static void GenerateDaysOfThWeekAndAddToWeek()
        {//There ist a better solution (Gernerating dynamic) Fuck BackEnd
            for (int i = 0; i < 7; i++)
            {
                Day d = new Day((DayOfTheWeek)i);
                CurrWeek.AddDay(d);
            }
            CurrDay = DayOfTheWeek.Monday;
        }

        /// <summary>
        /// Calls the add new product window
        /// </summary>
        private void AddNewProduct()
        {
            NewProduct np = new NewProduct();
            np.Show();
            np.Closed += (a, b) =>
            {
                LoadProducts();
            };
        }

       /// <summary>
       /// opens the window to show all products
       /// </summary>
        private void ListAllProducts()
        {
            OutputOfProducts o = new OutputOfProducts();
            o.Show();

            o.Closed += (a, b) =>
            {
                SaveProducts();
                LoadProducts();
            };
        }

        /// <summary>
        /// Opens the window to add new entrys
        /// </summary>
        private void AddNewEntry()
        {
            NewEntry ne = new NewEntry();
            ne.Show();
            ne.Closed += (x, y) =>
            {
                FillEntrys();
                ReCalcAndUpdateInfoLine();
            };
        }

        /// <summary>
        /// Recalculate the Infolin und updates it
        /// </summary>
        public void ReCalcAndUpdateInfoLine()
        {
            double tenPriceAll = 0,twenPriceAll =0,priceAll = 0;

            foreach (Day d in CurrWeek.DaysInWeek)
            {
                if (d.Name == CurrDay)
                {
                    foreach (Entry e in d.Entrys)
                    {
                        foreach (Product p in Products)
                        {
                            if (p == e.ProductForEntry)
                            {
                                if (p.Tax == 20)
                                    twenPriceAll += e.Price;
                                else if (p.Tax == 10)
                                    tenPriceAll += e.Price;
                                priceAll += e.Price;
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            double tenPer = (tenPriceAll / 100) * 10;
            double twentPer = (twenPriceAll / 100) * 20;

            DailyIincomeOutput.Text = "Heutige Einnahmen: " + Convert.ToString(priceAll, CultureInfo.InvariantCulture) + "€"; //this culture thnigy is because stuff (servers) can interpret things in the string diffrent
            In10Output.Text = "Mwst in 10%: " + Convert.ToString(tenPer, CultureInfo.InvariantCulture) + "€";
            In20Output.Text = "Mwst in 20%: " + Convert.ToString(twentPer, CultureInfo.InvariantCulture) + "€";

            UpdateToBankField();
        }

        /// <summary>
        /// Calls window to edit the InfoBar
        /// </summary>
        private void EditInfoBar()
        {
            EditInfo e = new EditInfo();
            e.Show();
            e.Closed += (x, y) =>
            {
                FillWeekInfo();
            };
        }

        #region Events

        /// <summary>
        /// Shows the Help (How this programm is supposed to work)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("");
            //todo
        }

        /// <summary>
        /// Shows AGB's of this programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AGB_Click(object sender, MouseButtonEventArgs e)
        {
            //todo
        }

        /// <summary>
        /// Calls window to addBankAmount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToBank_Click(object sender, MouseButtonEventArgs e)
        {
            ToBankWindow w = new ToBankWindow();
            w.Show();
            w.Closed += (x, y) =>
            {
                FillWeekInfo();
            };
        }

        /// <summary>
        /// Makes window moveable
        /// </summary>
        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        /// <summary>
        /// Calls Mehtod to edit the info bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditInfo_Click(object sender, MouseButtonEventArgs e)
        {
            EditInfoBar();
        }
        /// <summary>
        /// Closes Programm and saves Products and Entrys (to be sure)
        /// </summary>
        private void CloseProgramm_CLICK(object sender, RoutedEventArgs e)
        {
            SaveProducts();
            SaveEntrys();
            Environment.Exit(0);
        }

        /// <summary>
        /// Minimizes Programm
        /// </summary>
        private void MinimiteProgramm_CLICK(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

       /// <summary>
       /// Calls function to List all Products
       /// </summary>
        private void ListAllProducts_Click(object sender, MouseButtonEventArgs e)
        {
            ListAllProducts();
        }

       /// <summary>
       /// Calls function to add a new Product
       /// </summary>
        private void AddNewProduct_Click(object sender, MouseButtonEventArgs e)
        {
            AddNewProduct();
         
        }
        /// <summary>
        /// Calls a currently DBG function to show all weeks
        /// </summary>
        private void ShowAllEntrys_Click(object sender, MouseButtonEventArgs e)
        {
            RecapOfWeek r = new RecapOfWeek();
            r.Show();
        }
        /// <summary>
        /// Opens a window to pick another week
        /// </summary>
        private void ChangeWeek_Click(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();

            mw.Closed += (x,y) =>
            {
                if(CloseWindow)
                Close();
                CloseWindow = true;
            };
        }

       /// <summary>
       /// Calls the function to add a new entry
       /// </summary>
        private void AddNewEntry_Click(object sender, RoutedEventArgs e)
        {
            AddNewEntry();
        }

        #region WhatDayIsChecked

        private void MondayChecked(object sender, RoutedEventArgs e)
        {
            CurrDay = DayOfTheWeek.Monday;
            DayOutput.Text = "Montag";
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void TuesdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Dienstag";
            CurrDay = DayOfTheWeek.Tuesday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void WednsdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Mittwoch";
            CurrDay = DayOfTheWeek.Wednesday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void ThursdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Donnerstag";
            CurrDay = DayOfTheWeek.Thursday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void FridayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Freitag";
            CurrDay = DayOfTheWeek.Friday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void SaturdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Samstag";
            CurrDay = DayOfTheWeek.Saturday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        private void SundayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Sonntag";
            CurrDay = DayOfTheWeek.Sunday;
            FillEntrys();
            ReCalcAndUpdateInfoLine();
        }

        #endregion

        /// <summary>
        /// Displays the credits
        /// </summary>
        private void Credits_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Made by: Lukas Struckl\nSpecial Thanks to: Lennart Putz");
        }

     /// <summary>
     /// Calls window to edit entry
     /// </summary>
        private void EditTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //string weekNr, string name, DateTime exTime, double lastCashDesk
            EditDataGrid ed = new EditDataGrid();
            ed.Show();

            ed.Closed += (x, y) =>
            {
                string nr = CurrWeek.WeekNr;
                CurrWeek = new Week(nr);
                GenerateDaysOfThWeekAndAddToWeek();
                LoadWeek(nr);
                FillEntrys();
                ReCalcAndUpdateInfoLine();
            };
        }

        //Calls functiion to backup files
        private void BackUp_Click(object sender, MouseButtonEventArgs e)
        {
            MakeBackUp();
        }

      
        #endregion

        #region Saves
        
        /// <summary>
        /// Calls Backup window to save backup
        /// </summary>
        private void MakeBackUp()
        {
            BackUpInput bp = new BackUpInput();
            bp.Show();
        }

      /// <summary>
      /// Saves all Product
      /// </summary>
        public static void SaveProducts()
        {
            using (StreamWriter writer = new StreamWriter(@"Data\products.p", append: false))
            {
                foreach (Product t in Products)
                {
                    writer.Write(t.Id + ";" + t.Name + ";" + t.Price + ";" + t.Tax + ";" + t.Amount + ";" + t.KindOfAmount + "\n");
                }
            }
        }
      /// <summary>
      /// Saves the érror msg´s 
      /// </summary>
      /// <param name="e"></param>
        public static void SaveErrorMsg(Exception e)
        {
            try
            {
                using (StreamWriter errorWriter = new StreamWriter(@"ProgrammFiles\error.log", append: true))
                {
                    errorWriter.WriteLine("[ " + DateTime.Now + " ]" + e.Message + " | " + e.StackTrace + "\n");
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

       /// <summary>
       /// Saves all entrys in current week
       /// </summary>
        public static void SaveEntrys()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(@"Data\" + CurrWeek.WeekNr + ".week", append: false))
                {
                    foreach (Day d in CurrWeek.DaysInWeek)
                    {
                        writer.WriteLine("[" + d.Name + "]");
                        foreach (Entry e in d.Entrys)
                        {
                            writer.WriteLine(e.ProductForEntry.Id + ";" + e.Amount + ";" + e.AmountOnTheHouse);
                        }
                    }

                    writer.WriteLine("[info]");
                    writer.Write(CurrWeek.ExactTime + ";" + CurrWeek.EditName + ";" + CurrWeek.LastChashDesk);
                    foreach (Day d in CurrWeek.DaysInWeek)
                    {
                            writer.Write(";" + d.ToBank);
                    }
                    writer.WriteLine("\n[end]");
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
                SaveErrorMsg(e);
            }
        }
        #endregion

        #region Loads and Splits

        /// <summary>
        /// Loads week File into programm
        /// </summary>
        /// <param name="weekName"></param>
        public void LoadWeek(string weekName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(@"Data\" + weekName + ".week"))
                {
                    string line = reader.ReadLine(), headLine = null;

                    while (line != "[end]")
                    {
                        if (line != null && line.Contains("["))
                            headLine = line.ToLower();

                        switch (headLine)
                        {
                            case "[monday]":
                                CurrDay = DayOfTheWeek.Monday;
                                break;
                            case "[tuesday]":
                                CurrDay = DayOfTheWeek.Tuesday;
                                break;
                            case "[wednesday]":
                                CurrDay = DayOfTheWeek.Wednesday;
                                break;
                            case "[friday]":
                                CurrDay = DayOfTheWeek.Friday;
                                break;
                            case "[saturday]":
                                CurrDay = DayOfTheWeek.Saturday;
                                break;
                            case "[sunday]":
                                CurrDay = DayOfTheWeek.Sunday;
                                break;
                            case "[info]":
                                line = reader.ReadLine();
                                string[] splittedLine = line?.Split(';');
                                CurrWeek.ExactTime = Convert.ToDateTime(splittedLine[0]);
                                CurrWeek.EditName = splittedLine[1];
                                CurrWeek.LastChashDesk = Convert.ToInt32(splittedLine[2]);
                                CurrWeek.GetCurrentDay().SetBankAmount(Convert.ToDouble(splittedLine[3]));
                                for (int i = 0; i < CurrWeek.DaysInWeek.Count; i++)
                                {
                                    CurrWeek.DaysInWeek[i].SetBankAmount(Convert.ToDouble(splittedLine[i + 3]));
                                }
                                break;
                        }
                        SplitAndAddLoadedEntry(line);
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Log("Fehler beim Laden der Entrys: " + e.Message);
                SaveErrorMsg(e);
            }
            finally
            {
                CurrDay = DayOfTheWeek.Monday;
                Monday.IsChecked = true;
            }
        }

      /// <summary>
      /// Split lines into better strings to use and adds them to the currDay
      /// </summary>
      /// <param name="line"></param>
        private void SplitAndAddLoadedEntry(string line)
        {
            if (line != null)
            {
                string[] splittedLine = line.Split(';');

                foreach (Product p in Products)
                {
                    if (Convert.ToString(p.Id) == splittedLine[0])
                    {
                        Entry e = new Entry(p, Convert.ToInt32(splittedLine[1]), Convert.ToInt32(splittedLine[2]), Convert.ToInt32(splittedLine[1]) * p.Price); 
                        CurrWeek.GetCurrentDayAndAddEntry(e);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// loads products into programm
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
                Log(e.Message);
                SaveErrorMsg(e);
            }

            string[] firstSplit = allProducts.Split('\n');
            try
            {
                Products.Clear();
                for (int i = 0; i < firstSplit.Length - 1; i++)
                {
                    string[] secondSplit = firstSplit[i].Split(';');
                    if (secondSplit[i] != "" || secondSplit[i] != null)
                    {
                        Product p = new Product(secondSplit[1], Convert.ToDouble(secondSplit[2]), Convert.ToInt32(secondSplit[3]), Convert.ToDouble(secondSplit[4]), secondSplit[5], Convert.ToInt32(secondSplit[0]) - 1);
                        Products.Add(p);
                    }
                }
            }
           catch (Exception e)
            {
                Log(e.Message);
                SaveErrorMsg(e);
            }
        }
        #endregion
        #region Output
        /// <summary>
        /// a Log function
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            MessageBox.Show("[ " + DateTime.Now + " ] " + msg);
        }
        #endregion

    }
}