namespace TermiNet.Event
{
    using System;
    using TermiNet.ReservedCodes;

    /// <summary>
    /// Event that is fired when an app terminates
    /// </summary>
    public class TerminateEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateEventArgs"/> class.
        /// Sets the <paramref name="exitCode"/> to <see cref="TerminatorBuilder.MaxErrorExitCode"/> in case
        /// it greater than <see cref="TerminatorBuilder.MaxErrorExitCode"/>
        /// </summary>
        /// <param name="exitCode">Use 0 for success. On Unix restrict codes to range from 170 to 250
        /// for unsuccessful exit or use <see cref="UnixCode"/> if a reserved code fits your needs.
        /// On Windows, no restrictions are in place.</param>
        /// <param name="exitMessage"></param>
        public TerminateEventArgs(int exitCode, string? exitMessage = null)
        {
            this.ExitCode = exitCode;
            this.ExitMessage = exitMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateEventArgs"/> class.
        /// Sets the <paramref name="exitCode"/> to <see cref="TerminatorBuilder.MaxErrorExitCode"/> in case
        /// it greater than <see cref="TerminatorBuilder.MaxErrorExitCode"/>
        /// </summary>
        /// <param name="exitCode">Use <see cref="UnixCode.OK"/> for success. On Unix restrict codes to range from 170 to 250
        /// for unsuccessful exit or use <see cref="UnixCode"/> if a reserved code fits your needs.
        /// On Windows, no restrictions are in place.
        /// A code greater than <see cref="TerminatorBuilder.MaxErrorExitCode"/>
        /// will be set to <see cref="TerminatorBuilder.MaxErrorExitCode"/>.</param>
        /// <param name="exitMessage"></param>
        public TerminateEventArgs(UnixCode exitCode, string? exitMessage = null)
        {
            this.ExitCode = (int)exitCode;
            this.ExitMessage = exitMessage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Exit code
        /// </summary>
#if NET5_0
        public int ExitCode { get; init; }
#else
        public int ExitCode { get; private set; }
#endif

        /// <summary>
        /// Exit message, optional
        /// </summary>
#if NET5_0
        public string? ExitMessage { get; init; }
#else
        public string? ExitMessage { get; private set; }
#endif
        #endregion

        #region Methods

        /// <summary>
        /// ToString() Override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.ExitCode}: {this.ExitMessage}";
        }

        #endregion
    }
}
