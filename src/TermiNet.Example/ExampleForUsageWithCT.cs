namespace TermiNet.Example
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TermiNet.Event;

    static class ExampleForUsageWithCT
    {
        public async static Task Run()
        {
            // Create cancellation token
            var cts = new CancellationTokenSource();

            // Setup termintator
            var builder = TerminatorBuilder.CreateBuilder()
                .RegisterCancellationToken(cts.Token, 20, "Terminated by CTS");
            builder.TerminateEventHandler += Exit_Terminating;
            var terminator = builder.Build();

            // Display something on console
            var x = Task.Run(async () =>
            {
                while (true)
                {
                    Console.WriteLine("Hello");
                    await Task.Delay(2000).ConfigureAwait(false);                    
                }
            });

            Console.WriteLine("Press a key");
            Console.ReadKey();

            // Cancel termination
            cts.Cancel();

            // Application has exited before this line is executed
            Console.WriteLine("I should not run...");
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
