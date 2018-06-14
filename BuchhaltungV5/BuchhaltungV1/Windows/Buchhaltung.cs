using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BuchhaltungV1.Windows;
using MySql.Data.MySqlClient;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Rectangle = System.Windows.Shapes.Rectangle; //To assign the Rectangle to the correct Library (Two diffrent)

/*
 * overall implementation of sql
 */
namespace BuchhaltungV4
{
    /// <summary>
    /// Interaktionslogik für Buchhaltung.xaml 
    /// MainWindow of Programm
    /// Made by Struckl Lukas
    /// </summary>
    public partial class Buchhaltung
    {
        #region Var´s
        public static List<Product> Products = new List<Product>(); //list of all products
        public static Week CurrWeek; //the current week gets saved
        public static DayOfTheWeek CurrDay; //the current day for adding products
        public static List<DataBoxForGrid> Db = new List<DataBoxForGrid>(); //For editing to have references
        public static bool CloseWindow; //to close window
        public static bool IsAdmin; //to see if user is a admin
        public const string ConnectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=Buchhaltung;SslMode=none"; //connection string for mysql user
        private static MySqlConnection _connection; //sql connection
        public static string Username; //current username logged in
        #endregion

        #region Ctor´s

        /// <inheritdoc />
        /// <summary>
        /// Constructor if Week doesn´t exist
        /// </summary>
        /// <param name="weekNr"></param>
        /// <param name="name"></param>
        /// <param name="exTime"></param>
        /// <param name="lastCashDesk"></param>
        /// <param name="isAdmin"></param>
        public Buchhaltung(string weekNr, string name, DateTime exTime, double lastCashDesk,string username, int isAdmin)
        {
            InitializeComponent();
            Username = username;

            IsAdmin = Convert.ToBoolean(isAdmin);
            LoadProducts();
            CurrWeek = new Week(weekNr, exTime, name, lastCashDesk); //Selects the current week
            GenerateDaysOfThWeekAndAddToWeek();
            SaveEntrys();
            DayOutput.Text = "Montag";
            FillWeekInfo();

            if (IsAdmin)
                ShowAdminFeatures();
        }

        /// <summary>
        /// Constructor if week does exist
        /// </summary>
        /// <param name="weekNr"></param>
        /// <param name="isAdmin"></param>
        public Buchhaltung(string weekNr,string username, int isAdmin)
        {
            InitializeComponent();
            Username = username;

            IsAdmin = Convert.ToBoolean(isAdmin);
            LoadProducts();
            CurrWeek = new Week(weekNr); //Selects the current week
            GenerateDaysOfThWeekAndAddToWeek();
            LoadWeek(weekNr); //Needs to be loaded after Products 
            ReCalcAndUpdateInfoLine();
            DayOutput.Text = "Montag";
            FillWeekInfo();

            if (IsAdmin)
                ShowAdminFeatures();
        }

        #endregion

        #region FillData

        /// <summary>
        /// Shows all the admin features
        /// </summary>
        private void ShowAdminFeatures()
        {
            AdminUserGrid.Opacity = 100;
        }

        /// <summary>
        /// Fills Info Box with standart information
        /// </summary>
        private void FillWeekInfo()
        {
            WeekNrOutput.Text = "WochenNr:  " + CurrWeek.WeekNr;
            DateOutput.Text = "Datum:  " + CurrWeek.ExactTime.Day + "." + CurrWeek.ExactTime.Month + "." +
                              CurrWeek.ExactTime.Year;
            PrevCashDeskOutput.Text = "Alter Kassenstand  " + CurrWeek.LastChashDesk + "€";
            NameOutput.Text = "Name:  " + CurrWeek.EditName;
            UpdateToBankField();
        }

        /// <summary>
        /// Seperatly updates the Money of the current day to the bank (For just changing the Day)
        /// </summary>
        private void UpdateToBankField() => ToBankOutput.Text = "An Bank: " + CurrWeek.GetCurrentDay().ToBank + "€";


        /// <summary>
        /// Fills the entry list with entrys
        /// </summary>
        public void FillEntryTable()
        {
            TableDay.Items.Clear();
            TableDay.Items.Refresh();
            Db.Clear(); //Database to Fill the Table
            foreach (Day d in CurrWeek.DaysInWeek)
            {
                int counter = 0;
                if (d.Name == CurrDay)
                {
                    foreach (Entry e in d.Entrys)
                    {
                        counter++;
                        DataBoxForGrid data = new DataBoxForGrid(e.ProductForEntry.Id, e.ProductForEntry.Name, e.Amount,
                            e.AmountOnTheHouse, e.ProductForEntry.Price, e.Price, counter, CurrDay,
                            e.ProductForEntry.Amount,
                            e.ProductForEntry.KindOfAmount); //Makes a new Object to add to the table
                        TableDay.Items.Add(data);
                        Db.Add(data);
                    }

                    break;
                }
            }
        }


        /// <summary>
        /// Recalculate the Infoline und updates it
        /// </summary>
        public void ReCalcAndUpdateInfoLine()
        {
            double tenPriceAll = 0, twenPriceAll = 0, priceAll = 0;

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

            DailyIincomeOutput.Text =
                "Heutige Einnahmen: " + Convert.ToString(priceAll, CultureInfo.InvariantCulture) +
                "€"; //this culture thnigy is because stuff (servers) can interpret things in the string diffrent
            In10Output.Text = "Mwst in 10%: " + Convert.ToString(tenPer, CultureInfo.InvariantCulture) + "€";
            In20Output.Text = "Mwst in 20%: " + Convert.ToString(twentPer, CultureInfo.InvariantCulture) + "€";

            UpdateToBankField();
        }

        #endregion

        #region GenerateData

        /// <summary>
        /// Generates the days into the week
        /// </summary>
        private static void GenerateDaysOfThWeekAndAddToWeek()
        {
            //There ist a better solution (Gernerating dynamic) Fuck BackEnd
            for (int i = 0; i < 7; i++)
            {
                Day d = new Day((DayOfTheWeek) i);
                CurrWeek.AddDay(d);
            }

            CurrDay = DayOfTheWeek.Monday;
        }

        #endregion

        #region CallWindows
        /// <summary>
        /// Logs you out
        /// </summary>
        private void Logout()
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }

        /// <summary>
        /// shows the user window
        /// </summary>
        private void ShowAllUsers()
        {
            UserWindow uw = new UserWindow();
            uw.Show();
        }

        /// <summary>
        /// Calls window to change the week
        /// </summary>
        private void ChangeWeek()
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.SetLoggedIn();
            mw.
                IsAdmin = Convert.ToInt32(IsAdmin);

            mw.Closed += (x, y) =>
            {
                if (CloseWindow)
                    Close();
                CloseWindow = true;
            };
        }

        /// <summary>
        /// Calls the add new product window
        /// </summary>
        private void AddNewProduct()
        {
            NewProduct np = new NewProduct();
            np.Show();
        }

        /// <summary>
        /// opens the window to show all products
        /// </summary>
        private void ListAllProducts()
        {
            OutputOfProducts o = new OutputOfProducts();
            o.Show();
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
                FillEntryTable();
                ReCalcAndUpdateInfoLine();
            };
        }

        /// <summary>
        /// Calls window to edit the InfoBar
        /// </summary>
        private void EditInfoBar()
        {
            EditInfo e = new EditInfo();
            e.Show();
            e.Closed += (x, y) => { FillWeekInfo(); };
        }

        #endregion

        #region Events

        /// <summary>
        /// Expands the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandUserSettings(object sender, MouseButtonEventArgs e) => UserComboBox.IsDropDownOpen = true;

        /// <summary>
        /// Expands Week
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandWeek(object sender, MouseButtonEventArgs e) => ComboBoxWeek.IsDropDownOpen = true;

        /// <summary>
        /// Expands the Product Drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandProducts(object sender, MouseButtonEventArgs e) =>
            ComboBoxProducts.IsDropDownOpen = true;

        /// <summary>
        /// Changes the picture to drop up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosedDropDownDay(object sender, EventArgs e)
        {
            ArrowDropUp.Opacity = 0;
            ArrowDropDown.Opacity = 100;
        }

        /// <summary>
        /// Changes the picture to a drop up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenedDropDownDay(object sender, EventArgs e)
        {
            ArrowDropDown.Opacity = 0;
            ArrowDropUp.Opacity = 100;
        }

        /// <summary>
        /// Expands the WeekDropDown 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandDaysInWeek(object sender, MouseButtonEventArgs e) =>
            DropDownWeek.IsDropDownOpen = true;

        /// <summary>
        /// Shows all users event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllUsers_Click(object sender, MouseButtonEventArgs e) => ShowAllUsers();

        /// <summary>
        /// Logs you out event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, MouseButtonEventArgs e) => Logout();

        /// <summary>
        /// Shows as which user you are logged on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoggedOn_MouseDown(object sender, MouseButtonEventArgs e) =>
            Log("Du bist als '" + Username + "' angemeldet");
        
        /// <summary>
        /// Shows the Help (How this programm is supposed to work)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, MouseButtonEventArgs e) => MessageBox.Show(
            "Start: \n     Um einen Eintrag zu erstellen muss man erst ein Produkt anlegen. \n      (Unter Produkt -> Neues Produkt)\n\nProdukte anzeigen und Bearbeiten:\n         Um Produkte zu bearbeiten muss man alle Proukte anzeigen lassen.\n         (Produkt -> Alle Anzeigen)\n         Doppelclick auf die zu bearbeitende Zeile und neue Werte eintragen.\n         (Achtung! Beim Komma einen '.' verwenden)\n\nEinträge:\n      Um einen neuen Eintrag zu erstellen auf 'Neuen Eintrag' drücken.\n      Weiteren Anweisungen am Blidschirm folgen.\n\nEinträge bearbeiten:\n        Doppelclick in die Tabelle.\n        Nummer der zu bearbeitenden Zeile eintragen.\n        Werte umschreiben.\n\nAn Bank überweisen:\n       An Bank\n       (Unter Sonstige -> An Bank)\n\nInfo Zeile bearbeiten:\n        Info Berab. (Unter Woche -> Info Bearb.)\n        Neue Werte eintragen\n\nZusammenfassung anzeigen:\n               Zusammenfass.\n               (Unter Woche -> Zusammenfass.)\n\nWoche wechseln:\n       Andere Woche.\n       (Unter Woche -> Andere Woche)\n\nBackup erstellen:\n        Backup\n        (Unter Sonstige -> Backup)");

        /// <summary>
        /// Calls window to addBankAmount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToBank_Click(object sender, MouseButtonEventArgs e)
        {
            ToBankWindow w = new ToBankWindow();
            w.Show();
            w.Closed += (x, y) => { FillWeekInfo(); };
        }

        /// <summary>
        /// Makes window moveable
        /// </summary>
        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Calls Mehtod to edit the info bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditInfo_Click(object sender, MouseButtonEventArgs e) => EditInfoBar();

        /// <summary>
        /// Closes Programm and saves Products and Entrys (to be sure)
        /// </summary>
        private void CloseProgramm_CLICK(object sender, RoutedEventArgs e)
        {
            SaveEntrys();//todo delete if implemented with sql
            Environment.Exit(0);
        }

        /// <summary>
        /// Minimizes Programm
        /// </summary>
        private void MinimizeProgramm_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        /// <summary>
        /// Calls function to List all Products
        /// </summary>
        private void ListAllProducts_Click(object sender, MouseButtonEventArgs e) => ListAllProducts();

        /// <summary>
        /// Calls function to add a new Product
        /// </summary>
        private void AddNewProduct_Click(object sender, MouseButtonEventArgs e) => AddNewProduct();

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
        private void ChangeWeek_Click(object sender, MouseButtonEventArgs e) => ChangeWeek();
   
        /// <summary>
        /// Calls the function to add a new entry
        /// </summary>
        private void AddNewEntry_Click(object sender, RoutedEventArgs e) => AddNewEntry();

        #region WhatDayIsChecked

        private void MondayChecked(object sender, RoutedEventArgs e)
        {
            CurrDay = DayOfTheWeek.Monday;
            DayOutput.Text = "Montag";
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void TuesdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Dienstag";
            CurrDay = DayOfTheWeek.Tuesday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void WednsdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Mittwoch";
            CurrDay = DayOfTheWeek.Wednesday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void ThursdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Donnerstag";
            CurrDay = DayOfTheWeek.Thursday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void FridayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Freitag";
            CurrDay = DayOfTheWeek.Friday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void SaturdayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Samstag";
            CurrDay = DayOfTheWeek.Saturday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        private void SundayChecked(object sender, RoutedEventArgs e)
        {
            DayOutput.Text = "Sonntag";
            CurrDay = DayOfTheWeek.Sunday;
            FillEntryTable();
            ReCalcAndUpdateInfoLine();
        }

        #endregion

        /// <summary>
        /// Displays the credits
        /// </summary>
        private void Credits_Click(object sender, MouseButtonEventArgs e) => MessageBox.Show(
            "Made by: Lukas Struckl\nSpecial Thanks to: Lennart Putz\n\nAll rights are withheld from the developers\n\nDo not distribute this Programm without the permission of the developers\n\nE-Mail: struckl.lukas@gmail.com");

        /// <summary>
        /// Calls window to edit entry
        /// </summary>
        private void EditTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //string weekNr, string name, DateTime exTime, double lastCashDesks
            EditDataGrid ed = new EditDataGrid();
            ed.Show();

            ed.Closed += (x, y) =>
            {
                string nr = CurrWeek.WeekNr;
                CurrWeek = new Week(nr);
                GenerateDaysOfThWeekAndAddToWeek();
                LoadWeek(nr);
                FillEntryTable();
                ReCalcAndUpdateInfoLine();
            };
        }

        /// <summary>
        /// Calls functiion to backup files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackUp_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;
            FolderBrowserDialog ofd = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.Desktop};
            string path ="";

            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(ofd.SelectedPath)) return;
            path = ofd.SelectedPath;

            if (t.Text.Contains("Lade"))
                LoadBackUp(path);
            else
                MakeBackUp(path);
        }

        /// <summary>
        /// Expands else dropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandElse(object sender, MouseButtonEventArgs e) => ElseComboBox.IsDropDownOpen = true;

        /// <summary>
        /// Expands Info dropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownExpandInfo(object sender, MouseButtonEventArgs e) => InfoComboBox.IsDropDownOpen = true;

        #endregion

        #region Saves

        /// <summary>
        /// Saves Backup
        /// </summary>
        /// <param name="path"></param>
        private void MakeBackUp(string path)
        {
            try
            {
                string source = Directory.GetCurrentDirectory() + @"\Data";
                foreach (string newPath in Directory.GetFiles(source, "*", SearchOption.TopDirectoryOnly))
                {
                    File.Copy(newPath,newPath.Replace(source,path),true);
                }

                MessageBox.Show("Backup erstellt!");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                SaveErrorMsg(ex);
            }
        }

        /// <summary>
        /// Loads Backup
        /// </summary>
        /// <param name="path"></param>
        private void LoadBackUp(string path)
        {
            try
            {
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly))
                {
                    File.Copy(file, file.Replace(path, Directory.GetCurrentDirectory() + @"\Data"), true);
                }

                MessageBox.Show("Backup geladen!");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                SaveErrorMsg(ex);
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
                        writer.WriteLine("[" + d.Name + "]"); //Saves the ´Name of the Day
                        foreach (Entry e in d.Entrys)
                        {
                            writer.WriteLine(e.ProductForEntry.Id + ";" + e.Amount + ";" +
                                             e.AmountOnTheHouse); //Saves the single Entrys of that day
                        }
                    }

                    writer.WriteLine("[info]"); //saves for the info
                    writer.Write(CurrWeek.ExactTime + ";" + CurrWeek.EditName + ";" + CurrWeek.LastChashDesk);

                    foreach (Day d in CurrWeek.DaysInWeek)
                    {
                        writer.Write(";" + d.ToBank); //the ToBank values for each day
                    }

                    writer.WriteLine("\n[end]"); //signalizes the finish of the document
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

                        switch (headLine) //sets the current day (if info sets the info and stops the programm)
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
                                if (splittedLine != null)
                                {
                                    CurrWeek.ExactTime = Convert.ToDateTime(splittedLine[0]);
                                    CurrWeek.EditName = splittedLine[1];
                                    CurrWeek.LastChashDesk = Convert.ToInt32(splittedLine[2]);
                                    CurrWeek.GetCurrentDay().SetBankAmount(Convert.ToDouble(splittedLine[3]));
                                    for (int i = 0; i < CurrWeek.DaysInWeek.Count; i++)
                                        CurrWeek.DaysInWeek[i].SetBankAmount(Convert.ToDouble(splittedLine[i + 3]));
                                }

                                break;
                        }

                        SplitAndAddLoadedEntry(
                            line); //calls a functione to splitup a line and add it to the correct day
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
                CurrDay = DayOfTheWeek.Monday; //sets the current day to Monday and checks it
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
                        Entry e = new Entry(p, Convert.ToInt32(splittedLine[1]), Convert.ToInt32(splittedLine[2]),
                            Convert.ToInt32(splittedLine[1]) * p.Price);
                        CurrWeek.GetCurrentDayAndAddEntry(e);
                        break;
                    }
                }
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

    #endregion

    #region SQL 

        /// <summary>
        /// loads products into programm
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                string query = "SELECT * FROM product";
                CreateConnection();

                MySqlCommand commandDatabase = new MySqlCommand(query, _connection) {CommandTimeout = 60};
                _connection.Open();

                MySqlDataReader reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string name = reader.GetString(1);
                        string price = reader.GetString(2);
                        string tax = reader.GetString(3);
                        string amount = reader.GetString(4);
                        string kindOfAmount = reader.GetString(5);
                        string group = reader.GetString(6);
                        Products.Add(new Product(name, Convert.ToDouble(price), Convert.ToInt32(tax),
                            Convert.ToDouble(amount), kindOfAmount, Convert.ToInt32(id), group));
                    }
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                SaveErrorMsg(ex);
                Log(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new Connection
        /// </summary>
        protected static void CreateConnection() => _connection = new MySqlConnection(ConnectionString);

        /// <summary>
        /// Closes the connection
        /// </summary>
        protected static void CloseConnection() => _connection.Close();
        #endregion

        #region Animations

        /// <summary>
        /// Animates the Rectangle from LightGRay to DarkGray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseEnterChangeColorFromDarkToGray(object sender, MouseEventArgs e)
        {
            Rectangle r = (Rectangle) sender;

            r.Fill = new SolidColorBrush(Colors
                .DarkGray); //If I don´t have this this shit does not work but i have no idea why (Appereantly need a SolidColorBrush first even if it has it already from the xml)

            r.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetAnimFromGrayToDarkGray());
        }

        /// <summary>
        /// Animates the Rectangle from DarkGray to LightGray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeaveChangeColorFromGrayToDarkGray(object sender, MouseEventArgs e)
        {
            Rectangle r = (Rectangle) sender;

            r.Fill = new SolidColorBrush(Colors
                .Gray); //If I don´t have this this shit does not work but i have no idea why

            r.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetAnimFromDarkToGray());
        }

        /// <summary>
        /// Animates the Rectangle from LightGRay to DarkGray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseEnterChangeColorFromLightToDarkGray(object sender, MouseEventArgs e)
        {
            Rectangle r = (Rectangle) sender;

            r.Fill = new SolidColorBrush(Colors
                .LightGray); //If I don´t have this this shit does not work but i have no idea why

            r.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetAnimFromLightoToDarkGray());
        }

        /// <summary>
        /// Animates the Rectangle from DarkGray to LightGray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeaveChangeColorFromDarkToLightGray(object sender, MouseEventArgs e)
        {
            Rectangle r = (Rectangle) sender;

            r.Fill = new SolidColorBrush(Colors
                .DarkGray); //If I don´t have this this shit does not work but i have no idea why

            r.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetAnimFromDarkToLightkGray());
        }

        /// <summary>
        /// Generates an animation from LightGray to DarkGray
        /// </summary>
        /// <returns></returns>
        private ColorAnimation GetAnimFromLightoToDarkGray()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Colors.LightGray,
                To = Colors.DarkGray,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            return animation;
        }

        /// <summary>
        /// Generates an animation from DarkGray to LightGray
        /// </summary>
        /// <returns></returns>
        private ColorAnimation GetAnimFromDarkToLightkGray()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Colors.DarkGray,
                To = Colors.LightGray,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            return animation;
        }

        /// <summary>
        /// Generates an animation from LightGray to DarkGray
        /// </summary>
        /// <returns></returns>
        private ColorAnimation GetAnimFromDarkToGray()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Colors.LightGray,
                To = Colors.DarkGray,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            return animation;
        }

        /// <summary>
        /// Generates an animation from DarkGray to LightGray
        /// </summary>
        /// <returns></returns>
        private ColorAnimation GetAnimFromGrayToDarkGray()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Colors.DarkGray,
                To = Colors.LightGray,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            return animation;
        }



        #endregion
    }
}