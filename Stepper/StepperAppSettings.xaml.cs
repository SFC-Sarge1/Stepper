using System;
using System.Collections.Generic;
using System.Configuration;
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
using MahApps.Metro.Controls;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for StepperAppSettings.xaml
    /// </summary>
    public partial class StepperAppSettings : MetroWindow
    {
        public StepperAppSettings()
        {
            InitializeComponent();
            // Create a StackPanel to hold the settings controls
            StackPanel panel = new StackPanel();

            // Iterate over each setting
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                // Create a label and a textbox for the setting
                Label label = new Label() { Content = currentProperty.Name };
                TextBox textBox = new TextBox() { Text = Properties.Settings.Default[currentProperty.Name].ToString() };
                // When the textbox loses focus, update the setting
                textBox.LostFocus += (sender, args) =>
                {
                    if (int.TryParse(textBox.Text, out int newIntValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newIntValue;
                        Properties.Settings.Default.Save();
                    }
                    else if (bool.TryParse(textBox.Text, out bool newBoolValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newBoolValue;
                        Properties.Settings.Default.Save();
                    }
                    else if (decimal.TryParse(textBox.Text, out decimal newDecimalValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newDecimalValue;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default[currentProperty.Name] = textBox.Text;
                        Properties.Settings.Default.Save();
                    }
                };

                // Add the label and textbox to the panel
                panel.Children.Add(label);
                panel.Children.Add(textBox);
            }

            // Create a ScrollViewer
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            // Add the StackPanel to the ScrollViewer
            scrollViewer.Content = panel;

            // Add the ScrollViewer to the window
            this.Content = scrollViewer; 
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void CancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
