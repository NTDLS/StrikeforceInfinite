namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents an action that triggers an explosion effect for a sprite within the animation engine.
    /// </summary>
    /// <remarks>Use this class to initiate an explosion animation for a specific sprite identified by its
    /// unique identifier. This action is typically used to visually indicate destruction or impact in sprite-based
    /// animations.</remarks>
    public class AeSpriteActionExplode
        : AeSpriteAction
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionExplode(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
