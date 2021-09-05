using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Endogine;

namespace MusicGameApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string path = Application.ExecutablePath;
            EndogineHub endogine = new EndogineHub(path);
            string[] aAvailableStrategies = StageBase.GetAvailableRenderers();
            //Endogine.MdiParent mdiParent = new MdiParent();
            //SetupWindow wndSetup = new SetupWindow(aAvailableStrategies, mdiParent);
            SetupWindow wndSetup = new SetupWindow(aAvailableStrategies, null);
            wndSetup.ShowDialog();

            Main main = new Main();
            main.Show();

            endogine.Init(main, null, null);
            main.EndogineInitDone();

            MusicGame.Midi.Main game = new MusicGame.Midi.Main();

            while (endogine.MainLoop())
                Application.DoEvents();

        }
    }
}