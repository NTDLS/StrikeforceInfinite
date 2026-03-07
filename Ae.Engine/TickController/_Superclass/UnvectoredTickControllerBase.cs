namespace Ae.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T>
        : ITickController<T> where T : class
    {
        public AeEngine Engine { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(AeEngine engine)
        {
            Engine = engine;
        }
    }
}
