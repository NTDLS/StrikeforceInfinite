private AeSpriteAttachment? _thrusterLeft;
private AeSpriteAttachment? _thrusterRight;

public override void OnMaterialized()
{
    Orientation.Degrees = AeRandom.Between(0, 359);

    //AddAIController(new AILogisticsHostileEngagement(Engine, this, [Engine.Player.Sprite]));

    //SetCurrentAIController<AILogisticsHostileEngagement>();

    _thrusterLeft = Attachments.Single(o => o.AssetKey == "Sprites/Enemy/Boss/Devastator/Jet.Left");
    _thrusterRight = Attachments.Single(o => o.AssetKey == "Sprites/Enemy/Boss/Devastator/Jet.Right");

    RecalculateMovementVectorFromOrientation();
}

private float TargetThrottle
{
    get
    {
        if (_thrusterLeft?.IsDeadOrExploded == true && _thrusterRight?.IsDeadOrExploded == true)
        {
            return 0.05f; // idle drift
        }
        else if (_thrusterLeft?.IsDeadOrExploded == true || _thrusterRight?.IsDeadOrExploded == true)
        {
            return 0.5f;  // limp mode
        }
        else
        {
            return 1.0f;  // full thrust
        }
    }
}

public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
{
    Throttle = AeMath.Damp(Throttle, TargetThrottle, 0.01f, epoch);

    var offset = Orientation * new AeVector(40f, 40f);

    if (_thrusterLeft?.IsDeadOrExploded == true)
    {
        Engine.Sprites.Particles.EmitConeAt(_thrusterLeft.CalculatedLocation + offset, _thrusterLeft.CalculatedOrientation.Degrees, 15f, 2, 150f, 250f, AeRenderingUtility.GetRandomHotColor(), new Size(1, 1));
    }
    if (_thrusterRight?.IsDeadOrExploded == true)
    {
        Engine.Sprites.Particles.EmitConeAt(_thrusterRight.CalculatedLocation + offset, _thrusterRight.CalculatedOrientation.Degrees, 15f, 2, 150f, 250f, AeRenderingUtility.GetRandomHotColor(), new Size(1, 1));
    }

    if (HullHealth <= Metadata.Hull / 2)
    {
        Engine.Sprites.Particles.ParticleBlastAt(this, AeRandom.Between(0, 1));
    }

    base.ApplyMotion(epoch, cameraDisplacement);
}
