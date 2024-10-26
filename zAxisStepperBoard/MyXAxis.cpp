#include "MyXAxis.h"
#include <Arduino.h>

XAxis::XAxis(float initialPosition)
	: xAxisCurrentPosition(initialPosition),
	xAxisMoveMM(initialPosition),
	xAxisMotorSpeed(initialPosition),
	xAxisNewPosition(initialPosition),
	xAxisAcceleration(initialPosition),
	xAxisStepperMotorMaxSpeed(initialPosition),
	xAxisLimitSwitchMoveMM(initialPosition),
	xAxisDistanceToGo(initialPosition),
	xAxisSetToZeroPosition(false),
	xAxisWasSetToZeroPosition(false),
	xAxisStepperLimitSwitchCWPressed(false),
	xAxisStepperLimitSwitchCCWPressed(false),
	xAxisStepperLimitSwitchCWReleased(false),
	xAxisStepperLimitSwitchCCWReleased(false),
	xAxisDirectionPin(4),
	xAxisPulsePin(2)
{
}

//void XAxis::moveXAxis(float moveAmount) 
//{
//    xAxisMoveMM = moveAmount;
//    xAxisCurrentPosition += xAxisMoveMM;
//    xAxisNewPosition = xAxisCurrentPosition; // Update new position
//    Serial.print("Moved bx (mm): ");
//    Serial.println(xAxisMoveMM);
//    Serial.print("Current Position (mm): ");
//    Serial.println(xAxisCurrentPosition);
//    Serial.print("New Position (mm): ");
//    Serial.println(xAxisNewPosition);
//}

