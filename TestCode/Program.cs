// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;

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
            /*
            string[] sids = new string[] { "S-1-5-21-149779583-363096731-646672791-5361", "S-1-5-21-149779583-363096731-646672791-5360", "S-1-5-21-149779583-363096731-646672791" };
            foreach (string sidString in sids)
            {
                string ntAccountName = string.Empty;
                Console.WriteLine(sidString);
                System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(sidString);
                if (sid.IsValidTargetType(typeof(System.Security.Principal.NTAccount)))
                {
                    try
                    {
                        System.Security.Principal.NTAccount ntAccount = (System.Security.Principal.NTAccount)sid.Translate(typeof(System.Security.Principal.NTAccount));
                        ntAccountName = ntAccount.Value;
                    }
                    catch (System.Security.Principal.IdentityNotMappedException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }

                Console.WriteLine(ntAccountName);

                Console.WriteLine();
            }
            */            

#if DEBUG
            Console.Write("`n`nPress <ENTER> to close this program.");
            Console.ReadLine();
#endif
        }
    }
}
