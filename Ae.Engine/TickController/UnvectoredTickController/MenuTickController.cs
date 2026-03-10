using Ae.Engine.Menu;
using SharpDX.Direct2D1;
using System.Collections.Generic;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    public class MenuTickController
        : UnvectoredTickControllerBase<AeMenu>
    {
        public delegate void CollectionAccessor(List<AeMenu> sprites);
        public delegate T CollectionAccessorT<T>(List<AeMenu> sprites);

        private AeMenu? _current = null;
        public AeMenu? Current { get => _current; }

        public MenuTickController(AeEngine engine)
            : base(engine) { }

        public void Render(RenderTarget renderTarget, float epoch)
            => _current?.Render(renderTarget, epoch);

        public void Show(AeMenu menu)
        {
            Unload(_current);
            _current = menu;
        }

        public void Unload(AeMenu? menu)
        {
            if (_current == menu)
            {
                //QueuedForDeletion is set in MenuBase.Close, so if it is true, then MenuBase.Close has already been called.
                if (_current?.QueuedForDeletion == false)
                {
                    _current.Close();
                }
                _current = null;
            }
        }

        public override void ExecuteWorldClockTick() => _current?.HandleInput();
    }
}
