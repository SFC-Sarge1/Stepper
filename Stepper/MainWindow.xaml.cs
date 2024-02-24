using System.Windows;
using System.IO.Ports;

namespace Stepper
{
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public SerialPort sp = new SerialPort();
        public string myPortName; // Serial Port Name
        public int baudRate = 9600;
        public string stringValue = "";
        public int zeroXaxis = 0;
        public int zeroYaxis = 0;
        public int milliseconds = 10;
        public string axis = "X";
        public string home = "S";
        public string currentXAxis = "0.00";
        public string currentYAxis = "0.00";
        public string previousXAxis = "0.00";
        public string previousYAxis = "0.00";
        bool XaxisChanged = false;
        bool YaxisChanged = false;
        public string XaxisStepperMoveTemp = "0.00";
        public string YaxisStepperMoveTemp = "0.00";
        public string XaxisTempHomeValue = "0.00";
        public string YaxisTempHomeValue = "0.00";
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
        public MainWindow()
        {
            InitializeComponent();
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                txtBaudRate.Text = "9600";
                baudRate = Convert.ToInt32(txtBaudRate.Text);
            String selectedItem = cmbComPort.Items.CurrentItem.ToString();
            cmbComPort.SelectedItem = selectedItem;
            myPortName = selectedItem;
                sp = new(myPortName, baudRate);
                if (sp.IsOpen == false) { sp.Open(); }
            }
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
        public async void XZero(int delay)
        {
            await Task.Delay(delay);
            txtXaxisStepperCurrent.Text = "0.00";
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = "0.00";
            home = "S";
        }
        public async void YZero(int delay)
                {
            await Task.Delay(delay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = "0.00";
            txtYaxisStepperCurrent.Text = "0.00";
            home = "S";
            }
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
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (sp.IsOpen == true) { sp.Close(); }
            base.OnClosing(e);
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
           }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
            }
        }
        private void XaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            XaxisStepperMoveTemp = txtXaxisStepperMove.Text;
        }
        private void YaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            YaxisStepperMoveTemp = txtYaxisStepperMove.Text;
        }
        private void XaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            XaxisChanged = true;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
        }
        private void YaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            YaxisChanged = true;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
        }
    }
}