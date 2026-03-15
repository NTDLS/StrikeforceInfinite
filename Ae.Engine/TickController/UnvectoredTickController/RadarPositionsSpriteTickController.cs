using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.TextBlock;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    /// <summary>
    /// Controls the tick-based updates for radar position sprites, managing their visibility and grouping based on
    /// spatial overlap.
    /// </summary>
    /// <remarks>This controller is responsible for handling the logic that determines which radar position
    /// indicators are visible and how overlapping indicators are grouped and displayed. It operates within the world
    /// clock tick cycle and interacts with the sprite manager to add new radar position indicators. Use this controller
    /// when you need to manage radar position text blocks in a tick-driven environment, ensuring that overlapping
    /// indicators are consolidated for clarity.</remarks>
    public class RadarPositionsSpriteTickController
        : UnvectoredTickControllerBase<AeSpriteRadarPositionTextBlock>
    {
        private readonly SpriteManager _manager;

        /// <summary>
        /// Initializes a new instance of the RadarPositionsSpriteTickController class using the specified engine and
        /// sprite manager.
        /// </summary>
        /// <param name="engine">The engine instance used to drive the controller's operations.</param>
        /// <param name="manager">The sprite manager responsible for managing radar position sprites.</param>
        public RadarPositionsSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine)
        {
            _manager = manager;
        }

        /// <summary>
        /// Processes the world clock tick by updating the visibility and text of overlapping radar position indicators.
        /// </summary>
        /// <remarks>This method identifies groups of radar position text blocks that overlap within a
        /// specified area and consolidates their display. Only the first indicator in each group is made visible, with
        /// its text updated to reflect the range of distance values for the group. This helps reduce visual clutter in
        /// the radar display. The method should be called once per world clock tick to ensure accurate and efficient
        /// indicator management.</remarks>
        public override void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<AeSpriteRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<AeSpriteRadarPositionTextBlock>();
                var groups = new List<List<AeSpriteRadarPositionTextBlock>>();
                var radarTexts = Engine.Sprites.VisibleOfType<AeSpriteRadarPositionTextBlock>();

                foreach (var parent in radarTexts)
                {
                    if (accountedFor.Contains(parent) == false)
                    {
                        var group = new List<AeSpriteRadarPositionTextBlock>();
                        foreach (var child in radarTexts)
                        {
                            if (accountedFor.Contains(child) == false)
                            {
                                if (parent != child && parent.IntersectsAABB(child, new AeVector(100, 100)))
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
                        member.IsVisible = false;
                    }

                    group[0].Text = min.ToString("#,#") + "-" + max.ToString("#,#");
                    group[0].IsVisible = true;
                }
            }
        }

        #region Factories.

        /// <summary>
        /// Creates and adds a new radar position indicator sprite to the manager.
        /// </summary>
        /// <remarks>The created sprite uses the "Sprites/Radar Indicator/16x16" asset and is managed by
        /// the internal manager. Use the returned object to configure or manipulate the indicator as needed.</remarks>
        /// <returns>An instance of AeSpriteRadarPositionIndicator representing the newly added radar position indicator sprite.</returns>
        public AeSpriteRadarPositionIndicator Add()
            => _manager.Add<AeSpriteRadarPositionIndicator>("Sprites/Radar Indicator/16x16");

        #endregion
    }
}
