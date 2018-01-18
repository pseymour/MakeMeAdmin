// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceProcess;

    /// <summary>
    /// Contains the main entry point for the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Service.Program.Main.", EventID.DebugMessage);
#endif
            */
            ServiceBase.Run(new MakeMeAdminService());
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Service.Program.Main.", EventID.DebugMessage);
#endif
            */
        }
    }
}
