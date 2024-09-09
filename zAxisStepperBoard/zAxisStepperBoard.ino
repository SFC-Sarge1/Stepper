// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 03-26-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 09-07-2024
// ***********************************************************************
// <copyright file="ArdunioStepper.ino" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#include <MultiStepper.h>
#include <AccelStepper.h>
#include <LimitSwitch.h>
#include <iostream>

//DEBUG=1 works, DEBUG=0 works now!
#define DEBUG 0
#define APP_VERSION "1.0.0"
#define BUILD_VERSION "001"
#define DIRECTION_CCW -1
#define DIRECTION_CW 1
#define BUFFER_SIZE 64
#define LIMIT_SWITCH1_PIN 12 // Pin for limit switch
#define LIMIT_SWITCH2_PIN 14 // Pin for limit switch

/// <summary>
/// Enum for the current ESP32 Board Axis value X=1, Y=2, Z=3
/// </summary>
enum ESP32BoardAxis
{
    X,
    Y,
    Z
};
/// <summary>
/// The axis number of the ESP32 board code in running on.
/// Axis integer value X=1, Y=2, Z=3
/// </summary>
int axisNumber = 3; // Axis integer value X=1, Y=2, Z=3
/// <summary>
/// The current axis casted from the axis number.
/// </summary>
ESP32BoardAxis currentAxis = static_cast<ESP32BoardAxis>(axisNumber);;
/// <summary>
/// The x axis LimitSwitch Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN); // Pin for limit switch
/// <summary>
/// The x axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch xAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN); // Pin for limit switch
/// <summary>
/// The y axis LimitSwitch Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN); // Pin for limit switch
/// <summary>
/// The y axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch yAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN); // Pin for limit switch
/// <summary>
/// The z axis LimitSwitch Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCW(LIMIT_SWITCH1_PIN); // Pin for limit switch
/// <summary>
/// The z axis LimitSwitch Counter-Clockwise
/// </summary>
LimitSwitch zAxisStepperMotorLimitSwitchCCW(LIMIT_SWITCH2_PIN); // Pin for limit switch
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
/// The buffer index
/// </summary>
int bufferIndex = 0;
/// <summary>
/// The x axis direction pin
/// </summary>
constexpr int xAxisDirectionPin = 4;
/// <summary>
/// The x axis pulse pin
/// </summary>
constexpr int xAxisPulsePin = 2;
/// <summary>
/// The y axis direction pin
/// </summary>
constexpr int yAxisDirectionPin = 4;
/// <summary>
/// The y axis pulse pin
/// </summary>
constexpr int yAxisPulsePin = 2;
/// <summary>
/// The z axis direction pin
/// </summary>
constexpr int zAxisDirectionPin = 4;
/// <summary>
/// The z axis pulse pin
/// </summary>
constexpr int zAxisPulsePin = 2;
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
AccelStepper xAxisStepperMotor = AccelStepper(stepperMotorInterfaceType, xAxisPulsePin, xAxisDirectionPin);
/// <summary>
/// The y axis stepper motor
/// </summary>
AccelStepper yAxisStepperMotor = AccelStepper(stepperMotorInterfaceType, yAxisPulsePin, yAxisDirectionPin);
/// <summary>
/// The z axis stepper motor
/// </summary>
AccelStepper zAxisStepperMotor = AccelStepper(stepperMotorInterfaceType, zAxisPulsePin, zAxisDirectionPin);
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
/// The x axis new position
/// </summary>
float xAxisNewPosition = 0.00;
/// <summary>
/// The x axis move mm
/// </summary>
float xAxisMoveMM = 0.00;
/// <summary>
/// The x axis distance to go
/// </summary>
float xAxisDistanceToGo = 0.00;
/// <summary>
/// The y axis distance to go
/// </summary>
float yAxisDistanceToGo = 0.00;
/// <summary>
/// The z axis distance to go
/// </summary>
float zAxisDistanceToGo = 0.00;
/// <summary>
/// The x axis motor speed
/// </summary>
float xAxisMotorSpeed = 400.00;
/// <summary>
/// The x axis set to zero position
/// </summary>
bool xAxisSetToZeroPosition = false;
/// <summary>
/// The x axis was set to zero position
/// </summary>
bool xAxisWasSetToZeroPosition = false;
/// <summary>
/// The y axis acceleration
/// </summary>
float yAxisAcceleration = 50.00;
/// <summary>
/// The y axis stepper motor maximum speed
/// </summary>
float yAxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The y axis current position
/// </summary>
float yAxisCurrentPosition = 0.00;
/// <summary>
/// The y axis new position
/// </summary>
float yAxisNewPosition = 0.00;
/// <summary>
/// The y axis move mm
/// </summary>
float yAxisMoveMM = 0.00;
/// <summary>
/// The y axis motor speed
/// </summary>
float yAxisMotorSpeed = 400.00;
/// <summary>
/// The y axis set to zero position
/// </summary>
bool yAxisSetToZeroPosition = false;
/// <summary>
/// The y axis was set to zero position
/// </summary>
bool yAxisWasSetToZeroPosition = false;
/// <summary>
/// The z axis acceleration
/// </summary>
float zAxisAcceleration = 100.00;
/// <summary>
/// The z axis stepper motor maximum speed
/// </summary>
float zAxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The z axis current position
/// </summary>
float zAxisCurrentPosition = 0.00;
/// <summary>
/// The z axis new position
/// </summary>
float zAxisNewPosition = 0.00;
/// <summary>
/// The z axis move mm
/// </summary>
float zAxisMoveMM = 0.00;
/// <summary>
/// The z axis motor speed
/// </summary>
float zAxisMotorSpeed = 400.00;
/// <summary>
/// The z axis set to zero position
/// </summary>
bool zAxisSetToZeroPosition = false;
/// <summary>
/// The z axis was set to zero position
/// </summary>
bool zAxisWasSetToZeroPosition = false;
/// <summary>
/// The axis
/// </summary>
String Axis = "X";
/// <summary>
/// The serial data
/// </summary>
String serialData[] = { "XY", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
/// <summary>
/// The serial data index
/// </summary>
int serialDataIndex = 0;
/// <summary>
/// Setups this instance.
/// </summary>
void setup()
{
    Serial.begin(9600);
    printNonBlocking("Application Version: " + String(APP_VERSION));
    printNonBlocking("Build Version: " + String(BUILD_VERSION));

    //Common stuff.
    zAxisStepperMotorLimitSwitchCCW.setDebounceTime(50); // set debounce time to 50 milliseconds
    zAxisStepperMotorLimitSwitchCW.setDebounceTime(50); // set debounce time to 50 milliseconds

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
    zAxisCurrentPosition = 0.00;
    zAxisNewPosition = serialData[7].toFloat();
    zAxisMotorSpeed = serialData[8].toFloat();
    zAxisSetToZeroPosition = serialData[9].toInt();
    zAxisStepperMotor.setMaxSpeed(zAxisStepperMotorMaxSpeed);
    zAxisStepperMotor.setCurrentPosition(0.00);

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
        zAxisStepperMotorLimitSwitchCCW.loop();
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
/// xes the motor run.
/// </summary>
static void xMotorRun()
{
    xAxisStepperMotor.moveTo(xAxisMoveMM);
    xAxisStepperMotor.setSpeed(xAxisMotorSpeed);
    xAxisStepperMotor.setAcceleration(xAxisAcceleration);
    if (xAxisSetToZeroPosition == true)
    {
        xAxisSetToZeroPosition = false;
        xAxisWasSetToZeroPosition = true;
        xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
        xAxisMoveMM = xAxisCurrentPosition;
        printNonBlocking("X," + (String)xAxisCurrentPosition);
        //NVIC_SystemReset();  //call reset on Ardunio board
        ESP.restart(); //call reset on ESP32 board
    }
    else if (xAxisStepperMotor.distanceToGo() != 0 && xAxisSetToZeroPosition == false)
    {
        xAxisStepperMotor.runSpeedToPosition();
        if (xAxisStepperMotorLimitSwitchCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            xAxisStepperMotor.stop();
            xAxisStepperMotor.setCurrentPosition(xAxisMoveMM);
            xDirection *= DIRECTION_CCW; // change direction
            xAxisMoveMM = xDirection * 4.00;
            xAxisStepperMotor.moveTo(xAxisMoveMM);
            xAxisStepperMotor.setAcceleration(xAxisAcceleration);
            xAxisStepperMotor.setSpeed(xAxisMotorSpeed);
        }
        else if (xAxisStepperMotorLimitSwitchCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            xDirection *= DIRECTION_CW; // change direction
            xAxisMoveMM = xDirection * 4.00;
        }
        if (xAxisStepperMotorLimitSwitchCCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            xAxisStepperMotor.stop();
            xAxisStepperMotor.setCurrentPosition(xAxisMoveMM);
            xDirection *= DIRECTION_CCW; // change direction
            xAxisMoveMM = xDirection * 4.00;
            xAxisStepperMotor.moveTo(xAxisMoveMM);
            xAxisStepperMotor.setAcceleration(xAxisAcceleration);
            xAxisStepperMotor.setSpeed(xAxisMotorSpeed);
        }
        else if (xAxisStepperMotorLimitSwitchCCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            xDirection *= DIRECTION_CW; // change direction
            xAxisMoveMM = xDirection * 4.00;
        }
    }
    else if (xAxisStepperMotor.distanceToGo() == 0 && xAxisSetToZeroPosition == false)
    {
        xAxisCurrentPosition = xAxisStepperMotor.currentPosition();
        printNonBlocking("X," + (String)xAxisCurrentPosition);
    }
}
/// <summary>
/// ies the motor run.
/// </summary>
static void yMotorRun()
{
    yAxisStepperMotor.moveTo(yAxisMoveMM);
    yAxisStepperMotor.setSpeed(yAxisMotorSpeed);
    yAxisStepperMotor.setAcceleration(yAxisAcceleration);
    if (yAxisSetToZeroPosition == true)
    {
        yAxisSetToZeroPosition = false;
        yAxisWasSetToZeroPosition = true;
        yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
        yAxisMoveMM = yAxisCurrentPosition;
        printNonBlocking("Y," + (String)yAxisCurrentPosition);
        //NVIC_SystemReset();  //call reset on Ardunio board
        ESP.restart(); //call reset on ESP32 board
    }
    else if (yAxisStepperMotor.distanceToGo() != 0 && yAxisSetToZeroPosition == false)
    {
        yAxisStepperMotor.runSpeedToPosition();
        if (yAxisStepperMotorLimitSwitchCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            yAxisStepperMotor.stop();
            yAxisStepperMotor.setCurrentPosition(yAxisMoveMM);
            yDirection *= DIRECTION_CCW; // change direction
            yAxisMoveMM = yDirection * 4.00;
            yAxisStepperMotor.moveTo(yAxisMoveMM);
            yAxisStepperMotor.setAcceleration(yAxisAcceleration);
            yAxisStepperMotor.setSpeed(yAxisMotorSpeed);
        }
        else if (yAxisStepperMotorLimitSwitchCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            yDirection *= DIRECTION_CW; // change direction
            yAxisMoveMM = yDirection * 4.00;
        }
        if (yAxisStepperMotorLimitSwitchCCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            yAxisStepperMotor.stop();
            yAxisStepperMotor.setCurrentPosition(yAxisMoveMM);
            yDirection *= DIRECTION_CCW; // change direction
            yAxisMoveMM = yDirection * 4.00;
            yAxisStepperMotor.moveTo(yAxisMoveMM);
            yAxisStepperMotor.setAcceleration(yAxisAcceleration);
            yAxisStepperMotor.setSpeed(yAxisMotorSpeed);
        }
        else if (yAxisStepperMotorLimitSwitchCCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            yDirection *= DIRECTION_CW; // change direction
            yAxisMoveMM = yDirection * 4.00;
        }
    }
    else if (yAxisStepperMotor.distanceToGo() == 0 && yAxisSetToZeroPosition == false)
    {
        yAxisCurrentPosition = yAxisStepperMotor.currentPosition();
        printNonBlocking("Y," + (String)yAxisCurrentPosition);
    }
}
/// <summary>
/// zs the motor run.
/// </summary>
static void zMotorRun()
{
    zAxisStepperMotor.moveTo(zAxisMoveMM);
    zAxisStepperMotor.setAcceleration(zAxisAcceleration);
    zAxisStepperMotor.setSpeed(zAxisMotorSpeed);
    if (zAxisSetToZeroPosition == true)
    {
        zAxisSetToZeroPosition = false;
        zAxisWasSetToZeroPosition = true;
        zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
        zAxisMoveMM = zAxisCurrentPosition;
        printNonBlocking("Z," + (String)zAxisCurrentPosition);
        //NVIC_SystemReset();  //call reset on Ardunio board
        ESP.restart(); //call reset on ESP32 board
    }
    else if (zAxisStepperMotor.distanceToGo() != 0 && zAxisSetToZeroPosition == false)
    {
        zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
        printNonBlocking("Z," + (String)zAxisCurrentPosition);
        zAxisStepperMotor.runSpeedToPosition();
        if (zAxisStepperMotorLimitSwitchCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            zAxisStepperMotor.stop();
            zAxisStepperMotor.setCurrentPosition(zAxisMoveMM);
            zDirection *= DIRECTION_CCW; // change direction
            zAxisMoveMM = zDirection * 4.00;
            zAxisStepperMotor.moveTo(zAxisMoveMM);
            zAxisStepperMotor.setAcceleration(zAxisAcceleration);
            zAxisStepperMotor.setSpeed(zAxisMotorSpeed);
        }
        else if (zAxisStepperMotorLimitSwitchCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            zDirection *= DIRECTION_CW; // change direction
            zAxisMoveMM = zDirection * 4.00;
        }
        if (zAxisStepperMotorLimitSwitchCCW.isPressed())
        {
            printNonBlocking("The limit switch: TOUCHED");
            zAxisStepperMotor.stop();
            zAxisStepperMotor.setCurrentPosition(zAxisMoveMM);
            zDirection *= DIRECTION_CCW; // change direction
            zAxisMoveMM = zDirection * 4.00;
            zAxisStepperMotor.moveTo(zAxisMoveMM);
            zAxisStepperMotor.setAcceleration(zAxisAcceleration);
            zAxisStepperMotor.setSpeed(zAxisMotorSpeed);
        }
        else if (zAxisStepperMotorLimitSwitchCCW.isReleased())
        {
            printNonBlocking("The limit switch: RELEASED");
            zDirection *= DIRECTION_CW; // change direction
            zAxisMoveMM = zDirection * 4.00;
        }

    }
    else if (zAxisStepperMotor.distanceToGo() == 0 && zAxisSetToZeroPosition == false)
    {
        zAxisCurrentPosition = zAxisStepperMotor.currentPosition();
        printNonBlocking("Z," + (String)zAxisCurrentPosition);
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
    if (bufferIndex > 0) {
        Serial1.write(buffer, bufferIndex);
        bufferIndex = 0;
    }
}
