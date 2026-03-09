public bool FireToggler { get; set; }

public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
{
    // Since the turret.BaseLocation is relative to the top-left corner of the base sprite, we need
    // to get the position relative to the center of the base sprite image so that we can rotate around that.
    var turretOffset = LocationRelativeToOwner.EnsureNotNull() - (RootOwner.Size / 2.0f);

    // Apply the rotated offsets to get the new turret location relative to the base sprite center.
    Location = RootOwner.Location + turretOffset.RotatedBy(RootOwner.Orientation.DegreesSigned);

    if (DistanceTo(Engine.Player.Sprite) < 1000)
    {
        //Rotate the turret toward the player.
        var deltaAngleToPlayer = this.HeadingAngleToInSignedDegrees(Engine.Player.Sprite);
        if (deltaAngleToPlayer < 1)
        {
            Orientation.Degrees -= 45f * epoch;
        }
        else if (deltaAngleToPlayer > 1)
        {
            Orientation.Degrees += 45f * epoch;
        }

        if (deltaAngleToPlayer.IsBetween(-10, 10))
        {
            if (FireToggler)
            {
                var pointRight = Orientation.RotatedBy(90f) * new AeVector(10, 10);
                FireToggler = !FireWeapon("Sprites/Weapon/Thunderstrike Missile", Location + pointRight);
            }
            else
            {
                var pointLeft = Orientation.RotatedBy(-90) * new AeVector(10, 10);
                FireToggler = FireWeapon("Sprites/Weapon/Thunderstrike Missile", Location + pointLeft);
            }
        }
    }
}
