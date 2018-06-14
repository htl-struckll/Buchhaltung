using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BuchhaltungV4;
using MySql.Data.MySqlClient;

namespace BuchhaltungV1.Windows
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        #region Var´s
        private static List<User> _users;
        private const string ConnectionString = Buchhaltung.ConnectionString;
        private static MySqlConnection _connection;
        #endregion

        public UserWindow()
        {
            InitializeComponent();
            Topmost = true;

            _users = new List<User>();
            GetUsers();
            FillTable();
        }


        /// <summary>
        /// sets the item source
        /// </summary>
        private void FillTable()
        {
            UserTable.ItemsSource = _users;
        }


        #region Event´s

        /// <summary>
        /// Search function / Gets called when text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UserTable.Items.Filter = filteredData =>
                {
                    if (SearchBox.Text.Equals(""))
                        return true;
                    if (SearchBox.Text.ToLower().Trim().Equals("admin") && filteredData is User uA && uA.IsAdmin)
                        return true;

                    return filteredData is User p && p.Name.ToString() == SearchBox.Text.Trim()
                           | (Convert.ToString(p.Name).ToLower().Contains(SearchBox.Text.ToLower()));
                };
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }

        /// <summary>
        /// Hides ghost text if textbox got search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            GhostText.Opacity = 0;
        }

        /// <summary>
        /// Shows ghost text if textbox lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Equals(""))
                GhostText.Opacity = 100;
        }

        /// <summary>
        /// Gets called when Table is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void User_TableLoaded(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn userTableColumn in UserTable.Columns)
            {
                userTableColumn.IsReadOnly = true;
            }
            UserTable.Columns[0].Header = "Id";
            UserTable.Columns[1].Header = "Benutzername";
            UserTable.Columns[2].Header = "Admin";
        }

        /// <summary>
        /// Calls edit User window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (UserTable.SelectedItem is User ut)
            {
                EditUser editUser = new EditUser(ut.Name);
                editUser.Show();

                editUser.Closed += (x, y) =>
                {
                    _users = new List<User>();
                    GetUsers();
                    FillTable();
                };
            }
        }
        /// <summary>
        /// Calls the new user window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            NewUser nu = new NewUser(_users[_users.Count-1].Id+1);
            nu.Show();

            nu.Closed += (x, y) =>
            {
                _users = new List<User>();
                GetUsers();
                FillTable();
            };
        }

        /// <summary>
        /// Deletes the selected user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UserTable.SelectedItem is User ut)
            {
                string name = ut.Name;
                //returns if selected name is you
                if (name.Equals(Buchhaltung.Username))
                {
                    Buchhaltung.Log("Du kannst dich nicht selbst löschen");
                    return;
                }

                if (MessageBox.Show("'" + name + "' löschen?", "Sicher löschen", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _users.Remove(ut);
                    RemoveUser(name);
                    UserTable.Items.Refresh();
                }
            }
        }
        #endregion

        #region SQL

        /// <summary>
        /// Gets the usernames
        /// </summary>
        private void GetUsers()
        {
            try
            {
                const string query = "SELECT id,username,isAdmin FROM user";

                CreateConnection();
                _connection.Open();

                MySqlCommand commandDatabase = new MySqlCommand(query, _connection) { CommandTimeout = 60 };

                MySqlDataReader reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string userName = reader.GetString(1);
                        string isAdmin = reader.GetString(2);
                        _users.Add(new User(Convert.ToInt32(id), userName, Convert.ToBoolean(Convert.ToInt32(isAdmin))));
                    }
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }

        /// <summary>
        /// Removes the user
        /// </summary>
        /// <param name="name">Name to remove</param>
        private void RemoveUser(string name)
        {
            try
            {
                string query = "DELETE FROM user WHERE username = '" + name + "'";
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
        /// Creates a new Connection
        /// </summary>
        protected static void CreateConnection() => _connection = new MySqlConnection(ConnectionString);

        /// <summary>
        /// Closes the connection
        /// </summary>
        protected static void CloseConnection() => _connection.Close();

        #endregion
    }
}

