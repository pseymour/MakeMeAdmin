// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// This class contains the main entry point for the application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            /*
#if DEBUG
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("qps-ploc");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("qps-ploc");
#endif
            */
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SubmitRequestForm());
        }
    }
}
