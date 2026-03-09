public bool FireToggler { get; set; }

public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
{
    if (DistanceTo(Engine.Player.Sprite) < 1000)
    {
        //Rotate the turret toward the player.
        var deltaAngleToPlayer = this.HeadingAngleToInSignedDegrees(Engine.Player.Sprite);
        if (deltaAngleToPlayer < 1)
        {
            Orientation.Degrees -= 0.25f;
        }
        else if (deltaAngleToPlayer > 1)
        {
            Orientation.Degrees += 0.25f;
        }

        if (deltaAngleToPlayer.IsBetween(-10, 10))
        {
            if (FireToggler)
            {
                var pointRight = Orientation.RotatedBy(90) * new AeVector(21, 21);
                FireToggler = !FireWeapon("Sprites/Weapon/Lancer", Location + pointRight);
            }
            else
            {
                var pointLeft = Orientation.RotatedBy(-90) * new AeVector(21, 21);
                FireToggler = !FireWeapon("Sprites/Weapon/Lancer", Location + pointLeft);
            }
        }
    }

    base.ApplyIntelligence(epoch, cameraDisplacement);
}
