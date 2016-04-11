﻿using System;
using System.Windows.Forms;
using WinYourDesktopLibrary;
using static WinYourDesktopLibrary.Interpreter;
using static System.Reflection.Assembly;

/*
    Program
    Main entry point
*/

namespace WinYourDesktop
{
    static class Program
    {
        static string ProjectVersion
        {
            get
            {
                return
                    GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        static string ProjectName
        {
            get
            {
                return
                    GetExecutingAssembly().GetName().Name;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            int len = args.Length;

            // No arguments
            if (len == 0)
            {
                ShowForm();
                return 0;
            }

            string file = args[len - 1];

            if (len > 1)
            {
                for (int i = 0; i < len; i++)
                {
                    switch (args [i])
                    {
                        case "/debug":
                            ShowForm(file);
                            return 0;
                    }
                }
            }

            ErrorCode err = Run(file);

            if (err != ErrorCode.Success)
            {
                Application.EnableVisualStyles();
                MessageBox.Show($"{err} - {err.GetErrorMessage()} (0x{err:X4})",
                    $"WinYourDesktop - {err}",
                    MessageBoxButtons.OK);
            }

            return err.ToInt();
        }

        static void ShowForm(string pPath = null)
        {
            Application.EnableVisualStyles();

            if (pPath == null)
                Application.Run(new FormMain());
            else
                Application.Run(new FormMain(pPath));
        }
    }
}