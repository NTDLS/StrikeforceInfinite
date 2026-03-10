using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.TextBlock;
using System;
using System.Collections.Generic;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Interactive.Ship
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    [AssetClass("Ship", "", AeBaseAssetType.Image, true)]
    public class AeSpriteShip
        : AeSpriteInteractive
    {
        private readonly Dictionary<string, AeSpriteWeapon> _droneWeaponsCache = new();
        public AeSpriteRadarPositionIndicator? RadarPositionIndicator { get; protected set; }
        public AeSpriteRadarPositionTextBlock? RadarPositionText { get; protected set; }

        public AeSpriteShip(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
        }

        public void AdjustRadarPositionIndicator()
        {
            if (RadarPositionIndicator != null && RadarPositionText != null)
            {
                if (Engine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(Engine.Player.Sprite));

                    RadarPositionText.IsVisible = Engine.Player.Sprite.IsVisible;
                    RadarPositionText.IsFixedPosition = true;
                    RadarPositionIndicator.IsVisible = Engine.Player.Sprite.IsVisible;
                    RadarPositionIndicator.IsFixedPosition = true;

                    float requiredAngleRadians = Engine.Player.Sprite.AngleToInSignedRadians(this);

                    RadarPositionIndicator.Location = Engine.Display.CenterCanvas
                        + new AeVector(requiredAngleRadians) * new AeVector(200, 200);
                    RadarPositionIndicator.Orientation.RadiansSigned = requiredAngleRadians;

                    RadarPositionText.Location = Engine.Display.CenterCanvas
                        + new AeVector(requiredAngleRadians) * new AeVector(120, 120);
                    RadarPositionIndicator.Orientation.RadiansSigned = requiredAngleRadians;
                }
                else
                {
                    RadarPositionText.IsVisible = false;
                    RadarPositionIndicator.IsVisible = false;
                }
            }
        }

        public override void Cleanup()
        {
            RadarPositionIndicator?.QueueForDelete();
            RadarPositionText?.QueueForDelete();

            base.Cleanup();
        }
    }
}
