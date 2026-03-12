using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.TextBlock;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the radar position indicator sprite used to display the current position on the radar.
        /// </summary>
        public AeSpriteRadarPositionIndicator? RadarPositionIndicator { get; protected set; }

        /// <summary>
        /// Gets the text block representing the radar position information.
        /// </summary>
        public AeSpriteRadarPositionTextBlock? RadarPositionText { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteShip class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance that manages the game logic and rendering for the sprite ship.</param>
        /// <param name="assetKey">The key identifying the asset to be used for the sprite ship's visual representation. Cannot be null or
        /// empty.</param>
        public AeSpriteShip(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
        }

        /// <summary>
        /// Updates the radar position indicator and associated text to reflect the player's current location and
        /// visibility on the radar display.
        /// </summary>
        /// <remarks>The indicator and text are shown only when the player's sprite is outside the main
        /// render bounds. Both elements are positioned relative to the center of the display and oriented based on the
        /// player's angle. If the player is within the render bounds, the indicator and text are hidden.</remarks>
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

        /// <summary>
        /// Releases resources associated with the radar position indicator and text elements, and performs additional
        /// cleanup defined in the base class.
        /// </summary>
        /// <remarks>Call this method when the radar position indicator and text elements are no longer
        /// needed to ensure proper resource management. Overrides the base class cleanup to include additional element
        /// deletion.</remarks>
        public override void Cleanup()
        {
            RadarPositionIndicator?.QueueForDelete();
            RadarPositionText?.QueueForDelete();

            base.Cleanup();
        }
    }
}
