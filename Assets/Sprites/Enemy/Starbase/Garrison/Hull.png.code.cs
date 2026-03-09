public override void OnMaterialized()
{
	RecalculateMovementVectorFromOrientation();
	base.OnMaterialized();
}
