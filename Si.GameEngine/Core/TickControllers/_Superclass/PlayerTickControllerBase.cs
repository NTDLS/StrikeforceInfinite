﻿using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Core.TickControllers._Superclass
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" sprite.
    /// </summary>
    public class PlayerTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public GameEngineCore GameEngine { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual SiVector ExecuteWorldClockTick(double epochTimeepoch) => new();

        public PlayerTickControllerBase(GameEngineCore gameEngine)
        {
            GameEngine = gameEngine;
        }
    }
}
