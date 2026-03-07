using SharpDX.Direct2D1;

namespace Ae.Rendering
{
    public class SiCriticalRenderTargets
    {
        public BitmapRenderTarget? IntermediateRenderTarget { get; set; }
        public WindowRenderTarget? ScreenRenderTarget { get; set; }
    }
}
