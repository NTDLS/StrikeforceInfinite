namespace Ae.Engine.Compiler
{
    /// <summary>
    /// Interface used to mark compiled sprites.
    /// This allows us to identify compiled sprites vs. compiled scripts, and to handle them differently when needed.
    /// </summary>
    public interface IAeRuntimeCompiledSpriteAsset
        : IAeRuntimeCompiled
    {
    }
}
