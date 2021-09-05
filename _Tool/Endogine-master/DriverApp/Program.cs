using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Endogine;

namespace DriverApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            EndogineHub endogine = new EndogineHub(Application.ExecutablePath);

            Main main = new Main();
            main.Show();

            endogine.Init(main, null, null);
            main.EndogineInitDone();

            Driver.Game game = new Driver.Game();

            while (endogine.MainLoop())
                Application.DoEvents();

        }
    }
}
