using System;
using System.Threading;

namespace Si.Engine.EngineLibrary
{
    public class RenderLoopInvocation
    {
        public AutoResetEvent Event = new(false);
        public Guid Id { get; set; }
        public Action Action { get; set; }
        public EngineCore Engine { get; set; }

        public RenderLoopInvocation(EngineCore engine, Action action)
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
