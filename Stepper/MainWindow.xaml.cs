// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 11-25-2024
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
    using System.Runtime.CompilerServices;
    using System;
    using Windows.Devices.Geolocation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private StepperAppSettings _newSettingsWindow;
        public StepperAppSettings NewSettingsWindow
        {
            get => _newSettingsWindow;
            private set
            {
                if (_newSettingsWindow != value)
                {
                    _newSettingsWindow = value;
                    LogInformation($"NewSettingsWindow updated to {value}");
                }
            }
        }
        private DispatcherTimer _countdownTimer;
        public DispatcherTimer CountdownTimer
        {
            get => _countdownTimer;
            set
            {
                if (_countdownTimer != value)
                {
                    _countdownTimer = value;
                    LogInformation($"CountdownTimer updated to {value}");
                }
            }
        }
        private static bool _limitSwitchPressed = false;
        public static bool LimitSwitchPressed
        {
            get => _limitSwitchPressed;
            set
            {
                if (_limitSwitchPressed != value)
                {
                    _limitSwitchPressed = value;
                    LogInformation($"LimitSwitchPressed updated to {value}");
                }
            }
        }
        private DateTime _targetEndTime;
        public DateTime TargetEndTime
        {
            get => _targetEndTime;
            set
            {
                if (_targetEndTime != value)
                {
                    _targetEndTime = value;
                    LogInformation($"TargetEndTime updated to {value}");
                }
            }
        }
        private int _zeroXaxis;
        public int ZeroXaxis
        {
            get => _zeroXaxis;
            private set
            {
                _zeroXaxis = value;
                if (_zeroXaxis != value)
                {
                    _zeroXaxis = value;
                    LogInformation($"ZeroXaxis updated to {value}");
                }
            }
        }
        private int _zeroYaxis;
        public int ZeroYaxis
        {
            get => _zeroYaxis;
            private set
            {
                _zeroYaxis = value;
                if (_zeroYaxis != value)
                {
                    _zeroYaxis = value;
                    LogInformation($"ZeroYaxis updated to {value}");
                }
            }
        }
        private int _zeroZaxis;
        public int ZeroZaxis
        {
            get => _zeroZaxis;
            private set
            {
                _zeroZaxis = value;
                if (_zeroZaxis != value)
                {
                    _zeroZaxis = value;
                    LogInformation($"ZeroZaxis updated to {value}");
                }
            }
        }
        private bool _xaxisChanged;
        public bool XaxisChanged
        {
            get => _xaxisChanged;
            private set
            {
                _xaxisChanged = value;
                if (_xaxisChanged)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"XaxisChanged updated to {value}");
                }
            }
        }
        private bool _yaxisChanged;
        public bool YaxisChanged
        {
            get => _yaxisChanged;
            private set
            {
                _yaxisChanged = value;
                if (_yaxisChanged)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"YaxisChanged updated to {value}");
                }
            }
        }
        private bool _zaxisChanged;
        public bool ZaxisChanged
        {
            get => _zaxisChanged;
            private set
            {
                _zaxisChanged = value;
                if (_zaxisChanged)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"ZaxisChanged updated to {value}");
                }
            }
        }
        private string _xaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string XaxisStepperMoveTemp
        {
            get => _xaxisStepperMoveTemp;
            private set
            {
                _xaxisStepperMoveTemp = value;
                if (_xaxisStepperMoveTemp != value)
                {
                    _xaxisStepperMoveTemp = value;
                    LogInformation($"XaxisStepperMoveTemp updated to {value}");
                }
            }
        }
        private string _yaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string YaxisStepperMoveTemp
        {
            get => _yaxisStepperMoveTemp;
            private set
            {
                _yaxisStepperMoveTemp = value;
                if (_yaxisStepperMoveTemp != value)
                {
                    _yaxisStepperMoveTemp = value;
                    LogInformation($"YaxisStepperMoveTemp updated to {value}");
                }
            }
        }
        private string _zaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string ZaxisStepperMoveTemp
        {
            get => _zaxisStepperMoveTemp;
            private set
            {
                if (_zaxisStepperMoveTemp != value)
                {
                    _zaxisStepperMoveTemp = value;
                    LogInformation($"ZaxisStepperMoveTemp updated to {value}");
                }
            }
        }
        private static ILogger? _logger;
        public static ILogger? Logger
        {
            get => _logger;
            private set
            {
                if (_logger != value)
                {
                    _logger = value;
                    LogInformation($"Logger updated to {value}");
                }
            }
        }
        private static ILoggerFactory? _loggerFactory;
        public static ILoggerFactory? LoggerFactory
        {
            get => _loggerFactory;
            private set
            {
                if (_loggerFactory != value)
                {
                    _loggerFactory = value;
                    LogInformation($"Logger updated to {value}");
                }
            }
        }
        public SerialPort xSerialPort;
        public SerialPort ySerialPort;
        public SerialPort zSerialPort;
        private bool _xAxisRunToCompletion = false;
        public bool XAxisRunToCompletion
        {
            get => _xAxisRunToCompletion;
            set
            {
                _xAxisRunToCompletion = value;
                if (_xAxisRunToCompletion)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"XAxisRunToCompletion updated to {value}");
                }
            }
        }
        private bool _yAxisRunToCompletion = false;
        public bool YAxisRunToCompletion
        {
            get => _yAxisRunToCompletion;
            set
            {
                _yAxisRunToCompletion = value;
                if (_yAxisRunToCompletion)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"YAxisRunToCompletion updated to {value}");
                }
            }
        }
        private bool _zAxisRunToCompletion = false;
        public bool ZAxisRunToCompletion
        {
            get => _zAxisRunToCompletion;
            set
            {
                _zAxisRunToCompletion = value;
                if (_zAxisRunToCompletion)
                {
                    //LimitSwitchUpdateCompleted = true;
                    LogInformation($"ZAxisRunToCompletion updated to {value}");
                }
            }
        }
        /// <summary>
        /// The message sent to the serial port. 
        /// </summary>
        static byte[] message = new byte[6000];
        /// <summary>The x axis absolute position</summary>
        private float _xAxisAbsolutePosition = 0.00f;
        /// <summary>Gets the x axis absolute position.</summary>
        /// <value>The x axis absolute position.</value>
        public float XAxisAbsolutePosition
        {
            get => _xAxisAbsolutePosition;
            private set
            {
                if (_xAxisAbsolutePosition != value)
                {
                    _xAxisAbsolutePosition = value;
                    LogInformation($"XAxisAbsolutePosition updated to {value}");
                }
            }
        }
        /// <summary>The y axis absolute position</summary>
        private float _yAxisAbsolutePosition = 0.00f;
        /// <summary>Gets the y axis absolute position.</summary>
        /// <value>The y axis absolute position.</value>
        public float YAxisAbsolutePosition
        {
            get => _yAxisAbsolutePosition;
            private set
            {
                if (_yAxisAbsolutePosition != value)
                {
                    _yAxisAbsolutePosition = value;
                    LogInformation($"YAxisAbsolutePosition updated to {value}");
                }
            }
        }
        private float _completedXAxisCurrentPosition = 0.00f;
        public float CompletedXAxisCurrentPosition
        {
            get => _completedXAxisCurrentPosition;
            private set
            {
                if (_completedXAxisCurrentPosition != value)
                {
                    _completedXAxisCurrentPosition = value;
                    LogInformation($"CompletedXAxisCurrentPosition updated to {value}");
                }
            }
        }
        private float _completedYAxisCurrentPosition = 0.00f;
        public float CompletedYAxisCurrentPosition
        {
            get => _completedYAxisCurrentPosition;
            private set
            {
                if (_completedYAxisCurrentPosition != value)
                {
                    _completedYAxisCurrentPosition = value;
                    LogInformation($"CompletedYAxisCurrentPosition updated to {value}");
                }
            }
        }
        private float _completedZAxisCurrentPosition = 0.00f;
        public float CompletedZAxisCurrentPosition
        {
            get => _completedZAxisCurrentPosition;
            private set
            {
                if (_completedZAxisCurrentPosition != value)
                {
                    _completedZAxisCurrentPosition = value;
                    LogInformation($"CompletedZAxisCurrentPosition updated to {value}");
                }
            }
        }
        /// <summary>
        /// Occurs when X axis target reached property is changed.
        /// Define the event using the delegate.
        /// </summary>
        public event AxisTargetReachedEventHandler? XAxisTargetReachedChanged;
        /// <summary>
        /// X axis Backing field for the _xAxisTargetReached property.
        /// </summary>
        private bool _xAxisTargetReached = false;
        /// <summary>
        /// Gets or sets a value indicating whether X axis target reached.
        /// Property for _xAxisTargetReached with event raising.
        /// </summary>
        /// <value><c>true</c> if X axis target reached; otherwise, <c>false</c>.</value>
        public bool XAxisTargetReached
        {
            get => _xAxisTargetReached;
            set
            {
                if (_xAxisTargetReached != value)
                {
                    _xAxisTargetReached = value;
                    LogInformation($"XAxisTargetReached updated to {value}");
                    OnXAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Y axis target reached Backing field for the _xAxisTargetReached property.
        /// </summary>
        private bool _yAxisTargetReached = false;
        /// <summary>
        /// Gets or sets a value indicating whether Y axis target reached.
        /// Property for _xAxisTargetReached with event raising.
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
                    LogInformation($"YAxisTargetReached updated to {value}");
                    OnYAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Occurs when Y axis target reached changed.
        /// Define the event using the delegate.
        /// </summary>
        public event AxisTargetReachedEventHandler? YAxisTargetReachedChanged;
        /// <summary>
        /// Method to raise the X axis target reached event.
        /// Handles the <see cref="E:XAxisTargetReachedChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnXAxisTargetReachedChanged(EventArgs e)
        {
            XAxisTargetReachedChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Handles the <see cref="E:YAxisTargetReachedChanged" /> event.
        /// Method to raise the event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnYAxisTargetReachedChanged(EventArgs e)
        {
            YAxisTargetReachedChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Define a delegate for the event; AxisTargetReachedEventHandler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void AxisTargetReachedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when Z axis target reached changed.
        /// Define the event using the delegate.
        /// </summary>
        public event AxisTargetReachedEventHandler? ZAxisTargetReachedChanged;
        /// <summary>
        /// The x axis target reached Backing field for the _xAxisTargetReached property.
        /// </summary>
        private bool _zAxisTargetReached = false;
        /// <summary>
        /// Gets or sets a value indicating whether Z axis target reached.
        /// Property for _xAxisTargetReached with event raising.
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
                    LogInformation($"ZAxisTargetReached updated to {value}");
                    OnZAxisTargetReachedChanged(EventArgs.Empty);
                }
            }
        }
        /// <include file="Stepper.xml" path="doc/members/member[@name='M:Stepper.MainWindow.OnZAxisTargetReachedChanged(EventArgs)']" />
        protected virtual void OnZAxisTargetReachedChanged(EventArgs e)
        {
            ZAxisTargetReachedChanged?.Invoke(this, e);
        }

        /// <summary>Delegate AxisAbsolutePositionEventHandler</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void AxisAbsolutePositionEventHandler(object sender, EventArgs e);
        /// <summary>Occurs when [z axis absolute position changed].</summary>
        public event AxisAbsolutePositionEventHandler? ZAxisAbsolutePositionChanged;
        /// <summary>Handles the <see cref="E:ZAxisAbsolutePositionChanged" /> event.</summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnZAxisAbsolutePositionChanged(EventArgs e)
        {
            ZAxisAbsolutePositionChanged?.Invoke(this, e);
        }

        /// <summary>The z axis absolute position</summary>
        private float _zAxisAbsolutePosition = 0.00f;
        /// <summary>Gets the z axis absolute position.</summary>
        /// <value>The z axis absolute position.</value>
        public float ZAxisAbsolutePosition
        {
            get => _zAxisAbsolutePosition;
            private set
            {
                //if (_zAxisAbsolutePosition != value)
                //{
                _zAxisAbsolutePosition = value;
                LogInformation($"ZAxisAbsolutePosition updated to {value}");
                OnZAxisAbsolutePositionChanged(EventArgs.Empty);
                //}
            }
        }

        /// <summary>
        /// The current X, Y, or Z axis being worked with.
        /// </summary>
        private string _currentAxis = "Z";
        public string CurrentAxis
        {
            get => _currentAxis;
            private set
            {
                if (_currentAxis != value)
                {
                    _currentAxis = value;
                    LogInformation($"CurrentAxis updated to {value}");
                }
            }
        }
        private bool _limitSwitchUpdateCompleted = false;
        public bool LimitSwitchUpdateCompleted
        {
            get => _limitSwitchUpdateCompleted;
            set
            {
                _limitSwitchUpdateCompleted = value;
                if (_limitSwitchUpdateCompleted)
                {
                    LimitSwitchPressed = false;
                    LogInformation($"LimitSwitchUpdateCompleted updated to {value}");
                }
            }
        }
        public bool isClockwise = true;
        public float newPosition = 0.00f;
        public float distanceInMM = 0.00f;
        public float stepsPerRevolution = 200.0f;
        public float distancePerRevolution = 4.0f;
        /// <summary>
        /// Flag to indicate if the CountdownTimer_Tick method is running.
        /// </summary>
        private bool _isCountdownTimerRunning = false;


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
            ZAxisAbsolutePositionChanged += MainWindow_ZAxisAbsolutePositionChanged; // Subscribe to the event
            NewSettingsWindow = new StepperAppSettings();
        }
        /// <summary>
        /// Initializes the countdown timer.
        /// </summary>
        private void InitializeCountdownTimer()
        {
            CountdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            CountdownTimer.Tick += CountdownTimer_Tick;
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
        /// <summary>
        /// Handles the XAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_XAxisTargetReachedChanged(object sender, EventArgs e)
        {
            LogInformation("X Axis Target Reached changed.");
            if (XAxisTargetReached)
            {
                try
                {
                    if (float.TryParse(txtXaxisStepperMove.Text, out float stepperMove))
                    {
                        CompletedXAxisCurrentPosition += stepperMove;
                        txtXaxisStepperCurrent.Text = CompletedXAxisCurrentPosition.ToString("F2");
                        txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                        txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, "Error updating Z Axis Current Position.");
                }
            }
        }
        /// <summary>
        /// Handles the YAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_YAxisTargetReachedChanged(object sender, EventArgs e)
        {
            LogInformation("Y Axis Target Reached changed.");
            if (YAxisTargetReached)
            {
                try
                {
                    if (float.TryParse(txtYaxisStepperMove.Text, out float stepperMove))
                    {
                        CompletedYAxisCurrentPosition += stepperMove;
                        txtYaxisStepperCurrent.Text = CompletedYAxisCurrentPosition.ToString("F2");
                        txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, "Error updating Z Axis Current Position.");
                }
            }
        }
        /// <summary>
        /// Handles the ZAxisTargetReachedChanged event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainWindow_ZAxisTargetReachedChanged(object sender, EventArgs e)
        {
            if (ZAxisTargetReached)
            {
                LogInformation("Z Axis Target Reached changed.");
                if (LimitSwitchPressed)
                {
                    LogInformation("Z Axis LimitSwitchPressed.");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LogInformation($"Z Axis Absolute Position: {ZAxisAbsolutePosition}.");
                        ZAxisAbsolutePosition += newPosition;
                        LogInformation($"Z Axis New Position: {newPosition}.");
                        txtZaxisStepperCurrent.Text = ZAxisAbsolutePosition.ToString("F2");
                        LogInformation($"Z Axis Position: {ZAxisAbsolutePosition}.");
                        txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                        txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        LimitSwitchPressed = false;
                    });
                }
                else
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (float.TryParse(txtZaxisStepperMove.Text, out float stepperMove))
                            {
                                ZAxisAbsolutePosition += stepperMove;
                                txtZaxisStepperCurrent.Text = ZAxisAbsolutePosition.ToString("F2");
                                LogInformation($"Z Axis Position: {ZAxisAbsolutePosition}.");
                                txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
                                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                                //LimitSwitchPressed = false;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "Error updating Z Axis Current Position.");
                    }
                }

            }
        }
        // Event handler for ZAxisAbsolutePositionChanged
        private void MainWindow_ZAxisAbsolutePositionChanged(object sender, EventArgs e)
        {
            LogInformation("Z Axis Absolute Position changed.");
            // Add your logic here to handle the event
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
#if !DEBUG
                MessageBox.Show($"Access to the port {serialPort.PortName} is denied. Please check your permissions.", "COMPort: Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
            catch (IOException ex)
            {
                LogError(ex, $"I/O error occurred while opening the port {serialPort.PortName}.");
#if !DEBUG
                MessageBox.Show($"I/O error occurred while opening the port {serialPort.PortName}. Please check the connection.", "COMPort: Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
            catch (InvalidOperationException ex)
            {
                LogError(ex, $"The specified port {serialPort.PortName} is already open.");
#if DEBUG
                MessageBox.Show($"The specified port {serialPort.PortName} is already open. Please close any other applications using this port.", "COMPort: is already open: Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
            catch (Exception ex)
            {
                LogError(ex, $"Unexpected error occurred while opening the port {serialPort.PortName}.");
#if !DEBUG
                MessageBox.Show($"Unexpected error occurred while opening the port {serialPort.PortName}: {ex.Message}", "COMPort: Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
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

            XAxisAbsolutePosition = 0.00f;
            YAxisAbsolutePosition = 0.00f;
            ZAxisAbsolutePosition = 0.00f;
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
            TargetEndTime = endTime;
            CountdownTimer.Start();
        }
        /// <summary>
        /// Handles the Tick event of the CountdownTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (_isCountdownTimerRunning)
            {
                return; // Exit if the method is already running
            }

            _isCountdownTimerRunning = true;
            try
            {
                TimeSpan remainingTime = TargetEndTime - DateTime.Now;
                if (remainingTime <= TimeSpan.Zero)
                {
                    remainingTime = TimeSpan.Zero;
                    CountdownTimer.Stop();
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                    EnableControls();
                    LogInformation($"{Properties.Settings.Default.CountDownText} Completed");
                    //switch (CurrentAxis)
                    //{
                    //    case ("X"):
                    //        XAxisTargetReached = true;
                    //        break;
                    //    case ("Y"):
                    //        YAxisTargetReached = true;
                    //        break;
                    //    case ("Z"):
                    //        //ZAxisTargetReached = true;
                    //        break;
                    //}
                    return;
                }
                // Update the UI with the remaining time
                DisableControls();
                CountdownLabel.Content = $"{Properties.Settings.Default.CountdownTimer} {remainingTime.ToString(@"hh\:mm\:ss")}";
            }
            finally
            {
                _isCountdownTimerRunning = false;
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
        /// Handle the Serial Port X axis received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        private void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string in_data = sp.ReadLine();
                // Split the input data into lines
                string[] lines = in_data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Use LINQ to filter and process the lines
                IEnumerable<string> filteredLines = lines.Where(line =>
                    line.Contains($"Axis {CurrentAxis} CW STOPPED {CurrentAxis} Axis CW Motor Current Position:") ||
                    line.Contains($"Axis {CurrentAxis} CCW STOPPED {CurrentAxis} Axis CCW Motor Current Position:") ||
                    line.Contains($"Axis {CurrentAxis} reached target position Current Position:") ||
                    line.StartsWith("Z"));
                if (!filteredLines.Any()) return;
                // ProcessReceivedData(CurrentAxis, filteredLines);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in XdataReceivedHandler");
#if DEBUG
                MessageBox.Show($"{ex} Error in XdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
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
                SerialPort sp = (SerialPort)sender;
                string in_data = sp.ReadLine();
                // Split the input data into lines
                string[] lines = in_data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Use LINQ to filter and process the lines
                IEnumerable<string> filteredLines = lines.Where(line =>
                    line.Contains($"Axis {CurrentAxis} CW STOPPED {CurrentAxis} Axis CW Motor Current Position:") ||
                    line.Contains($"Axis {CurrentAxis} CCW STOPPED {CurrentAxis} Axis CCW Motor Current Position:") ||
                    line.Contains($"Axis {CurrentAxis} reached target position Current Position:") ||
                    line.StartsWith("Z"));
                if (!filteredLines.Any()) return;
                //ProcessReceivedData(CurrentAxis, filteredLines);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in YdataReceivedHandler");
#if DEBUG
                MessageBox.Show($"{ex} Error in YdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
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
                SerialPort sp = (SerialPort)sender;
                string in_data = sp.ReadLine();
                // Split the input data into lines
                string[] lines = in_data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (!lines.Any()) return;
                if (lines.Any(line => line.StartsWith("Z"))) return;
                var CW = lines
                    .Where(line => line.Contains($"Axis {CurrentAxis} CW STOPPED Axis {CurrentAxis} CW Motor Current Position:"))
                    .Select(line => line.Replace($"Axis {CurrentAxis} CW STOPPED Axis {CurrentAxis} CW Motor Current Position:", "").Trim());

                var CCW = lines
                    .Where(line => line.Contains($"Axis {CurrentAxis} CCW STOPPED Axis {CurrentAxis} CCW Motor Current Position:"))
                    .Select(line => line.Replace($"Axis {CurrentAxis} CCW STOPPED Axis {CurrentAxis} CCW Motor Current Position:", "").Trim());

                var reached = lines
                   .Where(line => line.Contains($"Axis {CurrentAxis} reached target position Current Position:"))
                   .Select(line => line.Replace($"Axis {CurrentAxis} reached target position Current Position:", "").Trim());

                string positionString = "0.00";

                if (CW.Any())
                {
                    positionString = CW.First();
                }
                else if (CCW.Any())
                {
                    positionString = CCW.First();
                }
                else if (reached.Any())
                {
                    positionString = reached.First();
                }

                if (float.TryParse(positionString, out float currentPosition))
                {
                    distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
                    if (lines[0].ToString().Contains($"Axis {CurrentAxis} CW STOPPED Axis {CurrentAxis} CW Motor Current Position:"))
                    {
                        isClockwise = true;
                        newPosition = isClockwise ? distanceInMM : -distanceInMM;
                        LimitSwitchPressed = true;
                        ZAxisTargetReached = true;
                        LogInformation($"CW Limit Switch Activated!");
                    }
                    else if (lines[0].ToString().Contains($"Axis {CurrentAxis} CCW STOPPED Axis {CurrentAxis} CCW Motor Current Position:"))
                    {
                        isClockwise = false;
                        newPosition = isClockwise ? distanceInMM : -distanceInMM;
                        LimitSwitchPressed = true;
                        ZAxisTargetReached = true;
                        LogInformation($"CCW Limit Switch Activated!");
                    }
                    else if (lines[0].Contains("reached"))
                    {
                        if (IsNegative(currentPosition))
                        {
                            isClockwise = true;
                        }
                        else
                        {
                            isClockwise = false;
                        }
                        newPosition = isClockwise ? distanceInMM : -distanceInMM;
                        LimitSwitchPressed = false;
                        ZAxisTargetReached = true;
                        LogInformation($"Axis {CurrentAxis} reached target position Current Position:");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error in ZdataReceivedHandler");
#if !DEBUG
                MessageBox.Show($"{ex} Error in ZdataReceivedHandler", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
            }
        }
        ///// <summary>
        ///// Processes the received data.
        ///// </summary>
        ///// <param name="axis">The axis.</param>
        ///// <param name="indata">The indata.</param>
        //private void ProcessReceivedData(string axis, IEnumerable<string> indata)
        //{
        //    //string[] lines = indata.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string line in indata)
        //    {
        //        if (line.Contains($"{axis} Axis CW STOPPED {axis} Axis CW Motor Current Position:") ||
        //            line.Contains($"{axis} Axis CCW STOPPED {axis} Axis CCW Motor Current Position:"))
        //        {
        //            //LimitSwitchPressed = true;
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                CountdownTimer.Stop();
        //                CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
        //                EnableControls();
        //                if (!LimitSwitchUpdateCompleted)
        //                {
        //                    LogInformation($"{Properties.Settings.Default.CountDownText} Completed");
        //                }

        //                string positionString = line.Replace($"{axis} Axis CW STOPPED {axis} Axis CW Motor Current Position:", "")
        //                                            .Replace($"{axis} Axis CCW STOPPED {axis} Axis CCW Motor Current Position:", "")
        //                                            .Trim();
        //                if (float.TryParse(positionString, out float currentPosition))
        //                {
        //                    distanceInMM = (currentPosition / stepsPerRevolution) * distancePerRevolution;
        //                    if (line.Contains("CW"))
        //                    {
        //                        if (LimitSwitchUpdateCompleted) return;
        //                        isClockwise = true;
        //                        newPosition = isClockwise ? distanceInMM : -distanceInMM;

        //                        ZAxisTargetReached = true;

        //                        //UpdateAxisPosition(axis, distanceInMM);
        //                        LogInformation($"CW Limit Switch Activated!");
        //                    }
        //                    else if (line.Contains("CCW"))
        //                    {
        //                        //if (LimitSwitchUpdateCompleted) return;
        //                        isClockwise = false;
        //                        newPosition = isClockwise ? distanceInMM : -distanceInMM;
        //                        ZAxisTargetReached = true;
        //                        //UpdateAxisPosition(axis, -distanceInMM);
        //                        LogInformation($"CCW Limit Switch Activated!");
        //                    }
        //                }
        //            });
        //        }
        //    }
        //}
        ///// <summary>
        ///// Updates the axis position.
        ///// </summary>
        ///// <param name="axis">The axis.</param>
        ///// <param name="distanceInMM">The distance in mm.</param>
        ///// <param name="isClockwise">if set to <c>true</c> [is clockwise].</param>
        //private void UpdateAxisPosition(string axis, float distanceInMM)
        //{
        //    newPosition = isClockwise ? distanceInMM : -distanceInMM;
        //    switch (axis)
        //    {
        //        case "X":
        //            XAxisAbsolutePosition += newPosition;
        //            txtXaxisStepperCurrent.Text = XAxisAbsolutePosition.ToString("F2");
        //            LimitSwitchUpdateCompleted = true;
        //            break;
        //        case "Y":
        //            YAxisAbsolutePosition += newPosition;
        //            txtYaxisStepperCurrent.Text = YAxisAbsolutePosition.ToString("F2");
        //            LimitSwitchUpdateCompleted = true;
        //            break;
        //        case "Z":
        //            ZAxisAbsolutePosition += newPosition;
        //            txtZaxisStepperCurrent.Text = ZAxisAbsolutePosition.ToString("F2");
        //            LimitSwitchUpdateCompleted = true;
        //            LimitSwitchPressed = false;
        //            break;
        //    }
        //    Properties.Settings.Default.Save();
        //    Logger.LogInformation($"{axis} Axis Motor Current Position: {newPosition}");
        //}
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
                        string axis = "X";
                        XAxisTargetReached = false;
                        //LimitSwitchUpdateCompleted = false;
                        await RunAxis(axis, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort);
                        break;
                    }
                case Button button when button == btnRunYAxis:
                    {
                        string axis = "Y";
                        YAxisTargetReached = false;
                        //LimitSwitchUpdateCompleted = false;
                        await RunAxis(axis, txtYaxisStepperMove, txtYaxisStepperCurrent, txtYaxisMotorSpeed, ckbYaxisResetToZero, ySerialPort);
                        break;
                    }
                case Button button when button == btnRunZAxis:
                    {
                        string axis = "Z";
                        ZAxisTargetReached = false;
                        //LimitSwitchUpdateCompleted = false;
                        await RunAxis(axis, txtZaxisStepperMove, txtZaxisStepperCurrent, txtZaxisMotorSpeed, ckbZaxisResetToZero, zSerialPort);
                        break;
                    }
                case Button button when button == btnRunXYAxis:
                    {
                        string axis = "X";
                        XAxisTargetReached = false;
                        //LimitSwitchUpdateCompleted = false;
                        await RunAxis(axis, txtXaxisStepperMove, txtXaxisStepperCurrent, txtXaxisMotorSpeed, ckbXaxisResetToZero, xSerialPort);
                        axis = "Y";
                        YAxisTargetReached = false;
                        //LimitSwitchUpdateCompleted = false;
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
#if DEBUG
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", "Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
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
                        XAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtXaxisStepperCurrent.Text = XAxisAbsolutePosition.ToString("F2");
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
                        YAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = YAxisAbsolutePosition.ToString("F2");
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
                        ZAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        //txtZaxisStepperCurrent.Text = ZAxisAbsolutePosition.ToString("F2");
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
                        XAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtXaxisStepperCurrent.Text = XAxisAbsolutePosition.ToString("F2");
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
                        YAxisAbsolutePosition = float.Parse("0.00", CultureInfo.InvariantCulture);
                        txtYaxisStepperCurrent.Text = YAxisAbsolutePosition.ToString("F2");
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
                //currentPosition += stepperMove;
                string command = $"{axis},{txtXaxisStepperMove.Text.Trim()},{txtXaxisMotorSpeed.Text.Trim()},0,{txtYaxisStepperMove.Text.Trim()},{txtYaxisMotorSpeed.Text.Trim()},0,{txtZaxisStepperMove.Text.Trim()},{txtZaxisMotorSpeed.Text.Trim()},0";
                serialPort.Write(command);
                StartCountdown(targetEndTime, axis, stepperMove, currentPosition, stepperSpeed);
                switch (axis)
                {
                    case ("X"):
                        //CompletedXAxisCurrentPosition = currentPosition;
                        break;
                    case ("Y"):
                        //CompletedYAxisCurrentPosition = currentPosition;
                        break;
                    case ("Z"):
                        //CompletedZAxisCurrentPosition = currentPosition;
                        //if (!LimitSwitchPressed)
                        //{
                        //    ZAxisAbsolutePosition += currentPosition;
                        //}
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
        public bool IsNegative(float number) => number < 0;
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
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 100.00 RPM: {MotorMovementSeconds}");

            }
            else if (MotorSpeed <= 200.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 200.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 300.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 300.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 400.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 400.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 500.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 500.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 600.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 600.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 700.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 700.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 800.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 800.00 RPM: {MotorMovementSeconds}");
            }
            else if (MotorSpeed <= 900.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 900.00 RPM: {MotorMovementSeconds}");
                MotorMovementSeconds = stepperMove * OneRev;
            }
            else if (MotorSpeed <= 1000.00m)
            {
                decimal RevPerSecond = MotorSpeed / 60m;
                decimal OneRev = 1m / RevPerSecond;
                MotorMovementSeconds = stepperMove * OneRev;
                LogInformation($"{Axis} Axis MotorMovementSeconds for <= 1000.00 RPM: {MotorMovementSeconds}");
            }

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
        // Event handler for the Checked event
        /// <summary>
        /// Handles the Checked event of the ckbAxisResetToZero control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private async void ckbAxisResetToZero_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                switch (checkBox.Name)
                {
                    case nameof(ckbXaxisResetToZero):
                        await ZeroAxis("X");
                        txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        break;
                    case nameof(ckbYaxisResetToZero):
                        await ZeroAxis("Y");
                        txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        break;
                    case nameof(ckbZaxisResetToZero):
                        await ZeroAxis("Z");
                        txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                        break;
                }
            }
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
            if (sender is not TextBox textBox) return;

            string axis = textBox.Name switch
            {
                nameof(txtXaxisStepperMove) => "X",
                nameof(txtYaxisStepperMove) => "Y",
                nameof(txtZaxisStepperMove) => "Z",
                _ => throw new ArgumentException("Invalid TextBox name", nameof(sender))
            };

            decimal stepperMoveValue = axis switch
            {
                "X" => Properties.Settings.Default.XaxisStepperMove,
                "Y" => Properties.Settings.Default.YaxisStepperMove,
                "Z" => Properties.Settings.Default.ZaxisStepperMove,
                _ => throw new ArgumentException("Invalid axis", nameof(axis))
            };

            if (textBox.Text == stepperMoveValue.ToString(CultureInfo.InvariantCulture))
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                if (decimal.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newValue))
                {
                    Properties.Settings.Default[$"{axis}axisStepperMove"] = newValue;
                    Properties.Settings.Default.Save();
                    LogInformation($"{axis} Axis stepper move updated to {newValue}");
                }
                else
                {
                    LogError(new FormatException("Invalid format for stepper move value"), $"{axis} Axis error occurred while updating stepper move.");
                    MessageBox.Show($"{axis} Axis error occurred: Invalid format for stepper move value.", "Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
#if DEBUG
                    MessageBox.Show($"An error occurred while closing the {axis} Axis SerialPort: {ioex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
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