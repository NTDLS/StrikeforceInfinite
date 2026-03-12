using SharpDX.Direct2D1;

namespace Ae.Engine.Rendering
{
    internal class AeCriticalRenderTargets
    {
        public BitmapRenderTarget? IntermediateRenderTarget { get; set; }
        public WindowRenderTarget? ScreenRenderTarget { get; set; }
    }
}
