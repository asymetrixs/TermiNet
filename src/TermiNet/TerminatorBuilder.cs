namespace TermiNet
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using TermiNet.Event;
    using TermiNet.Interfaces;
    using TermiNet.Validation;

    /// <summary>
    /// Configures and builds the <see cref="Terminator"/>
    /// </summary>
    public class TerminatorBuilder
    {
        #region Fields

        /// <summary>
        /// Store for <see cref="TerminateEventArgs"/>
        /// </summary>
        private Dictionary<Type, TerminateEventArgs> _registry = new();

        /// <summary>
        /// If true, CTRL+C is registered and handled by TermiNet
        /// </summary>
        private bool _registerCTRLC = false;

        /// <summary>
        /// Pre termination action is called before <see cref="TerminateEventHandler"/>
        /// </summary>
        private Action? preTerminationAction = null;

        /// <summary>
        /// Termination token
        /// </summary>
        private TerminationToken? _terminationToken = null;

        /// <summary>
        /// Default clean exit code
        /// </summary>
        public static readonly int DefaultCleanExitCode = 0;

        /// <summary>
        /// Default error exit code
        /// </summary>
        public static readonly int DefaultErrorExitCode = 1;

        /// <summary>
        /// Default SIGINT (CTRL-C) exit code
        /// </summary>
        public static readonly int CtrlCSIGINTExitCode = 130;

        /// <summary>
        /// Maximal exit code (number, not severity)
        /// </summary>
        public static readonly int MaxErrorExitCode = 255;

        /// <summary>
        /// Operating System Platform
        /// </summary>
        public static readonly OSPlatform OsPlatform;

        /// <summary>
        /// Validation level
        /// </summary>
        private readonly ValidationLevel _validationLevel;

        #endregion

        #region Constructor

        static TerminatorBuilder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OsPlatform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                OsPlatform = OSPlatform.FreeBSD;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OsPlatform = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OsPlatform = OSPlatform.Windows;
            }
            else
            {
                throw new SystemException("Cannot resolve OS platform");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminatorBuilder"/> class.
        /// </summary>
        /// <param name="validationLevel"></param>
        internal TerminatorBuilder(ValidationLevel validationLevel)
        {
            this._validationLevel = validationLevel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Event is called before application exists
        /// </summary>
        public event EventHandler<TerminateEventArgs>? TerminateEventHandler;

        #endregion

        #region Functions

        /// <summary>
        /// Creates a <see cref="TerminatorBuilder"/> instance.
        /// </summary>
        /// <param name="validationLevel">Set level of validation for the builder. Multiple values are possible, combine with | (pipe).
        /// <see cref="ValidationLevel.None"/> renders other flags obsolete.</param>
        /// <returns>Terminator builder instance</returns>
        public static TerminatorBuilder CreateBuilder(ValidationLevel validationLevel)
        {
            return new TerminatorBuilder(validationLevel);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers an exit code and message for an specific exception type
        /// </summary>
        /// <param name="terminateEventArgs">Arguments</param>
        public TerminatorBuilder Register<T>(TerminateEventArgs terminateEventArgs)
            where T : Exception
        {
            Validator.Validate(this._validationLevel, terminateEventArgs);

            var type = typeof(T);
            if (this._registry.ContainsKey(type))
            {
                throw new ArgumentException($"Type {nameof(type)} is already registered");
            }

            this._registry.Add(type, terminateEventArgs);

            return this;
        }

        /// <summary>
        /// Registers a cancellation token that terminates the application using TermiNet when it gets cancelled
        /// </summary>
        /// <param name="token">Token to attach to</param>
        /// <param name="terminateEventArgs">Termination event arguments</param>
        /// <returns></returns>
        public TerminatorBuilder RegisterCancellationToken(CancellationToken token, TerminateEventArgs terminateEventArgs)
        {
            if (this._terminationToken is not null)
            {
                throw new ArgumentException("A cancellation token is already registered.");
            }

            Validator.Validate(this._validationLevel, terminateEventArgs);

            this._terminationToken = new TerminationToken(token, terminateEventArgs);

            return this;
        }

        /// <summary>
        /// Build the <see cref="Terminator"/>
        /// </summary>
        public ITerminator Build()
        {
            Validator.Validate(this._validationLevel, this._registry);

            var terminator = new Terminator(
                this._registry,
                this._registerCTRLC,
                this.TerminateEventHandler,
                this.preTerminationAction,
                this._terminationToken);

            return terminator;
        }


        /// <summary>
        /// Registers CTRL+C (SIGINT) to terminate the application properly. Also executes <see cref="OnTerminating(TerminateEventArgs)" /> before.
        /// </summary>
        public TerminatorBuilder RegisterCtrlC()
        {
            this._registerCTRLC = true;

            return this;
        }

        /// <summary>
        /// Registers a pre termination action
        /// </summary>
        /// <param name="preTermination">Action to be called before termination event is called</param>
        /// <returns></returns>
        public TerminatorBuilder RegisterPreTerminationAction(Action preTermination)
        {
            if (this.preTerminationAction is not null)
            {
                throw new NotSupportedException("An action is already registered");
            }

            this.preTerminationAction = preTermination;

            return this;
        }

        #endregion
    }
}
