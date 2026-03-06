using Si.Engine.Sprite._Superclass.TextBlock;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;

namespace Si.Engine.Sprite._Superclass.Interactive.Ship
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    public class SpriteShip
        : SpriteInteractive
    {
        private readonly Dictionary<string, SpriteWeapon> _droneWeaponsCache = new();
        public SpriteRadarPositionIndicator? RadarPositionIndicator { get; protected set; }
        public SpriteRadarPositionTextBlock? RadarPositionText { get; protected set; }

        public SpriteShip(EngineCore engine, string assetKey)
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
                        + new SiVector(requiredAngleRadians) * new SiVector(200, 200);
                    RadarPositionIndicator.Orientation.RadiansSigned = requiredAngleRadians;

                    RadarPositionText.Location = Engine.Display.CenterCanvas
                        + new SiVector(requiredAngleRadians) * new SiVector(120, 120);
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
