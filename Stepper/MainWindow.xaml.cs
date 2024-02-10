using System.Windows;
using UtilityDelta.Gpio.Implementation;
using UtilityDelta.Stepper;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace Stepper
{
    using System.Text.RegularExpressions;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort sp = new SerialPort();
        public string myPortName = "COM5"; // Serial Port Name
        public string stringValue = "";
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
            myPortName = (string)cmbComPort.SelectedItem;
            int baudRate = Convert.ToInt32(txtBaudRate.Text);
            sp = new(myPortName, baudRate);
            sp.Open();
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            stringValue = txtStepperXMove.Text.Trim() + "," + txtStepperYMove.Text.Trim() + "," + txtMotorSpeed.Text.Trim() + "," + txtMotorAcceleration.Text.Trim() + ",";
            sp.Write(stringValue);
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            txtStepperXMove.Text = "0.00";
            txtStepperYMove.Text = "0.00";
            stringValue = txtStepperXMove.Text.Trim() + "," + txtStepperYMove.Text.Trim() + "," + txtMotorSpeed.Text.Trim() + "," + txtMotorAcceleration.Text.Trim() + ",";
            sp.Write(stringValue);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            sp.Close();
            base.OnClosing(e);
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            txtStepperXMove.Text = "0.00";
            txtStepperYMove.Text = "0.00";
            stringValue = txtStepperXMove.Text.Trim() + "," + txtStepperYMove.Text.Trim() + "," + txtMotorSpeed.Text.Trim() + "," + txtMotorAcceleration.Text.Trim() + ",";
            sp.Write(stringValue);
        }
    }
}