using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Munition;
using System.Linq;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a sprite entity in the game world that can interact with munitions, sustain damage, and undergo state
    /// changes such as exploding or reviving.
    /// </summary>
    /// <remarks>AeSprite provides methods for handling hits from munitions, managing shield and hull health,
    /// and controlling the sprite's lifecycle. The class supports extensibility through virtual methods, allowing
    /// derived types to customize hit and explosion behavior. Attachments associated with the sprite are also managed
    /// during explosion events. This class is intended to be used as a base for game objects that require collision,
    /// damage, and destruction mechanics.</remarks>
    public partial class AeSprite
    {
        /// <summary>
        /// Restores the object to an active state if it has been marked as dead or exploded.
        /// </summary>
        /// <remarks>Call this method to reset the object's status after it has been deactivated due to
        /// death or explosion. This enables further interactions or operations that require the object to be
        /// active.</remarks>
        public void ReviveDeadOrExploded()
        {
            IsDeadOrExploded = false;
        }

        /// <summary>
        /// Allows for the testing of hits from a munition, 
        /// </summary>
        /// <param name="munition">The munition object that is being tested for.</param>
        /// <param name="hitTestPosition">The position to test for hit.</param>
        /// <returns></returns>
        public virtual bool TryMunitionHit(AeSpriteMunition munition, AeVector hitTestPosition)
        {
            if (IntersectsAabb(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Processes the impact of a munition on the object, applying damage and triggering explosion if health is
        /// depleted.
        /// </summary>
        /// <remarks>This method updates the object's state based on the munition's effect. If the
        /// object's health reaches zero or below, an explosion is triggered.</remarks>
        /// <param name="munition">The munition that has struck the object. Cannot be null.</param>
        public virtual void MunitionHit(AeSpriteMunition munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                Explode();
            }
        }

        /// <summary>
        /// Subtract from the objects hullHealth.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(int damage)
        {
            if (ShieldHealth > 0)
            {
                Engine.Audio.PlayRandomShieldHit();
                damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                damage = damage < 1 ? 1 : damage;
                damage = damage > ShieldHealth ? ShieldHealth : damage; //No need to go negative with the damage.
                ShieldHealth -= damage;

                OnHit?.Invoke(this, AeDamageType.Shield, damage);
            }
            else
            {
                Engine.Audio.PlayRandomHullHit();
                damage = damage > HullHealth ? HullHealth : damage; //No need to go negative with the damage.
                HullHealth -= damage;

                OnHit?.Invoke(this, AeDamageType.Hull, damage);
            }
        }

        /// <summary>
        /// Hits this object with a given munition.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(AeSpriteMunition munition)
        {
            if (munition.Weapon?.Metadata != null)
            {
                Hit(AeRandom.Between(munition.Weapon.Metadata.Damage, 0));
            }
        }

        /// <summary>
        /// Triggers the explosion sequence for the object and its visible attachments.
        /// </summary>
        /// <remarks>After calling this method, the object is marked as exploded and becomes invisible.
        /// All visible attachments are also exploded. If the object is not an attachment, it is queued for deletion.
        /// The method raises the OnExplode event to notify subscribers.</remarks>
        public virtual void Explode()
        {
            foreach (var attachment in Attachments.Where(o => o._isVisible))
            {
                attachment.Explode();
            }

            IsDeadOrExploded = true;
            _isVisible = false;

            if (this is not AeSpriteAttachment) //Attachments are deleted when the owning object is deleted.
            {
                QueueForDelete();
            }

            OnExplode?.Invoke(this);
        }

        /// <summary>
        /// Triggers a small fire explosion animation at the current object's location.
        /// </summary>
        /// <remarks>Use this method to visually indicate that the object has been hit or destroyed. The
        /// explosion effect is randomly selected and may vary each time the method is called.</remarks>
        public virtual void HitExplosion()
        {
            Engine.Sprites.Animations.AddRandomSmallFireExplosionAt(this);
        }
    }
}

