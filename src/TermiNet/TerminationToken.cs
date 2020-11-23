namespace TermiNet
{
    using System.Threading;

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
        /// <param name="exitCode">Exit code when token gets cancelled</param>
        /// <param name="exitMessage">Exit message when token gets cancelled</param>
        internal TerminationToken(CancellationToken token, int exitCode, string? exitMessage = null)
        {
            this.Token = token;
            this.ExitCode = exitCode;
            this.ExitMessage = exitMessage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Cancellation token
        /// </summary>
        internal CancellationToken Token { get; init; }

        /// <summary>
        /// Exit code
        /// </summary>
        internal int ExitCode { get; init; }

        /// <summary>
        /// Exit message
        /// </summary>
        internal string? ExitMessage { get; init; }

        #endregion
    }
}
