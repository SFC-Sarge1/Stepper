// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 03-26-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 11-14-2024
// ***********************************************************************
// <copyright file="zAxisStepperBoard.ino" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#include <AccelStepper.h>
#include <MultiStepper.h>
#include <LimitSwitch.h>
#include <iostream>

//DEBUG=1 works, DEBUG=0 works now!
#define DEBUG 0
#define APP_VERSION "1.0.0"
#define BUILD_VERSION "001"
#define DIRECTION_CCW -1
#define DIRECTION_CW 1
#define BUFFER_SIZE 64
#define LIMIT_SWITCH1_PIN 14   // Pin for limit switch
#define LIMIT_SWITCH2_PIN 27  // Pin for limit switch

/// <summary>
/// The z axis LimitSwitch Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // create LimitSwitch object that attach to pin 8
/// <summary>
/// The z axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // create LimitSwitch object that attach to pin 10
/// <summary>
/// The z axis direction Clockwise
/// </summary>
int zDirection = DIRECTION_CW;
/// <summary>
/// The buffer size
/// </summary>
char buffer[BUFFER_SIZE];
/// <summary>
/// The buffer size
/// </summary>
int bufferIndex = 0;
/// <summary>
/// The stepper motor interface type
/// </summary>
constexpr int stepperMotorInterfaceType = 1;
/// <summary>
/// The stepper motor steps per rev
/// </summary>
float stepperMotorStepsPerRev = 200.00;
/// <summary>
/// The one full rotation moves mm
/// </summary>
float oneFullRotationMovesMM = 4.00;
/// <summary>
/// The z axis direction pin
/// </summary>
int zAxisDirectionPin = 4;
/// <summary>
/// The z axis pulse pin
/// </summary>
int zAxisPulsePin = 2;
/// <summary>
/// The z axis stepper motor
/// </summary>
AccelStepper zAxisStepperMotor(stepperMotorInterfaceType, zAxisPulsePin, zAxisDirectionPin);
/// <summary>
/// The axis
/// </summary>
String Axis = "Z";
/// <summary>
/// The serial data
/// </summary>
String serialData[] = { "Z", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
/// <summary>
/// The serial data
/// </summary>
int serialDataIndex = 0;
/// <summary>
/// The z axis move mm
/// </summary>
/// <summary>
/// The z axis move mm
/// </summary>
float zAxisMoveMM;  // Public variable for z-axis movement
/// <summary>
/// The z axis current position
/// </summary>
/// <summary>
/// The z axis current position
/// </summary>
float zAxisCurrentPosition;  // Public variable for current position
/// <summary>
/// The z axis motor speed
/// </summary>
/// <summary>
/// The z axis motor speed
/// </summary>
float zAxisMotorSpeed = 400.00;  // Public variable for motor speed
/// <summary>
/// The z axis new position
/// </summary>
/// <summary>
/// The z axis new position
/// </summary>
float zAxisNewPosition;  // Public variable for new position
/// <summary>
/// The z axis acceleration
/// </summary>
float zAxisAcceleration = 1000.00;
/// <summary>
/// The z axis stepper motor maximum speed
/// </summary>
float zAxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The z axis move mm
/// </summary>
float zAxisLimitSwitchMoveMM = 0.00;
/// <summary>
/// The z axis set to zero position
/// </summary>
bool zAxisSetToZeroPosition = false;
/// <summary>
/// The z axis was set to zero position
/// </summary>
bool zAxisWasSetToZeroPosition = false;
/// <summary>
/// The z axis stepper limit switch cw pressed
/// </summary>
bool zAxisStepperLimitSwitchCWPressed = false;
/// <summary>
/// The z axis stepper limit switch CCW pressed
/// </summary>
bool zAxisStepperLimitSwitchCCWPressed = false;
/// <summary>
/// The z axis stepper limit switch cw released
/// </summary>
bool zAxisStepperLimitSwitchCWReleased = false;
/// <summary>
/// The z axis stepper limit switch CCW released
/// </summary>
bool zAxisStepperLimitSwitchCCWReleased = false;
/// <summary>
/// The z axis distance to go
/// </summary>
float zAxisDistanceToGo = 0.00;
/// <summary>
/// The limit switch cw triggered
/// </summary>
static bool limitSwitchCWTriggered = false;
/// <summary>
/// The limit switch CCW triggered
/// </summary>
static bool limitSwitchCCWTriggered = false;
/// <summary>
/// The limit switch triggered
/// </summary>
static int limitSwitchTriggered;
/// <summary>
/// The reset flag
/// </summary>
bool resetFlag = false;  // Flag to indicate if a reset is needed
/// <summary>
/// Setups this instance.
/// </summary>
void setup()
{
	Serial.begin(9600);
	//printNonBlocking("Application Version: " + String(APP_VERSION));
	//printNonBlocking("Build Version: " + String(BUILD_VERSION));

	pinMode(LIMIT_SWITCH1_PIN, INPUT_PULLUP);  // Use internal pull-up resistor
	pinMode(LIMIT_SWITCH2_PIN, INPUT_PULLUP);  // Use internal pull-up resistor
	//Common stuff.
	zAxisStepperMotorLimitSwitchCCW.setDebounceTime(50);  // set debounce time to 50 milliseconds
	zAxisStepperMotorLimitSwitchCW.setDebounceTime(50);   // set debounce time to 50 milliseconds

	serialData[0] = "Z";
	Axis = serialData[0];

	//Z axis stuff.

	serialData[7] = "0.00";
	serialData[8] = "400.00";
	serialData[9] = "0";
	zAxisNewPosition = serialData[7].toFloat();
	zAxisMotorSpeed = serialData[8].toFloat();
	zAxisSetToZeroPosition = serialData[9].toInt();
	zAxisCurrentPosition = 0.00;
	zAxisStepperMotor.setMaxSpeed(zAxisStepperMotorMaxSpeed);
	zAxisStepperMotor.setCurrentPosition(zAxisCurrentPosition);
	zAxisMoveMM = zAxisNewPosition;
	serialDataIndex = 0;
}

/// <summary>
/// Loops this instance.
/// </summary>
void loop()
{
	if (Serial.available())
	{
		serialData[serialDataIndex] = Serial.readStringUntil(',');
		serialDataIndex++;

		if (serialDataIndex == 10)
		{
			zAxisStepperMotor.setCurrentPosition(0.00);
			Serial.println("serialData: " + (String)serialData[0] + "," + (String)serialData[1] + "," + (String)serialData[2] + "," + (String)serialData[3] + "," + (String)serialData[4] + "," + (String)serialData[5] + "," + (String)serialData[6] + "," + (String)serialData[7] + "," + (String)serialData[8] + "," + (String)serialData[9]);
			serialDataIndex = 0;
			Axis = serialData[0];

			//Z axis stuff
			zMotorConfig(serialData[7].toFloat(), serialData[8].toFloat(), serialData[9].toFloat());
			zMotorRun();
		}
	}
}

/// <summary>
/// z axis motor configuration.
/// </summary>
/// <param name="data7">The data7.</param>
/// <param name="data8">The data8.</param>
/// <param name="data9">The data9.</param>
static void zMotorConfig(float data7, float data8, float data9)
{
	//Z axis stuff
	zAxisNewPosition = data7;
	zAxisMotorSpeed = data8;
	if (zAxisMotorSpeed > zAxisStepperMotorMaxSpeed)
	{
		zAxisMotorSpeed = zAxisStepperMotorMaxSpeed;
	}
	zAxisSetToZeroPosition = data9;
	zAxisStepperMotor.setMaxSpeed(zAxisStepperMotorMaxSpeed);

	zAxisMoveMM = static_cast<long>((zAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev);

}
static void zMotorRun()
{
    // Check limit switches
    zAxisStepperMotorLimitSwitchCW.loop();
    zAxisStepperMotorLimitSwitchCCW.loop();
    zAxisStepperMotor.setAcceleration(zAxisAcceleration);
    zAxisStepperMotor.setMaxSpeed(zAxisMotorSpeed);
    zAxisStepperMotor.moveTo(zAxisMoveMM);

    if (zAxisSetToZeroPosition == true)
    {
        zAxisSetToZeroPosition = false;
        zAxisWasSetToZeroPosition = true;
        zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
        zAxisMoveMM = 0.00;
        zAxisStepperMotor.setCurrentPosition(zAxisMoveMM);
        limitSwitchTriggered = 1;
        Serial.println("ESP 32 Board Reset.");
        delay(3000);
        ESP.restart();  // Reset the ESP32 board
    }

    while (zAxisStepperMotor.distanceToGo() != 0)
    {
        if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
        {
            zAxisStepperMotor.stop();
            limitSwitchCWTriggered = true;
            Serial.println("Z Axis CW STOPPED");
           //return;
        }

        if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
        {
            zAxisStepperMotor.stop();
            limitSwitchCCWTriggered = true;
            Serial.println("Z Axis CCW STOPPED");
            //return;
        }
        zAxisStepperMotor.run();
    }

    if (zAxisStepperMotor.distanceToGo() == 0)
    {
        limitSwitchCWTriggered = false;
        limitSwitchCCWTriggered = false;
        limitSwitchTriggered = 1;
        Serial.println("Z Axis reached target position, Z Axis Current Position: " + String(zAxisCurrentPosition));
        zAxisStepperMotor.stop();  // Stop the motor
        zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
        return;
    }
}

