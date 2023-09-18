﻿using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.TickHandlers.Interfaces;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickHandlers
{
    internal class TextBlockSpriteTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteTextBlock => _controller.VisibleOfType<subType>();
        public List<SpriteTextBlock> Visible() => _controller.VisibleOfType<SpriteTextBlock>();
        public List<subType> OfType<subType>() where subType : SpriteTextBlock => _controller.OfType<subType>();

        public TextBlockSpriteTickHandler(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public SpriteRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, HgPoint location)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteRadarPositionTextBlock(_core, format, color, location);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, HgPoint location, bool isPositionStatic)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteTextBlock(_core, format, color, location, isPositionStatic);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteTextBlock Create(TextFormat format, SolidColorBrush color, HgPoint location, bool isPositionStatic, string name)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteTextBlock(_core, format, color, location, isPositionStatic);
                obj.Name = name;
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteTextBlock obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}