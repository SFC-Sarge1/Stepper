// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 11-23-2024
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This allows you to control the X,Y and Y Axis</summary>
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
    using System.Windows.Shapes;
    using Windows.Devices.Geolocation;
    using System.Runtime.CompilerServices;
    using System.Windows.Markup;
    using System;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// The x position updated
        /// </summary>
        bool xPositionUpdated = false;
        /// <summary>
        /// The y position updated
        /// </summary>
        bool yPositionUpdated = false;
        /// <summary>
        /// The x position updated
        /// </summary>
        bool zPositionUpdated = false;
        /// <summary>
        /// The x cancellation token source
        /// </summary>
        private CancellationTokenSource _xCancellationTokenSource = new();
        /// <summary>
        /// The y cancellation token source
        /// </summary>
        private CancellationTokenSource _yCancellationTokenSource = new();
        /// <summary>
        /// The x cancellation token source
        /// </summary>
        private CancellationTokenSource _zCancellationTokenSource = new();
        /// <summary>
        /// The cancellation token source
        /// </summary>
        public CancellationTokenSource cancellationTokenSource = new();
        /// <summary>
        /// Creates new setting swindow.
        /// </summary>
        /// <value>The new settings window.</value>
        public StepperAppSettings NewSettingsWindow { get; private set; }
        /// <summary>
        /// The countdown timer
        /// </summary>
        private DispatcherTimer countdownTimer;
        /// <summary>
        /// The x limit switch press
        /// </summary>
        private static bool _xLimitSwitchPress = false;
        /// <summary>
        /// The y limit switch press
        /// </summary>
        private static bool _yLimitSwitchPress = false;
        /// <summary>
        /// The x limit switch press
        /// </summary>
        private static bool _zLimitSwitchPress = false;
        /// <summary>
        /// The limit switch pressed
        /// </summary>
        public static bool LimitSwitchPressed = false;
        /// <summary>
        /// The target end time
        /// </summary>
        DateTime targetEndTime;
        /// <summary>
        /// Gets the xero xaxis.
        /// </summary>
        /// <value>The xero xaxis.</value>
        public int ZeroXaxis { get; private set; } = Properties.Settings.Default.zeroXaxis;
        /// <summary>
        /// Gets the xero yaxis.
        /// </summary>
        /// <value>The xero yaxis.</value>
        public int ZeroYaxis { get; private set; } = Properties.Settings.Default.zeroYaxis;
        /// <summary>
        /// Gets the xero xaxis.
        /// </summary>
        /// <value>The xero xaxis.</value>
        public int ZeroZaxis { get; private set; } = Properties.Settings.Default.zeroZaxis;
        /// <summary>
        /// The xaxis changed
        /// </summary>
        public bool XaxisChanged = Properties.Settings.Default.YaxisChanged;
        /// <summary>
        /// The yaxis changed
        /// </summary>
        public bool YaxisChanged = Properties.Settings.Default.YaxisChanged;
        /// <summary>
        /// The xaxis changed
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
        /// Gets the xaxis stepper move temporary.
        /// </summary>
        /// <value>The xaxis stepper move temporary.</value>
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
        /// The x stepper move
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
        public SerialPort? xSerialPort;
        /// <summary>
        /// The y serial port
        /// </summary>
        public SerialPort? ySerialPort;
        /// <summary>
        /// The x serial port
        /// </summary>
        public SerialPort? zSerialPort;
        /// <summary>
        /// The message
        /// </summary>
        private static byte[] _message = new byte[6000];
        /// <summary>
        /// The x axis run completed
        /// </summary>
        public bool xAxisRunToCompletion = false;
        /// <summary>
        /// The y axis run completed
        /// </summary>
        public bool yAxisRunToCompletion = false;
        /// <summary>
        /// The x axis run completed
        /// </summary>
        public bool zAxisRunToCompletion = false;
        /// <summary>
        /// The serial port
        /// </summary>
        static byte[] message = new byte[6000];
        /// <summary>
        /// The Y Axis Current Position
        /// </summary>
        private static float xAxisAbsolutePosition = 0.00f;
        /// <summary>
        /// The y axis absolute position
        /// </summary>
        private static float yAxisAbsolutePosition = 0.00f;
        /// <summary>
        /// The x axis absolute position
        /// </summary>
        private static float zAxisAbsolutePosition = 0.00f;
        /// <summary>
        /// The x axis clear absolute position
        /// </summary>
        private static bool xAxisClearAbsolutePosition;
        /// <summary>
        /// The y axis clear absolute position
        /// </summary>
        private static bool yAxisClearAbsolutePosition;
        /// <summary>
        /// The x axis clear absolute position
        /// </summary>
        private static bool zAxisClearAbsolutePosition;
        /// <summary>
        /// The esp32 rebooted
        /// </summary>
        public static int esp32Rebooted = 0;
        // Define the event using the delegate
        /// <summary>
        /// Occurs when X axis target reached changed].
        /// </summary>
        public event AxisTargetReachedEventHandler? XAxisTargetReachedChanged;
        // Backing field for the _xAxisTargetReached property
        /// <summary>
        /// The x axis target reached
        /// </summary>
        private bool _xAxisTargetReached;
        // Property for _xAxisTargetReached with event raising
        /// <summary>
        /// Gets or sets a value indicating whether X axis target reached].
        /// </summary>
        /// <value><c>true</c> if X axis target reached]; otherwise, <c>false</c>.</value>
        public bool XAxisTargetReached
        {
            get => _xAxisTargetReached;
            set
            {
                if (_xAxisTargetReached != value)
                {
                    _xAxisTargetReached = value;
                    OnXAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        // Method to raise the event
        /// <summary>
        /// Handles the <see cref="E:XAxisTargetReachedChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnXAxisTargetReachedChanged(EventArgs e)
        {
            XAxisTargetReachedChanged?.Invoke(this, e);
        }
        // Define the event using the delegate
        /// <summary>
        /// Occurs when Y axis target reached changed].
        /// </summary>
        public event AxisTargetReachedEventHandler? YAxisTargetReachedChanged;
        // Backing field for the _xAxisTargetReached property
        /// <summary>
        /// The y axis target reached
        /// </summary>
        private bool _yAxisTargetReached;
        // Property for _xAxisTargetReached with event raising
        /// <summary>
        /// Gets or sets a value indicating whether Y axis target reached].
        /// </summary>
        /// <value><c>true</c> if Y axis target reached]; otherwise, <c>false</c>.</value>
        public bool YAxisTargetReached
        {
            get => _yAxisTargetReached;
            set
            {
                if (_yAxisTargetReached != value)
                {
                    _yAxisTargetReached = value;
                    OnYAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        // Method to raise the event
        /// <summary>
        /// Handles the <see cref="E:YAxisTargetReachedChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnYAxisTargetReachedChanged(EventArgs e)
        {
            YAxisTargetReachedChanged?.Invoke(this, e);
        }
        // Define a delegate for the event
        /// <summary>
        /// Delegate AxisTargetReachedEventHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void AxisTargetReachedEventHandler(object sender, EventArgs e);
        // Define the event using the delegate
        /// <summary>
        /// Occurs when Z axis target reached changed].
        /// </summary>
        public event AxisTargetReachedEventHandler? ZAxisTargetReachedChanged;
        // Backing field for the _xAxisTargetReached property
        /// <summary>
        /// The x axis target reached
        /// </summary>
        private bool _zAxisTargetReached;
        // Property for _xAxisTargetReached with event raising
        /// <summary>
        /// Gets or sets a value indicating whether Z axis target reached].
        /// </summary>
        /// <value><c>true</c> if Z axis target reached]; otherwise, <c>false</c>.</value>
        public bool ZAxisTargetReached
        {
            get => _zAxisTargetReached;
            set
            {
                if (_zAxisTargetReached != value)
                {
                    _zAxisTargetReached = value;
                    OnZAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// The current axis
        /// </summary>
        public string CurrentAxis = "Z";
        /// <summary>
        /// The completed X axis current position
        /// </summary>
        private static float CompletedXAxisCurrentPosition = 0.00f;
        /// <summary>
        /// The completed Y axis current position
        /// </summary>
        private static float CompletedYAxisCurrentPosition = 0.00f;
        /// <summary>
        /// The completed Z axis current position
        /// </summary>
        private static float CompletedZAxisCurrentPosition = 0.00f;
        // Method to raise the event
        /// <summary>
        /// Handles the <see cref="E:ZAxisTargetReachedChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnZAxisTargetReachedChanged(EventArgs e)
        {
            ZAxisTargetReachedChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();
            InitializeLogger();
            InitializeSerialPorts();
            InitializeCountdownTimer();

            LogInformation("Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            LogInformation($"Version: {displayableVersion}");
            ResizeMode = ResizeMode.NoResize;
#if DEBUG
            UpdateVersionInfo();
#endif
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            XAxisTargetReachedChanged += MainWindow_XAxisTargetReachedChanged;
            YAxisTargetReachedChanged += MainWindow_YAxisTargetReachedChanged;
            ZAxisTargetReachedChanged += MainWindow_ZAxisTargetReachedChanged;
            NewSettingsWindow = new StepperAppSettings();
        }
        /// <summary>
        /// Initializes the countdown timer.
        /// </summary>
        private void InitializeCountdownTimer()
        {
            countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countdownTimer.Tick += CountdownTimer_Tick;
        }
        // Helper method for logging with line number
        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lineNumber">The line number.</param>
        public static void LogInformation(string message, [CallerLineNumber] int lineNumber = 0)
        {
            Logger?.LogInformation($"{message} (Line: {lineNumber})");
        }
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <param name="lineNumber">The line number.</param>
        public static void LogError(Exception ex, string message, [CallerLineNumber] int lineNumber = 0)
        {
            Logger?.LogError(ex, $"{message} (Line: {lineNumber})");
        }
        /// <summary>
        /// Updates the version information.
        /// </summary>
        private void UpdateVersionInfo()
        {
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
            Version version = new(major, minor, build, revision);
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{major}.{minor}.{build}.{revision} ({buildDate})";
            LogInformation($"Version: {displayableVersion}");
            VersionTxt.Text = $"Version: {displayableVersion}";
            XDocument doc = XDocument.Load(Properties.Settings.Default.ProjectFilePath);
            string versionString = $"{major}.{minor}.{build}.{revision}";
            UpdateVersionElement(doc, "AssemblyVersion", versionString);
            UpdateVersionElement(doc, "FileVersion", versionString);
            doc.Save(Properties.Settings.Default.ProjectFilePath);
            Properties.Settings.Default.BuildVersion = $"Version: {displayableVersion}";
            LogInformation($"Version: {displayableVersion}");
        }
        // Event handler method
        ///// <summary>
        ///// Handles the XPositionUpdatedChanged event of the MainWindow control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //private void MainWindow_XPositionUpdatedChanged(object sender, EventArgs e)
        //{
        //    LogInformation("X Position Updated changed.");
        //    // Add your custom logic here
        //}
        //// Event handler method
        ///// <summary>
        ///// Handles the YPositionUpdatedChanged event of the MainWindow control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //private void MainWindow_YPositionUpdatedChanged(object sender, EventArgs e)
        //{
        //    LogInformation("Y Position Updated changed.");
        //    // Add your custom logic here
        //}
        ///// <summary>
        ///// Handles the ZPositionUpdatedChanged event of the MainWindow control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //private void MainWindow_ZPositionUpdatedChanged(object sender, EventArgs e)
        //{
        //    LogInformation("Z Position Updated changed.");
        //    // Add your custom logic here
        //}
        // Event handler method
        /// <summary>
        /// Handles the XAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_XAxisTargetReachedChanged(object sender, EventArgs e)
        {
            LogInformation("Z Axis Target Reached changed.");
            if (XAxisTargetReached)
            {
            }
        }
        /// <summary>
        /// Handles the YAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_YAxisTargetReachedChanged(object sender, EventArgs e)
        {
            LogInformation("Z Axis Target Reached changed.");
            if (YAxisTargetReached)
            {
            }
        }
        /// <summary>
        /// Handles the ZAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_ZAxisTargetReachedChanged(object sender, EventArgs e)
        {
            LogInformation("Z Axis Target Reached changed.");
            if (ZAxisTargetReached)
            {
            }
        }
        /// <summary>
        /// Updates the version element.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="System.ArgumentNullException">doc</exception>
        /// <exception cref="System.ArgumentException">Element name cannot be null or empty. - elementName</exception>
        /// <exception cref="System.ArgumentException">Version cannot be null or empty. - version</exception>
        private void UpdateVersionElement(XDocument doc, string elementName, string version)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (string.IsNullOrEmpty(elementName)) throw new ArgumentException("Element name cannot be null or empty.", nameof(elementName));
            if (string.IsNullOrEmpty(version)) throw new ArgumentException("Version cannot be null or empty.", nameof(version));

            var element = doc.Descendants(elementName).FirstOrDefault();
            if (element != null)
            {
                element.Value = version;
                LogInformation($"Updated {elementName} to version {version}.");
            }
            else
            {
                LogInformation($"Element {elementName} not found in the provided XML document.");
            }
        }
        /// <summary>
        /// Initializes the serial ports.
        /// </summary>
        private void InitializeSerialPorts()
        {
            LogInformation("Initializing Serial Ports.");

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
        /// <exception cref="System.ArgumentNullException">serialPort</exception>
        private void OpenSerialPort(SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            try
            {
                if (serialPort.IsOpen)
                {
                    LogInformation($"SerialPort {serialPort.PortName} is already open.");
                    return;
                }

                serialPort.Open();
                LogInformation($"SerialPort {serialPort.PortName} opened successfully.");

                if (serialPort.BytesToRead > 0)
                {
                    int bytesRead = serialPort.Read(message, 0, serialPort.BytesToRead);
                    LogInformation($"SerialPort {serialPort.PortName} read {bytesRead} bytes.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex, $"Access to the port {serialPort.PortName} is denied.");
                MessageBox.Show($"Access to the port {serialPort.PortName} is denied. Please check your permissions.", "COMPort: Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                LogError(ex, $"I/O error occurred while opening the port {serialPort.PortName}.");
#if DEBUG
                MessageBox.Show($"I/O error occurred while opening the port {serialPort.PortName}. Please check the connection.", "COMPort: Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
            catch (InvalidOperationException ex)
            {
                LogError(ex, $"The specified port {serialPort.PortName} is already open.");
                MessageBox.Show($"The specified port {serialPort.PortName} is already open. Please close any other applications using this port.", "COMPort: is already open: Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Unexpected error occurred while opening the port {serialPort.PortName}.");
                MessageBox.Show($"Unexpected error occurred while opening the port {serialPort.PortName}: {ex.Message}", "COMPort: Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Initializes the settings.
        /// </summary>
        private void InitializeSettings()
        {
            InitializeTextBox(txtXaxisStepperCurrent, Properties.Settings.Default.XaxisStepperCurrent);
            InitializeTextBox(txtYaxisStepperCurrent, Properties.Settings.Default.YaxisStepperCurrent);
            InitializeTextBox(txtZaxisStepperCurrent, Properties.Settings.Default.ZaxisStepperCurrent);

            InitializeTextBox(txtXaxisMotorSpeed, Properties.Settings.Default.XaxisMotorSpeed);
            InitializeTextBox(txtYaxisMotorSpeed, Properties.Settings.Default.YaxisMotorSpeed);
            InitializeTextBox(txtZaxisMotorSpeed, Properties.Settings.Default.ZaxisMotorSpeed);

            InitializeTextBox(txtXaxisStepperMove, Properties.Settings.Default.XaxisStepperMove);
            InitializeTextBox(txtYaxisStepperMove, Properties.Settings.Default.YaxisStepperMove);
            InitializeTextBox(txtZaxisStepperMove, Properties.Settings.Default.ZaxisStepperMove);

            InitializeCheckBox(ckbXaxisResetToZero, Properties.Settings.Default.ckbXaxisResetToZeroIsChecked);
            InitializeCheckBox(ckbYaxisResetToZero, Properties.Settings.Default.ckbYaxisResetToZeroIsChecked);
            InitializeCheckBox(ckbZaxisResetToZero, Properties.Settings.Default.ckbZaxisResetToZeroIsChecked);

            xAxisAbsolutePosition = 0.00f;
            yAxisAbsolutePosition = 0.00f;
            xAxisAbsolutePosition = 0.00f;
        }
        /// <summary>
        /// Initializes a text box with the specified value.
        /// </summary>
        /// <param name="textBox">The text box to initialixe.</param>
        /// <param name="value">The value to set in the text box.</param>
        private void InitializeTextBox(TextBox textBox, decimal value)
        {
            textBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Initializes a check box with the specified value.
        /// </summary>
        /// <param name="checkBox">The check box to initialixe.</param>
        /// <param name="isChecked">The value to set in the check box.</param>
        private void InitializeCheckBox(CheckBox checkBox, bool isChecked)
        {
            checkBox.IsChecked = isChecked;
        }
        /// <summary>
        /// Initializes the logger.
        /// </summary>
        private void InitializeLogger()
        {
            // Determine the path of the assembly folder
            string assemblyFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Define the log folder path
            string logFolderPath = System.IO.Path.Combine(assemblyFolderPath, "Logs");
            string logBackUpFolderPath = System.IO.Path.Combine(assemblyFolderPath, logFolderPath, "BackUpLogs");

            // Create the log folder if it doesn't exist
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
            if (!Directory.Exists(logBackUpFolderPath))
            {
                Directory.CreateDirectory(logBackUpFolderPath);
            }

            string logFileName = "Stepper";
            string logFilePath = System.IO.Path.Combine(logFolderPath, logFileName + ".log");
            //string fileLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), logFileName + ".log");
            string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            //string fileLogPathBackup = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{logFileName}_{dateTimeString}.log");
            string fileLogPathBackup = System.IO.Path.Combine(logBackUpFolderPath, logFileName + $".log{logFileName}_{dateTimeString}.log");

            if (File.Exists(logFilePath))
            {
                if (File.Exists(fileLogPathBackup))
                {
                    File.Delete(fileLogPathBackup);
                }
                File.Move(logFilePath, fileLogPathBackup);
                File.Delete(logFilePath);
            }

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddProvider(new FileLoggerProvider(logFilePath));
            });
            Logger = LoggerFactory.CreateLogger<MainWindow>();
            LogInformation("Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            LogInformation($"Version: {displayableVersion}");
        }
        /// <summary>
        /// Starts the countdown.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="currentStepperPosition">The current stepper position.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <param name="stepperSpeed">The stepper speed.</param>
        private void StartCountdown(DateTime endTime, string axis, float currentStepperPosition, float stepperMove, float stepperSpeed)
        {
            targetEndTime = endTime;
            countdownTimer.Start();
        }
        /// <summary>
        /// Handles the Tick event of the CountdownTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = targetEndTime - DateTime.Now;
            if (remainingTime <= TimeSpan.Zero)
            {
                remainingTime = TimeSpan.Zero;
                countdownTimer.Stop();
                CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                EnableControls();
                LogInformation($"{Properties.Settings.Default.CountDownText} Completed");
                switch (CurrentAxis)
                {
                    case ("X"):
                        UpdateCurrentPosition(CurrentAxis, CompletedXAxisCurrentPosition);
                        break;
                    case ("Y"):
                        UpdateCurrentPosition(CurrentAxis, CompletedYAxisCurrentPosition);
                        break;
                    case ("Z"):
                        UpdateCurrentPosition(CurrentAxis, CompletedZAxisCurrentPosition);
                        break;
                }
                return;
            }
            // Update the UI with the remaining time
            DisableControls();
            CountdownLabel.Content = $"{Properties.Settings.Default.CountdownTimer} {remainingTime.ToString(@"hh\:mm\:ss")}";
        }
        /// <summary>
        /// Updates the current position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="currentPosition">The current position.</param>
        private void UpdateCurrentPosition(string axis, float currentPosition)
        {
            switch (axis)
            {
                case ("X"):
                    txtXaxisStepperCurrent.Text = currentPosition.ToString("F2");
                    txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                    txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    break;
                case ("Y"):
                    txtYaxisStepperCurrent.Text = currentPosition.ToString("F2");
                    txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                    txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    break;
                case ("Z"):
                    txtZaxisStepperCurrent.Text = currentPosition.ToString("F2");
                    txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                    txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    break;
            }
        }
        /// <summary>
        /// Disables the controls.
        /// </summary>
        private void DisableControls()
        {
            var controlsToDisable = new Control[]
            {
                txtXaxisStepperMove, txtYaxisStepperMove, txtZaxisStepperMove,
                txtXaxisStepperCurrent, txtYaxisStepperCurrent, txtZaxisStepperCurrent,
                txtXaxisMotorSpeed, txtYaxisMotorSpeed, txtZaxisMotorSpeed,
                ckbXaxisResetToZero, ckbYaxisResetToZero, ckbZaxisResetToZero,
                btnRunXAxis, btnRunYAxis, btnRunZAxis, btnRunXYAxis
            };

            SetControlsEnabled(controlsToDisable, false);
        }
        /// <summary>
        /// Enables the controls.
        /// </summary>
        private void EnableControls()
        {
            var controlsToEnable = new Control[]
            {
                txtXaxisStepperMove, txtYaxisStepperMove, txtZaxisStepperMove,
                txtXaxisStepperCurrent, txtYaxisStepperCurrent, txtZaxisStepperCurrent,
                txtXaxisMotorSpeed, txtYaxisMotorSpeed, txtZaxisMotorSpeed,
                ckbXaxisResetToZero, ckbYaxisResetToZero, ckbZaxisResetToZero,
                btnRunXAxis, btnRunYAxis, btnRunZAxis, btnRunXYAxis
            };

            SetControlsEnabled(controlsToEnable, true);
        }
        /// <summary>
        /// Sets the enabled state of the specified controls.
        /// </summary>
        /// <param name="controls">The controls to set the enabled state for.</param>
        /// <param name="isEnabled">if set to <c>true</c> the controls will be enabled; otherwise, they will be disabled.</param>
        private void SetControlsEnabled(Control[] controls, bool isEnabled)
        {
            foreach (var control in controls)
            {
                control.IsEnabled = isEnabled;
            }
        }
        /// <summary>
        /// X in data received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Xsp = (SerialPort)sender;
                string indata = Xsp.ReadExisting();
                string axis = "X";
                LogInformation($"{axis} Axis Data Received: {indata}");

                XAxisTargetReached = true;
                xAxisRunToCompletion = true;

                if (indata.Contains($" {axis} Axis CW STOPPED") || indata.Contains($" {axis} Axis CCW STOPPED"))
                {
                    LogInformation($" Zindata Contains: {axis} Axis CW STOPPED or {axis} Axis CCW STOPPED");
                    if (indata.Contains($" {axis} Axis CW Motor Current Position:") || indata.Contains($" {axis} Axis CCW Motor Current Position:"))
                    {
                        UpdateCurrentPosition(axis, CompletedZAxisCurrentPosition);
                        UpdateLimitSwitchMotorPosition(axis, indata);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in XdataReceivedHandler");
                MessageBox.Show($"{ex} Error in XdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Y in data received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void YdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Ysp = (SerialPort)sender;
                string indata = Ysp.ReadExisting();
                string axis = "Y";
                //LogInformation($"{axis} Axis Data Received: {indata}");

                YAxisTargetReached = true; // Set the flag to indicate the target is reached
                yAxisRunToCompletion = true;

                if (indata.Contains($" {axis} Axis CW STOPPED") || indata.Contains($" {axis} Axis CCW STOPPED"))
                {
                    LogInformation($" Zindata Contains: {axis} Axis CW STOPPED or {axis} Axis CCW STOPPED");
                    if (indata.Contains($" {axis} Axis CW Motor Current Position:") || indata.Contains($" {axis} Axis CCW Motor Current Position:"))
                    {
                        UpdateCurrentPosition(axis, CompletedZAxisCurrentPosition);
                        UpdateLimitSwitchMotorPosition(axis, indata);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in YdataReceivedHandler");
                MessageBox.Show($"{ex} Error in YdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        /// <summary>
        /// Z in data received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void ZdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Zsp = (SerialPort)sender;
                string indata = Zsp.ReadExisting();
                String axis = "Z";
                if (indata.Contains($" {axis} Axis CW STOPPED") || indata.Contains($" {axis} Axis CCW STOPPED"))
                {
                    LogInformation($" Zindata Contains: {axis} Axis CW STOPPED or {axis} Axis CCW STOPPED");
                    if (indata.Contains($" {axis} Axis CW Motor Current Position:") || indata.Contains($" {axis} Axis CCW Motor Current Position:"))
                    {
                        UpdateCurrentPosition(axis, CompletedZAxisCurrentPosition);
                        UpdateLimitSwitchMotorPosition(axis, indata);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in ZdataReceivedHandler");
#if DEBUG
                MessageBox.Show($"{ex} Error in ZdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
            }
        }
        private void UpdateLimitSwitchMotorPosition(string axis, string data)
        {
            string[] lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;
                float distanceInMM = 0.00f;
                if (line.Contains($"{axis} Axis CW Motor Current Position:") || line.Contains($"{axis} Axis CCW Motor Current Position:"))
                {
                    string positionString = line.Replace($"{axis} Axis CW Motor Current Position:", "")
                                                .Replace($"{axis} Axis CCW Motor Current Position:", "")
                                                .Trim();
                    if (float.TryParse(positionString, out float currentPosition))
                    {
                        distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
                        if (line.Contains("CW"))
                        {
                            UpdateAxisPosition(axis, distanceInMM, line.Contains("CW"));
                        }
                        if (line.Contains("CCW"))
                        {
                            UpdateAxisPosition(axis, -distanceInMM, line.Contains("CCW"));
                        }
                    }
                }
            }
        }
        private void UpdateAxisPosition(string axis, float distanceInMM, bool isClockwise)
        {
            float newPosition = isClockwise ? distanceInMM : -distanceInMM;
            switch (axis)
            {
                case "X":
                    xAxisAbsolutePosition += newPosition;
                    txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                    break;
                case "Y":
                    yAxisAbsolutePosition += newPosition;
                    txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                    break;
                case "Z":
                    zAxisAbsolutePosition += newPosition;
                    txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2");
                    break;
            }
            Properties.Settings.Default.Save();
            Logger.LogInformation($"{axis} Axis Motor Current Position: {newPosition}");
        }

        /// <summary>
        /// Handles the Click event of the AxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void AxisRun_Click(object sender, RoutedEventArgs e)
        {
            switch (sender)
            {
                case Button button when button == btnRunXAxis:
                    {
                        String axis = "X";
                        await RunAxis(axis, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort);
                        break;
                    }
                case Button button when button == btnRunYAxis:
                    {
                        String axis = "Y";
                        await RunAxis(axis, txtYaxisStepperMove, txtYaxisStepperCurrent, txtYaxisMotorSpeed, ckbYaxisResetToZero, ySerialPort);
                        break;
                    }
                case Button button when button == btnRunZAxis:
                    {
                        String axis = "Z";
                        await RunAxis(axis, txtZaxisStepperMove, txtZaxisStepperCurrent, txtZaxisMotorSpeed, ckbZaxisResetToZero, zSerialPort);
                        break;
                    }
                case Button button when button == btnRunXYAxis:
                    {
                        String axis = "X";
                        await RunAxis(axis, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort);
                        axis = "Y";
                        await RunAxis(axis, txtYaxisStepperMove, txtYaxisStepperCurrent, txtYaxisMotorSpeed, ckbYaxisResetToZero, ySerialPort);
                        break;
                    }
            }
        }
        /// <summary>
        /// Runs the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="StepperCurrent">The stepper current.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        /// <exception cref="System.ArgumentException">'{nameof(axis)}' cannot be null or empty. - axis</exception>
        /// <exception cref="System.ArgumentNullException">serialPort</exception>
        private async Task RunAxis(string axis, TextBox stepperMoveTextBox, TextBox StepperCurrent, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, SerialPort serialPort)
        {
            LogInformation($"{axis} Axis Run button clicked:");

            try
            {
                if (resetToZeroCheckBox.IsChecked == true)
                {
                    await ResetAxisToZero(axis, serialPort);
                }
                else
                {
                    float currentStepperPosition = float.Parse(StepperCurrent.Text.Trim(), CultureInfo.InvariantCulture);
                    float moveDistance = float.Parse(stepperMoveTextBox.Text, CultureInfo.InvariantCulture);
                    float motorSpeed = float.Parse(motorSpeedTextBox.Text, CultureInfo.InvariantCulture);
                    MoveAxis(axis, serialPort, currentStepperPosition, moveDistance, motorSpeed);
                }
            }
            catch (Exception ex)
            {
                LogInformation($"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", "Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Resets the axis to xero.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="serialPort">The serial port.</param>
        private async Task ResetAxisToZero(string axis, SerialPort serialPort)
        {
            string command = string.Empty;
            switch (axis)
            {
                case "X":
                    command = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text.Trim()},1,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        xAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        LogError(ex, "Error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        LogError(ex, "Overflow error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Y":
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text.Trim()},1,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        yAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Z":
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtZaxisMotorSpeed.Text.Trim()},1";
                    try
                    {
                        zAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "XY":
                    command = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text.Trim()},1,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        xAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        LogError(ex, "Error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        LogError(ex, "Overflow error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text.Trim()},1,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        yAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;

            }

            serialPort.Write(command);
            LogInformation(message: $"{axis} Axis Run Event to reset Axis to xero: {command}");
            await ZeroAxis(axis);
        }
        /// <summary>
        /// Moves the axis to new position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <param name="stepperSpeed">The stepper speed.</param>
        private void MoveAxis(string axis, SerialPort serialPort, float currentPosition, float stepperMove, float stepperSpeed)
        {
            try
            {
                decimal motorMovementSeconds = UpdateMotorTimer(axis, Convert.ToDecimal(stepperSpeed), Convert.ToDecimal(stepperMove));
                int movementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(motorMovementSeconds);
                TimeSpan countdownTime = TimeSpan.FromMilliseconds(movementTimer);
                DateTime targetEndTime = DateTime.Now.Add(countdownTime);
                CurrentAxis = axis;
                currentPosition += stepperMove;
                string command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                serialPort.Write(command);
                StartCountdown(targetEndTime, axis, stepperMove, currentPosition, stepperSpeed);
                switch (axis)
                {
                    case ("X"):
                        CompletedXAxisCurrentPosition = currentPosition;
                        break;
                    case ("Y"):
                        CompletedYAxisCurrentPosition = currentPosition;
                        break;
                    case ("Z"):
                        CompletedZAxisCurrentPosition = currentPosition;
                        break;
                }
                LogInformation($"{axis} Axis Run Event: {command}");
            }
            catch (FormatException ex)
            {
                LogError(ex, $"Error converting stepper current text to float for {axis} axis.");
            }
            catch (OverflowException ex)
            {
                LogError(ex, $"Overflow error converting stepper current text to float for {axis} axis.");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Unexpected error in MoveAxis for {axis} axis.");
            }
        }
        /// <summary>
        /// Updates the xero status.
        /// </summary>
        private void UpdateZeroStatus()
        {
            ZeroXaxis = ckbXaxisResetToZero.IsChecked == true ? 1 : 0;
            ZeroYaxis = ckbYaxisResetToZero.IsChecked == true ? 1 : 0;
            ZeroZaxis = ckbZaxisResetToZero.IsChecked == true ? 1 : 0;

            LogInformation($"Updated Zero Status - X: {ZeroXaxis}, Y: {ZeroYaxis}, Z: {ZeroZaxis}");
        }
        /// <summary>
        /// Determines whether the specified number is negative.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns><c>true</c> if the specified number is negative; otherwise, <c>false</c>.</returns>
        public bool IsNegative(decimal number) => number < 0;
        /// <summary>
        /// Updates the motor timer.
        /// </summary>
        /// <param name="Axis">The axis.</param>
        /// <param name="MotorSpeed">The motor speed.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <returns>System.Decimal.</returns>
        private decimal UpdateMotorTimer(string Axis, decimal MotorSpeed, decimal stepperMove)
        {
            if (IsNegative(stepperMove))
            {
                stepperMove = Math.Abs(stepperMove);
            }
            decimal MotorMovementSeconds = 0.00m;

            if (MotorSpeed <= 100.00m)
            {
                MotorMovementSeconds = stepperMove / 1;
            }
            else if (MotorSpeed <= 200.00m)
            {
                MotorMovementSeconds = stepperMove / 3;
            }
            else if (MotorSpeed <= 300.00m)
            {
                MotorMovementSeconds = stepperMove / 5;
            }
            else if (MotorSpeed <= 400.00m)
            {
                MotorMovementSeconds = stepperMove / 7;
            }
            else if (MotorSpeed <= 500.00m)
            {
                MotorMovementSeconds = stepperMove / 9;
            }
            else if (MotorSpeed <= 600.00m)
            {
                MotorMovementSeconds = stepperMove / 11;
            }
            else if (MotorSpeed <= 700.00m)
            {
                MotorMovementSeconds = stepperMove / 13;
            }
            else if (MotorSpeed <= 800.00m)
            {
                MotorMovementSeconds = stepperMove / 15;
            }
            else if (MotorSpeed <= 900.00m)
            {
                MotorMovementSeconds = stepperMove / 17;
            }
            else if (MotorSpeed <= 1000.00m)
            {
                MotorMovementSeconds = stepperMove / 19;
            }

            LogInformation($"{Axis} Axis MotorMovementSeconds: {MotorMovementSeconds}");
            return MotorMovementSeconds;
        }
        /// <summary>
        /// Handles the Click event of the AxisPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
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
                    LogInformation(message: $"{buttonText} {portName} not connected.");
                }
            }

            portButton.Content = $"{buttonText} {portName}";
        }
        /// <summary>
        /// Zeroes the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        private async Task ZeroAxis(string axis)
        {
            LogInformation($"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(Properties.Settings.Default.Milliseconds);

            decimal ZeroValue = Properties.Settings.Default.Value_0_00;
            string ZeroValueString = ZeroValue.ToString("F2");

            switch (axis)
            {
                case "X":
                    UpdateAxisToZero(txtXaxisStepperCurrent, txtXaxisStepperMove, ckbXaxisResetToZero, ZeroValue, ZeroValueString);
                    Properties.Settings.Default.XaxisStepperCurrent = ZeroValue;
                    Properties.Settings.Default.XaxisStepperMove = ZeroValue;
                    break;
                case "Y":
                    UpdateAxisToZero(txtYaxisStepperCurrent, txtYaxisStepperMove, ckbYaxisResetToZero, ZeroValue, ZeroValueString);
                    Properties.Settings.Default.YaxisStepperCurrent = ZeroValue;
                    Properties.Settings.Default.YaxisStepperMove = ZeroValue;
                    break;
                case "Z":
                    UpdateAxisToZero(txtZaxisStepperCurrent, txtZaxisStepperMove, ckbZaxisResetToZero, ZeroValue, ZeroValueString);
                    Properties.Settings.Default.ZaxisStepperCurrent = ZeroValue;
                    Properties.Settings.Default.ZaxisStepperMove = ZeroValue;
                    break;
                case "XY":
                    UpdateAxisToZero(txtXaxisStepperCurrent, txtXaxisStepperMove, ckbXaxisResetToZero, ZeroValue, ZeroValueString);
                    UpdateAxisToZero(txtYaxisStepperCurrent, txtYaxisStepperMove, ckbYaxisResetToZero, ZeroValue, ZeroValueString);
                    Properties.Settings.Default.XaxisStepperCurrent = ZeroValue;
                    Properties.Settings.Default.XaxisStepperMove = ZeroValue;
                    Properties.Settings.Default.YaxisStepperCurrent = ZeroValue;
                    Properties.Settings.Default.YaxisStepperMove = ZeroValue;
                    break;
            }

            Properties.Settings.Default.Save();
            LogInformation($"{axis} Axis Current Location Set to Zero");
        }
        /// <summary>
        /// Updates the axis to xero.
        /// </summary>
        /// <param name="stepperCurrentTextBox">The stepper current text box.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to xero CheckBox.</param>
        /// <param name="ZeroValue">The Zero value.</param>
        /// <param name="ZeroValueString">The Zero value string.</param>
        private void UpdateAxisToZero(TextBox stepperCurrentTextBox, TextBox stepperMoveTextBox, CheckBox resetToZeroCheckBox, decimal ZeroValue, string ZeroValueString)
        {
            stepperCurrentTextBox.Text = ZeroValueString;
            stepperMoveTextBox.Text = ZeroValueString;
            resetToZeroCheckBox.IsChecked = false;
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
        /// Handles the GotFocus event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                switch (textBox.Name)
                {
                    case nameof(txtXaxisStepperMove):
                        XaxisStepperMoveTemp = textBox.Text;
                        break;
                    case nameof(txtYaxisStepperMove):
                        YaxisStepperMoveTemp = textBox.Text;
                        break;
                    case nameof(txtZaxisStepperMove):
                        ZaxisStepperMoveTemp = textBox.Text;
                        break;
                }
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
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
                        LogInformation(message: $"{axis} Axis error occurred: {ex.Message}");
                        MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void AxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            HandleAxisStepperMove(sender as TextBox, "mouse");
        }

        /// <summary>
        /// Handles the TouchUp event of the AxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void AxisStepperMove_TouchUp(object sender, TouchEventArgs e)
        {
            HandleAxisStepperMove(sender as TextBox, "touch");
        }

        /// <summary>
        /// Handles the AxisStepperMove event.
        /// </summary>
        /// <param name="textBox">The TextBox control.</param>
        /// <param name="inputType">The input type (mouse or touch).</param>
        private void HandleAxisStepperMove(TextBox textBox, string inputType)
        {
            if (textBox == null) return;

            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                textBox.Text = mainWindow.Result.ToString();
                LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.FormatException">Invalid format for motor speed value.</exception>
        private void AxisMotorSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            string axis = textBox.Name switch
            {
                nameof(txtXaxisMotorSpeed) => "X",
                nameof(txtYaxisMotorSpeed) => "Y",
                nameof(txtZaxisMotorSpeed) => "Z",
                _ => throw new ArgumentException("Invalid TextBox name", nameof(sender))
            };

            decimal motorSpeedValue = axis switch
            {
                "X" => Properties.Settings.Default.XaxisMotorSpeed,
                "Y" => Properties.Settings.Default.YaxisMotorSpeed,
                "Z" => Properties.Settings.Default.ZaxisMotorSpeed,
                _ => throw new ArgumentException("Invalid axis", nameof(axis))
            };

            if (textBox.Text == motorSpeedValue.ToString())
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                try
                {
                    if (decimal.TryParse(textBox.Text, out decimal newValue))
                    {
                        Properties.Settings.Default[$"{axis}axisMotorSpeed"] = newValue;
                        Properties.Settings.Default.Save();
                        LogInformation($"{axis} Axis motor speed updated to {newValue}");
                    }
                    else
                    {
                        throw new FormatException("Invalid format for motor speed value.");
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, $"{axis} Axis error occurred while updating motor speed.");
                    MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", "Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void AxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            HandleMotorSpeedInput(sender as TextBox, "mouse");
        }

        /// <summary>
        /// Handles the TouchUp event of the AxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void AxisMotorSpeed_TouchUp(object sender, TouchEventArgs e)
        {
            HandleMotorSpeedInput(sender as TextBox, "touch");
        }

        /// <summary>
        /// Handles the motor speed input.
        /// </summary>
        /// <param name="textBox">The TextBox control.</param>
        /// <param name="inputType">The input type (mouse or touch).</param>
        private void HandleMotorSpeedInput(TextBox textBox, string inputType)
        {
            if (textBox == null) return;

            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                textBox.Text = mainWindow.Result.ToString();
                LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
            }
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the AxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void AxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            HandleAxisStepperCurrentInput(sender as TextBox, "mouse");
        }

        /// <summary>
        /// Handles the TouchUp event of the AxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        private void AxisStepperCurrent_TouchUp(object sender, TouchEventArgs e)
        {
            HandleAxisStepperCurrentInput(sender as TextBox, "touch");
        }

        /// <summary>
        /// Handles the AxisStepperCurrent input.
        /// </summary>
        /// <param name="textBox">The TextBox control.</param>
        /// <param name="inputType">The input type (mouse or touch).</param>
        private void HandleAxisStepperCurrentInput(TextBox textBox, string inputType)
        {
            if (textBox == null) return;

            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
            {
                textBox.Text = mainWindow.Result.ToString();
                LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
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
            LogInformation(message: $"Stepper Motor Controller Loading Application Settings form.");
            // Show the new window
            NewSettingsWindow.Activate();
            NewSettingsWindow.ShowDialog();
        }
        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LogInformation(message: $"Stepper Motor Controller MainWindow loaded");
            txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            LogInformation(message: "Set MainWindow Media Brushes to White.");
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
                LogInformation("Stepper Motor Controller MainWindow Closing.");

                // Close serial ports
                CloseSerialPort(xSerialPort, "X");
                CloseSerialPort(ySerialPort, "Y");
                CloseSerialPort(xSerialPort, "Z");

                // Save settings
                Properties.Settings.Default.Save();
                LogInformation("Properties Settings Saved.");

                // Close settings window
                if (NewSettingsWindow != null)
                {
                    NewSettingsWindow.Close();
                    LogInformation("Properties Settings Form Closed.");
                }

                LogInformation("Stepper Motor Controller Form Closed.");
            }
            catch (IOException ioex)
            {
                LogError(ioex, "Error occurred while closing the MainWindow.");
                MessageBox.Show($"An error occurred while closing the application: {ioex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                LogError(ex, "Unexpected error occurred while closing the MainWindow.");
                MessageBox.Show($"An unexpected error occurred while closing the application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Closes the specified serial port.
        /// </summary>
        /// <param name="serialPort">The serial port to close.</param>
        /// <param name="axis">The axis associated with the serial port.</param>
        private void CloseSerialPort(SerialPort serialPort, string axis)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                    LogInformation($"SerialPort for {axis} Axis closed.");
                }
                catch (IOException ioex)
                {
                    LogError(ioex, $"Error occurred while closing the {axis} Axis SerialPort.");
                    MessageBox.Show($"An error occurred while closing the {axis} Axis SerialPort: {ioex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

    }
}