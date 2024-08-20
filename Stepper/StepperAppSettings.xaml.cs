// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 03-20-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 08-10-2024
// ***********************************************************************
// <copyright file="StepperAppSettings.xaml.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Stepper Motor Controller Application Settings.</summary>
// ***********************************************************************
namespace Stepper
{
    using System.Configuration;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interaction logic for StepperAppSettings.xaml
    /// </summary>
    public partial class StepperAppSettings : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepperAppSettings" /> class.
        /// </summary>
        public StepperAppSettings()
        {
            InitializeComponent();
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Window Opened.");
            ResizeMode = ResizeMode.NoResize;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            VersionTxt.Text = $"Version: {displayableVersion}";
            Closing += StepperAppSettings_Closing;
            // Get the children as a list and sort by Name
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings form is rebuilt each and every time it is opened.");
            // Create a label and a textbox for the setting
            Border labelHeaderWithBorder = new()
            {
                BorderBrush = Brushes.White,  // Color of the border
                BorderThickness = new Thickness(1),  // Thickness of the border
                Child = new Label()
                {
                    Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                    HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                    Width = 200,  // Width of the TextBox
                    Height = 30,   // Height of the TextBox
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    FontSize = 16,
                    Content = "Name",

                }
            };
            TextBox UserValue = new()
            {
                Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                BorderBrush = Brushes.White,  // Color of the border
                BorderThickness = new Thickness(2),  // Thickness of the border
                HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                Width = 400,  // Width of the TextBox
                Height = 30,   // Height of the TextBox
                Text = "Value",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                FontSize = 16,
                IsReadOnly = true
            };
            string backupValue = "Value";
            UserValue.TouchDown += (sender, args) =>
            {
                Keypad mainWindow = new(this);
                if (mainWindow.ShowDialog() == true)
                {
                    if (mainWindow.Result == null)
                    {
                        UserValue.Text = backupValue;
                    }
                    else
                    {
                        UserValue.Text = mainWindow.Result.ToString();
                    }
                }
            };
            UserValue.MouseUp += (sender, args) =>
            {
                Keypad mainWindow = new(this);
                if (mainWindow.ShowDialog() == true)
                {
                    if (mainWindow.Result == null)
                    {
                        UserValue.Text = backupValue;
                    }
                    else
                    {
                        UserValue.Text = mainWindow.Result.ToString();
                    }
                }
            };
            UserValue.LostFocus += (sender, args) =>
            {
                MainWindow._logger.LogInformation($"Headers Name and Value added to the Settings Form.");
                Properties.Settings.Default.Save();
                MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
            };
            // Add the label and textbox to the WrapPanel
            MySettings.Children.Add(labelHeaderWithBorder);
            MySettings.Children.Add(UserValue);
            var sortedSettings = Properties.Settings.Default.Properties.Cast<SettingsProperty>().OrderBy(p => p.Name);

            // Iterate over each setting to fill the WrapPanel with the settings
            foreach (SettingsProperty currentProperty in sortedSettings)
            {
                if (int.TryParse(Properties.Settings.Default[propertyName: currentProperty.Name].ToString(), out int newIntValue))
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
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    MainWindow._logger.LogInformation($"int: {newIntValue} {currentProperty.Name} added to the Settings Form.");
                    string backupValue1 = Properties.Settings.Default[propertyName: currentProperty.Name].ToString();
                    UserIntAppSettings.TouchDown += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserIntAppSettings.Text = backupValue1;
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
                        Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToInt32(UserIntAppSettings.Text);
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"int: {UserIntAppSettings.Text} {currentProperty.Name} Saved to the Settings.");
                    };
                }
                else if (bool.TryParse(Properties.Settings.Default[propertyName: currentProperty.Name].ToString(), out bool newBoolValue))
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
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Convert.ToBoolean(Properties.Settings.Default[propertyName: currentProperty.Name].ToString())
                    };
                    MainWindow._logger.LogInformation($"bool: {newBoolValue} {currentProperty.Name} added to the Settings Form.");
                    bool backupValue2 = Convert.ToBoolean(Properties.Settings.Default[propertyName: currentProperty.Name].ToString());
                    // Add items to the ComboBox
                    UserBoolAppSettings.Items.Add(false);
                    UserBoolAppSettings.Items.Add(true);
                    // When the textbox loses focus, update the setting
                    UserBoolAppSettings.LostFocus += (sender, args) =>
                    {
                        if (UserBoolAppSettings.SelectedItem.ToString() == null)
                        {
                            UserBoolAppSettings.SelectedItem = backupValue2;
                            Properties.Settings.Default[propertyName: currentProperty.Name] = backupValue2;
                            Properties.Settings.Default.Save();
                            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        }
                        else
                        {
                            Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToBoolean(UserBoolAppSettings.SelectedItem);
                            Properties.Settings.Default.Save();
                            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                            MainWindow._logger.LogInformation($"bool: {UserBoolAppSettings.SelectedItem} {currentProperty.Name} Saved to the Settings.");
                        }
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelBoolUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserBoolAppSettings);
                }
                else if (decimal.TryParse(Properties.Settings.Default[propertyName: currentProperty.Name].ToString(), out decimal newDecimalValue))
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
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                    };
                    MainWindow._logger.LogInformation($"decimal: {newDecimalValue} {currentProperty.Name} added to the Settings Form.");
                    string backupValue3 = Properties.Settings.Default[propertyName: currentProperty.Name].ToString();
                    UserDecimalAppSettings.TouchDown += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserDecimalAppSettings.Text = backupValue3;
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
                        Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToDecimal(UserDecimalAppSettings.Text);
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"decimal: {UserDecimalAppSettings.Text} {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelDecimalUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserDecimalAppSettings);
                }
                else if (currentProperty.Name.ToString() == "XComPort")
                {
                    // Create a label and a textbox for the setting
                    Border labelXComUserAppSettingsWithBorder = new()
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
                    ComboBox UserXComAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    for (int i = 1; i <= 10; i++)
                    {
                        string comPort = $"COM{i}";
                        UserXComAppSettings.Items.Add(comPort);
                        MainWindow._logger.LogInformation($"X{comPort}: {currentProperty.Name} added to the Settings Form.");
                    }
                    // When the textbox loses focus, update the setting
                    UserXComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserXComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"XComPort: values for {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelXComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserXComAppSettings);
                }
                else if (currentProperty.Name.ToString() == "YComPort")
                {
                    // Create a label and a textbox for the setting
                    Border labelYComUserAppSettingsWithBorder = new()
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
                    ComboBox UserYComAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    for (int i = 1; i <= 10; i++)
                    {
                        string comPort = $"COM{i}";
                        UserYComAppSettings.Items.Add(comPort);
                        MainWindow._logger.LogInformation($"Y{comPort}: {currentProperty.Name} added to the Settings Form.");
                    }
                    // When the textbox loses focus, update the setting
                    UserYComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserYComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"YComPort: values for {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelYComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserYComAppSettings);
                }
                else if (currentProperty.Name.ToString() == "ZComPort")
                {
                    // Create a label and a textbox for the setting
                    Border labelZComUserAppSettingsWithBorder = new()
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
                    ComboBox UserZComAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    for (int i = 1; i <= 10; i++)
                    {
                        string comPort = $"COM{i}";
                        UserZComAppSettings.Items.Add(comPort);
                        MainWindow._logger.LogInformation($"Z{comPort}: {currentProperty.Name} added to the Settings Form.");
                    }
                    // When the textbox loses focus, update the setting
                    UserZComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserZComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"ZComPort: values for {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelZComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserZComAppSettings);
                }
                else if (currentProperty.Name.ToString().ToUpper().Contains("COM"))
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
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    for (int i = 1; i <= 10; i++)
                    {
                        string comPort = $"COM{i}";
                        UserComAppSettings.Items.Add(comPort);
                        MainWindow._logger.LogInformation($"{comPort}: {currentProperty.Name} added to the Settings Form.");
                    }
                    // When the textbox loses focus, update the setting
                    UserComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"ZComPort: values for {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserComAppSettings);
                }
                else if (currentProperty.Name.GetType() == typeof(string))
                {
                    Border labelStringWithBorder = new()
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
                    TextBox UserStringValue = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 400,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                    };
                    string backupStringValue = "StringValue";
                    UserValue.TouchDown += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserStringValue.Text = backupStringValue;
                            }
                            else
                            {
                                UserStringValue.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserStringValue.MouseUp += (sender, args) =>
                    {
                        Keypad mainWindow = new(this);
                        if (mainWindow.ShowDialog() == true)
                        {
                            if (mainWindow.Result == null)
                            {
                                UserStringValue.Text = backupStringValue;
                            }
                            else
                            {
                                UserStringValue.Text = mainWindow.Result.ToString();
                            }
                        }
                    };
                    UserStringValue.LostFocus += (sender, args) =>
                    {
                        //Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToInt32(UserIntAppSettings.Text);
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"int: {UserStringValue.Text} {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelStringWithBorder);
                    MySettings.Children.Add(UserStringValue);
                }
            }

        }

        /// <summary>
        /// Handles the Closing event of the StepperAppSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void StepperAppSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved and Window Closing.");
        }
        /// <summary>
        /// Handles the Click event of the SaveSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            MySettings.Children.Cast<SettingsProperty>().OrderBy(Children => Children.Name);
            //Properties.Settings.Default.Properties.Cast<SettingsProperty>().OrderBy(setting => setting.Name);
            // Get the children as a list and sort by Name
            Properties.Settings.Default.Save();
            MainWindow.timer.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval)); // Set the timer to tick every 1 millisecond
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved.");
            Close();
        }
    }
}
