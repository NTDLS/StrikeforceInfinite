/// <summary>
/// Keeps an object swooping past an object at an indirect angle.
/// </summary>
private readonly string _boostResourceName = "AILogisticsHostileEngagement_Boost";

public override void OnMaterialized()
{
    //We could set some parameters here.
    //Parameters.Set("Player", Engine.Sprites.AllVisiblePlayers.First());
    //Parameters.Set("MaxDistance", 1000);

    Owner.RenewableResources.Create(_boostResourceName, 800, 0, 10);

    owner.OnHit += Owner_OnHit;

    SetAIState(new GotoRadiusOfObservedObject(this));

}

#region AI States.

private class GotoRadiusOfObservedObject
    : IAeAIStateHandler
{
    private readonly AeAIStateMachine _stateMachine;
    private AeRotationDirection _rotateDirection;
    private float _rotationAngle = AeRandom.Variance(5, 0.10f);
    private readonly AeVector _targetLocation;
    private readonly AeSprite _targetPlayerSprite;

    public GotoRadiusOfObservedObject(AeAIStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _targetPlayerSprite = stateMachine.Engine.Sprites.AllVisiblePlayers.First();
        _targetLocation = _targetPlayerSprite.Location.RandomAtDistance(10, 50);

        var deltaAngle = stateMachine.Owner.HeadingAngleToInSignedDegrees(_targetLocation);
        _rotateDirection = deltaAngle >= 0 ? AeRotationDirection.Clockwise : AeRotationDirection.CounterClockwise;
    }

    public void Tick(float epoch)
    {
        if (_stateMachine.TimeInStateSeconds >= 2.5)
        {
            _rotationAngle = AeMath.Damp(_rotationAngle, 25, decayRatePerSecond: 4.5f, epoch);
        }

        //Throttle up during the turn.
        _stateMachine.Owner.Throttle = AeMath.Damp(_stateMachine.Owner.Throttle, 1.5f, 0.2f, epoch);

        if (_rotationAngle >= 4.9f)
        {
            //Lets just change the state....
        }

        if (_stateMachine.Owner.RotateMovementVectorIfNotPointingAt(_targetPlayerSprite, _rotationAngle, _rotateDirection, 10.0f, epoch) == false)
        {
            _stateMachine.SetAIState(new SteadyOnCurrentPath(_stateMachine));
        }
    }
}

private class SteadyOnCurrentPath
    : IAeAIStateHandler
{
    private readonly AeAIStateMachine _stateMachine;
    private float _burndownEpochs = 3;

    public SteadyOnCurrentPath(AeAIStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Tick(float epoch)
    {
        //Throttle down during the steady path.
        _stateMachine.Owner.Throttle = AeMath.Damp(_stateMachine.Owner.Throttle, 0, 0.2f, epoch);

        _burndownEpochs -= epoch;

        if (_burndownEpochs <= 0)
        {
            _stateMachine.SetAIState(new GotoRadiusOfObservedObject(_stateMachine));
        }
    }
}

#endregion

private void Owner_OnHit(AeSprite sender, AeDamageType damageType, int damageAmount)
{
    /*
    if (sender.HullHealth <= 10)
    {
        //Do something different when we get low on health?
        ChangeState(new AIStateTransitionToEvasiveEscape(this));
    }
    */
}

