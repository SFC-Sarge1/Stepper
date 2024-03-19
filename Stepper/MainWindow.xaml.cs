
namespace Stepper
{
    using System.Windows;
    using System.IO.Ports;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using System.Configuration;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public SerialPort sp = new();
        public string myPortName; // Serial Port Name
        public string stringValue = "";
        public int zeroXaxis = 0;
        public int zeroYaxis = 0;
        public int zeroZaxis = 0;
        public int milliseconds = 10;
        public string axis = "X";
        public string currentXAxis = "0.00";
        public string currentYAxis = "0.00";
        public string currentZAxis = "0.00";
        public string previousXAxis = "0.00";
        public string previousYAxis = "0.00";
        public string previousZAxis = "0.00";
        bool XaxisChanged = false;
        bool YaxisChanged = false;
        bool ZaxisChanged = false;
        public string XaxisStepperMoveTemp = "0.00";
        public string YaxisStepperMoveTemp = "0.00";
        public string ZaxisStepperMoveTemp = "0.00";
        public string selectedItem;
        public string DefaultValues = "0.00";
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
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.XaxisStepperCurrent;
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.YaxisStepperCurrent;
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.ZaxisStepperCurrent;
            txtXaxisMotorSpeed.Text = Properties.Settings.Default.XaxisMotorSpeed;
            txtYaxisMotorSpeed.Text = Properties.Settings.Default.YaxisMotorSpeed;
            txtZaxisMotorSpeed.Text = Properties.Settings.Default.ZaxisMotorSpeed;
            txtXaxisStepperMove.Text = Properties.Settings.Default.XaxisStepperMove;
            txtYaxisStepperMove.Text = Properties.Settings.Default.YaxisStepperMove;
            txtZaxisStepperMove.Text = Properties.Settings.Default.ZaxisStepperMove;
            ckbXaxisResetToZero.IsChecked = Properties.Settings.Default.ckbXaxisResetToZeroIsChecked;
            ckbYaxisResetToZero.IsChecked = Properties.Settings.Default.ckbYaxisResetToZeroIsChecked;
            ckbZaxisResetToZero.IsChecked = Properties.Settings.Default.ckbZaxisResetToZeroIsChecked;
            txtBaudRate.Text = Properties.Settings.Default.BaudRate;
            DefaultValues = Properties.Settings.Default.DefaultValues;
            selectedItem = cmbComPort.Items.CurrentItem.ToString();
            if (selectedItem == "COM1")
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
            else if(selectedItem == "COM5")
            {
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
        }
        private void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "X";
            previousXAxis = txtXaxisStepperMove.Text;
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + DefaultValues + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                XZero(milliseconds);
            }
            if (ckbXaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                Thread.Sleep(milliseconds * 100);
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        private void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "Y";
            previousYAxis = txtYaxisStepperMove.Text;
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + DefaultValues + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroYaxis = 0;
                YZero(milliseconds);
            }
            if (ckbYaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                Thread.Sleep(milliseconds * 100);
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        private void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "Z";
            previousZAxis = txtZaxisStepperMove.Text;
            if (ckbZaxisResetToZero.IsChecked == true)
            {
                zeroZaxis = 1;
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + DefaultValues + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroZaxis = 0;
                ZZero(milliseconds);
            }
            if (ckbZaxisResetToZero.IsChecked == false)
            {
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                Thread.Sleep(milliseconds * 100);
                txtZaxisStepperCurrent.Text = currentZAxis;
                ZaxisChanged = false;
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.ZaxisStepperCurrent = txtZaxisStepperCurrent.Text;
            Properties.Settings.Default.ZaxisStepperMove = txtZaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        private void XYAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = "XY";
            if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                zeroYaxis = 1;
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + DefaultValues + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + DefaultValues + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                zeroXaxis = 0;
                zeroYaxis = 0;
                XYZero(milliseconds);
            }
            else if (ckbXaxisResetToZero.IsChecked == true && ckbYaxisResetToZero.IsChecked == false)
            {
                zeroXaxis = 1;
                zeroYaxis = 0;
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + DefaultValues + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + DefaultValues + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                Thread.Sleep(milliseconds * 100);
                txtXaxisStepperCurrent.Text = currentXAxis;
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void XZero(int delay)
        {
            await Task.Delay(delay);
            txtXaxisStepperCurrent.Text = DefaultValues;
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = DefaultValues;
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void YZero(int delay)
        {
            await Task.Delay(delay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = DefaultValues;
            txtYaxisStepperCurrent.Text = DefaultValues;
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void ZZero(int delay)
        {
            await Task.Delay(delay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = DefaultValues;
            txtZaxisStepperCurrent.Text = DefaultValues;
            Properties.Settings.Default.ZaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.ZaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void XYZero(int delay)
        {
            await Task.Delay(delay);
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperCurrent.Text = DefaultValues;
            txtXaxisStepperMove.Text = DefaultValues;
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = DefaultValues;
            txtYaxisStepperCurrent.Text = DefaultValues;
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
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
            XaxisStepperMoveTemp = txtXaxisStepperMove.Text;
        }
        private void YaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            YaxisStepperMoveTemp = txtYaxisStepperMove.Text;
        }
        private void ZaxisStepperMove_GotFocus(object sender, EventArgs e)
        {
            ZaxisStepperMoveTemp = txtZaxisStepperMove.Text;
        }
        private void XaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            XaxisChanged = true;
            txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtXaxisStepperMove.Text == Properties.Settings.Default.XaxisStepperMove)
            {
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void YaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            YaxisChanged = true;
            txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtYaxisStepperMove.Text == Properties.Settings.Default.YaxisStepperMove)
            {
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void ZaxisStepperMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZaxisChanged = true;
            txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.Red;
            if (txtZaxisStepperMove.Text == Properties.Settings.Default.ZaxisStepperMove)
            {
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
        }
        private void XaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperMove.Text = mainWindow.Result;
        }
        private void YaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperMove.Text = mainWindow.Result;
        }
        private void ZaxisStepperMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperMove.Text = mainWindow.Result;
        }
        private void txtXaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisMotorSpeed.Text = mainWindow.Result;
        }
        private void txtYaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisMotorSpeed.Text = mainWindow.Result;
        }
        private void txtZaxisMotorSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisMotorSpeed.Text = mainWindow.Result;
        }
        private void txtXaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtXaxisStepperCurrent.Text = mainWindow.Result;
        }
        private void txtYaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtYaxisStepperCurrent.Text = mainWindow.Result;
        }
        private void txtZaxisStepperCurrent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtZaxisStepperCurrent.Text = mainWindow.Result;
        }
        private void txtBaudRate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keypad mainWindow = new Keypad(this);
            if (mainWindow.ShowDialog() == true)
                txtBaudRate.Text = mainWindow.Result;
        }

        private void cmbComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = cmbComPort.Items.CurrentItem.ToString();
            if (sp.IsOpen == true && selectedItem == "COM1")
            {
                sp.Close();
            }
            if (selectedItem == "COM1")
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
    }
}
