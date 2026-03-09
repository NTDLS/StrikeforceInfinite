public override bool Fire()
{
    if (CanFire)
    {
        FireSound?.Play();

		for (int i = -15; i < 15; i++) // Create an initial spread so the bullets don't come from the same point.
		{
			var offset = Owner.Orientation.RotatedBy(90) * new AeVector(i, i);
			Engine.Sprites.Munitions.Add(this, Owner.Location + offset);
		}
		MunitionQuantity--;

        return true;
    }
    return false;
}
