namespace TermiNet
{
    using TermiNet.Event;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Static class that handles exiting
    /// </summary>
    public static class Exit
    {
        #region Constructor

        /// <summary>
        /// Sets up OS platform specific parameters
        /// </summary>
        static Exit()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                DefaultCleanExitCode = 0;
                DefaultErrorExitCode = 1;
                MaxErrorExitCode = 255;
                OSPlatform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                DefaultCleanExitCode = 0;
                DefaultErrorExitCode = 1;
                MaxErrorExitCode = 255;
                OSPlatform = OSPlatform.FreeBSD;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                DefaultCleanExitCode = 0;
                DefaultErrorExitCode = 1;
                MaxErrorExitCode = 255;
                OSPlatform = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                /// TODO
                DefaultCleanExitCode = 0;
                DefaultErrorExitCode = 1;
                MaxErrorExitCode = 9999;
                OSPlatform = OSPlatform.Windows;
            }
            else
            {
                throw new SystemException("Cannot resolve OS platform");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Store for <see cref="ExitEventArgs"/>
        /// </summary>
        private static Dictionary<Type, ExitEventArgs> _registry = new();

        /// <summary>
        /// Event handler gets called before application terminates
        /// </summary>
        public static event EventHandler<ExitEventArgs> Terminating;

        /// <summary>
        /// Default OS platform exit code on clean exit
        /// </summary>
        public static int DefaultCleanExitCode { get; private set; }

        /// <summary>
        /// Default OS platform exit code on error
        /// </summary>
        public static int DefaultErrorExitCode { get; private set; }

        /// <summary>
        /// Maximum possible error code, e.g. on Linnux 255
        /// </summary>
        public static int MaxErrorExitCode { get; private set; }

        /// <summary>
        /// OS platform
        /// </summary>
        public static OSPlatform OSPlatform { get; private set; }

        #endregion

        #region Functions

        /// <summary>
        /// Is called before the app exits and could be used for instance for logging
        /// </summary>
        /// <param name="e">Event</param>
        private static void OnTerminating(ExitEventArgs e)
        {
            EventHandler<ExitEventArgs> handler = Terminating;
            handler?.Invoke(null, e);
        }

        /// <summary>
        /// Terminates the app
        /// </summary>
        [DoesNotReturn]
        public static void Terminate()
        {
            _Terminate(new ExitEventArgs(DefaultCleanExitCode, null));
        }

        /// <summary>
        /// Terminates the app
        /// </summary>
        /// <param name="exitCode">Exit code</param>
        /// <param name="exitMessage">Exit message for use in <see cref="OnTerminating(ExitEventArgs)"/></param>
        [DoesNotReturn]
        public static void Terminate(int exitCode, string exitMessage = null)
        {
            if (exitCode > MaxErrorExitCode)
            {
                exitCode = MaxErrorExitCode;
            }

            _Terminate(new ExitEventArgs(exitCode, exitMessage));
        }

        /// <summary>
        /// Terminates the app
        /// </summary>
        /// <param name="exitEventArgs">Event with exit code and message.</param>
        [DoesNotReturn]
        public static void Terminate(ExitEventArgs exitEventArgs)
        {
            _Terminate(exitEventArgs);
        }

        /// <summary>
        /// Terminates the app. Uses registered information or <see cref="DefaultErrorExitCode"/> and the exceptions name and message.
        /// </summary>
        /// <param name="e">Exception that will be used to determine the exit code (if registered).</param>
        [DoesNotReturn]
        public static void Terminate(Exception e)
        {
            if (_registry.TryGetValue(e.GetType(), out ExitEventArgs exitEventArgs))
            {
                _Terminate(exitEventArgs);
            }
            else
            {
                _Terminate(new ExitEventArgs(DefaultErrorExitCode, $"Unspecified error. {e.GetType().Name}: {e.Message}"));
            }
        }

        /// <summary>
        /// Actually terminates the app
        /// </summary>
        /// <param name="exitEventArgs"></param>
        [DoesNotReturn]
        private static void _Terminate(ExitEventArgs exitEventArgs)
        {
            OnTerminating(exitEventArgs);

            Environment.Exit(exitEventArgs.ExitCode);
        }

        /// <summary>
        /// Registers an exit code and message for an specific exception type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exitCode"></param>
        /// <param name="exitMessage"></param>
        public static void Register<T>(int exitCode, string exitMessage = null)
            where T : Exception
        {
            var type = typeof(T);
            if (_registry.ContainsKey(type))
            {
                throw new ArgumentException($"Type {nameof(type)} is already registered");
            }

            _registry.Add(type, new ExitEventArgs(exitCode, exitMessage));
        }

        /// <summary>
        /// Registers <see cref="ExitEventArgs"/> for an exception type
        /// </summary>
        /// <typeparam name="T">Type of the exception</typeparam>
        /// <param name="args">Arguments</param>
        public static void Register<T>(ExitEventArgs args)
            where T : Exception
        {
            var type = typeof(T);
            if (_registry.ContainsKey(type))
            {
                throw new ArgumentException($"Type {nameof(type)} is already registered");
            }

            _registry.Add(type, args);
        }

        /// <summary>
        /// Validates that the registered exit codes are within the OS platform range.
        /// </summary>
        public static void Validate()
        {
            if (_registry.Any(item => item.Value.ExitCode < DefaultCleanExitCode || item.Value.ExitCode > DefaultErrorExitCode))
            {
                throw new ArgumentOutOfRangeException($"Exit codes are out of range for OS plattform {OSPlatform}: {DefaultCleanExitCode} < X <= {DefaultErrorExitCode}");
            }
        }

        #endregion
    }
}
