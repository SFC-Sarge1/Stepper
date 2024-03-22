namespace Stepper
{
using System.Configuration;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            // Iterate over each setting to fill the WrapPanel with the settings
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                if (int.TryParse(Properties.Settings.Default[currentProperty.Name].ToString(), out int newIntValue))
                {
                    // Create a label and a textbox for the setting
                    Border labelIntUserAppSettingsWithBorder = new()
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
                    TextBox UserIntAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[currentProperty.Name].ToString()
                    };
                    string backupValue = Properties.Settings.Default[currentProperty.Name].ToString();
                    UserIntAppSettings.TouchDown += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserIntAppSettings.Text = backupValue;
                            }
                            else
                            {
                                UserIntAppSettings.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserIntAppSettings.MouseUp += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserIntAppSettings.Text = backupValue;
                            }
                            else
                            {
                                UserIntAppSettings.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserIntAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[currentProperty.Name] = Convert.ToInt32(UserIntAppSettings.Text);
                        Properties.Settings.Default.Save();
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelIntUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserIntAppSettings);
                }
                else if (bool.TryParse(Properties.Settings.Default[currentProperty.Name].ToString(), out bool newBoolValue))
                {
                    // Create a label and a textbox for the setting
                    Border labelBoolUserAppSettingsWithBorder = new()
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
                    ComboBox UserBoolAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[currentProperty.Name].ToString(),
                        SelectedItem = Convert.ToBoolean(Properties.Settings.Default[currentProperty.Name].ToString())
                    };
                    bool backupValue = Convert.ToBoolean(Properties.Settings.Default[currentProperty.Name].ToString());
                    // Add items to the ComboBox
                    UserBoolAppSettings.Items.Add(false);
                    UserBoolAppSettings.Items.Add(true);
                    // When the textbox loses focus, update the setting
                    UserBoolAppSettings.LostFocus += (sender, args) =>
                    {
                        if (UserBoolAppSettings.SelectedItem.ToString() == null)
                        {
                            UserBoolAppSettings.SelectedItem = backupValue;
                            Properties.Settings.Default[currentProperty.Name] = backupValue;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            Properties.Settings.Default[currentProperty.Name] = Convert.ToBoolean(UserBoolAppSettings.SelectedItem); 
                            Properties.Settings.Default.Save();
                        }
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelBoolUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserBoolAppSettings);
                }
                else if (decimal.TryParse(Properties.Settings.Default[currentProperty.Name].ToString(), out decimal newDecimalValue))
                {
                    // Create a label and a textbox for the setting
                    Border labelDecimalUserAppSettingsWithBorder = new()
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
                    TextBox UserDecimalAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[currentProperty.Name].ToString(),
                    };
                    string backupValue = Properties.Settings.Default[currentProperty.Name].ToString();
                    UserDecimalAppSettings.TouchDown += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserDecimalAppSettings.Text = backupValue;
                            }
                            else
                            {
                                UserDecimalAppSettings.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserDecimalAppSettings.MouseUp += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserDecimalAppSettings.Text = backupValue;
                            }
                            else
                            {
                                UserDecimalAppSettings.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserDecimalAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[currentProperty.Name] = Convert.ToDecimal(UserDecimalAppSettings.Text);
                        Properties.Settings.Default.Save();
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelDecimalUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserDecimalAppSettings);
                }
                else if (Properties.Settings.Default[currentProperty.Name].ToString().Contains("COM"))
                {
                    // Create a label and a textbox for the setting
                    Border labelComUserAppSettingsWithBorder = new()
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
                    ComboBox UserComAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    UserComAppSettings.Items.Add("COM1");
                    UserComAppSettings.Items.Add("COM2");
                    UserComAppSettings.Items.Add("COM3");
                    UserComAppSettings.Items.Add("COM4");
                    UserComAppSettings.Items.Add("COM5");
                    UserComAppSettings.Items.Add("COM6");
                    UserComAppSettings.Items.Add("COM7");
                    UserComAppSettings.Items.Add("COM8");
                    UserComAppSettings.Items.Add("COM9");
                    // When the textbox loses focus, update the setting
                    UserComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[currentProperty.Name] = UserComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserComAppSettings);
                }
                else
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
                    // Create a label and a ComboBox for the setting
                    Border labelComUserAppSettingsWithBorder = new()
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
                    ComboBox UserAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 200,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    UserAppSettings.Items.Add("X");
                    UserAppSettings.Items.Add("Y");
                    UserAppSettings.Items.Add("Z");
                    UserAppSettings.Items.Add("XY");
                    // When the textbox loses focus, update the setting
                    UserAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[currentProperty.Name] = UserAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserAppSettings);
                }
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            MainWindow.timer.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval)); // Set the timer to tick every 1 millisecond

            this.Close();
        }
    }
}
