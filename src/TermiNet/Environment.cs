﻿namespace TermiNet
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using TermiNet.Interfaces;

    /// <summary>
    /// Environment
    /// </summary>
    internal class Environment : IEnvironment
    {
        #region Fields

        /// <summary>
        /// Holds OS Platform
        /// </summary>
        private OSPlatform? _osPlatform;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the OS Platform
        /// </summary>
        public OSPlatform OSPlatform
        {
            get
            {
                if (this._osPlatform is null)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        this._osPlatform = OSPlatform.Linux;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    {
                        this._osPlatform = OSPlatform.FreeBSD;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        this._osPlatform = OSPlatform.OSX;
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        this._osPlatform = OSPlatform.Windows;
                    }
                    else
                    {
                        throw new SystemException("Cannot resolve OS platform");
                    }
                }

                return this._osPlatform.Value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exits
        /// </summary>
        /// <param name="code"></param>
        [DoesNotReturn]
        public void Exit(int code)
        {
            System.Environment.Exit(code);
        }

        #endregion
    }
}
