namespace Si.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class SiSpriteActionSpawn : SiSpriteAction
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float OrientationDegreesSigned { get; set; }
        public float Throttle { get; set; }
        //public float BoostPercentage { get; set; }
        public float Speed { get; set; }
        public float RotationSpeed { get; set; }
        //public float Boost { get; set; }

        public string SpriteType { get; set; }

        public SiSpriteActionSpawn(uint spriteUID, string spriteType)
            : base(spriteUID)
        {
            SpriteType = spriteType;
        }
    }
}
