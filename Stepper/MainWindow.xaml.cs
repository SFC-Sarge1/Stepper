// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 08-20-2024
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This allows you to control the X,Y and Z Axis</summary>
// ***********************************************************************

namespace Stepper
{
    using System.Windows;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using System.Reflection;
    using System.Windows.Threading;
    using System.IO;
    using System.IO.Ports;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using System.Diagnostics;
    using System.Xml.Linq;
    using UtilityDelta.Stepper;
    using System.Windows.Markup;
    using System.Threading;
    using System.Timers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private CancellationTokenSource _xCancellationTokenSource = new();
        private CancellationTokenSource _yCancellationTokenSource = new();
        private CancellationTokenSource _zCancellationTokenSource = new();
        public StepperAppSettings NewSettingsWindow { get; private set; }
        public static DispatcherTimer? xTimer { get; private set; }
        public static DispatcherTimer? yTimer { get; private set; }
        public static DispatcherTimer? zTimer { get; private set; }
        public TimeSpan xCountdownTime { get; private set; } = new();
        public TimeSpan yCountdownTime { get; private set; } = new();
        public TimeSpan zCountdownTime { get; private set; } = new();
        public DateTime xTargetEndTime { get; private set; } = new();
        public DateTime yTargetEndTime { get; private set; } = new();
        public DateTime zTargetEndTime { get; private set; } = new();
        public static Stopwatch xStopwatch { get; private set; } = new();
        public static Stopwatch yStopwatch { get; private set; } = new();
        public static Stopwatch zStopwatch { get; private set; } = new();
        public TimeSpan ElapsedTime { get; private set; } = new();
        public TimeSpan RemainingTime { get; private set; } = new();
        public int ZeroXaxis { get; private set; } = Properties.Settings.Default.zeroXaxis;
        public int ZeroYaxis { get; private set; } = Properties.Settings.Default.zeroYaxis;
        public int ZeroZaxis { get; private set; } = Properties.Settings.Default.zeroZaxis;
        public string Axis { get; private set; } = Properties.Settings.Default.RootAxisZ.ToString();
        public string CurrentXAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string CurrentYAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string CurrentZAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string PreviousXAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string PreviousYAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string PreviousZAxis { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public bool XaxisChanged { get; private set; } = Properties.Settings.Default.ZaxisChanged;
        public bool YaxisChanged { get; private set; } = Properties.Settings.Default.ZaxisChanged;
        public bool ZaxisChanged { get; private set; } = Properties.Settings.Default.ZaxisChanged;
        public string XaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string YaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public string ZaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        public decimal StepperMove { get; private set; }
        public decimal xStepperMove { get; private set; }
        public decimal yStepperMove { get; private set; }
        public decimal zStepperMove { get; private set; }
        public static ILogger Logger { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }
        public SerialPort xSerialPort { get; private set; } = new();
        public SerialPort ySerialPort { get; private set; } = new();
        public SerialPort zSerialPort { get; private set; } = new();
        private static byte[] _message = new byte[6000];
        public static float XCurrentPosition { get; private set; }
        public static float YCurrentPosition { get; private set; }
        public static float ZCurrentPosition { get; private set; }
        private bool xAxisRunCompleted = false;
        private bool yAxisRunCompleted = false;
        private bool zAxisRunCompleted = false;
        /// <summary>
        /// The serial port
        /// </summary>
        private SerialPort _XserialPort;
        private SerialPort _YserialPort;
        private SerialPort _ZserialPort;
        /// <summary>
        /// The message
        /// </summary>
        static byte[] message = new byte[6000];
        /// <summary>
        /// The Z Axis Current Position
        /// </summary>
        public static float zCurrentPosition;
        /// <summary>
        /// The Z Axis Absolute Position
        /// </summary>
        public float zAxisAbsolutePosition { get; private set; } = 0.00f;
        public bool xAxisClearAbsolutePosition { get; private set; } = false;
        public bool yAxisClearAbsolutePosition { get; private set; } = false;
        public bool zAxisClearAbsolutePosition { get; private set; } = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();
            InitializeSerialPorts();
            InitializeSettings();
            InitializeLogger();
            InitializeTimers();

            // Get current date and time
            DateTime now = DateTime.Now;

            // Format the date and time in a suitable format for a filename
            string dateTimeString = now.ToString("yyyyMMdd_HHmmss");

            Logger.LogInformation(message: $"Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            Logger.LogInformation(message: $"Version: {displayableVersion}");
            ResizeMode = ResizeMode.NoResize;
#if DEBUG
            int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
            int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;
            int tempRevision = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            int revision = tempRevision;
            if (revision > 10)
            {
                revision = 0;
                build++;
            }
            if (build > 10)
            {
                build = 0;
                minor++;
            }
            if (minor > 10)
            {
                minor = 0;
                major++;
            }
            else
            {
                revision = tempRevision + 1;
            }
            version = new(major, minor, build, revision);
            buildDate = DateTime.Now;
            displayableVersion = $"{major}.{minor}.{build}.{revision} ({buildDate})";
            Logger.LogInformation(message: $"Version: {displayableVersion}");
            VersionTxt.Text = $"Version: {displayableVersion}";
            XDocument doc = XDocument.Load(Properties.Settings.Default.ProjectFilePath);
            // Find the Version element and change its value
            var versionElement = doc.Descendants("AssemblyVersion").FirstOrDefault();
            if (versionElement != null)
            {
                versionElement.Value = $"{major}.{minor}.{build}.{revision}";
            }
            var versionElement1 = doc.Descendants("FileVersion").FirstOrDefault();
            if (versionElement1 != null)
            {
                versionElement1.Value = $"{major}.{minor}.{build}.{revision}";
            }
            // Save the modified XML file
            doc.Save(Properties.Settings.Default.ProjectFilePath);
            Properties.Settings.Default.BuildVersion = $"Version: {displayableVersion}";
            Logger.LogInformation(message: $"Version: {displayableVersion}");
#endif
            zTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval)) // Set the zTimer to tick every 1 millisecond
            };
            zTimer.Tick += Timer_Tick; // Specify what happens when the zTimer ticks
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            NewSettingsWindow = new StepperAppSettings();

        }
        private void InitializeSerialPorts()
        {
            InitializeSerialPort(ref _XserialPort, Properties.Settings.Default.XComPort, XdataReceivedHandler, btnXAxisPort, "X Axis Port");
            InitializeSerialPort(ref _YserialPort, Properties.Settings.Default.YComPort, YdataReceivedHandler, btnYAxisPort, "Y Axis Port");
            InitializeSerialPort(ref _ZserialPort, Properties.Settings.Default.ZComPort, ZdataReceivedHandler, btnZAxisPort, "Z Axis Port");
        }

        private void InitializeSerialPort(ref SerialPort serialPort, string portName, SerialDataReceivedEventHandler dataReceivedHandler, Button portButton, string buttonText)
        {
            serialPort = new SerialPort(portName, Properties.Settings.Default.BaudRate)
            {
                RtsEnable = true,
                DtrEnable = true
            };
            serialPort.DataReceived += dataReceivedHandler;
            OpenSerialPort(serialPort);
            portButton.Content = $"{buttonText} {portName}";
        }
        private void OpenSerialPort(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
                if (serialPort.BytesToRead > 0)
                {
                    serialPort.Read(message, 0, serialPort.BytesToRead);
                    Logger.LogInformation($"SerialPort Read = {serialPort.BytesToRead}");
                }
            }
            catch
            {
                Logger.LogInformation($"SerialPort {serialPort.PortName} not connected.");
            }
        }
        private void InitializeSettings()
        {
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
        }
        private void InitializeLogger()
        {
            string logFileName = "Stepper";
            string fileLogPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), logFileName + ".log");
            string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileLogPathBackup = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{logFileName}_{dateTimeString}.log");

            if (File.Exists(fileLogPath))
            {
                if (File.Exists(fileLogPathBackup))
                {
                    File.Delete(fileLogPathBackup);
                }
                File.Move(fileLogPath, fileLogPathBackup);
                File.Delete(fileLogPath);
            }

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddProvider(new FileLoggerProvider(fileLogPath));
            });
            Logger = LoggerFactory.CreateLogger<MainWindow>();
            Logger.LogInformation("Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            Logger.LogInformation($"Version: {displayableVersion}");
        }
        private void InitializeTimers()
        {
            zTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval))
            };
            zTimer.Tick += Timer_Tick;
        }

        private static void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Xsp = (SerialPort)sender;
                string Xindata = Xsp.ReadExisting();
                Logger.LogInformation(message: $"X Axis Data Received: {Xindata}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in XdataReceivedHandler");
            }
        }
        /// <summary>
        /// Y data received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private static void YdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Ysp = (SerialPort)sender;
                string Yindata = Ysp.ReadExisting();
                Logger.LogInformation(message: $"Y Axis Data Received: {Yindata}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in YdataReceivedHandler");
            }
        }
        private void EnableControls()
        {
            txtXaxisStepperMove.IsEnabled = true;
            txtYaxisStepperMove.IsEnabled = true;
            txtZaxisStepperMove.IsEnabled = true;
            txtXaxisStepperCurrent.IsEnabled = true;
            txtYaxisStepperCurrent.IsEnabled = true;
            txtZaxisStepperCurrent.IsEnabled = true;
            txtXaxisMotorSpeed.IsEnabled = true;
            txtYaxisMotorSpeed.IsEnabled = true;
            txtZaxisMotorSpeed.IsEnabled = true;
            ckbXaxisResetToZero.IsEnabled = true;
            ckbYaxisResetToZero.IsEnabled = true;
            ckbZaxisResetToZero.IsEnabled = true;
            btnRunXAxis.IsEnabled = true;
            btnRunYAxis.IsEnabled = true;
            btnRunZAxis.IsEnabled = true;
            btnRunXYAxis.IsEnabled = true;
        }

        /// <summary>
        /// Z data received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void ZdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Zsp = (SerialPort)sender;
                string Zindata = Zsp.ReadExisting();
                Logger.LogInformation(message: $"Z Axis Data Received: {Zindata}");
                // Check if the message indicates the motor has stopped
                if (Zindata.Contains("Z Axis CW Motor Stopped"))
                {
                    zTimer.Stop();
                    zStopwatch.Stop();
                    // Cancel the delay task
                    _zCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        ZaxisChanged = true;
                        CountdownLabel.Content = "";
                        //ZZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbZaxisResetToZero.IsChecked = true;
                        zAxisClearAbsolutePosition = false;
                        RoutedEventArgs e = new();
                        ZAxisRun_Click(sender, e);
                        try
                        {
                            // Start a new task for the delay
                            Task.Run(async () =>
                            {
                                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay) * 2);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    // Perform any UI updates or further actions after the delay
                                    //MessageBox.Show("Delay completed after motor stop.", "Delay", MessageBoxButton.OK, MessageBoxImage.Information);
                                    // Check if the message contains the current motor position
                                    if (zAxisRunCompleted && Zindata.Contains("Z Axis CW Motor Current Position:"))
                                    {

                                        string[] lines = Zindata.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (string line in lines)
                                        {
                                            if (line.StartsWith("Z Axis CW Motor Current Position:"))
                                            {
                                                string positionString = line.Replace("Z Axis CW Motor Current Position:", "").Trim();
                                                if (float.TryParse(positionString, out float currentPosition))
                                                {
                                                    // Convert steps to mm
                                                    float stepsPerRevolution = 200.0f;
                                                    float distancePerRevolution = 4.0f;
                                                    float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
                                                    zAxisAbsolutePosition = zAxisAbsolutePosition + distanceInMM;
                                                    Application.Current.Dispatcher.Invoke(() =>
                                                    {
                                                        ckbZaxisResetToZero.IsChecked = false;
                                                        txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString("F2");
                                                        // Update the UI or log the current position
                                                        txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2"); // Display distance in mm
                                                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2"));
                                                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                                                        Properties.Settings.Default.Save();
                                                        zAxisRunCompleted = false;
                                                        Logger.LogInformation(message: $"Z Axis CW Motor Current Position: {txtZaxisStepperCurrent.Text}");
                                                        //MessageBox.Show($"Z Axis Motor Current Position: {currentPosition}", "Motor Position", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        zAxisClearAbsolutePosition = true;
                                                        //zAxisAbsolutePosition = 0.00f;
                                                    });
                                                }
                                            }
                                        }
                                    }

                                });
                            });
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error in ZdataReceivedHandler Task.Run(async ()");
                            MessageBox.Show(ex.ToString() + " Error in ZdataReceivedHandler Task.Run(async ()", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        //MessageBox.Show("Z Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }
                if (Zindata.Contains("Z Axis CCW Motor Stopped"))
                {
                    zTimer.Stop();
                    zStopwatch.Stop();
                    // Cancel the delay task
                    _zCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        ZaxisChanged = true;
                        CountdownLabel.Content = "";
                        //ZZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbZaxisResetToZero.IsChecked = true;
                        RoutedEventArgs e = new();
                        ZAxisRun_Click(sender, e);
                        try
                        {
                            // Start a new task for the delay
                            Task.Run(async () =>
                            {
                                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay) * 2);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    // Perform any UI updates or further actions after the delay
                                    //MessageBox.Show("Delay completed after motor stop.", "Delay", MessageBoxButton.OK, MessageBoxImage.Information);
                                    // Check if the message contains the current motor position
                                    if (zAxisRunCompleted && Zindata.Contains("Z Axis CCW Motor Current Position:"))
                                    {

                                        string[] lines = Zindata.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (string line in lines)
                                        {
                                            if (line.StartsWith("Z Axis CCW Motor Current Position:"))
                                            {
                                                string positionString = line.Replace("Z Axis CCW Motor Current Position:", "").Trim();
                                                if (float.TryParse(positionString, out float currentPosition))
                                                {
                                                    // Convert steps to mm
                                                    float stepsPerRevolution = 200.0f;
                                                    float distancePerRevolution = 4.0f;
                                                    float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
                                                    zAxisAbsolutePosition = zAxisAbsolutePosition - distanceInMM;
                                                    Application.Current.Dispatcher.Invoke(() =>
                                                    {
                                                        ckbZaxisResetToZero.IsChecked = false;
                                                        txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
                                                        // Update the UI or log the current position
                                                        txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2"); // Display distance in mm
                                                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2"));
                                                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                                                        Properties.Settings.Default.Save();
                                                        zAxisRunCompleted = false;
                                                        Logger.LogInformation(message: $"Z Axis CCW Motor Current Position: {txtZaxisStepperCurrent.Text}");
                                                        //MessageBox.Show($"Z Axis Motor Current Position: {currentPosition}", "Motor Position", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        zAxisClearAbsolutePosition = true;

                                                    });
                                                }
                                            }
                                        }
                                    }

                                });
                            });
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error in ZdataReceivedHandler Task.Run(async ()");
                            MessageBox.Show(ex.ToString() + " Error in ZdataReceivedHandler Task.Run(async ()", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        //MessageBox.Show("Z Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ZdataReceivedHandler");
                MessageBox.Show(ex.ToString() + " Error in ZdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Determines whether the specified number is negative.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns><c>true</c> if the specified number is negative; otherwise, <c>false</c>.</returns>
        public bool IsNegative(decimal number)
        {
            if (number < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Handles the Click event of the X Axis Run event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            Logger.LogInformation(message: $"{Axis}AxisRun_Click button clicked:");
            try
            {
                Axis = Properties.Settings.Default.RootAxisX;
                PreviousXAxis = txtXaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;

                if (ckbXaxisResetToZero.IsChecked == true)
                {
                    ZeroXaxis = 1;
                    stringValue1 = $"{Axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    _XserialPort.Write(stringValue1);
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroXaxis = 0;
                    XZero(Properties.Settings.Default.Milliseconds, Axis);
                }
                if (ckbXaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{Axis},{(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                    {
                        xStepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                        Logger.LogInformation(message: $"{Axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {xStepperMove}");
                    }
                    else
                    {
                        xStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, xStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    _XserialPort.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    Logger.LogInformation(message: $"{Axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    xCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    xTargetEndTime = DateTime.Now.Add(xCountdownTime);
                    xTimer.Start();
                    xStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = xTargetEndTime: {xTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation(message: $"{Axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{Axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the Y Axis Run event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Axis = Properties.Settings.Default.RootAxisY;
                Logger.LogInformation(message: $"{Axis}AxisRun_Click button clicked:");
                PreviousYAxis = txtYaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;

                if (ckbYaxisResetToZero.IsChecked == true)
                {
                    ZeroYaxis = 1;
                    stringValue1 = $"{Axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    _YserialPort.Write(stringValue1);
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroYaxis = 0;
                    YZero(Properties.Settings.Default.Milliseconds, Axis);
                }
                if (ckbYaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{Axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                    {
                        yStepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                        Logger.LogInformation(message: $"{Axis} Axis negative value: {txtYaxisStepperMove.Text} converted to positive decimal: {yStepperMove}");
                    }
                    else
                    {
                        yStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, yStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    _YserialPort.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    Logger.LogInformation(message: $"{Axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    yCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    yTargetEndTime = DateTime.Now.Add(yCountdownTime);
                    yTimer.Start();
                    yStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = yTargetEndTime: {yTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation(message: $"{Axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{Axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the Z Axis Run event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Axis = Properties.Settings.Default.RootAxisZ;
                Logger.LogInformation(message: $"{Axis}AxisRun_Click button clicked:");
                PreviousZAxis = txtZaxisStepperMove.Text.ToString();
                string stringValue;
                string stringValue1;


                if (ckbZaxisResetToZero.IsChecked == true)
                {
                    ZeroZaxis = 1;
                    stringValue1 = $"{Axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{Properties.Settings.Default.Value_0_00},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    _ZserialPort.Write(stringValue1);
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroZaxis = 0;
                    ZZero(Properties.Settings.Default.Milliseconds, Axis);
                }
                if (ckbZaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{Axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text))},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    if (Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()) < 0)
                    {
                        zStepperMove = Math.Abs(Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()));
                        Logger.LogInformation(message: $"{Axis} Axis negative value: {txtZaxisStepperMove.Text} converted to positive decimal: {zStepperMove}");
                    }
                    else
                    {
                        zStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, zStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    _ZserialPort.Write(stringValue);
                    // Create a new CancellationTokenSource for this operation
                    _zCancellationTokenSource = new CancellationTokenSource();
                    try
                    {
                        await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay), _zCancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        Logger.LogInformation(message: $"{Axis} Axis Run Event: Task was canceled due to limit switch trigger.");
                        return;
                    }
                    Logger.LogInformation(message: $"{Axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    zCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    zTargetEndTime = DateTime.Now.Add(zCountdownTime);
                    zTimer.Start();
                    zStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = zTargetEndTime: {zTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                // Set the flag to indicate completion
                zAxisRunCompleted = true;
            }
            catch (Exception ex)
            {
                Logger.LogInformation(message: $"{Axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{Axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the Click event of the X and Y Axis Run event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Axis = Properties.Settings.Default.RootAxisXY;
                Logger.LogInformation(message: $"{Axis}AxisRun_Click button clicked:");
                string stringValue;
                string stringValue1;

                if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
                {
                    ZeroXaxis = 1;
                    ZeroYaxis = 1;
                    stringValue1 = $"{Axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    ckbXaxisResetToZero.IsChecked = false;
                    ckbYaxisResetToZero.IsChecked = false;
                    _XserialPort.Write(stringValue1);
                    _YserialPort.Write(stringValue1);
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroXaxis = 0;
                    ZeroYaxis = 0;
                    XYZero(Properties.Settings.Default.Milliseconds, Axis);
                }
                else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
                {
                    ZeroXaxis = 1;
                    ZeroYaxis = 0;
                    stringValue1 = $"{Axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    ckbXaxisResetToZero.IsChecked = false;
                    _XserialPort.Write(stringValue1);
                    _YserialPort.Write(stringValue1);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset X Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroXaxis = 0;
                    ZeroYaxis = 0;
                    CurrentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                    {
                        yStepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                        Logger.LogInformation(message: $"{Axis} Axis negative value: {txtYaxisStepperMove.Text} converted to positive decimal: {yStepperMove}");
                    }
                    else
                    {
                        yStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, xStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    xCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    xTargetEndTime = DateTime.Now.Add(xCountdownTime);
                    xTimer.Start();
                    xStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = xTargetEndTime: {xTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
                {
                    ZeroXaxis = 0;
                    ZeroYaxis = 1;
                    stringValue1 = $"{Axis},{(Convert.ToDecimal(txtXaxisStepperCurrent) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{txtXaxisMotorSpeed.Text},{ZeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    ckbYaxisResetToZero.IsChecked = false;
                    _YserialPort.Write(stringValue1);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    Logger.LogInformation(message: $"{Axis} Axis Run Event to reset Y Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    ZeroXaxis = 0;
                    ZeroYaxis = 0;
                    CurrentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                    {
                        yStepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                        Logger.LogInformation(message: $"{Axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {yStepperMove}");
                    }
                    else
                    {
                        yStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, xStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    xCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    xTargetEndTime = DateTime.Now.Add(zCountdownTime);
                    xTimer.Start();
                    xStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = xTargetEndTime: {xTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
                {
                    ZeroXaxis = 0;
                    ZeroYaxis = 0;
                    stringValue = $"{Axis},{(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{ZeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{ZeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{ZeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue);
                    _XserialPort.Write(stringValue);
                    _YserialPort.Write(stringValue);
                    await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                    Logger.LogInformation(message: $"{Axis} Axis Run Event: {stringValue}");
                    stringValue = "";
                    CurrentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                    XaxisChanged = false;
                    txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    CurrentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) >= Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()))
                    {
                        if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                        {
                            yStepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                            Logger.LogInformation(message: $"{Axis} Axis negative value: {txtXaxisStepperMove.Text} converted to positive decimal: {yStepperMove}");
                        }
                        else
                        {
                            yStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                        {
                            yStepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                            Logger.LogInformation(message: $"{Axis} Axis negative value: {txtYaxisStepperMove.Text} converted to positive decimal: {yStepperMove}");
                        }
                        else
                        {
                            yStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                        }
                    }
                    decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
                    decimal MotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    MotorMovementSeconds = UpdateMotorTimer(Axis, MotorSpeed, xStepperMove);
                    int myMovementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(MotorMovementSeconds);
                    Logger.LogInformation(message: $"{Axis} Axis myMovementTimer int: {Properties.Settings.Default.Milliseconds} * {MotorMovementSeconds} = {myMovementTimer}");
                    xCountdownTime = TimeSpan.FromMilliseconds(myMovementTimer);
                    xTargetEndTime = DateTime.Now.Add(xCountdownTime);
                    xTimer.Start();
                    xStopwatch.Start();
                    Logger.LogInformation(message: $"{Axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {myMovementTimer} = xTargetEndTime: {xTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation(message: $"{Axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{Axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Sets the stepper motor X Axis to zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="Axis">The Axis.</param>
        public async void XZero(int myDelay, string Axis)
        {
            Logger.LogInformation(message: $"Setting {Axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            Logger.LogInformation(message: $"{Axis} Axis Current Location Set to Zero");
        }
        /// <summary>
        /// Sets the stepper motor Y Axis to zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="Axis">The Axis.</param>
        public async void YZero(int myDelay, string Axis)
        {
            Logger.LogInformation(message: $"Setting {Axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            Logger.LogInformation(message: $"{Axis} Axis Current Location Set to Zero");
        }
        /// <summary>
        /// Sets the stepper motor Z Axis to zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="Axis">The Axis.</param>
        public async void ZZero(int myDelay, string Axis)
        {
            Logger.LogInformation(message: $"Setting {Axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(myDelay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
            Logger.LogInformation(message: $"{Axis} Axis Current Location Set to Zero");
            if (zAxisClearAbsolutePosition)
            {
                zAxisAbsolutePosition = 0.00f;
            }

        }
        /// <summary>
        /// Sets the stepper motor X and Y Axis to zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="Axis">The Axis.</param>
        public async void XYZero(int myDelay, string Axis)
        {
            Logger.LogInformation(message: $"Setting {Axis} Axis Current Location Set to Zero on DRO");
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
            Logger.LogInformation(message: $"{Axis} Axis Current Location Set to Zero");
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
                try
                {
                    Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"X Axis error occurred: {ex.Message}");
                    MessageBox.Show($"X Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"X Axis error occurred: {ex.Message}");
                    MessageBox.Show($"X Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"Y Axis error occurred: {ex.Message}");
                    MessageBox.Show($"Y Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"Y Axis error occurred: {ex.Message}");
                    MessageBox.Show($"Y Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"Z Axis error occurred: {ex.Message}");
                    MessageBox.Show($"Z Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                try
                {
                    Properties.Settings.Default.ZaxisMotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"Z Axis error occurred: {ex.Message}");
                    MessageBox.Show($"Z Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
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
            Logger.LogInformation(message: $"Stepper Motor Controller Loading Application Settings form.");
            // Show the new window
            NewSettingsWindow.Activate();
            NewSettingsWindow.ShowDialog();

        }
        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            ElapsedTime = zStopwatch.Elapsed;
            RemainingTime = zTargetEndTime - DateTime.Now;
            if (DateTime.Now < zTargetEndTime)
            {
                switch (Axis)
                {
                    case "X":
                        txtXaxisStepperMove.IsEnabled = false;
                        txtYaxisStepperMove.IsEnabled = false;
                        txtZaxisStepperMove.IsEnabled = false;
                        ckbXaxisResetToZero.IsEnabled = false;
                        ckbYaxisResetToZero.IsEnabled = false;
                        ckbZaxisResetToZero.IsEnabled = false;
                        txtXaxisStepperCurrent.IsEnabled = false;
                        txtYaxisStepperCurrent.IsEnabled = false;
                        txtZaxisStepperCurrent.IsEnabled = false;
                        txtXaxisMotorSpeed.IsEnabled = false;
                        txtYaxisMotorSpeed.IsEnabled = false;
                        txtZaxisMotorSpeed.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunZAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        XaxisChanged = true;
                        break;
                    case "Y":
                        txtXaxisStepperMove.IsEnabled = false;
                        txtYaxisStepperMove.IsEnabled = false;
                        txtZaxisStepperMove.IsEnabled = false;
                        ckbXaxisResetToZero.IsEnabled = false;
                        ckbYaxisResetToZero.IsEnabled = false;
                        ckbZaxisResetToZero.IsEnabled = false;
                        txtXaxisStepperCurrent.IsEnabled = false;
                        txtYaxisStepperCurrent.IsEnabled = false;
                        txtZaxisStepperCurrent.IsEnabled = false;
                        txtXaxisMotorSpeed.IsEnabled = false;
                        txtYaxisMotorSpeed.IsEnabled = false;
                        txtZaxisMotorSpeed.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunZAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        YaxisChanged = true;
                        break;
                    case "Z":
                        txtXaxisStepperMove.IsEnabled = false;
                        txtYaxisStepperMove.IsEnabled = false;
                        txtZaxisStepperMove.IsEnabled = false;
                        txtXaxisStepperCurrent.IsEnabled = false;
                        txtYaxisStepperCurrent.IsEnabled = false;
                        txtZaxisStepperCurrent.IsEnabled = false;
                        txtXaxisMotorSpeed.IsEnabled = false;
                        txtYaxisMotorSpeed.IsEnabled = false;
                        txtZaxisMotorSpeed.IsEnabled = false;
                        ckbXaxisResetToZero.IsEnabled = false;
                        ckbYaxisResetToZero.IsEnabled = false;
                        ckbZaxisResetToZero.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunZAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        ZaxisChanged = true;
                        break;
                    case "XY":
                        txtXaxisStepperMove.IsEnabled = false;
                        txtYaxisStepperMove.IsEnabled = false;
                        txtZaxisStepperMove.IsEnabled = false;
                        ckbXaxisResetToZero.IsEnabled = false;
                        ckbYaxisResetToZero.IsEnabled = false;
                        ckbZaxisResetToZero.IsEnabled = false;
                        txtXaxisStepperCurrent.IsEnabled = false;
                        txtYaxisStepperCurrent.IsEnabled = false;
                        txtZaxisStepperCurrent.IsEnabled = false;
                        txtXaxisMotorSpeed.IsEnabled = false;
                        txtYaxisMotorSpeed.IsEnabled = false;
                        txtZaxisMotorSpeed.IsEnabled = false;
                        btnRunXAxis.IsEnabled = false;
                        btnRunYAxis.IsEnabled = false;
                        btnRunZAxis.IsEnabled = false;
                        btnRunXYAxis.IsEnabled = false;
                        XaxisChanged = true;
                        YaxisChanged = true;
                        break;
                }
                if (ckbXaxisResetToZero.IsChecked == false || ckbYaxisResetToZero.IsChecked == false || ckbZaxisResetToZero.IsChecked == false)
                {
                    Logger.LogInformation(message: $"Stepper Motor Controller Disable {Axis} Axis controls while moving to location.");
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} {ElapsedTime.ToString(@"hh\:mm\:ss\.fffff")}";
                    Logger.LogInformation(message: $"Time remaining: {ElapsedTime.ToString(@"hh\:mm\:ss")} zTargetEndTime = {zTargetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                if (ckbXaxisResetToZero.IsChecked == true || ckbYaxisResetToZero.IsChecked == true || ckbZaxisResetToZero.IsChecked == true)
                {
                    ckbXaxisResetToZero.IsChecked = false;
                    ckbYaxisResetToZero.IsChecked = false;
                    ckbZaxisResetToZero.IsChecked = false;
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                }
            }
            if (DateTime.Now >= zTargetEndTime)
            {
                CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                switch (Axis)
                {
                    case "X":
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        CurrentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                        txtXaxisStepperCurrent.Text = CurrentXAxis;
                        XaxisChanged = false;
                        txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "Y":
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        CurrentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                        txtYaxisStepperCurrent.Text = CurrentYAxis;
                        YaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "Z":
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        CurrentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                        txtZaxisStepperCurrent.Text = CurrentZAxis;
                        ZaxisChanged = false;
                        txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.ZaxisMotorSpeed = Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtZaxisStepperCurrent.Text);
                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                    case "XY":
                        txtXaxisStepperMove.IsEnabled = true;
                        txtYaxisStepperMove.IsEnabled = true;
                        txtZaxisStepperMove.IsEnabled = true;
                        ckbXaxisResetToZero.IsEnabled = true;
                        ckbYaxisResetToZero.IsEnabled = true;
                        ckbZaxisResetToZero.IsEnabled = true;
                        txtXaxisStepperCurrent.IsEnabled = true;
                        txtYaxisStepperCurrent.IsEnabled = true;
                        txtZaxisStepperCurrent.IsEnabled = true;
                        txtXaxisMotorSpeed.IsEnabled = true;
                        txtYaxisMotorSpeed.IsEnabled = true;
                        txtZaxisMotorSpeed.IsEnabled = true;
                        btnRunXAxis.IsEnabled = true;
                        btnRunYAxis.IsEnabled = true;
                        btnRunZAxis.IsEnabled = true;
                        btnRunXYAxis.IsEnabled = true;
                        CurrentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                        txtXaxisStepperCurrent.Text = CurrentXAxis;
                        XaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        CurrentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                        txtYaxisStepperCurrent.Text = CurrentYAxis;
                        YaxisChanged = false;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
                        Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                }
                Logger.LogInformation(message: $"Stepper Motor Controller Enable {Axis} Axis controls after moving to location.");
                zStopwatch.Stop(); // Stop the zTimer when the countdown reaches
                Logger.LogInformation(message: $"Stepper Motor Controller Stopwatch Stopped.");
                zTimer.Stop();
                Logger.LogInformation(message: $"Stepper Motor Controller Timer Stopped.");
                zStopwatch.Reset();
                Logger.LogInformation(message: $"Stepper Motor Controller Stopwatch Reset.");
            }
        }
        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.LogInformation(message: $"Stepper Motor Controller MainWindow loaded");
            txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            Logger.LogInformation(message: "Set MainWindow Media Brushes to White.");
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
                Logger.LogInformation(message: $"Stepper Motor Controller MainWindow Closing.");
                _XserialPort.Close();
                _YserialPort.Close();
                _ZserialPort.Close();
                Logger.LogInformation(message: $"Stepper Motor Controller Serial Ports closed.");
                Properties.Settings.Default.Save();
                Logger.LogInformation(message: $"Properties Settings Saved.");
                Logger.LogInformation(message: $"Properties Settings Form Closed.");
                Logger.LogInformation(message: $"Stepper Motor Controller Form Closed.");
                NewSettingsWindow.Close();
            }
            catch (IOException ioex)
            {
                Logger.LogInformation(message: $"Stepper Motor Controller error occurred: {ioex.Message}");
                MessageBox.Show($"Stepper Motor Controller An error occurred: {ioex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                ZeroXaxis = 1;
                ZeroYaxis = 1;
                ZeroZaxis = 1;
                Logger.LogInformation(message: "Updated X,Y,Z Zero.IsChecked status to 1.");
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                ZeroXaxis = 1;
                ZeroYaxis = 0;
                ZeroZaxis = 0;
                Logger.LogInformation(message: "Updated X Zero.IsChecked status to 1 and Y,Z Zero.IsChecked status to 0.");
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true && ckbZaxisResetToZero.IsChecked == false)
            {
                ZeroXaxis = 0;
                ZeroYaxis = 1;
                ZeroZaxis = 0;
                Logger.LogInformation(message: "Updated X,Z Zero.IsChecked status to 0 and Y Zero.IsChecked status to 1.");
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == true)
            {
                ZeroXaxis = 0;
                ZeroYaxis = 0;
                ZeroZaxis = 1;
                Logger.LogInformation(message: "Updated X,Y Zero.IsChecked status to 0 and Z Zero.IsChecked status to 1.");
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                ZeroXaxis = 0;
                ZeroYaxis = 0;
                ZeroZaxis = 0;
                Logger.LogInformation(message: "Updated X,Y,Z Zero.IsChecked status to 0.");
            }
        }
        /// <summary>
        /// Updates the motor zTimer.
        /// </summary>
        /// <param name="Axis">The Axis.</param>
        /// <param name="MotorSpeed">The motor speed.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <returns>System.Decimal.</returns>
        private decimal UpdateMotorTimer(string Axis, decimal MotorSpeed, decimal stepperMove)
        {
            decimal MotorMovementSeconds = Convert.ToDecimal(0.00);
            if (MotorSpeed <= Convert.ToDecimal(100.00))
            {
                MotorMovementSeconds = (stepperMove / 2);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: {stepperMove} / 2 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(200.00))
            {
                MotorMovementSeconds = (stepperMove / 4);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: {stepperMove} / 4 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(300.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(1.5);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 1.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(400.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 2;
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 2 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(500.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(2.5);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 2.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(600.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 3;
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 3 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(700.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(3.5);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 3.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(800.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 4;
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 4 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(900.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / Convert.ToDecimal(4.5);
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 4.5 = {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= Convert.ToDecimal(1000.00))
            {
                MotorMovementSeconds = (stepperMove / 4) / 5;
                Logger.LogInformation(message: $"{Axis} Axis MotorMovementSeconds decimal: ({stepperMove} / 4) / 5 = {MotorMovementSeconds}");
            }
            return MotorMovementSeconds;
        }

        /// <summary>
        /// Handles the Click event of the XAxisPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void XAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _XserialPort.Close();
            if (_XserialPort.IsOpen == false)
            {
                try
                {
                    _XserialPort.PortName = Properties.Settings.Default.XComPort; // Set your port name
                    _XserialPort.BaudRate = Properties.Settings.Default.BaudRate; // Set your baud rate
                    // Enable RTS and DTR
                    _XserialPort.RtsEnable = true;
                    _XserialPort.DtrEnable = true;
                    _XserialPort.DataReceived += new SerialDataReceivedEventHandler(XdataReceivedHandler);
                    _XserialPort.Open();
                }
                catch
                {
                    Logger.LogInformation(message: $"X Axis SerialPort {Properties.Settings.Default.XComPort} not connected.");
                }
            }

            btnXAxisPort.Content = $"X Axis Port {Properties.Settings.Default.XComPort}";
        }

        /// <summary>
        /// Handles the Click event of the YAxisPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void YAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _YserialPort.Close();
            if (_YserialPort.IsOpen == false)
            {
                try
                {
                    _YserialPort.PortName = Properties.Settings.Default.YComPort; // Set your port name
                    _YserialPort.BaudRate = Properties.Settings.Default.BaudRate; // Set your baud rate
                    // Enable RTS and DTR
                    _YserialPort.RtsEnable = true;
                    _YserialPort.DtrEnable = true;
                    _YserialPort.DataReceived += new SerialDataReceivedEventHandler(YdataReceivedHandler);
                    _YserialPort.Open();
                }
                catch
                {
                    Logger.LogInformation(message: $"Y Axis SerialPort {Properties.Settings.Default.YComPort} not connected.");
                }
            }

            btnYAxisPort.Content = $"Y Axis Port {Properties.Settings.Default.YComPort}";
        }

        /// <summary>
        /// Handles the Click event of the ZAxisPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ZAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _ZserialPort.Close();
            if (_ZserialPort.IsOpen == false)
            {
                try
                {
                    int i = 0;
                    _ZserialPort.PortName = Properties.Settings.Default.ZComPort; // Set your port name
                    _ZserialPort.BaudRate = Properties.Settings.Default.BaudRate; // Set your baud rate
                    // Enable RTS and DTR
                    _ZserialPort.RtsEnable = true;
                    _ZserialPort.DtrEnable = true;
                    _ZserialPort.DataReceived += new SerialDataReceivedEventHandler(ZdataReceivedHandler);
                    _ZserialPort.Open();
                    if ((i = _ZserialPort.BytesToRead) > 0)
                    {
                        _ZserialPort.Read(message, 0, i);
                        //Console.WriteLine("Read=" + i.ToString());
                        Logger.LogInformation(message: $"Z Axis SerialPort Read = " + i.ToString());
                    }
                }
                catch
                {
                    Logger.LogInformation(message: $"Z Axis SerialPort {Properties.Settings.Default.ZComPort} not connected.");
                }
            }

            btnZAxisPort.Content = $"Z Axis Port {Properties.Settings.Default.ZComPort}";
        }
        private async void StartDelayTask(string zindata)
        {
            try
            {
                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay) * 2);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (zAxisRunCompleted && (zindata.Contains("Z Axis CW Motor Current Position:") || zindata.Contains("Z Axis CCW Motor Current Position:")))
                    {
                        UpdateMotorPosition(zindata);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in StartDelayTask");
                MessageBox.Show($"{ex} Error in StartDelayTask", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void UpdateMotorPosition(string zindata)
        {
            string[] lines = zindata.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith("Z Axis CW Motor Current Position:") || line.StartsWith("Z Axis CCW Motor Current Position:"))
                {
                    string positionString = line.Replace("Z Axis CW Motor Current Position:", "").Replace("Z Axis CCW Motor Current Position:", "").Trim();
                    if (float.TryParse(positionString, out float currentPosition))
                    {
                        float stepsPerRevolution = 200.0f;
                        float distancePerRevolution = 4.0f;
                        float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
                        zAxisAbsolutePosition += line.StartsWith("Z Axis CW") ? distanceInMM : -distanceInMM;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ckbZaxisResetToZero.IsChecked = false;
                            txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString("F2");
                            txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2");
                            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2"));
                            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                            Properties.Settings.Default.Save();
                            zAxisRunCompleted = false;
                            Logger.LogInformation($"Z Axis Motor Current Position: {txtZaxisStepperCurrent.Text}");
                            zAxisClearAbsolutePosition = true;
                        });
                    }
                }
            }
        }

    }
}