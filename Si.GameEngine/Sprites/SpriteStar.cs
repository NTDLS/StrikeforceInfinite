﻿using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.IO;

namespace Si.GameEngine.Sprites
{
    public class SpriteStar : SpriteBase
    {
        private const string _assetPath = @"Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteStar(Engine gameCore)
            : base(gameCore)
        {
            selectedImageIndex = SiRandom.Generator.Next(0, 1000) % _imageCount;
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = SiRandom.Generator.Next(0, gameCore.Display.TotalCanvasSize.Width);
            Y = SiRandom.Generator.Next(0, gameCore.Display.TotalCanvasSize.Height);

            Velocity.MaxSpeed = 3;

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ThrottlePercentage = SiRandom.Generator.Next(8, 10) / 10.0;
            }
            else
            {
                Velocity.ThrottlePercentage = SiRandom.Generator.Next(4, 8) / 10.0;
            }
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            //LocalX -= Velocity.MaxSpeed * Velocity.ThrottlePercentage;
            //LocalY -= Velocity.MaxSpeed * Velocity.ThrottlePercentage;

            if (_gameCore.Display.TotalCanvasBounds.IntersectsWith(Bounds) == false) //Remove off-screen stars.
            {
                QueueForDelete();
            }
        }
    }
}
