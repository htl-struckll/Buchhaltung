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
using System.IO;
using System.Threading;

/*
 * Design überarbeiten (  this.Topmost = true;)
 */
namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für Buchhaltung.xaml
    /// </summary>
    public partial class Buchhaltung : Window
    {
        public static List<Product> products = new List<Product>();
        public static string nameOfProduct, priceOfProduct, taxOfProduct, amountOfProduct, kindOfAmount;
        public static Week currWeek;
        public static DayOfTheWeek currDay;
        public static List<DataBoxForGrid> db = new List<DataBoxForGrid>(); //For editing to have references
        public static bool closeWindow = false;

        public Buchhaltung(string weekNr)
        {
            InitializeComponent();
            LoadProducts();

            currWeek = new Week(weekNr); //Selects the current week

            GenerateDaysOfThWeekAndAddToWeek();

            LoadWeek(weekNr); //Needs to be loaded after Products 

            dayOutput.Text = "Montag";

                  
        }

        //gives the choice between yes and no
        public static bool ShowYesNoMessageBox(string text, string caption)
        {
            MessageBoxResult result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                return true;
            return false;
        }

        public void FillEntrys()
        {
            TableDay.Items.Clear();
            TableDay.Items.Refresh();
            db.Clear();
            int counter;
            foreach (Day d in currWeek.DaysInWeek)
            {
                counter = 0;
                if (d.Name == currDay)
                {
                    foreach (Entry e in d.Entrys)
                    {
                        counter++;
                        var data = new DataBoxForGrid(e.ProductForEntry.ID, e.ProductForEntry.Name, e.Amount, e.AmountOnTheHouse, e.ProductForEntry.Price, e.Price, counter, currDay,e.ProductForEntry.Amount,e.ProductForEntry.KindOfAmount);
                        db.Add(data);
                        TableDay.Items.Add(data);
                    }
                    break;
                }
            }
        }

        //Generates all the days in the week (empty)
        private void GenerateDaysOfThWeekAndAddToWeek()
        {//There ist a better solution (Gernerating dynamic) Fuck BackEnd
            for (int i = 0; i < 7; i++)
            {
                Day d = new Day((DayOfTheWeek)i);
                currWeek.AddDay(d);
            }
            currDay = DayOfTheWeek.monday;
        }

        //Calls AddProduct window and Adds the input to the list
        private void AddNewProduct()
        {
            NewProduct np = new NewProduct();
            np.Show();
            np.Closed += (a, b) =>
            {
                LoadProducts();
            };
        }

        //Shows all products
        private void ListAllProducts()
        {
            OutputOfProducts o = new OutputOfProducts();
            o.Show();
        }

        //Opens a window for a new entry
        private void AddNewEntry()
        {
            NewEntry ne = new NewEntry();
            ne.Show();
            ne.Closed += (x, y) =>
                FillEntrys();
        }

        #region Events

        //This fcking shit sucks so much dick i want to hang myself PLZ KILL IT ALREADY
        private void OnChecked(object sender,RoutedEventArgs e)
        {
            MessageBox.Show("test");
        }

        //Makes window moveble
        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //Closes Porgramm
        private void CloseProgramm_CLICK(object sender, RoutedEventArgs e)
        {
            //Call save funcition
            SaveProducts();
            Environment.Exit(0);
        }

        //Minimites Programm
        private void MinimiteProgramm_CLICK(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //Calls function to show all Products located in the text file
        private void ListAllProducts_Click(object sender, MouseButtonEventArgs e)
        {
            ListAllProducts();
        }

        //Calls function to add a new Product
        private void AddNewProduct_Click(object sender, MouseButtonEventArgs e)
        {
            AddNewProduct();
         
        }
        //Calls function of all entrys
        private void ShowAllEntrys_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show( currWeek.ToString());
        }
        //gets next week
        private void ChangeWeek_Click(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();

            mw.Closed += (x,y) =>
            {
                if(closeWindow)
                 this.Close();
                closeWindow = true;
            };
        }

        //Call function to add a new entry to the table
        private void AddNewEntry_Click(object sender, RoutedEventArgs e)
        {
            AddNewEntry();
        }

        private void MondayChecked(object sender, RoutedEventArgs e)
        {
            currDay = DayOfTheWeek.monday;
            dayOutput.Text = "Montag";
            FillEntrys();
        }

        private void TuesdayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Dienstag";
            currDay = DayOfTheWeek.tuesday;
            FillEntrys();
        }

        private void WednsdayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Mittwoch";
            currDay = DayOfTheWeek.wednsday;
            FillEntrys();
        }

        private void ThursdayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Donnerstag";
            currDay = DayOfTheWeek.thursday;
            FillEntrys();
        }

        private void FridayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Freitag";
            currDay = DayOfTheWeek.friday;
            FillEntrys();
        }

        private void SaturdayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Samstag";
            currDay = DayOfTheWeek.saturday;
            FillEntrys();
        }

        private void SundayChecked(object sender, RoutedEventArgs e)
        {
            dayOutput.Text = "Sonntag";
            currDay = DayOfTheWeek.sunday;
            FillEntrys();
        }
        //Shows Credits
        private void Credits_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Made by: Lukas Struckl\nSpecial Thanks to: Lennart Putz");
        }

        //Calls function to edit the table
        private void EditTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditDataGrid ed = new EditDataGrid();
            ed.Show();

            ed.Closed += (x, y) =>
            {
                FillEntrys();
            };
        }

        //Calls function to make backUp
        private void BackUp_Click(object sender, MouseButtonEventArgs e)
        {
            MakeBackUp();
        }

        //FillsEntrys
      
        #endregion

        #region Saves
        
        //Generates a backup
        private void MakeBackUp()
        {
            BackUpInput bp = new BackUpInput();
            bp.Show();
        }

        //Saves all Products
        public static void SaveProducts()
        {
            using (StreamWriter writer = new StreamWriter(@"Data\products.p", append: false))
            {
                for (int i = 0; i < products.Count; i++)
                {
                    writer.Write(products[i].ID + ";" + products[i].Name + ";" + products[i].Price + ";" + products[i].Tax + ";" + products[i].Amount + ";" + products[i].KindOfAmount + "\n");
                }
            }
        }
        //Saves ErrorMsg
        static public void SaveErrorMsg(Exception e)
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

        //Saves all Entrys in the current week
        static public void SaveEntrys()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(@"Data\" + currWeek.WeekNr + ".week", append: false))
                {
                    foreach (Day d in currWeek.DaysInWeek)
                    {
                        writer.WriteLine("[" + d.Name + "]");
                        foreach (Entry e in d.Entrys)
                        {
                            writer.WriteLine(e.ProductForEntry.ID + ";" + e.Amount + ";" + e.AmountOnTheHouse);
                        }
                    }
                    writer.Write("[end]");
                }
                MessageBox.Show("Erfolgreich gespeichert!");
            }
            catch (Exception e)
            {
                Log(e.Message);
                SaveErrorMsg(e);
            }
        }
        #endregion

        #region Loads and Splits
        //Loads the week files
        public void LoadWeek(string weekName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(@"Data\" + weekName + ".week"))
                {
                    string line = "[monday]";
                    while (line != "[end]")
                    {
                        //Change to switsches and cases
                        if (line == "[monday]")
                        {
                            currDay = DayOfTheWeek.monday;
                            while (line != "[tuesday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[tuesday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[tuesday]")
                        {
                            currDay = DayOfTheWeek.wednsday;
                            while (line != "[wednesday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[wednesday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[wednesday]")
                        {
                            currDay = DayOfTheWeek.wednsday;
                            while (line != "[thursday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[thursday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[thursday]")
                        {
                            currDay = DayOfTheWeek.thursday;
                            while (line != "[firday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[friday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[friday]")
                        {
                            currDay = DayOfTheWeek.friday;
                            while (line != "[saturday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[saturday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[saturday]")
                        {
                            currDay = DayOfTheWeek.thursday;
                            while (line != "[sunday]" && line != "[end]")
                            {
                                line = reader.ReadLine();
                                if (line != "[sunday]")
                                    SplitLoadedEntry(line);
                            }
                        }
                        else if (line == "[sunday]")
                        {
                            currDay = DayOfTheWeek.sunday;
                            while (line != "[end]")
                            {
                                line = reader.ReadLine();
                            }
                        }
                    }
                }
                Log("Alle Einträge wurden geladen!");
                currDay = DayOfTheWeek.monday;
                FillEntrys();
            }
            catch (Exception e)
            {
                Log("Fehler beim Laden der Entrys: " + e.Message + e.StackTrace);
                SaveErrorMsg(e);
            }
        }

        //Splits the Line which is loaded from the week files
        private void SplitLoadedEntry(string line)
        {
            if (line != null)
            {
                string[] splittedLine = line.Split(';');

                foreach (Product p in products)
                {
                    if (Convert.ToString(p.ID) == splittedLine[0])
                    {
                       //DBG  MessageBox.Show(p.Id + " | " + (splittedLine[1]) + " | " + (splittedLine[2]) + " | " + Convert.ToString(Convert.ToInt32(splittedLine[1]) * p.Price)); 
                        Entry e = new Entry(p, Convert.ToInt32(splittedLine[1]), Convert.ToInt32(splittedLine[2]), Convert.ToInt32(splittedLine[1]) * p.Price); 
                        currWeek.GetCurrentDayAndAddEntry(e);
                        break;
                    }
                }
            }
        }

        //LoadsProducts
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
                products.Clear();
                for (int i = 0; i < firstSplit.Length - 1; i++)
                {
                    string[] secondSplit = firstSplit[i].Split(';');
                    if (secondSplit[i] != "")
                    {
                        Product p = new Product(secondSplit[1], Convert.ToDouble(secondSplit[2]), Convert.ToInt32(secondSplit[3]), Convert.ToDouble(secondSplit[4]), secondSplit[5], Convert.ToInt32(secondSplit[0]) - 1);
                        products.Add(p);
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
        static public void Log(string msg)
        {
            MessageBox.Show("[ " + DateTime.Now + " ] " + msg);
        }
        #endregion
    }
}