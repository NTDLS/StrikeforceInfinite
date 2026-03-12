using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using NTDLS.DatagramMessaging;
using System;
using System.Collections.Generic;
using System.Net;

namespace Ae.MpClientToServerComms
{
    /// <summary>
    /// Provides a buffer for recording and batching sprite actions, enabling efficient dispatch of updates to clients
    /// at the end of the game loop.
    /// </summary>
    /// <remarks>Use this class to accumulate sprite actions such as motion, hits, spawns, explosions, and
    /// deletions. Actions are buffered and sent together to clients, reducing network overhead and ensuring consistent
    /// state updates. Set ShouldRecordEvents to control whether actions are recorded. Call FlushSpriteVectorsToClients
    /// to dispatch all buffered actions and clear the buffer.</remarks>
    public class SpriteActionBuffer
    {
        private readonly List<AeSpriteAction> _spriteActionBuffer = new();

        /// <summary>
        /// Gets or sets a value indicating whether events should be recorded.
        /// </summary>
        public bool ShouldRecordEvents { get; set; } = true;

        private void AppendBuffer(AeSpriteAction? action)
        {
            if (ShouldRecordEvents && action != null)
            {
                _spriteActionBuffer.Add(action);
            }
        }

        /// <summary>
        /// Buffers sprite vector information so that all of the updates can be sent at one time at the end of the game loop.
        /// </summary>
        public void RecordMotion(AeSpriteActionMotion? action)
            => AppendBuffer(action);

        /// <summary>
        /// Records a hit event between a sprite and a munition for later processing.
        /// </summary>
        /// <param name="spriteUID">The unique identifier of the sprite that was hit.</param>
        /// <param name="munitionUID">The unique identifier of the munition that caused the hit.</param>
        public void RecordHit(uint spriteUID, uint munitionUID)
            => AppendBuffer(new AeSpriteActionHit(spriteUID, munitionUID));

        /// <summary>
        /// Records a spawn action for later processing or analysis.
        /// </summary>
        /// <param name="action">The spawn action to record. If null, no action is recorded.</param>
        public void RecordSpawn(AeSpriteActionSpawn? action)
            => AppendBuffer(action);

        /// <summary>
        /// Records an explosion action for the specified sprite, queuing it for processing.
        /// </summary>
        /// <param name="spriteUID">The unique identifier of the sprite for which the explosion action is recorded.</param>
        public void RecordExplode(uint spriteUID)
            => AppendBuffer(new AeSpriteActionExplode(spriteUID));

        /// <summary>
        /// Records a delete action for the specified sprite, scheduling it for removal.
        /// </summary>
        /// <remarks>This method does not immediately remove the sprite; it queues the delete action for
        /// processing. Ensure that the spriteUID corresponds to a sprite that has not already been deleted.</remarks>
        /// <param name="spriteUID">The unique identifier of the sprite to be deleted. Must reference a valid, existing sprite.</param>
        public void RecordDelete(uint spriteUID)
            => AppendBuffer(new AeSpriteActionDelete(spriteUID));

        /// <summary>
        /// Dispatches all buffered sprite actions to the specified client endpoints using the provided messenger.
        /// </summary>
        /// <remarks>This method clears the sprite action buffer after dispatching actions. Only endpoints
        /// that are not null receive the actions.</remarks>
        /// <param name="dmMessenger">The messenger used to send sprite action collections to each client endpoint.</param>
        /// <param name="iPEndPoints">A collection of client network endpoints to which the sprite actions will be dispatched. Endpoints that are
        /// null are ignored.</param>
        public void FlushSpriteVectorsToClients(DmMessenger dmMessenger, IEnumerable<IPEndPoint?>? iPEndPoints)
        {
            if (_spriteActionBuffer.Count > 0 && iPEndPoints != null)
            {
                //if (State.PlayMode != SiPlayMode.SinglePlayer && RpcClient?.IsConnected == true)
                //var spriteActions = new SiSpriteActions(_spriteActionBuffer);

                //spriteActions.ConnectionId = State.ConnectionId;

                //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                //UdpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, spriteActions);

                var actionCollection = new AeSpriteActionCollection(_spriteActionBuffer.ToArray());

                //Task.Run(() => ??
                //Parallel.ForEach(sessions, session => ??

                foreach (var iPEndPoint in iPEndPoints)
                {
                    if (iPEndPoint != null)
                    {
                        dmMessenger.Dispatch(actionCollection, iPEndPoint);
                    }
                }

                Console.WriteLine($"Flushed {_spriteActionBuffer.Count} sprite actions to clients.");

                _spriteActionBuffer.Clear();
            }
        }
    }
}
