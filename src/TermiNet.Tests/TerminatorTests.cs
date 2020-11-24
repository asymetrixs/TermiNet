namespace TermiNet.Tests
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using TermiNet.Event;
    using TermiNet.Interfaces;
    using TermiNet.ReservedCodes;
    using TermiNet.Validation;
    using Xunit;

    public class TerminatorTests
    {
        [Fact]
        public void TerminateWithException()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None)
                .Register<ArgumentException>(new TerminateEventArgs(201))
                .Register<IndexOutOfRangeException>(new TerminateEventArgs(202, "Test 202"));

            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(new ArgumentException());

            // Assert
            Assert.Equal(201, environment.ExitCode);
        }

        [Fact]
        public void TerminateWithNonRegisteredException()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None)
                .Register<ArgumentException>(new TerminateEventArgs(201));
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(new IndexOutOfRangeException("Test 210"));

            // Assert
            Assert.Equal(1, environment.ExitCode);
            Assert.Equal($"Unspecified error. IndexOutOfRangeException: Test 210", exitMessage);
        }

        [Fact]
        public void TerminateWithInteger()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(205, "Test 205");

            // Assert
            Assert.Equal(205, environment.ExitCode);
            Assert.Equal("Test 205", exitMessage);
        }

        [Fact]
        public void TerminatePreTerminationAction()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            bool actionExecuted = false;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            builder.RegisterPreTerminationAction(() =>
            {
                actionExecuted = true;
            });
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(205, "Test 205");

            // Assert
            Assert.Equal(205, environment.ExitCode);
            Assert.Equal("Test 205", exitMessage);
            Assert.True(actionExecuted);
        }

        [Fact]
        public void TerminateSecondPreTerminationAction()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            Exception exception = null;
            bool actionExecuted = false;


            builder.RegisterPreTerminationAction(() =>
            {
                actionExecuted = true;
            });

            try
            {
                builder.RegisterPreTerminationAction(() =>
                {
                    actionExecuted = true;
                });
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(NotSupportedException), exception.GetType());
            Assert.Equal("A pre-termination action is already registered", exception.Message);
        }

        [Fact]
        public void RegisterMultipleCtrlCActions()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            Exception exception = null;

            builder.RegisterCtrlC(() => {; });
            try
            {
                builder.RegisterCtrlC(() => {; });
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(NotSupportedException), exception.GetType());
            Assert.Equal("A CTRL+C action is already registered", exception.Message);
        }

        [Fact]
        public void TerminateWithIntegerAboveMax()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(256, "Test 256");

            // Assert
            Assert.Equal(255, environment.ExitCode);
            Assert.Equal("Test 256", exitMessage);
        }

        [Fact]
        public void TerminateWithEventArgs()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate(new TerminateEventArgs(206, "Test 206"));

            // Assert
            Assert.Equal(206, environment.ExitCode);
            Assert.Equal("Test 206", exitMessage);
        }

        [Fact]
        public void TerminateWithUnixCodeEventArgs()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            var tea = new TerminateEventArgs(UnixCode.CONFIG, "Test 11");
            var teaToString = tea.ToString();
            terminator.Terminate(tea);

            // Assert
            Assert.Equal(78, environment.ExitCode);
            Assert.Equal("Test 11", exitMessage);
            Assert.Equal("78: Test 11", teaToString);
        }

        [Fact]
        public void TerminateClean()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            terminator.Terminate();

            // Assert
            Assert.Equal(0, environment.ExitCode);
            Assert.Null(exitMessage);
        }

        [Fact]
        public void CancellationTokenAndEventHandler()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var cts = new CancellationTokenSource();
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeOnlyOnce);
            builder.RegisterCancellationToken(cts.Token, new TerminateEventArgs(21, "Test 21"));
            string exitMessage = null;
            builder.TerminateEventHandler += (object sender, TerminateEventArgs args) =>
            {
                exitMessage = args.ExitMessage;
            };
            var terminator = builder.Build();

            // Replace environment
            replaceEnvironment(terminator, environment);

            // Act
            cts.Cancel();

            // Assert
            Assert.Equal(21, environment.ExitCode);
            Assert.Equal("Test 21", exitMessage);
        }

        [Fact]
        public void SecondCancellationTokenAndEventHandler()
        {
            // Arrage
            var environment = new TestEnvironment(OSPlatform.Linux);
            var cts = new CancellationTokenSource();
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeOnlyOnce);
            builder.RegisterCancellationToken(cts.Token, new TerminateEventArgs(21, "Test 21"));
            Exception exception = null;

            // Act
            try
            {
                builder.RegisterCancellationToken(cts.Token, new TerminateEventArgs(21, "Test 21"));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Equal(typeof(ArgumentException), exception.GetType());
            Assert.Equal("A cancellation token is already registered.", exception.Message);
        }

        private void replaceEnvironment(ITerminator terminator, TestEnvironment environment)
        {
            Type typeInQuestion = typeof(Terminator);
            FieldInfo field = typeInQuestion.GetField("_environment", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(terminator, environment);
        }
    }
}
