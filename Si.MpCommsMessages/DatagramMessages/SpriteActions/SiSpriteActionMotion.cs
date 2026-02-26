namespace Si.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class SiSpriteActionMotion : SiSpriteAction
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float OrientationDegreesSigned { get; set; }
        public float Throttle { get; set; }
        //public float BoostPercentage { get; set; }
        public float Speed { get; set; }
        public float RotationSpeed { get; set; }
        //public float Boost { get; set; }

        public SiSpriteActionMotion(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
