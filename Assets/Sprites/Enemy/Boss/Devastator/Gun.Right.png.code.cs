public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
{
    if (this.IsPointingAt(Engine.Player.Sprite, 10, 1000))
    {
        FireWeapon("Sprites/Weapon/Lancer");
    }

    base.ApplyIntelligence(epoch, cameraDisplacement);
}
