using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermiNet.ReservedCodes
{
    public class Linux
    {
        public Dictionary<int, string> _reservedCodes = new();

        public Linux()
        {
            this._reservedCodes.Add(1, "Catchall for general errors");
            this._reservedCodes.Add(2, "Misuse of shell builtins (according to Bash documentation)");
            this._reservedCodes.Add(126, "Command invoked cannot execute");
            this._reservedCodes.Add(127, "\"command not found\"");
            this._reservedCodes.Add(128, "Invalid argument to exit");
            this._reservedCodes.Add(130, "Script terminated by Control-C");
            this._reservedCodes.Add(255, "Exit status out of range");
        }
    }
}
