using System;
using System.Threading;
using static Si.Library.SiConstants;

namespace Si.Engine.EngineLibrary
{
    public class RenderLoopInterjection
    {
        public AutoResetEvent Event = new(false);
        public Guid Id { get; set; }
        public Action Action { get; set; }
        public EngineCore Engine { get; set; }
        public RenderLoopInterjectionLifetime Lifetime { get; set; }

        public RenderLoopInterjection(EngineCore engine, RenderLoopInterjectionLifetime lifetime, Action action)
        {
            Id = Guid.NewGuid();
            Engine = engine;
            Lifetime = lifetime;
            Action = action;
        }

        public void Execute()
        {
            Action();
            Event.Set();
            if (Lifetime == RenderLoopInterjectionLifetime.Once)
            {
                Engine.RemoveRenderLoopInterjection(this);
            }
        }

        public void Wait()
        {
            Event.WaitOne();
        }
    }
}
