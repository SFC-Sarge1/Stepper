
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
    using ControlzEx.Theming;
    using System.IO;
    using System.Runtime.Intrinsics.Arm;


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
        //1 RPM move the screw 4mm. Each revolution move the screw 4mm. How far will the screw move in 20 RPM, and how many seconds will it take?
        //4mm / 1 revolution = 1 revolution / 20 RPM = 0.05 revolutions
        //0.05 revolutions * 4mm = 0.2mm
        //0.2mm / 20 RPM = 0.01mm per second
        //0.01mm per second * 1000 = 10 milliseconds
        //The distance the screw moves is directly proportional to the RPM.If 1 RPM moves the screw 4mm, then at 20 RPM(which is 20 times faster), the screw would move 20 times the distance.
        //So, the distance the screw would move at 20 RPM is 20×4 = 80mm.
        //Therefore, at 20 RPM, the screw would move 80mm.
        //As for the time, if we consider that each revolution(1 RPM) takes 60 seconds(as there are 60 seconds in a minute), then at 20 RPM, each revolution would take
        //<math xmlns="http://www.w3.org/1998/Math/MathML" display="block"><semantics><mrow><mfrac><mn>60</mn><mn>20</mn></mfrac><mo>=</mo><mn>3</mn></mrow><annotation encoding="application/x-tex">\frac{60}{20} = 3</annotation></semantics></math>
        // seconds.Therefore, to move 80mm at 20 RPM, it would take
        //<math xmlns="http://www.w3.org/1998/Math/MathML" display="block"><semantics><mrow><mfrac><mn>80</mn><mn>4</mn></mfrac><mo>×</mo><mn>3</mn><mo>=</mo><mn>60</mn></mrow><annotation encoding="application/x-tex">\frac{80}{4} \times 3 = 60</annotation></semantics></math>
        // seconds.So, it would take 60 seconds to move the screw 80mm at 20 RPM.
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
        public async void XAxisRun_Click(object sender, RoutedEventArgs e)
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
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                }
                decimal part1 = stepperMove / 4;
                decimal part2 = 60 / Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                sp.Write(stringValue);
                await Task.Delay(delay);
                currentXAxis = Convert.ToString(Convert.ToDecimal(txtXaxisStepperCurrent.Text) + Convert.ToDecimal(txtXaxisStepperMove.Text));
                txtXaxisStepperCurrent.Text = currentXAxis;
                XaxisChanged = false;
                txtXaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.XaxisStepperCurrent = Convert.ToDecimal(txtXaxisStepperCurrent.Text);
            Properties.Settings.Default.XaxisStepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void YAxisRun_Click(object sender, RoutedEventArgs e)
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
                if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                }
                decimal part1 = stepperMove / 4;
                decimal part2 = 60 / Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                sp.Write(stringValue);
                await Task.Delay(delay);
                currentYAxis = Convert.ToString(Convert.ToDecimal(txtYaxisStepperCurrent.Text) + Convert.ToDecimal(txtYaxisStepperMove.Text));
                txtYaxisStepperCurrent.Text = currentYAxis;
                YaxisChanged = false;
                txtYaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.YaxisStepperCurrent = Convert.ToDecimal(txtYaxisStepperCurrent.Text);
            Properties.Settings.Default.YaxisStepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void ZAxisRun_Click(object sender, RoutedEventArgs e)
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
                if (Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtZaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text.Trim());
                }
                decimal part1 = stepperMove / 4;
                decimal part2 = 60 / Convert.ToDecimal(txtZaxisMotorSpeed.Text);
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                sp.Write(stringValue);
                currentZAxis = Convert.ToString(Convert.ToDecimal(txtZaxisStepperCurrent.Text) + Convert.ToDecimal(txtZaxisStepperMove.Text));
                await Task.Delay(delay);
                txtZaxisStepperCurrent.Text = currentZAxis;
                ZaxisChanged = false;
                txtZaxisStepperMove.BorderBrush = System.Windows.Media.Brushes.White;
            }
            Properties.Settings.Default.ZaxisStepperCurrent = Convert.ToDecimal(txtZaxisStepperCurrent.Text);
            Properties.Settings.Default.ZaxisStepperMove = Convert.ToDecimal(txtZaxisStepperMove.Text);
            Properties.Settings.Default.Save();
        }
        public async void XYAxisRun_Click(object sender, RoutedEventArgs e)
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
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                {
                    stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                }
                else
                {
                    stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                }
                decimal part1 = stepperMove / 4;
                decimal part2 = 60 / Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                await Task.Delay(delay);
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
                decimal part1 = stepperMove / 4;
                decimal part2 = 60 / Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                await Task.Delay(delay);
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
                decimal part2 = Convert.ToDecimal("0.00");
                if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) >= Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()))
                { 
                    if (Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtXaxisStepperMove.Text.Trim()));
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtXaxisStepperMove.Text.Trim());
                    }
                    part2 = 60 / Convert.ToDecimal(txtXaxisMotorSpeed.Text);
                }
                else
                {
                    if (Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()) < 0)
                    {
                        stepperMove = Math.Abs(Convert.ToDecimal(txtYaxisStepperMove.Text.Trim()));
                    }
                    else
                    {
                        stepperMove = Convert.ToDecimal(txtYaxisStepperMove.Text.Trim());
                    }
                    part2 = 60 / Convert.ToDecimal(txtYaxisMotorSpeed.Text);
                }
                decimal part1 = stepperMove / 4;
                decimal myDelay = part1 * part2;
                int delay = milliseconds * Convert.ToInt32(myDelay);
                await Task.Delay(delay);
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
