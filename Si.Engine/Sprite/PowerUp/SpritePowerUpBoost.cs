using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.PowerUp
{
    internal class SpritePowerupBoost
        : SpritePowerup
    {
        private readonly int imageCount = 3;

        public SpritePowerupBoost(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            PowerupAmount = 100;

            int multiplier = SiRandom.Between(0, imageCount - 1);
            PowerupAmount *= multiplier + 1;
        }

        public override void ApplyIntelligence(float epoch, SiVector cameraDisplacement)
        {
            if (IntersectsAABB(Engine.Player.Sprite))
            {
                //_engine.Player.Sprite.AvailableBoost += PowerupAmount;
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
