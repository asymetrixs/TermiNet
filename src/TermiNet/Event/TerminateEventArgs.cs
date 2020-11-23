﻿namespace TermiNet.Event
{
    using System;

    /// <summary>
    /// Event that is fired when an app terminates
    /// </summary>
    public class TerminateEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateEventArgs"/> class.
        /// </summary>
        /// <param name="exitCode">Use 0 for success. On Unix restrict codes to range from 150 to 250 for unsuccessful exit. On Windows, no restrictions are in place.</param>
        /// <param name="exitMessage"></param>
        public TerminateEventArgs(int exitCode, string? exitMessage = null)
        {
            this.ExitCode = exitCode;
            this.ExitMessage = exitMessage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Exit code
        /// </summary>
        public int ExitCode { get; init; }

        /// <summary>
        /// Exit message, optional
        /// </summary>
        public string? ExitMessage { get; init; }

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
