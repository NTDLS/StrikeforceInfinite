using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy._Superclass
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemyBase
        : SpriteInteractiveShipBase
    {
        public SpriteEnemyBase(EngineCore engine, string assetKey)
                : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();

            RadarPositionIndicator = _engine.Sprites.RadarPositions.Add();
            RadarPositionIndicator.IsVisible = false;

            RadarPositionText = _engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new SiVector());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void OrientationChanged() => LocationChanged();

        public override void Explode()
        {
            _engine.Player.Stats.Bounty += Metadata.Bounty ?? 0;

            /*
            if (SiRandom.PercentChance(10))
            {
                var powerup = SiRandom.Between(0, 4) switch
                {
                    0 => new SpritePowerupAmmo(_engine),
                    1 => new SpritePowerupBoost(_engine),
                    2 => new SpritePowerupBounty(_engine),
                    3 => new SpritePowerupRepair(_engine),
                    4 => new SpritePowerupShield(_engine),
                    _ => null as SpritePowerupBase
                };

                if (powerup != null)
                {
                    powerup.Location = Location;
                    _engine.Sprites.Powerups.Add(powerup);
                }
            }
            */
            base.Explode();
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            /*
            //When an enemy has boost available, it will use it.
            if (AvailableBoost > 0)
            {
                if (ThrottlePercentage < 1.0) //Ramp up the boost until it is at 100%
                {
                    ThrottlePercentage += _engine.Settings.EnemyVelocityRampUp;
                }
                AvailableBoost -= MaximumBoostSpeed * ThrottlePercentage; //Consume boost.

                if (AvailableBoost < 0) //Sanity check available boost.
                {
                    AvailableBoost = 0;
                }
            }
            else if (ThrottlePercentage > 0) //Ramp down the boost.
            {
                ThrottlePercentage -= _engine.Settings.EnemyVelocityRampDown;
                if (ThrottlePercentage < 0)
                {
                    ThrottlePercentage = 0;
                }
            }
            */

            base.ApplyMotion(epoch, displacementVector);

            FixRadarPositionIndicator();
        }
    }
}
