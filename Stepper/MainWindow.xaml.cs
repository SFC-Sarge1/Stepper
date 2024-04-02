// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 04-02-2024
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This allows you to control the X,Y and Z axis</summary>
// ***********************************************************************

namespace Stepper
{
    using System.Windows;
    using System.IO.Ports;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using System.Reflection;
    using System.Windows.Threading;
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// The timer
        /// </summary>
        public static DispatcherTimer? timer;
        /// <summary>
        /// The countdown time
        /// </summary>
        public TimeSpan countdownTime = new();
        /// <summary>
        /// The target end time
        /// </summary>
        public DateTime targetEndTime = new();
        /// <summary>
        /// The stopwatch
        /// </summary>
        public Stopwatch stopwatch = new();
        /// <summary>
        /// The elapsed time
        /// </summary>
        TimeSpan elapsedTime = new();
        /// <summary>
        /// The remaining time
        /// </summary>
        public TimeSpan remainingTime = new();
        /// <summary>
        /// The sp
        /// </summary>
        public SerialPort sp = new();
        /// <summary>
        /// My port name
        /// </summary>
        public string myPortName; // Serial Port Name (COM1, COM2, COM3, etc.)
        /// <summary>
        /// The zero xaxis
        /// </summary>
        public int zeroXaxis = Properties.Settings.Default.zeroXaxis;
        /// <summary>
        /// The zero yaxis
        /// </summary>
        public int zeroYaxis = Properties.Settings.Default.zeroYaxis;
        /// <summary>
        /// The zero zaxis
        /// </summary>
        public int zeroZaxis = Properties.Settings.Default.zeroZaxis;
        /// <summary>
        /// The axis
        /// </summary>
        public string axis = Properties.Settings.Default.RootAxisX.ToString();
        /// <summary>
        /// The current x axis
        /// </summary>
        public string currentXAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The current y axis
        /// </summary>
        public string currentYAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The current z axis
        /// </summary>
        public string currentZAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The previous x axis
        /// </summary>
        public string previousXAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The previous y axis
        /// </summary>
        public string previousYAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The previous z axis
        /// </summary>
        public string previousZAxis = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The xaxis changed
        /// </summary>
        public bool XaxisChanged = Properties.Settings.Default.XaxisChanged;
        /// <summary>
        /// The yaxis changed
        /// </summary>
        public bool YaxisChanged = Properties.Settings.Default.YaxisChanged;
        /// <summary>
        /// The zaxis changed
        /// </summary>
        public bool ZaxisChanged = Properties.Settings.Default.ZaxisChanged;
        /// <summary>
        /// The xaxis stepper move temporary
        /// </summary>
        public string XaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The yaxis stepper move temporary
        /// </summary>
        public string YaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The zaxis stepper move temporary
        /// </summary>
        public string ZaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The selected item
        /// </summary>
        public string selectedItem;
        /// <summary>
        /// The stepper move
        /// </summary>
        public decimal stepperMove;
        /// <summary>
        /// The logger
        /// </summary>
        public static ILogger _logger;
        /// <summary>
        /// The logger factory
        /// </summary>
        public static ILoggerFactory loggerFactory;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("Stepper.log"))
            {
                File.Delete("Stepper.log");
            }
            // Create an instance of ILoggerFactory (usually done during application startup)
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddProvider(new FileLoggerProvider("Stepper.log")); // Log to a file named "Stepper.log"
            });
            _logger = loggerFactory.CreateLogger<MainWindow>();
            _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Application Started.");
            ResizeMode = ResizeMode.NoResize;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            VersionTxt.Text = "Version: " + displayableVersion;
            Properties.Settings.Default.BuildVersion = "Version: " + displayableVersion;
            _logger.LogInformation(message: "Version:" + displayableVersion);
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval)) // Set the timer to tick every 1 millisecond
            };
            timer.Tick += Timer_Tick; // Specify what happens when the timer ticks
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.XaxisStepperCurrent.ToString();
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.YaxisStepperCurrent.ToString();
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.ZaxisStepperCurrent.ToString();
            txtXaxisMotorSpeed.Text = Properties.Settings.Default.XaxisMotorSpeed.ToString();
            txtYaxisMotorSpeed.Text = Properties.Settings.Default.YaxisMotorSpeed.ToString();
            txtZaxisMotorSpeed.Text = Properties.Settings.Default.ZaxisMotorSpeed.ToString();
            txtXaxisStepperMove.Text = Properties.Settings.Default.XaxisStepperMove.ToString();
            txtYaxisStepperMove.Text = Properties.Settings.Default.YaxisStepperMove.ToString();
            txtZaxisStepperMove.Text = Properties.Settings.Default.ZaxisStepperMove.ToString();
            ckbXaxisResetToZero.IsChecked = Properties.Settings.Default.ckbXaxisResetToZeroIsChecked;
            ckbYaxisResetToZero.IsChecked = Properties.Settings.Default.ckbYaxisResetToZeroIsChecked;
            ckbZaxisResetToZero.IsChecked = Properties.Settings.Default.ckbZaxisResetToZeroIsChecked;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            txtBaudRate.Text = Properties.Settings.Default.BaudRate.ToString();
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string portName in ports)
                {
                    if (portName == "COM1")
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            cmbComPort.Items.Add(portName);
                            _logger.LogInformation(message: $"Available COM ports: {portName}");
                        }
                        catch (IOException ioex)
                        {
                            _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}");
                            //MessageBox.Show($"An error occurred: {ioex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ex.Message}");
                        }
                    }
                }
                cmbComPort.SelectedIndex = 0;
            }
            catch (IOException ioex)
            {
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ex.Message}");
            }
            if (cmbComPort.SelectedItem == null)
            {
                MessageBox.Show("No COM ports available!\nPlease connect Arduino Uno Device using USB cable and try again!", $"{Properties.Settings.Default.AppName} Board Missing on COM Port!", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            selectedItem = cmbComPort.SelectedItem.ToString();
            if (selectedItem == Properties.Settings.Default.COM4.ToString())
            {
                try
                {
                    cmbComPort.Text = selectedItem;
                    myPortName = selectedItem;
                    sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                    if (sp.IsOpen == false)
                    {
                        sp.Open();
                        _logger.LogInformation(message: $"Opened Serial Port at: {myPortName}, {txtBaudRate.Text}");
                    }
                }
                catch (IOException ioex)
                {
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}");
                    MessageBox.Show($"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                myPortName = selectedItem;
            }
            else if (selectedItem == Properties.Settings.Default.COM5.ToString())
            {
                try
                {
                    myPortName = selectedItem;
                    cmbComPort.Text = selectedItem;
                    sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                    if (sp.IsOpen == false)
                    {
                        sp.Open();
                        _logger.LogInformation(message: $"Opened Serial Port at: {myPortName}, {txtBaudRate.Text}");
                    }
                }
                catch (IOException ioex)
                {
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}");
                    MessageBox.Show($"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (selectedItem == Properties.Settings.Default.COM6.ToString())
            {
                try
                {
                    myPortName = selectedItem;
                    cmbComPort.Text = selectedItem;
                    sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                    if (sp.IsOpen == false)
                    {
                        sp.Open();
                        _logger.LogInformation(message: $"Opened Serial Port at: {myPortName}, {txtBaudRate.Text}");
                    }
                }
                catch (IOException ioex)
                {
                    _logger.LogInformation(message: $"An error occurred: {ioex.Message}");
                    MessageBox.Show($"An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Content = StepperMotorControl;
        }
        /// <summary>
        /// Handles the Click event of the XAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation(message: $"{axis}AxisRun_Click button clicked:");
            try
            {
                axis = Properties.Settings.Default.RootAxisX;
                previousXAxis = txtXaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;
                if (ckbXaxisResetToZero.IsChecked == true)
                {
                    zeroXaxis = 1;
                    stringValue1 = axis + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroXaxis = 0;
                    XZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbXaxisResetToZero.IsChecked == false)
                {
                    stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                        _logger.LogInformation(message: $"{axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    sp.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the YAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                axis = Properties.Settings.Default.RootAxisY;
                _logger.LogInformation(message: $"{axis}AxisRun_Click button clicked:");
                previousYAxis = txtYaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;
                if (ckbYaxisResetToZero.IsChecked == true)
                {
                    zeroYaxis = 1;
                    stringValue1 = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroYaxis = 0;
                    YZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbYaxisResetToZero.IsChecked == false)
                {
                    stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                        _logger.LogInformation(message: $"{axis} Axis negitive value: {txtYaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    sp.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the ZAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                axis = Properties.Settings.Default.RootAxisZ;
                _logger.LogInformation(message: $"{axis}AxisRun_Click button clicked:");
                previousZAxis = txtZaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;
                if (ckbZaxisResetToZero.IsChecked == true)
                {
                    zeroZaxis = 1;
                    stringValue1 = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroZaxis = 0;
                    ZZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbZaxisResetToZero.IsChecked == false)
                {
                    stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    if (Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()));
                        _logger.LogInformation(message: $"{axis} Axis negative value: {txtZaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    sp.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the XYAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                axis = Properties.Settings.Default.RootAxisXY;
                _logger.LogInformation(message: $"{axis}AxisRun_Click button clicked:");
                string stringValue;
                string stringValue1;
                if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
                {
                    zeroXaxis = 1;
                    zeroYaxis = 1;
                    stringValue1 = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroXaxis = 0;
                    zeroYaxis = 0;
                    XYZero(Properties.Settings.Default.Milliseconds, axis);
                }
                else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
                {
                    zeroXaxis = 1;
                    zeroYaxis = 0;
                    stringValue1 = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset X Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroXaxis = 0;
                    zeroYaxis = 0;
                    currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                        _logger.LogInformation(message: $"{axis} Axis negitive value: {txtYaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
                {
                    zeroXaxis = 0;
                    zeroYaxis = 1;
                    stringValue1 = Properties.Settings.Default.RootAxisXY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue1);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Y Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroXaxis = 0;
                    zeroYaxis = 0;
                    currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                        _logger.LogInformation(message: $"{axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
                {
                    zeroXaxis = 0;
                    zeroYaxis = 0;
                    stringValue = Properties.Settings.Default.RootAxisXY + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                    sp.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    _logger.LogInformation(message: $"{axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                    XaxisChanged = false;
                    txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) >= Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()))
                    {
                        if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                        {
                            stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                            _logger.LogInformation(message: $"{axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                        }
                        else
                        {
                            stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                        {
                            stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                            _logger.LogInformation(message: $"{axis} Axis negative value: {txtYaxisStepperMove.Text} converted to positive decimal: {stepperMove}");
                        }
                        else
                        {
                            stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                        }
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(axis, MotorSpeed, stepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    _logger.LogInformation(message: $"{axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    countdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    targetEndTime = DateTime.Now.Add(countdownTime);
                    timer.Start();
                    stopwatch.Start();
                    _logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = targetEndTime: {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// xes the zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="axis">The axis.</param>
        public async void XZero(int myDelay, string axis)
        {
            _logger.LogInformation(message: $"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            _logger.LogInformation(message: $"{axis} Axis Current Location Set to Zero");
            ResetSerialPort();        
        }
        /// <summary>
        /// ies the zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="axis">The axis.</param>
        public async void YZero(int myDelay, string axis)
        {
            _logger.LogInformation(message: $"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            _logger.LogInformation(message: $"{axis} Axis Current Location Set to Zero");
            ResetSerialPort();
        }
        /// <summary>
        /// zs the zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="axis">The axis.</param>
        public async void ZZero(int myDelay, string axis)
        {
            _logger.LogInformation(message: $"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            _logger.LogInformation(message: $"{axis} Axis Current Location Set to Zero");
            ResetSerialPort();
        }
        /// <summary>
        /// Xies the zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        public async void XYZero(int myDelay, string axis)
        {
            _logger.LogInformation(message: $"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            _logger.LogInformation(message: $"{axis} Axis Current Location Set to Zero");
            ResetSerialPort();
        }

        /// <summary>
        /// CheckBoxes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            UpdateZeroStatus();
        }
        /// <summary>
        /// Handles the GotFocus event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void XaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            XaxisStepperMoveTemp = txtXaxisStepperMove.Text.ToString();
        }
        /// <summary>
        /// Handles the GotFocus event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void YaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            YaxisStepperMoveTemp = txtYaxisStepperMove.Text.ToString();
        }
        /// <summary>
        /// Handles the GotFocus event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ZaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            ZaxisStepperMoveTemp = txtZaxisStepperMove.Text.ToString();
        }
        /// <summary>
        /// Handles the TextChanged event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void XaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            XaxisChanged = true;
            if (txtXaxisStepperMove.Text == Properties.Settings.Default.XaxisStepperMove.ToString())
            {
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the XaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void XaxisMotorSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtXaxisMotorSpeed.Text == Properties.Settings.Default.XaxisMotorSpeed.ToString())
            {
                txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void YaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            YaxisChanged = true;
            if (txtYaxisStepperMove.Text == Properties.Settings.Default.YaxisStepperMove.ToString())
            {
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the YaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void YaxisMotorSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtYaxisMotorSpeed.Text == Properties.Settings.Default.YaxisMotorSpeed.ToString())
            {
                txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void ZaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZaxisChanged = true;
            if (txtZaxisStepperMove.Text == Properties.Settings.Default.ZaxisStepperMove.ToString())
            {
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the ZaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void ZaxisMotorSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtZaxisMotorSpeed.Text == Properties.Settings.Default.ZaxisMotorSpeed.ToString())
            {
                txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.Red;
                Properties.Settings.Default.ZaxisMotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void XaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void XaxisStepperMove_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void YaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void YaxisStepperMove_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void ZaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void ZaxisStepperMove_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisStepperMove.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtXaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtXaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtXaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtXaxisMotorSpeed_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtYaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtYaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtYaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtYaxisMotorSpeed_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtZaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtZaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtZaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtZaxisMotorSpeed_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisMotorSpeed.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtXaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtXaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtXaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtXaxisStepperCurrent_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtXaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtYaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtYaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtYaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtYaxisStepperCurrent_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtYaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtZaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtZaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtZaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtZaxisStepperCurrent_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtZaxisStepperCurrent.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtBaudRate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void txtBaudRate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtBaudRate.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"BaudRate mouse controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the txtBaudRate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void txtBaudRate_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                txtBaudRate.Text = mainWindow.Result.ToString();
                _logger.LogInformation(message: $"BaudRate touch controlled Keypad returned: {mainWindow.Result}");
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the txtBaudRate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void txtBaudRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtBaudRate.Text == Properties.Settings.Default.BaudRate.ToString())
            {
                txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                Properties.Settings.Default.BaudRate = Convert.ToInt32(txtBaudRate.Text);
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Handles the SelectionChanged event of the cmbComPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void cmbComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cmbComPort.SelectedIndex = 0;
                selectedItem = cmbComPort.SelectedItem.ToString();
                if (sp.IsOpen == true && selectedItem == Properties.Settings.Default.COM1.ToString())
                {
                    sp.Close();
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Closed Serial Port at: {myPortName}, {txtBaudRate.Text}");
                }
                if (selectedItem == Properties.Settings.Default.COM1.ToString())
                {
                    cmbComPort.Items.MoveCurrentToLast();
                    selectedItem = cmbComPort.SelectedItem.ToString();
                    cmbComPort.SelectedItem = selectedItem;
                    myPortName = selectedItem;
                    cmbComPort.Text = selectedItem;
                    sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                    if (sp.IsOpen == false)
                    {
                        sp.Open();
                        _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Opened Serial Port at: {myPortName}, {txtBaudRate.Text}");
                    }
                }
            }
            catch (IOException ioex)
            {
                _logger.LogInformation(message: $"Stepper Motor Controller An error occurred: {ioex.Message}");
                // Handle the exception
                MessageBox.Show($"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the OnPreviewTextInput event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs" /> instance containing the event data.</param>
        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Use SelectionStart property to find the caret position.
            // Insert the previewed text into the existing text in the textbox.
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            double val;
            // If parsing is successful, set Handled to false
            e.Handled = !double.TryParse(fullText,
                                         NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                                         CultureInfo.InvariantCulture,
                                         out val);
        }

        /// <summary>
        /// Handles the Click event of the AppSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AppSettings_Click(object sender, RoutedEventArgs e)
        {
            StepperAppSettings newSettingsWindow = new StepperAppSettings();
            _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Loading Application Settings form.");
            // Show the new window
            newSettingsWindow.Show();
        }
        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = stopwatch.Elapsed;
            remainingTime = targetEndTime - DateTime.Now;
            if (DateTime.Now < targetEndTime)
            {
                switch (axis)
                {
                    case "X":
                        txtXaxisStepperMove.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        XaxisChanged = true;
                        break;
                    case "Y":
                        txtYaxisStepperMove.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        YaxisChanged = true;
                        break;
                    case "Z":
                        txtZaxisStepperMove.IsEnabled = false;
                        btnRunZAxis.IsEnabled = false;
                        ZaxisChanged = true;
                        break;
                    case "XY":
                        txtXaxisStepperMove.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        txtYaxisStepperMove.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        XaxisChanged = true;
                        YaxisChanged = true;
                        break;
                }
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Disable {axis} Axis controls while moving to location.");
                CountdownLabel.Content = $"Working Timer {elapsedTime.ToString(@"hh\:mm\:ss\.fff")}";
                _logger.LogInformation(message: $"Time remaining: {elapsedTime.ToString(@"hh\:mm\:ss")} targetEndTime = {targetEndTime.ToString(@"hh\:mm\:ss")}");
            }
            if (DateTime.Now >= targetEndTime)
            {
                CountdownLabel.Content = "";
                switch (axis)
                {
                    case "X":
                        txtXaxisStepperMove.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                        txtXaxisStepperCurrent.Text = currentXAxis;
                        XaxisChanged = false;
                        txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "Y":
                        txtYaxisStepperMove.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                        txtYaxisStepperCurrent.Text = currentYAxis;
                        YaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "Z":
                        txtZaxisStepperMove.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                        txtZaxisStepperCurrent.Text = currentZAxis;
                        ZaxisChanged = false;
                        txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.ZaxisMotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtZaxisStepperCurrent.Text);
                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "XY":
                        txtXaxisStepperMove.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                        txtXaxisStepperCurrent.Text = currentXAxis;
                        XaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                        txtYaxisStepperCurrent.Text = currentYAxis;
                        YaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                }
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Enable {axis} Axis controls after moving to location.");
                stopwatch.Stop(); // Stop the timer when the countdown reaches
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Stopwatch Stopped.");
                timer.Stop();
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Timer Stopped.");
                stopwatch.Reset();
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Stopwatch Reset.");
            }
        }
        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} MainWindow loaded");
            txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtBaudRate.BorderBrush = System.Windows.Media.Brushes.White;
            _logger.LogInformation(message: "Set MainWindow Media Brushes to White.");
        }
        /// <summary>
        /// Handles the Closing event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} MainWindow Closing");
                Properties.Settings.Default.Save();
                if (sp.IsOpen == true)
                {
                    sp.Close();
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName}: Closed Serial Port at: {myPortName}, {txtBaudRate.Text}");
                }
            }
            catch (IOException ioex)
            {
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} error occurred: {ioex.Message}");
                MessageBox.Show($"{Properties.Settings.Default.AppName} An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
         
        /// <summary>
        /// Handles the TouchUp event of the cmbComPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void cmbComPort_TouchUp(object sender, TouchEventArgs e)
        {
            cmbComPort.Text = cmbComPort.SelectedItem.ToString();
        }

        /// <summary>
        /// Handles the TouchUp event of the ResetToZero control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void ResetToZero_TouchUp(object sender, TouchEventArgs e)
        {
            UpdateZeroStatus();
        }
        /// <summary>
        /// Updates the zero status.
        /// </summary>
        private void UpdateZeroStatus()
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true && ckbZaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                zeroZaxis = 1;
                _logger.LogInformation(message: "Updated X,Y,Z Zero.IsChecked status to 1.");
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                zeroZaxis = 0;
                _logger.LogInformation(message: "Updated X Zero.IsChecked status to 1 and Y,Z Zero.IsChecked status to 0.");
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                zeroZaxis = 0;
                _logger.LogInformation(message: "Updated X,Z Zero.IsChecked status to 0 and Y Zero.IsChecked status to 1.");
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                zeroZaxis = 1;
                _logger.LogInformation(message: "Updated X,Y Zero.IsChecked status to 0 and Z Zero.IsChecked status to 1.");
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                zeroZaxis = 0;
                _logger.LogInformation(message: "Updated X,Y,Z Zero.IsChecked status to 0.");
            }
        }
        /// <summary>
        /// Updates the motor timer.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="MotorSpeed">The motor speed.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <returns>System.Decimal.</returns>
        private decimal UpdateMotorTimer(string axis, decimal MotorSpeed, decimal stepperMove)
        {
            decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
            if (MotorSpeed <= Convert.ToDecimal(100.00))
            {
                MotorMovementSeconds = (stepperMove / 2);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: {stepperMove} / 2 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(200.00))
            {
                MotorMovementSeconds = (stepperMove / 4);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: {stepperMove} / 4 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(300.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(1.5);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 1.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(400.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 2;
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 2 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(500.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(2.5);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 2.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(600.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 3;
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 3 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(700.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(3.5);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 3.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(800.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 4;
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 4 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(900.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(4.5);
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 4.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(1000.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 5;
                _logger.LogInformation(message: $"{axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 5 = {MotorMovementSeconds}");
            }
            return MotorMovementSeconds;
        }
        /// <summary>
        /// Resets the serial port.
        /// </summary>
        private void ResetSerialPort()
        {
            try
            {
                if (sp.IsOpen == true)
                {
                    sp.Close();
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Closed Serial Port at: {myPortName}, {txtBaudRate.Text}");
                }
                selectedItem = cmbComPort.SelectedItem.ToString();
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                    _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Opened Serial Port at: {myPortName}, {txtBaudRate.Text}");
                }
            }
            catch (IOException ioex)
            {
                _logger.LogInformation(message: $"{Properties.Settings.Default.AppName} Reset Serial Port error occurred: {ioex.Message}");
                MessageBox.Show($"An error occurred: {ioex.Message}", $"{Properties.Settings.Default.AppName} Reset Serial Port Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
