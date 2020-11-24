namespace TermiNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using TermiNet.Event;
    using TermiNet.ReservedCodes;

    /// <summary>
    /// Validates the <see cref="Terminator"/> configuration
    /// </summary>
    internal class Validator
    {
        /// <summary>
        /// Performs checks on an individual termination event
        /// </summary>
        /// <param name="validationLevel">Validation level</param>
        /// <param name="terminateEventArgs">Termination event</param>
        internal static void Validate(ValidationLevel validationLevel, TerminateEventArgs terminateEventArgs)
        {
            if (validationLevel.HasFlag(ValidationLevel.ExitCodeWithBoundaries))
            {
                if (terminateEventArgs.ExitCode > TerminatorBuilder.MaxErrorExitCode)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(terminateEventArgs.ExitCode)} cannot be greater than {nameof(TerminatorBuilder.MaxErrorExitCode)}: {TerminatorBuilder.MaxErrorExitCode}");
                }
                else if (terminateEventArgs.ExitCode < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(terminateEventArgs.ExitCode)} cannot be less than 0.");
                }
            }

            if (validationLevel.HasFlag(ValidationLevel.ExitCodeNotInReservedSpace) && TerminatorBuilder.OsPlatform != OSPlatform.Windows)
            {
                if (Enum.IsDefined(typeof(UnixCode), terminateEventArgs.ExitCode))
                {
                    var linuxExitCode = (UnixCode)terminateEventArgs.ExitCode;

                    throw new ArgumentException($"Exit code {terminateEventArgs.ExitCode} is already defined by {nameof(UnixCode)}: {linuxExitCode}");
                }
            }
        }

        /// <summary>
        /// Performs checks on the event store
        /// </summary>
        /// <param name="validationLevel">Validation level</param>
        /// <param name="registry">Event store</param>
        internal static void Validate(ValidationLevel validationLevel, Dictionary<Type, TerminateEventArgs> registry)
        {
            if (validationLevel.HasFlag(ValidationLevel.ExitCodeOnlyOnce))
            {
                var multipleUsage = (from r in registry
                                     group r by r.Value.ExitCode
                                into m
                                     where m.Count() > 1
                                     select m);

                if (multipleUsage.Any())
                {
                    var code = multipleUsage.First();

                    throw new ArgumentException($"Code {code.Key} is used more than once.");
                }
            }
        }
    }
}
