// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 03-20-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 03-30-2024
// ***********************************************************************
// <copyright file="StepperAppSettings.xaml.cs"company="Computer Question">
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
        /// Initializes a new instance of the <see cref="StepperAppSettings"/> class.
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

            // Iterate over each setting to fill the WrapPanel with the settings
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
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
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    MainWindow._logger.LogInformation($"int {newIntValue} {currentProperty.Name} added to the Settings Form.");
                    string backupValue = Properties.Settings.Default[propertyName: currentProperty.Name].ToString();
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
                        Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToInt32(UserIntAppSettings.Text);
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"int {UserIntAppSettings.Text} {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelIntUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserIntAppSettings);
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
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Convert.ToBoolean(Properties.Settings.Default[propertyName: currentProperty.Name].ToString())
                    };
                    MainWindow._logger.LogInformation($"bool {newBoolValue} {currentProperty.Name} added to the Settings Form.");
                    bool backupValue = Convert.ToBoolean(Properties.Settings.Default[propertyName: currentProperty.Name].ToString());
                    // Add items to the ComboBox
                    UserBoolAppSettings.Items.Add(false);
                    UserBoolAppSettings.Items.Add(true);
                    // When the textbox loses focus, update the setting
                    UserBoolAppSettings.LostFocus += (sender, args) =>
                    {
                        if (UserBoolAppSettings.SelectedItem.ToString() == null)
                        {
                            UserBoolAppSettings.SelectedItem = backupValue;
                            Properties.Settings.Default[propertyName: currentProperty.Name] = backupValue;
                            Properties.Settings.Default.Save();
                            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        }
                        else
                        {
                            Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToBoolean(UserBoolAppSettings.SelectedItem);
                            Properties.Settings.Default.Save();
                            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                            MainWindow._logger.LogInformation($"bool {UserBoolAppSettings.SelectedItem} {currentProperty.Name} Saved to the Settings.");
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
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                    };
                    MainWindow._logger.LogInformation($"decimal {newDecimalValue} {currentProperty.Name} added to the Settings Form.");
                    string backupValue = Properties.Settings.Default[propertyName: currentProperty.Name].ToString();
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
                        Properties.Settings.Default[propertyName: currentProperty.Name] = Convert.ToDecimal(UserDecimalAppSettings.Text);
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"decimal {UserDecimalAppSettings.Text} {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelDecimalUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserDecimalAppSettings);
                }
                else if (Properties.Settings.Default[propertyName: currentProperty.Name].ToString().Contains("COM"))
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
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    UserComAppSettings.Items.Add("COM1");
                    MainWindow._logger.LogInformation($"COM1 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM2");
                    MainWindow._logger.LogInformation($"COM2 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM3");
                    MainWindow._logger.LogInformation($"COM3 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM4");
                    MainWindow._logger.LogInformation($"COM4 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM5");
                    MainWindow._logger.LogInformation($"COM5 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM6");
                    MainWindow._logger.LogInformation($"COM6 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM7");
                    MainWindow._logger.LogInformation($"COM7 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM8");
                    MainWindow._logger.LogInformation($"COM8 {currentProperty.Name} added to the Settings Form.");
                    UserComAppSettings.Items.Add("COM9");
                    MainWindow._logger.LogInformation($"COM9 {currentProperty.Name} added to the Settings Form.");
                    // When the textbox loses focus, update the setting
                    UserComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"COM values for {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelComUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserComAppSettings);
                }
                else if (Properties.Settings.Default[propertyName: currentProperty.Name].ToString().Contains("Version"))
                {
                    // Create a label and a textbox for the setting
                    Border labelStringUserAppSettingsWithBorder = new()
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
                    TextBox UserStringAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                    };
                    MainWindow._logger.LogInformation($"Version Number: {currentProperty.Name} added to the Settings Form.");
                    UserStringAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserStringAppSettings.Text;
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"Version {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelStringUserAppSettingsWithBorder);
                    MySettings.Children.Add(UserStringAppSettings);
                }
                else
                {
                    // Create a label and a textbox for the setting
                    Border labelRootComUserApp = new()
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
                    ComboBox UserRootComAppSettings = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0), // Margin of the Label
                        BorderBrush = Brushes.White,  // Color of the border
                        BorderThickness = new Thickness(2),  // Thickness of the border
                        HorizontalAlignment = HorizontalAlignment.Left, // Align the label to the left
                        Width = 250,  // Width of the TextBox
                        Height = 25,   // Height of the TextBox
                        Text = Properties.Settings.Default[propertyName: currentProperty.Name].ToString(),
                        SelectedItem = Properties.Settings.Default[propertyName: currentProperty.Name].ToString()
                    };
                    // Add items to the ComboBox
                    UserRootComAppSettings.Items.Add("X");
                    MainWindow._logger.LogInformation($"X: {currentProperty.Name} added to the Settings Form.");
                    UserRootComAppSettings.Items.Add("Y");
                    MainWindow._logger.LogInformation($"Y: {currentProperty.Name} added to the Settings Form.");
                    UserRootComAppSettings.Items.Add("Z");
                    MainWindow._logger.LogInformation($"Z: {currentProperty.Name} added to the Settings Form.");
                    UserRootComAppSettings.Items.Add("XY");
                    MainWindow._logger.LogInformation($"XY: {currentProperty.Name} added to the Settings Form.");
                    // When the textbox loses focus, update the setting
                    UserRootComAppSettings.LostFocus += (sender, args) =>
                    {
                        Properties.Settings.Default[propertyName: currentProperty.Name] = UserRootComAppSettings.SelectedItem.ToString();
                        Properties.Settings.Default.Save();
                        MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved after control lost focus.");
                        MainWindow._logger.LogInformation($"Axis Values: {currentProperty.Name} Saved to the Settings.");
                    };
                    // Add the label and textbox to the WrapPanel
                    MySettings.Children.Add(labelRootComUserApp);
                    MySettings.Children.Add(UserRootComAppSettings);
                }
            }
        }
        /// <summary>
        /// Handles the Closing event of the StepperAppSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void StepperAppSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved and Window Closing.");
        }
        /// <summary>
        /// Handles the Click event of the SaveSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            MainWindow.timer.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval)); // Set the timer to tick every 1 millisecond
            MainWindow._logger.LogInformation("Stepper Motor Controller Application Settings Saved.");
            Close();
        }
    }
}
