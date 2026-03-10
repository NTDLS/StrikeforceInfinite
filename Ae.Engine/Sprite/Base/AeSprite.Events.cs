using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Base
{
    public partial class AeSprite
    {
        public delegate void HitEvent(AeSprite sender, AeDamageType damageType, int damageAmount);
        public event HitEvent? OnHit;

        public delegate void QueuedForDeleteEvent(AeSprite sender);
        public event QueuedForDeleteEvent? OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(AeSprite sender);
        public event VisibilityChangedEvent? OnVisibilityChanged;

        public delegate void ExplodeEvent(AeSprite sender);
        public event ExplodeEvent? OnExplode;

        public virtual void VisibilityChanged() { }
        public virtual void LocationChanged() { }
        public virtual void OrientationChanged() { }

        /// <summary>
        /// Called after the sprite is created and added to the engine.
        /// This is useful for doing things that require the sprite to be fully initialized and part of the engine,
        /// such as adding child sprites or accessing engine services.
        /// </summary>
        public virtual void OnMaterialized() { }
    }
}
