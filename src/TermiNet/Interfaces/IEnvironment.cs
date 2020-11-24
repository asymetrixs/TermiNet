namespace TermiNet.Interfaces
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interface proxying the environment
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Exits
        /// </summary>
        /// <param name="code"></param>
#if NET5_0
        [DoesNotReturn]
#endif
        void Exit(int code);

        /// <summary>
        /// Operating System Platform
        /// </summary>
        OSPlatform OSPlatform { get; }
    }
}
