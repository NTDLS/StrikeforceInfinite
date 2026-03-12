namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a sprite entity within the engine, providing events and virtual methods for responding to changes in
    /// state, visibility, location, orientation, and lifecycle events such as materialization and deletion.
    /// </summary>
    /// <remarks>AeSprite serves as a base class for sprite objects in the engine. It exposes events for
    /// handling interactions such as hits, visibility changes, deletion, and explosions, allowing derived classes or
    /// consumers to respond to these occurrences. Virtual methods can be overridden to customize behavior when the
    /// sprite's visibility, location, or orientation changes, or when it is materialized. This class is intended to be
    /// extended for specific sprite behaviors and integrated with engine systems.</remarks>
    public partial class AeSprite
    {
        /// <summary>
        /// Represents a method that handles hit events for a sprite, providing information about the damage type and
        /// amount.
        /// </summary>
        /// <param name="sender">The sprite instance that received the hit event.</param>
        /// <param name="damageType">The type of damage inflicted on the sprite.</param>
        /// <param name="damageAmount">The amount of damage applied to the sprite. Must be a non-negative integer.</param>
        public delegate void HitEvent(AeSprite sender, AeDamageType damageType, int damageAmount);
        /// <summary>
        /// Represents a method that handles hit events for a sprite, providing information about the damage type and
        /// amount.
        /// </summary>
        public event HitEvent? OnHit;

        /// <summary>
        /// Represents a method that handles the event when an AeSprite instance is queued for deletion.
        /// </summary>
        /// <param name="sender">The AeSprite instance that is being queued for deletion.</param>
        public delegate void QueuedForDeleteEvent(AeSprite sender);
        /// <summary>
        /// Represents a method that handles the event when an AeSprite instance is queued for deletion.
        /// </summary>
        public event QueuedForDeleteEvent? OnQueuedForDelete;

        /// <summary>
        /// Represents a method that is called when the visibility of an AeSprite changes.
        /// </summary>
        /// <param name="sender">The AeSprite instance whose visibility has changed.</param>
        public delegate void VisibilityChangedEvent(AeSprite sender);
        /// <summary>
        /// Represents a method that is called when the visibility of an AeSprite changes.
        /// </summary>
        public event VisibilityChangedEvent? OnVisibilityChanged;

        /// <summary>
        /// Represents the method that will handle an explosion event for an AeSprite instance.
        /// </summary>
        /// <param name="sender">The AeSprite instance that triggered the explosion event.</param>
        public delegate void ExplodeEvent(AeSprite sender);
        /// <summary>
        /// Represents the method that will handle an explosion event for an AeSprite instance.
        /// </summary>
        public event ExplodeEvent? OnExplode;

        /// <summary>
        /// Invoked when the visibility state of the element changes.
        /// </summary>
        /// <remarks>Override this method to respond to visibility changes, such as updating UI elements
        /// or triggering related actions. The base implementation does nothing.</remarks>
        public virtual void VisibilityChanged() { }

        /// <summary>
        /// Called when the location of the associated object changes.
        /// </summary>
        /// <remarks>Override this method to respond to location changes in derived classes. This method
        /// is intended to be used as a notification point for subclasses and does not perform any action by
        /// default.</remarks>
        public virtual void LocationChanged() { }

        /// <summary>
        /// Handles changes in device or display orientation.
        /// </summary>
        /// <remarks>Override this method to implement custom behavior when the orientation changes. This
        /// method is typically called by the framework in response to orientation events.</remarks>
        public virtual void OrientationChanged() { }

        /// <summary>
        /// Called after the sprite is created and added to the engine.
        /// This is useful for doing things that require the sprite to be fully initialized and part of the engine,
        /// such as adding child sprites or accessing engine services.
        /// </summary>
        public virtual void OnMaterialized() { }
    }
}
