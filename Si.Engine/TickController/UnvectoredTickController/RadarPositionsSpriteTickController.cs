﻿using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.TickController.UnvectoredTickController
{
    public class RadarPositionsSpriteTickController : UnvectoredTickControllerBase<SpriteRadarPositionTextBlock>
    {
        private readonly SpriteManager _manager;

        public RadarPositionsSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine)
        {
            _manager = manager;
        }

        public override void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<SpriteRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<SpriteRadarPositionTextBlock>();
                var groups = new List<List<SpriteRadarPositionTextBlock>>();
                var radarTexts = Engine.Sprites.VisibleOfType<SpriteRadarPositionTextBlock>();

                foreach (var parent in radarTexts)
                {
                    if (accountedFor.Contains(parent) == false)
                    {
                        var group = new List<SpriteRadarPositionTextBlock>();
                        foreach (var child in radarTexts)
                        {
                            if (accountedFor.Contains(child) == false)
                            {
                                if (parent != child && parent.IntersectsAABB(child, new SiVector(100, 100)))
                                {
                                    group.Add(child);
                                    accountedFor.Add(child);
                                }
                            }
                        }
                        if (group.Count > 0)
                        {
                            group.Add(parent);
                            accountedFor.Add(parent);
                            groups.Add(group);
                        }
                    }
                }
                return groups;
            })();

            if (overlappingIndicators.Count > 0)
            {
                foreach (var group in overlappingIndicators)
                {
                    var min = group.Min(o => o.DistanceValue);
                    var max = group.Min(o => o.DistanceValue);

                    foreach (var member in group)
                    {
                        member.Visible = false;
                    }

                    group[0].Text = min.ToString("#,#") + "-" + max.ToString("#,#");
                    group[0].Visible = true;
                }
            }
        }

        #region Factories.

        public SpriteRadarPositionIndicator Add()
        {
            var obj = new SpriteRadarPositionIndicator(Engine);
            _manager.Add(obj);
            return obj;
        }

        #endregion
    }
}
