using System;
using System.Runtime.CompilerServices;

namespace Ae.Engine.Helpers
{
    internal class AeThreading
    {
        /// <summary>
        /// Returns a value allowing for only single threaded access.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Interlock<T, P>(P p, Func<P, T> proc) where P : class
        {
            lock (p)
            {
                return proc(p);
            }
        }
    }
}
