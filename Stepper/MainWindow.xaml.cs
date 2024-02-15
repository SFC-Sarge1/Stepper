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
        public string stringValue = "";
        public int zeroXaxis = 0;
        public int zeroYaxis = 0;
        public int baudRate = 9600;
        public int milliseconds = 4000;
        public string axis = "X";

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
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = "0.00";
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                btnXAxisGoHome.IsEnabled = false;
            }
            if (ckbXaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                btnXAxisGoHome.IsEnabled = true;
            }
        }
        private void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "Y";
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = "0.00";
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroYaxis = 0;
                btnYAxisGoHome.IsEnabled = false;
            }
            if (ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                btnYAxisGoHome.IsEnabled = true;
            }
        }
        private void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "XY";
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = "0.00";
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = "0.00";
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                btnXAxisGoHome.IsEnabled = false;
                btnYAxisGoHome.IsEnabled = false;
                btnXYAxisGoHome.IsEnabled = false;
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = "0.00";
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                btnXAxisGoHome.IsEnabled = true;
                btnYAxisGoHome.IsEnabled = false;
                btnXYAxisGoHome.IsEnabled = false;
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = "0.00";
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                btnXAxisGoHome.IsEnabled = true;
                btnYAxisGoHome.IsEnabled = false;
                btnXYAxisGoHome.IsEnabled = false;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                btnXAxisGoHome.IsEnabled = true;
                btnYAxisGoHome.IsEnabled = true;
                btnXYAxisGoHome.IsEnabled = true;
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (sp.IsOpen == true) { sp.Close(); }
            base.OnClosing(e);
        }

        private void XAxisGoHome_Click(object sender, RoutedEventArgs e)
        {
            axis = "X";
            txtXaxisStepperMove.Text = "0.00";
            ckbXaxisResetToZero.IsChecked = false;
            zeroXaxis = 0;
            stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
        }
        private void YAxisGoHome_Click(object sender, RoutedEventArgs e)
        {
            axis = "Y";
            txtYaxisStepperMove.Text = "0.00";
            ckbYaxisResetToZero.IsChecked = false;
            zeroYaxis = 0;
            stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
        }
        private void XYAxisGoHome_Click(object sender, RoutedEventArgs e)
        {
            axis = "XY";
            txtXaxisStepperMove.Text = "0.00";
            ckbXaxisResetToZero.IsChecked = false;
            zeroXaxis = 0;
            txtYaxisStepperMove.Text = "0.00";
            ckbYaxisResetToZero.IsChecked = false;
            zeroYaxis = 0;
            stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
        }
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                btnXAxisGoHome.IsEnabled = false;
                btnYAxisGoHome.IsEnabled = false;
                btnXYAxisGoHome.IsEnabled = false;
           }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                btnXAxisGoHome.IsEnabled = false;
                btnYAxisGoHome.IsEnabled = true;
                btnXYAxisGoHome.IsEnabled = true;
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                btnXAxisGoHome.IsEnabled = true;
                btnYAxisGoHome.IsEnabled = false;
                btnXYAxisGoHome.IsEnabled = true;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                btnXAxisGoHome.IsEnabled = true;
                btnYAxisGoHome.IsEnabled = true;
                btnXYAxisGoHome.IsEnabled = true;
            }
        }
    }
}