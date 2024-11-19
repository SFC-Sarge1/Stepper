// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 11-18-2024
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
        /// The z position updated
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
        /// The z cancellation token source
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
        /// The x limit switch press
        /// </summary>
        private static bool _xLimitSwitchPress = false;
        /// <summary>
        /// The y limit switch press
        /// </summary>
        private static bool _yLimitSwitchPress = false;
        /// <summary>
        /// The z limit switch press
        /// </summary>
        private static bool _zLimitSwitchPress = false;
        /// <summary>
        /// The limit switch pressed
        /// </summary>
        public static bool LimitSwitchPressed = false;
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
        public int ZeroZaxis { get; private set; } = Properties.Settings.Default.zeroYaxis;
        /// <summary>
        /// The xaxis changed
        /// </summary>
        public bool XaxisChanged = Properties.Settings.Default.YaxisChanged;
        /// <summary>
        /// The yaxis changed
        /// </summary>
        public bool YaxisChanged = Properties.Settings.Default.YaxisChanged;
        /// <summary>
        /// The zaxis changed
        /// </summary>
        public bool ZaxisChanged = Properties.Settings.Default.YaxisChanged;
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
        public bool xAxisRunToCompletion = false;
        /// <summary>
        /// The y axis run completed
        /// </summary>
        public bool yAxisRunToCompletion = false;
        /// <summary>
        /// The z axis run completed
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
        /// The z axis absolute position
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
        /// The z axis clear absolute position
        /// </summary>
        private static bool zAxisClearAbsolutePosition;
        /// <summary>
        /// The esp32 rebooted
        /// </summary>
        public static int esp32Rebooted = 0;
        // Add a flag to indicate if the Z axis has reached its target position
        /// <summary>
        /// The x axis target reached
        /// </summary>
        public bool xAxisTargetReached = false;
        /// <summary>
        /// The y axis target reached
        /// </summary>
        public bool yAxisTargetReached = false;
        /// <summary>
        /// The z axis target reached
        /// </summary>
        public bool zAxisTargetReached = false;

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

            Logger.LogInformation("Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            Logger.LogInformation($"Version: {displayableVersion}");
            ResizeMode = ResizeMode.NoResize;
#if DEBUG
            UpdateVersionInfo();
#endif
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            NewSettingsWindow = new StepperAppSettings();
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
            Logger.LogInformation($"Version: {displayableVersion}");
            VersionTxt.Text = $"Version: {displayableVersion}";
            XDocument doc = XDocument.Load(Properties.Settings.Default.ProjectFilePath);
            string versionString = $"{major}.{minor}.{build}.{revision}";
            UpdateVersionElement(doc, "AssemblyVersion", versionString);
            UpdateVersionElement(doc, "FileVersion", versionString);
            doc.Save(Properties.Settings.Default.ProjectFilePath);
            Properties.Settings.Default.BuildVersion = $"Version: {displayableVersion}";
            Logger.LogInformation($"Version: {displayableVersion}");
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
                Logger.LogInformation($"Updated {elementName} to version {version}.");
            }
            else
            {
                Logger.LogWarning($"Element {elementName} not found in the provided XML document.");
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
        /// <exception cref="System.ArgumentNullException">serialPort</exception>
        private void OpenSerialPort(SerialPort serialPort)
        {
            if (serialPort == null)
            {
                Logger.LogError("SerialPort is null.");
                throw new ArgumentNullException(nameof(serialPort));
            }

            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.LogInformation($"SerialPort {serialPort.PortName} is already open.");
                    return;
                }

                serialPort.Open();
                Logger.LogInformation($"SerialPort {serialPort.PortName} opened successfully.");

                if (serialPort.BytesToRead > 0)
                {
                    int bytesRead = serialPort.Read(message, 0, serialPort.BytesToRead);
                    Logger.LogInformation($"SerialPort {serialPort.PortName} read {bytesRead} bytes.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(ex, $"Access to the port {serialPort.PortName} is denied.");
                MessageBox.Show($"Access to the port {serialPort.PortName} is denied. Please check your permissions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                Logger.LogError(ex, $"I/O error occurred while opening the port {serialPort.PortName}.");
                MessageBox.Show($"I/O error occurred while opening the port {serialPort.PortName}. Please check the connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, $"The specified port {serialPort.PortName} is already open.");
                MessageBox.Show($"The specified port {serialPort.PortName} is already open. Please close any other applications using this port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Unexpected error occurred while opening the port {serialPort.PortName}.");
                MessageBox.Show($"Unexpected error occurred while opening the port {serialPort.PortName}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            zAxisAbsolutePosition = 0.00f;
        }

        /// <summary>
        /// Initializes a text box with the specified value.
        /// </summary>
        /// <param name="textBox">The text box to initialize.</param>
        /// <param name="value">The value to set in the text box.</param>
        private void InitializeTextBox(TextBox textBox, decimal value)
        {
            textBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Initializes a check box with the specified value.
        /// </summary>
        /// <param name="checkBox">The check box to initialize.</param>
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
            string logFileName = "Stepper";
            string fileLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), logFileName + ".log");
            string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileLogPathBackup = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{logFileName}_{dateTimeString}.log");

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
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (sender == xTimer)
            {
                xAxisRunToCompletion = true;
                HandleTimerTick("X", xStopwatch, xTargetEndTime, ref xAxisRunToCompletion, ref xAxisClearAbsolutePosition, ref xAxisAbsolutePosition, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, ref XaxisChanged, ref LimitSwitchPressed);
            }
            else if (sender == yTimer)
            {
                yAxisRunToCompletion = true;
                HandleTimerTick("Y", yStopwatch, yTargetEndTime, ref yAxisRunToCompletion, ref yAxisClearAbsolutePosition, ref yAxisAbsolutePosition, txtYaxisStepperMove, txtYaxisStepperCurrent, txtYaxisMotorSpeed, ckbYaxisResetToZero, ref YaxisChanged, ref LimitSwitchPressed);
            }
            else if (sender == zTimer)
            {
                zAxisRunToCompletion = true;
                HandleTimerTick("Y", zStopwatch, zTargetEndTime, ref zAxisRunToCompletion, ref zAxisClearAbsolutePosition, ref zAxisAbsolutePosition, txtZaxisStepperMove, txtZaxisStepperCurrent, txtZaxisMotorSpeed, ckbZaxisResetToZero, ref ZaxisChanged, ref LimitSwitchPressed);
            }
        }

        /// <summary>
        /// Handles the timer tick.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="targetEndTime">The target end time.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run completed].</param>
        /// <param name="axisClearAbsolutePosition">if set to <c>true</c> [axis clear absolute position].</param>
        /// <param name="axisAbsolutePosition">The axis absolute position.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="stepperCurrentTextBox">The stepper current text box.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="axisChanged">if set to <c>true</c> [axis changed].</param>
        /// <param name="LimitSwitchPressed">if set to <c>true</c> [Limit Switch Pressed].</param></param>
        private void HandleTimerTick(string axis, Stopwatch stopwatch, DateTime targetEndTime, ref bool axisRunToCompletion, ref bool axisClearAbsolutePosition, ref float axisAbsolutePosition, TextBox stepperMoveTextBox, TextBox stepperCurrentTextBox, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, ref bool axisChanged, ref bool LimitSwitchPressed)
        {
            if (axisRunToCompletion)
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
                    // Stop the timer and prevent further updates
                    if (axis == "X")
                    {
                        xTimer.Stop();
                        xStopwatch.Stop();
                    }
                    if (axis == "Y")
                    {
                        yTimer.Stop();
                        yStopwatch.Stop();
                    }
                    if (axis == "Z")
                    {
                        zTimer.Stop();
                        zStopwatch.Stop();
                    }
                    Logger.LogInformation($"Stepper Motor Controller Timer Stopped.");
                    stopwatch.Reset();
                    Logger.LogInformation($"Stepper Motor Controller Stopwatch Reset.");
                }
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
        /// Xdatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Xsp = (SerialPort)sender;
                string Xindata = Xsp.ReadExisting();
                Logger.LogInformation($"X Axis Data Received: {Xindata}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    xAxisTargetReached = true; // Set the flag to indicate the target is reached
                    if (Xindata.Contains("X Axis CW STOPPED") || Xindata.Contains("X Axis CCW STOPPED"))
                    {
                        HandleXAxisStop(Xindata, xAxisTargetReached);
                    }
                    else if (Xindata.Contains("X Axis Absolute Position"))
                    {
                        UpdateXAxisAbsolutePosition(Xindata, xAxisTargetReached);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in XdataReceivedHandler");
                MessageBox.Show($"{ex} Error in XdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handles the x axis stop.
        /// </summary>
        /// <param name="Xindata">The xindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void HandleXAxisStop(string Xindata, bool AxisTargetReached)
        {
            xTimer.Stop();
            xStopwatch.Stop();
            _xCancellationTokenSource.Cancel();
            _xLimitSwitchPress = true;
            LimitSwitchPressed = true;

            EnableControls();
            XaxisChanged = true;
            ckbXaxisResetToZero.IsChecked = false;
            xAxisClearAbsolutePosition = false;
            xAxisTargetReached = true; // Set the flag to indicate the target is reached
            CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";

            StartDelayTask("X", Xindata, AxisTargetReached);
            Logger.LogInformation("X Axis Motor Stopped");
        }

        /// <summary>
        /// Updates the x axis absolute position.
        /// </summary>
        /// <param name="Xindata">The xindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateXAxisAbsolutePosition(string Xindata, bool AxisTargetReached)
        {
            if (AxisTargetReached) return; // Do not update if the target is reached
            string[] XindataArrax = Xindata.Split(' ');
            if (XindataArrax.Length > 4 && float.TryParse(XindataArrax[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float currentPosition))
            {
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;
                float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;

                xAxisAbsolutePosition += distanceInMM;
                txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture);
                Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(xAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture));
                Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                Properties.Settings.Default.Save();

                xAxisRunToCompletion = false;
                xAxisClearAbsolutePosition = true;
                Logger.LogInformation($"X Axis Absolute Position: {txtXaxisStepperCurrent.Text}");
            }
        }

        /// <summary>
        /// Ydatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void YdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Ysp = (SerialPort)sender;
                string Yindata = Ysp.ReadExisting();
                Logger.LogInformation($"Y Axis Data Received: {Yindata}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    yAxisTargetReached = true; // Set the flag to indicate the target is reached
                    if (Yindata.Contains("Y Axis CW STOPPED") || Yindata.Contains("Y Axis CCW STOPPED"))
                    {
                        HandleYAxisStop(Yindata, yAxisTargetReached);
                    }
                    else if (Yindata.Contains("Y Axis Absolute Position"))
                    {
                        UpdateYAxisAbsolutePosition(Yindata, yAxisTargetReached);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in YdataReceivedHandler");
                MessageBox.Show($"{ex} Error in YdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handles the y axis stop.
        /// </summary>
        /// <param name="Yindata">The yindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void HandleYAxisStop(string Yindata, bool AxisTargetReached)
        {
            yTimer.Stop();
            yStopwatch.Stop();
            _yCancellationTokenSource.Cancel();
            _yLimitSwitchPress = true;
            LimitSwitchPressed = true;

            EnableControls();
            YaxisChanged = true;
            ckbYaxisResetToZero.IsChecked = false;
            yAxisClearAbsolutePosition = false;
            yAxisTargetReached = true; // Set the flag to indicate the target is reached
            CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";

            StartDelayTask("Y", Yindata, AxisTargetReached);
            Logger.LogInformation("Y Axis Motor Stopped");
        }

        /// <summary>
        /// Updates the y axis absolute position.
        /// </summary>
        /// <param name="Yindata">The yindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateYAxisAbsolutePosition(string Yindata, bool AxisTargetReached)
        {
            if (AxisTargetReached) return; // Do not update if the target is reached
            string[] YindataArray = Yindata.Split(' ');
            if (YindataArray.Length > 4 && float.TryParse(YindataArray[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float currentPosition))
            {
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;
                float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;

                yAxisAbsolutePosition += distanceInMM;
                txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture);
                Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(yAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture));
                Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                Properties.Settings.Default.Save();

                yAxisRunToCompletion = false;
                yAxisClearAbsolutePosition = true;
                Logger.LogInformation($"Y Axis Absolute Position: {txtYaxisStepperCurrent.Text}");
            }
        }

        /// <summary>
        /// Zdatas the received handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void ZdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort Zsp = (SerialPort)sender;
                string Zindata = Zsp.ReadExisting();

                Logger.LogInformation($"Z Axis Data Received: {Zindata}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    zAxisTargetReached = true; // Set the flag to indicate the target is reached
                    if (Zindata.Contains("Z Axis CW STOPPED") || Zindata.Contains("Z Axis CCW STOPPED"))
                    {
                        HandleZAxisStop(Zindata, zAxisTargetReached);
                    }
                    if (Zindata.Contains("Z Axis CW Motor Current Position:") || Zindata.Contains("Z Axis CCW Motor Current Position:"))
                    {
                        UpdateZAxisAbsolutePosition(Zindata, zAxisTargetReached);
                    }
                        
                    else if (Zindata.Contains("Z Axis Absolute Position"))
                    {
                        UpdateZAxisAbsolutePosition(Zindata, zAxisTargetReached);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ZdataReceivedHandler");
                MessageBox.Show($"{ex} Error in ZdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handles the z axis stop.
        /// </summary>
        /// <param name="Zindata">The zindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void HandleZAxisStop(string Zindata, bool AxisTargetReached)
        {
            zTimer.Stop();
            zStopwatch.Stop();
            _zCancellationTokenSource.Cancel();
            LimitSwitchPressed = true;

            EnableControls();
            ZaxisChanged = true;
            ckbZaxisResetToZero.IsChecked = false;
            zAxisClearAbsolutePosition = false;
            _zLimitSwitchPress = true;
            zAxisTargetReached = true; // Set the flag to indicate the target is reached
            CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";

            StartDelayTask("Z", Zindata, AxisTargetReached);
            Logger.LogInformation("Z Axis Motor Stopped");
        }

        /// <summary>
        /// Updates the z axis absolute position.
        /// </summary>
        /// <param name="Zindata">The zindata.</param>
        /// <param name="AxisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateZAxisAbsolutePosition(string Zindata, bool AxisTargetReached)
        {
            if (AxisTargetReached) return; // Do not update if the target is reached

            string[] ZindataArray = Zindata.Split(' ');
            if (ZindataArray.Length > 4 && float.TryParse(ZindataArray[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float currentPosition))
            {
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;
                float distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;

                zAxisAbsolutePosition += distanceInMM;
                txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture);
                Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2", CultureInfo.InvariantCulture));
                Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                Properties.Settings.Default.Save();

                zAxisRunToCompletion = false;
                zAxisClearAbsolutePosition = true;
                Logger.LogInformation($"Z Axis Absolute Position: {txtZaxisStepperCurrent.Text}");
            }
        }
        /// <summary>
        /// Handles the Click event of the AxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public async void AxisRun_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnRunXAxis)
            {
                _xLimitSwitchPress = false;
                xAxisTargetReached = false; // Set the flag to indicate the target is reached
                await RunAxisAsync("X", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort, xAxisRunToCompletion, xAxisTargetReached, _xCancellationTokenSource);
            }
            else if (sender == btnRunYAxis)
            {
                _yLimitSwitchPress = false;
                yAxisTargetReached = false; // Set the flag to indicate the target is reached
                await RunAxisAsync("Y", txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, ySerialPort, yAxisRunToCompletion, yAxisTargetReached, _yCancellationTokenSource);
            }
            else if (sender == btnRunZAxis)
            {
                _zLimitSwitchPress = false;
                zAxisTargetReached = false; // Set the flag to indicate the target is reached
                await RunAxisAsync("Z", txtZaxisStepperMove, txtZaxisMotorSpeed, ckbZaxisResetToZero, zSerialPort, zAxisRunToCompletion, zAxisTargetReached, _zCancellationTokenSource);
            }
            else if (sender == btnRunXYAxis)
            {
                _xLimitSwitchPress = false;
                xAxisTargetReached = false; // Set the flag to indicate the target is reached
                await RunAxisAsync("X", txtXaxisStepperMove, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort, xAxisRunToCompletion, xAxisTargetReached, _xCancellationTokenSource);
                while (!xAxisRunToCompletion)
                {
                    await Task.Delay(100);
                }
                _yLimitSwitchPress = false;
                yAxisTargetReached = false; // Set the flag to indicate the target is reached
                await RunAxisAsync("Y", txtYaxisStepperMove, txtYaxisMotorSpeed, ckbYaxisResetToZero, ySerialPort, yAxisRunToCompletion, yAxisTargetReached, _yCancellationTokenSource);
            }
        }

        /// <summary>
        /// Runs the specified axis asynchronously.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero check box.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">The axis run to completion flag.</param>
        /// <param name="axisTargetReached">The axis target reached flag.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task RunAxisAsync(string axis, TextBox stepperMoveTextBox, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, SerialPort serialPort, bool axisRunToCompletion, bool axisTargetReached, CancellationTokenSource cancellationTokenSource)
        {
            await RunAxis(axis, stepperMoveTextBox, motorSpeedTextBox, resetToZeroCheckBox, serialPort, axisRunToCompletion, axisTargetReached, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Runs the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="System.ArgumentException">'{nameof(axis)}' cannot be null or empty. - axis</exception>
        /// <exception cref="System.ArgumentNullException">serialPort</exception>
        private async Task RunAxis(string axis, TextBox stepperMoveTextBox, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, SerialPort serialPort, bool axisRunToCompletion, bool axisTargetReached, CancellationToken token)
        {
            if (string.IsNullOrEmpty(axis))
            {
                throw new ArgumentException($"'{nameof(axis)}' cannot be null or empty.", nameof(axis));
            }

            if (serialPort is null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            Logger.LogInformation($"{axis} Axis Run button clicked:");

            try
            {
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;

                if (resetToZeroCheckBox.IsChecked == true)
                {
                    await ResetAxisToZero(axis, serialPort);
                }
                else
                {
                    float currentStepperPosition = float.Parse(stepperMoveTextBox.Text.Trim(), CultureInfo.InvariantCulture) / stepsPerRevolution * distancePerRevolution;
                    float moveDistance = float.Parse(stepperMoveTextBox.Text, CultureInfo.InvariantCulture) / stepsPerRevolution * distancePerRevolution;
                    float motorSpeed = float.Parse(motorSpeedTextBox.Text, CultureInfo.InvariantCulture);
                    await HandleAxisRun("Y", stepperMoveTextBox, motorSpeedTextBox, resetToZeroCheckBox, serialPort, axisRunToCompletion, axisTargetReached, stepsPerRevolution, distancePerRevolution, token);
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"{axis} Axis error occurred: {ex.Message}");
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", "Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Handles the axis run.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="motorSpeedTextBox">The motor speed text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        /// <param name="stepsPerRevolution">The steps per revolution.</param>
        /// <param name="distancePerRevolution">The distance per revolution.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        private async Task HandleAxisRun(string axis, TextBox stepperMoveTextBox, TextBox motorSpeedTextBox, CheckBox resetToZeroCheckBox, SerialPort serialPort, bool axisRunToCompletion, bool axisTargetReached, float stepsPerRevolution, float distancePerRevolution, CancellationToken token)
        {
            if (resetToZeroCheckBox.IsChecked == true)
            {
                await ResetAxisToZero(axis, serialPort);
            }
            else
            {
                float currentStepperPosition = float.Parse(stepperMoveTextBox.Text.Trim(), CultureInfo.InvariantCulture) / stepsPerRevolution * distancePerRevolution;
                float moveDistance = float.Parse(stepperMoveTextBox.Text, CultureInfo.InvariantCulture) / stepsPerRevolution * distancePerRevolution;
                float motorSpeed = float.Parse(motorSpeedTextBox.Text, CultureInfo.InvariantCulture);
                await MoveAxis(axis, serialPort, axisRunToCompletion, axisTargetReached, currentStepperPosition, moveDistance, motorSpeed, token);
            }
        }

        /// <summary>
        /// Resets the axis to zero.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="serialPort">The serial port.</param>
        private async Task ResetAxisToZero(string axis, SerialPort serialPort)
        {
            string command = string.Empty;
            switch (axis)
            {
                case "X":
                    command = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text.Trim()},1,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        xAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        Logger.LogError(ex, "Error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Invalid format for X axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        Logger.LogError(ex, "Overflow error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show("X axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "Y":
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text.Trim()},1,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        yAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        Logger.LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Invalid format for Y axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        Logger.LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Y axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        Logger.LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Invalid format for Z axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        Logger.LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Z axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        Logger.LogError(ex, "Error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Invalid format for X axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        Logger.LogError(ex, "Overflow error converting txtXaxisStepperCurrent.Text to float.");
                        MessageBox.Show("X axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text.Trim()},1,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                    try
                    {
                        yAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                    }
                    catch (FormatException ex)
                    {
                        Logger.LogError(ex, "Error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Invalid format for Y axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (OverflowException ex)
                    {
                        Logger.LogError(ex, "Overflow error converting txtYaxisStepperCurrent.Text to float.");
                        MessageBox.Show("Y axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;

            }

            serialPort.Write(command);
            Logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {command}");
            await ZeroAxis(axis);
        }
        /// <summary>
        /// Moves the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="stepperMove">The stepper move.</param>
        /// <param name="stepperSpeed">The stepper speed.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        private async Task MoveAxis(string axis, SerialPort serialPort, bool axisRunToCompletion, bool axisTargetReached, float currentPosition, float stepperMove, float stepperSpeed, CancellationToken token)
        {
            try
            {
                decimal motorMovementSeconds = 1;
                int movementTimer;
                float stepsPerRevolution = 200.0f;
                float distancePerRevolution = 4.0f;
                //float distanceInMM = 0.00f;

                string command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";

                // Send command to the serial port
                serialPort.Write(command);
                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay));
                Logger.LogInformation($"{axis} Axis Run Event: {command}");

                // Calculate movement timer
                motorMovementSeconds = UpdateMotorTimer(axis, Convert.ToDecimal(stepperSpeed), Convert.ToDecimal(stepperMove));
                movementTimer = Properties.Settings.Default.Milliseconds * Convert.ToInt32(motorMovementSeconds);
                TimeSpan countdownTime = TimeSpan.FromMilliseconds(movementTimer);
                DateTime targetEndTime = DateTime.Now.Add(countdownTime);

                // Start timer and stopwatch
                StartTimer(axis, targetEndTime);
                StartStopwatch(axis);

                // Calculate current stepper position and move distance
                float currentStepperPosition = currentPosition / stepsPerRevolution * distancePerRevolution;
                float moveDistance = stepperMove / stepsPerRevolution * distancePerRevolution;

                // Update absolute position based on movement direction
                if (axis == "X")
                {
                    UpdateAxisPosition(axis, ref xAxisAbsolutePosition, currentStepperPosition, moveDistance, axisRunToCompletion, axisTargetReached);
                }
                else if (axis == "Y")
                {
                    UpdateAxisPosition(axis, ref yAxisAbsolutePosition, currentStepperPosition, moveDistance, axisRunToCompletion, axisTargetReached);
                }
                else if (axis == "Z")
                {
                    UpdateAxisPosition(axis, ref zAxisAbsolutePosition, currentStepperPosition, moveDistance, axisRunToCompletion, axisTargetReached);
                }

                Logger.LogInformation($"{axis} Axis Current Time: {DateTime.Now:hh\\:mm\\:ss} + {movementTimer} = {targetEndTime:hh\\:mm\\:ss}");
            }
            catch (FormatException ex)
            {
                Logger.LogError(ex, $"Error converting stepper current text to float for {axis} axis.");
                MessageBox.Show($"Invalid format for {axis} axis stepper current value.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException ex)
            {
                Logger.LogError(ex, $"Overflow error converting stepper current text to float for {axis} axis.");
                MessageBox.Show($"{axis} axis stepper current value is too large or too small.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Unexpected error in MoveAxis for {axis} axis.");
                MessageBox.Show($"Unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the axis position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="axisAbsolutePosition">The axis absolute position.</param>
        /// <param name="currentStepperPosition">The current stepper position.</param>
        /// <param name="moveDistance">The move distance.</param>
        /// <param name="axisRunToCompletion">if set to <c>true</c> [axis run to completion].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateAxisPosition(string axis, ref float axisAbsolutePosition, float currentStepperPosition, float moveDistance, bool axisRunToCompletion, bool axisTargetReached)
        {
            if (axisTargetReached) return; // Do not update if the target is reached

            if (IsNegative(Convert.ToDecimal(moveDistance)))
            {
                axisAbsolutePosition -= moveDistance;
                UpdateAxisPosition(axis, axisAbsolutePosition, true, axisTargetReached);
            }
            else
            {
                axisAbsolutePosition += moveDistance;
                UpdateAxisPosition(axis, axisAbsolutePosition, false, axisTargetReached);

            }
        }
        /// <summary>
        /// Updates the zero status.
        /// </summary>
        private void UpdateZeroStatus()
        {
            ZeroXaxis = ckbXaxisResetToZero.IsChecked == true ? 1 : 0;
            ZeroYaxis = ckbYaxisResetToZero.IsChecked == true ? 1 : 0;
            ZeroZaxis = ckbZaxisResetToZero.IsChecked == true ? 1 : 0;

            Logger.LogInformation($"Updated Zero Status - X: {ZeroXaxis}, Y: {ZeroYaxis}, Z: {ZeroZaxis}");
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
            decimal MotorMovementSeconds = 0.00m;

            if (MotorSpeed <= 100.00m)
            {
                MotorMovementSeconds = stepperMove / 2;
            }
            else if (MotorSpeed <= 200.00m)
            {
                MotorMovementSeconds = stepperMove / 4;
            }
            else if (MotorSpeed <= 300.00m)
            {
                MotorMovementSeconds = stepperMove / 6;
            }
            else if (MotorSpeed <= 400.00m)
            {
                MotorMovementSeconds = stepperMove / 8;
            }
            else if (MotorSpeed <= 500.00m)
            {
                MotorMovementSeconds = stepperMove / 10;
            }
            else if (MotorSpeed <= 600.00m)
            {
                MotorMovementSeconds = stepperMove / 12;
            }
            else if (MotorSpeed <= 700.00m)
            {
                MotorMovementSeconds = stepperMove / 14;
            }
            else if (MotorSpeed <= 800.00m)
            {
                MotorMovementSeconds = stepperMove / 16;
            }
            else if (MotorSpeed <= 900.00m)
            {
                MotorMovementSeconds = stepperMove / 18;
            }
            else if (MotorSpeed <= 1000.00m)
            {
                MotorMovementSeconds = stepperMove / 20;
            }

            Logger.LogInformation($"{Axis} Axis MotorMovementSeconds: {MotorMovementSeconds}");
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
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private async void StartDelayTask(string axis, string data, bool axisTargetReached)
        {
            try
            {
                await Task.Delay(Convert.ToInt32(Properties.Settings.Default.MillisecondDelay) * 2);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if ((axis == "X" && !xAxisRunToCompletion && (data.Contains("X Axis CW Motor Current Position:") || data.Contains("X Axis CCW Motor Current Position:"))) ||
                                                    (axis == "Y" && !yAxisRunToCompletion && (data.Contains("Y Axis CW Motor Current Position:") || data.Contains("Y Axis CCW Motor Current Position:"))) ||
                                                    (axis == "Z" && !zAxisRunToCompletion && (data.Contains("Z Axis CW Motor Current Position:") || data.Contains("Z Axis CCW Motor Current Position:"))))
                    {
                        UpdateMotorPosition(axis, data, axisTargetReached);
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
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateMotorPosition(string axis, string data, bool axisTargetReached)
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
                            UpdateAxisPosition(axis, distanceInMM, line.Contains("CW"), axisTargetReached);
                        }
                        if (line.Contains("CCW"))
                        {
                            UpdateAxisPosition(axis, -distanceInMM, line.Contains("CCW"), axisTargetReached);
                        }
                    }
                }
                else if (line.Contains($"{axis} Axis Absolute Position"))
                {
                    string positionString = line.Replace($"{axis} Axis Absolute Position", "").Trim();
                    if (float.TryParse(positionString, out float currentPosition))
                    {
                        distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;

                        UpdateAxisAbsolutePosition(axis, distanceInMM);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the axis position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="distanceInMM">The distance in mm.</param>
        /// <param name="isClockwise">if set to <c>true</c> [is clockwise].</param>
        /// <param name="axisTargetReached">if set to <c>true</c> [axis target reached].</param>
        private void UpdateAxisPosition(string axis, float distanceInMM, bool isClockwise, bool axisTargetReached)
        {
            float newPosition = isClockwise ? distanceInMM : -distanceInMM;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (axisTargetReached) return; // Do not update if the target is reached
                switch (axis)
                {
                    case "X":
                        _xLimitSwitchPress = false;
                        xAxisTargetReached = true; // Set the flag to indicate the target is reached
                        xAxisAbsolutePosition += newPosition;
                        txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(xAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        xAxisRunToCompletion = false;
                        xAxisClearAbsolutePosition = true;
                        break;
                    case "Y":
                        _yLimitSwitchPress = false;
                        yAxisTargetReached = true; // Set the flag to indicate the target is reached
                        yAxisAbsolutePosition += newPosition;
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(yAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        yAxisRunToCompletion = true;
                        yAxisClearAbsolutePosition = true;
                        break;
                    case "Z":
                        _zLimitSwitchPress = false;
                        zAxisTargetReached = true; // Set the flag to indicate the target is reached
                        zAxisAbsolutePosition += newPosition;
                        txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                        zAxisRunToCompletion = false;
                        zAxisClearAbsolutePosition = true;
                        break;
                }
                Properties.Settings.Default.Save();
                Logger.LogInformation($"{axis} Axis Motor Current Position: {newPosition}");
            });
        }

        /// <summary>
        /// Updates the axis absolute position.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="distanceInMM">The distance in mm.</param>
        private void UpdateAxisAbsolutePosition(string axis, float distanceInMM)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (xAxisTargetReached || yAxisTargetReached || zAxisTargetReached) return; // Do not update if the target is reached
                switch (axis)
                {
                    case "X":
                        if (xAxisTargetReached || _xLimitSwitchPress) return; // Do not update if the target is reached or limit switch is pressed
                        xAxisAbsolutePosition += distanceInMM;
                        txtXaxisStepperCurrent.Text = xAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(xAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        xAxisRunToCompletion = false;
                        xAxisClearAbsolutePosition = true;
                        break;
                    case "Y":
                        if (yAxisTargetReached || _yLimitSwitchPress) return; // Do not update if the target is reached or limit switch is pressed
                        yAxisAbsolutePosition += distanceInMM;
                        txtYaxisStepperCurrent.Text = yAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(yAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        yAxisRunToCompletion = true;
                        yAxisClearAbsolutePosition = true;
                        break;
                    case "Z":
                        if (zAxisTargetReached || _zLimitSwitchPress) return; // Do not update if the target is reached or limit switch is pressed
                        zAxisAbsolutePosition += distanceInMM;
                        txtZaxisStepperCurrent.Text = zAxisAbsolutePosition.ToString("F2");
                        Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(zAxisAbsolutePosition.ToString("F2"));
                        Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
                        zAxisRunToCompletion = false;
                        zAxisClearAbsolutePosition = true;
                        break;
                }
                Properties.Settings.Default.Save();
                Logger.LogInformation($"{axis} Axis Absolute Position: {distanceInMM}");
            });
        }

        /// <summary>
        /// Determines whether the specified number is negative.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns><c>true</c> if the specified number is negative; otherwise, <c>false</c>.</returns>
        public bool IsNegative(decimal number) => number < 0;

        /// <summary>
        /// Zeroes the axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        private async Task ZeroAxis(string axis)
        {
            Logger.LogInformation($"Setting {axis} Axis Current Location Set to Zero on DRO");
            await Task.Delay(Properties.Settings.Default.Milliseconds);

            decimal zeroValue = Properties.Settings.Default.Value_0_00;
            string zeroValueString = zeroValue.ToString("F2");

            switch (axis)
            {
                case "X":
                    UpdateAxisToZero(txtXaxisStepperCurrent, txtXaxisStepperMove, ckbXaxisResetToZero, zeroValue, zeroValueString);
                    Properties.Settings.Default.XaxisStepperCurrent = zeroValue;
                    Properties.Settings.Default.XaxisStepperMove = zeroValue;
                    break;
                case "Y":
                    UpdateAxisToZero(txtYaxisStepperCurrent, txtYaxisStepperMove, ckbYaxisResetToZero, zeroValue, zeroValueString);
                    Properties.Settings.Default.YaxisStepperCurrent = zeroValue;
                    Properties.Settings.Default.YaxisStepperMove = zeroValue;
                    break;
                case "Z":
                    UpdateAxisToZero(txtZaxisStepperCurrent, txtZaxisStepperMove, ckbZaxisResetToZero, zeroValue, zeroValueString);
                    Properties.Settings.Default.ZaxisStepperCurrent = zeroValue;
                    Properties.Settings.Default.ZaxisStepperMove = zeroValue;
                    break;
                case "XY":
                    UpdateAxisToZero(txtXaxisStepperCurrent, txtXaxisStepperMove, ckbXaxisResetToZero, zeroValue, zeroValueString);
                    UpdateAxisToZero(txtYaxisStepperCurrent, txtYaxisStepperMove, ckbYaxisResetToZero, zeroValue, zeroValueString);
                    Properties.Settings.Default.XaxisStepperCurrent = zeroValue;
                    Properties.Settings.Default.XaxisStepperMove = zeroValue;
                    Properties.Settings.Default.YaxisStepperCurrent = zeroValue;
                    Properties.Settings.Default.YaxisStepperMove = zeroValue;
                    break;
            }

            Properties.Settings.Default.Save();
            Logger.LogInformation($"{axis} Axis Current Location Set to Zero");
        }

        /// <summary>
        /// Updates the axis to zero.
        /// </summary>
        /// <param name="stepperCurrentTextBox">The stepper current text box.</param>
        /// <param name="stepperMoveTextBox">The stepper move text box.</param>
        /// <param name="resetToZeroCheckBox">The reset to zero CheckBox.</param>
        /// <param name="zeroValue">The zero value.</param>
        /// <param name="zeroValueString">The zero value string.</param>
        private void UpdateAxisToZero(TextBox stepperCurrentTextBox, TextBox stepperMoveTextBox, CheckBox resetToZeroCheckBox, decimal zeroValue, string zeroValueString)
        {
            stepperCurrentTextBox.Text = zeroValueString;
            stepperMoveTextBox.Text = zeroValueString;
            resetToZeroCheckBox.IsChecked = false;
        }

        /// <summary>
        /// Starts the timer for the specified axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="targetEndTime">The target end time.</param>
        /// <exception cref="System.InvalidOperationException">Timer for axis {axis} is not initialized.</exception>
        private void StartTimer(string axis, DateTime targetEndTime)
        {
            try
            {
                DispatcherTimer? timer = axis switch
                {
                    "X" => xTimer,
                    "Y" => yTimer,
                    "Z" => zTimer,
                    _ => throw new ArgumentException($"Invalid axis: {axis}", nameof(axis))
                };

                if (timer == null)
                {
                    throw new InvalidOperationException($"Timer for axis {axis} is not initialized.");
                }

                switch (axis)
                {
                    case "X":
                        xTargetEndTime = targetEndTime;
                        break;
                    case "Y":
                        yTargetEndTime = targetEndTime;
                        break;
                    case "Z":
                        zTargetEndTime = targetEndTime;
                        break;
                }

                timer.Start();
                Logger.LogInformation($"{axis} Axis Timer Started. Target End Time: {targetEndTime:hh\\:mm\\:ss}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error starting timer for {axis} axis.");
                MessageBox.Show($"Error starting timer for {axis} axis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Starts the stopwatch for the specified axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        private void StartStopwatch(string axis)
        {
            try
            {
                Stopwatch stopwatch = axis switch
                {
                    "X" => xStopwatch,
                    "Y" => yStopwatch,
                    "Z" => zStopwatch,
                    _ => throw new ArgumentException($"Invalid axis: {axis}", nameof(axis))
                };

                stopwatch.Start();
                Logger.LogInformation($"{axis} Axis Stopwatch Started.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error starting stopwatch for {axis} axis.");
                MessageBox.Show($"Error starting stopwatch for {axis} axis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                Logger.LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
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
                        Logger.LogInformation($"{axis} Axis motor speed updated to {newValue}");
                    }
                    else
                    {
                        throw new FormatException("Invalid format for motor speed value.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"{axis} Axis error occurred while updating motor speed.");
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
                Logger.LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation($"{textBox.Name} Axis {inputType} controlled Keypad returned: {mainWindow.Result}");
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
                Logger.LogInformation("Stepper Motor Controller MainWindow Closing.");

                // Close serial ports
                CloseSerialPort(xSerialPort, "X");
                CloseSerialPort(ySerialPort, "Y");
                CloseSerialPort(zSerialPort, "Z");

                // Save settings
                Properties.Settings.Default.Save();
                Logger.LogInformation("Properties Settings Saved.");

                // Close settings window
                if (NewSettingsWindow != null)
                {
                    NewSettingsWindow.Close();
                    Logger.LogInformation("Properties Settings Form Closed.");
                }

                Logger.LogInformation("Stepper Motor Controller Form Closed.");
            }
            catch (IOException ioex)
            {
                Logger.LogError(ioex, "Error occurred while closing the MainWindow.");
                MessageBox.Show($"An error occurred while closing the application: {ioex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while closing the MainWindow.");
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
                    Logger.LogInformation($"SerialPort for {axis} Axis closed.");
                }
                catch (IOException ioex)
                {
                    Logger.LogError(ioex, $"Error occurred while closing the {axis} Axis SerialPort.");
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