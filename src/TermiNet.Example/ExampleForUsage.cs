namespace TermiNet.Example
{
    using System;
    using TermiNet.Event;

    static class ExampleForUsage
    {
        public static void Run()
        {
            Console.WriteLine("Hello World!");

            var builder = TerminatorBuilder.CreateBuilder()
                .RegisterCtrlC()
                .RegisterPreTerminationAction(() => { Console.WriteLine("Pre Termination Action"); })
                .Register<ArgumentException>(100, "Optional example message");
            builder.TerminateEventHandler += Exit_Terminating;

            var terminator = builder.Build();

            try
            {
                throw new ArgumentException("test");
            }
            catch (Exception e)
            {
                terminator.Terminate(e);
            }
        }

        /// <summary>
        /// Event method that is called before this app exits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Exit_Terminating(object? sender, TerminateEventArgs e)
        {
            Console.WriteLine($"Exiting: {e}");
        }
    }
}
