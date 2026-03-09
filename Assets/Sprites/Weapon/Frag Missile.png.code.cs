private bool _toggle = false;

public override bool Fire()
{
    if (CanFire)
    {
        FireSound?.Play();
        MunitionQuantity--;

        var offset = Owner.Orientation.RotatedBy(90.Invert(_toggle)) * new AeVector(10, 10);

        _toggle = !_toggle;

        if (LockedTargets?.Count > 0)
        {
            foreach (var weaponLock in LockedTargets.Where(o => o.LockType == AeConstants.SiWeaponsLockType.Hard))
            {
                Engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, Owner.Location + offset);
            }
        }
        else
        {
            Engine.Sprites.Munitions.Add(this, Owner.Location + offset);
        }

        return true;
    }
    return false;
}
