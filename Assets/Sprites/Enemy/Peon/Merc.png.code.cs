private AeSpriteAnimation? _thrusterAnimation;
private AeSpriteAnimation? _boosterAnimation;

public override void OnMaterialized()
{
	RecalculateMovementVectorFromOrientation();

	OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

	_thrusterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustStandard32x32", (o) =>
	{
		o.Location = Location;
		o.Orientation = Orientation;
		o.IsVisible = true;
		o.OwnerUID = UID;
	});

	_boosterAnimation = Engine.Sprites.Animations.Add("Sprites/Animation/ThrustBoost32x32", (o) =>
	{
		o.Location = Location;
		o.Orientation = Orientation;
		o.IsVisible = true;
		o.OwnerUID = UID;
	});

	UpdateThrustAnimationPositions();

	//AddAIController(new AILogisticsHostileEngagement(Engine, this, [Engine.Player.Sprite]));
	//SetCurrentAIController<AILogisticsHostileEngagement>();
	base.OnMaterialized();
}

public override void LocationChanged() => UpdateThrustAnimationPositions();

private void UpdateThrustAnimationPositions()
{
	var pointBehind = (Orientation * -1) * new AeVector(20, 20);

	if (_thrusterAnimation != null && _thrusterAnimation.IsVisible)
	{
		_thrusterAnimation.Orientation = Orientation;
		_thrusterAnimation.Location = Location + pointBehind;
	}
	if (_boosterAnimation != null && _boosterAnimation.IsVisible)
	{
		_boosterAnimation.Orientation = Orientation;
		_boosterAnimation.Location = Location + pointBehind;
	}
}

private void EnemyBase_OnVisibilityChanged(AeSprite sender)
{
	_thrusterAnimation?.IsVisible = false;
	_boosterAnimation?.IsVisible = false;
}

public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
{
	base.ApplyIntelligence(epoch, cameraDisplacement);
	ApplyWeaponsLogic();
}

private void ApplyWeaponsLogic()
{
	var playersIAmPointingAt = GetPointingAtOf(Engine.Sprites.AllVisiblePlayers, 2.0f);
	if (playersIAmPointingAt.Any())
	{
		var closestDistance = ClosestDistanceOf(playersIAmPointingAt);

		if (closestDistance < 1000)
		{
			if (closestDistance > 500 && HasWeaponAndAmmo("Sprites/Weapon/Vulcan Cannon"))
			{
				FireWeapon("Sprites/Weapon/Vulcan Cannon");
			}
			else if (closestDistance > 0 && HasWeaponAndAmmo("Sprites/Weapon/Dual Vulcan Cannon"))
			{
				FireWeapon("Sprites/Weapon/Dual Vulcan Cannon");
			}
		}
	}
}

/// <summary>
/// Moves the sprite based on its thrust/boost (velocity).
/// </summary>
/// <param name="cameraDisplacement"></param>
public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
{
	base.ApplyMotion(epoch, cameraDisplacement);

	_thrusterAnimation?.IsVisible = MovementVector.Sum() > 0;
	_boosterAnimation?.IsVisible = Throttle > 1;
}

public override void Cleanup()
{
	_thrusterAnimation?.QueueForDelete();
	_boosterAnimation?.QueueForDelete();

	base.Cleanup();
}
