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
        public string MotorPin4 = "PE4"; // Blue - 28BYJ48
        public string MotorPin5 = "PE5"; // Pink - 28BYJ48
        public string MotorPin6 = "PE6"; // Yellow - 28BYJ48
        public string MotorPin7 = "PE7"; // Orange - 28BYJ48
        public string MotorPin8 = "PE8"; // Blue - 28BYJ48
        public string MotorPin9 = "PE9"; // Pink - 28BYJ48
        public string MotorPin10 = "PE10"; // Yellow - 28BYJ48
        public string MotorPin11 = "PE11"; // Orange - 28BYJ48
        public int Speed0 = 0;
        public int Move0 = 0;
        public string dirClockwise = "Clockwise"; //Stepper Moter Direction is Clockwise
        public string dirCounterClockwise = "CounterClockwise"; //Stepper Moter Direction is Counter Clockwise
        //public string FullStep = 
        //public string HalfStep =

        public MainWindow()
        {
            InitializeComponent();
            // Provides list of available serial ports
            string[] portNames = SerialPort.GetPortNames();
            // First available port
            string myPortName = portNames[0];
            int baudRate = Convert.ToInt32(txtBaudRate.Text);
            SerialPort sp = new(myPortName, baudRate);
            sp.Open();
            sp.WriteLine("Hello World!");
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            var ControllerPin = txtControllerPin.Text;
            using (var controller = new PinController(new FileIo(), new SysfsPinMapper()))
            {
                var pin136 = controller.GetGpioPin(ControllerPin);
                pin136.PinValue = true;
                Thread.Sleep(5000);
                pin136.PinValue = false;
            }
            using (var pins = new PinController(new FileIo(), new ChipProPinMapper()))
            {
                using (var stepper = new StepperMotor(Convert.ToInt32(txtStepperMove.Text),
                    pins.GetGpioPin(MotorPin4), pins.GetGpioPin(MotorPin5), pins.GetGpioPin(MotorPin6), pins.GetGpioPin(MotorPin7)))
                {
                    stepper.SetSpeed(Convert.ToInt32(txtStepperSpeed.Text));
                    var signalPin = pins.GetGpioPin(MotorPin8);
                    if (cmbDirection.SelectedValue.ToString() == dirClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Clockwise, signalPin, Convert.ToInt32(txtStepperMove.Text));
                        //calculate difference between txtXaxis.Text and txtStepperMove.Text
                        stepper.Move(Convert.ToInt32(txtStepperMove.Text), Direction.Clockwise);
                    }
                    else if (cmbDirection.SelectedValue.ToString() == dirCounterClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Anticlockwise, signalPin, Convert.ToInt32(txtStepperMove.Text));
                        stepper.Move(Convert.ToInt32(txtStepperMove.Text), Direction.Anticlockwise);
                    }
                    Thread.Sleep(1000);
                }
            }
            using (var pins = new PinController(new FileIo(), new ChipProPinMapper()))
            {
                using (var stepper = new StepperMotor(Convert.ToInt32(txtStepperMove.Text),
                    pins.GetGpioPin(MotorPin8), pins.GetGpioPin(MotorPin9), pins.GetGpioPin(MotorPin10), pins.GetGpioPin(MotorPin11)))
                {
                    stepper.SetSpeed(Convert.ToInt32(txtStepperSpeed.Text));
                    var signalPin = pins.GetGpioPin(MotorPin8);
                    if (cmbDirection.SelectedValue.ToString() == dirClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Clockwise, signalPin, Convert.ToInt32(txtStepperMove.Text));
                        stepper.Move(Convert.ToInt32(txtStepperMove.Text), Direction.Clockwise);
                    }
                    else if (cmbDirection.SelectedValue.ToString() == dirCounterClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Anticlockwise, signalPin, Convert.ToInt32(txtStepperMove.Text));
                        stepper.Move(Convert.ToInt32(txtStepperMove.Text), Direction.Anticlockwise);
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            using (var pins = new PinController(new FileIo(), new ChipProPinMapper()))
            {
                using (var stepper = new StepperMotor(Convert.ToInt32(txtStepperMove.Text),
                    pins.GetGpioPin(MotorPin4), pins.GetGpioPin(MotorPin5), pins.GetGpioPin(MotorPin6), pins.GetGpioPin(MotorPin7)))
                {
                    stepper.SetSpeed(Speed0);
                    var signalPin = pins.GetGpioPin(MotorPin8);
                    if (cmbDirection.SelectedValue.ToString() == dirClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Clockwise, signalPin, Move0);
                        stepper.Move(Move0, Direction.Clockwise);
                    }
                    else if (cmbDirection.SelectedValue.ToString() == dirCounterClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Anticlockwise, signalPin, Move0);
                        stepper.Move(Move0, Direction.Anticlockwise);
                    }
                }
            }
            using (var pins = new PinController(new FileIo(), new ChipProPinMapper()))
            {
                using (var stepper = new StepperMotor(Convert.ToInt32(txtStepperMove.Text),
                    pins.GetGpioPin(MotorPin8), pins.GetGpioPin(MotorPin9), pins.GetGpioPin(MotorPin10), pins.GetGpioPin(MotorPin11)))
                {
                    stepper.SetSpeed(Speed0);
                    var signalPin = pins.GetGpioPin(MotorPin8);
                    if (cmbDirection.SelectedValue.ToString() == dirClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Clockwise, signalPin, Move0);
                        stepper.Move(Move0, Direction.Clockwise);
                    }
                    else if (cmbDirection.SelectedValue.ToString() == dirCounterClockwise)
                    {
                        stepper.SetInitialPosition(Direction.Anticlockwise, signalPin, Move0);
                        stepper.Move(Move0, Direction.Anticlockwise);
                    }
                }
            }
        }

    }
}