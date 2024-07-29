// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 07-24-2024
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This allows you to control the X,Y and Z axis</summary>
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
        /// My port name
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
        /// The stepper move
        /// </summary>
        public decimal stepperMove;
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
        /// The xy stepper move
        /// </summary>
        public decimal xyStepperMove;

        /// <summary>
        /// The logger
        /// </summary>
        public static ILogger _logger;
        /// <summary>
        /// The logger factory
        /// </summary>
        public static ILoggerFactory loggerFactory;
        /// <summary>
        /// The xserial port
        /// </summary>
        public SerialPort _XserialPort = new SerialPort();
        /// <summary>
        /// The yserial port
        /// </summary>
        public SerialPort _YserialPort = new SerialPort();
        /// <summary>
        /// The zserial port
        /// </summary>
        public SerialPort _ZserialPort = new SerialPort();
        //public SerialPort xSp;
        //public string xSpResult;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            string LogFileName = "Stepper.log";
            string FileLogPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), LogFileName);
            string FileLogPathBackup = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{LogFileName}.old");
            if (File.Exists(FileLogPath))
            {
                if (File.Exists(FileLogPathBackup))
                {
                    File.Delete(FileLogPathBackup);
                    File.Move(FileLogPath, FileLogPathBackup);
                    File.Delete(FileLogPath);
                }
                else
                {
                    File.Move(FileLogPath, FileLogPathBackup);
                    File.Delete(FileLogPath);
                }
            }
            // Create an instance of ILoggerFactory (usually done during application startup)
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddProvider(new FileLoggerProvider(FileLogPath)); // Log to a file named "Stepper.log"
            });
            _logger = loggerFactory.CreateLogger<MainWindow>();
            _logger.LogInformation(message: $"Stepper Motor Controller Application Started.");
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            _logger.LogInformation(message: $"Version: {displayableVersion}");
            _XserialPort = new(Properties.Settings.Default.XComPort, Properties.Settings.Default.BaudRate);
            if (_XserialPort.IsOpen == false)
            {
                try
                {
                    _XserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"X Axis SerialPort {Properties.Settings.Default.XComPort} not connected.");
                }
            }
            _YserialPort = new(Properties.Settings.Default.YComPort, Properties.Settings.Default.BaudRate);
            if (_YserialPort.IsOpen == false)
            {
                try
                {
                    _YserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"Y Axis SerialPort {Properties.Settings.Default.YComPort} not connected.");
                }
            }
            _ZserialPort = new(Properties.Settings.Default.ZComPort, Properties.Settings.Default.BaudRate);
            if (_ZserialPort.IsOpen == false)
            {
                try
                {
                    _ZserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"Z Axis SerialPort {Properties.Settings.Default.ZComPort} not connected.");
                }
            }
            _XserialPort.DataReceived += new SerialDataReceivedEventHandler(XdataReceivedHandler);
            _YserialPort.DataReceived += new SerialDataReceivedEventHandler(YdataReceivedHandler);
            _ZserialPort.DataReceived += new SerialDataReceivedEventHandler(ZdataReceivedHandler);
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
            VersionTxt.Text = "Version: " + displayableVersion;
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
            Properties.Settings.Default.BuildVersion = "Version: " + displayableVersion;
            _logger.LogInformation(message: "Version:" + displayableVersion);
#endif
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
        }
        private static void XdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Xsp = (SerialPort)sender;
            string Xindata = Xsp.ReadExisting();
            Console.WriteLine("X Axis Data Received:");
            Console.Write(Xindata);
        }
        private static void YdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Ysp = (SerialPort)sender;
            string Yindata = Ysp.ReadExisting();
            Console.WriteLine("Y Axis Data Received:");
            Console.Write(Yindata);
        }
        private static void ZdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Zsp = (SerialPort)sender;
            string Zindata = Zsp.ReadExisting();
            Console.WriteLine("Z Axis Data Received:");
            Console.Write(Zindata);
        }
        /// <summary>
        /// Determines wheZther the specified number is negative.
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
                string[] xStepper;
                if (ckbXaxisResetToZero.IsChecked == true)
                {
                    zeroXaxis = 1;
                    stringValue1 = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{zeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //ckbXaxisResetToZero.IsChecked = false;
                    _XserialPort.Write(stringValue1);
                    //SendDataToLattepanda.SendData(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroXaxis = 0;
                    XZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbXaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{axis},{(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{txtXaxisMotorSpeed.Text},{zeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
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
                    _XserialPort.Write(stringValue);
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
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                string[] yStepper;

                if (ckbYaxisResetToZero.IsChecked == true)
                {
                    zeroYaxis = 1;
                    stringValue1 = $"{axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{zeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    //ckbYaxisResetToZero.IsChecked = false;
                    _YserialPort.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroYaxis = 0;
                    YZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbYaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{zeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
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
                    //SendDataToLattepanda.SendData(stringValue);
                    _YserialPort.Write(stringValue);
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
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                string[] zStepper;

                if (ckbZaxisResetToZero.IsChecked == true)
                {
                    zeroZaxis = 1;
                    stringValue1 = $"{axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{zeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{zeroYaxis},{Properties.Settings.Default.Value_0_00},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    //ckbZaxisResetToZero.IsChecked = false;
                    _ZserialPort.Write(stringValue1);
                    _logger.LogInformation(message: $"{axis} Axis Run Event to reset Axis to zero: {stringValue1}");
                    stringValue1 = "";
                    zeroZaxis = 0;
                    ZZero(Properties.Settings.Default.Milliseconds, axis);
                }
                if (ckbZaxisResetToZero.IsChecked == false)
                {
                    stringValue = $"{axis},{txtXaxisStepperMove.Text},{txtXaxisMotorSpeed.Text},{zeroXaxis},{txtYaxisStepperMove.Text},{txtYaxisMotorSpeed.Text},{zeroYaxis},{(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text))},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
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
                    //SendDataToLattepanda.SendData(stringValue);
                    _ZserialPort.Write(stringValue);
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
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                string[] xyStepper;

                if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
                {
                    zeroXaxis = 1;
                    zeroYaxis = 1;
                    stringValue1 = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{zeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    ckbXaxisResetToZero.IsChecked = false;
                    ckbYaxisResetToZero.IsChecked = false;
                    _XserialPort.Write(stringValue1);
                    _YserialPort.Write(stringValue1);
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
                    stringValue1 = $"{axis},{Properties.Settings.Default.Value_0_00},{txtXaxisMotorSpeed.Text},{zeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    ckbXaxisResetToZero.IsChecked = false;
                    _XserialPort.Write(stringValue1);
                    _YserialPort.Write(stringValue1);
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
                    stringValue1 = $"{axis},{(Convert.ToDecimal(txtXaxisStepperCurrent) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{txtXaxisMotorSpeed.Text},{zeroXaxis},{Properties.Settings.Default.Value_0_00},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue1);
                    ckbYaxisResetToZero.IsChecked = false;
                    _YserialPort.Write(stringValue1);
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
                    stringValue = $"{axis},{(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text))},{zeroXaxis},{(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text))},{txtYaxisMotorSpeed.Text},{zeroYaxis},{txtZaxisStepperMove.Text},{txtZaxisMotorSpeed.Text},{zeroZaxis}";
                    //SendDataToLattepanda.SendData(stringValue);
                    _XserialPort.Write(stringValue);
                    _YserialPort.Write(stringValue);
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
                MessageBox.Show($"{axis} Axis error occurred: {ex.Message}", $"Stepper Motor Controller Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //ResetSerialPort();        
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
        }
        /// <summary>
        /// Xies the zero.
        /// </summary>
        /// <param name="myDelay">The myDelay.</param>
        /// <param name="axis">The axis.</param>
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
        }
        //private void ZdataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        //{
        //    xSp = (SerialPort)sender;
        //}

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
            _logger.LogInformation(message: $"Stepper Motor Controller Loading Application Settings form.");
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
                    _logger.LogInformation(message: $"Stepper Motor Controller Disable {axis} Axis controls while moving to location.");
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} {elapsedTime.ToString(@"hh\:mm\:ss\.fffff")}";
                    _logger.LogInformation(message: $"Time remaining: {elapsedTime.ToString(@"hh\:mm\:ss")} targetEndTime = {targetEndTime.ToString(@"hh\:mm\:ss")}");
                }
                if (ckbXaxisResetToZero.IsChecked == true || ckbYaxisResetToZero.IsChecked == true || ckbZaxisResetToZero.IsChecked == true)
                {
                    ckbXaxisResetToZero.IsChecked = false;
                    ckbYaxisResetToZero.IsChecked = false;
                    ckbZaxisResetToZero.IsChecked = false;
                    CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                }
            }
            if (DateTime.Now >= targetEndTime)
            {
                CountdownLabel.Content = $"{Properties.Settings.Default.CountDownText} Completed";
                switch (axis)
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
                        currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                        txtXaxisStepperCurrent.Text = currentXAxis;
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
                        currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                        txtYaxisStepperCurrent.Text = currentYAxis;
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
                        currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                        txtZaxisStepperCurrent.Text = currentZAxis;
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
                        Properties.Settings.Default.XaxisMotorSpeed = Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                        Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
                        Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
                        Properties.Settings.Default.YaxisMotorSpeed = Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                        Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
                        Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
                        Properties.Settings.Default.Save();
                        break;
                }
                _logger.LogInformation(message: $"Stepper Motor Controller Enable {axis} Axis controls after moving to location.");
                stopwatch.Stop(); // Stop the timer when the countdown reaches
                _logger.LogInformation(message: $"Stepper Motor Controller Stopwatch Stopped.");
                timer.Stop();
                _logger.LogInformation(message: $"Stepper Motor Controller Timer Stopped.");
                stopwatch.Reset();
                _logger.LogInformation(message: $"Stepper Motor Controller Stopwatch Reset.");
            }
        }
        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation(message: $"Stepper Motor Controller MainWindow loaded");
            txtXaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisMotorSpeed.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperCurrent.BorderBrush = System.Windows.Media.Brushes.White;
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
                _XserialPort.Close();
                _YserialPort.Close();
                _ZserialPort.Close();

                _logger.LogInformation(message: $"Stepper Motor Controller Serial Ports closed.");
                _logger.LogInformation(message: $"Stepper Motor Controller MainWindow Closing.");
                Properties.Settings.Default.Save();
            }
            catch (IOException ioex)
            {
                _logger.LogInformation(message: $"Stepper Motor Controller error occurred: {ioex.Message}");
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

        private void XAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _XserialPort = new("COM7", Properties.Settings.Default.BaudRate);
            if (_XserialPort.IsOpen == false)
            {
                try
                {
                    _XserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"X Axis SerialPort COM7 not connected.");
                }
            }

            btnXAxisPort.Content = "X Axis Port 7";
        }

        private void YAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _YserialPort = new("COM8", Properties.Settings.Default.BaudRate);
            if (_YserialPort.IsOpen == false)
            {
                try
                {
                    _YserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"Y Axis SerialPort COM8 not connected.");
                }
            }

            btnYAxisPort.Content = "Y Axis Port 8";
        }

        private void ZAxisPort_Click(object sender, RoutedEventArgs e)
        {
            _ZserialPort = new("COM5", Properties.Settings.Default.BaudRate);
            if (_ZserialPort.IsOpen == false)
            {
                try
                {
                    _ZserialPort.Open();
                }
                catch
                {
                    _logger.LogInformation(message: $"Z Axis SerialPort COM5 not connected.");
                }
            }

            btnZAxisPort.Content = "Z Axis Port 5";
        }
    }
}
