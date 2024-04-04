using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TermiNet.Interfaces;

namespace TermiNet.Tests;

public class TestEnvironment : IEnvironment
{
    public TestEnvironment(OSPlatform osPlatform)
    {
        this.OSPlatform = OSPlatform;
    }

    public int ExitCode { get; private set; }

    public OSPlatform OSPlatform { get; private set; }

    public void Exit(int code)
    {
        this.ExitCode = code;
    }
}
