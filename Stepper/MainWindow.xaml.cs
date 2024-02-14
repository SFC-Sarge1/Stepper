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
            try
            {
                txtBaudRate.Text = "9600";
                baudRate = Convert.ToInt32(txtBaudRate.Text);
                cmbComPort.SelectedValue = "COM3";
                myPortName = "COM3";
                sp = new(myPortName, baudRate);
                if (sp.IsOpen == false) { sp.Open(); }
            }
            catch (System.IO.IOException ex)
            {
                string error = ex.Message;
                if (error.Contains("COM3"))
                {
                txtBaudRate.Text = "9600";
            baudRate = Convert.ToInt32(txtBaudRate.Text);
                    cmbComPort.SelectedValue = "COM5";
                    myPortName = "COM5";
            sp = new(myPortName, baudRate);
            if (sp.IsOpen == false) { sp.Open(); }
        }
            }
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = "0.00";
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = "0.00";
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
                ckbXaxisResetToZero.IsChecked = false;
                txtXaxisStepperMove.Text = "0.00";
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
            }
            else if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
                zeroYaxis = 1;
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                ckbYaxisResetToZero.IsChecked = false;
                txtYaxisStepperMove.Text = "0.00";
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + ",";
                sp.Write(stringValue);
            }
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            txtXaxisStepperMove.Text = "0.00";
            txtYaxisStepperMove.Text = "0.00";
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                ckbXaxisResetToZero.IsChecked = false;
                zeroXaxis = 0;
            }
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                ckbYaxisResetToZero.IsChecked = false;
                zeroYaxis = 0;
            }
            stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (sp.IsOpen == true) { sp.Close(); }
            base.OnClosing(e);
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            txtXaxisStepperMove.Text = "0.00";
            ckbXaxisResetToZero.IsChecked = false;
            zeroXaxis = 0;
            txtYaxisStepperMove.Text = "0.00";
            ckbYaxisResetToZero.IsChecked = false;
            zeroYaxis = 0;
            stringValue = txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + zeroYaxis.ToString() + ",";
            sp.Write(stringValue);
        }
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 0;
            }
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 0;
            }
        }
    }
}