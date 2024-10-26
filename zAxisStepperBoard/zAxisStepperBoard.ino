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
#include "MyXAxis.h"
#include "MyYAxis.h"
#include "MyZAxis.h"
#include <AccelStepper.h>
#include <MultiStepper.h>
#include <AccelStepperWithDistance.h>
#include <LimitSwitch.h>
#include <iostream>
#include <ezButton.h>

//DEBUG=1 works, DEBUG=0 works now!
#define DEBUG 0
#define APP_VERSION "1.0.0"
#define BUILD_VERSION "001"
#define DIRECTION_CCW -1
#define DIRECTION_CW 1
#define BUFFER_SIZE 64
#define LIMIT_SWITCH1_PIN 12  // Pin for limit switch
#define LIMIT_SWITCH2_PIN 13  // Pin for limit switch

/// <summary>
/// Enum for the current ESP32 Board Axis value X=1, Y=2, Z=3
/// </summary>
enum ESP32BoardAxis
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
XAxis xAxis(0.00);
YAxis yAxis(0.00);
ZAxis zAxis(0.00);
/// <summary>
/// The axis number of the ESP32 board code is running on.
/// Axis integer value X=1, Y=2, Z=3
/// </summary>
int axisNumber = 3;  // Axis integer value X=1, Y=2, Z=3
/// <summary>
/// The current axis casted from the axis number.
/// </summary>
ESP32BoardAxis currentAxis = static_cast<ESP32BoardAxis>(axisNumber);
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
//LimitSwitch zAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // Pin for limit switch
ezButton zAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN); // create ezButton object that attach to pin 12
/// <summary>
/// The z axis LimitSwitch Counter-Clockwise
/// </summary>
//LimitSwitch zAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // Pin for limit switch
ezButton zAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN); // create ezButton object that attach to pin 14
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
/// The x axis stepper motor
/// </summary>
AccelStepper xAxisStepperMotor(stepperMotorInterfaceType, xAxis.xAxisPulsePin, xAxis.xAxisDirectionPin);
/// <summary>AccelStepper
/// The y axis stepper motor
/// </summary>
AccelStepper yAxisStepperMotor(stepperMotorInterfaceType, yAxis.yAxisPulsePin, yAxis.yAxisDirectionPin);
/// <summary>
/// The z axis stepper motor
/// </summary>
AccelStepper zAxisStepperMotor(stepperMotorInterfaceType, zAxis.zAxisPulsePin, zAxis.zAxisDirectionPin);
/// <summary>
/// The axis
/// </summary>
String Axis = "X";
/// <summary>
/// The serial data
/// </summary>
String serialData[] = { "XY", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
/// <summary>
/// The serial data
/// </summary>
int serialDataIndex = 0;
/// <summary>
/// The z axis
/// </summary>
ZAxis zAxisFloat(0.00);
/// <summary>
/// Setups this instance.
/// </summary>
void setup()
{
	Serial.begin(9600);
	printNonBlocking("Application Version: " + String(APP_VERSION));
	printNonBlocking("Build Version: " + String(BUILD_VERSION));
	

	//Common stuff.
	zAxisStepperMotorLimitSwitchCCW.setDebounceTime(50);  // set debounce time to 50 milliseconds
	zAxisStepperMotorLimitSwitchCW.setDebounceTime(50);   // set debounce time to 50 milliseconds

	serialData[0] = "XY";
	Axis = serialData[0];

	//X axis stuff.

	serialData[1] = "0.00";
	serialData[2] = "400.00";
	serialData[3] = "0";
	xAxis.xAxisCurrentPosition = 0.00;
	xAxis.xAxisNewPosition = serialData[1].toFloat();
	xAxis.xAxisMotorSpeed = serialData[2].toFloat();
	xAxis.xAxisSetToZeroPosition = serialData[3].toInt();
	xAxisStepperMotor.setMaxSpeed(xAxis.xAxisStepperMotorMaxSpeed);
	xAxisStepperMotor.setCurrentPosition(0.00);

	//Y axis stuff.

	serialData[4] = "0.00";
	serialData[5] = "400.00";
	serialData[6] = "0";
	yAxis.yAxisCurrentPosition = 0.00;
	yAxis.yAxisNewPosition = serialData[4].toFloat();
	yAxis.yAxisMotorSpeed = serialData[5].toFloat();
	yAxis.yAxisSetToZeroPosition = serialData[6].toInt();
	yAxisStepperMotor.setMaxSpeed(yAxis.yAxisStepperMotorMaxSpeed);
	yAxisStepperMotor.setCurrentPosition(0.00);

	//Z axis stuff.

	serialData[7] = "0.00";
	serialData[8] = "400.00";
	serialData[9] = "0";
	zAxis.zAxisNewPosition = serialData[7].toFloat();
	zAxis.zAxisMotorSpeed = serialData[8].toFloat();
	zAxis.zAxisSetToZeroPosition = serialData[9].toInt();
	zAxis.zAxisCurrentPosition = 0.00;
	zAxisStepperMotor.setMaxSpeed(zAxis.zAxisStepperMotorMaxSpeed);
	zAxisStepperMotor.setCurrentPosition(zAxis.zAxisCurrentPosition);
	zAxis.zAxisMoveMM = zAxis.zAxisNewPosition;
	serialDataIndex = 0;

}

/// <summary>
/// Loops this instance.
/// </summary>
void loop()
{
	switch (currentAxis)
	{
	case X:
		xAxisStepperMotorLimitSwitchCW.loop();
		xAxisStepperMotorLimitSwitchCCW.loop();
		break;
	case Y:
		yAxisStepperMotorLimitSwitchCW.loop();
		yAxisStepperMotorLimitSwitchCCW.loop();
		break;
	case Z:
		zAxisStepperMotorLimitSwitchCW.loop();
		if (zAxisStepperMotorLimitSwitchCW.isPressed())
		{
			zAxis.zAxisStepperLimitSwitchCWPressed = true;
			zAxis.zAxisStepperLimitSwitchCCWPressed = false;
		}
		zAxisStepperMotorLimitSwitchCCW.loop();
		if (zAxisStepperMotorLimitSwitchCCW.isPressed())
		{
			zAxis.zAxisStepperLimitSwitchCWPressed = false;
			zAxis.zAxisStepperLimitSwitchCCWPressed = true;
		}
		break;
	}


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
	xAxis.xAxisNewPosition = data1;
	xAxis.xAxisMotorSpeed = data2;
	if (xAxis.xAxisMotorSpeed > xAxis.xAxisStepperMotorMaxSpeed) {
		xAxis.xAxisMotorSpeed = xAxis.xAxisStepperMotorMaxSpeed;
	}
	xAxis.xAxisSetToZeroPosition = data3;
	xAxis.xAxisMoveMM = (xAxis.xAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
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
	yAxis.yAxisNewPosition = data4;
	yAxis.yAxisMotorSpeed = data5;
	if (yAxis.yAxisMotorSpeed > yAxis.yAxisStepperMotorMaxSpeed) {
		yAxis.yAxisMotorSpeed = yAxis.yAxisStepperMotorMaxSpeed;
	}
	yAxis.yAxisSetToZeroPosition = data6;
	yAxis.yAxisMoveMM = (yAxis.yAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
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
	zAxis.zAxisNewPosition = data7;
	zAxis.zAxisMotorSpeed = data8;
	if (zAxis.zAxisMotorSpeed > zAxis.zAxisStepperMotorMaxSpeed)
	{
		zAxis.zAxisMotorSpeed = zAxis.zAxisStepperMotorMaxSpeed;
	}
	zAxis.zAxisSetToZeroPosition = data9;
	zAxis.zAxisMoveMM = (zAxis.zAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
}
/// <summary>
/// xes the motor run.
/// </summary>
static void xMotorRun()
{
	xAxisStepperMotor.moveTo(xAxis.xAxisMoveMM);
	xAxisStepperMotor.setSpeed(xAxis.xAxisMotorSpeed);
	xAxisStepperMotor.setAcceleration(xAxis.xAxisAcceleration);
	if (xAxis.xAxisSetToZeroPosition == true)
	{
		xAxis.xAxisSetToZeroPosition = false;
		xAxis.xAxisWasSetToZeroPosition = true;
		xAxis.xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
		xAxis.xAxisMoveMM = xAxis.xAxisCurrentPosition;
		xAxisStepperMotor.setCurrentPosition(xAxis.xAxisMoveMM);
		printNonBlocking("X," + (String)xAxis.xAxisCurrentPosition);
		NVIC_SystemReset();  //call reset on Arduino board
		//ESP.restart();  //call reset on ESP32 board
	}
	else if (xAxisStepperMotor.distanceToGo() != 0 && xAxis.xAxisSetToZeroPosition == false)
	{
		xAxisStepperMotor.runSpeedToPosition();
		if (xAxisStepperMotorLimitSwitchCW.isPressed())
		{
			printNonBlocking("The limit switch: TOUCHED");
			xAxisStepperMotor.stop();
			xAxisStepperMotor.setCurrentPosition(xAxis.xAxisMoveMM);
			xDirection *= DIRECTION_CCW;  // change direction
			xAxis.xAxisMoveMM = xDirection * 4.00;
			xAxisStepperMotor.moveTo(xAxis.xAxisMoveMM);
			xAxisStepperMotor.setAcceleration(xAxis.xAxisAcceleration);
			xAxisStepperMotor.setSpeed(xAxis.xAxisMotorSpeed);
		}
		else if (xAxisStepperMotorLimitSwitchCW.isReleased())
		{
			printNonBlocking("The limit switch: RELEASED");
			xDirection *= DIRECTION_CW;  // change direction
			xAxis.xAxisMoveMM = xDirection * 4.00;
		}
		if (xAxisStepperMotorLimitSwitchCCW.isPressed())
		{
			printNonBlocking("The limit switch: TOUCHED");
			xAxisStepperMotor.stop();
			xAxisStepperMotor.setCurrentPosition(xAxis.xAxisMoveMM);
			xDirection *= DIRECTION_CCW;  // change direction
			xAxis.xAxisMoveMM = xDirection * 4.00;
			xAxisStepperMotor.moveTo(xAxis.xAxisMoveMM);
			xAxisStepperMotor.setAcceleration(xAxis.xAxisAcceleration);
			xAxisStepperMotor.setSpeed(xAxis.xAxisMotorSpeed);
		}
		else if (xAxisStepperMotorLimitSwitchCCW.isReleased())
		{
			printNonBlocking("The limit switch: RELEASED");
			xDirection *= DIRECTION_CW;  // change direction
			xAxis.xAxisMoveMM = xDirection * 4.00;
		}
	}
	else if (xAxisStepperMotor.distanceToGo() == 0 && xAxis.xAxisSetToZeroPosition == false)
	{
		xAxis.xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
		printNonBlocking("X," + (String)xAxis.xAxisCurrentPosition);
	}
}
/// <summary>
/// ies the motor run.
/// </summary>
static void yMotorRun()
{
	yAxisStepperMotor.moveTo(yAxis.yAxisMoveMM);
	yAxisStepperMotor.setSpeed(yAxis.yAxisMotorSpeed);
	yAxisStepperMotor.setAcceleration(yAxis.yAxisAcceleration);
	if (yAxis.yAxisSetToZeroPosition == true)
	{
		yAxis.yAxisSetToZeroPosition = false;
		yAxis.yAxisWasSetToZeroPosition = true;
		yAxis.yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
		yAxis.yAxisMoveMM = yAxis.yAxisCurrentPosition;
		yAxisStepperMotor.setCurrentPosition(yAxis.yAxisMoveMM);
		printNonBlocking("Y," + (String)yAxis.yAxisCurrentPosition);
		NVIC_SystemReset();  //call reset on Arduino board
		//ESP.restart();  //call reset on ESP32 board
	}
	else if (yAxisStepperMotor.distanceToGo() != 0 && yAxis.yAxisSetToZeroPosition == false)
	{
		yAxisStepperMotor.runSpeedToPosition();
		if (yAxisStepperMotorLimitSwitchCW.isPressed())
		{
			printNonBlocking("The limit switch: TOUCHED");
			yAxisStepperMotor.stop();
			yAxisStepperMotor.setCurrentPosition(yAxis.yAxisMoveMM);
			yDirection *= DIRECTION_CCW;  // change direction
			yAxis.yAxisMoveMM = yDirection * 4.00;
			yAxisStepperMotor.moveTo(yAxis.yAxisMoveMM);
			yAxisStepperMotor.setAcceleration(yAxis.yAxisAcceleration);
			yAxisStepperMotor.setSpeed(yAxis.yAxisMotorSpeed);
		}
		else if (yAxisStepperMotorLimitSwitchCW.isReleased())
		{
			printNonBlocking("The limit switch: RELEASED");
			yDirection *= DIRECTION_CW;  // change direction
			yAxis.yAxisMoveMM = yDirection * 4.00;
		}
		if (yAxisStepperMotorLimitSwitchCCW.isPressed())
		{
			printNonBlocking("The limit switch: TOUCHED");
			yAxisStepperMotor.stop();
			yAxisStepperMotor.setCurrentPosition(yAxis.yAxisMoveMM);
			yDirection *= DIRECTION_CCW;  // change direction
			yAxis.yAxisMoveMM = yDirection * 4.00;
			yAxisStepperMotor.moveTo(yAxis.yAxisMoveMM);
			yAxisStepperMotor.setAcceleration(yAxis.yAxisAcceleration);
			yAxisStepperMotor.setSpeed(yAxis.yAxisMotorSpeed);
		}
		else if (yAxisStepperMotorLimitSwitchCCW.isReleased())
		{
			printNonBlocking("The limit switch: RELEASED");
			yDirection *= DIRECTION_CW;  // change direction
			yAxis.yAxisMoveMM = yDirection * 4.00;
		}
	}
	else if (yAxisStepperMotor.distanceToGo() == 0 && yAxis.yAxisSetToZeroPosition == false)
	{
		yAxis.yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
		printNonBlocking("Y," + (String)yAxis.yAxisCurrentPosition);
	}
}
/// <summary>
/// zs the motor run.
/// </summary>
static void zMotorRun()
{
	zAxisStepperMotor.moveTo(zAxis.zAxisMoveMM);
	zAxisStepperMotor.setAcceleration(zAxis.zAxisAcceleration);
	zAxisStepperMotor.setSpeed(zAxis.zAxisMotorSpeed);
	zAxis.zAxisCurrentPosition = 0.00;


	if (zAxis.zAxisSetToZeroPosition == true)
	{
		zAxis.zAxisSetToZeroPosition = false;
		zAxis.zAxisWasSetToZeroPosition = true;
		zAxis.zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
		zAxis.zAxisMoveMM = 0.00;
		zAxisStepperMotor.setCurrentPosition(zAxis.zAxisMoveMM);
		printNonBlocking("Z," + (String)zAxis.zAxisCurrentPosition);
		NVIC_SystemReset();  //call reset on Arduino or clone board
		//ESP.restart();  //call reset on ESP32 board
	}
	else if (zAxisStepperMotor.distanceToGo() != 0 && zAxis.zAxisSetToZeroPosition == false)
	{
		printNonBlocking("Z Axis Current Position, " + (String)zAxisStepperMotor.currentPosition());
		if (zAxis.zAxisStepperLimitSwitchCWPressed = true)
		{
			printNonBlocking("The Clockwise limit switch: isPressed.");
			zAxisStepperMotor.stop();
			zAxis.zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
			zAxisStepperMotor.setCurrentPosition(zAxis.zAxisCurrentPosition);
			zDirection *= DIRECTION_CCW;  // change direction
			zAxis.zAxisLimitSwitchMoveMM = zDirection * 4.00;
			zAxisStepperMotor.moveTo(zAxis.zAxisLimitSwitchMoveMM);
			zAxisStepperMotor.setAcceleration(zAxis.zAxisAcceleration);
			zAxisStepperMotor.setSpeed(zAxis.zAxisMotorSpeed);
			zAxisStepperMotor.run();
		}
		else if (zAxis.zAxisStepperLimitSwitchCWReleased = true)
		{
			printNonBlocking("The limit switch: RELEASED");
			zDirection *= DIRECTION_CW;  // change direction
			zAxis.zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
			zAxisStepperMotor.setCurrentPosition(zAxis.zAxisCurrentPosition);
			zAxis.zAxisMoveMM = zDirection * 4.00;
		}
		if (zAxis.zAxisStepperLimitSwitchCCWPressed = true)
		{
			printNonBlocking("The Counter-Clockwise limit switch: isPressed.");
			zAxisStepperMotor.stop();
			zAxis.zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
			zAxisStepperMotor.setCurrentPosition(zAxis.zAxisCurrentPosition);
			zDirection *= DIRECTION_CW;  // change direction
			zAxis.zAxisLimitSwitchMoveMM = zDirection * 4.00;
			zAxisStepperMotor.moveTo(zAxis.zAxisLimitSwitchMoveMM);
			zAxisStepperMotor.setAcceleration(zAxis.zAxisAcceleration);
			zAxisStepperMotor.setSpeed(zAxis.zAxisMotorSpeed);
			zAxisStepperMotor.run();
		}
		else if (zAxis.zAxisStepperLimitSwitchCCWReleased = true)
		{
			printNonBlocking("The limit switch: RELEASED");
			zDirection *= DIRECTION_CCW;  // change direction
			zAxis.zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
			zAxisStepperMotor.setCurrentPosition(zAxis.zAxisCurrentPosition);
			zAxis.zAxisMoveMM = zDirection * 4.00;
		}
		else
		{
			printNonBlocking("The limit switch: Not Touched or used.");
			zAxisStepperMotor.moveTo(zAxis.zAxisMoveMM);
			zAxisStepperMotor.setAcceleration(zAxis.zAxisAcceleration);
			zAxisStepperMotor.setSpeed(zAxis.zAxisMotorSpeed);
			zAxisStepperMotor.runSpeedToPosition();
		}
	}
	else if (zAxisStepperMotor.distanceToGo() == 0 && zAxis.zAxisSetToZeroPosition == false)
	{
		printNonBlocking("Z Axis Current Position, " + (String)zAxisStepperMotor.currentPosition());
	}
}
/// <summary>
/// Serials the write.
/// </summary>
/// <param name="c">The c.</param>
void serialWrite(char c)
{
	// Add data to the buffer
	buffer[bufferIndex] = c;
	bufferIndex++;
	// If buffer is full, send data
	if (bufferIndex == BUFFER_SIZE)
	{
		Serial1.write(buffer, BUFFER_SIZE);
		bufferIndex = 0;
		//delay(1000); // Wait for 1 second before sending the next data point
	}
}

/// <summary>
/// Prints the non blocking.
/// </summary>
/// <param name="s">The s.</param>
void printNonBlocking(const String& s)
{
	for (unsigned int i = 0; i < s.length(); i++)
	{
		serialWrite(s[i]);
	}
	// If there's any data left in the buffer, send it
	if (bufferIndex > 0)
	{
		Serial1.write(buffer, bufferIndex);
		bufferIndex = 0;
	}
}
