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

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaction logic for BackUpInput.xaml
    /// </summary>
    public partial class BackUpInput : Window
    {
        public BackUpInput()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            if(inputPath.Text != "")
                MakeBackUp(inputPath.Text);
        }

        private void StandartClick(object sender, RoutedEventArgs e)
        {
            MakeBackUp("BackUp");
        }

        private void MakeBackUp(string path)
        {
            try
            {
                for (int i = 0; i < 53; i++)
                {
                    //gets the whole week and saves it
                    if (File.Exists(@"Data\" + i + @".week"))
                    {
                        string text = "";
                        using (StreamReader reader = new StreamReader(@"Data\" + i + @".week"))
                        {
                            text = reader.ReadToEnd();
                        }

                        using (StreamWriter writer = new StreamWriter(path + @"\" + i + @".week", append: false))
                        {
                            writer.Write(text);
                        }
                    }
                }

                //gets the products and saves them
                string textPr = "";
                using (StreamReader reader = new StreamReader(@"Data\products.p"))
                {
                    textPr = reader.ReadToEnd();
                }

                using (StreamWriter writer = new StreamWriter(path + @"\products.p", append: false))
                {
                    writer.Write(textPr);
                }

                MessageBox.Show("BackUp erfolgreich erstellt!");
                this.Close();
            }
            catch (Exception ex)
            {
                Buchhaltung.Log(ex.Message);
                Buchhaltung.SaveErrorMsg(ex);
            }
        }
    }
}
