using NTDLS.DatagramMessaging;
using Si.MpLibrary.DatagramMessages.SpriteActions;

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

        public void RecordSpawn(SiSpriteActionSpawn? action)
            => AppendBuffer(action);

        public void RecordExplode(uint spriteUID)
            => AppendBuffer(new SiSpriteActionExplode(spriteUID));

        public void RecordDelete(uint spriteUID)
            => AppendBuffer(new SiSpriteActionDelete(spriteUID));

        public void FlushSpriteVectorsToClients(DmMessenger dmMessenger, IEnumerable<Session> sessions)
        {
            if (_spriteActionBuffer.Count > 0)
            {
                //if (State.PlayMode != SiPlayMode.SinglePlayer && RpcClient?.IsConnected == true)
                //var spriteActions = new SiSpriteActions(_spriteActionBuffer);

                //spriteActions.ConnectionId = State.ConnectionId;

                //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                //UdpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, spriteActions);

                foreach (var vectorAction in _spriteActionBuffer)
                {
                    //Task.Run(() => ??
                    //Parallel.ForEach(sessions, session => ??

                    foreach (var session in sessions)
                    {
                        if (session.DatagramEndPoint != null)
                        {
                            dmMessenger.Dispatch(vectorAction, session.DatagramEndPoint);
                        }
                    }
                }

                Console.WriteLine($"Flushed {_spriteActionBuffer.Count} sprite actions to clients.");

                _spriteActionBuffer.Clear();
            }
        }
    }
}
