#include "MyYAxis.h"
#include <Arduino.h>

YAxis::YAxis(float initialPosition)
	: yAxisCurrentPosition(initialPosition),
	yAxisMoveMM(initialPosition),
	yAxisMotorSpeed(initialPosition),
	yAxisNewPosition(initialPosition),
	yAxisAcceleration(initialPosition),
	yAxisStepperMotorMaxSpeed(initialPosition),
	yAxisLimitSwitchMoveMM(initialPosition),
	yAxisDistanceToGo(initialPosition),
	yAxisSetToZeroPosition(false),
	yAxisWasSetToZeroPosition(false),
	yAxisStepperLimitSwitchCWPressed(false),
	yAxisStepperLimitSwitchCCWPressed(false),
	yAxisStepperLimitSwitchCWReleased(false),
	yAxisStepperLimitSwitchCCWReleased(false),
	yAxisDirectionPin(4),
	yAxisPulsePin(2)
{
}

//void YAxis::moveYAxis(float moveAmount) 
//{
//    yAxisMoveMM = moveAmount;
//    yAxisCurrentPosition += yAxisMoveMM;
//    yAxisNewPosition = yAxisCurrentPosition; // Update new position
//    Serial.print("Moved by (mm): ");
//    Serial.println(yAxisMoveMM);
//    Serial.print("Current Position (mm): ");
//    Serial.println(yAxisCurrentPosition);
//    Serial.print("New Position (mm): ");
//    Serial.println(yAxisNewPosition);
//}
