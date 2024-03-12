// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 12-19-2023
//
// Last Modified By : sfcsarge
// Last Modified On : 03-08-2024
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.IO.Ports;

/// <summary>
/// The Stepper namespace.
/// </summary>
namespace Stepper
{
    using System.Globalization;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// The sp
        /// </summary>
        public SerialPort sp = new SerialPort();
        /// <summary>
        /// My port name
        /// </summary>
        public string myPortName; // Serial Port Name
        /// <summary>
        /// The baud rate
        /// </summary>
        public int baudRate = 9600;
        /// <summary>
        /// The string value
        /// </summary>
        public string stringValue = "";
        /// <summary>
        /// The zero xaxis
        /// </summary>
        public int zeroXaxis = 0;
        /// <summary>
        /// The zero yaxis
        /// </summary>
        public int zeroYaxis = 0;
        /// <summary>
        /// The zero zaxis
        /// </summary>
        public int zeroZaxis = 0;
        /// <summary>
        /// The milliseconds
        /// </summary>
        public int milliseconds = 10;
        /// <summary>
        /// The axis
        /// </summary>
        public string axis = "X";
        /// <summary>
        /// The home
        /// </summary>
        public string home = "S";
        /// <summary>
        /// The current x axis
        /// </summary>
        public string currentXAxis = "0.00";
        /// <summary>
        /// The current y axis
        /// </summary>
        public string currentYAxis = "0.00";
        /// <summary>
        /// The current z axis
        /// </summary>
        public string currentZAxis = "0.00";
        /// <summary>
        /// The previous x axis
        /// </summary>
        public string previousXAxis = "0.00";
        /// <summary>
        /// The previous y axis
        /// </summary>
        public string previousYAxis = "0.00";
        /// <summary>
        /// The previous z axis
        /// </summary>
        public string previousZAxis = "0.00";
        /// <summary>
        /// The xaxis changed
        /// </summary>
        bool XaxisChanged = false;
        /// <summary>
        /// The yaxis changed
        /// </summary>
        bool YaxisChanged = false;
        /// <summary>
        /// The zaxis changed
        /// </summary>
        bool ZaxisChanged = false;
        /// <summary>
        /// The xaxis stepper move temporary
        /// </summary>
        public string XaxisStepperMoveTemp = "0.00";
        /// <summary>
        /// The yaxis stepper move temporary
        /// </summary>
        public string YaxisStepperMoveTemp = "0.00";
        /// <summary>
        /// The zaxis stepper move temporary
        /// </summary>
        public string ZaxisStepperMoveTemp = "0.00";
        /// <summary>
        /// The xaxis temporary home value
        /// </summary>
        public string XaxisTempHomeValue = "0.00";
        /// <summary>
        /// The yaxis temporary home value
        /// </summary>
        public string YaxisTempHomeValue = "0.00";
        /// <summary>
        /// The zaxis temporary home value
        /// </summary>
        public string ZaxisTempHomeValue = "0.00";
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
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtBaudRate.Text = "9600";
            baudRate = Convert.ToInt32(txtBaudRate.Text);
            String selectedItem = cmbComPort.Items.CurrentItem.ToString();
            cmbComPort.SelectedItem = selectedItem;
            myPortName = selectedItem;
            sp = new(myPortName, baudRate);
            if (sp.IsOpen == false) { sp.Open(); }
        }
        /// <summary>
        /// Handles the Click event of the XAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "X";
            home = "S";
            previousXAxis = txtXaxisStepperMove.Text;
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + "0.00" + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                XZero(milliseconds);
            }
            if (ckbXaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        /// <summary>
        /// Handles the Click event of the YAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "Y";
            home = "S";
            previousYAxis = txtYaxisStepperMove.Text;
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 1;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + "0.00" + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroYaxis = 0;
                YZero(milliseconds);
            }
            if (ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        /// <summary>
        /// Handles the Click event of the ZAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "Z";
            home = "S";
            previousZAxis = txtZaxisStepperMove.Text;
            if (ckbZaxisResetToZero.IsChecked == true)
            {
                zeroZaxis = 1;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + "0.00" + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroZaxis = 0;
                ZZero(milliseconds);
            }
            if (ckbZaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString() + ",";
                sp.Write(stringValue);
                currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                txtZaxisStepperCurrent.Text = currentZAxis;
                ZaxisChanged = false;
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        /// <summary>
        /// Handles the Click event of the XYAxisRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "XY";
            home = "S";
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + "0.00" + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + "0.00" + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                XYZero(milliseconds);
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + "0.00" + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                txtYaxisStepperCurrent.Text = currentYAxis;
                XZero(milliseconds);
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + "0.00" + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                YZero(milliseconds);
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                stringValue = axis + "," + home + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                byte[] rawData = Encoding.UTF8.GetBytes(stringValue);
                string result = Encoding.UTF8.GetString(rawData);
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                txtXaxisStepperCurrent.Text = currentXAxis;
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        /// <summary>
        /// xes the zero.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public async void XZero(int delay)
        {
            await Task.Delay(delay);
            txtXaxisStepperCurrent.Text = "0.00";
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = "0.00";
            home = "S";
        }
        /// <summary>
        /// ies the zero.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public async void YZero(int delay)
        {
            await Task.Delay(delay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = "0.00";
            txtYaxisStepperCurrent.Text = "0.00";
            home = "S";
        }
        /// <summary>
        /// zs the zero.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public async void ZZero(int delay)
        {
            await Task.Delay(delay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = "0.00";
            txtZaxisStepperCurrent.Text = "0.00";
            home = "S";
        }
        /// <summary>
        /// Xies the zero.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public async void XYZero(int delay)
        {
            await Task.Delay(delay);
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperCurrent.Text = "0.00";
            txtXaxisStepperMove.Text = "0.00";
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = "0.00";
            txtYaxisStepperCurrent.Text = "0.00";
            home = "S";
        }
        /// <summary>
        /// Handles the <see cref="E:Closing" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (sp.IsOpen == true) { sp.Close(); }
            base.OnClosing(e);
        }

        /// <summary>
        /// CheckBoxes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true && ckbZaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                zeroZaxis = 1;
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                zeroZaxis = 0;
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                zeroZaxis = 0;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                zeroZaxis = 1;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false && ckbZaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                zeroZaxis = 0;
            }
        }
        /// <summary>
        /// Handles the GotFocus event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void XaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            XaxisStepperMoveTemp = txtXaxisStepperMove.Text;
        }
        /// <summary>
        /// Handles the GotFocus event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void YaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            YaxisStepperMoveTemp = txtYaxisStepperMove.Text;
        }
        /// <summary>
        /// Handles the GotFocus event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ZaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            ZaxisStepperMoveTemp = txtZaxisStepperMove.Text;
        }
        /// <summary>
        /// Handles the TextChanged event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void XaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            XaxisChanged = true;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
        }
        /// <summary>
        /// Handles the TextChanged event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void YaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            YaxisChanged = true;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
        }
        /// <summary>
        /// Handles the TextChanged event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void ZaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZaxisChanged = true;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the XaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void XaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperMove.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the YaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void YaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperMove.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the ZaxisStepperMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ZaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperMove.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtXaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtXaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisMotorSpeed.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtYaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtYaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisMotorSpeed.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtZaxisMotorSpeed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtZaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisMotorSpeed.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtXaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtXaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperCurrent.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtYaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtYaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperCurrent.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtZaxisStepperCurrent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtZaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperCurrent.Text = mainWindow.Result;
        }
        /// <summary>
        /// Handles the PreviewMouseUp event of the txtBaudRate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void txtBaudRate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtBaudRate.Text = mainWindow.Result;
        }

    }
}