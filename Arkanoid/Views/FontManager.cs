using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace Arkanoid.Views
{
    public static class FontManager
    {
        private static PrivateFontCollection privateFonts = new PrivateFontCollection();
        public static Font PressStartFont { get; private set; }

        public static void LoadFont()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream fontStream = assembly.GetManifestResourceStream("Arkanoid.Resources.Fonts.PressStart2P-Regular.ttf"))
            {
                if (fontStream == null)
                    throw new Exception("Не удалось найти шрифт в ресурсах.");

                string tempFile = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    fontStream.CopyTo(fileStream);
                }

                privateFonts.AddFontFile(tempFile);
                PressStartFont = new Font(privateFonts.Families[0], 12f);

                try { File.Delete(tempFile); } catch { }
            }
        }
    }
}
