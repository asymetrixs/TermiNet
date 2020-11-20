namespace TermiNet.Example
{
    using TermiNet.Event;
    using System;

    class Program
    {
        /// <summary>
        /// Example 1
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            /// Exists with 260 when a <see cref="ArgumentException"/> is passed
            Exit.Register<ArgumentException>(260, "I am stupid");
            
            // Event handler will be called with the exit event
            Exit.Terminating += Exit_Terminating;

            // Checks that registered exist codes are inside the boundary
            Exit.Validate();
            
            try
            {
                throw new ArgumentException("test");
            }
            catch(Exception e)
            {
                Exit.Terminate(e);
            }
        }

        /// <summary>
        /// Example 2
        /// </summary>
        static void Example2()
        {
            // Clean exit
            Exit.Terminate();
        }

        /// <summary>
        /// Exmaple 3
        /// </summary>
        static void Example3()
        {
            /// Results in <see cref="Exit.DefaultErrorExitCode"/> as exit code
            Exit.Terminate(new Exception("test"));
        }

        /// <summary>
        /// Exmaple 4
        /// </summary>
        static void Example4()
        {
            // Results in exit code 200
            Exit.Terminate(200, "Exit code");
        }

        /// <summary>
        /// Example 5
        /// </summary>
        static void Example5()
        {
            // Results in exit code 200
            // Event handler will be called with the exit event
            Exit.Terminating += Exit_Terminating;
            Exit.Terminate(new ExitEventArgs(200, "Test"));
        }

        /// <summary>
        /// Event method that is called before this app exits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Exit_Terminating(object sender, ExitEventArgs e)
        {
            Console.WriteLine($"Exiting: {e}");
        }
    }
}
