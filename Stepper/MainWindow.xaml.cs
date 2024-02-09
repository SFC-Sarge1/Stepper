using System.Windows;
using UtilityDelta.Gpio.Implementation;
using UtilityDelta.Stepper;
using System.IO.Ports;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort sp = new SerialPort();
        public string myPortName = "COM5"; // Serial Port Name
        public string stringValue = "";
        public MainWindow()
        {
            InitializeComponent();
            int baudRate = Convert.ToInt32(txtBaudRate.Text);
            sp = new(myPortName, baudRate);
            sp.Open();
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            stringValue = txtStepperXMove.Text.Trim() + "," + txtStepperYMove.Text.Trim() + ",";
            sp.Write(stringValue);
                    }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            txtStepperXMove.Text = "0.00";
            txtStepperYMove.Text = "0.00";
            stringValue = txtStepperXMove.Text.Trim() + "," + txtStepperYMove.Text.Trim() + ",";
            sp.Write(stringValue);
                    }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
                    {
            sp.Close();
            base.OnClosing(e);
                    }
    }
}