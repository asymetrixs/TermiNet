namespace TermiNet.Interfaces
{
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
        void Exit(int code);

        /// <summary>
        /// Operating System Platform
        /// </summary>
        OSPlatform OSPlatform { get; }
    }
}
