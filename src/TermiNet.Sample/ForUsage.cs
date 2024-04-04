using System;
using TermiNet.Event;
using TermiNet.Validation;

namespace TermiNet.Sample;

static class ForUsage
{
    public static void Run()
    {
        Console.WriteLine("Hello World!");

        // Set up builder
        var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None)
            .RegisterCtrlC()
            .RegisterPreTerminationAction(() => { Console.WriteLine("Pre Termination Action"); })
            .Register<ArgumentException>(new TerminateEventArgs(77, "Optional example message"));
        builder.TerminateEventHandler += Exit_Terminating;

        // Build terminator
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
