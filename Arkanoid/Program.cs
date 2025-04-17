using System;
using System.Windows.Forms;
using Arkanoid.Controllers;
using Arkanoid.Views;

namespace Arkanoid
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainMenuView = new Views.MainMenuView();
            var mainMenuController = new Controllers.MainMenuController(mainMenuView);
            Application.Run(mainMenuView);
        }
    }
}

