using System;

namespace TermiNet.Validation;

/// <summary>
/// Configures how validation is taking place.
/// </summary>
[Flags]
public enum ValidationLevel
{
    /// <summary>
    /// No validation takes place
    /// </summary>
    None = 1,

    /// <summary>
    /// Makes sure that reserved (C/C++, /usr/include/sysexit.h)
    /// exit codes are not used and that no exit code is used more than once.
    /// </summary>
    ExitCodeOnlyOnce = 2,

    /// <summary>
    /// Check that exit codes is not in the reserved space.
    /// </summary>
    ExitCodeNotInReservedSpace = 4,

    /// <summary>
    /// Checks that exit codes are within allowed minimum and maximum of OS platform
    /// </summary>
    ExitCodeWithBoundaries = 8
}
