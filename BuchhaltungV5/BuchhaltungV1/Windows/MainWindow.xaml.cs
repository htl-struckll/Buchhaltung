using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;
using Hash = BCrypt.Net.BCrypt; //Overwritting the bcypt

namespace BuchhaltungV4
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region var
        public string Username = "";
        public int IsAdmin = 1;
        private const string ConnectionString = Buchhaltung.ConnectionString;
        private static readonly MySqlConnection Connection = new MySqlConnection(ConnectionString);
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            LookForWeeks();
            AnimateAndCheck();
            Topmost = true;
        }

        #region Event´s
        /// <summary>
        /// Login Key down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Login();
        }

        /// <summary>
        /// Login click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e) => Login();

        /// <summary>
        /// Calls the function to start the prorgramm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartProgramm_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = true; //Only important when called by ChangeWeek from Buchhaltungs
            StartProgramm();
        }


        /// <summary>
        /// Starts Programm on Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartProgramm();
        }


        /// <summary>
        /// Updates week nr in textBox from selected DropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateWeekNrTxtBox(object sender, SelectionChangedEventArgs e)
        {
            string fullWeek = (string) ComboOfWeeks.SelectedValue;
            string[] splittedWeekNr = fullWeek.Split('.');
            WeekNrTextBox.Text = splittedWeekNr[0];
        }

        /// <summary>
        /// Closes Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.CloseWindow = false;
            Close();
        }

        /// <summary>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        /// <summary>
        /// makes the window moveable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        /// <summary>
        /// Calls AnimateAndCheck function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckWeek_txtChanged(object sender, TextChangedEventArgs e) => AnimateAndCheck();

        /// <summary>
        /// Expands week
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandWeek(object sender, MouseEventArgs e) => DatePicker.IsDropDownOpen = true;

        /// <summary>
        /// Updates Date with the selected Date from the Picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDate(object sender, SelectionChangedEventArgs e) => DateTextBox.Text =
            Convert.ToString(Convert.ToDateTime(DatePicker.SelectedDate).ToShortDateString());

        /// <summary>
        /// Checks if input is a number with RegEx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewWeekNr(object sender, TextCompositionEventArgs e) =>
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);

        #endregion

        #region Check´s

        /// <summary>
        /// Checks if given login information is correct and returns value
        /// </summary>
        /// <param name="name">Username input</param>
        /// <param name="pwd">Password input</param>
        /// <returns>true if given login infomation is correct</returns>
        private bool CheckLoginInfo(string name, char[] pwd)
        {
            try
            {
                string query = "SELECT password FROM user WHERE username LIKE @name";
                MySqlCommand cmd = new MySqlCommand(query, Connection) {CommandTimeout = 60};

                Connect();

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Prepare();

                string retVal = cmd.ExecuteScalar() == null ? "." : cmd.ExecuteScalar().ToString();

                CloseConnection();

                return !retVal.Equals(".") && Hash.CheckPassword(GetStringFromChar(pwd), retVal);
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }

            return false;
        }

        /// <summary>
        /// Checks if user wrote something into the fields
        /// </summary>
        /// <returns>true if input is ok</returns>
        private bool CheckInput() => UsernameInput.Text.Length > 0 && PasswordInput.Password.Length > 0;

        /// <summary>
        /// Looks if File Exists When not it expands additional info
        /// </summary>
        private void AnimateAndCheck()
        {
            if (IsAdmin.Equals(0)) return; //returns if user is not an admin

            Expander ex = NewWeekExpander;
            if (WeekNrTextBox.Text != "" && File.Exists(@"Data\" + WeekNrTextBox.Text + @".week") ||
                WeekNrTextBox.Text == "")
            {
                DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));

                ex.BeginAnimation(OpacityProperty, animation);
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));

                NewWeekExpander.IsExpanded = true;

                ex.BeginAnimation(OpacityProperty, animation);
            }
        }
        #endregion

        #region sql

        /// <summary>
        /// Opens the sql connection
        /// </summary>
        private void Connect() => Connection.Open();

        /// <summary>
        /// Closes the sql connection
        /// </summary>
        private void CloseConnection() => Connection.Close();

        #endregion

        #region misc
        /// <summary>
        /// Gets if the user is an admin
        /// </summary>
        /// <param name="name"></param>
        /// <returns>1 if admin/0 if user/2 if error</returns>
        private int GetIsAdmin(string name)
        {
            try
            {
                string query = "SELECT isAdmin FROM user WHERE @name LIKE username";
                MySqlCommand cmd = new MySqlCommand(query, Connection) { CommandTimeout = 60 };

                Connect();

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Prepare();

                string retVal = cmd.ExecuteScalar().ToString();

                CloseConnection();

                return Convert.ToInt32(retVal);
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }

            return 2;
        }

        /// <summary>
        /// Gets the string from a char array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string GetStringFromChar(char[] array)
        {
            string retVal = "";
            foreach (char c in array)
            {
                retVal += c;
            }

            return retVal;
        }

        /// <summary>
        /// Sets the window to a logged in stage
        /// </summary>
        public void SetLoggedIn()
        {
            LoginGrid.Opacity = 0;
            LoginGrid.IsHitTestVisible = false;
            MainGrid.Opacity = 100;
        }
        /// <summary>
        /// Fills the comboBox
        /// </summary>
        private void LookForWeeks()
        {
            for (int i = 0; i < 53; i++)
            {
                if (File.Exists(@"Data\" + i + @".week"))
                {
                    if (!ComboOfWeeks.Items.Contains(i))
                        ComboOfWeeks.Items.Add(i + ". Woche");
                }
            }
        }
        #endregion

        /// <summary>
        /// Starts the programm with the data needed
        /// </summary>
        private void StartProgramm()
        {
            try
            {
                if (Convert.ToInt32(WeekNrTextBox.Text) > 0 && Convert.ToInt32(WeekNrTextBox.Text) < 56) //Week in year
                {
                    if (File.Exists(@"Data\" + WeekNrTextBox.Text + @".week")) //Checks if the week already existst
                    {//todo You see if the week exists When it exists Count(weekNr) do this / if not do the other thing and handle that shit in the Buchhaltung.cs
                        Buchhaltung b = new Buchhaltung(WeekNrTextBox.Text,Username, IsAdmin);
                        b.Show();
                    }
                    else
                    {
                        //If one if is not correct writes error and returns
                        if (OldCashDesk.Text.Length == 0) 
                        {
                            Buchhaltung.Log("Kein alter Kassenstand eingetragen!");
                            return;
                        }if (NameInput.Text.Length == 0)
                        {
                            Buchhaltung.Log("Kein Name eingetragen!");
                            return;
                        } //end of return if

                        string oldCashDeskStr = OldCashDesk.Text;
                        if (oldCashDeskStr.Contains("€"))
                            oldCashDeskStr = oldCashDeskStr.Replace('€', ' ').Trim();
                        Buchhaltung b = new Buchhaltung(WeekNrTextBox.Text, NameInput.Text,
                            Convert.ToDateTime(DateTextBox.Text), Convert.ToDouble(oldCashDeskStr),Username,IsAdmin);
                        b.Show();
                    }
                    Close();
                }
                else
                    Buchhaltung.Log("Ungültige Woche");
            }
            catch (Exception e)
            {
                Buchhaltung.SaveErrorMsg(e);
                Buchhaltung.Log(e.Message);
            }
        }

        /// <summary>
        /// Logs you in with the correct login information
        /// </summary>
        private void Login()
        {
            if (CheckInput())
            {
                if (CheckLoginInfo(UsernameInput.Text, PasswordInput.Password.ToCharArray())) { 
                    SetLoggedIn();
                    IsAdmin = GetIsAdmin(UsernameInput.Text);
                    Username = UsernameInput.Text;

                    if(IsAdmin.Equals(2))
                        Buchhaltung.Log("Es ist ein error bei der Admin authentifizierung aufgetreten! Sie sind jetzt ein User.");
                }
                else
                    Buchhaltung.Log("Es konnte keine Benutzername/Passwort kombination gefunden werden");
            }
            else
                Buchhaltung.Log("Keine vollständige Eingabe");
        }
    }
}