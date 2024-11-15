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
/// The x axis LimitSwitch Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // Pin for limit switch
/// <summary>
/// The x axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // Pin for limit switch
/// <summary>
/// The x axis direction Clockwise
/// </summary>
int xDirection = DIRECTION_CW;
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
AccelStepper xAxisStepperMotor(stepperMotorInterfaceType, xAxisPulsePin, xAxisDirectionPin);
/// <summary>
/// The axis
/// </summary>
String Axis = "X";
/// <summary>
/// The serial data
/// </summary>
String serialData[] = { "X", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
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
	xAxisStepperMotorLimitSwitchCCW.setDebounceTime(50);  // set debounce time to 50 milliseconds
	xAxisStepperMotorLimitSwitchCW.setDebounceTime(50);   // set debounce time to 50 milliseconds

	serialData[0] = "X";
	Axis = serialData[0];

	//X axis stuff.

	serialData[1] = "0.00";
	serialData[2] = "400.00";
	serialData[3] = "0";
	xAxisCurrentPosition = 0.00;
	xAxisNewPosition = serialData[1].toFloat();
	xAxisMotorSpeed = serialData[2].toFloat();
	xAxisSetToZeroPosition = serialData[3].toInt();
	xAxisCurrentPosition = 0.00;
	xAxisStepperMotor.setMaxSpeed(xAxisStepperMotorMaxSpeed);
	xAxisStepperMotor.setCurrentPosition(xAxisCurrentPosition);
	xAxisMoveMM = xAxisNewPosition;

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
			serialDataIndex = 0;
			Axis = serialData[0];

			//X axis stuff
			xMotorConfig(serialData[1].toFloat(), serialData[2].toFloat(), serialData[3].toFloat());
			xMotorRun();
		}
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
	//xAxisMoveMM = (xAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
	xAxisMoveMM = static_cast<long>((xAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev);

}
/// <summary>
/// X Axis motor run.
/// </summary>
static void xMotorRun()
{
    // Check limit switches
    xAxisStepperMotorLimitSwitchCW.loop();
    xAxisStepperMotorLimitSwitchCCW.loop();
    xAxisStepperMotor.setAcceleration(xAxisAcceleration);
    xAxisStepperMotor.setMaxSpeed(xAxisMotorSpeed);

    if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
    {
        xAxisStepperMotor.stop();
        limitSwitchCWTriggered = true;
        Serial.println("Z Axis CW STOPPED");
        Serial.println("Z Axis Motor Stopped");
        return;
    }

    if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
    {
        xAxisStepperMotor.stop();
        limitSwitchCCWTriggered = true;
        Serial.println("Z Axis CCW STOPPED");
        Serial.println("Z Axis Motor Stopped");
        return;
    }

    if (xAxisSetToZeroPosition == true)
    {
        xAxisSetToZeroPosition = false;
        xAxisWasSetToZeroPosition = true;
        xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
        xAxisMoveMM = 0.00;
        xAxisStepperMotor.setCurrentPosition(xAxisMoveMM);
        limitSwitchTriggered = 1;
        Serial.println("ESP 32 Board Reset.");
        delay(3000);
        ESP.restart();  // Reset the ESP32 board
    }
    else
    {
        xAxisStepperMotor.moveTo(xAxisMoveMM);
    }

    while (xAxisStepperMotor.distanceToGo() != 0)
    {
        xAxisStepperMotor.run();

        if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
        {
            xAxisStepperMotor.stop();
            limitSwitchCWTriggered = true;
            Serial.println("Z Axis CW STOPPED");
            Serial.println("Z Axis Motor Stopped");
            return;
        }

        if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
        {
            xAxisStepperMotor.stop();
            limitSwitchCCWTriggered = true;
            Serial.println("Z Axis CCW STOPPED");
            Serial.println("Z Axis Motor Stopped");
            return;
        }
    }

    if (xAxisStepperMotor.distanceToGo() == 0)
    {
        Serial.println("Z Axis reached target position");
        xAxisStepperMotor.stop();  // Stop the motor
        xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
        Serial.println("Z Axis Current Position: " + String(xAxisCurrentPosition));
    }

    // Reset limit switch flags if moving in the reverse direction
    if (xAxisStepperMotor.distanceToGo() < 0 && limitSwitchCWTriggered)
    {
        limitSwitchCWTriggered = false;
        Serial.println("Z Axis CW Limit Switch Reset");
    }
    else if (xAxisStepperMotor.distanceToGo() > 0 && limitSwitchCCWTriggered)
    {
        limitSwitchCCWTriggered = false;
        Serial.println("Z Axis CCW Limit Switch Reset");
    }
    else if (xAxisStepperMotor.distanceToGo() == 0 && xAxisSetToZeroPosition == false)
    {
        limitSwitchCWTriggered = false;
        limitSwitchCCWTriggered = false;
        limitSwitchTriggered = 1;
        xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
        Serial.println("Z Axis Current Position: " + String(xAxisCurrentPosition));
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