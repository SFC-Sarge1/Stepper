# Stepper Motor Controller

## Overview
This repository contains the code for a simple Stepper Motor Controller. The application is written in C# and uses WPF for the user interface. It communicates with an Arduino board over serial communication to control a stepper motor.

## Prerequisites
- .NET 8.0-windows or later
- Arduino Uno R4 WIFI or Lattepanda 3 Delta
- Visual Studio 2022 Community Edition
- 3 each StepperOnline nema 23 stepper motors
- 3 each StepperOnline DM556T Stepper Motor Driver
- Arduino for Visual Studio, Visual Micro - Release 2024.0223.00 - 5th March 2024 - VS 2017, 2019, 2022

## Setup
1. Connect your stepper motor to the DM556T Stepper Motor Driver and your Arduino board.
2. The example code assumes the motors is connected to pins:
3. Stepper Motor X axis 3, 4
4. Stepper Motor Y axis 5, 6
5. Stepper Motor Z axis 7, 8
6. Upload the provided Arduino sketch to your board.
7. Note the COM port that your Arduino is connected to. This application will load the COM Ports to the form show you can choose.

## Running the Application
1. Open the solution in Visual Studio 2022 Community Edition or higher.
3. Build and run the application.

## Usage
Enter the the distance in milimeters you want each of the X,Y, and Z axis to move. Click the X, Y, or Z axis button to move to the requested milimeters.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
MIT
