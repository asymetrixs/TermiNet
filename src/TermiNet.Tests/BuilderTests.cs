namespace TermiNet.Tests
{
    using System;
    using TermiNet.Event;
    using TermiNet.Validation;
    using Xunit;

    public class BuilderTests
    {
        [Fact]
        public void CreateBuilder()
        {
            // Arrage

            // Act
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);

            // Assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void SameExceptionTwice()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            Exception exception = null;

            // Act
            builder.Register<Exception>(new TerminateEventArgs(20, "Test 20"));

            try
            {
                builder.Register<Exception>(new TerminateEventArgs(20, "Test 20"));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(builder);
            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public void SameExitCodeTwiceNoValidation()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.None);
            builder.Register<Exception>(new TerminateEventArgs(20, "Test 20"));
            builder.Register<ArgumentException>(new TerminateEventArgs(20, "Test 20"));

            // Act
            var terminator = builder.Build();

            // Assert
            Assert.NotNull(terminator);
        }

        [Fact]
        public void SameExitCodeTwiceValidate()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeOnlyOnce);
            builder.Register<Exception>(new TerminateEventArgs(20, "Test 20"));
            builder.Register<ArgumentException>(new TerminateEventArgs(20, "Test 20"));
            Exception exception = null;

            // Act
            try
            {
                var terminator = builder.Build();
            }
            catch (Exception e)
            {
                exception = e;
            }


            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public void SameExitCodeTwiceValidateIgnore()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeOnlyOnce & ValidationLevel.None);
            builder.Register<Exception>(new TerminateEventArgs(20, "Test 20"));
            builder.Register<ArgumentException>(new TerminateEventArgs(20, "Test 20"));
            Exception exception = null;

            // Act
            var terminator = builder.Build();

            // Assert
            Assert.NotNull(terminator);
        }

        [Fact]
        public void ExitCodeWithBoundariesMinusOne()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeWithBoundaries);
            Exception exception = null;

            // Act
            try
            {
                builder.Register<Exception>(new TerminateEventArgs(-1, "Test -1"));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentOutOfRangeException), exception.GetType());
        }

        [Fact]
        public void ExitCodeWithBoundaries256()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeWithBoundaries);
            Exception exception = null;

            // Act
            try
            {
                builder.Register<Exception>(new TerminateEventArgs(256, "Test 256"));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentOutOfRangeException), exception.GetType());
        }

        [Fact]
        public void ExitCodeInReservedSpace()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeNotInReservedSpace);
            Exception exception = null;

            // Act
            try
            {
                builder.Register<Exception>(new TerminateEventArgs(130, "Test 130"));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(typeof(ArgumentException), exception.GetType());
        }

        [Fact]
        public void ExitCodeNotInReservedSpace()
        {
            // Arrage
            var builder = TerminatorBuilder.CreateBuilder(ValidationLevel.ExitCodeNotInReservedSpace);
            builder.Register<Exception>(new TerminateEventArgs(202, "Test 202"));

            var terminator = builder.Build();

            // Assert
            Assert.NotNull(terminator);
        }
    }
}
