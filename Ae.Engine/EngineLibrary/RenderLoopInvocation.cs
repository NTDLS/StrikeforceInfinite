using System;
using System.Threading;

namespace Ae.Engine.EngineLibrary
{
    public class RenderLoopInvocation
    {
        public AutoResetEvent Event = new(false);
        public Guid Id { get; set; }
        public Action Action { get; set; }
        public SiEngine Engine { get; set; }

        public RenderLoopInvocation(SiEngine engine, Action action)
        {
            Id = Guid.NewGuid();
            Engine = engine;
            Action = action;
        }

        public void Execute()
        {
            Action();
            Event.Set();
            Engine.RemoveRenderLoopInvocation(this);
        }

        public void Wait()
        {
            Event.WaitOne();
        }
    }
}
