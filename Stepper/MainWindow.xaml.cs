// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 11-07-2024
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
    using System.Diagnostics;
    using System.Xml.Linq;
    using System.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// The x cancellation token source
        /// </summary>
        private CancellationTokenSource _xCancellationTokenSource = new();
        /// <summary>
        /// The y cancellation token source
        /// </summary>
        private CancellationTokenSource _yCancellationTokenSource = new();
        /// <summary>
        /// The z cancellation token source
        /// </summary>
        private CancellationTokenSource _zCancellationTokenSource = new();
        /// <summary>
        /// Creates new settingswindow.
        /// </summary>
        /// <value>The new settings window.</value>
        public StepperAppSettings NewSettingsWindow { get; private set; }
        /// <summary>
        /// The x timer
        /// </summary>
        private static DispatcherTimer? _xTimer;
        /// <summary>
        /// The y timer
        /// </summary>
        private static DispatcherTimer? _yTimer;
        /// <summary>
        /// The z timer
        /// </summary>
        private static DispatcherTimer? _zTimer;
        /// <summary>
        /// Gets the x timer.
        /// </summary>
        /// <value>The x timer.</value>
        public static DispatcherTimer? xTimer { get => _xTimer; private set => _xTimer = value; }
        /// <summary>
        /// Gets the y timer.
        /// </summary>
        /// <value>The y timer.</value>
        public static DispatcherTimer? yTimer { get => _yTimer; private set => _yTimer = value; }
        /// <summary>
        /// Gets the z timer.
        /// </summary>
        /// <value>The z timer.</value>
        public static DispatcherTimer? zTimer { get => _zTimer; private set => _zTimer = value; }
        /// <summary>
        /// Gets the x target end time.
        /// </summary>
        /// <value>The x target end time.</value>
        public DateTime xTargetEndTime { get; private set; } = new();
        /// <summary>
        /// Gets the y target end time.
        /// </summary>
        /// <value>The y target end time.</value>
        public DateTime yTargetEndTime { get; private set; } = new();
        /// <summary>
        /// Gets the z target end time.
        /// </summary>
        /// <value>The z target end time.</value>
        public DateTime zTargetEndTime { get; private set; } = new();
        /// <summary>
        /// Gets the x stopwatch.
        /// </summary>
        /// <value>The x stopwatch.</value>
        public static Stopwatch xStopwatch { get; private set; } = new();
        /// <summary>
        /// Gets the y stopwatch.
        /// </summary>
        /// <value>The y stopwatch.</value>
        public static Stopwatch yStopwatch { get; private set; } = new();
        /// <summary>
        /// Gets the z stopwatch.
        /// </summary>
        /// <value>The z stopwatch.</value>
        public static Stopwatch zStopwatch { get; private set; } = new();
        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        /// <value>The elapsed time.</value>
        public TimeSpan ElapsedTime { get; private set; } = new();
        /// <summary>
        /// Gets the remaining time.
        /// </summary>
        /// <value>The remaining time.</value>
        public TimeSpan RemainingTime { get; private set; } = new();
        /// <summary>
        /// Gets the zero xaxis.
        /// </summary>
        /// <value>The zero xaxis.</value>
        public int ZeroXaxis { get; private set; } = Properties.Settings.Default.zeroXaxis;
        /// <summary>
        /// Gets the zero yaxis.
        /// </summary>
        /// <value>The zero yaxis.</value>
        public int ZeroYaxis { get; private set; } = Properties.Settings.Default.zeroYaxis;
        /// <summary>
        /// Gets the zero zaxis.
        /// </summary>
        /// <value>The zero zaxis.</value>
        public int ZeroZaxis { get; private set; } = Properties.Settings.Default.zeroZaxis;
        /// <summary>
        /// The xaxis changed
        /// </summary>
        public bool XaxisChanged = Properties.Settings.Default.ZaxisChanged;
        /// <summary>
        /// The yaxis changed
        /// </summary>
        public bool YaxisChanged = Properties.Settings.Default.ZaxisChanged;
        /// <summary>
        /// The zaxis changed
        /// </summary>
        public bool ZaxisChanged = Properties.Settings.Default.ZaxisChanged;
        /// <summary>
        /// Gets the xaxis stepper move temporary.
        /// </summary>
        /// <value>The xaxis stepper move temporary.</value>
        public string XaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// Gets the yaxis stepper move temporary.
        /// </summary>
        /// <value>The yaxis stepper move temporary.</value>
        public string YaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// Gets the zaxis stepper move temporary.
        /// </summary>
        /// <value>The zaxis stepper move temporary.</value>
        public string ZaxisStepperMoveTemp { get; private set; } = Properties.Settings.Default.Value_0_00.ToString();
        /// <summary>
        /// The x stepper move
        /// </summary>
        public decimal xStepperMove;
        /// <summary>
        /// The y stepper move
        /// </summary>
        public decimal yStepperMove;
        /// <summary>
        /// The z stepper move
        /// </summary>
        public decimal zStepperMove;
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public static ILogger? Logger { get; private set; }
        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public static ILoggerFactory? LoggerFactory { get; private set; }
        /// <summary>
        /// The x serial port
        /// </summary>
        public SerialPort xSerialPort;
        /// <summary>
        /// The y serial port
        /// </summary>
        public SerialPort ySerialPort;
        /// <summary>
        /// The z serial port
        /// </summary>
        public SerialPort zSerialPort;
        /// <summary>
        /// The message
        /// </summary>
        private static byte[] _message = new byte[6000];
        /// <summary>
        /// The x axis run completed
        /// </summary>
        public bool xAxisRunCompleted = false;
        /// <summary>
        /// The y axis run completed
        /// </summary>
        public bool yAxisRunCompleted = false;
        /// <summary>
        /// The z axis run completed
        /// </summary>
        public bool zAxisRunCompleted = false;
        /// <summary>
        /// The serial port
        /// </summary>
        static byte[] message = new byte[6000];
        /// <summary>
        /// The Z Axis Current Position
        /// </summary>
        private static float xAxisAbsolutePosition;
        /// <summary>
        /// The y axis absolute position
        /// </summary>
        private static float yAxisAbsolutePosition;
        /// <summary>
        /// The z axis absolute position
        /// </summary>
        private static float zAxisAbsolutePosition;
        /// <summary>
        /// The x axis clear absolute position
        /// </summary>
        private static bool xAxisClearAbsolutePosition;
        /// <summary>
        /// The y axis clear absolute position
        /// </summary>
        private static bool yAxisClearAbsolutePosition;
        /// <summary>
        /// The z axis clear absolute position
        /// </summary>
        private static bool zAxisClearAbsolutePosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();
            InitializeSettings();
            InitializeLogger();
            InitializeSerialPorts();
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
            string versionString = $"{major}.{minor}.{build}.{revision}";
            UpdateVersionElement(doc, "AssemblyVersion", versionString);
            UpdateVersionElement(doc, "FileVersion", versionString);
            // Save the modified XML file
            doc.Save(Properties.Settings.Default.ProjectFilePath); Properties.Settings.Default.BuildVersion = $"Version: {displayableVersion}";
            Logger.LogInformation(message: $"Version: {displayableVersion}");
#endif
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            NewSettingsWindow = new StepperAppSettings();

        }
        // Update the Version elements
        /// <summary>
        /// Updates the version element.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="version">The version.</param>
        private void UpdateVersionElement(XDocument doc, string elementName, string version)
        {
            var element = doc.Descendants(elementName).FirstOrDefault();
            if (element != null)
            {
                element.Value = version;
            }
        }

        /// <summary>
        /// Initializes the serial ports.
        /// </summary>
        private void InitializeSerialPorts()
        {
            InitializeSerialPort(ref xSerialPort, Properties.Settings.Default.XComPort, XdataReceivedHandler, btnXAxisPort, "X Axis Port");
            InitializeSerialPort(ref ySerialPort, Properties.Settings.Default.YComPort, YdataReceivedHandler, btnYAxisPort, "Y Axis Port");
            InitializeSerialPort(ref zSerialPort, Properties.Settings.Default.ZComPort, ZdataReceivedHandler, btnZAxisPort, "Z Axis Port");
        }

        /// <summary>
        /// Initializes the serial port.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="portName">Name of the port.</param>
        /// <param name="dataReceivedHandler">The data received handler.</param>
        /// <param name="portButton">The port button.</param>
        /// <param name="buttonText">The button text.</param>
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
        /// <summary>
        /// Opens the serial port.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
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
        /// <summary>
        /// Initializes the settings.
        /// </summary>
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
        /// <summary>
        /// Initializes the logger.
        /// </summary>
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
        /// <summary>
        /// Initializes the timers.
        /// </summary>
        private void InitializeTimers()
        {
            InitializeTimer(ref _xTimer, tickHandler: Timer_Tick);
            InitializeTimer(ref _yTimer, tickHandler: Timer_Tick);
            InitializeTimer(ref _zTimer, tickHandler: Timer_Tick);
        }
        /// <summary>
        /// Initializes the timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="tickHandler">The tick handler.</param>
        private void InitializeTimer(ref DispatcherTimer? timer, EventHandler? tickHandler)
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.MilisecondTimerInterval))
            };
            timer.Tick += tickHandler!;
        }
        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (sender == xTimer)
            {
                HandleTimerTick("X", xStopwatch, xTargetEndTime, ref xAxisRunCompleted, ref xAxisClearAbsolutePosition, ref xAxisAbsolutePosition, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, ref XaxisChanged);
            }
            else if (sender == yTimer)
            {
                HandleTimerTick("Y", yStopwatch, yTargetEndTime, ref yAxisRunCompleted, ref yAxisClearAbsolutePosition, ref yAxisAbsolutePosition, txtYaxisStepperMove, txtYaxisStepperCurrent, txtYaxisMotorSpeed, ckbYaxisResetToZero, ref YaxisChanged);
            }
            else if (sender == zTimer)
            {
                HandleTimerTick("Z", zStopwatch, zTargetEndTime, ref zAxisRunCompleted, ref zAxisClearAbsolutePosition, ref zAxisAbsolutePosition, txtZaxisStepperMove, txtZaxisStepperCurrent, txtZaxisMotorSpeed, ckbZaxisResetToZero, ref ZaxisChanged);
            }
        }

        /// <summary>
        /// Handles the timer tick.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="targetEndTime">The target end time.</param>
        /// <param name="axisRunCompleted">if set to <c>true</c> [axis run completed].</param>
        /// <param name="axisClearAbsolutePosition">if set to <c>true</c> [axis clear absolute position].</param>
        /// <param name="axisAbsolutePosition">The axis absolute position.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="stepperCurrentTextBox">The stepper current text box.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="axisChanged">if set to <c>true</c> [axis changed].</param>
        private void HandleTimerTick(string axis, Stopwatch stopwatch, DateTime targetEndTime, ref bool axisRunCompleted, ref bool axisClearAbsolutePosition, ref float axisAbsolutePosition, TextBox stepperMoveTextBox, TextBox stepperCurrentTextBox, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, ref bool axisChanged)
        {
            ElapsedTime = stopwatch.Elapsed;
            RemainingTime = targetEndTime - DateTime.Now;
            if (DateTime.Now < targetEndTime)
            {
                DisableControls();
                axisChanged = true;
                if (!resetToZeroCheckBox.IsChecked.GetValueOrDefault())
                {
                    Logger.LogInformation($"Stepper Motor Controller Disable {axis} Axis controls while moving to location.");
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} {ElapsedTime.ToString(@"hh\:mm\:ss\.fffff")}";
                    Logger.LogInformation($"Time remaining: {ElapsedTime.ToString(@"hh\:mm\:ss")} targetEndTime = {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                else
                {
                    resetToZeroCheckBox.IsChecked = false;
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                }
            }
            else
            {
                CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                EnableControls();
                string currentAxis = (Convert.ToDecimal(stepperCurrentTextBox.Text) + Convert.ToDecimal(stepperMoveTextBox.Text)).ToString();
                stepperCurrentTextBox.Text = currentAxis;
                axisChanged = false;
                stepperMoveTextBox.BorderBrush = System.Windows.Media.Brushes.White;
                motorSpeedTextBox.BorderBrush = System.Windows.Media.Brushes.White;
                Properties.Settings.Default[$"{axis}axisMotorSpeed"] = Convert.ToDecimal(motorSpeedTextBox.Text);
                Properties.Settings.Default[$"{axis}axisStepperCurrent"] = Convert.ToDecimal(stepperCurrentTextBox.Text);
                Properties.Settings.Default[$"{axis}axisStepperMove"] = Convert.ToDecimal(stepperMoveTextBox.Text);
                Properties.Settings.Default.Save();
                Logger.LogInformation($"Stepper Motor Controller Enable {axis} Axis controls after moving to location.");
                stopwatch.Stop();
                Logger.LogInformation($"Stepper Motor Controller Stopwatch Stopped.");
                if (axis == "X") xTimer.Stop();
                if (axis == "Y") yTimer.Stop();
                if (axis == "Z") zTimer.Stop();
                Logger.LogInformation($"Stepper Motor Controller Timer Stopped.");
                stopwatch.Reset();
                Logger.LogInformation($"Stepper Motor Controller Stopwatch Reset.");
            }
        }

        /// <summary>
        /// Disables the controls.
        /// </summary>
        private void DisableControls()
        {
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
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
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
        /// Xdatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Xsp = (SerialPort)sender;
                string Xindata = Xsp.ReadExisting();
                Logger.LogInformation(message: $"X Axis Data Received: {Xindata}");
                // Check if the message indicates the motor has stopped
                if (Xindata.Contains("X Axis CW Motor Stopped"))
                {
                    xTimer.Stop();
                    xStopwatch.Stop();
                    // Cancel the delay task
                    _xCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        EnableControls();
                        XaxisChanged = true;
                        CountdownLabel.Content = "";
                        //XZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbXaxisResetToZero.IsChecked = true;
                        xAxisClearAbsolutePosition = false;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("X", Xindata);

                        //MessageBox.Show("X Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }
                if (Xindata.Contains("X Axis CCW Motor Stopped"))
                {
                    xTimer.Stop();
                    xStopwatch.Stop();
                    // Cancel the delay task
                    _xCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        EnableControls();
                        XaxisChanged = true;
                        CountdownLabel.Content = "";
                        //XZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbXaxisResetToZero.IsChecked = true;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("X", Xindata);

                        //MessageBox.Show("X Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in XdataReceivedHandler");
                MessageBox.Show(ex.ToString() + " Error in XdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Ydatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void YdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Ysp = (SerialPort)sender;
                string Yindata = Ysp.ReadExisting();
                Logger.LogInformation(message: $"Z Axis Data Received: {Yindata}");
                // Check if the message indicates the motor has stopped
                if (Yindata.Contains("Y Axis CW Motor Stopped"))
                {
                    yTimer.Stop();
                    yStopwatch.Stop();
                    // Cancel the delay task
                    _yCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        EnableControls();
                        YaxisChanged = true;
                        CountdownLabel.Content = "";
                        //YZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbYaxisResetToZero.IsChecked = true;
                        yAxisClearAbsolutePosition = false;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("Y", Yindata);

                        //MessageBox.Show("Y Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }
                if (Yindata.Contains("Y Axis CCW Motor Stopped"))
                {
                    yTimer.Stop();
                    yStopwatch.Stop();
                    // Cancel the delay task
                    _yCancellationTokenSource.Cancel();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        EnableControls();
                        YaxisChanged = true;
                        CountdownLabel.Content = "";
                        //ZZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbYaxisResetToZero.IsChecked = true;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("Y", Yindata);

                        //MessageBox.Show("Y Axis Motor has stopped due to limit switch trigger.", "Motor Stopped", MessageBoxButton.OK, MessageBoxImage.Information);
                    });

                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in YdataReceivedHandler");
                MessageBox.Show(ex.ToString() + " Error in YdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Zdatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs"/> instance containing the event data.</param>
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
                        EnableControls();
                        ZaxisChanged = true;
                        CountdownLabel.Content = "";
                        //ZZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbZaxisResetToZero.IsChecked = true;
                        zAxisClearAbsolutePosition = false;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("Z", Zindata);
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
                        EnableControls();
                        ZaxisChanged = true;
                        CountdownLabel.Content = "";
                        //ZZero(Properties.Settings.Default.Milliseconds, Properties.Settings.Default.RootAxisZ);
                        ckbZaxisResetToZero.IsChecked = true;
                        RoutedEventArgs e = new();
                        AxisRun_Click(sender, e);
                        StartDelayTask("Z", Zindata);

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
        /// Handles the Click event of the AxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public async void AxisRun_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnRunXAxis)
            {
                await RunAxis("X", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, xSerialPort);
            }
            else if (sender == btnRunYAxis)
            {
                await RunAxis("Y", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, ySerialPort);
            }
            else if (sender == btnRunZAxis)
            {
                await RunAxis("Z", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, zSerialPort);
            }
            else if (sender == btnRunXYAxis)
            {
                await RunAxis("X", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, xSerialPort);
                while (!xAxisRunCompleted)
                {
                    await Task.Delay(100);
                }
                await RunAxis("Y", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, ySerialPort);
            }
        }
        private async Task RunAxis(string axis, TextBox XstepperMoveTextBox, TextBox XmotorSpeedTextBox, CheckBox XresetToZeroCheckBox, TextBox YstepperMoveTextBox, TextBox YmotorSpeedTextBox, CheckBox YresetToZeroCheckBox, TextBox ZstepperMoveTextBox, TextBox ZmotorSpeedTextBox, CheckBox ZresetToZeroCheckBox, SerialPort serialPort)
        {
            if (string.IsNullOrEmpty(axis))
            {
                throw new ArgumentException($"'{nameof(axis)}' cannot be null or empty.", nameof(axis));
            }

            if (serialPort is null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            Logger.LogInformation(message: $"{axis} Axis Run button clicked:");

            try
            {
                switch (axis)
                {
                    case "X":
                        if (XresetToZeroCheckBox.IsChecked == true)
                        {
                            await ResetAxisToZero(axis, serialPort);
                        }
                        else
                        {
                            await MoveAxis(axis, serialPort);
                        }
                        break;
                    case "Y":
                        if (YresetToZeroCheckBox.IsChecked == true)
                        {
                            await ResetAxisToZero(axis, serialPort);
                        }
                        else
                        {
                            await MoveAxis(axis, serialPort);
                        }
                        break;
                    case "Z":
                        if (ZresetToZeroCheckBox.IsChecked == true)
                        {
                            await ResetAxisToZero(axis, serialPort);
                        }
                        else
                        {
                            await MoveAxis(axis, serialPort);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task ResetAxisToZero(string axis, SerialPort serialPort)
        {
            string command = string.Empty;
            switch (axis)
            {
                case "X":
                    command = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text.Trim()},1,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    break;
                case "Y":
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text.Trim()},1,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    break;
                case "Z":
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtZaxisMotorSpeed.Text.Trim()},1";
                    break;
            }

            serialPort.Write(command);
            Logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {command}");
            await ZeroAxis(axis);
        }
        private async Task MoveAxis(string axis, SerialPort serialPort)
        {
            decimal motorMovementSeconds = 1;
            int movementTimer;

            string command = string.Empty;
            switch (axis)
            {
                case "X":
                    motorMovementSeconds = UpdateMotorTimer(axis, Convert.ToDecimal(txtXaxisMotorSpeed.Text.Trim()), Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    break;
                case "Y":
                    motorMovementSeconds = UpdateMotorTimer(axis, Convert.ToDecimal(txtYaxisMotorSpeed.Text.Trim()), Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    break;
                case "Z":
                    motorMovementSeconds = UpdateMotorTimer(axis, Convert.ToDecimal(txtZaxisMotorSpeed.Text.Trim()), Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()));
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    break;
            }

            serialPort.Write(command);
            await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
            Logger.LogInformation(message: $"{axis} Axis Run Event: {command}");
            movementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(motorMovementSeconds);
            TimeSpan countdownTime = TimeSpan.FromMilliseconds(movementTimer);
            DateTime targetEndTime = DateTime.Now.Add(countdownTime);
            StartTimer(axis, targetEndTime);
            StartStopwatch(axis);

            switch (axis)
            {
                case "X":
                    xAxisRunCompleted = true;
                    break;
                case "Y":
                    yAxisRunCompleted = true;
                    break;
                case "Z":
                    zAxisRunCompleted = true;
                    break;
            }

            Logger.LogInformation(message: $"{axis} Axis Current Time: {DateTime.Now.ToString(@"hh\:mm\:ss")} + {movementTimer} = {targetEndTime.ToString(@"hh\:mm\:ss")}");
        }

        /// <summary>
        /// Zeroes the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        private async Task ZeroAxis(string axis)
        {
            Logger.LogInformation(message: $"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(Properties.Settings.Default.Milliseconds);
            if (axis == "X")
            {
                txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
                Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            }
            else if (axis == "Y")
            {
                txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
                Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            }
            else if (axis == "Z")
            {
                txtZaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
                ckbZaxisResetToZero.IsChecked = false;
                txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
                Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtZaxisStepperCurrent.Text);
                Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
            }
            Properties.Settings.Default.Save();
            Logger.LogInformation(message: $"{axis} Axis Current Location Set to Zero");
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="targetEndTime">The target end time.</param>
        private void StartTimer(string axis, DateTime targetEndTime)
        {
            if (axis == "X")
            {
                xTargetEndTime = targetEndTime;
                xTimer.Start();
            }
            else if (axis == "Y")
            {
                yTargetEndTime = targetEndTime;
                yTimer.Start();
            }
            else if (axis == "Z")
            {
                zTargetEndTime = targetEndTime;
                zTimer.Start();
            }
        }

        /// <summary>
        /// Starts the stopwatch.
        /// </summary>
        /// <param name="axis">The axis.</param>
        private void StartStopwatch(string axis)
        {
            if (axis == "X")
            {
                xStopwatch.Start();
            }
            else if (axis == "Y")
            {
                yStopwatch.Start();
            }
            else if (axis == "Z")
            {
                zStopwatch.Start();
            }
        }
        /// <summary>
        /// Xies the zero.
        /// </summary>
        /// <param name="myDelay">My delay.</param>
        /// <param name="Axis">The axis.</param>
        public async void XYZero(int myDelay, string Axis)
        {
            Logger.LogInformation(message: $"Setting {Axis} Axis Current Location Set to Zero on DRO");

            // Call XZero
            await ZeroAxis(Properties.Settings.Default.RootAxisX);

            // Wait for XZero to complete
            while (xAxisClearAbsolutePosition)
            {
                await Task.Delay(100);
            }

            // Call YZero
            await ZeroAxis(Properties.Settings.Default.RootAxisY);

            // Wait for YZero to complete
            while (yAxisClearAbsolutePosition)
            {
                await Task.Delay(100);
            }

            Logger.LogInformation(message: $"{Axis} Axis Current Location Set to Zero");
        }
        /// <summary>
        /// CheckBoxes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            UpdateZeroStatus();
        }
        /// <summary>
        /// Handles the GotFocus event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            if (sender == txtXaxisStepperMove)
            {
                XaxisStepperMoveTemp = txtXaxisStepperMove.Text.ToString();
            }
            else if (sender == txtYaxisStepperMove)
            {
                YaxisStepperMoveTemp = txtYaxisStepperMove.Text.ToString();
            }
            else if (sender == txtZaxisStepperMove)
            {
                ZaxisStepperMoveTemp = txtZaxisStepperMove.Text.ToString();
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void AxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            bool axisChanged = false;
            string axis = string.Empty;
            decimal stepperMoveValue = 0;

            if (textBox == txtXaxisStepperMove)
            {
                axisChanged = true;
                axis = "X";
                stepperMoveValue = Properties.Settings.Default.XaxisStepperMove;
            }
            else if (textBox == txtYaxisStepperMove)
            {
                axisChanged = true;
                axis = "Y";
                stepperMoveValue = Properties.Settings.Default.YaxisStepperMove;
            }
            else if (textBox == txtZaxisStepperMove)
            {
                axisChanged = true;
                axis = "Z";
                stepperMoveValue = Properties.Settings.Default.ZaxisStepperMove;
            }

            if (axisChanged)
            {
                if (textBox.Text == stepperMoveValue.ToString())
                {
                    textBox.BorderBrush = System.Windows.Media.Brushes.White;
                }
                else
                {
                    textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    try
                    {
                        decimal newValue = Convert.ToDecimal(textBox.Text);
                        Properties.Settings.Default[$"{axis}axisStepperMove"] = newValue;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                        MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void AxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisStepperMove)
                {
                    txtXaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisStepperMove)
                {
                    txtYaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisStepperMove)
                {
                    txtZaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
        private void AxisStepperMove_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisStepperMove)
                {
                    txtXaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisStepperMove)
                {
                    txtYaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisStepperMove)
                {
                    txtZaxisStepperMove.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void AxisMotorSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string axis = string.Empty;
            decimal motorSpeedValue = 0;

            if (textBox == txtXaxisMotorSpeed)
            {
                axis = "X";
                motorSpeedValue = Properties.Settings.Default.XaxisMotorSpeed;
            }
            else if (textBox == txtYaxisMotorSpeed)
            {
                axis = "Y";
                motorSpeedValue = Properties.Settings.Default.YaxisMotorSpeed;
            }
            else if (textBox == txtZaxisMotorSpeed)
            {
                axis = "Z";
                motorSpeedValue = Properties.Settings.Default.ZaxisMotorSpeed;
            }

            if (textBox.Text == motorSpeedValue.ToString())
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                try
                {
                    decimal newValue = Convert.ToDecimal(textBox.Text);
                    Properties.Settings.Default[$"{axis}axisMotorSpeed"] = newValue;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    Logger.LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                    MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void AxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisMotorSpeed)
                {
                    txtXaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisMotorSpeed)
                {
                    txtYaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisMotorSpeed)
                {
                    txtZaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
        private void AxisMotorSpeed_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisMotorSpeed)
                {
                    txtXaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisMotorSpeed)
                {
                    txtYaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisMotorSpeed)
                {
                    txtZaxisMotorSpeed.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void AxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisStepperCurrent)
                {
                    txtXaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisStepperCurrent)
                {
                    txtYaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisStepperCurrent)
                {
                    txtZaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis mouse controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the TouchUp event of the AxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
        private void AxisStepperCurrent_TouchUp(object sender, TouchEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                if (sender == txtXaxisStepperCurrent)
                {
                    txtXaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"X Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtYaxisStepperCurrent)
                {
                    txtYaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Y Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
                else if (sender == txtZaxisStepperCurrent)
                {
                    txtZaxisStepperCurrent.Text = mainWindow.Result.ToString();
                    Logger.LogInformation(message: $"Z Axis touch controlled Keypad returned: {mainWindow.Result}");
                }
            }
        }
        /// <summary>
        /// Handles the OnPreviewTextInput event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
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
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AppSettings_Click(object sender, RoutedEventArgs e)
        {
            Logger.LogInformation(message: $"Stepper Motor Controller Loading Application Settings form.");
            // Show the new window
            NewSettingsWindow.Activate();
            NewSettingsWindow.ShowDialog();

        }
        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Logger.LogInformation(message: $"Stepper Motor Controller MainWindow Closing.");
                xSerialPort.Close();
                ySerialPort.Close();
                zSerialPort.Close();
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
        /// <param name="e">The <see cref="TouchEventArgs"/> instance containing the event data.</param>
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
        /// Updates the motor timer.
        /// </summary>
        /// <param name="Axis">The axis.</param>
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
        /// Handles the Click event of the AxisPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AxisPort_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnXAxisPort)
            {
                HandlePortClick(ref xSerialPort, Properties.Settings.Default.XComPort, XdataReceivedHandler, btnXAxisPort, "X Axis Port");
            }
            else if (sender == btnYAxisPort)
            {
                HandlePortClick(ref ySerialPort, Properties.Settings.Default.YComPort, YdataReceivedHandler, btnYAxisPort, "Y Axis Port");
            }
            else if (sender == btnZAxisPort)
            {
                HandlePortClick(ref zSerialPort, Properties.Settings.Default.ZComPort, ZdataReceivedHandler, btnZAxisPort, "Z Axis Port");
            }
        }
        /// <summary>
        /// Handles the port click.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="portName">Name of the port.</param>
        /// <param name="dataReceivedHandler">The data received handler.</param>
        /// <param name="portButton">The port button.</param>
        /// <param name="buttonText">The button text.</param>
        private void HandlePortClick(ref SerialPort serialPort, string portName, SerialDataReceivedEventHandler dataReceivedHandler, Button portButton, string buttonText)
        {
            serialPort.Close();
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.PortName = portName; // Set your port name
                    serialPort.BaudRate = Properties.Settings.Default.BaudRate; // Set your baud rate
                                                                                // Enable RTS and DTR
                    serialPort.RtsEnable = true;
                    serialPort.DtrEnable = true;
                    serialPort.DataReceived += dataReceivedHandler;
                    serialPort.Open();
                }
                catch
                {
                    Logger.LogInformation(message: $"{buttonText} {portName} not connected.");
                }
            }

            portButton.Content = $"{buttonText} {portName}";
        }
        /// <summary>
        /// Starts the delay task.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="data">The data.</param>
        private async void StartDelayTask(string axis, string data)
        {
            try
            {
                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay) * 2);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if ((axis == "X" && xAxisRunCompleted && (data.Contains("X Axis CW Motor Current Position:") || data.Contains("X Axis CCW Motor Current Position:"))) ||
                        (axis == "Y" && yAxisRunCompleted && (data.Contains("Y Axis CW Motor Current Position:") || data.Contains("Y Axis CCW Motor Current Position:"))) ||
                        (axis == "Z" && zAxisRunCompleted && (data.Contains("Z Axis CW Motor Current Position:") || data.Contains("Z Axis CCW Motor Current Position:"))))
                    {
                        UpdateMotorPosition(axis, data);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in StartDelayTask");
                MessageBox.Show($"{ex} Error in StartDelayTask", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Updates the motor position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="data">The data.</param>
        private void UpdateMotorPosition(string axis, string data)
        {
            string[] lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Contains($"{axis} Axis CW Motor Current Position:") || line.Contains($"{axis} Axis CCW Motor Current Position:"))
                {
                    string positionString = line.Replace($"{axis} Axis CW Motor Current Position:", "").Replace($"{axis} Axis CCW Motor Current Position:", "").Trim();
                    if (float.TryParse(positionString, out float currentPosition))
                    {
                        float stepsPerRevolution = 200.0f;
                        float distancePerRevolution = 4.0f;
                        float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;

                        if (axis == "X")
                        {
                            if (line.Contains("CW"))
                            {
                                xAxisAbsolutePosition += line.Contains("CW") ? distanceInMM : -distanceInMM;
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ckbXaxisResetToZero.IsChecked = false;
                                    txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString("F2");
                                    txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                                    Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(xAxisAbsolutePosition.ToString("F2"));
                                    Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                                    Properties.Settings.Default.Save();
                                    xAxisRunCompleted = false;
                                    Logger.LogInformation($"X Axis Motor Current Position: {txtXaxisStepperCurrent.Text}");
                                    xAxisClearAbsolutePosition = true;
                                });
                            }
                        }
                        else if (axis == "Y")
                        {
                            yAxisAbsolutePosition += line.Contains("CW") ? distanceInMM : -distanceInMM;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ckbYaxisResetToZero.IsChecked = false;
                                txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString("F2");
                                txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                                Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(yAxisAbsolutePosition.ToString("F2"));
                                Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                                Properties.Settings.Default.Save();
                                yAxisRunCompleted = false;
                                Logger.LogInformation($"Y Axis Motor Current Position: {txtYaxisStepperCurrent.Text}");
                                yAxisClearAbsolutePosition = true;
                            });
                        }
                        else if (axis == "Z")
                        {
                            zAxisAbsolutePosition += line.Contains("CW") ? distanceInMM : -distanceInMM;
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
}