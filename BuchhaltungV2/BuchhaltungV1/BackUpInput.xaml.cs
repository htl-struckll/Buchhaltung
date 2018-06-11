// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackUpInput.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for BackUpInput.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BuchhaltungV1
{
    using System;
    using System.IO;
    using System.Windows;

    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for BackUpInput.xaml
    /// </summary>
    public partial class BackUpInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackUpInput"/> class.
        /// </summary>
        public BackUpInput()
        {
            InitializeComponent();
        }

        #region Events

        //Calls Savefunction  with custom path
        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            if(InputPath.Text != "")
                MakeBackUp(InputPath.Text);
        }

        //Calls Savefunction with standart path
        private void StandartClick(object sender, RoutedEventArgs e)
        {
            MakeBackUp("BackUp");
        }
        

        #endregion

        /// <summary>
        /// Saves All Data into BackUp path
        /// </summary>
        /// <param name="path"></param>
        private void MakeBackUp(string path)
        {
            try
            {
                for (int i = 0; i < 53; i++)
                {
                    //gets the whole week and saves it
                    if (File.Exists(@"Data\" + i + @".week"))
                    {
                        string text;
                        using (StreamReader reader = new StreamReader(@"Data\" + i + @".week")) //reads the file
                        {
                            text = reader.ReadToEnd();
                        }

                        using (StreamWriter writer = new StreamWriter(path + @"\" + i + @".week", append: false)) //saves the file
                        {
                            writer.Write(text);
                        }
                    }
                }

                //gets the products and saves them
                string textPr;
                using (StreamReader reader = new StreamReader(@"Data\products.p")) //reads the file
                {
                    textPr = reader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(path + @"\products.p", append: false)) //saves the file
                {
                    writer.Write(textPr);
                }

                MessageBox.Show("BackUp erfolgreich erstellt!");
                Close();
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }
        }
    }
}
