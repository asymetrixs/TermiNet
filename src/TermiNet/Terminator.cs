namespace TermiNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using TermiNet.Event;

    public class Terminator : ITerminator
    {
        #region Fields

        /// <summary>
        /// Store for <see cref="TerminateEventArgs"/>
        /// </summary>
        private Dictionary<Type, TerminateEventArgs> _registry = new();

        /// <summary>
        /// Event is called before application exists
        /// </summary>
        private EventHandler<TerminateEventArgs>? _terminateEventHandler = null;

        /// <summary>
        /// Pre termination action is called before <see cref="_terminateEventHandler"/>
        /// </summary>
        private Action? _preTerminationAction = null;

        /// <summary>
        /// Termination token
        /// </summary>
        private TerminationToken? _terminationToken = null;

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

            #region OS Platform specifics

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.DefaultCleanExitCode = 0;
                this.DefaultErrorExitCode = 1;
                this.CtrlCSIGINTExitCode = 130;
                this.MaxErrorExitCode = 255;
                this.OSPlatform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                this.DefaultCleanExitCode = 0;
                this.DefaultErrorExitCode = 1;
                this.CtrlCSIGINTExitCode = 130;
                this.MaxErrorExitCode = 255;
                this.OSPlatform = OSPlatform.FreeBSD;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.DefaultCleanExitCode = 0;
                this.DefaultErrorExitCode = 1;
                this.CtrlCSIGINTExitCode = 130;
                this.MaxErrorExitCode = 255;
                this.OSPlatform = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                /// TODO
                this.DefaultCleanExitCode = 0;
                this.DefaultErrorExitCode = 1;
                this.CtrlCSIGINTExitCode = 0;
                this.MaxErrorExitCode = 9999;
                this.OSPlatform = OSPlatform.Windows;
            }
            else
            {
                throw new SystemException("Cannot resolve OS platform");
            }

            #endregion

            if (registerCtrlC)
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
            }

            this._registry = errorCodeRegistry;

            this._terminateEventHandler = terminateEventHandlers;

            this._preTerminationAction = preTerminationAction;

            this._terminationToken = terminationToken;

            if (this._terminationToken is not null)
            {
                // Registers a callback to terminate the application
                this._terminationToken.Token.Register(() =>
                {
                    _Terminate(new TerminateEventArgs(this._terminationToken.ExitCode, this._terminationToken.ExitMessage));
                });
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Default OS platform exit code for CTRL+C (SIGINT)
        /// </summary>
        public int CtrlCSIGINTExitCode { get; private set; }

        /// <summary>
        /// Default OS platform exit code on clean exit
        /// </summary>
        public int DefaultCleanExitCode { get; private set; }

        /// <summary>
        /// Default OS platform exit code on error
        /// </summary>
        public int DefaultErrorExitCode { get; private set; }

        /// <summary>
        /// Maximum possible error code, e.g. on Linnux 255
        /// </summary>
        public int MaxErrorExitCode { get; private set; }

        /// <summary>
        /// OS platform
        /// </summary>
        public OSPlatform OSPlatform { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Terminates the app signaling a clean exit
        /// </summary>
        [DoesNotReturn]
        public void Terminate()
        {
            _Terminate(new TerminateEventArgs(this.DefaultCleanExitCode, null));
        }

        /// <summary>
        /// Terminates the app
        /// </summary>
        /// <param name="exitCode">Exit code</param>
        /// <param name="exitMessage">Exit message for use in <see cref="OnTerminating(TerminateEventArgs)"/></param>
        [DoesNotReturn]
        public void Terminate(int exitCode, string? exitMessage = null)
        {
            if (exitCode > this.MaxErrorExitCode)
            {
                exitCode = this.MaxErrorExitCode;
            }

            _Terminate(new TerminateEventArgs(exitCode, exitMessage));
        }

        /// <summary>
        /// Terminates the app. Uses registered information or <see cref="DefaultErrorExitCode"/> and the exceptions name and message.
        /// </summary>
        /// <param name="e">Exception that will be used to determine the exit code (if registered).</param>
        [DoesNotReturn]
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
        [DoesNotReturn]
        public void Terminate(TerminateEventArgs exitEventArgs)
        {
            _Terminate(exitEventArgs);
        }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        internal void Validate()
        {
            if (this._registry.Any(item => item.Value.ExitCode < this.DefaultCleanExitCode || item.Value.ExitCode > this.MaxErrorExitCode)
                || this._terminationToken?.ExitCode < this.DefaultCleanExitCode || this._terminationToken?.ExitCode > this.MaxErrorExitCode)
            {
                throw new ArgumentOutOfRangeException($"Exit codes are out of range for OS plattform {this.OSPlatform}. Exit code must be between {this.DefaultCleanExitCode} (clean exit) and {this.MaxErrorExitCode}");
            }
        }

        /// <summary>
        /// Is called before the app exits and could be used for instance for logging
        /// </summary>
        /// <param name="e">Event</param>
        private void OnTerminating(TerminateEventArgs e)
        {
            _terminateEventHandler?.Invoke(null, e);
        }

        /// <summary>
        /// Event handler catching CTRL+C (SIGINT)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DoesNotReturn]
        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            Terminate(new TerminateEventArgs(this.CtrlCSIGINTExitCode));
        }

        /// <summary>
        /// Actually terminates the app
        /// </summary>
        /// <param name="exitEventArgs"></param>
        [DoesNotReturn]
        private void _Terminate(TerminateEventArgs exitEventArgs)
        {
            if (this._preTerminationAction is not null)
            {
                this._preTerminationAction();
            }

            OnTerminating(exitEventArgs);

            Environment.Exit(exitEventArgs.ExitCode);
        }

        #endregion
    }
}
