namespace TermiNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using TermiNet.Event;
    using TermiNet.Interfaces;

    /// <summary>
    /// Is used by an application to initiate termination of the application
    /// </summary>
    public class Terminator : ITerminator
    {
        #region Fields

        /// <summary>
        /// Store for <see cref="TerminateEventArgs"/>
        /// </summary>
        private readonly Dictionary<Type, TerminateEventArgs> _registry = new();

        /// <summary>
        /// Event is called before application exists
        /// </summary>
        private readonly EventHandler<TerminateEventArgs>? _terminateEventHandler = null;

        /// <summary>
        /// Pre termination action is called before <see cref="_terminateEventHandler"/>
        /// </summary>
        private readonly Action? _preTerminationAction = null;

        /// <summary>
        /// Termination token
        /// </summary>
        private readonly TerminationToken? _terminationToken = null;

        /// <summary>
        /// Environment
        /// </summary>
        private IEnvironment _environment = new Environment();

        #endregion

        #region Constructor

        /// <summary>
        /// Sets up OS platform specific parameters
        /// </summary>
        /// <param name="errorCodeRegistry"></param>
        /// <param name="registerCtrlC"></param>
        /// <param name="terminateEventHandlers"></param>
        /// <param name="preTerminationAction"></param>
        internal Terminator(Dictionary<Type, TerminateEventArgs> errorCodeRegistry,
            bool registerCtrlC = false,
            EventHandler<TerminateEventArgs>? terminateEventHandlers = null,
            Action? preTerminationAction = null,
            TerminationToken? terminationToken = null)
        {
            _ = errorCodeRegistry ?? throw new ArgumentNullException(nameof(errorCodeRegistry));

            if (registerCtrlC)
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
            }

            this._registry = errorCodeRegistry;
            this._terminateEventHandler = terminateEventHandlers;
            this._preTerminationAction = preTerminationAction;
            this._terminationToken = terminationToken;

            // Termination token
            if (this._terminationToken is not null)
            {
                // Registers a callback to terminate the application
                this._terminationToken.Token.Register(() =>
                {
                    _Terminate(this._terminationToken.TerminateEvent);
                });
            }

            // Get platform specific values
            this.CtrlCSIGINTExitCode = TerminatorBuilder.CtrlCSIGINTExitCode;
            this.DefaultCleanExitCode = TerminatorBuilder.DefaultCleanExitCode;
            this.DefaultErrorExitCode = TerminatorBuilder.DefaultErrorExitCode;
            this.MaxErrorExitCode = TerminatorBuilder.MaxErrorExitCode;
            this.OsPlatform = TerminatorBuilder.OsPlatform;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Default OS platform exit code for CTRL+C (SIGINT)
        /// </summary>
#if NET5_0
        public int CtrlCSIGINTExitCode { get; init; }
#else
        public int CtrlCSIGINTExitCode { get; private set; }
#endif

        /// <summary>
        /// Default OS platform exit code on clean exit
        /// </summary>
#if NET5_0
        public int DefaultCleanExitCode { get; init; }
#else
        public int DefaultCleanExitCode { get; private set; }
#endif

        /// <summary>
        /// Default OS platform exit code on error
        /// </summary>
#if NET5_0
        public int DefaultErrorExitCode { get; init; }
#else
        public int DefaultErrorExitCode { get; private set; }
#endif

        /// <summary>
        /// Maximum possible error code, e.g. on Linnux 255
        /// </summary>
#if NET5_0
        public int MaxErrorExitCode { get; init; }
#else
        public int MaxErrorExitCode { get; private set; }
#endif

        /// <summary>
        /// OS platform
        /// </summary>
#if NET5_0
        public OSPlatform OsPlatform { get; init; }
#else
        public OSPlatform OsPlatform { get; private set; }
#endif

        #endregion

        #region Methods

        /// <summary>
        /// Terminates the app signaling a clean exit
        /// </summary>
#if NET5_0
        [DoesNotReturn]
#endif
        public void Terminate()
        {
            _Terminate(new TerminateEventArgs(this.DefaultCleanExitCode, null));
        }

        /// <summary>
        /// Terminates the app. Sets the <paramref name="exitCode"/> to <see cref="Terminator.MaxErrorExitCode"/>
        /// in case it greater than <see cref="Terminator.MaxErrorExitCode"/>
        /// </summary>
        /// <param name="exitCode">Use 0 for success. On Unix restrict codes to range from 170 to 250
        /// for unsuccessful exit. On Windows, no restrictions are in place.</param>
        /// <param name="exitMessage">Exit message for use in <see cref="OnTerminating(TerminateEventArgs)"/></param>
#if NET5_0
        [DoesNotReturn]
#endif
        public void Terminate(int exitCode, string? exitMessage = null)
        {
            if (exitCode > this.MaxErrorExitCode)
            {
                exitCode = this.MaxErrorExitCode;
            }

            _Terminate(new TerminateEventArgs(exitCode, exitMessage));
        }

        /// <summary>
        /// Terminates the app. Uses registered information or <see cref="this.DefaultErrorExitCode"/> and the exceptions name and message.
        /// </summary>
        /// <param name="e">Exception that will be used to determine the exit code (if registered).</param>
#if NET5_0
        [DoesNotReturn]
#endif
        public void Terminate(Exception e)
        {
            if (_registry.TryGetValue(e.GetType(), out TerminateEventArgs? exitEventArgs))
            {
                _Terminate(exitEventArgs);
            }
            else
            {
                _Terminate(new TerminateEventArgs(this.DefaultErrorExitCode, $"Unspecified error. {e.GetType().Name}: {e.Message}"));
            }
        }

        /// <summary>
        /// Terminates the app with the specified <paramref name="exitEventArgs"/>
        /// </summary>
        /// <param name="exitEventArgs">Event with exit code and message.</param>
#if NET5_0
        [DoesNotReturn]
#endif
        public void Terminate(TerminateEventArgs exitEventArgs)
        {
            _Terminate(exitEventArgs);
        }


        /// <summary>
        /// Is called before the app exits and could be used for instance for logging
        /// </summary>
        /// <param name="e">Event</param>
        private void OnTerminating(TerminateEventArgs e)
        {
            _terminateEventHandler?.Invoke(this, e);
        }

        /// <summary>
        /// Event handler catching CTRL+C (SIGINT)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if NET5_0
        [DoesNotReturn]
#endif
        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Terminate(new TerminateEventArgs(this.CtrlCSIGINTExitCode));
        }

        /// <summary>
        /// Actually terminates the app
        /// </summary>
        /// <param name="exitEventArgs"></param>
#if NET5_0
        [DoesNotReturn]
#endif
        private void _Terminate(TerminateEventArgs exitEventArgs)
        {
            if (this._preTerminationAction is not null)
            {
                this._preTerminationAction();
            }

            OnTerminating(exitEventArgs);

            _environment.Exit(exitEventArgs.ExitCode);
        }

#endregion
    }
}
