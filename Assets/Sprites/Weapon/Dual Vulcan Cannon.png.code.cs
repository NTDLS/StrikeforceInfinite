public override bool Fire()
{
    if (CanFire)
    {
        FireSound?.Play();

		var offsetRight = Owner.Orientation.RotatedBy(90) * new AeVector(5, 5);
		Engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);

		var offsetLeft = Owner.Orientation.RotatedBy(-90) * new AeVector(5, 5);
		Engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);

        MunitionQuantity--;

        return true;
    }
    return false;
}
