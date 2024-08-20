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
//#include <MultiStepper.h>
//#include <AccelStepper.h>
#include <MobaTools.h>
//DEBUG=1 works, DEBUG=0 works now!
#define DEBUG 0
#define APP_VERSION "1.0.0"
#define BUILD_VERSION "001"
#define MAX_POSITION 0x7FFFFFFF  // maximum of position we can set (long type)
#define DIRECTION_CCW -1
#define DIRECTION_CW 1
#define BUFFER_SIZE 64

MoToTimer Pause;
MoToStepper XaxisStepperMotorMoto(200, STEPDIR);                   // X-Axis
MoToStepper YaxisStepperMotorMoto(200, STEPDIR);                   // Y-Axis
MoToStepper ZaxisStepperMotorMoto(200, STEPDIR);                   // Z-Axis

bool ZaxisStepperMotorLimitSwitchCCW = false;
bool ZaxisStepperMotorLimitSwitchCW = false;

/// </summary>
char buffer[BUFFER_SIZE];
/// <summary>
/// The buffer index
/// </summary>
int bufferIndex = 0;
/// <summary>
/// The limit switch Counter Clockwise pin = 11
/// </summary>
int limitSwitchCCWPin = 35;
/// <summary>
/// The limit switch Clockwise pin = 10
/// </summary>
int limitSwitchCWPin = 34;
/// <summary>
/// The X axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int XaxisDirectionPin = 4;
/// <summary>
/// The Z axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int XaxisPulsePin = 2;
/// <summary>
/// The Y axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int YaxisDirectionPin = 4;
/// <summary>
/// The Y axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int YaxisPulsePin = 2;
/// <summary>
/// The Z axis direction pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int ZaxisDirectionPin = 4;
/// <summary>
/// The Z axis pulse pin
/// </summary>
/// Stepper Motor Driver Settings
constexpr int ZaxisPulsePin = 2;
///// <summary>
///// The stepper motor interface type
///// </summary>
///// Stepper Motor Settings
//constexpr int StepperMotorInterfaceType = 1;
/// <summary>
/// The stepper motor steps per rotation = 200.00
/// </summary>
/// total steps to make 1 full rotation 360 deg based on 1.8 deg per step and switches on DM556T Stepper Motor Driver
float StepperMotorStepsPerRev = 200.00;
/// <summary>
/// The one full rotation = 4.00 millimeters
/// </summary>
/// 1 full rotation 360 deg moves the ball screw nut 4.00mm.
float OneFullRotationMovesMM = 4.00;
///// <summary>
///// The X axis stepper motor
///// </summary>
///// X axis Stepper Motor Settings (Left to Right)
//AccelStepper XaxisStepperMotorMoto = AccelStepper(StepperMotorInterfaceType, XaxisPulsePin, XaxisDirectionPin);
///// <summary>
///// The Y axis stepper motor
///// </summary>
///// Y axis Stepper Motor Settings (Front to Back)
//AccelStepper YaxisStepperMotorMoto = AccelStepper(StepperMotorInterfaceType, YaxisPulsePin, YaxisDirectionPin);
///// <summary>
///// The Z axis stepper motor
///// </summary>
///// Z axis Stepper Motor Settings (Up and Down)
//AccelStepper ZaxisStepperMotorMoto = AccelStepper(StepperMotorInterfaceType, ZaxisPulsePin, ZaxisDirectionPin);
/// <summary>
/// The X axis stepper motor maximum speed
/// </summary>
/// X axis max motor speed.
float XaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The X axis current position
/// </summary>
/// X axis current position.
float XaxisCurrentPosition = 0.00;
/// <summary>
/// The X axis new position
/// </summary>
/// X axis new position.
float XaxisNewPosition = 0.00;
/// <summary>
/// The X axis move millimeters.
/// </summary>
/// X axis how many millimeters to move ball screw nut. = (XaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev
float XaxisMoveMM = 0.00;
/// <summary>
/// The X axis distance to go
/// </summary>
/// X axis distance to go.
float XaxisDistanceToGo = 0.00;
/// <summary>
/// The Y axis distance to go
/// </summary>
/// Y axis distance to go.
float YaxisDistanceToGo = 0.00;
/// <summary>
/// The X axis motor speed
/// </summary>
/// X axis motor speed.
float XaxisMotorSpeed = 400.00;
/// <summary>
/// The X axis set to zero position
/// </summary>
/// X axis set to zero.
bool XaxisSetToZeroPosition = false;
/// <summary>
/// The X axis was set to zero position
/// </summary>
/// X axis was set to zero.
bool XaxisWasSetToZeroPosition = false;
/// <summary>
/// The Y axis stepper motor maximum speed
/// </summary>
/// Y axis max motor speed.
float YaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The Y axis current position
/// </summary>
/// Y axis current position.
float YaxisCurrentPosition = 0.00;
/// <summary>
/// The Y axis new position
/// </summary>
/// Y axis new position.
float YaxisNewPosition = 0.00;
/// <summary>
/// The Y axis move millimeters.
/// </summary>
/// Y axis how many millimeters to move ball screw nut. = (YaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev
float YaxisMoveMM = 0.00;
/// <summary>
/// The Y axis motor speed
/// </summary>
/// Y axis moter speed.
float YaxisMotorSpeed = 400.00;
/// <summary>
/// The Y axis set to zero position
/// </summary>
/// Y axis set to zero.
bool YaxisSetToZeroPosition = false;
/// <summary>
/// The Y axis was set to zero position
/// </summary>
/// Y axis was set to zero.
bool YaxisWasSetToZeroPosition = false;
/// <summary>
/// The Z axis stepper motor maximum speed
/// </summary>
/// Z axis max motor speed.
float ZaxisStepperMotorMaxSpeed = 1000.00;
/// <summary>
/// The Z axis current position
/// </summary>
/// Z axis current position.
float ZaxisCurrentPosition = 0.00;
/// <summary>
/// The Z axis new position
/// </summary>
/// Z axis new position.
float ZaxisNewPosition = 0.00;
/// <summary>
/// The Z axis move millimeters.
/// </summary>
/// Z axis how many millimeters to move ball screw nut. = (ZaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev
float ZaxisMoveMM = 0.00;
/// <summary>
/// The Z axis motor speed
/// </summary>
/// Z axis motor speed.
float ZaxisMotorSpeed = 400.00;
/// <summary>
/// The Z axis set to zero position
/// </summary>
/// Z axis set to zero.
bool ZaxisSetToZeroPosition = false;
/// <summary>
/// The Z axis was set to zero position
/// </summary>
bool ZaxisWasSetToZeroPosition = false;
/// <summary>
/// The default axis = X.
/// </summary>
/// Axis to Move.
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
 /// Setup this instance.
 /// </summary>
 /// The setup function runs once when you press reset or power the board
 void setup()
{
    Serial.begin(9600);
    printNonBlocking("Application Version: " + String(APP_VERSION));
    printNonBlocking("Build Version: " + String(BUILD_VERSION));

    //Common stuff.

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
    XaxisStepperMotorMoto.setMaxSpeed(XaxisStepperMotorMaxSpeed);
    //XaxisStepperMotorMoto.setCurrentPosition(0.00);

    //Y axis stuff.

    SerialData[4] = "0.00";
    SerialData[5] = "400.00";
    SerialData[6] = "0";
    YaxisCurrentPosition = 0.00;
    YaxisNewPosition = SerialData[4].toFloat();
    YaxisMotorSpeed = SerialData[5].toFloat();
    YaxisSetToZeroPosition = SerialData[6].toInt();
    YaxisStepperMotorMoto.setMaxSpeed(YaxisStepperMotorMaxSpeed);
    //YaxisStepperMotorMoto.setCurrentPosition(0.00);

    //Z axis stuff.

    SerialData[7] = "0.00";
    SerialData[8] = "400.00";
    SerialData[9] = "0";
    ZaxisCurrentPosition = 0.00;
    ZaxisNewPosition = SerialData[7].toFloat();
    ZaxisMotorSpeed = SerialData[8].toFloat();
    ZaxisSetToZeroPosition = SerialData[9].toInt();
    ZaxisStepperMotorMoto.setMaxSpeed(ZaxisStepperMotorMaxSpeed);
    //ZaxisStepperMotorMoto.setCurrentPosition(0.00);
    SerialDataIndex = 0;
    pinMode(limitSwitchCCWPin, INPUT_PULLUP);
    pinMode(limitSwitchCWPin, INPUT_PULLUP);
    // attach(STEPpin, DIRpin)
    ZaxisStepperMotorMoto.attach(ZaxisPulsePin, ZaxisDirectionPin);            
    ZaxisStepperMotorMoto.setZero();

}

/// <summary>
/// Loops this instance.
/// </summary>
/// The loop function runs over and over again until power down or reset
void loop()
{
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
 /// X Axis Motor Configuration
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
/// Y Axis Motor Configuration
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
static void ZMotorConfig(float data7, float data8, float data9) 
{
    //Z axis stuff
    ZaxisNewPosition = data7;
    ZaxisMotorSpeed = data8;
    if (ZaxisMotorSpeed > ZaxisStepperMotorMaxSpeed) {
        ZaxisMotorSpeed = ZaxisStepperMotorMaxSpeed;
    }
    ZaxisSetToZeroPosition = data9;
    ZaxisMoveMM = (ZaxisNewPosition / OneFullRotationMovesMM) * StepperMotorStepsPerRev;
}
/// <summary>
/// X axis motor run function.
/// </summary>
/// Run the X axis motor to position.
static void XMotorRun() 
{
    if (XaxisSetToZeroPosition == true) 
    {
        XaxisSetToZeroPosition = false;
        XaxisWasSetToZeroPosition = true;
        XaxisStepperMotorMoto.setZero(0);
        XaxisStepperMotorMoto.setSpeed(XaxisMotorSpeed);
        XaxisStepperMotorMoto.setSpeedSteps(XaxisMotorSpeed);
        XaxisCurrentPosition = XaxisStepperMotorMoto.currentPosition();
        XaxisMoveMM = XaxisCurrentPosition;
        printNonBlocking("X," + (String)XaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (XaxisStepperMotorMoto.distanceToGo() != 0 && XaxisSetToZeroPosition == false) 
    {
        XaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        XaxisStepperMotorMoto.setSpeedSteps(ZaxisMotorSpeed);
        XaxisStepperMotorMoto.setRampLen(ZaxisMotorSpeed);
        XaxisStepperMotorMoto.writeSteps(ZaxisMoveMM);
    }
    else if (XaxisStepperMotorMoto.distanceToGo() == 0 && XaxisSetToZeroPosition == false) 
    {
        XaxisStepperMotorMoto.setZero(0);
        XaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        XaxisCurrentPosition = XaxisStepperMotorMoto.currentPosition();
        printNonBlocking("X," + (String)XaxisCurrentPosition);
    }
}
/// <summary>
/// Y axis motor run function.
/// </summary>
/// Run the Y axis motor to position.
static void YMotorRun() 
{
    if (YaxisSetToZeroPosition == true) 
    {
        YaxisSetToZeroPosition = false;
        YaxisWasSetToZeroPosition = true;
        YaxisStepperMotorMoto.setZero(0);
        YaxisStepperMotorMoto.setSpeed(YaxisMotorSpeed);
        YaxisStepperMotorMoto.setSpeedSteps(YaxisMotorSpeed);
        YaxisCurrentPosition = YaxisStepperMotorMoto.currentPosition();
        YaxisMoveMM = YaxisCurrentPosition;
        printNonBlocking("Y," + (String)YaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (YaxisStepperMotorMoto.distanceToGo() != 0 && YaxisSetToZeroPosition == false) 
    {
        YaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        YaxisStepperMotorMoto.setSpeedSteps(ZaxisMotorSpeed);
        YaxisStepperMotorMoto.setRampLen(ZaxisMotorSpeed);
        YaxisStepperMotorMoto.writeSteps(ZaxisMoveMM);
    }
    else if (YaxisStepperMotorMoto.distanceToGo() == 0 && YaxisSetToZeroPosition == false) 
    {
        YaxisStepperMotorMoto.setZero(0);
        YaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        YaxisCurrentPosition = YaxisStepperMotorMoto.currentPosition();
        printNonBlocking("Y," + (String)YaxisCurrentPosition);
    }
}
/// <summary>
/// Z axis motor run function.
/// </summary>
/// Run the Z axis motor to position.
static void ZMotorRun()
{
    if (ZaxisSetToZeroPosition == true)
    {
        ZaxisSetToZeroPosition = false;
        ZaxisWasSetToZeroPosition = true;
        ZaxisStepperMotorMoto.setZero(0);
        ZaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        ZaxisStepperMotorMoto.setSpeedSteps(ZaxisMotorSpeed);
        ZaxisCurrentPosition = ZaxisStepperMotorMoto.currentPosition();
        ZaxisMoveMM = ZaxisCurrentPosition;
        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
        //NVIC_SystemReset();  //call reset
        ESP.restart();
    }
    else if (ZaxisStepperMotorMoto.distanceToGo() != 0 && ZaxisSetToZeroPosition == false)
    {
        if (digitalRead(limitSwitchCCWPin) == HIGH && digitalRead(limitSwitchCWPin) == HIGH)
        {
            ZaxisStepperMotorMoto.setZero(0);
            ZaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
            ZaxisStepperMotorMoto.setSpeedSteps(ZaxisMotorSpeed);
            ZaxisStepperMotorMoto.setRampLen(ZaxisMotorSpeed);
            ZaxisStepperMotorMoto.writeSteps(ZaxisMoveMM);
            printNonBlocking("Z, Running Speed To Position.");
            ZaxisStepperMotorMoto.setZero(0);
            ZaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
            ZaxisStepperMotorMoto.setSpeedSteps(ZaxisMotorSpeed);
            Pause.setTime(1000);
        }
    }
    else if (ZaxisStepperMotorMoto.distanceToGo() == 0 && ZaxisSetToZeroPosition == false)
    {
        ZaxisStepperMotorMoto.setZero(0);
        ZaxisStepperMotorMoto.setSpeed(ZaxisMotorSpeed);
        ZaxisCurrentPosition = ZaxisStepperMotorMoto.currentPosition();
        printNonBlocking("Z," + (String)ZaxisCurrentPosition);
    }
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