namespace TermiNet.Example
{
    using System;
    using System.Runtime.InteropServices;
    using TermiNet.Event;
    using TermiNet.Interfaces;

    static class ExampleforTesting
    {
        public static void Run()
        {
            var fakeTerminator = new FakeTerminator();

            // Register fakeTerminator as ITerminator in your IoC container
            // Retrieve fakeTerminator using ITerminator

            fakeTerminator.Terminate(55, "Last message");

            // no real termination, but testing framework can retrieve the exit code and message
            Console.WriteLine(fakeTerminator.LastExitCode);
            Console.Write(fakeTerminator.LastExitMessage);
        }
    }

    /// <summary>
    /// Class resolving exit code in test
    /// </summary>
    public class FakeTerminator : ITerminator
    {
        /// <summary>
        /// Gets the last exit code
        /// </summary>
        public int LastExitCode { get; private set; }

        /// <summary>
        /// Gets the last exit message
        /// </summary>
        public string? LastExitMessage { get; private set; }

        public int CtrlCSIGINTExitCode => 5;

        public int DefaultCleanExitCode => 0;

        public int DefaultErrorExitCode => 1;

        public int MaxErrorExitCode => 100;

        public OSPlatform OsPlatform => OSPlatform.OSX;

        public void Terminate()
        {
            throw new NotImplementedException();
        }

        public void Terminate(int exitCode, string? exitMessage = null)
        {
            this.LastExitCode = exitCode;
            this.LastExitMessage = exitMessage;
        }

        public void Terminate(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Terminate(TerminateEventArgs exitEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
