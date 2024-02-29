﻿using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.GameEngine.Core.TickControllers
{
    public class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerupBase>
    {
        public PowerupsSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(double epoch, SiVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
            }
        }

        public T Create<T>(double x, double y) where T : SpritePowerupBase
        {
            object[] param = { GameEngine };
            var obj = (SpritePowerupBase)Activator.CreateInstance(typeof(T), param);
            obj.Location = new SiVector(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
