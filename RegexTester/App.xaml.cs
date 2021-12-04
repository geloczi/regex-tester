using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace RegexTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(RegexTester));

        internal static Configuration Config { get; private set; } = new Configuration();

        private static string ConfigPath = Path.Combine(AppDataFolder, "config.json");

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Directory.Exists(AppDataFolder))
                Directory.CreateDirectory(AppDataFolder);

            try
            {
                Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath)) ?? Config;
            }
            catch { }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config));

            base.OnExit(e);
        }
    }
}
