#include "MyZAxis.h"
#include <Arduino.h>

ZAxis::ZAxis(float initialPosition)
    : zAxisCurrentPosition(initialPosition), 
    zAxisMoveMM(initialPosition),
    zAxisMotorSpeed(initialPosition), 
    zAxisNewPosition(initialPosition),
	zAxisAcceleration(initialPosition),
	zAxisStepperMotorMaxSpeed(initialPosition),
	zAxisLimitSwitchMoveMM(initialPosition),
	zAxisDistanceToGo(initialPosition),
	zAxisSetToZeroPosition(false),
	zAxisWasSetToZeroPosition(false),
	zAxisStepperLimitSwitchCWPressed(false),
	zAxisStepperLimitSwitchCCWPressed(false),
	zAxisStepperLimitSwitchCWReleased(false),
	zAxisStepperLimitSwitchCCWReleased(false),
	zAxisDirectionPin(4),
	zAxisPulsePin(2)
{
}

//void ZAxis::moveZAxis(float moveAmount) 
//{
//    zAxisMoveMM = moveAmount;
//    zAxisCurrentPosition += zAxisMoveMM;
//    zAxisNewPosition = zAxisCurrentPosition; // Update new position
//    Serial.print("Moved by (mm): ");
//    Serial.println(zAxisMoveMM);
//    Serial.print("Current Position (mm): ");
//    Serial.println(zAxisCurrentPosition);
//    Serial.print("New Position (mm): ");
//    Serial.println(zAxisNewPosition);
//}

