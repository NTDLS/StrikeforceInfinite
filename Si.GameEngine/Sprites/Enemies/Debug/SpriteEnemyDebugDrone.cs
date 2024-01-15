﻿using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Enemies.Debug
{
    internal class SpriteEnemyDebugDrone : SpriteEnemyDebug, ISpriteDrone
    {
        public SpriteEnemyDebugDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}