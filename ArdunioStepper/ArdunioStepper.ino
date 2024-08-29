// ***********************************************************************
// Assembly         : 
// Author           : sfcsarge
// Created          : 03-26-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 08-10-2024
// ***********************************************************************
// <copyright file="ArdunioStepper.ino" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#include <MultiStepper.h>
#include <AccelStepper.h>
#include <ezButton.h>

//DEBUG=1 works, DEBUG=0 works now!
#define DEBUG 0
#define APP_VERSION "1.0.0"
#define BUILD_VERSION "001"
#define DIRECTION_CCW -1
#define DIRECTION_CW 1
#define BUFFER_SIZE 64
#define MAX_POSITION 0x7FFFFFFF  // maximum of position we can set (long type)
#define XSTATE_CHANGE_DIR   1
#define XSTATE_MOVE         2
#define XSTATE_MOVING       3
#define XSTATE_ZERO         4
#define YSTATE_CHANGE_DIR   1
#define YSTATE_MOVE         2
#define YSTATE_MOVING       3
#define YSTATE_ZERO         4
#define ZSTATE_CHANGE_DIR   1
#define ZSTATE_MOVE         2
#define ZSTATE_MOVING       3
#define ZSTATE_ZERO         4


ezButton ZaxisStepperMotorLimitSwitchCW(12);
ezButton ZaxisStepperMotorLimitSwitchCCW(14);

int XaxisStepperMotorState = XSTATE_MOVE;
int YaxisStepperMotorState = YSTATE_MOVE;
int ZaxisStepperMotorState = ZSTATE_MOVE;
//
int Xdirection = DIRECTION_CW;
int Ydirection = DIRECTION_CW;
int Zdirection = DIRECTION_CW;

//long XtargetPos = 0;
//long YtargetPos = 0;
//long ZtargetPos = 0;

/// </summary>
char buffer[BUFFER_SIZE];
/// <summary>
/// The buffer index
/// </summary>
int bufferIndex = 0;
/// <summary>
/// The X axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The xaxis direction pin
/// </summary>
constexpr int XaxisDirectionPin = 4;
/// <summary>
/// The Z axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The xaxis pulse pin
/// </summary>
constexpr int XaxisPulsePin = 2;
/// <summary>
/// The Y axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The yaxis direction pin
/// </summary>
constexpr int YaxisDirectionPin = 4;
/// <summary>
/// The Y axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The yaxis pulse pin
/// </summary>
constexpr int YaxisPulsePin = 2;
/// <summary>
/// The Z axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The zaxis direction pin
/// </summary>
constexpr int ZaxisDirectionPin = 4;
/// <summary>
/// The Z axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
/// <summary>
/// The zaxis pulse pin
/// </summary>
constexpr int ZaxisPulsePin = 2;
/// <summary>
/// The stepper motor interface type
/// </summary>
/// Stepper Motor Settings
/// <summary>
/// The stepper motor interface type
/// </summary>
constexpr int StepperMotorInterfaceType = 1;
/// <summary>
/// The stepper motor steps per rotation = 200.00
/// </summary>
/// total steps to make 1 full rotation 360 deg based on 1.8 deg per step and switches on DM556T Stepper Motor Driver
/// <summary>
/// The stepper motor steps per rev
/// </summary>
float StepperMotorStepsPerRev = 200.00;
/// <summary>
/// The one full rotation = 4.00 millimeters
/// </summary>
/// 1 full rotation 360 deg moves the ball screw nut 4.00mm.
/// <summary>
/// The one full rotation moves mm
/// </summary>
float OneFullRotationMovesMM = 4.00;
/// <summary>
/// The X axis stepper motor
/// </summary>
/// X axis Stepper Motor Settings (Left to Right)
/// <summary>
/// The xaxis stepper motor
/// </summary>
AccelStepper XaxisStepperMotor = AccelStepper(StepperMotorInterfaceType, XaxisPulsePin, XaxisDirectionPin);
/// <summary>
/// The Y axis stepper motor
/// </summary>
/// Y axis Stepper Motor Settings (Front to Back)
/// <summary>
/// The yaxis stepper motor
/// </summary>
AccelStepper YaxisStepperMotor = AccelStepper(StepperMotorInterfaceType, YaxisPulsePin, YaxisDirectionPin);
/// <summary>
/// The Z axis stepper motor
/// </summary>
/// Z axis Stepper Motor Settings (Up and Down)
/// <summary>
/// The zaxis stepper motor
/// </summary>
AccelStepper ZaxisStepperMotor = AccelStepper(StepperMotorInterfaceType, ZaxisPulsePin, ZaxisDirectionPin);
/// <summary>
/// The xaxis acceleration
/// </summary>
float XaxisAcceleration = 50.00;
/// <summary>
/// The X axis stepper motor maximum speed
/// </summary>
/// X axis max motor speed.
/// <summary>
/// The xaxis stepper motor maximum speed
/// </summary>
float XaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The X axis current position
/// </summary>
/// X axis current position.
/// <summary>
/// The xaxis current position
/// </summary>
float XaxisCurrentPosition = 0.00;
/// <summary>
/// The X axis new position
/// </summary>
/// X axis new position.
/// <summary>
/// The xaxis new position
/// </summary>
float XaxisNewPosition = 0.00;
/// <summary>
/// The X axis move millimeters.
/// </summary>
/// X axis how many millimeters to move ball screw nut. = (XaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev
/// <summary>
/// The xaxis move mm
/// </summary>
float XaxisMoveMM = 0.00;
/// <summary>
/// The X axis distance to go
/// </summary>
/// X axis distance to go.
/// <summary>
/// The xaxis distance to go
/// </summary>
float XaxisDistanceToGo = 0.00;
/// <summary>
/// The Y axis distance to go
/// </summary>
/// Y axis distance to go.
/// <summary>
/// The yaxis distance to go
/// </summary>
float YaxisDistanceToGo = 0.00;
/// <summary>
/// The X axis motor speed
/// </summary>
/// X axis motor speed.
/// <summary>
/// The xaxis motor speed
/// </summary>
float XaxisMotorSpeed = 400.00;
/// <summary>
/// The X axis set to zero position
/// </summary>
/// X axis set to zero.
/// <summary>
/// The xaxis set to zero position
/// </summary>
bool XaxisSetToZeroPosition = false;
/// <summary>
/// The X axis was set to zero position
/// </summary>
/// X axis was set to zero.
/// <summary>
/// The xaxis was set to zero position
/// </summary>
bool XaxisWasSetToZeroPosition = false;
/// <summary>
/// The yaxis acceleration
/// </summary>
float YaxisAcceleration = 50.00;
/// <summary>
/// The Y axis stepper motor maximum speed
/// </summary>
/// Y axis max motor speed.
/// <summary>
/// The yaxis stepper motor maximum speed
/// </summary>
float YaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The Y axis current position
/// </summary>
/// Y axis current position.
/// <summary>
/// The yaxis current position
/// </summary>
float YaxisCurrentPosition = 0.00;
/// <summary>
/// The Y axis new position
/// </summary>
/// Y axis new position.
/// <summary>
/// The yaxis new position
/// </summary>
float YaxisNewPosition = 0.00;
/// <summary>
/// The Y axis move millimeters.
/// </summary>
/// Y axis how many millimeters to move ball screw nut. = (YaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev
/// <summary>
/// The yaxis move mm
/// </summary>
float YaxisMoveMM = 0.00;
/// <summary>
/// The Y axis motor speed
/// </summary>
/// Y axis moter speed.
/// <summary>
/// The yaxis motor speed
/// </summary>
float YaxisMotorSpeed = 400.00;
/// <summary>
/// The Y axis set to zero position
/// </summary>
/// Y axis set to zero.
/// <summary>
/// The yaxis set to zero position
/// </summary>
bool YaxisSetToZeroPosition = false;
/// <summary>
/// The Y axis was set to zero position
/// </summary>
bool YaxisWasSetToZeroPosition = false;
/// <summary>
/// The zaxis acceleration
/// </summary>
float ZaxisAcceleration = 50.00;
/// <summary>
/// The Z axis stepper motor maximum speed
/// </summary>
float ZaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The Z axis current position
/// </summary>
float ZaxisCurrentPosition = 0.00;
/// <summary>
/// The Z axis new position
/// </summary>
float ZaxisNewPosition = 0.00;
/// <summary>
/// The Z axis move millimeters.
/// </summary>
float ZaxisMoveMM = 0.00;
/// <summary>
/// The Z axis motor speed
/// </summary>
float ZaxisMotorSpeed = 400.00;
/// <summary>
/// The Z axis set to zero position
/// </summary>
bool ZaxisSetToZeroPosition = false;
/// <summary>
/// The Z axis was set to zero position
/// </summary>
bool ZaxisWasSetToZeroPosition = false;
/// <summary>
/// The default axis = X.
/// </summary>
String Axis = "X";
/// <summary>
/// The serial data
/// </summary>
/// The first three comma separated values are for X axis, the last three comma separated values are for Y axis.
/// SerialData[0] What Axis to move. Support "X", "Y", "Z", or "XY"
/// SerialData[1] is the X Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[2] is the X Axis Speed.
/// SerialData[3] is X Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for X Axis current position to zero.
/// SerialData[4] is the Y Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[5] is the Y Axis Speed.
/// SerialData[6] is Y Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Y Axis current position to zero.
/// SerialData[7] is the Z Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[8] is the Z Axis Speed.
/// SerialData[9] is Z Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Z Axis current position to zero.
String SerialData[] = { "XY", "0.00", "400.00", "0", "0.00", "400.00", "0", "0.00", "400.00", "0" };
/// <summary>
/// The serial data index
/// </summary>
/// The first three comma separated values are for X axis, the last three comma separated values are for Y axis.
/// SerialData[0] What Axis to move. Support "X", "Y", "Z", or "XY"
/// SerialData[1] is the X Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[2] is the X Axis Speed.
/// SerialData[3] is X Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for X Axis current position to zero.
/// SerialData[4] is the Y Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[5] is the Y Axis Speed.
/// SerialData[6] is Y Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Y Axis current position to zero.
/// SerialData[7] is the Z Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[8] is the Z Axis Speed.
/// SerialData[9] is Z Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Z Axis current position to zero.
/// The first three comma separated values are for X axis, the last three comma separated values are for Y axis.
/// SerialData[0] What Axis to move. Support "X", "Y", "Z", or "XY"
/// SerialData[1] is the X Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[2] is the X Axis Speed.
/// SerialData[3] is X Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for X Axis current position to zero.
/// SerialData[4] is the Y Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[5] is the Y Axis Speed.
/// SerialData[6] is Y Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Y Axis current position to zero.
/// SerialData[7] is the Z Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[8] is the Z Axis Speed.
/// SerialData[9] is Z Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Z Axis current position to zero.
/// The first three comma separated values are for X axis, the last three comma separated values are for Y axis.
/// SerialData[0] What Axis to move. Support "X", "Y", "Z", or "XY"
/// SerialData[1] is the X Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[2] is the X Axis Speed.
/// SerialData[3] is X Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for X Axis current position to zero.
/// SerialData[4] is the Y Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[5] is the Y Axis Speed.
/// SerialData[6] is Y Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Y Axis current position to zero.
/// SerialData[7] is the Z Axis millimeters to move to. Ball Screw 1204 has a 4mm movement per 1 full rotation 360 deg of the screw.
/// SerialData[8] is the Z Axis Speed.
/// SerialData[9] is Z Axis set to zero, False (0) or True (1) if you want to call setCurrentPosition(0.00) to set stepper for Z Axis current position to zero.
int SerialDataIndex = 0;
 /// <summary>
 /// The setup function runs once when you press reset or power the board.
 /// </summary>
void setup()
{
    Serial.begin(9600);
    printNonBlocking("Application Version: " + String(APP_VERSION));
    printNonBlocking("Build Version: " + String(BUILD_VERSION));

    //Common stuff.
    //ZaxisStepperMotorLimitSwitchCCW.setDebounceTime(50); // set debounce time to 50 milliseconds
    //ZaxisStepperMotorLimitSwitchCW.setDebounceTime(50); // set debounce time to 50 milliseconds
    SerialData[0] = "XY";
    Axis = SerialData[0];

    //X axis stuff.

    SerialData[1] = "0.00";
    SerialData[2] = "400.00";
    SerialData[3] = "0";
    XaxisCurrentPosition = 0.00;
    XaxisNewPosition = SerialData[1].toFloat();
    XaxisMotorSpeed = SerialData[2].toFloat();
    XaxisSetToZeroPosition = SerialData[3].toInt();
    XaxisStepperMotor.setMaxSpeed(XaxisStepperMotorMaxSpeed);
    XaxisStepperMotor.setCurrentPosition(0.00);
    
    //Y axis stuff.

    SerialData[4] = "0.00";
    SerialData[5] = "400.00";
    SerialData[6] = "0";
    YaxisCurrentPosition = 0.00;
    YaxisNewPosition = SerialData[4].toFloat();
    YaxisMotorSpeed = SerialData[5].toFloat();
    YaxisSetToZeroPosition = SerialData[6].toInt();
    YaxisStepperMotor.setMaxSpeed(YaxisStepperMotorMaxSpeed);
    YaxisStepperMotor.setCurrentPosition(0.00);

    //Z axis stuff.

    SerialData[7] = "0.00";
    SerialData[8] = "400.00";
    SerialData[9] = "0";
    ZaxisCurrentPosition = 0.00;
    ZaxisNewPosition = SerialData[7].toFloat();
    ZaxisMotorSpeed = SerialData[8].toFloat();
    ZaxisSetToZeroPosition = SerialData[9].toInt();
    ZaxisStepperMotor.setMaxSpeed(ZaxisStepperMotorMaxSpeed);
    ZaxisStepperMotor.setCurrentPosition(0.00);

    SerialDataIndex = 0;
 }

/// <summary>
/// The loop function runs over and over again until power down or reset.
/// </summary>
 void loop()
{
    //ZaxisStepperMotorLimitSwitchCW.loop();
    //ZaxisStepperMotorLimitSwitchCCW.loop();
    //if (ZaxisStepperMotorLimitSwitchCW.isPressed())
    //{
    //    printNonBlocking("The limit switch: TOUCHED");
    ////    Zdirection *= -1; // change direction
    ////    ZaxisMoveMM = Zdirection * 4.00;
    ////    ZaxisStepperMotor.setCurrentPosition(0); // set position
    ////    ZaxisStepperMotor.setAcceleration(ZaxisAcceleration); // set acceleration
    ////    ZaxisStepperMotor.moveTo(ZaxisMoveMM);
    ////    ZaxisStepperMotor.setSpeed(ZaxisMotorSpeed);
    //    ZaxisStepperMotor.stop();

    //}
    //if (ZaxisStepperMotorLimitSwitchCCW.isPressed()) 
    //{
    //    printNonBlocking("The limit switch: TOUCHED");
    ////    Zdirection *= -1; // change direction
    ////    ZaxisMoveMM = Zdirection * 4.00;
    ////    ZaxisStepperMotorState = ZSTATE_MOVE;
    ////    ZaxisStepperMotor.setCurrentPosition(0); // set position
    ////    ZaxisStepperMotor.moveTo(ZaxisMoveMM);
    ////    ZaxisStepperMotor.setAcceleration(ZaxisAcceleration); // set acceleration
    ////    ZaxisStepperMotor.setSpeed(ZaxisMotorSpeed);
    //    ZaxisStepperMotor.stop();
    //}

    if (Serial.available()) 
    {
        SerialData[SerialDataIndex] = Serial.readStringUntil(',');
        SerialDataIndex++;
        if (SerialDataIndex == 10) 
        {
            SerialDataIndex = 0;
            Axis = SerialData[0];
            if (Axis == "X") 
            {
                //X axis stuff
                XMotorConfig(SerialData[1].toFloat(), SerialData[2].toFloat(), SerialData[3].toFloat());
            }
            else if (Axis == "Y") 
            {
                //Y axis stuff
                YMotorConfig(SerialData[4].toFloat(), SerialData[5].toFloat(), SerialData[6].toFloat());
            }
            else if (Axis == "Z") 
            {
                //Z axis stuff
                ZMotorConfig(SerialData[7].toFloat(), SerialData[8].toFloat(), SerialData[9].toFloat());
            }
            else if (Axis == "XY") 
            {
                //X axis stuff
                XMotorConfig(SerialData[1].toFloat(), SerialData[2].toFloat(), SerialData[3].toFloat());
                //Y axis stuff
                YMotorConfig(SerialData[4].toFloat(), SerialData[5].toFloat(), SerialData[6].toFloat());
            }
        }
    }
    if (Axis == "X") 
    {
        XMotorRun();
    }
    else if (Axis == "Y") 
    {
        YMotorRun();
    }
    else if (Axis == "Z")
    {
        ZMotorRun();
    }
    else if (Axis == "XY") 
    {
        XMotorRun();
        YMotorRun();
    }
}

 /// <summary>
 /// X axis motor configuration.
 /// </summary>
 /// <param name="data1">The data1.</param>
 /// <param name="data2">The data2.</param>
 /// <param name="data3">The data3.</param>
/// <summary>
static void XMotorConfig(float data1, float data2, float data3)
 {
    //X axis stuff
    XaxisNewPosition = data1;
    XaxisMotorSpeed = data2;
    if (XaxisMotorSpeed > XaxisStepperMotorMaxSpeed) 
    {
        XaxisMotorSpeed = XaxisStepperMotorMaxSpeed;
    }
    XaxisSetToZeroPosition = data3;
    XaxisMoveMM = (XaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev;
}
/// <summary>
/// Y axis motor configuration.
/// </summary>
/// <param name="data4">The data4.</param>
/// <param name="data5">The data5.</param>
/// <param name="data6">The data6.</param>
/// <summary>
 static void YMotorConfig(float data4, float data5, float data6)
{
    //Y axis stuff
    YaxisNewPosition = data4;
    YaxisMotorSpeed = data5;
    if (YaxisMotorSpeed > YaxisStepperMotorMaxSpeed) 
    {
        YaxisMotorSpeed = YaxisStepperMotorMaxSpeed;
    }
    YaxisSetToZeroPosition = data6;
    YaxisMoveMM = (YaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev;
}
/// <summary>
/// Z axis motor configuration.
/// </summary>
/// <param name="data7">The data7.</param>
/// <param name="data8">The data8.</param>
/// <param name="data9">The data9.</param>
/// Z Axis Motor Configuration
/// <summary>
/// zs the motor configuration.
/// </summary>
/// <param name="data7">The data7.</param>
/// <param name="data8">The data8.</param>
/// <param name="data9">The data9.</param>
static void ZMotorConfig(float data7, float data8, float data9)
{
    //Z axis stuff
    ZaxisNewPosition = data7;
    ZaxisMotorSpeed = data8;
    if (ZaxisMotorSpeed > ZaxisStepperMotorMaxSpeed) 
    {
        ZaxisMotorSpeed = ZaxisStepperMotorMaxSpeed;
    }
    ZaxisSetToZeroPosition = data9;
    ZaxisMoveMM = (ZaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev;
}
/// <summary>
/// X axis motor run function.
/// </summary>
/// Run the X axis motor to position.
/// <summary>
/// xes the motor run.
/// </summary>
static void XMotorRun()
{
	XaxisStepperMotor.moveTo(XaxisMoveMM);
	XaxisStepperMotor.setSpeed(XaxisMotorSpeed);
	XaxisStepperMotor.setAcceleration(XaxisAcceleration);
    if (XaxisSetToZeroPosition == true) 
    {
        XaxisSetToZeroPosition = false;
        XaxisWasSetToZeroPosition = true;
        XaxisCurrentPosition = XaxisStepperMotor.currentPosition();
        XaxisMoveMM = XaxisCurrentPosition;
        printNonBlocking("X," + (String)XaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (XaxisStepperMotor.distanceToGo() != 0 && XaxisSetToZeroPosition == false) 
    {
        XaxisStepperMotor.runSpeedToPosition();
    }
    else if (XaxisStepperMotor.distanceToGo() == 0 && XaxisSetToZeroPosition == false) 
    {
        XaxisCurrentPosition = XaxisStepperMotor.currentPosition();
        printNonBlocking("X," + (String)XaxisCurrentPosition);
    }
}
/// <summary>
/// Y axis motor run function.
/// </summary>
/// Run the Y axis motor to position.
/// <summary>
/// ies the motor run.
/// </summary>
static void YMotorRun()
{
    YaxisStepperMotor.moveTo(YaxisMoveMM);
    YaxisStepperMotor.setSpeed(YaxisMotorSpeed);
    YaxisStepperMotor.setAcceleration(YaxisAcceleration);
    if (YaxisSetToZeroPosition == true)
    {
        YaxisSetToZeroPosition = false;
        YaxisWasSetToZeroPosition = true;
        YaxisCurrentPosition = YaxisStepperMotor.currentPosition();
        YaxisMoveMM = YaxisCurrentPosition;
        printNonBlocking("Y," + (String)YaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (YaxisStepperMotor.distanceToGo() != 0 && YaxisSetToZeroPosition == false) 
    {
        YaxisStepperMotor.runSpeedToPosition();
    }
    else if (YaxisStepperMotor.distanceToGo() == 0 && YaxisSetToZeroPosition == false) 
    {
        YaxisCurrentPosition = YaxisStepperMotor.currentPosition();
        printNonBlocking("Y," + (String)YaxisCurrentPosition);
    }
}
/// <summary>
/// Z axis motor run function.
/// </summary>
/// Run the Z axis motor to position.
/// <summary>
/// zs the motor run.
/// </summary>
static void ZMotorRun()
{
    ZaxisMoveMM = Zdirection * ZaxisMoveMM;
    ZaxisStepperMotor.moveTo(ZaxisMoveMM);
    ZaxisStepperMotor.setAcceleration(ZaxisAcceleration);
    ZaxisStepperMotor.setSpeed(ZaxisMotorSpeed);
    if (ZaxisSetToZeroPosition == true)
    {
        ZaxisSetToZeroPosition = false;
        ZaxisWasSetToZeroPosition = true;
        ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
        ZaxisMoveMM = ZaxisCurrentPosition;
        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (ZaxisStepperMotor.distanceToGo() != 0 && ZaxisSetToZeroPosition == false)
    {
        ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
        ZaxisStepperMotor.runSpeedToPosition();
    }
    else if (ZaxisStepperMotor.distanceToGo() == 0 && ZaxisSetToZeroPosition == false)
    {
        ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
    }

    //switch (ZaxisStepperMotorState)
    //{
    //    case YSTATE_ZERO:
    //        ZaxisSetToZeroPosition = false;
    //        ZaxisWasSetToZeroPosition = true;
    //        ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
    //        ZaxisMoveMM = ZaxisCurrentPosition;
    //        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
    //        //NVIC_SystemReset();  //call reset
    //        ESP.restart();
    //        break;
    //    //case ZSTATE_CHANGE_DIR:
    //    //    if (ZaxisStepperMotorLimitSwitchCCW.isPressed())
    //    //    {
    //    //        ZaxisStepperMotorState = ZSTATE_CHANGE_DIR;
    //    //        printNonBlocking("The limit switch 1: TOUCHED");
    //    //        Zdirection = DIRECTION_CW;
    //    //        printNonBlocking("CLOCKWISE");
    //    //        ZaxisMoveMM = 4.00;
    //    //    }
    //    //    if (ZaxisStepperMotorLimitSwitchCW.isPressed())
    //    //    {
    //    //        ZaxisStepperMotorState = ZSTATE_CHANGE_DIR;
    //    //        printNonBlocking("The limit switch 2: TOUCHED");
    //    //        Zdirection = DIRECTION_CCW;
    //    //        printNonBlocking("COUNTERCLOCKWISE");
    //    //        ZaxisMoveMM = 4.00;
    //    //    }

    //    //    ZaxisStepperMotorState = ZSTATE_MOVE; // after changing direction, go to the next state to move the motor
    //    //    break;
    //    case ZSTATE_MOVE:
    //        ZaxisMoveMM = Zdirection * ZaxisMoveMM;
    //        ZaxisStepperMotor.setCurrentPosition(0); // set position
    //        ZaxisStepperMotor.moveTo(ZaxisMoveMM);
    //        ZaxisStepperMotor.setAcceleration(ZaxisAcceleration); // set acceleration
    //        ZaxisStepperMotor.setSpeed(ZaxisMotorSpeed);
    //        ZaxisStepperMotorState = ZSTATE_MOVING; // after moving, go to the next state to keep the motor moving infinity
    //        break;
    //    case ZSTATE_MOVING: // without this state, the move will stop after reaching maximum position
    //        if (ZaxisStepperMotor.distanceToGo() == 0 && ZaxisSetToZeroPosition == false)
    //        {
    //            ZaxisStepperMotor.setCurrentPosition(0);   // reset position to 0
    //            ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
    //            printNonBlocking("Z," + (String)ZaxisCurrentPosition);
    //            ZaxisStepperMotor.stop();
    //            break;
    //        }
    //        else if (ZaxisStepperMotor.distanceToGo() != 0 && ZaxisSetToZeroPosition == false)
    //        {
    //            ZaxisStepperMotorState = ZSTATE_MOVING; // after moving, go to the next state to keep the motor moving infinity
    //            ZaxisStepperMotor.moveTo(ZaxisMoveMM);
    //            ZaxisStepperMotor.setAcceleration(ZaxisAcceleration); // set acceleration
    //            ZaxisStepperMotor.setSpeed(ZaxisMotorSpeed);
    //            ZaxisStepperMotor.run(); // MUST be called in loop() function
    //            ZaxisCurrentPosition = ZaxisStepperMotor.currentPosition();
    //            printNonBlocking("Z," + (String)ZaxisCurrentPosition);
    //        }
    //        break;
    //}
}
/// <summary>
/// Prints the serial message non-blocking.
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
/// Prints the serial message non-blocking.
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