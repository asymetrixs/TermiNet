using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace TermiNet.Interfaces;

/// <summary>
/// Interface proxying the environment
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Exits
    /// </summary>
    /// <param name="code"></param>
    [DoesNotReturn]
    void Exit(int code);

    /// <summary>
    /// Operating System Platform
    /// </summary>
    OSPlatform OSPlatform { get; }
}
