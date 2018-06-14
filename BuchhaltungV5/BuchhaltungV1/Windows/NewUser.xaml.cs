using System;
using System.Windows;
using BuchhaltungV4;
using MySql.Data.MySqlClient;

namespace BuchhaltungV1.Windows
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        #region Var´s
        private int _id = 0;
        private const string ConnectionString = Buchhaltung.ConnectionString;
        private static MySqlConnection _connection;
        #endregion

        public NewUser(int id)
        {
            InitializeComponent();
            Topmost = true;

            _id = id;
            FillFields(id);
        }

        

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;
            char[] pwd = PasswordInput.Password.ToCharArray(), pwdRetype = PasswordRetypeInput.Password.ToCharArray();
            bool isAdmin = IsAdminCheckBox.IsChecked != null && (bool) IsAdminCheckBox.IsChecked;

            //return ifs
            if (CheckUsernameExists(name))
            {
                Buchhaltung.Log("Name '" + name + "' already exists");
                return;
            }

            if (!new string(pwd).Equals(new string(pwdRetype)))
            {
                Buchhaltung.Log("Passwords do not match");
                return;
            }
            //end of return if

            SaveNewUser(_id,name,pwd,isAdmin);

            Close();
        }

        #region Checks

        /// <summary>
        /// Checks if username already exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckUsernameExists(string name) => GetUsernameCount(name) == 1;

        #endregion

        #region Window
        /// <summary>
        /// fiells all fields
        /// </summary>
        /// <param name="id"></param>
        private void FillFields(int id)
        {
            IdOutput.Text = id.ToString();
        }
        #endregion

        #region sql

        /// <summary>
        /// Makes new user
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="name">Username</param>
        /// <param name="pwd">Password</param>
        /// <param name="isAdmin">Is admin</param>
        private void SaveNewUser(int id,string name, char[] pwd, bool isAdmin)
        {
            try
            {
                string query =
                    "INSERT INTO `user`(`id`, `username`, `password`, `isAdmin`) VALUES (@id,@name,@pwd,@isAdmin)";

                CreateConnection();
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@pwd", new string(pwd));
                cmd.Parameters.AddWithValue("@isAdmin", isAdmin);
                cmd.Prepare();

                cmd.ExecuteScalar();

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
        }

        /// <summary>
        /// Gets the amount of usernames
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Should be 1</returns>
        private int GetUsernameCount(string name)
        {
            try
            {
                string query = "SELECT COUNT(id) FROM user WHERE username LIKE @name";

                CreateConnection();
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Prepare();

                object retVal = cmd.ExecuteScalar();

                CloseConnection();

                return Convert.ToInt32(retVal);
            }
            catch (Exception ex)
            {
                Buchhaltung.SaveErrorMsg(ex);
                Buchhaltung.Log(ex.Message);
            }
            return 0;
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
