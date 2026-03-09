public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
{
    if (this.IsPointingAt(Engine.Player.Sprite, 10, 1000))
    {
        FireWeapon("Sprites/Weapon/Vulcan Cannon");
    }

    base.ApplyIntelligence(epoch, cameraDisplacement);
}
