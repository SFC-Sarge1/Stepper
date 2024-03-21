using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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

namespace Stepper
{
    /// <summary>
    /// Interaction logic for StepperAppSettings.xaml
    /// </summary>
    public partial class StepperAppSettings : Window
    {
        public StepperAppSettings()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            VersionTxt.Text = "Version: " + displayableVersion;

            // Iterate over each setting
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                // Create a label and a textbox for the setting
                Border labelUserAppSettingsWithBorder = new()
                {
                    BorderBrush = Brushes.White,  // Color of the border
                    BorderThickness = new Thickness(1),  // Thickness of the border
                    Child = new Label()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Content = currentProperty.Name,
                    }
                };
                TextBox UserAppSettings = new()
                {
                    Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                    BorderBrush = Brushes.White,  // Color of the border
                    BorderThickness = new Thickness(2),  // Thickness of the border
                    HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                    Width = 200,  // Width of the TextBox
                    Height = 25,   // Height of the TextBox
                    Text = Properties.Settings.Default[currentProperty.Name].ToString()
                };
                // When the textbox loses focus, update the setting
                UserAppSettings.LostFocus += (sender, args) =>
                {
                    if (int.TryParse(UserAppSettings.Text, out int newIntValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newIntValue;
                        Properties.Settings.Default.Save();
                    }
                    else if (bool.TryParse(UserAppSettings.Text, out bool newBoolValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newBoolValue;
                        Properties.Settings.Default.Save();
                    }
                    else if (decimal.TryParse(UserAppSettings.Text, out decimal newDecimalValue))
                    {
                        Properties.Settings.Default[currentProperty.Name] = newDecimalValue;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default[currentProperty.Name] = UserAppSettings.Text;
                        Properties.Settings.Default.Save();
                    }
                };
                // Add the label and textbox to the WrapPanel
                MySettings.Children.Add(labelUserAppSettingsWithBorder);
                MySettings.Children.Add(UserAppSettings);
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
