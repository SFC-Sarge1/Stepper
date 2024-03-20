
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
        public string myPortName; // Serial Port Name (COM1, COM2, COM3, etc.)
        public string stringValue = "";
        public int zeroXaxis = Properties.Settings.Default.zeroXaxis;
        public int zeroYaxis = Properties.Settings.Default.zeroYaxis;
        public int zeroZaxis = Properties.Settings.Default.zeroZaxis;
        public int milliseconds = Properties.Settings.Default.Milliseconds;
        public string axis = Properties.Settings.Default.RootAxisX;
        public string currentXAxis = Properties.Settings.Default.Value_0_00;
        public string currentYAxis = Properties.Settings.Default.Value_0_00;
        public string currentZAxis = Properties.Settings.Default.Value_0_00;
        public string previousXAxis = Properties.Settings.Default.Value_0_00;
        public string previousYAxis = Properties.Settings.Default.Value_0_00;
        public string previousZAxis = Properties.Settings.Default.Value_0_00;
        public bool XaxisChanged = false;
        public bool YaxisChanged = false;
        public bool ZaxisChanged = false;
        public string XaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00;
        public string YaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00;
        public string ZaxisStepperMoveTemp = Properties.Settings.Default.Value_0_00;
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
            selectedItem = cmbComPort.Items.CurrentItem.ToString();
            if (selectedItem == Properties.Settings.Default.COM1)
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
            else if (selectedItem == Properties.Settings.Default.COM4)
            {
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
            else if (selectedItem == Properties.Settings.Default.COM5)
            {
                myPortName = selectedItem;
                cmbComPort.Text = selectedItem;
                sp = new(myPortName, Convert.ToInt32(txtBaudRate.Text));
                if (sp.IsOpen == false)
                {
                    sp.Open();
                }
            }
            // Create a ScrollViewer
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            // Add the Grid to the ScrollViewer
            scrollViewer.Content = StepperMotorControl;

            // Add the ScrollViewer to the window
            this.Content = scrollViewer;

        }
        private void XAxisRun_Click(object sender, RoutedEventArgs e)
        {
            axis = Properties.Settings.Default.RootAxisX;
            previousXAxis = txtXaxisStepperMove.Text;
            if (ckbXaxisResetToZero.IsChecked == true)
            {
                zeroXaxis = 1;
                stringValue = axis + "," + (Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text.Trim())).ToString() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = axis + "," + Properties.Settings.Default.Value_0_00 + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        private void YAxisRun_Click(object sender, RoutedEventArgs e)
        {
            previousYAxis = txtYaxisStepperMove.Text;
            if (ckbYaxisResetToZero.IsChecked == true)
            {
                zeroYaxis = 1;
                stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + (Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text.Trim())).ToString() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00 + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        private void ZAxisRun_Click(object sender, RoutedEventArgs e)
        {
            previousZAxis = txtZaxisStepperMove.Text;
            if (ckbZaxisResetToZero.IsChecked == true)
            {
                zeroZaxis = 1;
                stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + (Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text.Trim())).ToString() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
                sp.Write(stringValue);
                stringValue = Properties.Settings.Default.RootAxisZ + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + Properties.Settings.Default.Value_0_00 + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
            Properties.Settings.Default.ZaxisStepperCurrent = txtZaxisStepperCurrent.Text;
            Properties.Settings.Default.ZaxisStepperMove = txtZaxisStepperMove.Text;
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
                stringValue = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00 + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00 + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
                stringValue = Properties.Settings.Default.RootAxisXY + "," + Properties.Settings.Default.Value_0_00 + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + txtYaxisStepperMove.Text.Trim() + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
                stringValue = Properties.Settings.Default.RootAxisXY + "," + txtXaxisStepperMove.Text.Trim() + "," + txtXaxisMotorSpeed.Text.Trim() + "," + zeroXaxis.ToString() + "," + Properties.Settings.Default.Value_0_00 + "," + txtYaxisMotorSpeed.Text.Trim() + "," + zeroYaxis.ToString() + "," + txtZaxisStepperMove.Text.Trim() + "," + txtZaxisMotorSpeed.Text.Trim() + "," + zeroZaxis.ToString();
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
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void XZero(int delay)
        {
            await Task.Delay(delay);
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00;
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00;
            Properties.Settings.Default.XaxisStepperCurrent = txtXaxisStepperCurrent.Text;
            Properties.Settings.Default.XaxisStepperMove = txtXaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void YZero(int delay)
        {
            await Task.Delay(delay);
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00;
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00;
            Properties.Settings.Default.YaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.YaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void ZZero(int delay)
        {
            await Task.Delay(delay);
            ckbZaxisResetToZero.IsChecked = false;
            txtZaxisStepperMove.Text = Properties.Settings.Default.Value_0_00;
            txtZaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00;
            Properties.Settings.Default.ZaxisStepperCurrent = txtYaxisStepperCurrent.Text;
            Properties.Settings.Default.ZaxisStepperMove = txtYaxisStepperMove.Text;
            Properties.Settings.Default.Save();
        }
        public async void XYZero(int delay)
        {
            await Task.Delay(delay);
            ckbXaxisResetToZero.IsChecked = false;
            txtXaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00;
            txtXaxisStepperMove.Text = Properties.Settings.Default.Value_0_00;
            ckbYaxisResetToZero.IsChecked = false;
            txtYaxisStepperMove.Text = Properties.Settings.Default.Value_0_00;
            txtYaxisStepperCurrent.Text = Properties.Settings.Default.Value_0_00;
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
            if (sp.IsOpen == true && selectedItem == Properties.Settings.Default.COM1)
            {
                sp.Close();
            }
            if (selectedItem == Properties.Settings.Default.COM1)
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
    }
}
