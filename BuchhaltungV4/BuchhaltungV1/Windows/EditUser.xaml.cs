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
using BuchhaltungV4;
using MySql.Data.MySqlClient;

namespace BuchhaltungV1.Windows
{
    /// <summary>
    /// Interaction logic for EditUser.xaml
    /// </summary>
    public partial class EditUser : Window
    {
        #region var´s
        private const string ConnectionString = Buchhaltung.ConnectionString;
        private static MySqlConnection _connection;
        private int _id;
        private string _dbName;
        private bool _isAdmin;
        private char[] _pwd;
        private readonly string _oldName;
        #endregion

        public EditUser(string name)
        {
            InitializeComponent();
            Topmost = true;

            GetUser(name);
            FillWindow();
            _oldName = name;
        }

        #region SQL
        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="username"></param>
        private void GetUser(string username)
        {
            try
            {
                const string query = "SELECT id,username,isAdmin FROM user WHERE username LIKE @name";

                CreateConnection();
                _connection.Open();

                MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@name", username);
                cmd.Prepare();

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                         _id = Convert.ToInt32(reader.GetString(0));
                         _dbName = reader.GetString(1);
                         _isAdmin = Convert.ToBoolean(Convert.ToInt32(reader.GetString(2)));
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
        /// Save edited user
        /// </summary>
        private void SaveNewUser(bool pwdChanged)
        {
            try
            {
                string query = pwdChanged ? "UPDATE user SET username=@username,password=@_pwd,isAdmin=@isAdmin WHERE id LIKE @id" : "UPDATE user SET username=@username,isAdmin=@isAdmin WHERE id LIKE @id";

                CreateConnection();
                _connection.Open();

                MySqlCommand cmd = new MySqlCommand(query, _connection);

                if (pwdChanged)
                    cmd.Parameters.AddWithValue("@_pwd", BCrypt.Net.BCrypt.HashPassword(new string(_pwd),BCrypt.Net.BCrypt.GenerateSalt()));
                cmd.Parameters.AddWithValue("@isAdmin", _isAdmin);
                cmd.Parameters.AddWithValue("@id", _id);
                cmd.Parameters.AddWithValue("@username", _dbName);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                Buchhaltung.Log("ERROR: " + ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }
        }

        /// <summary>
        /// Check if username already exists
        /// </summary>
        /// <param name="newName">New username</param>
        /// <returns>true / false</returns>
        private bool CheckIfUserNameExists(string newName)
        {
            try
            {
                CreateConnection();
                const string query = "SELECT COUNT(username)  FROM user WHERE username LIKE @name";

                CreateConnection();
                _connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, _connection);

                cmd.Parameters.AddWithValue("@name", newName);
                cmd.Prepare();

                object retVal = cmd.ExecuteScalar();

                CloseConnection();

                return (retVal.ToString().Equals("0") || _oldName == newName);
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }

            return false;
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

        #region Events
        /// <summary>
        /// Edits the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// sets ghosttext when field is losing focus and no input is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pwd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Password.Equals(""))
                GhostTextPwdInput.Opacity = 100;
            else
                GhostTextPwdRetypeInput.Text = "Passwort eingeben!";
        }

        /// <summary>
        /// hides ghost text when field got focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pwd_GotFocus(object sender, RoutedEventArgs e)
        {
            GhostTextPwdInput.Opacity = 0;
        }

        /// <summary>
        /// sets ghosttext when field is losing focus and no input is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PwdRetype_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordRetypeInput.Password.Equals(""))
                GhostTextPwdRetypeInput.Opacity = 100;
        }

        /// <summary>
        /// hides ghost text when field got focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PwdRetype_GotFocus(object sender, RoutedEventArgs e)
        {
            GhostTextPwdRetypeInput.Opacity = 0;
        }
        #endregion

        #region Window
        /// <summary>
        /// Fills the window with the user information
        /// </summary>
        private void FillWindow()
        {
            IdOutput.Text = Convert.ToString(_id);
            NameInput.Text = _dbName;
            IsAdminCheckBox.IsChecked = _isAdmin;
        }


        #endregion

        #region Checks

        /// <summary>
        /// edits user
        /// </summary>
        private void Edit()
        {
            char[] newPwd = PasswordInput.Password.ToCharArray(), newPwdRetype = PasswordRetypeInput.Password.ToCharArray();
            string newName = NameInput.Text;
            bool isAdmin = Convert.ToBoolean(IsAdminCheckBox.IsChecked);

            if (CheckIfUserNameExists(newName))
            {
                if (CheckInput(newPwd, newPwdRetype))
                {
                    _isAdmin = isAdmin;
                    if (newPwd.Length != 0)
                           _pwd = newPwd;
                    _dbName = newName;
                    SaveNewUser(newPwd.Length != 0);

                    Close();
                }
                else
                    Buchhaltung.Log("Password´s are not equal.");
            }
            else
                Buchhaltung.Log("Username already exists");
        }


        /// <summary>
        /// Checks if _pwd is ok
        /// </summary>
        /// <param name="pwd">Password</param>
        /// <param name="pwdRetyped">Password retyped</param>
        /// <returns>true / false</returns>
        private static bool CheckInput(IReadOnlyCollection<char> pwd, char[] pwdRetyped)
        {
            if (pwdRetyped == null) throw new ArgumentNullException(nameof(pwdRetyped));
            return pwd.ToString().Equals(pwdRetyped.ToString()) || (pwd.Count == 0 && pwdRetyped.Length == 0);
        }
        #endregion
    }
}
