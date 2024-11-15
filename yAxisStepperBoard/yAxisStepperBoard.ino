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
/// The y axis LimitSwitch Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN);  // Pin for limit switch
/// <summary>
/// The y axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN);  // Pin for limit switch
/// <summary>
/// The y axis direction Clockwise
/// </summary>
int yDirection = DIRECTION_CW;
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
/// The y axis direction pin
/// </summary>
int yAxisDirectionPin = 4;
/// <summary>
/// The y axis pulse pin
/// </summary>
int yAxisPulsePin = 2;
/// <summary>AccelStepper
/// The y axis stepper motor
/// </summary>
AccelStepper yAxisStepperMotor(stepperMotorInterfaceType, yAxisPulsePin, yAxisDirectionPin);
/// <summary>
/// The axis
/// </summary>
String Axis = "Y";
/// <summary>
/// The serial data
/// </summary>
String serialData[] = { "Y", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
/// <summary>
/// The serial data
/// </summary>
int serialDataIndex = 0;
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
	yAxisStepperMotorLimitSwitchCCW.setDebounceTime(50);  // set debounce time to 50 milliseconds
	yAxisStepperMotorLimitSwitchCW.setDebounceTime(50);   // set debounce time to 50 milliseconds

	serialData[0] = "Y";
	Axis = serialData[0];

	//Y axis stuff.

	serialData[4] = "0.00";
	serialData[5] = "400.00";
	serialData[6] = "0";
	yAxisCurrentPosition = 0.00;
	yAxisNewPosition = serialData[4].toFloat();
	yAxisMotorSpeed = serialData[5].toFloat();
	yAxisSetToZeroPosition = serialData[6].toInt();
	yAxisCurrentPosition = 0.00;
	yAxisStepperMotor.setMaxSpeed(yAxisStepperMotorMaxSpeed);
	yAxisStepperMotor.setCurrentPosition(0.00);
	yAxisMoveMM = yAxisNewPosition;

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

				//Y axis stuff
				yMotorConfig(serialData[4].toFloat(), serialData[5].toFloat(), serialData[6].toFloat());
				yMotorRun();
		}
	}
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
	//yAxisMoveMM = (yAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev;
	yAxisMoveMM = static_cast<long>((yAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev);

}
/// <summary>
/// Y Axis motor run.
/// </summary>
static void yMotorRun()
{
    // Check limit switches
    yAxisStepperMotorLimitSwitchCW.loop();
    yAxisStepperMotorLimitSwitchCCW.loop();
    yAxisStepperMotor.setAcceleration(yAxisAcceleration);
    yAxisStepperMotor.setMaxSpeed(yAxisMotorSpeed);

    if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
    {
        yAxisStepperMotor.stop();
        limitSwitchCWTriggered = true;
        Serial.println("Z Axis CW STOPPED");
        Serial.println("Z Axis Motor Stopped");
        return;
    }

    if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
    {
        yAxisStepperMotor.stop();
        limitSwitchCCWTriggered = true;
        Serial.println("Z Axis CCW STOPPED");
        Serial.println("Z Axis Motor Stopped");
        return;
    }

    if (yAxisSetToZeroPosition == true)
    {
        yAxisSetToZeroPosition = false;
        yAxisWasSetToZeroPosition = true;
        yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
        yAxisMoveMM = 0.00;
        yAxisStepperMotor.setCurrentPosition(yAxisMoveMM);
        limitSwitchTriggered = 1;
        Serial.println("ESP 32 Board Reset.");
        delay(3000);
        ESP.restart();  // Reset the ESP32 board
    }
    else
    {
        yAxisStepperMotor.moveTo(yAxisMoveMM);
    }

    while (yAxisStepperMotor.distanceToGo() != 0)
    {
        yAxisStepperMotor.run();

        if (!digitalRead(LIMIT_SWITCH1_PIN))  // NC switch is pressed when the pin reads LOW
        {
            yAxisStepperMotor.stop();
            limitSwitchCWTriggered = true;
            Serial.println("Z Axis CW STOPPED");
            Serial.println("Z Axis Motor Stopped");
            return;
        }

        if (!digitalRead(LIMIT_SWITCH2_PIN))  // NC switch is pressed when the pin reads LOW
        {
            yAxisStepperMotor.stop();
            limitSwitchCCWTriggered = true;
            Serial.println("Z Axis CCW STOPPED");
            Serial.println("Z Axis Motor Stopped");
            return;
        }
    }

    if (yAxisStepperMotor.distanceToGo() == 0)
    {
        Serial.println("Z Axis reached target position");
        yAxisStepperMotor.stop();  // Stop the motor
        yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
        Serial.println("Z Axis Current Position: " + String(yAxisCurrentPosition));
    }

    // Reset limit switch flags if moving in the reverse direction
    if (yAxisStepperMotor.distanceToGo() < 0 && limitSwitchCWTriggered)
    {
        limitSwitchCWTriggered = false;
        Serial.println("Z Axis CW Limit Switch Reset");
    }
    else if (yAxisStepperMotor.distanceToGo() > 0 && limitSwitchCCWTriggered)
    {
        limitSwitchCCWTriggered = false;
        Serial.println("Z Axis CCW Limit Switch Reset");
    }
    else if (yAxisStepperMotor.distanceToGo() == 0 && yAxisSetToZeroPosition == false)
    {
        limitSwitchCWTriggered = false;
        limitSwitchCCWTriggered = false;
        limitSwitchTriggered = 1;
        yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
        Serial.println("Z Axis Current Position: " + String(yAxisCurrentPosition));
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