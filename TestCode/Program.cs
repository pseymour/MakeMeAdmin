// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;
    using System.Security.Principal;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                Console.WriteLine("Desktops Detected");
                Console.WriteLine("=================");

                System.Text.StringBuilder desktopNameString = new System.Text.StringBuilder("Desktops: ");
                string[] desktopNames = Desktop.GetDesktops();
                for (int j = 0; j < desktopNames.Length; j++)
                {
                    desktopNameString.Append(desktopNames[j]);
                    if (j < (desktopNames.Length - 1))
                    {
                        desktopNameString.Append(", ");
                    }
                }
                Console.WriteLine(desktopNameString.ToString());

                /*
                foreach (string desktop in Desktop.GetDesktops())
                {
                    Console.WriteLine(desktop);
                }
                */

                Console.WriteLine();

                Console.WriteLine("Window Stations Detected");
                Console.WriteLine("========================");
                System.Text.StringBuilder winStaNameString = new System.Text.StringBuilder("Window Stations: ");
                string[] winStaNames = Desktop.GetWindowStations();
                for (int j = 0; j < winStaNames.Length; j++)
                {
                    winStaNameString.Append(winStaNames[j]);
                    if (j < (winStaNames.Length - 1))
                    {
                        winStaNameString.Append(", ");
                    }
                }
                Console.WriteLine(winStaNameString.ToString());

                /*
                foreach (string winsta in Desktop.GetWindowStations())
                {
                    Console.WriteLine(winsta);
                }
                */
                long DESKTOP_ENUMERATE = 0x0040L;
                long DESKTOP_READOBJECTS = 0x0001L;
                long DESKTOP_WRITEOBJECTS = 0x0080L;
                long READ_CONTROL = 0x00020000L;

                long MAXIMUM_ALLOWED = 0x02000000L;

                //Console.WriteLine(LsaLogonSessions.LogonSessions.WTSGetActiveConsoleSessionId());
                //Console.WriteLine(LsaLogonSessions.LogonSessions.GetDesktopWindow());
                IntPtr inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenInputDesktop(0, false, 256);

                if (inputDesktopPtr == IntPtr.Zero)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("input desktop pointer was zero!");
                    Console.ResetColor();

                    inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenDesktop("Winlogon", 0, false, (uint)MAXIMUM_ALLOWED);
                    if (inputDesktopPtr == IntPtr.Zero)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("input desktop pointer is still zero!");
                        Console.ResetColor();
                    }
                    LsaLogonSessions.LogonSessions.SetThreadDesktop(inputDesktopPtr);
                    int lastError = Marshal.GetLastWin32Error();
                    Console.WriteLine("SetThreadDesktop Error: {0:N0}", lastError);
                    LsaLogonSessions.LogonSessions.CloseDesktop(inputDesktopPtr);
                    lastError = Marshal.GetLastWin32Error();
                    Console.WriteLine("CloseDesktop Error: {0:N0}", lastError);


                    /*
                    inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenDesktop("Default", 0, false, 256);
                    if (inputDesktopPtr == IntPtr.Zero)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("input desktop pointer is still zero!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("i was able to open the default desktop!");
                        Console.ResetColor();
                        bool switchResult = LsaLogonSessions.LogonSessions.SwitchDesktop(inputDesktopPtr);
                        Console.WriteLine("able to switch desktops: {0}", switchResult);
                    }
                    */
                }

                //int lastError = Marshal.GetLastWin32Error();
                Console.WriteLine(inputDesktopPtr);
                //Console.WriteLine("Last Error: {0:N0}", lastError);
                LsaLogonSessions.LogonSessions.CloseDesktop(inputDesktopPtr);


                System.Threading.Thread.Sleep(2000);
                Console.WriteLine();
            }


#if DEBUG
            Console.Write("\n\nPress <ENTER> to close this program.");
            Console.ReadLine();
#endif
        }
    }
}
