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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace BuchhaltungV1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
           LookForWeeks();
           this.Topmost = true;
        }

        private void LookForWeeks()
        {
            for (int i = 0; i < 53; i++)
            {
                if(File.Exists(@"Data\" + i + @".week"))
                {
                    if (!comboOfWeeks.Items.Contains(i))
                        comboOfWeeks.Items.Add(i + "te Woche");
                }
            }
        }

        private void StartProgramm_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.closeWindow = true;
            StartProgramm();
        }

        private void StartProgramm()
        {
            try
            {
                if (Convert.ToInt32(weekNrTextBox.Text) > 0 && Convert.ToInt32(weekNrTextBox.Text) < 56)
                {
                    if (!File.Exists(@"Data\" + weekNrTextBox.Text + @".week"))
                    {
                        MessageBox.Show("Die Woche wurde nicht gefunden! Es wird eine neue erstellt.");
                        using (StreamWriter writer = new StreamWriter(@"Data\" + weekNrTextBox.Text + @".week"))
                        {
                            writer.Write("[monday]\n[tuesday]\n[wednesday]\n[thursday]\n[saturday]\n[sunday]\n[end]");
                        }
                    }
                    Buchhaltung b = new Buchhaltung(weekNrTextBox.Text);
                    b.Show();
                    this.Close();
                }
                else
                    Buchhaltung.Log("Ungültige Woche");
            }
            catch (Exception e)
            {
                Buchhaltung.Log(e.Message);
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        private void CheckEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                StartProgramm();
        }

        private void UpdateWeekNrTxtBox(object sender, SelectionChangedEventArgs e)
        {
           string fullWeek =  (string)comboOfWeeks.SelectedValue;
            string[] splittedWeekNr = fullWeek.Split('t');
            weekNrTextBox.Text = splittedWeekNr[0];
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.closeWindow = false;
            this.Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Buchhaltung.closeWindow = false;
            this.Close();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Taskbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
