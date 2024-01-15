﻿using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDebugPlayerDrone : SpriteDebugPlayer, ISpriteDrone
    {
        public SpriteDebugPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
