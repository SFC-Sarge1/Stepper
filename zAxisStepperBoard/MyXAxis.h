#ifndef MyXAxis_h
#define MyXAxis_h

class XAxis
{
public:
    /// <summary>
    /// The x axis motor speed
    /// </summary>
    float xAxisMotorSpeed; // Public variable for motor speed
    /// <summary>
    /// The x axis new position
    /// </summary>
    float xAxisNewPosition; // Public variable for new position
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
    /// The x axis direction pin
    /// </summary>
    int xAxisDirectionPin = 4;
    /// <summary>
    /// The x axis pulse pin
    /// </summary>
    int xAxisPulsePin = 2;
    /// <summary>
    /// The x axis distance to go
    /// </summary>
    float xAxisDistanceToGo = 0.00;

    /// <summary>
    /// Initialixes a new instance of the <see cref="XAxis"/> class.
    /// </summary>
    /// <param name="initialPosition">The initial position.</param>
    XAxis(float initialPosition);

    //void moveXAxis(float moveAmount);
};

#endif
