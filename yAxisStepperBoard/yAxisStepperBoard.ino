// ***********************************************************************
// Assembly         :
// Author           : sfcsarge
// Created          : 03-26-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 10-26-2024
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
/// The x axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // Pin for limit switch
/// <summary>
/// The y axis LimitSwitch Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // Pin for limit switch
/// <summary>
/// The y axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // Pin for limit switch
/// <summary>
/// The z axis LimitSwitch Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // create LimitSwitch object that attach to pin 8
/// <summary>
/// The z axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // create LimitSwitch object that attach to pin 10
/// <summary>
/// The x axis direction Clockwise
/// </summary>
int xDirection = DIRECTION_CW;
/// <summary>
/// The y axis direction Clockwise
/// </summary>
int yDirection = DIRECTION_CW;
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
/// The x axis direction pin
/// </summary>
int xAxisDirectionPin = 4;
/// <summary>
/// The x axis pulse pin
/// </summary>
int xAxisPulsePin = 2;
/// <summary>
/// The y axis direction pin
/// </summary>
int yAxisDirectionPin = 4;
/// <summary>
/// The y axis pulse pin
/// </summary>
int yAxisPulsePin = 2;
/// <summary>
/// The z axis direction pin
/// </summary>
int zAxisDirectionPin = 4;
/// <summary>
/// The z axis pulse pin
/// </summary>
int zAxisPulsePin = 2;
/// <summary>
/// The x axis stepper motor
/// </summary>
AccelStepper xAxisStepperMotor(stepperMotorInterfaceType, xAxisPulsePin, xAxisDirectionPin);
/// <summary>AccelStepper
/// The y axis stepper motor
/// </summary>
AccelStepper yAxisStepperMotor(stepperMotorInterfaceType, yAxisPulsePin, yAxisDirectionPin);
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
/// The x axis motor speed
/// </summary>
float xAxisMotorSpeed;  // Public variable for motor speed
/// <summary>
/// The x axis new position
/// </summary>
float xAxisNewPosition;  // Public variable for new position
/// <summary>
/// The x axis acceleration
/// </summary>
float xAxisAcceleration = 50.00;
/// <summary>
/// The x axis stepper motor maximum speed
/// </summary>
float xAxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The x axis current position
/// </summary>
float xAxisCurrentPosition = 0.00;
/// <summary>
/// The x axis move mm
/// </summary>
float xAxisMoveMM = 0.00;
float xAxisLimitSwitchMoveMM = 0.00;
/// <summary>
/// The x axis set to zero position
/// </summary>
bool xAxisSetToZeroPosition = false;
/// <summary>
/// The x axis was set to zero position
/// </summary>
bool xAxisWasSetToZeroPosition = false;
/// <summary>
/// The x axis stepper limit switch cw pressed
/// </summary>
bool xAxisStepperLimitSwitchCWPressed = false;
/// <summary>
/// The x axis stepper limit switch CCW pressed
/// </summary>
bool xAxisStepperLimitSwitchCCWPressed = false;
/// <summary>
/// The x axis stepper limit switch cw released
/// </summary>
bool xAxisStepperLimitSwitchCWReleased = false;
/// <summary>
/// The x axis stepper limit switch CCW released
/// </summary>
bool xAxisStepperLimitSwitchCCWReleased = false;
/// <summary>
/// The x axis distance to go
/// </summary>
float xAxisDistanceToGo = 0.00;
/// <summary>
/// The y axis move mm
/// </summary>
float yAxisMoveMM;  // Public variable for y-axis movement
/// <summary>
/// The y axis current position
/// </summary>
float yAxisCurrentPosition;  // Public variable for current position
/// <summary>
/// The y axis motor speed
/// </summary>
float yAxisMotorSpeed;  // Public variable for motor speed
/// <summary>
/// The y axis new position
/// </summary>
float yAxisNewPosition;  // Public variable for new position
/// <summary>
/// The y axis acceleration
/// </summary>
float yAxisAcceleration = 50.00;
/// <summary>
/// The y axis stepper motor maximum speed
/// </summary>
float yAxisStepperMotorMaxSpeed = 1000.00;
float yAxisLimitSwitchMoveMM = 0.00;
/// <summary>
/// The y axis set to zero position
/// </summary>
bool yAxisSetToZeroPosition = false;
/// <summary>
/// The y axis was set to zero position
/// </summary>
bool yAxisWasSetToZeroPosition = false;
/// <summary>
/// The y axis stepper limit switch cw pressed
/// </summary>
bool yAxisStepperLimitSwitchCWPressed = false;
/// <summary>
/// The y axis stepper limit switch CCW pressed
/// </summary>
bool yAxisStepperLimitSwitchCCWPressed = false;
/// <summary>
/// The y axis stepper limit switch cw released
/// </summary>
bool yAxisStepperLimitSwitchCWReleased = false;
/// <summary>
/// The y axis stepper limit switch CCW released
/// </summary>
bool yAxisStepperLimitSwitchCCWReleased = false;
/// <summary>
/// The y axis distance to go
/// </summary>
float yAxisDistanceToGo = 0.00;
/// <summary>
/// The z axis move mm
/// </summary>
float zAxisMoveMM;  // Public variable for z-axis movement
/// <summary>
/// The z axis current position
/// </summary>
float zAxisCurrentPosition;  // Public variable for current position
/// <summary>
/// The z axis motor speed
/// </summary>
float zAxisMotorSpeed = 400.00;  // Public variable for motor speed
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
static bool limitSwitchCWTriggered = false;
static bool limitSwitchCCWTriggered = false;
static int limitSwitchTriggered;
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

	serialData[0] = "XY";
	Axis = serialData[0];

	//X axis stuff.

	serialData[1] = "0.00";
	serialData[2] = "400.00";
	serialData[3] = "0";
	xAxisCurrentPosition = 0.00;
	xAxisNewPosition = serialData[1].toFloat();
	xAxisMotorSpeed = serialData[2].toFloat();
	xAxisSetToZeroPosition = serialData[3].toInt();
	xAxisStepperMotor.setMaxSpeed(xAxisStepperMotorMaxSpeed);
	xAxisStepperMotor.setCurrentPosition(0.00);

	//Y axis stuff.

	serialData[4] = "0.00";
	serialData[5] = "400.00";
	serialData[6] = "0";
	yAxisCurrentPosition = 0.00;
	yAxisNewPosition = serialData[4].toFloat();
	yAxisMotorSpeed = serialData[5].toFloat();
	yAxisSetToZeroPosition = serialData[6].toInt();
	yAxisStepperMotor.setMaxSpeed(yAxisStepperMotorMaxSpeed);
	yAxisStepperMotor.setCurrentPosition(0.00);

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
	//switch (currentAxis)
	//{
	//case X:
	//	xAxisStepperMotorLimitSwitchCW.loop();
	//	xAxisStepperMotorLimitSwitchCCW.loop();
	//	break;
	//case Y:
	//	yAxisStepperMotorLimitSwitchCW.loop();
	//	yAxisStepperMotorLimitSwitchCCW.loop();
	//	break;
	//case Z:
	//	zAxisStepperMotorLimitSwitchCW.loop();
	//	zAxisStepperMotorLimitSwitchCCW.loop();
	//	break;
	//}
	if (Serial.available())
	{
		serialData[serialDataIndex] = Serial.readStringUntil(',');
		serialDataIndex++;
		if (serialDataIndex == 10)
		{
			serialDataIndex = 0;
			Axis = serialData[0];

			if (Axis == "X")
			{
				//X axis stuff
				xMotorConfig(serialData[1].toFloat(), serialData[2].toFloat(), serialData[3].toFloat());
			}
			else if (Axis == "Y")
			{
				//Y axis stuff
				yMotorConfig(serialData[4].toFloat(), serialData[5].toFloat(), serialData[6].toFloat());
			}
			else if (Axis == "Z")
			{
				//Z axis stuff
				zMotorConfig(serialData[7].toFloat(), serialData[8].toFloat(), serialData[9].toFloat());
			}
			else if (Axis == "XY")
			{
				//X axis stuff
				xMotorConfig(serialData[1].toFloat(), serialData[2].toFloat(), serialData[3].toFloat());
				//Y axis stuff
				yMotorConfig(serialData[4].toFloat(), serialData[5].toFloat(), serialData[6].toFloat());
			}
		}
	}
	if (Axis == "X")
	{
		xMotorRun();
	}
	else if (Axis == "Y")
	{
		yMotorRun();
	}
	else if (Axis == "Z")
	{
		zMotorRun();
	}
	else if (Axis == "XY")
	{
		xMotorRun();
		yMotorRun();
	}
}

/// <summary>
/// x axis motor configuration.
/// </summary>
/// <param name="data1">The data1.</param>
/// <param name="data2">The data2.</param>
/// <param name="data3">The data3.</param>
static void xMotorConfig(float data1, float data2, float data3)
{
	//X axis stuff
	xAxisNewPosition = data1;
	xAxisMotorSpeed = data2;
	if (xAxisMotorSpeed > xAxisStepperMotorMaxSpeed)
	{
		xAxisMotorSpeed = xAxisStepperMotorMaxSpeed;
	}
	xAxisSetToZeroPosition = data3;
	xAxisMoveMM = (xAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
}
/// <summary>
/// y axis motor configuration.
/// </summary>
/// <param name="data4">The data4.</param>
/// <param name="data5">The data5.</param>
/// <param name="data6">The data6.</param>
static void yMotorConfig(float data4, float data5, float data6)
{
	//Y axis stuff
	yAxisNewPosition = data4;
	yAxisMotorSpeed = data5;
	if (yAxisMotorSpeed > yAxisStepperMotorMaxSpeed)
	{
		yAxisMotorSpeed = yAxisStepperMotorMaxSpeed;
	}
	yAxisSetToZeroPosition = data6;
	yAxisMoveMM = (yAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
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
/// X Axis motor run.
/// </summary>
static void xMotorRun()
{
	// Check limit switches
	xAxisStepperMotorLimitSwitchCW.loop();
	xAxisStepperMotorLimitSwitchCCW.loop();
	xAxisStepperMotor.moveTo(xAxisMoveMM);
	xAxisStepperMotor.setAcceleration(xAxisAcceleration);
	xAxisStepperMotor.setSpeed(xAxisMotorSpeed);
	//xAxisCurrentPosition = 0.00;

	if (xAxisSetToZeroPosition == true)
	{
		xAxisSetToZeroPosition = false;
		xAxisWasSetToZeroPosition = true;
		xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
		xAxisMoveMM = xAxisCurrentPosition;
		xAxisStepperMotor.setCurrentPosition(xAxisMoveMM);
		//printNonBlocking("X," + (String)xAxisCurrentPosition);
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		Serial.println("ESP 32 Board Reset.");
		delay(3000);
		ESP.restart();  // Reset the ESP32 board
	}
	else if (xAxisStepperMotor.distanceToGo() != 0 && xAxisSetToZeroPosition == false)
	{
		if (!limitSwitchCWTriggered && !limitSwitchCCWTriggered)
		{
			//Serial.println("ESP 32 Board Reset.");
			bool completed = xAxisStepperMotor.runSpeedToPosition();
			if (completed)
			{
				xAxisCurrentPosition = xAxisCurrentPosition + 1.00f;
				//Serial.println("X Axis Current Position, " + (String)xAxisCurrentPosition);
				completed = false;
				return;
			}
		}
		else
		{
			xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
			xAxisStepperMotor.setCurrentPosition(xAxisCurrentPosition);
			return;
		}
	}
	// Reset limit switch flags if moving in the reverse direction
	else if (xAxisStepperMotor.distanceToGo() < 0 && limitSwitchCCWTriggered)
	{
		limitSwitchCWTriggered = false;
		Serial.println("X Axis CCW Limit Switch Pressed.");
		bool completed = xAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			xAxisCurrentPosition = xAxisCurrentPosition - 1.00f;
			completed = false;

		}
	}
	else if (xAxisStepperMotor.distanceToGo() > 0 && limitSwitchCWTriggered)
	{
		limitSwitchCCWTriggered = false;
		Serial.println("X Axis CW Limit Switch Pressed.");
		bool completed = xAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			xAxisCurrentPosition = xAxisCurrentPosition + 1.00f;
			completed = false;

		}
	}
	else if (xAxisStepperMotor.distanceToGo() == 0 && xAxisSetToZeroPosition == false)
	{
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		Serial.println("X Axis Absolute Position " + (String)xAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		ESP.restart();  // Reset the ESP32 board

	}
	if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
	{
		xAxisStepperMotor.stop();
		limitSwitchCWTriggered = true;
		limitSwitchCCWTriggered = false;
		Serial.println("X Axis CW Motor Stopped");
		Serial.println("X Axis CW Motor Current Position: " + (String)xAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}

	if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
	{
		xAxisStepperMotor.stop();
		limitSwitchCCWTriggered = true;
		limitSwitchCWTriggered = false;
		Serial.println("X Axis CCW Motor Stopped");
		Serial.println("X Axis CCW Motor Current Position: " + (String)xAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}
}
/// <summary>
/// Y Axis motor run.
/// </summary>
static void yMotorRun()
{
	// Check limit switches
	yAxisStepperMotorLimitSwitchCW.loop();
	yAxisStepperMotorLimitSwitchCCW.loop();
	yAxisStepperMotor.moveTo(yAxisMoveMM);
	yAxisStepperMotor.setAcceleration(yAxisAcceleration);
	yAxisStepperMotor.setSpeed(yAxisMotorSpeed);
	//yAxisCurrentPosition = 0.00;

	if (yAxisSetToZeroPosition == true)
	{
		yAxisSetToZeroPosition = false;
		yAxisWasSetToZeroPosition = true;
		yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
		yAxisMoveMM = yAxisCurrentPosition;
		yAxisStepperMotor.setCurrentPosition(yAxisMoveMM);
		//printNonBlocking("Y," + (String)yAxisCurrentPosition);
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		Serial.println("ESP 32 Board Reset.");
		delay(3000);
		ESP.restart();  // Reset the ESP32 board
	}
	else if (yAxisStepperMotor.distanceToGo() != 0 && yAxisSetToZeroPosition == false)
	{
		if (!limitSwitchCWTriggered && !limitSwitchCCWTriggered)
		{
			//Serial.println("ESP 32 Board Reset.");
			bool completed = yAxisStepperMotor.runSpeedToPosition();
			if (completed)
			{
				yAxisCurrentPosition = yAxisCurrentPosition + 1.00f;
				//Serial.println("Y Axis Current Position, " + (String)yAxisCurrentPosition);
				completed = false;
				return;
			}
		}
		else
		{
			yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
			yAxisStepperMotor.setCurrentPosition(yAxisCurrentPosition);
			return;
		}
	}
	// Reset limit switch flags if moving in the reverse direction
	else if (yAxisStepperMotor.distanceToGo() < 0 && limitSwitchCCWTriggered)
	{
		limitSwitchCWTriggered = false;
		Serial.println("Y Axis CCW Limit Switch Pressed.");
		bool completed = yAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			yAxisCurrentPosition = yAxisCurrentPosition - 1.00f;
			completed = false;

		}
	}
	else if (yAxisStepperMotor.distanceToGo() > 0 && limitSwitchCWTriggered)
	{
		limitSwitchCCWTriggered = false;
		Serial.println("Y Axis CW Limit Switch Pressed.");
		bool completed = yAxisStepperMotor.runSpeedToPosition();
		if (completed)
		{
			yAxisCurrentPosition = yAxisCurrentPosition + 1.00f;
			completed = false;

		}
	}
	else if (yAxisStepperMotor.distanceToGo() == 0 && yAxisSetToZeroPosition == false)
	{
		limitSwitchCWTriggered = false;
		limitSwitchCCWTriggered = false;
		Serial.println("Y Axis Absolute Position " + (String)yAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		ESP.restart();  // Reset the ESP32 board

	}
	if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
	{
		yAxisStepperMotor.stop();
		limitSwitchCWTriggered = true;
		limitSwitchCCWTriggered = false;
		Serial.println("Y Axis CW Motor Stopped");
		Serial.println("Y Axis CW Motor Current Position: " + (String)yAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}

	if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
	{
		yAxisStepperMotor.stop();
		limitSwitchCCWTriggered = true;
		limitSwitchCWTriggered = false;
		Serial.println("Y Axis CCW Motor Stopped");
		Serial.println("Y Axis CCW Motor Current Position: " + (String)yAxisCurrentPosition);
		//NVIC_SystemReset();  //call reset on Arduino or clone board
		//Serial.println("ESP 32 Board Reset.");
		//delay(3000);
		//ESP.restart();  // Reset the ESP32 board
		return;
	}
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