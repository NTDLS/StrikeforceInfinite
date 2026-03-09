public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
{
    IsVisible = !RootOwner.MovementVector.Magnitude().IsNearZero();
    base.ApplyIntelligence(epoch, cameraDisplacement);
}
