// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;

    /// <summary>
    /// This class defines the main entry point for the application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        internal static void Main()
        {
            string mySid = "S-1-5-21-149779583-363096731-64667279X";
            SecurityIdentifier sid = new SecurityIdentifier(mySid);
            System.Security.Principal.NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
            //System.Security.Principal.WindowsIdentity ident = new WindowsIdentity (
            

            Console.WriteLine(sid.Value);
            //Console.WriteLine(WindowsIdentity.GetCurrent().User.Value);
#if DEBUG
            Console.Write("\n\nPress <ENTER> to close this program.");
            Console.ReadLine();
#endif
        }
    }
}
