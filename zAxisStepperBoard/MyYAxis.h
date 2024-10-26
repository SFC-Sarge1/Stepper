#ifndef MyYAxis_h
#define MyYAxis_h

class YAxis
{
public:
    /// <summary>
    /// The y axis move mm
    /// </summary>
    float yAxisMoveMM; // Public variable for y-axis movement
    /// <summary>
    /// The y axis current position
    /// </summary>
    float yAxisCurrentPosition; // Public variable for current position
    /// <summary>
    /// The y axis motor speed
    /// </summary>
    float yAxisMotorSpeed; // Public variable for motor speed
    /// <summary>
    /// The y axis new position
    /// </summary>
    float yAxisNewPosition; // Public variable for new position
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
    /// The y axis direction pin
    /// </summary>
    int yAxisDirectionPin = 4;
    /// <summary>
    /// The y axis pulse pin
    /// </summary>
    int yAxisPulsePin = 2;
    /// <summary>
    /// The y axis distance to go
    /// </summary>
    float yAxisDistanceToGo = 0.00;

    /// <summary>
    /// Initializes a new instance of the <see cref="YAxis"/> class.
    /// </summary>
    /// <param name="initialPosition">The initial position.</param>
    YAxis(float initialPosition);

    //void moveYAxis(float moveAmount);
};

#endif
