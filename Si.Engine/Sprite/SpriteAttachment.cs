﻿using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite
{
    public class SpriteAttachment : SpriteInteractiveBase
    {
        private SpriteInteractiveBase _rootOwner = null;

        public SpriteAttachment(EngineCore engine)
            : base(engine)
        {
            Initialize();
            Velocity = new SiVelocity();
        }

        public SpriteAttachment(EngineCore engine, string imagePath)
            : base(engine)
        {
            Initialize(imagePath);
            Velocity = new SiVelocity();
        }

        /// <summary>
        /// Gets and caches the root owner of this attachement.
        /// </summary>
        /// <returns></returns>
        public SpriteInteractiveBase OwnerSprite
        {
            get
            {
                if (_rootOwner == null)
                {
                    _rootOwner = this;

                    do
                    {
                        _rootOwner = _engine.Sprites.GetSpriteByOwner<SpriteInteractiveBase>(_rootOwner.OwnerUID);

                    } while (_rootOwner.OwnerUID != 0);
                }
                return _rootOwner;
            }
        }
    }
}
