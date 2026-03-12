using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using System;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a skybox sprite asset used to render background environments in the engine.
    /// </summary>
    /// <remarks>This class specializes the base sprite functionality for skybox rendering, enabling parallax
    /// effects and background movement relative to camera displacement. Use this type to display immersive sky
    /// backgrounds in scenes. Thread safety is not guaranteed; access from multiple threads should be
    /// synchronized.</remarks>
    [AssetClass("Skybox", "", AeBaseAssetType.Image, true)]
    public class AeSpriteSkyBox
        : AeSprite
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteSkyBox class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and game logic for the sky box.</param>
        /// <param name="assetKey">The key identifying the asset to be used for the sky box images.</param>
        public AeSpriteSkyBox(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            //selectedImageIndex = SiRandom.Between(0, _imageCount - 1);

            //X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            //Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);
            //Z = int.MinValue;

            //Speed = 0.10f;

            //if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            //{
            //    Throttle = SiRandom.Between(8, 10) / 10.0f;
            //}
            //else
            //{
            //    Throttle = SiRandom.Between(4, 8) / 10.0f;
            //}
        }

        private AeVector _currentOffset = new();
        private readonly float _maxOffset = 200;

        /// <summary>
        /// Applies a motion offset to the camera based on the specified displacement vector for the given epoch.
        /// </summary>
        /// <remarks>The method adjusts the camera's position relative to the center of the current
        /// screen, limiting the offset to a maximum value. If the displacement vector is zero, no motion is
        /// applied.</remarks>
        /// <param name="epoch">The current time or frame index at which the motion is applied. Used to synchronize camera movement with the
        /// simulation or rendering cycle.</param>
        /// <param name="cameraDisplacement">A vector representing the desired displacement of the camera. The vector is normalized and used to
        /// incrementally adjust the camera's offset.</param>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            if (cameraDisplacement.Sum() != 0)
            {
                var offsetIncrement = new AeVector(cameraDisplacement.Normalize());

                offsetIncrement.X *= (1 - (Math.Abs(_currentOffset.X) / _maxOffset));
                offsetIncrement.Y *= (1 - (Math.Abs(_currentOffset.Y) / _maxOffset));

                _currentOffset += offsetIncrement;

                Location = Engine.Display.CenterOfCurrentScreen - _currentOffset;
            }
        }
    }
}
