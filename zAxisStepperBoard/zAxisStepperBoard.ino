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
/// Enum for the current ESP32 Board Axis value X=1, Y=2, Z=3
/// </summary>
enum ArduinoBoardAxis
{
	/// <summary>
	/// The x
	/// </summary>
	X,
	/// <summary>
	/// The y
	/// </summary>
	Y,
	/// <summary>
	/// The z
	/// </summary>
	Z
};
/// <summary>
/// The axis number of the ESP32 board code is running on.
/// Axis integer value X=1, Y=2, Z=3
/// </summary>
/// <summary>
/// The axis number
/// </summary>
int axisNumber = 2;  // Axis integer value X=0, Y=1, Z=2
/// <summary>
/// The current axis casted from the axis number.
/// </summary>
ArduinoBoardAxis currentAxis = static_cast<ArduinoBoardAxis>(axisNumber);
/// <summary>
/// The x axis LimitSwitch Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // Pin for limit switch
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
float zAxisAcceleration = 100.00;
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
	zAxisMoveMM = (zAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
}
/// <summary>
/// Z Axis motor run.
/// </summary>
static void zMotorRun()
{
	// Check limit switches
	zAxisStepperMotorLimitSwitchCW.loop();
	zAxisStepperMotorLimitSwitchCCW.loop();
	zAxisStepperMotor.moveTo(zAxisMoveMM);
	zAxisStepperMotor.setAcceleration(zAxisAcceleration);
	zAxisStepperMotor.setSpeed(zAxisMotorSpeed);
	//zAxisCurrentPosition = 0.00;

	if (zAxisSetToZeroPosition == true)
	{
		zAxisSetToZeroPosition = false;
		zAxisWasSetToZeroPosition = true;
		zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
		zAxisMoveMM = zAxisCurrentPosition;
		zAxisStepperMotor.setCurrentPosition(zAxisMoveMM);
		//printNonBlocking("Z," + (String)zAxisCurrentPosition);
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		Serial.println("ESP 32 Board Reset.");
		delay(3000);
		ESP.restart();  // Reset the ESP32 board
	}
	else if (zAxisStepperMotor.distanceToGo() != 0 && zAxisSetToZeroPosition == false)
	{
		if (!limitSwitchCWTriggered && !limitSwitchCCWTriggered)
		{
			//Serial.println("ESP 32 Board Reset.");
			bool completed = zAxisStepperMotor.runSpeedToPosition();
			if (completed)
			{
				zAxisCurrentPosition = zAxisCurrentPosition + 1.00f;
				//Serial.println("Z Axis Current Position, " + (String)zAxisCurrentPosition);
				completed = false;
				return;
			}
		}
		else
		{
			zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
			zAxisStepperMotor.setCurrentPosition(zAxisCurrentPosition);
			return;
		}
	}
	// Reset limit switch flags if moving in the reverse direction
	else if (zAxisStepperMotor.distanceToGo() < 0 && limitSwitchCCWTriggered)
	{
		limitSwitchCWTriggered = false;
		Serial.println("Z Axis CCW Limit Switch Pressed.");
		bool completed = zAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			zAxisCurrentPosition = zAxisCurrentPosition - 1.00f;
			completed = false;

		}
	}
	else if (zAxisStepperMotor.distanceToGo() > 0 && limitSwitchCWTriggered)
	{
		limitSwitchCCWTriggered = false;
		Serial.println("Z Axis CW Limit Switch Pressed.");
		bool completed = zAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			zAxisCurrentPosition = zAxisCurrentPosition + 1.00f;
			completed = false;

		}
	}
	else if (zAxisStepperMotor.distanceToGo() == 0 && zAxisSetToZeroPosition == false)
	{
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		Serial.println("Z Axis Absolute Position " + (String)zAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		ESP.restart();  // Reset the ESP32 board

	}
	if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
	{
		zAxisStepperMotor.stop();
		limitSwitchCWTriggered = true;
		limitSwitchCCWTriggered = false;
		Serial.println("Z Axis CW Motor Stopped");
		Serial.println("Z Axis CW Motor Current Position: " + (String)zAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}

	if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
	{
		zAxisStepperMotor.stop();
		limitSwitchCCWTriggered = true;
		limitSwitchCWTriggered = false;
		Serial.println("Z Axis CCW Motor Stopped");
		Serial.println("Z Axis CCW Motor Current Position: " + (String)zAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}
}
///// <summary>
///// Serials the write.
///// </summary>
///// <param name="c">The c.</param>
//void serialWrite(char c)
//{
//	// Add data to the buffer
//	buffer[bufferIndex] = c;
//	bufferIndex++;
//	// If buffer is full, send data
//	if (bufferIndex == BUFFER_SIZE)
//	{
//		Serial1.write(buffer, BUFFER_SIZE);
//		bufferIndex = 0;
//		//delay(1000); // Wait for 1 second before sending the next data point
//	}
//}

///// <summary>
///// Prints the non blocking.
///// </summary>
///// <param name="s">The s.</param>
//void printNonBlocking(const String& s)
//{
//	for (unsigned int i = 0; i < s.length(); i++)
//	{
//		serialWrite(s[i]);
//	}
//	// If there's any data left in the buffer, send it
//	if (bufferIndex > 0) {
//		Serial1.write(buffer, bufferIndex);
//		bufferIndex = 0;
//	}
//}