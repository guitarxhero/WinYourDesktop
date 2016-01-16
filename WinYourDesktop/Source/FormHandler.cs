﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WinYourDesktop
{
    partial class FormMain
    {
        #region Properties
        FileInfo CurrentFile;
        NotificationHandler NotificationHandler;
        ResourceManager RM;
        readonly string nl = Environment.NewLine;
        #endregion

        /// <summary>
        /// Relocate the panels, resize <see cref="FormMain"/>,
        /// and prepare language.
        /// </summary>
        /// <remarks>
        /// This is not in the Designer code because VS will 
        /// rewrite anything that was previously written there.
        /// So, this is untouched by VS.
        /// </remarks>
        void PostInitialize()
        {
            SuspendLayout();

            panelDebugger.Location =
                panelEditor.Location =
                panelSettings.Location =
                panelMain.Location =
                new Point(0, msMain.Height);

            AdjustClientSize(panelMain);

            Console.SetOut(new ConStreamWriter(txtRunOutput));

            NotificationHandler = new NotificationHandler(tsmNotifications, ImageListNotification);

            ChangeCulture();

            ResumeLayout();
        }

        #region Language
        /// <summary>
        /// Change to system's local.
        /// </summary>
        void ChangeCulture()
        {
            ChangeCulture(Thread.CurrentThread.CurrentCulture);
        }

        void ChangeCulture(CultureInfo pLanguage)
        {
            ChangeCulture(pLanguage.Name);
        }

        void ChangeCulture(string pLanguage)
        {
            switch (pLanguage)
            {
                case "en-Pirate":
                    RM = new ResourceManager("WinYourDesktop.Culture.en-Pirate",
                             typeof(FormMain).Assembly);
                    break;

                case "fr-FR":
                case "fr-CA":
                    RM = new ResourceManager("WinYourDesktop.Culture.fr-FR",
                             typeof(FormMain).Assembly);
                    break;

                // Default: English
                default:
                    RM = new ResourceManager("WinYourDesktop.Culture.en-US",
                             typeof(FormMain).Assembly);
                    break;
            }

            // === msMain
            // == App
            tsmApplication.Text = RM.GetString("tsmApplication");
            tsmiRestart.Text = RM.GetString("tsmiRestart");
            tsmiQuit.Text = RM.GetString("tsmiQuit");
            // == View
            tsmView.Text = RM.GetString("tsmView");
            tsmiHome.Text = RM.GetString("tsmiHome");
            tsmiEditor.Text = RM.GetString("tsmiEditor");
            tsmiDebugger.Text = RM.GetString("tsmiDebugger");
            tsmiSettings.Text = RM.GetString("tsmiSettings");
            // == Tools
            tsmTools.Text = RM.GetString("tsmTools");
            tsmiCreationWizard.Text = RM.GetString("tsmiCreationWizard");
            // == ?
            tsmiHelp.Text = RM.GetString("tsmiHelp");
            tsmiAbout.Text = RM.GetString("tsmiAbout");

            // === panelMain
            btnRun.Text = RM.GetString("btnRun");
            btnCreate.Text = RM.GetString("btnCreate");
            btnEdit.Text = RM.GetString("btnEdit");
            btnDebug.Text = RM.GetString("btnDebug");

            // === panelDebugger
            btnOpen.Text = RM.GetString("btnOpen");
            btnRunCopy.Text = RM.GetString("btnRunCopy");
            btnRunWithDebugger.Text = RM.GetString("btnRun");

            // === panelSettings
            lblSettingsLanguage.Text = RM.GetString("lblSettingsLanguage");

            // === ssMain
            sslblStatus.Text = RM.GetString("Welcome");
        }
        #endregion

        #region Viewing modes
        /// <summary>
        /// Viewing modes.
        /// </summary>
        enum ViewingMode : byte
        {
            Home,
            Editor,
            Debugger,
            Settings
        }

        void ToggleMode(ViewingMode pNewViewingMode)
        {
            SuspendLayout();

            switch (pNewViewingMode)
            {
                case ViewingMode.Home:
                    tsmiHome.Checked =
                        panelMain.Visible = true;
                    tsmiEditor.Checked =
                        tsmiDebugger.Checked =
                        tsmiSettings.Checked =
                        panelEditor.Visible =
                        panelDebugger.Visible =
                        panelSettings.Visible = false;
                    AdjustClientSize(panelMain);
                    break;

                case ViewingMode.Editor:
                    tsmiEditor.Checked =
                        panelEditor.Visible = true;
                    tsmiHome.Checked =
                        tsmiDebugger.Checked =
                        tsmiSettings.Checked =
                        panelMain.Visible =
                        panelDebugger.Visible =
                        panelSettings.Visible = false;
                    AdjustClientSize(panelEditor);
                    break;

                case ViewingMode.Debugger:
                    tsmiDebugger.Checked =
                        panelDebugger.Visible = true;
                    tsmiHome.Checked =
                        tsmiEditor.Checked =
                        tsmiSettings.Checked =
                        panelMain.Visible =
                        panelEditor.Visible =
                        panelSettings.Visible = false;
                    AdjustClientSize(panelDebugger);
                    break;

                case ViewingMode.Settings:
                    tsmiSettings.Checked =
                        panelSettings.Visible = true;
                    tsmiHome.Checked =
                        tsmiEditor.Checked =
                        tsmiDebugger.Checked =
                        panelMain.Visible =
                        panelEditor.Visible =
                        panelDebugger.Visible = false;
                    AdjustClientSize(panelSettings);
                    break;
            }

            ResumeLayout(true);
        }
        #endregion

        /// <summary>
        /// Adjusts the size of the client.
        /// </summary>
        /// <param name="pPanel">Panel</param>
        void AdjustClientSize(Panel pPanel)
        {
            ClientSize = new Size(pPanel.Width,
                msMain.Height +
                pPanel.Height +
                ssMain.Height
            );
        }

        /// <summary>
        /// Adjusts the size of the client.
        /// </summary>
        /// <param name="pPanelHeight">Panel's height.</param>
        void AdjustClientSize(int pPanelWidth, int pPanelHeight)
        {
            ClientSize = new Size(pPanelWidth,
                msMain.Height +
                pPanelHeight +
                ssMain.Height
            );
        }

        /// <summary>
        /// Set current file for the debugger or edit view.
        /// </summary>
        /// <param name="pPath">File path.</param>
        void SetCurrentFile(string pPath)
        {
            CurrentFile = new FileInfo(pPath);
            lblRunCurrentFile.Text = CurrentFile.Name;
            btnRunWithDebugger.Enabled = true;
        }
    }

    /// <summary>
    /// Console Reader
    /// </summary>
    public class ConStreamWriter : TextWriter
    {
        TextBox t = null;

        public ConStreamWriter(TextBox output)
        {
            t = output;
        }

        public override void Write(char value)
        {
            t.AppendText($"{value}");
        }

        public override void Write(string value)
        {
            t.AppendText(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
