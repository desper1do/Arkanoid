using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameProgress
{
    private static readonly string SavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "progress.dat");

    public static int UnlockedLevels { get; private set; } = 1;

    public static void SaveProgress(int unlockedLevels)
    {
        UnlockedLevels = unlockedLevels;
        using (var stream = File.Create(SavePath))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, UnlockedLevels);
        }
    }

    public static void LoadProgress()
    {
        if (File.Exists(SavePath))
        {
            using (var stream = File.OpenRead(SavePath))
            {
                var formatter = new BinaryFormatter();
                UnlockedLevels = (int)formatter.Deserialize(stream);
            }
        }
    }

    public static void ResetProgress()
    {
        UnlockedLevels = 1;
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}