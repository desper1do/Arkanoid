using System;
using System.Windows.Forms;
using Arkanoid.Controllers;
using Arkanoid.Models;
using Arkanoid.Views;

namespace Arkanoid
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FontManager.LoadFont();

            var mainMenuView = new MainMenuView();
            var mainMenuController = new MainMenuController(mainMenuView);
            Application.Run(mainMenuView);
        }
    }
}
