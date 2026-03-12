using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using NTDLS.DatagramMessaging;
using System;

namespace Ae.Engine.MultiPlay
{
    /// <summary>
    /// Handles datagram messages related to sprite actions within the engine context.
    /// </summary>
    /// <remarks>This handler processes various sprite action datagrams, such as delete, explode, spawn, and
    /// motion, by delegating to the appropriate methods. Intended for internal use within the engine's messaging
    /// infrastructure.</remarks>
    /// <param name="engine">The engine instance used to process sprite action datagram messages.</param>
    internal class DatagramMessageHandler(AeEngine engine)
        : IDmDatagramHandler
    {
        /// <summary>
        /// Deletes a sprite action from the specified context using the provided payload.
        /// </summary>
        /// <param name="context">The context in which the sprite action will be deleted. Cannot be null.</param>
        /// <param name="payload">The payload containing information about the sprite action to delete. Cannot be null.</param>
        public void SiSpriteActionDelete(DmContext context, AeSpriteActionDelete payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionExplode(DmContext context, AeSpriteActionExplode payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionSpawn(DmContext context, AeSpriteActionSpawn payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void SiSpriteActionVector(DmContext context, AeSpriteActionMotion payload)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}
