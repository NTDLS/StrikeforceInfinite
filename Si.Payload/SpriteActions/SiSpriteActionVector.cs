﻿namespace Si.Library.Payload.SpriteActions
{
    public class SiSpriteActionVector : SiSpriteAction
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }
        public double ThrottlePercentage { get; set; }
        public double BoostPercentage { get; set; }
        public double Speed { get; set; }
        public double Boost { get; set; }

        public SiSpriteActionVector(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
