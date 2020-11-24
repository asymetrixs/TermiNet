namespace TermiNet.Example
{
    using System;
    using TermiNet.Event;

    static class ExampleForUsageWithValidation
    {
        public static void Run()
        {
            // Set up builder
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeOnlyOnce | ValidationLevel.ExitCodeWithBoundaries)
                .Register<ArgumentException>(new TerminateEventArgs(77, "Optional example message"))
                .Register<NullReferenceException>(new TerminateEventArgs(77, "Optional example message"));
            
            // Build terminator, throws exception
            var terminator = builder.Build();

            // Application has exited before this line is executed
            Console.WriteLine("I should not run...");
        }
    }
}
