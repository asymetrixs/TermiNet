using System.Threading;
using TermiNet.Event;

namespace TermiNet;

/// <summary>
/// Holds information about a <see cref="CancellationToken"/> to hook onto it
/// </summary>
internal class TerminationToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationToken"/> class.
    /// </summary>
    /// <param name="token">Cancellation token to attach to</param>
    /// <param name="terminateEventArgs">Terminate event args</param>
    internal TerminationToken(CancellationToken token, TerminateEventArgs terminateEventArgs)
    {
        this.Token = token;
        this.TerminateEvent = terminateEventArgs;
    }

    /// <summary>
    /// Cancellation token
    /// </summary>
    internal CancellationToken Token { get; init; }

    /// <summary>
    /// Exit code
    /// </summary>
    internal TerminateEventArgs TerminateEvent { get; init; }
}
