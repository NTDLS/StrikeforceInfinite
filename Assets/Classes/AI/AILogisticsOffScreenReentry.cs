public override void OnMaterialized()
{
    SetAIState(new ExitScreen(this));
}

/// <summary>
/// Exit the screen at a high speed, then change state to start swooping back in.
/// </summary>
private class ExitScreen(AeAIStateMachine stateMachine)
    : IAeAIStateHandler
{
    private int _lastHullHealth = stateMachine.Owner.HullHealth;
    private AeRotationDirection _rotateDirection = AeRandom.FlipCoin() ? AeRotationDirection.Clockwise : AeRotationDirection.CounterClockwise;
    private float _rotationEpochs = 0;
    private float _rotationDegreesPerSec = 0;

    public void Tick(float epoch)
    {
        stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);

        if (stateMachine.Owner.IsWithinCurrentScaledScreenBounds == false)
        {
            stateMachine.SetAIState(new RotateToCenterScene(stateMachine));
        }
        else
        {
            if (_rotationEpochs <= 0 && stateMachine.Owner.HullHealth < _lastHullHealth)
            {
                _rotateDirection = AeRandom.FlipCoin() ? AeRotationDirection.Clockwise : AeRotationDirection.CounterClockwise;
                _rotationEpochs = AeRandom.Between(1.0f, 2.0f);
                _rotationDegreesPerSec = AeRandom.Between(20, 50);
            }
            _lastHullHealth = stateMachine.Owner.HullHealth;

            if (_rotationEpochs > 0)
            {
                _rotationEpochs -= epoch;
                if (_rotateDirection == AeRotationDirection.Clockwise)
                    stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                else
                    stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);
            }
        }
    }
}

/// <summary>
/// After exiting the screen, rotate to face the center of the screen.
/// </summary>
private class RotateToCenterScene(AeAIStateMachine stateMachine)
    : IAeAIStateHandler
{
    private readonly AeRotationDirection _rotateDirection = AeRandom.FlipCoin() ? AeRotationDirection.Clockwise : AeRotationDirection.CounterClockwise;
    private readonly float _rotationDegreesPerSec = AeRandom.Between(20, 50);

    public void Tick(float epoch)
    {
        if (stateMachine.Owner.IsPointingAt(stateMachine.Engine.Display.CenterOfCurrentScreen, 10.0f))
        {
            stateMachine.SetAIState(new ApproachTarget(stateMachine));
        }
        else
        {
            stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 1.0f, 1.0f, epoch);

            if (_rotateDirection == AeRotationDirection.Clockwise)
                stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
            else
                stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);
        }
    }
}

private class ApproachTarget(AeAIStateMachine stateMachine)
    : IAeAIStateHandler
{
    private float _lastDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);
    private int _lastHullHealth = stateMachine.Owner.HullHealth;

    public void Tick(float epoch)
    {
        var currentDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);

        if (currentDistance > _lastDistance)
        {
            stateMachine.SetAIState(new ExitScreen(stateMachine));
        }
        else
        {
            _lastDistance = currentDistance;

            if (stateMachine.Owner.HullHealth < _lastHullHealth - 5)
            {
                //If we take hits on approach, leave.
                stateMachine.SetAIState(new ExitScreen(stateMachine));
                return;
            }

            stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);
        }

        _lastHullHealth = stateMachine.Owner.HullHealth;
    }
}
