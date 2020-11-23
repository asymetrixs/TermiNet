namespace TermiNet.ReservedCodes
{
    /// <summary>
    /// Codes as described in https://tldp.org/LDP/abs/html/exitcodes.html
    /// and /usr/include/sysexit.h
    /// Codes may vary a bit depending on the shell used, e.g. bash or tcsh
    /// As most systems use bash, zsh, or sh, we will use these codes.
    /// </summary>
    public enum UnixCode
    {
        /// <summary>
        /// Successful termination
        /// </summary>
        OK = 0,

        /// <summary>
        /// Catchall for general errors
        /// </summary>
        CATCHALL = 1,

        /// <summary>
        /// Misuse of shell builtins (according to Bash documentation)
        /// </summary>
        MISUSE_OF_SHELL_BUILTINS = 2,

        /// <summary>
        /// Command line usage error
        /// </summary>
        USAGE = 64,

        /// <summary>
        /// Data format error
        /// </summary>
        DATAERR = 65,

        /// <summary>
        /// Cannot open input
        /// </summary>
        NOINPUT = 66,

        /// <summary>
        /// Addressee unknown
        /// </summary>
        NOUSER = 67,

        /// <summary>
        /// Host name unknown
        /// </summary>
        NOHOST = 68,

        /// <summary>
        /// Service unavailable
        /// </summary>
        UNAVAILABLE = 69,

        /// <summary>
        /// Internal software error
        /// </summary>
        SOFTWARE = 70,

        /// <summary>
        /// System error (e.g. can't fork)
        /// </summary>
        OSERR = 71,

        /// <summary>
        /// Critical OS file missing
        /// </summary>
        OSFILE = 72,

        /// <summary>
        /// Can't create (user) output file
        /// </summary>
        CANTCREAT = 73,

        /// <summary>
        /// Input/output error
        /// </summary>
        IOERR = 74,

        /// <summary>
        /// Temp failure: user is invited to retry
        /// </summary>
        TEMPFAIL = 75,

        /// <summary>
        /// Remote error in protocol
        /// </summary>
        PROTOCOL = 76,

        /// <summary>
        /// Permission denied
        /// </summary>
        NOPERM = 77,

        /// <summary>
        /// Configuration error
        /// </summary>
        CONFIG = 78,

        /// <summary>
        /// Command invoked cannot execute
        /// </summary>
        COMMAND_INVOKED_CANNOT_EXECUTE = 126,

        /// <summary>
        /// Command not found
        /// </summary>
        COMMAND_NOT_FOUND = 127,

        /// <summary>
        /// Invalid argument to exit
        /// </summary>
        INVALID_ARGUMENT_TO_EXIT = 128,

        /// <summary>
        /// SIGHUP
        /// </summary>
        SIGHUP = 129,

        /// <summary>
        /// SIGINT (Control-C)
        /// </summary>
        SIGINT = 130,

        /// <summary>
        /// SIGQUIT
        /// </summary>
        SIGQUIT = 131,

        /// <summary>
        /// SIGILL
        /// </summary>
        SIGILL = 132,

        /// <summary>
        /// SIGTRAP
        /// </summary>
        SIGTRAP = 133,

        /// <summary>
        /// SIGABRT
        /// </summary>
        SIGABRT = 134,

        /// <summary>
        /// SIGBUS
        /// </summary>
        SIGBUS = 135,

        /// <summary>
        /// SIGFPE
        /// </summary>
        SIGFPE = 136,

        /// <summary>
        /// SIGKILL (e.g. kill -9)
        /// </summary>
        SIGKILL = 137,

        /// <summary>
        /// SIGUSR1
        /// </summary>
        SIGUSR1 = 138,

        /// <summary>
        /// SIGSEGV
        /// </summary>
        SIGSRGV = 139,

        /// <summary>
        /// SIGUSR2
        /// </summary>
        SIGUSR2 = 140,

        /// <summary>
        /// SIGPIPE
        /// </summary>
        SIGPIPE = 141,

        /// <summary>
        /// SIGALRM
        /// </summary>
        SIGALRM = 142,

        /// <summary>
        /// SIGTERM
        /// </summary>
        SIGTERM = 143,

        /// <summary>
        /// SIGSTKFLT
        /// </summary>
        SIGSTKLFT = 144,

        /// <summary>
        /// SIGCHLD
        /// </summary>
        SIGCHLD = 145,

        /// <summary>
        /// SIGCONT
        /// </summary>
        SIGCONT = 146,

        /// <summary>
        /// SIGSTOP
        /// </summary>
        SIGSTOP = 147,

        /// <summary>
        /// SIGTSTP
        /// </summary>
        SIGTSTP = 148,

        /// <summary>
        /// SIGTTIN
        /// </summary>
        SIGTTIN = 149,

        /// <summary>
        /// SIGTTOU
        /// </summary>
        SIGTTOU = 150,

        /// <summary>
        /// SIGURG
        /// </summary>
        SIGURG = 151,

        /// <summary>
        /// SIGXCPU
        /// </summary>
        SIGXCPU = 152,

        /// <summary>
        /// SIGXFSZ
        /// </summary>
        SIGXFSZ = 153,

        /// <summary>
        /// SIGVTALRM
        /// </summary>
        SIGVTARLM = 154,

        /// <summary>
        /// SIGPROF
        /// </summary>
        SIGPROF = 155,

        /// <summary>
        /// SIGWINCH
        /// </summary>
        SIGWINCH = 156,

        /// <summary>
        /// SIGPOLL
        /// </summary>
        SIGPOLL = 157,

        /// <summary>
        /// SIGPWR
        /// </summary>
        SIGPWR = 158,

        /// <summary>
        /// SIGSYS
        /// </summary>
        SIGSYS = 159,

        /// <summary>
        /// Exit status out of range
        /// </summary>
        EXIT_STATUS_OUT_OF_RANGE = 255
    }
}
