using Si.MpLibrary.SpriteActions;
using System;

namespace Si.MpLibrary
{
    public class SpriteActionBuffer
    {
        private readonly List<SiSpriteAction> _spriteActionBuffer = new();
        public bool ShouldRecordEvents { get; set; } = true;

        private void AppendBuffer(SiSpriteAction? action)
        {
            if (ShouldRecordEvents && action != null)
            {
                _spriteActionBuffer.Add(action);
            }
        }

        /// <summary>
        /// Buffers sprite vector information so that all of the updates can be sent at one time at the end of the game loop.
        /// </summary>
        public void RecordVector(SiSpriteActionVector? action)
            => AppendBuffer(action);

        public void RecordExplode(uint spriteUID)
            => AppendBuffer(new SiSpriteActionExplode(spriteUID));

        public void FlushSpriteVectorsToClients()
        {
            if (_spriteActionBuffer.Count > 0)
            {
                //if (State.PlayMode != SiPlayMode.SinglePlayer && RpcClient?.IsConnected == true)
                //var spriteActions = new SiSpriteActions(_spriteActionBuffer);

                //spriteActions.ConnectionId = State.ConnectionId;

                //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                //UdpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, spriteActions);

                foreach(var vectorAction in _spriteActionBuffer.OfType< SiSpriteActionVector>())
                {
                    Console.WriteLine($"Action: x{vectorAction.X}, y{vectorAction.Y}");
                }

                Console.WriteLine($"Flushed {_spriteActionBuffer.Count} sprite actions to clients.");

                _spriteActionBuffer.Clear();
            }
        }
    }
}
