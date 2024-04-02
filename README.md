# Stepper Motor Controller

## Overview
This repository contains the code for a simple Stepper Motor Controller. The application is written in C# and uses WPF for the user interface. It communicates with an Arduino board over serial communication to control a stepper motor.

## Prerequisites
- .NET Framework 4.7.2 or later
- Arduino IDE
- An Arduino board
- A stepper motor

## Setup
1. Connect your stepper motor to your Arduino board. The example code assumes the motor is connected to pins 8, 9, 10, and 11.
2. Upload the provided Arduino sketch to your board.
3. Note the COM port that your Arduino is connected to.

## Running the Application
1. Open the solution in Visual Studio.
2. Replace "COM3" in MainWindow.xaml.cs with the COM port your Arduino is connected to.
3. Build and run the application.

## Usage
Enter the number of steps you want the motor to turn in the text box and click the "Send" button. The motor will turn the specified number of steps.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
MIT
