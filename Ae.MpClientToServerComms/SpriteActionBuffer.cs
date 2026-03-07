using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using NTDLS.DatagramMessaging;
using System.Net;

namespace Ae.MpClientToServerComms
{
    public class SpriteActionBuffer
    {
        private readonly List<AeSpriteAction> _spriteActionBuffer = new();
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

        public void RecordHit(uint spriteUID, uint munitionUID)
            => AppendBuffer(new AeSpriteActionHit(spriteUID, munitionUID));

        public void RecordSpawn(AeSpriteActionSpawn? action)
            => AppendBuffer(action);

        public void RecordExplode(uint spriteUID)
            => AppendBuffer(new AeSpriteActionExplode(spriteUID));

        public void RecordDelete(uint spriteUID)
            => AppendBuffer(new AeSpriteActionDelete(spriteUID));

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
