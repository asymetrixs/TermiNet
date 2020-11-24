namespace TermiNet.Tests
{
    using System.Runtime.InteropServices;
    using TermiNet.Interfaces;

    public class TestEnvironment : IEnvironment
    {
        public TestEnvironment(OSPlatform osPlatform)
        {
            this.OSPlatform = OSPlatform;
        }

        #region Properties

        public int ExitCode { get; private set; }

        public OSPlatform OSPlatform { get; private set; }

        #endregion

        #region Methods

        public void Exit(int code)
        {
            this.ExitCode = code;
        }

        #endregion
    }
}
