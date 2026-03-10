namespace Ae.Engine.TickController
{
    /// <summary>
    /// All tick-controllers are derived from this interface. They control the "movement of time" for all objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITickController<T> where T : class
    {
    }
}
