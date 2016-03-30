/*
Error Reporter - a convenient library for reporting errors

Copyright (C) 2016 Nathan Crawford
 
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
            reportError((Exception)e.ExceptionObject);
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            reportError(e.Exception);
        }

        private static void reportError(Exception ex)
        {
            try
            {
                // AppName is used in the message because caller.FullName is too verbose
                if (MessageBox.Show(
                    appName + " has encountered a error.\n\n" +
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
                    // Get base url
                    string url = baseUrl;

                    // Add error message to it
                    Assembly caller = Assembly.GetEntryAssembly();
                    string errorText = "Additional details:\n\n" +
                        "Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
                        "Program: " + caller.FullName + "\n" +
                        "Program location: " + caller.Location + "\n" +
                        "OS: " + Environment.OSVersion.ToString() + "\n" +
                        "OS Culture: " + System.Globalization.CultureInfo.CurrentCulture.Name + "\n" +
                        "Framework: " + Environment.Version + "\n\n" +
                        "Error:\n" +
                        ex.GetType().ToString() + " (" + ex.Message + ")\n" +
                        ex.StackTrace;
                    // URL encode message
                    url += System.Uri.EscapeDataString(errorText.Trim());

                    // Limit commandline to about 2000 characters (2000 seems to be the limit on my Windows 7 laptop)
                    if(url.Length > 2000)
                    {
                        string clippedMessage = System.Uri.EscapeDataString("\n(Report clipped)");
                        url = url.Substring(0, 2000 - clippedMessage.Length);
                        url += clippedMessage;
                    }

                    // Open URL in browser
                    System.Diagnostics.Process.Start(url);
                }
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
