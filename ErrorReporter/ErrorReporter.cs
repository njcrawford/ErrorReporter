/*
Error Reporter - a convenient library for reporting errors

Copyright (C) 2017 Nathan Crawford
 
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
02111-1307, USA.

A copy of the full GPL 2 license can be found in the docs directory.
You can contact me at http://www.njcrawford.com/contact
*/


using System;
using System.Reflection;
using System.Windows.Forms;

namespace NJCrawford
{
    public class ErrorReporter
    {
        // Limit commandline to about 2000 characters (2000 seems to be the limit on my Windows 7 laptop)
        private const int CommandLineMaxLength = 2000;

        // Application name to use in report
        private static string appName;

        // Base url
        private static string baseUrl;

        // Call this to enable reporting of unhandled exceptions.
        // appName is a string that will be used in the report to help identify which program it came from.
        public static void RegisterWebReport(string appName, string baseUrl)
        {
            ErrorReporter.appName = appName;
            ErrorReporter.baseUrl = baseUrl;

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
        }

        // Functions to deal with unhandled exceptions
        // Based on code from http://stevenbenner.com/2010/01/reporting-of-unhandled-exceptions-in-your-distributable-net-application/
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportErrorPrompt((Exception)e.ExceptionObject);
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ReportErrorPrompt(e.Exception);
        }

        public static void ReportErrorPrompt(Exception ex)
        {
            // AppName is used in the message because caller.FullName is too verbose
            if (MessageBox.Show(
                appName + " has encountered an error.\n\n" +
                    "Click Yes to report this error using a web browser.\n" +
                    "Click No if you'd rather not report the error.\n" +
                    "Please consider submitting the report to help find and fix this issue.\n\n" +
                    "Error:\n" +
                    ex.Message + "\n" +
                    ex.StackTrace,
                "Report Application Error?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                ReportError(ex, true);
            }
        }

        public static void ReportError(Exception ex, bool exitAfterReport)
        {
            try
            {
                // Get base url
                string url = baseUrl;

                // Add error message to it
                Assembly caller = Assembly.GetEntryAssembly();
                string errorText = "Additional details:\n\n" +
                    "Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
                    "Program: " + caller.FullName + "\n" +
                    "Program location: " + caller.Location + "\n" +
                    "OS: " + GetOSVersionString() + "\n" +
                    "OS Culture: " + System.Globalization.CultureInfo.CurrentCulture.Name + "\n" +
                    "Framework: " + Environment.Version + (Environment.Is64BitProcess ? " (x64)" : " (x86)") + "\n\n" +
                    "Error:\n" +
                    ex.GetType().ToString() + " (" + ex.Message + ")\n" +
                    ex.StackTrace;
                // URL encode message
                url += System.Uri.EscapeDataString(errorText.Trim());

                // Limit command line length
                if (url.Length > CommandLineMaxLength)
                {
                    string clippedMessage = System.Uri.EscapeDataString("\n(Report clipped)");
                    url = url.Substring(0, CommandLineMaxLength - clippedMessage.Length);
                    url += clippedMessage;
                }

                // Open URL in browser
                System.Diagnostics.Process.Start(url);
            }
            finally
            {
                if (exitAfterReport)
                {
                    Application.Exit();
                }
            }
        }

        static string GetOSVersionString()
        {
            string retval = "";

            try
            {
                // OS name
                // Example: Windows 7 Home Premium
                string productName;

                // CSDVersion isn't available when running as 32 bit process on 64 bit Windows due to
                // WoW64 oddities in the registry.
                // Example: Service Pack 1
                string servicePack;

                // Windows NT version
                // Example: 6.1
                string ntVersion;

                // Windows NT build
                // Example: 7601
                string ntBuild;

                // ReleaseId is useful for Windows 10
                // Example: 1511
                string releaseId;

                // Open registry and read values
                // The using statement will automatically close and clean up after the registry key.
                using (var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    productName = (string)reg.GetValue("ProductName");
                    servicePack = (string)reg.GetValue("CSDVersion");
                    ntVersion = (string)reg.GetValue("CurrentVersion");
                    ntBuild = (string)reg.GetValue("CurrentBuild");
                    releaseId = (string)reg.GetValue("ReleaseId");
                }

                if(!String.IsNullOrEmpty(productName))
                {
                    retval = productName;

                    if (!String.IsNullOrEmpty(servicePack))
                    {
                        retval += " " + servicePack;
                    }
                    if(!String.IsNullOrEmpty(releaseId))
                    {
                        retval += " release " + releaseId;
                    }
                }

                if(!String.IsNullOrEmpty(ntVersion))
                {
                    retval += " (" + ntVersion;
                    if(!String.IsNullOrEmpty(ntBuild))
                    {
                        retval += " build " + ntBuild;
                    }
                    retval += ")";
                }
            }
            // Catch any and all errors from accessing the registry. The program has already crashed
            // at this point, we're just trying to gather some useful information to report.
            catch { }

            // If there was an exception or nothing was found in the registry, fall back to 
            // Environment.OSVersion.
            if (String.IsNullOrWhiteSpace(retval))
            {
                // Old method - works up to Windows 8
                retval = Environment.OSVersion.ToString() + " (Based on Environment.OSVersion)";
            }

            // Add 32/64 bit
            retval += (Environment.Is64BitOperatingSystem ? " (x64)" : " (x86)");

            return retval;
        }
    }
}
