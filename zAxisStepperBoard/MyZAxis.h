#ifndef MyZAxis_h
#define MyZAxis_h

class ZAxis 
{
public:
    /// <summary>
    /// The z axis move mm
    /// </summary>
    float zAxisMoveMM; // Public variable for z-axis movement
    /// <summary>
    /// The z axis current position
    /// </summary>
    float zAxisCurrentPosition; // Public variable for current position
    /// <summary>
    /// The z axis motor speed
    /// </summary>
    float zAxisMotorSpeed = 400.00; // Public variable for motor speed
    /// <summary>
    /// The z axis new position
    /// </summary>
    float zAxisNewPosition; // Public variable for new position
    /// <summary>
    /// The z axis acceleration
    /// </summary>
    float zAxisAcceleration = 100.00;
    /// <summary>
    /// The z axis stepper motor maximum speed
    /// </summary>
    float zAxisStepperMotorMaxSpeed = 1000.00;
    /// <summary>
    /// The z axis move mm
    /// </summary>
    float zAxisLimitSwitchMoveMM = 0.00;
    /// <summary>
    /// The z axis set to zero position
    /// </summary>
    bool zAxisSetToZeroPosition = false;
    /// <summary>
    /// The z axis was set to zero position
    /// </summary>
    bool zAxisWasSetToZeroPosition = false;
    /// <summary>
    /// The z axis stepper limit switch cw pressed
    /// </summary>
    bool zAxisStepperLimitSwitchCWPressed = false;
    /// <summary>
    /// The z axis stepper limit switch CCW pressed
    /// </summary>
    bool zAxisStepperLimitSwitchCCWPressed = false;
    /// <summary>
    /// The z axis stepper limit switch cw released
    /// </summary>
    bool zAxisStepperLimitSwitchCWReleased = false;
    /// <summary>
    /// The z axis stepper limit switch CCW released
    /// </summary>
    bool zAxisStepperLimitSwitchCCWReleased = false;

    /// <summary>
    /// The z axis direction pin
    /// </summary>
    int zAxisDirectionPin = 4;
    /// <summary>
    /// The z axis pulse pin
    /// </summary>
    int zAxisPulsePin = 2;
    /// <summary>
    /// The z axis distance to go
    /// </summary>
    float zAxisDistanceToGo = 0.00;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZAxis"/> class.
    /// </summary>
    /// <param name="initialPosition">The initial position.</param>
    ZAxis(float initialPosition);

    //void moveZAxis(float moveAmount);
};

#endif
