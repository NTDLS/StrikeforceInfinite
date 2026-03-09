private bool _toggle = false;

public override bool Fire()
{
    if (CanFire)
    {
        Engine.Rendering.AddScreenShake(4, 100);
        FireSound?.Play();
        MunitionQuantity--;

        if (_toggle)
        {
            var offsetRight = Owner.Orientation.RotatedBy(90) * new AeVector(10, 10);
            Engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);
        }
        else
        {
            var offsetLeft = Owner.Orientation.RotatedBy(-90) * new AeVector(10, 10);
            Engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);
        }

        _toggle = !_toggle;

        return true;
    }
    return false;
}
