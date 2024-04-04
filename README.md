# TermiNet

Libary that can be used to properly terminate an application on Windows, Linux, FreeBSD, and OSX.

NuGet: <https://www.nuget.org/packages/TermiNet/>

## Why?

Today, .NET Core runs on Linux, FreeBSD, OSX, and Windows. There are big differences between *nix and Windows based operating systems on how error codes are interpreted.
On Linux for example a clean application termination has error code 0. This signals the OS that the application terminated in a clean state.
On Windows a clean exit also is 0. But on Linux an error code has to be in the range of 0 to 254. Everything that is out of this range will become 255.
Now, having an application that on Windows perfectly returns 2711 or 9283 to indicate a problem, on Linux both melt down to 255. This does not really allow for proper exit code analysis anymore.

## Advantages

TermiNet allows for configuration of exit codes in various ways. You can register exceptions and assign codes or provide an exit code directly. Also TermiNet allows to register an pre-termination event and an action, both are called before the application terminates. Moreover, using ```System.Exit(0)``` can cause trouble when testing the application. TermiNet provides an interface that can be used during testing.
Another idea is using TermiNet with docker or in a service. An application can implement TermiNet and exit with a specific exit code on a certain event, e.g. when a referred service in another docker container (or service) does not respond. Docker (or the service manager) can then evaluate the exit code and be configured to spwan the referred docker application (or service) first, before it spawns your application again.

## Examples

The following sections show some examples, of course they can also be combined.

### Simple termination

```cs
public static void Run()
{
    Console.WriteLine("Hello World!");

    var builder = TerminatorBuilder.CreateBuilder().Build();
    var terminator = builder.Build();

    terminator.Terminate(55, "Exiting with exit code 55.");
}
```

### SIGINT, Exception, Pre-Termination action

```cs
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

private static void Exit_Terminating(object? sender, TerminateEventArgs e)
{
    Console.WriteLine($"Exiting: {e}");
}
```

### Using CancellationToken
```cs
public static void Run()
{
    // Create cancellation token
    var cts = new CancellationTokenSource();

    // Setup termintator
    var builder = TerminatorBuilder.CreateBuilder()
        .RegisterCancellationToken(cts.Token, 20, "Terminated by CTS");
    
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
```
