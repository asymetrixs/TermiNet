namespace TermiNet
{
    using System;
    using System.Collections.Generic;
    using TermiNet.Event;

    /// <summary>
    /// Static class setting up the <see cref="Terminator"/>
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
        /// <returns></returns>
        public static TerminatorBuilder CreateBuilder()
        {
            return new TerminatorBuilder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers an exit code and message for an specific exception type
        /// </summary>
        /// <param name="exitCode">Exit Code</param>
        /// <param name="exitMessage">Exit Message</param>
        public TerminatorBuilder Register<T>(int exitCode, string? exitMessage = null)
            where T : Exception
        {
            var type = typeof(T);
            if (this._registry.ContainsKey(type))
            {
                throw new ArgumentException($"Type {nameof(type)} is already registered");
            }

            this._registry.Add(type, new TerminateEventArgs(exitCode, exitMessage));

            return this;
        }

        /// <summary>
        /// Registers <see cref="TerminateEventArgs"/> for an exception type
        /// </summary>
        /// <typeparam name="T">Type of the exception</typeparam>
        /// <param name="args">Arguments</param>
        public TerminatorBuilder Register<T>(TerminateEventArgs args)
            where T : Exception
        {
            var type = typeof(T);
            if (this._registry.ContainsKey(type))
            {
                throw new ArgumentException($"Type {nameof(type)} is already registered");
            }

            this._registry.Add(type, args);

            return this;
        }

        /// <summary>
        /// Build the <see cref="Terminator"/>
        /// </summary>
        public ITerminator Build()
        {
            var terminator = new Terminator(this._registry,
                this._registerCTRLC,
                this.TerminateEventHandler,
                this.preTerminationAction);

            terminator.Validate();

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
            if (this.TerminateEventHandler is not null)
            {
                throw new NotSupportedException("An action is already registered");
            }

            this.preTerminationAction = preTermination;

            return this;
        }

        #endregion
    }
}
