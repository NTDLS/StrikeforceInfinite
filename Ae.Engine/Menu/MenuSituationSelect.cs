using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Menu._Superclass;
using Ae.Engine.Situation._Superclass;
using Ae.Engine.Sprite.TextBlock;
using System.Linq;

namespace Ae.Engine.Menu
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a situation.
    /// </summary>
    internal class MenuSituationSelect
        : MenuBase
    {
        private readonly SpriteMenuItem _situationBlurb;

        public MenuSituationSelect(AeEngine engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new AeVector(offsetX, offsetY), "Select a Situation");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;

            offsetX = currentScaledScreenBounds.X + 40;
            offsetY += itemTitle.Height;

            _situationBlurb = AddTextBlock(new AeVector(offsetX, offsetY), "");
            _situationBlurb.X = offsetX + 300;
            _situationBlurb.Y = offsetY - _situationBlurb.Size.Height;

            //Use reflection to get a list of possible situation types.
            var situationTypes = AeReflection.GetSubClassesOf<SituationBase>().OrderBy(o => o.Name).ToList();

            //Move the debug situation to the top of the list.
            var situations = situationTypes.Where(o => o.Name.Contains("Debug")).FirstOrDefault();
            if (situations != null)
            {
                situationTypes.Remove(situations);
                situationTypes.Insert(0, situations);
            }

            foreach (var situationType in situationTypes)
            {
                var situationInstance = AeReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { engine, });

                var menuItem = AddSelectableItem(new AeVector(offsetX + 25, offsetY), situationInstance.Name, $"> {situationInstance.Name}");

                menuItem.UserData = situationInstance;

                menuItem.Y -= menuItem.Size.Height / 2;
                offsetY += 50;
            }

            OnSelectionChanged += SituationSelectMenu_OnSelectionChanged;
            OnExecuteSelection += SituationSelectMenu_OnExecuteSelection;
            OnEscape += SpMenuSituationSelect_OnEscape;

            VisibleSelectableItems().First().Selected = true;
        }

        private bool SpMenuSituationSelect_OnEscape()
        {
            _engine.Menus.Show(new MenuStartNewGame(_engine));
            return true;
        }

        private bool SituationSelectMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _engine.ResetGame();
                _engine.Situations.Select(situation.GetType().Name);
                _engine.Menus.Show(new MenuSelectLoadout(_engine));
            }
            return true;
        }

        private void SituationSelectMenu_OnSelectionChanged(SpriteMenuItem item)
        {
            if (item.UserData is SituationBase situation)
            {
                _situationBlurb.Text = situation.Description;
            }
        }
    }
}
