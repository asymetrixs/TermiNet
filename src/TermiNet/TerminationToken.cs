namespace TermiNet
{
    using System.Threading;
    using TermiNet.Event;

    /// <summary>
    /// Holds information about a <see cref="CancellationToken"/> to hook onto it
    /// </summary>
    internal class TerminationToken
    {
        #region Constructor

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

        #endregion

        #region Properties

        /// <summary>
        /// Cancellation token
        /// </summary>
#if NET5_0
        internal CancellationToken Token { get; init; }
#else
        internal CancellationToken Token { get; private set; }
#endif

        /// <summary>
        /// Exit code
        /// </summary>
#if NET5_0
        internal TerminateEventArgs TerminateEvent { get; init; }
#else
        internal TerminateEventArgs TerminateEvent { get; private set; }
#endif

        #endregion
    }
}
