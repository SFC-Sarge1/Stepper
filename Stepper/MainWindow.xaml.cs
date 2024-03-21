
namespace Stepper
{
    using System.Windows;
    using System.IO.Ports;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using System.Configuration;
    using System.Reflection;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public SerialPort sp = new();
        public string myPortName; // Serial Port Name (COM1, COM2, COM3, etc.)
        public string stringValue = "";
        public int zeroXaxis = Properties.Settings.Default.zeroXaxis;
        public int zeroYaxis = Properties.Settings.Default.zeroYaxis;
        public int zeroZaxis = Properties.Settings.Default.zeroZaxis;
        public int milliseconds = Properties.Settings.Default.Milliseconds;
        public string axis = Properties.Settings.Default.RootAxisX.ToString();
        public string currentXAxis = Properties.Settings.Default.Value_0_00.ToString();
        public string currentYAxis = Properties.Settings.Default.Value_0_00.ToString();
        public string currentZAxis = Properties.Settings.Default.Value_0_00.ToString();
        public string previousXAxis = Properties.Settings.Default.Value_0_00.ToString();
        public string previousYAxis = Properties.Settings.Default.Value_0_00.ToString();
        public string previousZAxis = Properties.Settings.Default.Value_0_00.ToString();
        public bool XaxisChanged = Properties.Settings.Default.XaxisChanged;
        public bool YaxisChanged = Properties.Settings.Default.YaxisChanged;
        public bool ZaxisChanged = Properties.Settings.Default.ZaxisChanged;
        public string XaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string YaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string ZaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00.ToString();
        public string selectedItem;
        public decimal stepperMove;
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
            ResizeMode = ResizeMode.NoResize;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = DateTime.Now;
            string displayableVersion = $"{version} ({buildDate})";
            VersionTxt.Text = "Version: " + displayableVersion;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
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
            txtBaudRate.Text = Properties.Settings.Default.BaudRate.ToString();
            selectedItem = cmbComPort.Items.CurrentItem.ToString();
            if (selectedItem == Properties.Settings.Default.COM1.ToString())
            {
                cmbComPort.Items.MoveCurrentToLast();
                selectedItem = cmbComPort.Items.CurrentItem.ToString();
                cmbComPort.SelectedItem = selectedItem;
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
            else if (selectedItem == Properties.Settings.Default.COM4.ToString())
            {
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
            else if (selectedItem == Properties.Settings.Default.COM5.ToString())
            {
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
            Content = StepperMotorControl;

        }
        private void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = Properties.Settings.Default.RootAxisX.ToString();
            previousXAxis = txtXaxisStepperMove.Text.ToString();
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                XZero(milliseconds);
            }
            if (ckbXaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        private void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            previousYAxis = txtYaxisStepperMove.Text.ToString();
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 1;
                stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroYaxis = 0;
                YZero(milliseconds);
            }
            if (ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        private void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            previousZAxis = txtZaxisStepperMove.Text.ToString();
            if (ckbZaxisResetToZero.IsChecked == true)
            {
                zeroZaxis = 1;
                stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroZaxis = 0;
                ZZero(milliseconds);
            }
            if (ckbZaxisResetToZero.IsChecked == false)
            {
                stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                if (Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
                txtZaxisStepperCurrent.Text = currentZAxis;
                ZaxisChanged = false;
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtZaxisStepperCurrent.Text);
            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        private void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                stringValue = Properties.Settings.Default.RootAxisXY + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                XYZero(milliseconds);
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                stringValue = Properties.Settings.Default.RootAxisXY + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
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
                stringValue = Properties.Settings.Default.RootAxisXY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisXY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00.ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                YZero(milliseconds);
            }
            if (ckbXaxisResetToZero.IsChecked == false && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 0;
                zeroYaxis = 0;
                stringValue = Properties.Settings.Default.RootAxisXY + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                }
                Thread.Sleep(milliseconds * Convert.ToInt32(((stepperMove / Properties.Settings.Default.Divider4_00)) / Properties.Settings.Default.mmPerRevolution));
                txtXaxisStepperCurrent.Text = currentXAxis;
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void XZero(int delay)
        {
            await Task.Delay(delay);
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void YZero(int delay)
        {
            await Task.Delay(delay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void ZZero(int delay)
        {
            await Task.Delay(delay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00.ToString();
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00.ToString();
            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void XYZero(int delay)
        {
            await Task.Delay(delay);
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
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (sp.IsOpen == true)
            {
                sp.Close();
            }
            base.OnClosing(e);
        }

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
        private void XaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            XaxisStepperMoveTemp = txtXaxisStepperMove.Text.ToString();
        }
        private void YaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            YaxisStepperMoveTemp = txtYaxisStepperMove.Text.ToString();
        }
        private void ZaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            ZaxisStepperMoveTemp = txtZaxisStepperMove.Text.ToString();
        }
        private void XaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            XaxisChanged = true;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtXaxisStepperMove.Text == Properties.Settings.Default.XaxisStepperMove.ToString())
            {
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void YaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            YaxisChanged = true;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtYaxisStepperMove.Text == Properties.Settings.Default.YaxisStepperMove.ToString())
            {
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void ZaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZaxisChanged = true;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtZaxisStepperMove.Text == Properties.Settings.Default.ZaxisStepperMove.ToString())
            {
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void XaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperMove.Text = mainWindow.Result.ToString();
        }
        private void YaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperMove.Text = mainWindow.Result.ToString();
        }
        private void ZaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperMove.Text = mainWindow.Result.ToString();
        }
        private void txtXaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisMotorSpeed.Text = mainWindow.Result.ToString();
        }
        private void txtYaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisMotorSpeed.Text = mainWindow.Result.ToString();
        }
        private void txtZaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisMotorSpeed.Text = mainWindow.Result.ToString();
        }
        private void txtXaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperCurrent.Text = mainWindow.Result.ToString();
        }
        private void txtYaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperCurrent.Text = mainWindow.Result.ToString();
        }
        private void txtZaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperCurrent.Text = mainWindow.Result.ToString();
        }
        private void txtBaudRate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new(this);
            if (mainWindow.ShowDialog() == true)
                txtBaudRate.Text = mainWindow.Result.ToString();
        }

        private void cmbComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = cmbComPort.Items.CurrentItem.ToString();
            if (sp.IsOpen == true && selectedItem == Properties.Settings.Default.COM1.ToString())
            {
                sp.Close();
            }
            if (selectedItem == Properties.Settings.Default.COM1.ToString())
            {
                cmbComPort.Items.MoveCurrentToLast();
                selectedItem = cmbComPort.Items.CurrentItem.ToString();
                cmbComPort.SelectedItem = selectedItem;
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
        }

        private void txtZaxisStepperCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtYaxisStepperCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtXaxisStepperCurrent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AppSettings_Click(object sender, RoutedEventArgs e)
        {
            StepperAppSettings newSettingsWindow = new StepperAppSettings();

            // Show the new window
            newSettingsWindow.Show();
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
