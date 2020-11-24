namespace TermiNet
{
    using System;
    using System.Runtime.InteropServices;
    using TermiNet.Event;

    public interface ITerminator
    {
        /// <summary>
        /// Default OS platform exit code for CTRL+C (SIGINT)
        /// </summary>
        int CtrlCSIGINTExitCode { get; }

        /// <summary>
        /// Default OS platform exit code on clean exit
        /// </summary>
        int DefaultCleanExitCode { get; }

        /// <summary>
        /// Default OS platform exit code on error
        /// </summary>
        int DefaultErrorExitCode { get; }

        /// <summary>
        /// Maximum possible error code, e.g. on Linnux 255
        /// </summary>
        int MaxErrorExitCode { get; }

        /// <summary>
        /// OS platform
        /// </summary>
        OSPlatform OsPlatform { get; }

        /// <summary>
        /// Terminates the app signaling a clean exit
        /// </summary>
        void Terminate();

        /// <summary>
        /// Terminates the app
        /// </summary>
        /// <param name="exitCode">Exit code</param>
        /// <param name="exitMessage">Exit message for use in <see cref="OnTerminating(TerminateEventArgs)"/></param>
        void Terminate(int exitCode, string? exitMessage = null);

        /// <summary>
        /// Terminates the app. Uses registered information or <see cref="DefaultErrorExitCode"/> and the exceptions name and message.
        /// </summary>
        /// <param name="e">Exception that will be used to determine the exit code (if registered).</param>
        void Terminate(Exception e);

        /// <summary>
        /// Terminates the app with the specified <paramref name="exitEventArgs"/>
        /// </summary>
        /// <param name="exitEventArgs">Event with exit code and message.</param>
        void Terminate(TerminateEventArgs exitEventArgs);
    }
}
