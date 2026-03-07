using SharpDX.Direct2D1;

namespace Ae.Rendering
{
    public class AeCriticalRenderTargets
    {
        public BitmapRenderTarget? IntermediateRenderTarget { get; set; }
        public WindowRenderTarget? ScreenRenderTarget { get; set; }
    }
}
