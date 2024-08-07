# Stepper Motor Controller

## Overview
This repository contains the code for a simple Stepper Motor Controller. The application is written in C# and uses WPF for the user interface. It communicates with an Arduino board over serial communication to control a stepper motor.

## Prerequisites
- .NET 8.0-windows or later
- 3 each Arduino Uno R4 WIFI, one for each axis.
- Visual Studio 2022 Community Edition
- 3 each StepperOnline nema 23 stepper motors
- 3 each StepperOnline DM556T Stepper Motor Driver
- Arduino for Visual Studio, Visual Micro - Release 2024.0223.00 - 5th March 2024 - VS 2017, 2019, 2022
- 6 each Limit Switches, two for each axis.

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
2. The application is written in C# using WPF.
3. The application support touch screens and keyboard input.
4. Main Window:
![MainWindow](https://github.com/SFC-Sarge1/Stepper/blob/master/MainWndow.jpg)
5. Setting Window:
![Settings](https://github.com/SFC-Sarge1/Stepper/blob/master/Settings.jpg)
6. KeyPad Popup Window
   ![KeyPad](https://github.com/SFC-Sarge1/Stepper/blob/master/KeyPad.jpg)  
7. The Application has a Setting file to maintain settings used by the application.
8. The Application creates new and replaces the Application log file each time it runs.
9. Build and run the application.

## Usage
Enter the the distance in milimeters you want each of the X,Y, and Z axis to move. Click the X, Y, or Z axis button to move to the requested milimeters.
