namespace Ae.Engine.Compiler
{
    /// <summary>
    /// Interface used to mark compiled user code.
    /// This allows us to identify compiled sprites vs. compiled scripts, and to handle them differently when needed.
    /// </summary>
    public interface IAeRuntimeCompiledCodeAsset
        : IAeRuntimeCompiled
    {
    }
}
