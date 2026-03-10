private bool _toggle = false;

public override bool Fire()
{
    if (CanFire)
    {
		Sounds?.OneOf()?.Play();
        MunitionQuantity--;

        var offset = Owner.Orientation.RotatedBy(90.Invert(_toggle)) * new AeVector(10, 10);
        Engine.Sprites.Munitions.Add(this, Owner.Location + offset);

        _toggle = !_toggle;

        return true;
    }

    return false;
}
