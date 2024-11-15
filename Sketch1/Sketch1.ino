/*
 Name:		Sketch1.ino
 Created:	11/11/2024 10:31:23
 Author:	sfcsarge
*/
#include <AccelStepper.h>
#include <MultiStepper.h>
/// <summary>
/// The z axis direction pin
/// </summary>
int zAxisDirectionPin = 4;
/// <summary>
/// The z axis pulse pin
/// </summary>
int zAxisPulsePin = 2;
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
/// The z axis move mm
/// </summary>
long zAxisMoveMM;  // Public variable for z-axis movement
/// <summary>
/// The z axis current step position
/// </summary>
long zAxisCurrentStepPosition;  // Public variable for current position
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
/// The z axis set to zero position
/// </summary>
bool zAxisSetToZeroPosition = false;


// the setup function runs once when you press reset or power the board
void setup() 
{
	//zAxisStepperMotor.setMaxSpeed(1000); // Set the maximum speed in steps per second
	//zAxisStepperMotor.setAcceleration(1000); // Set the acceleration in steps per second^2
	//Z axis stuff.

	serialData[7] = "0.00";
	serialData[8] = "400.00";
	serialData[9] = "0";
	zAxisNewPosition = serialData[7].toFloat();
	zAxisMotorSpeed = serialData[8].toFloat();
	zAxisSetToZeroPosition = serialData[9].toInt();
	zAxisStepperMotor.setMaxSpeed(zAxisStepperMotorMaxSpeed);
	zAxisStepperMotor.setCurrentPosition(0.00);

}

// the loop function runs over and over again until power down or reset
void loop() 
{
	if (Serial.available())
	{
		//zAxisStepperMotor.setCurrentPosition(0.00);
		Serial.println("Z Axis Position: 0.00");

		serialData[serialDataIndex] = Serial.readStringUntil(',');
		serialDataIndex++;
		if (serialDataIndex == 10)
		{
			zAxisStepperMotor.setCurrentPosition(0.00);
			serialDataIndex = 0;
			Axis = serialData[0];
			//Z axis stuff
			zAxisNewPosition = serialData[7].toFloat();
			zAxisMotorSpeed = serialData[8].toFloat();
			if (zAxisMotorSpeed > zAxisStepperMotorMaxSpeed)
			{
				zAxisMotorSpeed = zAxisStepperMotorMaxSpeed;
			}
			zAxisSetToZeroPosition = serialData[9].toFloat();
			zAxisMoveMM = static_cast<long>((zAxisNewPosition / oneFullRotationMovesMM) * stepperMotorStepsPerRev);
			//ESP.restart();  //call reset on ESP32 board

			zAxisStepperMotor.moveTo(zAxisMoveMM);
			zAxisStepperMotor.setAcceleration(1000);
			zAxisStepperMotor.setSpeed(zAxisMotorSpeed);
			Serial.println("Z Axis: " + (String)zAxisMoveMM);
			zAxisStepperMotor.runToNewPosition(zAxisMoveMM);
			//zAxisStepperMotor.runSpeedToPosition();
		}
	}
	//zAxisStepperMotor.runToNewPosition(1000); // Move 200 steps forward
	//delay(100); // Wait for one second
	//zAxisStepperMotor.runToNewPosition(0); // Move 200 steps backward
	//delay(100); // Wait for one second


}
