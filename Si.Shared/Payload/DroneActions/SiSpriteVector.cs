﻿namespace Si.Shared.Payload.DroneActions
{
    public class SiSpriteVector: SiSpriteAction
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }
        public double ThrottlePercentage { get; set; }
        public double BoostPercentage { get; set; }
    }
}